using sistemcommerce.Models;

namespace sistemcommerce.Datos.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio :  IRepositorio<Categoria>
    {
        void Actualizar(Categoria categoria);
    }
}
