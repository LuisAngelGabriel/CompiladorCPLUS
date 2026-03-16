using System.ComponentModel.DataAnnotations;

namespace CompiladorCPLUS.Models
{
    public class Compilacion
    {
        [Key]
        public int CompilacionId { get; set; } 

        
        [Required(ErrorMessage = "El código fuente es requerido")] 
        public string Codigo { get; set; } = null!;

        public DateTime Fecha { get; set; } = DateTime.Now;

        public bool TieneErrores { get; set; }

        public string? ResultadoAnalisis { get; set; }
    }
}
