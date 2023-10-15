using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace TrabajoENE
{
    public class ConexionBD
    {
        // Especifica hacia donde debe conectar
        // A modo de demostracion utiliza una db que solo existirá en el momento de ejecucion
        // Es mucho mas simple de manejar una DB SQLite en un archivo de texto que montar
        // todo un servidor SQLServer solo para una demostración
        private readonly string _connectionString = "Data Source=TrabajoENE.db;Version=3;";

        // Devuelve la conexion actual a la base de datos
        public SQLiteConnection ObtenerConexion()
        {
            return new SQLiteConnection(_connectionString);
        }

        // Al conectar, verifica si la base de datos tiene las tablas creadas,
        // de no ser asi, la crea con sus parametros
        public void VerificarYCrearTablas()
        {
            string query = @"
                DROP TABLE IF EXISTS Usuarios;
                DROP TABLE IF EXISTS Empleados;
                DROP TABLE IF EXISTS AFP;
                DROP TABLE IF EXISTS Salud;

                -- Crear la tabla Usuarios
                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    NombreUsuario TEXT UNIQUE NOT NULL,
                    Contraseña TEXT NOT NULL,
                    IntentosFallidos INTEGER NOT NULL DEFAULT 0,
                    CuentaBloqueada BOOLEAN NOT NULL DEFAULT 0
                );

                -- Crear la tabla Empleados
                CREATE TABLE IF NOT EXISTS Empleados (
                    Rut TEXT PRIMARY KEY,
                    Nombre TEXT,
                    ValorHora REAL,
                    ValorExtra REAL,
                    Afp TEXT,
                    Isapre TEXT
                );

                -- Insertar datos en la tabla Empleados
                INSERT INTO Empleados (Rut, Nombre, ValorHora, ValorExtra, Afp, Isapre)
                VALUES 
                ('12345678-9', 'Juan Perez', 5000, 7000, 'CUPRUM','FONASA'),
                ('98765432-1', 'Maria Rodriguez', 5000, 7000, 'PROVIDA','CONSALUD');

                -- Crear la tabla AFP
                CREATE TABLE IF NOT EXISTS AFP (
                    Nombre TEXT PRIMARY KEY,
                    Tasa REAL
                );

                -- Insertar datos en la tabla AFP
                INSERT INTO AFP (Nombre, Tasa)
                VALUES 
                ('CUPRUM', 7),
                ('MODELO', 9),
                ('CAPITAL', 12),
                ('PROVIDA', 13);

                -- Crear la tabla Salud
                CREATE TABLE IF NOT EXISTS Salud (
                    Nombre TEXT PRIMARY KEY,
                    Tasa REAL
                );

                -- Insertar datos en la tabla Salud
                INSERT INTO Salud (Nombre, Tasa)
                VALUES 
                ('FONASA', 12),
                ('CONSALUD', 13),
                ('MASVIDA', 14),
                ('BANMEDICA', 15);
            ";

            using (SQLiteConnection connection = ObtenerConexion())
            {
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            InsertarUsuario("admin", "admin");
        }

        public void InsertarUsuario(string nombreUsuario, string contraseña)
        {
            var valoresInsertar = new Dictionary<string, object>
            {
                { "NombreUsuario", nombreUsuario },
                { "Contraseña", BCrypt.Net.BCrypt.HashPassword(contraseña) },
            };

            Insertar("Usuarios", valoresInsertar);
        }

        public void Insertar(string tabla, Dictionary<string, object> valores)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                string parametros = "";
                foreach (var key in valores.Keys)
                {
                    if (parametros.Length > 0)
                    {
                        parametros += ", ";
                    }
                    parametros += "@" + key;
                }

                var columnas = string.Join(", ", valores.Keys);
                var query = $"INSERT INTO {tabla} ({columnas}) VALUES ({parametros})";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    foreach (var valor in valores)
                    {
                        cmd.Parameters.AddWithValue("@" + valor.Key, valor.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void Actualizar(string tabla, Dictionary<string, object> valores, string condicion)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                string setString = "";
                foreach (var key in valores.Keys)
                {
                    if (setString.Length > 0)
                    {
                        setString += ", ";
                    }
                    setString += $"{key} = @{key}";
                }

                var query = $"UPDATE {tabla} SET {setString} WHERE {condicion}";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    foreach (var valor in valores)
                    {
                        cmd.Parameters.AddWithValue("@" + valor.Key, valor.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void Eliminar(string tabla, string condicion)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                var query = $"DELETE FROM {tabla} WHERE {condicion}";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public List<Dictionary<string, object>> Leer(string tabla, string condicion = null)
        {
            var resultados = new List<Dictionary<string, object>>();

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                var query = $"SELECT * FROM {tabla} {(condicion != null ? $"WHERE {condicion}" : string.Empty)}";

                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fila = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fila[reader.GetName(i)] = reader.GetValue(i);
                        }
                        resultados.Add(fila);
                    }
                }

                conn.Close();
            }

            return resultados;
        }

        public DataTable ConvertirADataTable(List<Dictionary<string, object>> datos)
        {
            var dt = new DataTable();

            if (datos.Count > 0)
            {
                foreach (var key in datos[0].Keys)
                {
                    dt.Columns.Add(key);
                }

                foreach (var fila in datos)
                {
                    var dr = dt.NewRow();
                    foreach (var key in fila.Keys)
                    {
                        dr[key] = fila[key];
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

    }
}
