using System;
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
        // DB stores DPI as VARCHAR2
        public string Dpi { get; set; }
        public string Nit { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        // Estado stored as VARCHAR2 (e.g., 'ACT'/'INA' or '1'/'0')
        public string Estado { get; set; }

        // Auditoría (opcional)
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}