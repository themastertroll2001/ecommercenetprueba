using sistemcommerce.Datos.Repositorio.IRepositorio;
using sistemcommerce.Models;

namespace sistemcommerce.Datos.Repositorio
{
    public class VentaRepositorio : Repositorio<Venta>, IVentaRepositorio
    {
        private readonly ApplicationDbContext _db;
        public VentaRepositorio(ApplicationDbContext db) : base(db) { 
         _db = db;
        }

        public void Actualizar(Venta venta)
        {
            _db.Update(venta);
        }
    }
}
