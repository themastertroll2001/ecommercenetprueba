﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sistemcommerce.Datos;
using sistemcommerce.Datos.Repositorio.IRepositorio;
using sistemcommerce.Models;
using sistemcommerce.Utilidades;

namespace sistemcommerce.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _catRepo;
        public CategoriaController(ICategoriaRepositorio catRepo)
        {
          _catRepo = catRepo;
        }
        public IActionResult Index()
        {
            IEnumerable<Categoria> lista = _catRepo.ObtenerTodos();

            return View(lista);
        }
        //get
        public IActionResult Crear()
        {
   return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
          if(ModelState.IsValid)
            {
                _catRepo.Agregar(categoria);
                _catRepo.Grabar();
                TempData[WC.Exitosa] = "Categoria creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al crear nueva categoria";
          return View(categoria);
          
        }
        //get editar
        public IActionResult Editar(int? Id)
        {
          if (Id == null || Id== 0) { 
            return NotFound();
            }
            var obj = _catRepo.Obtener(Id.GetValueOrDefault());
            if(obj == null){
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Actualizar(categoria);
                _catRepo.Grabar();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);

        }
        //get eliminar
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _catRepo.Obtener(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Categoria categoria)
        {
            if (categoria == null)
            {
                return NotFound();
            }
            _catRepo.Remover(categoria);
            _catRepo.Grabar();
            return RedirectToAction(nameof(Index));
          

        }


    }
}
