using System;
using System.Data.SQLite;

namespace TrabajoENE
{
    public enum ResultadoAutenticacion
    {
        Exito,
        UsuarioNoEncontrado,
        ContraseñaIncorrecta,
        CuentaBloqueada,
        Error
    }
    public class Autenticacion
    {
        private readonly ConexionBD _conexionBD;

        public Autenticacion(ConexionBD conexionBD)
        {
            _conexionBD = conexionBD;
        }

        public ResultadoAutenticacion AutenticarUsuario(string nombreUsuario, string contraseña)
        {
            string query = @"
                SELECT 
                    Contraseña, 
                    CuentaBloqueada 
                FROM 
                    Usuarios 
                WHERE 
                    NombreUsuario = @NombreUsuario";

            try
            {
                // Conecta con la base de datos
                using (SQLiteConnection connection = _conexionBD.ObtenerConexion())
                {
                    // Ejecuta la query para buscar usuarios
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                        connection.Open();

                        // Obtiene las columnas
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Verifica si trajo columnas
                            if (reader.Read())
                            {
                                // Verifica si la cuenta esta bloqueada
                                bool cuentaBloqueada = reader.GetBoolean(reader.GetOrdinal("CuentaBloqueada"));
                                if (cuentaBloqueada)
                                {
                                    return ResultadoAutenticacion.CuentaBloqueada;
                                }

                                // Verifica con el hash encriptado la contraseña
                                string contraseñaHash = reader.GetString(reader.GetOrdinal("Contraseña"));
                                if (BCrypt.Net.BCrypt.Verify(contraseña, contraseñaHash))
                                {
                                    return ResultadoAutenticacion.Exito;
                                }
                                else
                                {
                                    return ResultadoAutenticacion.ContraseñaIncorrecta;
                                }
                            }
                            else
                            {
                                return ResultadoAutenticacion.UsuarioNoEncontrado;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return ResultadoAutenticacion.Error;
            }
        }
    }
}

