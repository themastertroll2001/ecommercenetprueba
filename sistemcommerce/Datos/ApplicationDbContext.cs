﻿
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sistemcommerce.Models;

namespace sistemcommerce.Datos
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<TipoAplicacion> TipoAplicacion { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<UsuarioAplicacion> UsuarioAplicacion { get; set; }
        public DbSet<Orden> Orden { get; set; }
        public DbSet<OrdenDetalle> OrdenDetalle { get; set; }
        public DbSet<Venta> Venta { get; set; }
        public DbSet<VentaDetalle> VentaDetalle { get; set; }

    }
}
