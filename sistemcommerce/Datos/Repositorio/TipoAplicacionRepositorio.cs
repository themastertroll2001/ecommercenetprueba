using sistemcommerce.Datos.Repositorio.IRepositorio;
using sistemcommerce.Models;

namespace sistemcommerce.Datos.Repositorio
{
    public class TipoAplicacionRepositorio : Repositorio<TipoAplicacion>, ITipoAplicacionRepositorio
    {
        private readonly ApplicationDbContext _db;
        public TipoAplicacionRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Actualizar(TipoAplicacion tipoAplicacion)
        {
            var tipoAnterior = _db.TipoAplicacion.FirstOrDefault(c => c.Id == tipoAplicacion.Id);
            if (tipoAnterior != null)
            {
                tipoAnterior.Nombre = tipoAplicacion.Nombre;
            }
        }
    }
}
