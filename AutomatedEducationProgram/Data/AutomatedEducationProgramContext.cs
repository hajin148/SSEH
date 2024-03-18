using AutomatedEducationProgram.Areas.Data;
using AutomatedEducationProgram.Models;
using Microsoft.EntityFrameworkCore;

namespace AutomatedEducationProgram.Data
{
    public class AutomatedEducationProgramContext : DbContext
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DbSet<AEPUser> AEPUsers { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<VocabularyWord> VocabularyWords { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }

        public DbSet<Following> Followings { get; set; }

        public DbSet<DocumentText> DocumentTexts { get; set; }

        public AutomatedEducationProgramContext(DbContextOptions<AutomatedEducationProgramContext> options, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AEPUser>().ToTable("AEPUser");
            modelBuilder.Entity<Note>().ToTable("Note");
            modelBuilder.Entity<VocabularyWord>().ToTable("VocabularyWord");
        }
    }
}
