using System.Linq;
using CadeteriaApi.Models;
using Microsoft.AspNetCore.Mvc;
using CadeteriaApi.ConAccesoADatos;

namespace CadeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CadeteriaController : ControllerBase
    {
        private Cadeteria cadeteria;
        private AccesoADatosCadeteria ADCadeteria;
        private AccesoADatosCadetes ADCadetes;
        private AccesoADatosPedidos ADPedidos;

        public CadeteriaController()
        {
            ADCadeteria = new AccesoADatosCadeteria();
            ADCadetes = new AccesoADatosCadetes();
            ADPedidos = new AccesoADatosPedidos();

            cadeteria = ADCadeteria.Obtener();
            cadeteria.AgregarListaCadetes(ADCadetes.Obtener());
            cadeteria.AgregarListaPedidos(ADPedidos.Obtener());
            // Linkear asignaciones por id guardadas
            foreach (var p in cadeteria.ListadoPedidos)
            {
                if (p.CadeteAsignado == null && _cadeteIds.TryGetValue(p.Id, out var cadId))
                {
                    var cad = cadeteria.ListadoCadetes.FirstOrDefault(c => c.Id == cadId);
                    if (cad != null) p.CadeteAsignado = cad;
                }
            }
        }

        // GET: api/cadeteria/pedidos
        [HttpGet("pedidos")]
        public ActionResult<IEnumerable<Pedido>> GetPedidos()
            => Ok(cadeteria.ListadoPedidos);

        // GET: api/cadeteria/cadetes
        [HttpGet("cadetes")]
        public ActionResult<IEnumerable<Cadete>> GetCadetes()
            => Ok(cadeteria.ListadoCadetes);

        // GET: api/cadeteria/informe
        [HttpGet("informe")]
        public ActionResult<Informe> GetInforme()
        {
            var pedidos = cadeteria.ListadoPedidos;
            var informe = new Informe
            {
                CantidadPedidos = pedidos.Count,
                CantidadEntregados = pedidos.Count(p => p.Estado == EstadoPedido.Entregado),
                CantidadPendientes = pedidos.Count(p => p.Estado == EstadoPedido.Pendiente),
                CantidadCancelados = pedidos.Count(p => p.Estado == EstadoPedido.Cancelado),
                Jornales = cadeteria.ListadoCadetes
                    .Select(c => new JornalCadete
                    {
                        IdCadete = c.Id,
                        Nombre = c.Nombre,
                        Monto = cadeteria.JornalACobrar(c.Id)
                    })
                    .ToList()
            };
            return Ok(informe);
        }

        // POST: api/cadeteria/pedidos
        [HttpPost("pedidos")]
        public ActionResult<Pedido> AgregarPedido([FromBody] CrearPedidoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Observacion)) return BadRequest("Observación requerida");
            int nextId = cadeteria.ListadoPedidos.Count == 0 ? 1 : cadeteria.ListadoPedidos.Max(p => p.Id) + 1;
            var pedido = new Pedido(nextId, dto.Observacion);
            cadeteria.AgregarPedido(pedido);
            ADPedidos.Guardar(cadeteria.ListadoPedidos);
            return CreatedAtAction(nameof(GetPedidos), new { id = pedido.Id }, pedido);
        }

        // PUT: api/cadeteria/asignar?idPedido=1&idCadete=2
        [HttpPut("asignar")]
        public IActionResult AsignarPedido([FromQuery] int idPedido, [FromQuery] int idCadete)
        {
            var pedido = cadeteria.ListadoPedidos.FirstOrDefault(p => p.Id == idPedido);
            var cadete = cadeteria.ListadoCadetes.FirstOrDefault(c => c.Id == idCadete);
            if (pedido == null || cadete == null) return NotFound();
            cadeteria.AsignarCadeteAPedido(idCadete, idPedido);
            ADPedidos.Guardar(cadeteria.ListadoPedidos);
            return NoContent();
        }

        // PUT: api/cadeteria/estado?idPedido=1&nuevoEstado=1
        [HttpPut("estado")]
        public IActionResult CambiarEstadoPedido([FromQuery] int idPedido, [FromQuery] EstadoPedido nuevoEstado)
        {
            var pedido = cadeteria.ListadoPedidos.FirstOrDefault(p => p.Id == idPedido);
            if (pedido == null) return NotFound();
            pedido.Estado = nuevoEstado;
            ADPedidos.Guardar(cadeteria.ListadoPedidos);
            return NoContent();
        }

        // PUT: api/cadeteria/cambiar-cadete?idPedido=1&idNuevoCadete=3
        [HttpPut("cambiar-cadete")]
        public IActionResult CambiarCadetePedido([FromQuery] int idPedido, [FromQuery] int idNuevoCadete)
        {
            var pedido = cadeteria.ListadoPedidos.FirstOrDefault(p => p.Id == idPedido);
            var cadete = cadeteria.ListadoCadetes.FirstOrDefault(c => c.Id == idNuevoCadete);
            if (pedido == null || cadete == null) return NotFound();
            pedido.CadeteAsignado = cadete;
            ADPedidos.Guardar(cadeteria.ListadoPedidos);
            return NoContent();
        }

        public record CrearPedidoDto(string Observacion);

        // Mapeo auxiliar de asignaciones persistidas (cargado por AccesoADatosPedidos)
        private static readonly Dictionary<int,int> _cadeteIds = AccesoADatosPedidos.CadeteIdsPorPedido;
    }
}
