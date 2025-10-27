namespace CadeteriaApi.Models
{
    public class Cadete
    {
        private int id;
        private string nombre;
        private string direccion;
        private string telefono;

        public int Id => id;
        public string Nombre => nombre;
        public string Direccion => direccion;
        public string Telefono => telefono;

        public Cadete(int id, string nombre, string direccion, string telefono)
        {
            this.id = id;
            this.nombre = nombre;
            this.direccion = direccion;
            this.telefono = telefono;
        }
    }
}
