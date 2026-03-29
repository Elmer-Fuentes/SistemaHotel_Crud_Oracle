using DATOS;
using MODELO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEGOCIO
{
    public class HuespedesNegocio
    {
        HuespedesDatos huespedesDatos = new HuespedesDatos();


        /* ----- CONSULTAR ----- */
        public List<HuespedesEntidad> MtdConsultarHuespedes()
        {
            try
            {
                DataTable dt = huespedesDatos.MtdConsultarHuespedes();

                List<HuespedesEntidad> lista = new List<HuespedesEntidad>();

                foreach (DataRow row in dt.Rows)
                {
                    HuespedesEntidad huesped = new HuespedesEntidad
                    {
                        CodigoHuesped = Convert.ToInt32(row["CodigoHuesped"]),
                        Nombre = row["Nombre"].ToString(),
                        TipoIdentificacion = row["TipoIdentificacion"].ToString(),
                        NumeroIdentificacion = Convert.ToInt64(row["NumeroIdentificacion"]),
                        FechaNacimiento = Convert.ToDateTime(row["FechaNacimiento"]),
                        Genero = Convert.ToChar(row["Genero"]),
                        // CORRECCIÓN: leer Teléfono como Int64 y manejar DBNull
                        Telefono = row["Telefono"] == DBNull.Value ? 0L : Convert.ToInt64(row["Telefono"]),
                        Direccion = row["Direccion"].ToString(),
                        Puntuacion = Convert.ToDecimal(row["Puntuacion"]),
                        Estado = Convert.ToInt32(row["Estado"]) == 1
                    };

                    lista.Add(huesped);
                }

                return lista;
            }
            catch
            {
                throw;
            }
        }


        /* ----- AGREGAR ----- */
        public string MtdAgregarHuespedes(HuespedesEntidad huesped)
        {
            if (huesped == null)
                throw new Exception("No se recibieron datos");

            if (huesped.CodigoHuesped != 0)
                throw new Exception("Codigo de huesped no necesita valor");

            if (string.IsNullOrWhiteSpace(huesped.Nombre))
                throw new Exception("No envió valor al campo nombre");

            if (string.IsNullOrWhiteSpace(huesped.TipoIdentificacion))
                throw new Exception("No envió valor al campo tipo de identificación");

            if (huesped.NumeroIdentificacion <= 0)
                throw new Exception("No envió valor al campo numero de identificación");

            if (huesped.FechaNacimiento >= DateTime.Today)
                throw new Exception("La fecha de nacimiento tiene que ser menor a la fecha del dia");

            if (huesped.Genero != 'M' && huesped.Genero != 'F' && huesped.Genero != 'O')
                throw new Exception("Valor ingresado en el campo genero no permitido");

            if (huesped.Telefono <= 0)
                throw new Exception("Telefono no válido");

            if (huesped.Puntuacion <= 0)
                throw new Exception("Numero de puntuación no es válida");

            try
            {
                return huespedesDatos.MtdAgregarHuespedes(huesped);
            }
            catch
            {
                throw;
            }
        }


        /* ----- EDITAR ----- */
        public string MtdEditarHuespedes(HuespedesEntidad huesped)
        {
            if (huesped == null)
                throw new Exception("No se recibieron datos");

            if (huesped.CodigoHuesped <= 0)
                throw new Exception("Debe enviar el código del huésped a editar");

            if (string.IsNullOrWhiteSpace(huesped.Nombre))
                throw new Exception("No envió valor al campo nombre");

            if (string.IsNullOrWhiteSpace(huesped.TipoIdentificacion))
                throw new Exception("No envió valor al campo tipo de identificación");

            if (huesped.NumeroIdentificacion <= 0)
                throw new Exception("Número de identificación no válido");

            if (huesped.FechaNacimiento >= DateTime.Today)
                throw new Exception("La fecha de nacimiento tiene que ser menor a la fecha del día");

            if (huesped.Genero != 'M' && huesped.Genero != 'F' && huesped.Genero != 'O')
                throw new Exception("Valor ingresado en el campo género no permitido");

            if (huesped.Telefono <= 0)
                throw new Exception("Teléfono no válido");

            if (huesped.Puntuacion <= 0)
                throw new Exception("Número de puntuación no es válida");

            try
            {
                return huespedesDatos.MtdEditarHuespedes(huesped);
            }
            catch
            {
                throw;
            }
        }


        /* ---- ELIMINAR ---- */
        public string MtdEliminarHuespedes(int CodigoHuesped)
        {
            if (CodigoHuesped <= 0)
                throw new Exception("Debe seleccionar un huésped válido");

            try
            {
                return huespedesDatos.MtdEliminarHuespedes(CodigoHuesped);
            }
            catch
            {
                throw;
            }
        }


        /* ---- BUSCAR ---- */
        public List<HuespedesEntidad> MtdBuscarHuespedes(string nombre)
        {
            try
            {
                DataTable dt = huespedesDatos.MtdBuscarHuespedes(nombre.Trim());

                List<HuespedesEntidad> lista = new List<HuespedesEntidad>();

                foreach (DataRow row in dt.Rows)
                {
                    // Dentro del foreach en MtdConsultarHuespedes:
                    HuespedesEntidad huesped = new HuespedesEntidad
                    {
                        CodigoHuesped = Convert.ToInt32(row["CodigoHuesped"]),
                        Nombre = row["Nombre"].ToString(),
                        TipoIdentificacion = row["TipoIdentificacion"].ToString(),

                        // USAR Int64 para el DPI/Identificación
                        NumeroIdentificacion = Convert.ToInt64(row["NumeroIdentificacion"]),

                        FechaNacimiento = Convert.ToDateTime(row["FechaNacimiento"]),
                        Genero = Convert.ToChar(row["Genero"]),

                        // ya estaba correcto aquí: Int64
                        Telefono = row["Telefono"] == DBNull.Value ? 0L : Convert.ToInt64(row["Telefono"]),

                        Direccion = row["Direccion"].ToString(),
                        Puntuacion = Convert.ToDecimal(row["Puntuacion"]),

                        // Para el estado, asegúrate de comparar bien el valor
                        Estado = row["Estado"].ToString() == "1"
                    };
                    lista.Add(huesped);
                }

                return lista;
            }
            catch
            {
                throw;
            }

        }
    }
}