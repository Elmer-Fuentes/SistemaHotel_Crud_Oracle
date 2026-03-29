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
        private bool isEditing = false;

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
            CambiarTitulo("Dpi", "DPI (Identificación)");
            CambiarTitulo("FechaIngreso", "F. Ingreso");
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
                // Map form fields to EmpleadosEntidad (DB stores DPI and Telefono as strings)
                EmpleadosEntidad empleado = new EmpleadosEntidad
                {
                    Nombre = txtNombre.Text.Trim(),
                    Dpi = txtDpi.Text.Trim(),
                    Nit = string.Empty,
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    FechaIngreso = dtpFechaContratacion.Value,
                    Direccion = string.Empty,
                    Telefono = string.Empty,
                    Estado = rdbActivo.Checked ? "1" : "0",
                    UsuarioCreacion = "SYSTEM"
                };

                string respuesta;
                if (isEditing)
                {
                    // Edit existing
                    if (!int.TryParse(txtCodigo.Text, out int codigo))
                    {
                        MessageBox.Show("Código inválido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    empleado.CodigoEmpleado = codigo;
                    respuesta = empleadosNegocio.MtdEditarEmpleados(empleado);
                }
                else
                {
                    respuesta = empleadosNegocio.MtdAgregarEmpleados(empleado);
                }
                MessageBox.Show(respuesta, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // reset edit mode and refresh
                isEditing = false;
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
            if (string.IsNullOrWhiteSpace(txtDpi.Text)) { txtDpi.Focus(); return false; }
            // Telefono is optional but if present ensure it's numeric-ish
            // if (!string.IsNullOrWhiteSpace(txtTelefono.Text) && !long.TryParse(txtTelefono.Text, out _)) { txtTelefono.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(cboxCargo.Text)) return false;

            return true;
        }

        private void dgvEmpleados_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // If the UI is in selection mode (checkbox visible), only toggle when clicking the checkbox column
            try
            {
                if (chkSeleccionar.Checked)
                {
                    // If clicked the checkbox column, toggle it
                    if (dgvEmpleados.Columns[e.ColumnIndex].Name == "Seleccionar")
                    {
                        dgvEmpleados.EndEdit();
                        object val = dgvEmpleados.Rows[e.RowIndex].Cells["Seleccionar"].Value;
                        bool estaSeleccionado = false;
                        if (val == null || val == DBNull.Value)
                            estaSeleccionado = false;
                        else
                        {
                            // Try parse common types
                            if (val is bool) estaSeleccionado = (bool)val;
                            else bool.TryParse(val.ToString(), out estaSeleccionado);
                        }

                        // toggle
                        dgvEmpleados.Rows[e.RowIndex].Cells["Seleccionar"].Value = !estaSeleccionado;

                        if (!estaSeleccionado)
                            MtdActivarFila(e.RowIndex);
                        else
                            MtdDesactivarFila();
                    }
                }
                else
                {
                    // Not in checkbox selection mode: clicking any cell selects the row and loads data
                    MtdActivarFila(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar fila: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkSeleccionar_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmpleados.Columns.Contains("Seleccionar"))
                {
                    dgvEmpleados.Columns["Seleccionar"].ReadOnly = !chkSeleccionar.Checked;
                }
                btnImprimir.Enabled = chkSeleccionar.Checked;
                // Clear any existing selection
                MtdDesactivarFila();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar modo seleccionar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtDpi.Text = emp.Dpi;
            // txtsalario UI exists but DB doesn't have salary; leave blank or show N/A
            txtsalario.Text = string.Empty;
            // cboxCargo not persisted in DB; leave selection alone
            cboxCargo.Text = cboxCargo.Text;
            dtpFechaNacimiento.Value = emp.FechaNacimiento ?? DateTime.Today;

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

        private void btnEditar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Seleccione un empleado para editar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Enter edit mode
            isEditing = true;
            MtdEstadoFilaSeleccionada(true);
            btnGuardar.Enabled = true;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnNuevo.Enabled = false;
            txtNombre.Focus();
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Seleccione un empleado para eliminar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Desea eliminar el empleado seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int codigo = 0;
                    if (!int.TryParse(txtCodigo.Text, out codigo))
                    {
                        MessageBox.Show("Código inválido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string resp = empleadosNegocio.MtdEliminarEmpleados(codigo);
                    MessageBox.Show(resp, "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MtdLimpiarYRefrescar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


    }
}