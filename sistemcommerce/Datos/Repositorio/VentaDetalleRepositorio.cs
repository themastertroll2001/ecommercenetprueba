using sistemcommerce.Datos.Repositorio.IRepositorio;
using sistemcommerce.Models;

namespace sistemcommerce.Datos.Repositorio
{
    public class VentaDetalleRepositorio : Repositorio<VentaDetalle>, IVentaDetalleRepositorio
    {
        private readonly ApplicationDbContext _db;
        public VentaDetalleRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void actualizar(VentaDetalle ventaDetalle)
        {
            _db.Update(ventaDetalle);
        }
    }
}
