﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;
using sib_api_v3_sdk.Model;
using sistemcommerce.Datos.Repositorio.IRepositorio;
using System.Linq.Expressions;

namespace sistemcommerce.Datos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Agregar(T entidad)
        {
           dbSet.Add(entidad);
        }

        public void Grabar()
        {
            _db.SaveChanges();
        }

        public T Obtener(int id)
        {
            return dbSet.Find(id);
        }

        public T ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirprop in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirprop); //ejemplo categoria , tipoaplicacion

                }
            }   
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> ObtenerTodos(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if(filtro != null)
            {
                query = query.Where(filtro);
            }
            if(incluirPropiedades != null)
            {
                foreach (var incluirprop in incluirPropiedades.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirprop); //ejemplo categoria , tipoaplicacion

                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);

            }
            if (!isTracking)
            {
                query = query.AsNoTracking();
            }
            return query.ToList();  
        }

        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);

        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }
    }
}
