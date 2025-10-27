using System.Text.Json;
using CadeteriaApi.Models;

namespace CadeteriaApi.ConAccesoADatos
{
    public class AccesoADatosCadeteria
    {
        private record CadeteriaJson(string Nombre, string Telefono);

        public Cadeteria Obtener()
        {
            if (!File.Exists(FilePaths.CadeteriaPath))
            {
                var defecto = new CadeteriaJson("Cadetería Express", "111-222-333");
                File.WriteAllText(FilePaths.CadeteriaPath, JsonSerializer.Serialize(defecto, FilePaths.JsonOptions));
            }

            var json = JsonSerializer.Deserialize<CadeteriaJson>(File.ReadAllText(FilePaths.CadeteriaPath))!;
            return new Cadeteria(json.Nombre, json.Telefono, new List<Cadete>());
        }
    }
}
