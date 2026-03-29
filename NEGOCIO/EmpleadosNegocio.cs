using DATOS;
using MODELO;
using System;
using System.Collections.Generic;
using System.Data;

namespace NEGOCIO
{
    public class EmpleadosNegocio
    {
        private readonly EmpleadosDatos datos = new EmpleadosDatos();

        public List<EmpleadosEntidad> MtdConsultarEmpleados()
        {
            DataTable dt = datos.MtdConsultarEmpleados();
            List<EmpleadosEntidad> lista = new List<EmpleadosEntidad>();

            // Helper to find column name case-insensitively
            string FindColumn(DataRow row, string name)
            {
                if (row.Table == null) return null;
                foreach (DataColumn col in row.Table.Columns)
                {
                    if (string.Equals(col.ColumnName, name, StringComparison.OrdinalIgnoreCase))
                        return col.ColumnName;
                }
                return null;
            }

            foreach (DataRow row in dt.Rows)
            {
                string cCodigo = FindColumn(row, "CodigoEmpleado");
                string cNombre = FindColumn(row, "Nombre");
                string cDpi = FindColumn(row, "Dpi");
                string cNit = FindColumn(row, "Nit");
                string cFechaN = FindColumn(row, "Fechanacimiento");
                string cFechaIngreso = FindColumn(row, "Fechaingreso");
                string cDireccion = FindColumn(row, "Direccion");
                string cTelefono = FindColumn(row, "Telefono");
                string cEstado = FindColumn(row, "Estado");

                var e = new EmpleadosEntidad
                {
                    CodigoEmpleado = cCodigo == null || row[cCodigo] == DBNull.Value ? 0 : Convert.ToInt32(row[cCodigo]),
                    Nombre = cNombre == null || row[cNombre] == DBNull.Value ? string.Empty : row[cNombre].ToString(),
                    Dpi = cDpi == null || row[cDpi] == DBNull.Value ? string.Empty : row[cDpi].ToString(),
                    Nit = cNit == null || row[cNit] == DBNull.Value ? string.Empty : row[cNit].ToString(),
                    FechaNacimiento = cFechaN == null || row[cFechaN] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row[cFechaN]),
                    FechaIngreso = cFechaIngreso == null || row[cFechaIngreso] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row[cFechaIngreso]),
                    Direccion = cDireccion == null || row[cDireccion] == DBNull.Value ? string.Empty : row[cDireccion].ToString(),
                    Telefono = cTelefono == null || row[cTelefono] == DBNull.Value ? string.Empty : row[cTelefono].ToString(),
                    Estado = cEstado == null || row[cEstado] == DBNull.Value ? string.Empty : row[cEstado].ToString()
                };
                lista.Add(e);
            }

            return lista;
        }

        public List<EmpleadosEntidad> MtdBuscarEmpleados(string nombre)
        {
            DataTable dt = datos.MtdBuscarEmpleados(nombre);
            List<EmpleadosEntidad> lista = new List<EmpleadosEntidad>();

            // Reuse same case-insensitive column lookup approach
            string FindColumn(DataRow row, string name)
            {
                if (row.Table == null) return null;
                foreach (DataColumn col in row.Table.Columns)
                {
                    if (string.Equals(col.ColumnName, name, StringComparison.OrdinalIgnoreCase))
                        return col.ColumnName;
                }
                return null;
            }

            foreach (DataRow row in dt.Rows)
            {
                string cCodigo = FindColumn(row, "CodigoEmpleado");
                string cNombre = FindColumn(row, "Nombre");
                string cDpi = FindColumn(row, "Dpi");
                string cNit = FindColumn(row, "Nit");
                string cFechaN = FindColumn(row, "Fechanacimiento");
                string cFechaIngreso = FindColumn(row, "Fechaingreso");
                string cDireccion = FindColumn(row, "Direccion");
                string cTelefono = FindColumn(row, "Telefono");
                string cEstado = FindColumn(row, "Estado");

                var e = new EmpleadosEntidad
                {
                    CodigoEmpleado = cCodigo == null || row[cCodigo] == DBNull.Value ? 0 : Convert.ToInt32(row[cCodigo]),
                    Nombre = cNombre == null || row[cNombre] == DBNull.Value ? string.Empty : row[cNombre].ToString(),
                    Dpi = cDpi == null || row[cDpi] == DBNull.Value ? string.Empty : row[cDpi].ToString(),
                    Nit = cNit == null || row[cNit] == DBNull.Value ? string.Empty : row[cNit].ToString(),
                    FechaNacimiento = cFechaN == null || row[cFechaN] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row[cFechaN]),
                    FechaIngreso = cFechaIngreso == null || row[cFechaIngreso] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row[cFechaIngreso]),
                    Direccion = cDireccion == null || row[cDireccion] == DBNull.Value ? string.Empty : row[cDireccion].ToString(),
                    Telefono = cTelefono == null || row[cTelefono] == DBNull.Value ? string.Empty : row[cTelefono].ToString(),
                    Estado = cEstado == null || row[cEstado] == DBNull.Value ? string.Empty : row[cEstado].ToString()
                };
                lista.Add(e);
            }

            return lista;
        }

        public string MtdAgregarEmpleados(EmpleadosEntidad empleado)
        {
            if (empleado == null) throw new ArgumentNullException(nameof(empleado));
            if (string.IsNullOrWhiteSpace(empleado.Nombre)) throw new ArgumentException("Nombre requerido");
            if (string.IsNullOrWhiteSpace(empleado.Dpi)) throw new ArgumentException("DPI requerido");

            return datos.MtdAgregarEmpleados(empleado);
        }

        public string MtdEditarEmpleados(EmpleadosEntidad empleado)
        {
            if (empleado == null) throw new ArgumentNullException(nameof(empleado));
            if (empleado.CodigoEmpleado <= 0) throw new ArgumentException("Código empleado requerido");
            return datos.MtdEditarEmpleados(empleado);
        }

        public string MtdEliminarEmpleados(int codigoEmpleado)
        {
            if (codigoEmpleado <= 0) throw new ArgumentException("Código empleado requerido");
            return datos.MtdEliminarEmpleados(codigoEmpleado);
        }
    }
}
