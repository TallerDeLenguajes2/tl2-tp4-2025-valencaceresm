namespace CadeteriaApi.Models
{
    public class Informe
    {
        public int CantidadPedidos { get; set; }
        public int CantidadEntregados { get; set; }
        public int CantidadPendientes { get; set; }
        public int CantidadCancelados { get; set; }
        public List<JornalCadete> Jornales { get; set; } = new();
    }

    public class JornalCadete
    {
        public int IdCadete { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Monto { get; set; }
    }
}
