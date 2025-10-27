using CadeteriaApi.Data;
using CadeteriaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CadeteriaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CadeteriaController : ControllerBase
    {
        // GET: api/cadeteria/pedidos
        [HttpGet("pedidos")]
        public ActionResult<IEnumerable<Pedido>> GetPedidos()
            => Ok(Store.Cadeteria.ListadoPedidos);

        // GET: api/cadeteria/cadetes
        [HttpGet("cadetes")]
        public ActionResult<IEnumerable<Cadete>> GetCadetes()
            => Ok(Store.Cadetes);

        // GET: api/cadeteria/informe
        [HttpGet("informe")]
        public ActionResult<Informe> GetInforme()
        {
            var pedidos = Store.Cadeteria.ListadoPedidos;
            var informe = new Informe
            {
                CantidadPedidos = pedidos.Count,
                CantidadEntregados = pedidos.Count(p => p.Estado == EstadoPedido.Entregado),
                CantidadPendientes = pedidos.Count(p => p.Estado == EstadoPedido.Pendiente),
                CantidadCancelados = pedidos.Count(p => p.Estado == EstadoPedido.Cancelado),
                Jornales = Store.Cadetes
                    .Select(c => new JornalCadete
                    {
                        IdCadete = c.Id,
                        Nombre = c.Nombre,
                        Monto = Store.Cadeteria.JornalACobrar(c.Id)
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
            var pedido = Store.CrearPedido(dto.Observacion);
            return CreatedAtAction(nameof(GetPedidos), new { id = pedido.Id }, pedido);
        }

        // PUT: api/cadeteria/asignar?idPedido=1&idCadete=2
        [HttpPut("asignar")]
        public IActionResult AsignarPedido([FromQuery] int idPedido, [FromQuery] int idCadete)
        {
            var pedido = Store.GetPedido(idPedido);
            var cadete = Store.GetCadete(idCadete);
            if (pedido == null || cadete == null) return NotFound();
            Store.Cadeteria.AsignarCadeteAPedido(idCadete, idPedido);
            return NoContent();
        }

        // PUT: api/cadeteria/estado?idPedido=1&nuevoEstado=1
        [HttpPut("estado")]
        public IActionResult CambiarEstadoPedido([FromQuery] int idPedido, [FromQuery] EstadoPedido nuevoEstado)
        {
            var pedido = Store.GetPedido(idPedido);
            if (pedido == null) return NotFound();
            pedido.Estado = nuevoEstado;
            return NoContent();
        }

        // PUT: api/cadeteria/cambiar-cadete?idPedido=1&idNuevoCadete=3
        [HttpPut("cambiar-cadete")]
        public IActionResult CambiarCadetePedido([FromQuery] int idPedido, [FromQuery] int idNuevoCadete)
        {
            var pedido = Store.GetPedido(idPedido);
            var cadete = Store.GetCadete(idNuevoCadete);
            if (pedido == null || cadete == null) return NotFound();
            pedido.CadeteAsignado = cadete;
            return NoContent();
        }

        public record CrearPedidoDto(string Observacion);
    }
}
