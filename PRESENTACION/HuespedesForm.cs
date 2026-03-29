using MODELO;
using NEGOCIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRESENTACION
{
    public partial class HuespedesForm : Form
    {
        // Instanciar capa Negocio 
        HuespedesNegocio  huespedNegocio  = new HuespedesNegocio();

        public HuespedesForm()
        {
            InitializeComponent();
        }

        private void HuespedesForm_Load(object sender, EventArgs e)
        {
            MtdConsultarHuespedes();
            MtdRenombrarNombreColumna();
            MtdEstadoFilaSelecionada(false); //  ***  Agregar metodo cuando trabaje opción Agregar
        }


        /*  -----   CONSULTAR   -----   */

        // Cambia el titulo a las columnas
        private void MtdRenombrarNombreColumna()
        {
            CambiarTitulo("CodigoHuesped", "Código");
            CambiarTitulo("TipoIdentificacion", "Código Identificación");
            CambiarTitulo("NumeroIdentificacion", "Número Identificación");
            CambiarTitulo("FechaNacimiento", "Fecha Nacimiento");
        }

        private void CambiarTitulo(string nombreColumna, string titulo)
        {
            if (dgvHuespedes.Columns.Contains(nombreColumna))
            {
                dgvHuespedes.Columns[nombreColumna].HeaderText = titulo;
            }
        }

        // Contar cantidad de lineas del DataGridView
        private void MtdContarTotalRegistros()
        {
            int totalRegistros = dgvHuespedes.Rows.Count;
            lblTotalRegistros.Text = "Total de registros: " + totalRegistros.ToString();
        }

        // Consultar datos de tabla Huespedes y mostrar en DataGridView
        private void MtdConsultarHuespedes()
        {
            try
            {
                dgvHuespedes.DataSource = huespedNegocio.MtdConsultarHuespedes();
                dgvHuespedes.ClearSelection();
                dgvHuespedes.CurrentCell = null;

                filaActiva = null;

                MtdContarTotalRegistros();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al consultar",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Convertir valores de las columnas Genero y Estado
        private void dgvHuespedes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvHuespedes.Columns[e.ColumnIndex].Name == "Genero" && e.Value != null)
            {
                switch ((char)e.Value)
                {
                    case 'M': e.Value = "Masculino"; break;
                    case 'F': e.Value = "Femenino"; break;
                    case 'O': e.Value = "Otro"; break;
                    default: e.Value = ""; break;
                }
                e.FormattingApplied = true;
            }
        }


        /*  -----   AGREGAR   -----   */
 
        // Controla el estado cuando hay una fila seleccionada
        private void MtdEstadoFilaSelecionada(bool Estado)
        {
            btnEditar.Enabled = Estado;
            btnEliminar.Enabled = Estado;
            btnCancelar.Enabled = Estado;
            btnNuevo.Enabled = !Estado;
            btnGuardar.Enabled = false;

            txtCodigo.Enabled = Estado;
            txtNombre.Enabled = Estado;
            cboxTipoIdentificacion.Enabled = Estado;
            txtNumeroIdentificacion.Enabled = Estado;
            dtpFechaNacimiento.Enabled = Estado;
            cboxGenero.Enabled = Estado;
            txtTelefono.Enabled = Estado;
            txtDireccion.Enabled = Estado;
            nudPuntuacion.Enabled = Estado;
            rdbActivo.Enabled = Estado;
            rdbInactivo.Enabled = Estado;
        }

        // Controla el estados para el boton Nuevo
        private void MtdEstadoBotonNuevo()
        {
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = true;
            btnNuevo.Enabled = false;
            btnGuardar.Enabled = true;

            txtCodigo.Enabled = false;
            txtNombre.Enabled = true;
            cboxTipoIdentificacion.Enabled = true;
            txtNumeroIdentificacion.Enabled = true;
            dtpFechaNacimiento.Enabled = true;
            cboxGenero.Enabled = true;
            txtTelefono.Enabled = true;
            txtDireccion.Enabled = true;
            nudPuntuacion.Enabled = true;
            rdbActivo.Enabled = true;
            rdbInactivo.Enabled = true;
        }

        // Limpia Formulario
        private void MtdLimpiarControlesForm()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            cboxTipoIdentificacion.SelectedIndex = -1;
            txtNumeroIdentificacion.Clear();
            dtpFechaNacimiento.Value = DateTime.Today;
            cboxGenero.SelectedIndex = -1;
            txtTelefono.Clear();
            txtDireccion.Clear();
            nudPuntuacion.Value = 0;
            rdbActivo.Checked = false;
            rdbInactivo.Checked = false;
            txtBuscarNombre.Clear();

            dgvHuespedes.ClearSelection();
            dgvHuespedes.CurrentCell = null;

            foreach (DataGridViewRow row in dgvHuespedes.Rows)
            {
                row.Cells["Seleccionar"].Value = false;
                row.DefaultCellStyle.BackColor = Color.White;
            }
        }

        // Boton Nuevo
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            MtdLimpiarControlesForm();
            MtdEstadoBotonNuevo();
            txtNombre.Focus();
        }

        // Boton Cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            MtdLimpiarControlesForm();
            MtdEstadoFilaSelecionada(false);
        }

        //  Valida datos ingresados en e formulario
        private bool MtdValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (cboxTipoIdentificacion.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione tipo de identificación", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboxTipoIdentificacion.DroppedDown = true;
                return false;
            }

            if (!Int64.TryParse(txtNumeroIdentificacion.Text, out _))
            {
                MessageBox.Show("Número de identificación inválido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumeroIdentificacion.Focus();
                return false;
            }

            if (cboxGenero.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione género", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboxGenero.DroppedDown = true;
                return false;
            }

            if (!int.TryParse(txtTelefono.Text, out _))
            {
                MessageBox.Show("Teléfono inválido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return false;
            }

            if (!rdbActivo.Checked && !rdbInactivo.Checked)
            {
                MessageBox.Show("Debe seleccionar el estado", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Boton Guardar
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (MtdValidarDatos() == false)
                return;

            try
            {
                char genero;

                if (cboxGenero.SelectedItem.ToString() == "Masculino")
                    genero = 'M';
                else if (cboxGenero.SelectedItem.ToString() == "Femenino")
                    genero = 'F';
                else if (cboxGenero.SelectedItem.ToString() == "Otro")
                    genero = 'O';
                else
                    throw new Exception("Género no válido");


                HuespedesEntidad huesped = new HuespedesEntidad
                {
                    Nombre = txtNombre.Text.Trim(),
                    TipoIdentificacion = cboxTipoIdentificacion.SelectedItem.ToString(),
                    NumeroIdentificacion = Convert.ToInt64(txtNumeroIdentificacion.Text),
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    Genero = genero,
                    Telefono = Convert.ToInt32(txtTelefono.Text),
                    Puntuacion = nudPuntuacion.Value,
                    Direccion = txtDireccion.Text.Trim(),
                    Estado = rdbActivo.Checked
                };

                string mensaje = huespedNegocio.MtdAgregarHuespedes(huesped);
                MessageBox.Show(mensaje, "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MtdLimpiarControlesForm();
                MtdConsultarHuespedes();
                MtdEstadoFilaSelecionada(false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        /*  -----   EDITAR   -----   */

        //Cargar datos de la fila selecciohnada en los controles del Form
        private void MtdCargarDatosFilaEnControlesForm(int filaSeleccionada)
        {
            var huesped = (HuespedesEntidad)dgvHuespedes.Rows[filaSeleccionada].DataBoundItem;

            string genero;

            if (huesped.Genero.ToString() == "M")
                genero = "Masculino";
            else if (huesped.Genero.ToString() == "F")
                genero = "Femenino";
            else if (huesped.Genero.ToString() == "O")
                genero = "Otro";
            else
                throw new Exception("Género no válido");

            txtCodigo.Text = huesped.CodigoHuesped.ToString();
            txtNombre.Text = huesped.Nombre;
            cboxTipoIdentificacion.SelectedItem = huesped.TipoIdentificacion;
            txtNumeroIdentificacion.Text = huesped.NumeroIdentificacion.ToString();
            dtpFechaNacimiento.Value = huesped.FechaNacimiento;
            cboxGenero.SelectedItem = genero;
            txtTelefono.Text = huesped.Telefono.ToString();
            txtDireccion.Text = huesped.Direccion;
            nudPuntuacion.Value = huesped.Puntuacion;

            rdbActivo.Checked = huesped.Estado;
            rdbInactivo.Checked = !huesped.Estado;
        }

        //Activar fila seleccionada
        private int? filaActiva = null;
        private void MtdActivarFilaSeleccionada(int filaSeleccionada)
        {
            if (filaActiva.HasValue && filaActiva.Value < dgvHuespedes.Rows.Count)
            {
                dgvHuespedes.Rows[filaActiva.Value].Cells["Seleccionar"].Value = false;
                dgvHuespedes.Rows[filaActiva.Value].DefaultCellStyle.BackColor = Color.White;
            }

            filaActiva = filaSeleccionada;

            dgvHuespedes.Rows[filaSeleccionada].Cells["Seleccionar"].Value = true;
            dgvHuespedes.Rows[filaSeleccionada].DefaultCellStyle.BackColor = Color.FromArgb(220, 235, 255);

            MtdCargarDatosFilaEnControlesForm(filaSeleccionada);
            MtdEstadoFilaSelecionada(true);
        }

        //Desactivar fila seleccionada
        private void MtdDesactivaFilaSeleccionada()
        {
            if (filaActiva.HasValue)
            {
                dgvHuespedes.Rows[filaActiva.Value].Cells["Seleccionar"].Value = false;
                dgvHuespedes.Rows[filaActiva.Value].DefaultCellStyle.BackColor = Color.White;
            }

            filaActiva = null;

            MtdLimpiarControlesForm();
            MtdEstadoFilaSelecionada(false);
        }

        // DataGridView = Gestiona los valores de la fila seleccionada 
        private void dgvHuespedes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvHuespedes.Columns[e.ColumnIndex].Name != "Seleccionar")
                return;

            if (!chkSeleccionar.Checked)
            {
                dgvHuespedes.Rows[e.RowIndex].Cells["Seleccionar"].Value = false;
                return;
            }

            bool seleccionActual = Convert.ToBoolean(
                dgvHuespedes.Rows[e.RowIndex].Cells["Seleccionar"].Value ?? false);

            if (seleccionActual)
                MtdDesactivaFilaSeleccionada();
            else
                MtdActivarFilaSeleccionada(e.RowIndex);
        }

        // CheckBox Selecionar Form : Activa y desactiva columna Seleccionar del DataGridView
        private void chkSeleccionar_CheckedChanged(object sender, EventArgs e)
        {
            dgvHuespedes.Columns["Seleccionar"].ReadOnly = !chkSeleccionar.Checked;
            btnImprimir.Enabled = chkSeleccionar.Checked;

            MtdDesactivaFilaSeleccionada();
        }

        // Boton Editar
        private void btnEditar_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Seleccione un huésped para editar","Aviso",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
 
            if (MtdValidarDatos() == false)
                return;

            try
            {
                char genero;

                if (cboxGenero.SelectedItem.ToString() == "Masculino")
                    genero = 'M';
                else if (cboxGenero.SelectedItem.ToString() == "Femenino")
                    genero = 'F';
                else if (cboxGenero.SelectedItem.ToString() == "Otro")
                    genero = 'O';
                else
                    throw new Exception("Género no válido");

                HuespedesEntidad huesped = new HuespedesEntidad
                {
                    CodigoHuesped = Convert.ToInt32(txtCodigo.Text),
                    Nombre = txtNombre.Text.Trim(),
                    TipoIdentificacion = cboxTipoIdentificacion.SelectedItem.ToString(),
                    NumeroIdentificacion = Convert.ToInt64(txtNumeroIdentificacion.Text),
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    Genero = genero,
                    Telefono = Convert.ToInt32(txtTelefono.Text),
                    Puntuacion = nudPuntuacion.Value,
                    Direccion = txtDireccion.Text.Trim(),
                    Estado = rdbActivo.Checked
                };

                string mensaje = huespedNegocio.MtdEditarHuespedes(huesped);
                MessageBox.Show(mensaje, "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MtdLimpiarControlesForm();
                MtdConsultarHuespedes();
                MtdEstadoFilaSelecionada(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /*  -----   ELIMINAR   -----   */

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                    throw new Exception("Debe seleccionar un huésped");

                int codigoHuesped = Convert.ToInt32(txtCodigo.Text);

                DialogResult confirmacion = MessageBox.Show("¿Está seguro de eliminar este huésped?", "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion == DialogResult.No)
                    return;

                string mensaje = huespedNegocio.MtdEliminarHuespedes(codigoHuesped);
                MessageBox.Show(mensaje, "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                filaActiva = null;

                MtdLimpiarControlesForm();
                MtdConsultarHuespedes();
                MtdEstadoFilaSelecionada(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /* ---- BUSCAR ---- */
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtBuscarNombre.Text.Trim();

                dgvHuespedes.DataSource = huespedNegocio.MtdBuscarHuespedes(nombre);

                dgvHuespedes.ClearSelection();
                dgvHuespedes.CurrentCell = null;

                filaActiva = null;

                MtdContarTotalRegistros();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al buscar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /* ---- LIMPIAR ---- */
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarNombre.Clear();
            MtdConsultarHuespedes();

            MtdLimpiarControlesForm();
            MtdEstadoFilaSelecionada(false);
            MtdContarTotalRegistros();
        }


    }
}
