using sistemcommerce.Models;

namespace sistemcommerce.Datos.Repositorio.IRepositorio
{
    public interface IVentaDetalleRepositorio : IRepositorio<VentaDetalle>
    {
        void actualizar(VentaDetalle ventaDetalle);
    }
}
