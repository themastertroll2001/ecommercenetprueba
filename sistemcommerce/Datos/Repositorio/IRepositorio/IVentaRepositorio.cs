using sistemcommerce.Models;

namespace sistemcommerce.Datos.Repositorio.IRepositorio
{
    public interface IVentaRepositorio : IRepositorio<Venta>
    {
        void Actualizar(Venta venta);
    }
}
