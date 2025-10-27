using System.Text.Json;
using CadeteriaApi.Models;

namespace CadeteriaApi.ConAccesoADatos
{
    public class AccesoADatosCadetes
    {
        private record CadeteJson(int Id, string Nombre, string Direccion, string Telefono);

        public List<Cadete> Obtener()
        {
            if (!File.Exists(FilePaths.CadetesPath))
            {
                var seed = new List<CadeteJson>
                {
                    new(1, "Juan Pérez", "Calle 123", "123456789"),
                    new(2, "María García", "Avenida 456", "987654321"),
                    new(3, "Carlos López", "Boulevard 789", "555666777"),
                };
                File.WriteAllText(FilePaths.CadetesPath, JsonSerializer.Serialize(seed, FilePaths.JsonOptions));
            }

            var data = JsonSerializer.Deserialize<List<CadeteJson>>(File.ReadAllText(FilePaths.CadetesPath)) ?? new();
            return data.Select(c => new Cadete(c.Id, c.Nombre, c.Direccion, c.Telefono)).ToList();
        }
    }
}
