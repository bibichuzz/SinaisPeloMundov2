using Microsoft.EntityFrameworkCore;
using SinaisPeloMundo.Models;

namespace SinaisPeloMundo.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options)
            : base(options) { }

        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<PassagemModel> Passagens { get; set; }
        public DbSet<ReservaHotelModel> Hoteis { get; set; }
        public DbSet<InterpreteModel> Interpretes { get; set; }
        public DbSet<PacoteModel> Pacotes { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PassagemModel>()
                .Property(p => p.Transporte)
                .HasConversion<string>();

            modelBuilder.Entity<PassagemModel>()
                .Property(p => p.TipoPassagem)
                .HasConversion<string>();

            modelBuilder.Entity<PedidoModel>()
                .Property(p => p.FormaPagamento)
                .HasConversion<string>();

            modelBuilder.Entity<ClienteModel>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<ClienteModel>()
                .HasIndex(c => c.Cpf)
                .IsUnique();

            modelBuilder.Entity<InterpreteModel>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<InterpreteModel>()
                .HasIndex(c => c.Cpf)
                .IsUnique();

            // RELACIONAMENTO 1:1
            // Pacote -> Passagem

            modelBuilder.Entity<PacoteModel>()
                .HasOne(p => p.Passagem)
                .WithOne(pa => pa.Pacote)
                .HasForeignKey<PacoteModel>(p => p.PassagemId);

            // RELACIONAMENTO 1:1
            // Pacote -> ReservaHotel

            modelBuilder.Entity<PacoteModel>()
                .HasOne(p => p.ReservaHotel)
                .WithOne(r => r.Pacote)
                .HasForeignKey<PacoteModel>(p => p.ReservaHotelId);

            // RELACIONAMENTO N:1
            // Muitos pacotes para um intérprete

            modelBuilder.Entity<PacoteModel>()
                .HasOne(p => p.Interprete)
                .WithMany(i => i.Pacotes)
                .HasForeignKey(p => p.InterpreteId);
        }
    }
}