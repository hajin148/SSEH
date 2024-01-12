using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Models;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Data
{
    public class AutomatedEducationProgramContext : DbContext
    {
        public AutomatedEducationProgramContext(DbContextOptions<AutomatedEducationProgramContext> options) : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>().ToTable("Note");
        }
    }
}
