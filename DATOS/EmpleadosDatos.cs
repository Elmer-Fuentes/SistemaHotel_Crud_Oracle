using MODELO;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATOS
{
    public class EmpleadosDatos
    {
        private readonly ConexionDatos conexionDatos = new ConexionDatos();

        public DataTable MtdConsultarEmpleados()
        {
            DataTable dt = new DataTable();
            try
            {
                // Usamos la conexión configurada en tu clase base de datos
                using (OracleConnection conn = conexionDatos.MtdConexionBDD())
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("SYSTEM.usp_ConsultarEmpleados", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetro de salida para el Cursor de Oracle
                        OracleParameter pCursor = new OracleParameter();
                        pCursor.ParameterName = "p_Cursor";
                        pCursor.OracleDbType = OracleDbType.RefCursor;
                        pCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pCursor);

                        // Llenar el DataTable con el resultado del procedimiento
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                // Propagamos el error para que la capa de negocio o presentación lo maneje
                throw new Exception("Error en Capa Datos al consultar empleados: " + ex.Message);
            }
        }

        public string MtdAgregarEmpleados(EmpleadosEntidad empleado)
        {
            using (OracleConnection conn = conexionDatos.MtdConexionBDD())
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("SYSTEM.usp_AgregarEmpleados", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_Nombre", OracleDbType.Varchar2).Value = empleado.Nombre;
                    cmd.Parameters.Add("p_NumeroDpi", OracleDbType.Int64).Value = empleado.NumeroDpi;
                    cmd.Parameters.Add("p_Genero", OracleDbType.Char).Value = empleado.Genero;
                    cmd.Parameters.Add("p_Cargo", OracleDbType.Varchar2).Value = empleado.Cargo;
                    cmd.Parameters.Add("p_Salario", OracleDbType.Decimal).Value = empleado.Salario;
                    cmd.Parameters.Add("p_FechaNacimiento", OracleDbType.Date).Value = empleado.FechaNacimiento;
                    cmd.Parameters.Add("p_FechaContratacion", OracleDbType.Date).Value = empleado.FechaContratacion;
                    cmd.Parameters.Add("p_Estado", OracleDbType.Int32).Value = empleado.Estado ? 1 : 0;
                    cmd.Parameters.Add("p_UsuarioCreacion", OracleDbType.Varchar2).Value = empleado.UsuarioCreacion;

                    var pResultado = new OracleParameter("p_Resultado", OracleDbType.Int32) { Direction = ParameterDirection.Output };
                    var pMensaje = new OracleParameter("p_Mensaje", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pResultado);
                    cmd.Parameters.Add(pMensaje);

                    cmd.ExecuteNonQuery();

                    if (pResultado.Value.ToString() == "0")
                        throw new Exception(pMensaje.Value.ToString());

                    return pMensaje.Value.ToString();
                }
            }
        }
    }
}