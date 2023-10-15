using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TrabajoENE
{
    public partial class FormRegistroSueldo : Form
    {
        private readonly ConexionBD _conexionBD;
        private string _rut;
        private List<Dictionary<string, object>> afps;
        private List<Dictionary<string, object>> isapres;

        public FormRegistroSueldo(string rut = null)
        {
            InitializeComponent();
            _conexionBD = new ConexionBD();
            CargarAFP();
            CargarIsapres();
            _rut = rut;
            Load += new EventHandler(FormRegistroSueldo_Load);
        }

        private void FormRegistroSueldo_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_rut))
            {
                CargarDatosEmpleado(_rut);
            }
        }

        private void CargarAFP()
        {
            afps = _conexionBD.Leer("AFP");
            foreach (var afp in afps)
            {
                comboBoxAFP.Items.Add(afp["Nombre"]);
            }
        }

        private void CargarIsapres()
        {
            isapres = _conexionBD.Leer("Salud");
            foreach (var isapre in isapres)
            {
                comboBoxIsapres.Items.Add(isapre["Nombre"]);
            }
        }

        private void CargarDatosEmpleado(string rut)
        {
            var empleado = _conexionBD.Leer("Empleados", $"Rut = '{rut}'")[0];
            if (empleado != null)
            {
                textBoxRUT.Text = empleado["Rut"].ToString();
                textBoxNombre.Text = empleado["Nombre"].ToString();
                textBoxValorHora.Text = empleado["ValorHora"].ToString();
                textBoxValorExtra.Text = empleado["ValorExtra"].ToString();

                string afp = empleado["Afp"].ToString();
                string isapre = empleado["Isapre"].ToString();

                // Seleccionar AFP en el ComboBox:
                int indexAFP = comboBoxAFP.FindStringExact(afp);
                if (indexAFP != -1)
                {
                    comboBoxAFP.SelectedIndex = indexAFP;
                }

                // Seleccionar Isapre en el ComboBox:
                int indexIsapre = comboBoxIsapres.FindStringExact(isapre);
                if (indexIsapre != -1)
                {
                    comboBoxIsapres.SelectedIndex = indexIsapre;
                }
            }

        }

        private void buttonCalcular_Click_1(object sender, EventArgs e)
        {
            // Validar los campos primero
            if (!double.TryParse(textBoxHorasTrabajadas.Text, out double horasTrabajadas) ||
                !double.TryParse(textBoxHorasExtras.Text, out double horasExtra) ||
                !double.TryParse(textBoxValorHora.Text, out double valorHora) ||
                !double.TryParse(textBoxValorExtra.Text, out double valorExtra))
            {
                MessageBox.Show("Por favor, ingrese valores válidos para las horas trabajadas, horas extras, valor por hora y valor extra.");
                return;
            }

            // Calcular sueldo bruto
            double sueldoBruto = CalculadoraSueldos.CalcularSueldoBruto(horasTrabajadas, valorHora, horasExtra, valorExtra);

            // Mostrar el sueldo bruto en textBoxSueldoBruto
            textBoxSueldoBruto.Text = sueldoBruto.ToString();

            // Calcular sueldo liquido
            int selectedIndexAFP = comboBoxAFP.SelectedIndex;
            int selectedIndexIsapres = comboBoxIsapres.SelectedIndex;
            if (selectedIndexAFP != -1 && afps.Count > selectedIndexAFP &&
                selectedIndexIsapres != -1 && isapres.Count > selectedIndexIsapres)
            {
                double tasaAFP = Convert.ToDouble(afps[selectedIndexAFP]["Tasa"]);
                double tasaIsapres = Convert.ToDouble(isapres[selectedIndexIsapres]["Tasa"]);

                double sueldoLiquido = CalculadoraSueldos.CalcularSueldoLiquido(sueldoBruto, tasaAFP, tasaIsapres);

                // Mostrar el sueldo liquido en textBoxSueldoLiquido
                textBoxSueldoLiquido.Text = sueldoLiquido.ToString();
            }
        }

        private void buttonLimpiar_Click_1(object sender, EventArgs e)
        {
            // Limpiar todos los campos
            textBoxHorasTrabajadas.Text = "";
            textBoxHorasExtras.Text = "";
            textBoxSueldoBruto.Text = "";
            textBoxSueldoLiquido.Text = "";
        }

        private void buttonGuardar_Click_1(object sender, EventArgs e)
        {
            string afpSeleccionada = comboBoxAFP.SelectedItem.ToString();
            string isapreSeleccionada = comboBoxIsapres.SelectedItem.ToString();

            // Validación básica de entrada
            if (string.IsNullOrWhiteSpace(textBoxRUT.Text) ||
                string.IsNullOrWhiteSpace(textBoxNombre.Text) ||
                string.IsNullOrWhiteSpace(textBoxValorHora.Text) ||
                string.IsNullOrWhiteSpace(textBoxValorExtra.Text) ||
                string.IsNullOrWhiteSpace(afpSeleccionada) ||
                string.IsNullOrWhiteSpace(isapreSeleccionada)
                )
            {
                MessageBox.Show("Por favor, complete todos los campos.");
                return;
            }
           
            // Crear un nuevo diccionario para almacenar los datos del empleado
            var datosEmpleado = new Dictionary<string, object>
            {
                { "Rut", textBoxRUT.Text },
                { "Nombre", textBoxNombre.Text },
                { "ValorHora", textBoxValorHora.Text },
                { "ValorExtra", textBoxValorExtra.Text },
                { "Afp", afpSeleccionada },
                { "Isapre", isapreSeleccionada }
            };

            // Si se está editando un empleado existente, actualizarlo en la base de datos
            if (_rut != null)
            {
                _conexionBD.Actualizar("Empleados", datosEmpleado, $"Rut = '{textBoxRUT.Text}'");
                MessageBox.Show("Empleado actualizado exitosamente.");
            }
            else  // Si se está agregando un nuevo empleado, insertarlo en la base de datos
            {
                _conexionBD.Insertar("Empleados", datosEmpleado);
                MessageBox.Show("Empleado agregado exitosamente.");
            }

            // Cerrar el formulario
            Close();
        }
    }
}
