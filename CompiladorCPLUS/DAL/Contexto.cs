using CompiladorCPLUS.Models;
using Microsoft.EntityFrameworkCore;

namespace CompiladorCPLUS.DAL
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        public DbSet<Compilacion> Compilaciones { get; set; }
    }
}
