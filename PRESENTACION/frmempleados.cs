using MODELO;
using NEGOCIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRESENTACION
{
    public partial class frmempleados : Form
    {
        private readonly EmpleadosNegocio empleadosNegocio = new EmpleadosNegocio();
        private EmpleadosEntidad empleadoSeleccionado = null;
        private int? filaActiva = null;

        public frmempleados()
        {
            InitializeComponent();
        }

        private void frmempleados_Load(object sender, EventArgs e)
        {
            MtdConsultarEmpleados();
            MtdRenombrarColumnas();
            MtdEstadoFilaSeleccionada(false);
        }

        private void MtdRenombrarColumnas()
        {
            CambiarTitulo("CodigoEmpleado", "ID");
            CambiarTitulo("NumeroDpi", "DPI (Identificación)");
            CambiarTitulo("FechaContratacion", "F. Contratación");
            CambiarTitulo("Salario", "Sueldo Base");
        }

        private void CambiarTitulo(string nombreColumna, string titulo)
        {
            if (dgvEmpleados.Columns.Contains(nombreColumna))
                dgvEmpleados.Columns[nombreColumna].HeaderText = titulo;
        }

        private void dgvEmpleados_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Formato para Género
            if (dgvEmpleados.Columns[e.ColumnIndex].Name == "Genero" && e.Value != null)
            {
                switch (e.Value.ToString())
                {
                    case "M": e.Value = "Masculino"; break;
                    case "F": e.Value = "Femenino"; break;
                    default: e.Value = "Otro"; break;
                }
            }

            // Formato para Salario (Moneda)
            if (dgvEmpleados.Columns[e.ColumnIndex].Name == "Salario" && e.Value != null)
            {
                e.Value = string.Format("{0:C2}", e.Value);
            }
        }

        // Consultar datos de tabla Empleados y mostrar en DataGridView
        private void MtdConsultarEmpleados()
        {
            try
            {
                var lista = empleadosNegocio.MtdConsultarEmpleados();
                dgvEmpleados.DataSource = lista;

                dgvEmpleados.ClearSelection();
                dgvEmpleados.CurrentCell = null;
                filaActiva = null;
                MtdContarTotalRegistros();
                MtdRenombrarColumnas();
            }
            catch (Exception ex)
            {
                // Mostrar el error detallado en caso de fallos en la conexión o en Oracle
                MessageBox.Show(ex.Message, "Error al consultar empleados", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MtdLimpiarYRefrescar()
        {
            try
            {
                // 1. Limpiar todos los campos de texto
                txtCodigo.Clear();
                txtNombre.Clear();
                txtDpi.Clear();

                // 2. Resetear ComboBoxes
                cboxGenero.SelectedIndex = -1;
                cboxCargo.SelectedIndex = -1;

                // 3. Resetear Controles de Fecha y Números
                dtpFechaNacimiento.Value = DateTime.Today;
                dtpFechaContratacion.Value = DateTime.Today;

                // 4. Resetear RadioButtons
                rdbActivo.Checked = false;
                rdbInactivo.Checked = false;

                // 5. Limpiar selección del DataGridView
                dgvEmpleados.ClearSelection();
                dgvEmpleados.CurrentCell = null;
                filaActiva = null;

                // 6. Recargar datos desde la base de datos
                MtdConsultarEmpleados();

                // 7. Actualizar contador
                MtdContarTotalRegistros();

                // 8. Opcional: Quitar colores de selección de filas
                foreach (DataGridViewRow row in dgvEmpleados.Rows)
                {
                    if (dgvEmpleados.Columns.Contains("Seleccionar"))
                    {
                        row.Cells["Seleccionar"].Value = false;
                    }
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al refrescar el formulario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!MtdValidarDatos()) return;

            try
            {
                char generoSeleccionado = cboxGenero.Text == "Masculino" ? 'M' :
                                         cboxGenero.Text == "Femenino" ? 'F' : 'O';

                EmpleadosEntidad empleado = new EmpleadosEntidad
                {
                    Nombre = txtNombre.Text.Trim(),
                    NumeroDpi = long.Parse(txtDpi.Text),
                    Cargo = cboxCargo.Text,
                    Salario = decimal.Parse(txtsalario.Text),
                    Genero = generoSeleccionado,
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    FechaContratacion = dtpFechaContratacion.Value,
                    Estado = rdbActivo.Checked,
                    UsuarioCreacion = "SYSTEM" // O el usuario logueado
                };

                string respuesta = empleadosNegocio.MtdAgregarEmpleados(empleado);
                MessageBox.Show(respuesta, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MtdLimpiarYRefrescar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MtdEstadoFilaSeleccionada(bool estado)
        {
            btnEditar.Enabled = estado;
            btnEliminar.Enabled = estado;
            btnCancelar.Enabled = estado;
            btnNuevo.Enabled = !estado;
            btnGuardar.Enabled = false;

            // Bloquear/Desbloquear campos
            txtNombre.Enabled = estado;
            txtDpi.Enabled = estado;
            cboxCargo.Enabled = estado;
            txtsalario.Enabled = estado;
            dtpFechaNacimiento.Enabled = estado;
            dtpFechaContratacion.Enabled = estado;
            cboxGenero.Enabled = estado;
        }

        // Contar cantidad de líneas del DataGridView de Empleados
        private void MtdContarTotalRegistros()
        {
            try
            {
                // Obtenemos el total de filas cargadas en el grid
                int totalRegistros = dgvEmpleados.Rows.Count;

                // Actualizamos el Label (asegúrate de que el nombre coincida con tu diseño)
                lblTotalRegistros.Text = "Total de empleados: " + totalRegistros.ToString();
            }
            catch (Exception ex)
            {
                // En caso de error, mostramos un mensaje discreto
                lblTotalRegistros.Text = "Total de registros: 0";
                Console.WriteLine("Error al contar registros: " + ex.Message);
            }
        }

        private void MtdEstadoBotonNuevo()
        {
            MtdEstadoFilaSeleccionada(true);
            btnGuardar.Enabled = true;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
        }

        private bool MtdValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { txtNombre.Focus(); return false; }
            if (!long.TryParse(txtDpi.Text, out _)) { txtDpi.Focus(); return false; }
            if (!decimal.TryParse(txtsalario.Text, out _)) { txtsalario.Focus(); return false; }
            if (cboxCargo.SelectedIndex == -1) return false;

            return true;
        }

        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvEmpleados.Columns[e.ColumnIndex].Name != "Seleccionar") return;

            dgvEmpleados.EndEdit();
            bool estaSeleccionado = Convert.ToBoolean(dgvEmpleados.Rows[e.RowIndex].Cells["Seleccionar"].Value);

            if (estaSeleccionado)
            {
                MtdActivarFila(e.RowIndex);
            }
            else
            {
                MtdDesactivarFila();
            }
        }

        // Desactivar fila seleccionada en Empleados
        private void MtdDesactivarFila()
        {
            try
            {
                // 1. Si hay una fila marcada anteriormente, restaurar su color original
                if (filaActiva.HasValue && filaActiva.Value < dgvEmpleados.Rows.Count)
                {
                    dgvEmpleados.Rows[filaActiva.Value].Cells["Seleccionar"].Value = false;
                    dgvEmpleados.Rows[filaActiva.Value].DefaultCellStyle.BackColor = Color.White;
                }

                // 2. Resetear el índice de la fila activa
                filaActiva = null;

                // 3. Limpiar los cuadros de texto y combos (Usando el método que ya tienes)
                MtdLimpiarYRefrescar();

                // 4. Cambiar el estado de los controles a "No seleccionado"
                // Esto deshabilitará botones de Editar/Eliminar y habilitará el de Nuevo
                MtdEstadoFilaSeleccionada(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desactivar la selección: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MtdActivarFila(int index)
        {
            var emp = (EmpleadosEntidad)dgvEmpleados.Rows[index].DataBoundItem;

            txtCodigo.Text = emp.CodigoEmpleado.ToString();
            txtNombre.Text = emp.Nombre;
            txtDpi.Text = emp.NumeroDpi.ToString();
            txtsalario.Text = emp.Salario.ToString();
            cboxCargo.Text = emp.Cargo;
            dtpFechaNacimiento.Value = emp.FechaNacimiento;

            MtdEstadoFilaSeleccionada(true);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // 1. Limpiar todos los controles para empezar desde cero
            MtdLimpiarYRefrescar();

            // 2. Cambiar el estado de los controles (Habilitar campos, preparar botones)
            MtdEstadoBotonNuevo();

            // 3. Poner el foco en el primer campo de entrada (Nombre)
            txtNombre.Focus();
        }
    }
}