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
            foreach (DataRow row in dt.Rows)
            {
                var e = new EmpleadosEntidad
                {
                    CodigoEmpleado = row["CodigoEmpleado"] == DBNull.Value ? 0 : Convert.ToInt32(row["CodigoEmpleado"]),
                    Nombre = row["Nombre"] == DBNull.Value ? string.Empty : row["Nombre"].ToString(),
                    NumeroDpi = row["NumeroDpi"] == DBNull.Value ? 0L : Convert.ToInt64(row["NumeroDpi"]),
                    Genero = row["Genero"] == DBNull.Value ? 'O' : Convert.ToChar(row["Genero"].ToString()[0]),
                    Cargo = row["Cargo"] == DBNull.Value ? string.Empty : row["Cargo"].ToString(),
                    Salario = row["Salario"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Salario"]),
                    FechaNacimiento = row["FechaNacimiento"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["FechaNacimiento"]),
                    FechaContratacion = row["FechaContratacion"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["FechaContratacion"]),
                    Estado = row["Estado"] == DBNull.Value ? false : Convert.ToInt32(row["Estado"]) == 1
                };
                lista.Add(e);
            }
            return lista;
        }

        public List<EmpleadosEntidad> MtdBuscarEmpleados(string nombre)
        {
            DataTable dt = datos.MtdBuscarEmpleados(nombre);
            List<EmpleadosEntidad> lista = new List<EmpleadosEntidad>();
            foreach (DataRow row in dt.Rows)
            {
                var e = new EmpleadosEntidad
                {
                    CodigoEmpleado = row["CodigoEmpleado"] == DBNull.Value ? 0 : Convert.ToInt32(row["CodigoEmpleado"]),
                    Nombre = row["Nombre"] == DBNull.Value ? string.Empty : row["Nombre"].ToString(),
                    NumeroDpi = row["NumeroDpi"] == DBNull.Value ? 0L : Convert.ToInt64(row["NumeroDpi"]),
                    Genero = row["Genero"] == DBNull.Value ? 'O' : Convert.ToChar(row["Genero"].ToString()[0]),
                    Cargo = row["Cargo"] == DBNull.Value ? string.Empty : row["Cargo"].ToString(),
                    Salario = row["Salario"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Salario"]),
                    FechaNacimiento = row["FechaNacimiento"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["FechaNacimiento"]),
                    FechaContratacion = row["FechaContratacion"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["FechaContratacion"]),
                    Estado = row["Estado"] == DBNull.Value ? false : Convert.ToInt32(row["Estado"]) == 1
                };
                lista.Add(e);
            }
            return lista;
        }

        public string MtdAgregarEmpleados(EmpleadosEntidad empleado)
        {
            if (empleado == null) throw new ArgumentNullException(nameof(empleado));
            if (string.IsNullOrWhiteSpace(empleado.Nombre)) throw new ArgumentException("Nombre requerido");
            if (empleado.NumeroDpi <= 0) throw new ArgumentException("DPI requerido");
            if (empleado.Salario <= 0) throw new ArgumentException("Salario inválido");

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
