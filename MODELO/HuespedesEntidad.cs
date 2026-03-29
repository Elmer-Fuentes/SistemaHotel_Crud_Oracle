using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELO
{
    public class HuespedesEntidad
    {
        [Key]
        [Required]
        public int CodigoHuesped { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string TipoIdentificacion { get; set; }
        [Required]
        public long NumeroIdentificacion { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }
        public char Genero { get; set; }
        public Int64 Telefono { get; set; }
        public string Direccion { get; set; }
        public decimal Puntuacion { get; set; }
        [Required]
        public bool Estado { get; set; }

    }
}
