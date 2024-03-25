using Microsoft.AspNetCore.Mvc.Rendering;
using sistemcommerce.Models;

namespace sistemcommerce.Datos.Repositorio.IRepositorio
{
    public interface IProductoRepositorio : IRepositorio<Producto>
    {
        void Actualizar(Producto producto);

        IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj);
    }
}
