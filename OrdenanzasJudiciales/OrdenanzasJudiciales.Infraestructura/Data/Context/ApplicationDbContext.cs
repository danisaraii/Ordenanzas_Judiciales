using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using OrdenanzasJudiciales.Dominio.Entidades;

namespace OrdenanzasJudiciales.Infraestructura.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<procesosOrdenanzas>().HasNoKey().ToView(null);
        }


        public DbSet<procesosOrdenanzas> procesosOrdenanzas { get; set; }

        // Puedes agregar más DbSet si tienes otras entidades
    }
}
