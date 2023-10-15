using System;
using System.Windows.Forms;

namespace TrabajoENE
{
    public partial class FormLogin : Form
    {
        private readonly ConexionBD _conexionBD;
        private readonly Autenticacion _autenticacion;

        // Inicializa el formulario y las clases
        public FormLogin()
        {
            InitializeComponent();
            _conexionBD = new ConexionBD();
            _conexionBD.VerificarYCrearTablas();
            _autenticacion = new Autenticacion(_conexionBD);
        }

        // Accion al presionar el boton de login
        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string nombreUsuario = textBoxUsuario.Text;
            string contraseña = textBoxContraseña.Text;

            string mensajeError = Validador.ValidarCredenciales(nombreUsuario, contraseña);
            if (!string.IsNullOrEmpty(mensajeError))
            {
                MessageBox.Show(mensajeError);
                return;
            }

            ResultadoAutenticacion resultado = _autenticacion.AutenticarUsuario(nombreUsuario, contraseña);
            switch (resultado)
            {
                case ResultadoAutenticacion.Exito:
                    this.Hide();
                    var formEmpleados = new FormEmpleados();
                    formEmpleados.Show();
                    break;
                case ResultadoAutenticacion.UsuarioNoEncontrado:
                    MessageBox.Show("Usuario no encontrado");
                    break;
                case ResultadoAutenticacion.ContraseñaIncorrecta:
                    MessageBox.Show("Contraseña incorrecta");
                    break;
                case ResultadoAutenticacion.CuentaBloqueada:
                    MessageBox.Show("Cuenta bloqueada");
                    break;
                case ResultadoAutenticacion.Error:
                    MessageBox.Show("Ocurrió un error durante la autenticación");
                    break;
            }
        }

        // Accion al presionar el boton de registro
        private void ButtonRegister_Click(object sender, EventArgs e)
        {
            //string nombreUsuario = textBoxUsuario.Text;
            //string contraseña = textBoxContraseña.Text;

            //string mensajeError = Validador.ValidarCredenciales(nombreUsuario, contraseña);
            //if (!string.IsNullOrEmpty(mensajeError))
            //{
            //    MessageBox.Show(mensajeError);
            //    return;
            //}

            //_conexionBD.InsertarUsuario(nombreUsuario, contraseña);

            //MessageBox.Show("Usuario registrado exitosamente");
            MessageBox.Show("Registro deshabilitado");
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
