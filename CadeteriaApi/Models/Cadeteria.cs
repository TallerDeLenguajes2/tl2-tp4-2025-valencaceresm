using System.Collections.Generic;
using System.Linq;

namespace CadeteriaApi.Models
{
    public class Cadeteria
    {
        private string nombre;
        private string telefono;
        private List<Cadete> listadoCadetes;
        private List<Pedido> listadoPedidos;

        public string Nombre { get => nombre; }
        public string Telefono { get => telefono; }
        public List<Cadete> ListadoCadetes { get => listadoCadetes; }
        public List<Pedido> ListadoPedidos { get => listadoPedidos; }

        public Cadeteria(string nombre, string telefono, List<Cadete> cadetes)
        {
            this.nombre = nombre;
            this.telefono = telefono;
            this.listadoCadetes = cadetes;
            this.listadoPedidos = new List<Pedido>();
        }

        public void AgregarPedido(Pedido pedido)
        {
            listadoPedidos.Add(pedido);
        }

        public void AsignarCadeteAPedido(int idCadete, int idPedido)
        {
            Cadete cadete = listadoCadetes.FirstOrDefault(c => c.Id == idCadete);
            Pedido pedido = listadoPedidos.FirstOrDefault(p => p.Id == idPedido);
            if (cadete != null && pedido != null)
            {
                pedido.CadeteAsignado = cadete;
            }
        }

        public double JornalACobrar(int idCadete)
        {
            var pedidosCadete = listadoPedidos.Where(p => p.CadeteAsignado != null && p.CadeteAsignado.Id == idCadete && p.Estado == EstadoPedido.Entregado);
            return pedidosCadete.Count() * 500;
        }

        public string MostrarNombreCadeteria()
        {
            return this.Nombre;
        }
    }
}
