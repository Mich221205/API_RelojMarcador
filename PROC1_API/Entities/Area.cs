using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROC1_API.Entities
{
    public class Area
    {
        public int ID_Area { get; set; }
        public string Nombre_Area { get; set; } = string.Empty;
        public int Jefe_Area { get; set; }
        public string? Codigo_Area { get; set; }
        public string? Jefe_Nombre { get; set; }

    }
}
