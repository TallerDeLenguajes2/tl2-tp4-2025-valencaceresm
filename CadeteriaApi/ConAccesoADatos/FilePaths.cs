using System.Text.Json;
using CadeteriaApi.Models;

namespace CadeteriaApi.ConAccesoADatos
{
    internal static class FilePaths
    {
        public static string DataDir
        {
            get
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "Data");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string CadeteriaPath => Path.Combine(DataDir, "Cadeteria.json");
        public static string CadetesPath => Path.Combine(DataDir, "Cadetes.json");
        public static string PedidosPath => Path.Combine(DataDir, "Pedidos.json");

        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };
    }
}
