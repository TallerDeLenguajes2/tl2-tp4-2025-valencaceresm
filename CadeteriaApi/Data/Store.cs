using CadeteriaApi.Models;

namespace CadeteriaApi.Data
{
    public static class Store
    {
        private static readonly object _lock = new();

        static Store()
        {
            // Seed
            Cadetes = new List<Cadete>
            {
                new Cadete(1, "Juan Pérez", "Calle 123", "123456789"),
                new Cadete(2, "María García", "Avenida 456", "987654321"),
                new Cadete(3, "Carlos López", "Boulevard 789", "555666777"),
            };
            Cadeteria = new Cadeteria("Cadetería Express", "111-222-333", Cadetes);
            _nextPedidoId = 1;
        }

        private static int _nextPedidoId;
        public static Cadeteria Cadeteria { get; }
        public static List<Cadete> Cadetes { get; }

        public static Pedido CrearPedido(string observacion)
        {
            lock (_lock)
            {
                var pedido = new Pedido(_nextPedidoId++, observacion);
                Cadeteria.AgregarPedido(pedido);
                return pedido;
            }
        }

        public static Pedido? GetPedido(int id) => Cadeteria.ListadoPedidos.FirstOrDefault(p => p.Id == id);
        public static Cadete? GetCadete(int id) => Cadetes.FirstOrDefault(c => c.Id == id);
    }
}
