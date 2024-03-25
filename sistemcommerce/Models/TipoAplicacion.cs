using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace sistemcommerce.Models
{
    public class TipoAplicacion
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombre es obligatorio. ")]
        public string Nombre{ get; set; }
       
    }
}
