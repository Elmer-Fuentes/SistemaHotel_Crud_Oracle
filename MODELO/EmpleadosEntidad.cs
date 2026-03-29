using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELO
{
    public class EmpleadosEntidad
    {
        public int CodigoEmpleado { get; set; }
        public string Nombre { get; set; }
        public long NumeroDpi { get; set; } // Usamos long por el tamaño del DPI
        public char Genero { get; set; }
        public string Cargo { get; set; }
        public decimal Salario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaContratacion { get; set; }
        public bool Estado { get; set; }

        // Auditoría
        public string UsuarioCreacion { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}