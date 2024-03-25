using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistemcommerce.Models
{
    public class Producto
    {
        public Producto()
        {
            TempMetroCuadrado = 1;
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Nombre de producto es requerido")]
        public string NombreProducto { get; set; }
        [Required(ErrorMessage ="Descripcion corta es requerida")]
        public string DescripcionCorta { get; set; }
        [Required(ErrorMessage ="Descripcion de producto es requerida")]
        public string DescripcionProducto { get; set; }
        [Required(ErrorMessage ="El precio de producto es requerido")]
        [Range(1, double.MaxValue, ErrorMessage ="El precio debe ser mayor a cero")]
        public double Precio { get; set; }

        public string? ImagenUrl { get; set; }

        //llaves foraneas

        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }
        public int TipoAplicacionId { get; set; }

        [ForeignKey("TipoAplicacionId")]
        public virtual TipoAplicacion? TipoAplicacion { get; set; }
        [NotMapped]// LA PROPIEDA NO SE AGREGA EN LA BD
        [Range(1, 10000)]
        public int TempMetroCuadrado { get; set; }
    }
}
