using Infnet.PesqMgm.Domain.Pesquisas;
using Microsoft.EntityFrameworkCore;

namespace Infnet.PesqMgm.Infrastructure.Data;

public class PesquisaDbContext : DbContext
{
    public PesquisaDbContext(DbContextOptions<PesquisaDbContext> options) : base(options)
    {
    }

    public DbSet<Pesquisa> Pesquisas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    // Respostas e Resultados podem ser adicionados aqui posteriormente

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            
            // Shadow Property para ID (já que não existe na classe de domínio)
            entity.Property<Guid>("Id").ValueGeneratedOnAdd();
            entity.HasKey("Id");

            entity.Property(u => u.Nome).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Perfil).HasConversion<string>();

            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Mapeamento de Pesquisa
        modelBuilder.Entity<Pesquisa>(entity =>
        {
            entity.ToTable("Pesquisas");
            
            entity.Property<Guid>("Id").ValueGeneratedOnAdd();
            entity.HasKey("Id");

            entity.Property(p => p.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Descricao).HasMaxLength(1000);
            entity.Property(p => p.Status).HasConversion<string>();

            // Relacionamento com Gestor
            entity.HasOne(p => p.Gestor).WithMany().IsRequired();

            // Mapeamento de Perguntas (Owned Entity - parte da Pesquisa)
            entity.OwnsMany(p => p.Perguntas, pergunta =>
            {
                pergunta.ToTable("Perguntas");
                pergunta.Property<Guid>("Id").ValueGeneratedOnAdd();
                pergunta.HasKey("Id");
                
                pergunta.Property(p => p.Texto).IsRequired();

                // Mapeamento da lista de strings (Opcoes) para JSON ou tabela separada (EF Core 8+)
                pergunta.PrimitiveCollection(p => p.Opcoes);
            });
        });
    }
}
