using System.Text.Json;
using CadeteriaApi.Models;

namespace CadeteriaApi.ConAccesoADatos
{
    public class AccesoADatosPedidos
    {
        internal record PedidoJson(int Id, string Observacion, string Estado, int? CadeteId);

        public static Dictionary<int,int> CadeteIdsPorPedido { get; } = new();

        public List<Pedido> Obtener()
        {
            if (!File.Exists(FilePaths.PedidosPath))
            {
                File.WriteAllText(FilePaths.PedidosPath, JsonSerializer.Serialize(new List<PedidoJson>(), FilePaths.JsonOptions));
            }

            CadeteIdsPorPedido.Clear();
            var data = JsonSerializer.Deserialize<List<PedidoJson>>(File.ReadAllText(FilePaths.PedidosPath)) ?? new();
            var pedidos = new List<Pedido>();
            foreach (var pj in data)
            {
                var p = new Pedido(pj.Id, pj.Observacion);
                if (Enum.TryParse<EstadoPedido>(pj.Estado, out var est)) p.Estado = est; else p.Estado = EstadoPedido.Pendiente;
                if (pj.CadeteId.HasValue) CadeteIdsPorPedido[p.Id] = pj.CadeteId.Value;
                pedidos.Add(p);
            }
            return pedidos;
        }

        public void Guardar(List<Pedido> pedidos)
        {
            var list = pedidos.Select(p => new PedidoJson(
                p.Id,
                p.Observacion,
                p.Estado.ToString(),
                p.CadeteAsignado?.Id
            )).ToList();
            File.WriteAllText(FilePaths.PedidosPath, JsonSerializer.Serialize(list, FilePaths.JsonOptions));
        }
    }
}
