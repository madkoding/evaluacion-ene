using System;

namespace TrabajoENE
{
    public class CalculadoraSueldos
    {
        public static double CalcularSueldoBruto(double horasTrabajadas, double valorHora, double horasExtra, double valorExtra)
        {
            try
            {
                if (horasTrabajadas < 0 || valorHora < 0 || horasExtra < 0 || valorExtra < 0)
                    throw new ArgumentException("Los valores no pueden ser negativos.");

                return (horasTrabajadas * valorHora) + (horasExtra * valorExtra);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al calcular sueldo bruto: " + ex.Message, ex);
            }
        }

        public static double CalcularDescuentoAFP(double sueldoBruto, double tasaAFP)
        {
            try
            {
                if (sueldoBruto < 0 || tasaAFP < 0)
                    throw new ArgumentException("El sueldo bruto y la tasa AFP no pueden ser negativos.");

                return sueldoBruto * (tasaAFP / 100);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al calcular descuento AFP: " + ex.Message, ex);
            }
        }

        public static double CalcularDescuentoSalud(double sueldoBruto, double tasaSalud)
        {
            try
            {
                if (sueldoBruto < 0 || tasaSalud < 0)
                    throw new ArgumentException("El sueldo bruto y la tasa de salud no pueden ser negativos.");

                return sueldoBruto * (tasaSalud / 100);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al calcular descuento de salud: " + ex.Message, ex);
            }
        }

        public static double CalcularSueldoLiquido(double sueldoBruto, double descuentoAFP, double descuentoSalud)
        {
            try
            {
                if (sueldoBruto < 0 || descuentoAFP < 0 || descuentoSalud < 0)
                    throw new ArgumentException("Los valores no pueden ser negativos.");

                return sueldoBruto - (descuentoAFP + descuentoSalud);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al calcular sueldo líquido: " + ex.Message, ex);
            }
        }
    }
}
