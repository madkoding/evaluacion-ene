namespace TrabajoENE
{
    public static class Validador
    {
        public static string ValidarCredenciales(string nombreUsuario, string contraseña)
        {
            // Valida si el campo usuario esta vacio o con espacios en blanco
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return "El nombre de usuario no puede estar vacío.";
            }

            // Valida si el campo contraseña esta vacio o con espacios en blanco
            if (string.IsNullOrWhiteSpace(contraseña))
            {
                return "La contraseña no puede estar vacía.";
            }

            // Valida que el largo de la contraseña sea de minimo 6 caracteres
            if (contraseña.Length < 5)
            {
                return "La contraseña debe tener al menos 5 caracteres.";
            }

            return string.Empty;  // Retorna vacía si todas las validaciones son exitosas
        }
    }
}
