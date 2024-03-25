using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sistemcommerce.Datos;
using sistemcommerce.Datos.Repositorio.IRepositorio;
using sistemcommerce.Models;
using sistemcommerce.Models.ViewModels;
using sistemcommerce.Utilidades;
using System.Security.Claims;
using System.Text;

namespace sistemcommerce.Controllers
{
    [Authorize]
    public class CarroController : Controller
    {
        private readonly IProductoRepositorio _productoRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUsuarioAplicacionRepositorio _usuarioRepo;
        private readonly IOrdenRepositorio _ordenRepo;
        private readonly IOrdenDetalleRepositorio _ordenDetalleRepo;
        private readonly IVentaRepositorio _ventaRepo;
        private readonly IVentaDetalleRepositorio _ventaDetalleRepo;

        [BindProperty]
        public ProductoUsuarioVM productoUsuarioVM {  get; set; }
        public CarroController(IProductoRepositorio productoRepo, IWebHostEnvironment webHostEnvironment, 
            IUsuarioAplicacionRepositorio usuarioRepo, IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo,
            IVentaRepositorio ventaRepo, IVentaDetalleRepositorio ventaDetalleRepo)
        {
            _productoRepo = productoRepo;
            _webHostEnvironment = webHostEnvironment;
            _usuarioRepo = usuarioRepo;
            _ordenRepo = ordenRepo;
            _ordenDetalleRepo = ordenDetalleRepo;
            _ventaRepo = ventaRepo;
            _ventaDetalleRepo = ventaDetalleRepo;

        }
        public IActionResult Index()
        {
            List<CarroCompra> carroCompraList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras)!= null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count()>0)
            {
                carroCompraList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            List<int> prodEnCarro = carroCompraList.Select(i=>i.ProductoId).ToList();
            //IEnumerable<Producto> prodList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            IEnumerable<Producto> prodList = _productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            List<Producto> prodListFinal = new List<Producto>();
            foreach(var objCarro in carroCompraList)
            {
                Producto prodtemp = prodList.FirstOrDefault(p => p.Id == objCarro.ProductoId);
                prodtemp.TempMetroCuadrado = objCarro.MetroCuadrado;
                prodListFinal.Add(prodtemp);
            }
            return View(prodListFinal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Producto> prodLista)
        {
            List<CarroCompra> carroCompraLista = new List<CarroCompra>();
            foreach (Producto prod in prodLista)
            {
                carroCompraLista.Add(new CarroCompra
                {
                    ProductoId = prod.Id,
                    MetroCuadrado = prod.TempMetroCuadrado
                });
            }
            HttpContext.Session.Set(WC.SessionCarroCompras, carroCompraLista);
            return RedirectToAction(nameof(Resumen));
        }
        public IActionResult Resumen ()
        {
            UsuarioAplicacion usuarioAplicacion;
            if (User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionOrdenId) != 0)
                {
                    Orden orden = _ordenRepo.ObtenerPrimero(u=>u.Id == HttpContext.Session.Get<int>(WC.SessionOrdenId));
                    usuarioAplicacion = new UsuarioAplicacion()
                    {
                        Email = orden.Email,
                        NombreCompleto = orden.NombreCompleto,
                        PhoneNumber = orden.Telefono
                    };
                }else
                { // si no pertenece a una orden
                    usuarioAplicacion = new UsuarioAplicacion();
                }
            }else
            {
                //traer usuario loggeado
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                usuarioAplicacion = _usuarioRepo.ObtenerPrimero(u => u.Id == claim.Value);
            }
          
            //lista productos y usuario
            List<CarroCompra> carroCompraList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroCompraList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
            List<int> prodEnCarro = carroCompraList.Select(i => i.ProductoId).ToList();
            //IEnumerable<Producto> prodList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            IEnumerable<Producto> prodList = _productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            productoUsuarioVM = new ProductoUsuarioVM()
            {
                // UsuarioAplicacion = _db.UsuarioAplicacion.FirstOrDefault(u => u.Id == claim.Value),
                UsuarioAplicacion = usuarioAplicacion,
              
            };
            foreach(var carro in carroCompraList)
            {
                Producto prodTemp = _productoRepo.ObtenerPrimero(p=>p.Id == carro.ProductoId);
                prodTemp.TempMetroCuadrado = carro.MetroCuadrado;
                productoUsuarioVM.ProductoLista.Add(prodTemp);
            }
            return View(productoUsuarioVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")]
        public async Task<IActionResult> ResumenPost(ProductoUsuarioVM productoUsuarioVM)
        {
            //captura de user conectado
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole(WC.AdminRole))
            {
                //crear venta
                Venta venta = new Venta()
                {
                    CreadoPorUsusarioId = claim.Value,
                    FinalVentaTotal = productoUsuarioVM.ProductoLista.Sum(x=>x.TempMetroCuadrado*x.Precio),
                    Direccion = productoUsuarioVM.UsuarioAplicacion.Direccion,
                    Ciudad = productoUsuarioVM.UsuarioAplicacion.Ciudad,
                    Telefono = productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                    NombreCompleto = productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                    FechaVenta = DateTime.Now,
                    EstadoVenta = WC.EstadoPendiente
                };
                _ventaRepo.Agregar(venta);
                _ventaRepo.Grabar();
                foreach (var prod in productoUsuarioVM.ProductoLista)
                {
                    VentaDetalle ventaDetalle = new VentaDetalle()
                    {

                    }
                }
            }else
            {
                //crear orden
                var rutaTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
           + "templates" + Path.DirectorySeparatorChar.ToString()
           + "PlantillaOrden.html";

                var subject = "Nueva Orden";
                string HtmlBody = "";

                using (StreamReader sr = System.IO.File.OpenText(rutaTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }
                // nombre: {0} email: {1} telefono {2} productos {3}

                StringBuilder productoListaSB = new StringBuilder();
                foreach (var prod in productoUsuarioVM.ProductoLista)
                {
                    productoListaSB.Append($" - Nombre: {prod.NombreProducto} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");

                }
                string messageBody = string.Format(HtmlBody,
                    productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                    productoUsuarioVM.UsuarioAplicacion.Email,
                    productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                    productoListaSB.ToString());

                //grabar orden y detalle bd
                Orden orden = new Orden()
                {
                    UsuarioAplicacionId = claim.Value,
                    NombreCompleto = productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                    Email = productoUsuarioVM.UsuarioAplicacion.Email,
                    Telefono = productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                    FechaOrden = DateTime.Now
                };
                _ordenRepo.Agregar(orden);
                _ordenRepo.Grabar();
                foreach (var prod in productoUsuarioVM.ProductoLista)
                {
                    OrdenDetalle ordenDetalle = new OrdenDetalle()
                    {
                        OrdenId = orden.Id,
                        ProductoId = prod.Id
                    };
                    _ordenDetalleRepo.Agregar(ordenDetalle);
                }
                _ordenDetalleRepo.Grabar();
            }
           

            return RedirectToAction(nameof(Confirmacion));
        }

        public  IActionResult Confirmacion()
        {
            HttpContext.Session.Clear();
            return View();

        }

        public IActionResult Remover(int Id)
        {
            List<CarroCompra> carroCompraList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroCompraList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }
          carroCompraList.Remove(carroCompraList.FirstOrDefault(p=>p.ProductoId==Id));
            HttpContext.Session.Set(WC.SessionCarroCompras, carroCompraList);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCarro(IEnumerable<Producto>prodLista)
        {
            List<CarroCompra> carroCompraLista = new List<CarroCompra>();
            foreach (Producto prod in prodLista)
            {
                carroCompraLista.Add(new CarroCompra
                {
                    ProductoId = prod.Id,
                    MetroCuadrado = prod.TempMetroCuadrado
                });
            }
            HttpContext.Session.Set(WC.SessionCarroCompras, carroCompraLista);
            return RedirectToAction(nameof(Index));
        }
    }
}
