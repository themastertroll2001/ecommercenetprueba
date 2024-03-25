using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemcommerce.Datos;
using sistemcommerce.Datos.Repositorio.IRepositorio;
using sistemcommerce.Models;
using sistemcommerce.Models.ViewModels;
using sistemcommerce.Utilidades;

namespace sistemcommerce.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductoController : Controller
    {
        private readonly IProductoRepositorio _productoRepo;
        //para imagen
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(IProductoRepositorio productoRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productoRepo = productoRepo;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //listar y traer datos de categoria y tipo aplicacion 
            //   IEnumerable<Producto> lista = _db.Producto.Include(c=>c.Categoria).Include(t=>t.TipoAplicacion);
            IEnumerable<Producto> lista = _productoRepo.ObtenerTodos(incluirPropiedades: "Categoria,TipoAplicacion");
            return View(lista);
        }
        //get
        public IActionResult Upsert(int? Id)
        {
     

            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                //CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                //{
                //    Text = c.NombreCategoria,
                //    Value = c.Id.ToString()
                //}),
                //TipoAplicacionLista = _db.TipoAplicacion.Select(c => new SelectListItem
                //{
                //    Text = c.Nombre,
                //    Value = c.Id.ToString()
                //})
                CategoriaLista = _productoRepo.ObtenerTodosDropdownList(WC.CategoriaNombre),
                TipoAplicacionLista = _productoRepo.ObtenerTodosDropdownList(WC.TipoAplicacionNombre)
            };

            if(Id == null)
            {
                //crea nuevo producto
                return View(productoVM);
            }else
            {
                productoVM.Producto = _productoRepo.Obtener(Id.GetValueOrDefault());
                if(productoVM.Producto == null)
                {
                    return  NotFound();
                }
                return View(productoVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if(productoVM.Producto.Id == 0)
                {
                    //crear img de prod
                    //Guid.NewGuid().ToString(); asigna id a la imagen que se graba
                    string upload = webRootPath + WC.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    _productoRepo.Agregar(productoVM.Producto);

                }else
                {
                    //actualizar prod
                    var objProducto = _productoRepo.ObtenerPrimero(p=>p.Id == productoVM.Producto.Id, isTracking:false);
                    if (files.Count > 0) //cargar nueva img
                    {
                        string upload = webRootPath + WC.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        //borrar img anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }//fin borrar img anterior

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    } // si no carga nueva img se mantiene img
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _productoRepo.Actualizar(productoVM.Producto);
                }
                _productoRepo.Grabar();
                return RedirectToAction("Index");
            }// si modelisvalid se llena nuevamenta las lista por si algo falla
             //productoVM.CategoriaLista = _db.Categoria.Select(c => new SelectListItem
             //{
             //    Text = c.NombreCategoria,
             //    Value = c.Id.ToString()
             //});
             //productoVM.TipoAplicacionLista = _db.TipoAplicacion.Select(c => new SelectListItem
             //{
             //    Text = c.Nombre,
             //    Value = c.Id.ToString()
             //});
            productoVM.CategoriaLista = _productoRepo.ObtenerTodosDropdownList(WC.CategoriaNombre);
            productoVM.TipoAplicacionLista = _productoRepo.ObtenerTodosDropdownList(WC.TipoAplicacionNombre);
            return View(productoVM);
        }
        //Get 
        public IActionResult Eliminar(int? Id)
        {
            if (Id==null || Id==0)
            {
                return NotFound();
            }
            Producto producto = _productoRepo.ObtenerPrimero(p => p.Id == Id, incluirPropiedades:"Categoria,TipoAplicacion");
            if(producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Producto producto)
        {
            if (producto == null)
            {
                return NotFound();
            } //eliminar img
            string upload = _webHostEnvironment.WebRootPath + WC.ImagenRuta;
            //borrar img anterior
            var anteriorFile = Path.Combine(upload, producto.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }//fin borrar img anterior
            _productoRepo.Remover(producto);
            _productoRepo.Grabar();
            return RedirectToAction(nameof(Index));

        }

    }
}
