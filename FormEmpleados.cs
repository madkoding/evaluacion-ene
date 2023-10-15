using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace TrabajoENE
{
    public partial class FormEmpleados : Form
    {
        private readonly ConexionBD _conexionBD;

        public FormEmpleados()
        {
            InitializeComponent();
            _conexionBD = new ConexionBD();
            CargarDatos();
        }

        private void CargarDatos()
        {
            dataGridView1.AutoGenerateColumns = true;
            List<Dictionary<string, object>> datosEmpleados = _conexionBD.Leer("Empleados");
            DataTable empleadosDataTable = _conexionBD.ConvertirADataTable(datosEmpleados);
            dataGridView1.DataSource = empleadosDataTable;
        }

        private void buttonModificar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var rutEmpleado = dataGridView1.SelectedRows[0].Cells["Rut"].Value.ToString();
                var formEditar = new FormRegistroSueldo(rutEmpleado);
                formEditar.FormClosed += (s, args) => CargarDatos();
                formEditar.Show();
            }
        }

        private void buttonEliminar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var rutEmpleado = dataGridView1.SelectedRows[0].Cells["Rut"].Value.ToString();
                _conexionBD.Eliminar("Empleados", $"Rut = '{rutEmpleado}'");
                CargarDatos();
            }
        }

        private void buttonAgregar_Click_1(object sender, EventArgs e)
        {
            var formEditar = new FormRegistroSueldo();
            formEditar.FormClosed += (s, args) => CargarDatos();
            formEditar.Show();
        }
    }
}
