namespace TrabajoENE
{
    public class Empleado
    {
        // Propiedades
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public double ValorHora { get; set; }
        public double ValorExtra { get; set; }
        public string Afp { get; set; }
        public string Isapre { get; set; }

        // Constructor
        public Empleado(string rut, string nombre, double valorHora, double valorExtra, string afp, string isapre)
        {
            Rut = rut;
            Nombre = nombre;
            ValorHora = valorHora;
            ValorExtra = valorExtra;
            Afp = afp;
            Isapre = isapre;
        }

        // Métodos
        public double CalcularSueldoBruto(double horasTrabajadas, double horasExtra)
        {
            // Llama al método CalcularSueldoBruto de la clase CalculadoraSueldos
            return CalculadoraSueldos.CalcularSueldoBruto(horasTrabajadas, ValorHora, horasExtra, ValorExtra);
        }
    }

}
