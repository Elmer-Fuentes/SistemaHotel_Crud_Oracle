using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using MODELO;

namespace DATOS
{
    public class HuespedesDatos
    {
        private readonly ConexionDatos conexionDatos = new ConexionDatos();


        /*  ---- CONSULTAR  ----  */
        public DataTable MtdConsultarHuespedes()            
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = conexionDatos.MtdConexionBDD())
            {
                using (OracleCommand cmd = new OracleCommand("usp_ConsultarHuespedes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // SYS_REFCURSOR de Oracle: se agrega como parámetro de salida
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor)
                       .Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return dt;
        }


        /*  ---- AGREGAR  ----  */
        public string MtdAgregarHuespedes(HuespedesEntidad huesped)
        {
            using (OracleConnection conn = conexionDatos.MtdConexionBDD())
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("usp_AgregarHuespedes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada — el orden debe coincidir con el SP de Oracle
                    cmd.Parameters.Add("p_Nombre", OracleDbType.Varchar2).Value = huesped.Nombre;
                    cmd.Parameters.Add("p_TipoIdentificacion", OracleDbType.Varchar2).Value = huesped.TipoIdentificacion;
                    cmd.Parameters.Add("p_NumeroIdentificacion", OracleDbType.Int64).Value = huesped.NumeroIdentificacion;
                    cmd.Parameters.Add("p_FechaNacimiento", OracleDbType.Date).Value = huesped.FechaNacimiento;
                    cmd.Parameters.Add("p_Genero", OracleDbType.Char).Value = huesped.Genero.ToString();
                    // CORRECCIÓN: enviar Telefono como Int64
                    cmd.Parameters.Add("p_Telefono", OracleDbType.Int64).Value = huesped.Telefono;
                    cmd.Parameters.Add("p_Direccion", OracleDbType.Varchar2).Value = huesped.Direccion;
                    cmd.Parameters.Add("p_Puntuacion", OracleDbType.Decimal).Value = huesped.Puntuacion;
                    // Oracle guarda Estado como NUMBER (1=activo, 0=inactivo)
                    cmd.Parameters.Add("p_Estado", OracleDbType.Int32).Value = huesped.Estado ? 1 : 0;

                    // Parámetros de salida
                    var pResultado = new OracleParameter("p_Resultado", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var pMensaje = new OracleParameter("p_Mensaje", OracleDbType.Varchar2, 500)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(pResultado);
                    cmd.Parameters.Add(pMensaje);

                    cmd.ExecuteNonQuery();

                    int resultado = Convert.ToInt32(pResultado.Value.ToString());
                    string mensaje = pMensaje.Value?.ToString() ?? "Sin mensaje del servidor";

                    if (resultado == 0)
                        throw new Exception(mensaje);

                    return mensaje;
                }
            }
        }


        /*  ---- EDITAR  ----  */
        public string MtdEditarHuespedes(HuespedesEntidad huesped)
        {
            using (OracleConnection conn = conexionDatos.MtdConexionBDD())
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("usp_EditarHuespedes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CodigoHuesped", OracleDbType.Int32).Value = huesped.CodigoHuesped;
                    cmd.Parameters.Add("p_Nombre", OracleDbType.Varchar2).Value = huesped.Nombre;
                    cmd.Parameters.Add("p_TipoIdentificacion", OracleDbType.Varchar2).Value = huesped.TipoIdentificacion;
                    cmd.Parameters.Add("p_NumeroIdentificacion", OracleDbType.Int64).Value = huesped.NumeroIdentificacion;
                    cmd.Parameters.Add("p_FechaNacimiento", OracleDbType.Date).Value = huesped.FechaNacimiento;
                    cmd.Parameters.Add("p_Genero", OracleDbType.Char).Value = huesped.Genero.ToString();
                    // CORRECCIÓN: enviar Telefono como Int64
                    cmd.Parameters.Add("p_Telefono", OracleDbType.Int64).Value = huesped.Telefono;
                    cmd.Parameters.Add("p_Direccion", OracleDbType.Varchar2).Value = huesped.Direccion;
                    cmd.Parameters.Add("p_Puntuacion", OracleDbType.Decimal).Value = huesped.Puntuacion;
                    cmd.Parameters.Add("p_Estado", OracleDbType.Int32).Value = huesped.Estado ? 1 : 0;

                    var pResultado = new OracleParameter("p_Resultado", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var pMensaje = new OracleParameter("p_Mensaje", OracleDbType.Varchar2, 500)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(pResultado);
                    cmd.Parameters.Add(pMensaje);

                    cmd.ExecuteNonQuery();

                    int resultado = Convert.ToInt32(pResultado.Value.ToString());
                    string mensaje = pMensaje.Value?.ToString() ?? "Sin mensaje del servidor";

                    if (resultado == 0)
                        throw new ApplicationException(mensaje);

                    return mensaje;
                }
            }
        }


        /*  ---- ELIMINAR  ----  */
        public string MtdEliminarHuespedes(int codigoHuesped)
        {
            using (OracleConnection conn = conexionDatos.MtdConexionBDD())
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand("usp_EliminarHuespedes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_CodigoHuesped", OracleDbType.Int32).Value = codigoHuesped;

                    var pResultado = new OracleParameter("p_Resultado", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var pMensaje = new OracleParameter("p_Mensaje", OracleDbType.Varchar2, 500)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(pResultado);
                    cmd.Parameters.Add(pMensaje);

                    cmd.ExecuteNonQuery();

                    int resultado = Convert.ToInt32(pResultado.Value.ToString());
                    string mensaje = pMensaje.Value?.ToString() ?? "Sin mensaje del servidor";

                    if (resultado == 0)
                        throw new Exception(mensaje);

                    return mensaje;
                }
            }
        }


        /*  ---- BUSCAR  ----  */
        public DataTable MtdBuscarHuespedes(string nombre)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = conexionDatos.MtdConexionBDD())
            {
                using (OracleCommand cmd = new OracleCommand("usp_BuscarHuespedes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_Nombre", OracleDbType.Varchar2).Value = nombre;

                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor)
                       .Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return dt;
        }
    }
}