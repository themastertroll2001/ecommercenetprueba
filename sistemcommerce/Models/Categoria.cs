using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace sistemcommerce.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Nombre de categoria obligatorio. ")]
        public string NombreCategoria { get; set; }

        [Required(ErrorMessage ="Orden es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage ="El orden debe ser Mayor a 0")]
        public int MostrarOrden { get; set; }
    }
}
