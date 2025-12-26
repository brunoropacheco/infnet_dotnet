using Infnet.PesqMgm.Domain.Pesquisas;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infnet.PesqMgm.Infrastructure.Data;

public class PesquisaDbContext : DbContext
{
    public PesquisaDbContext(DbContextOptions<PesquisaDbContext> options) : base(options)
    {
    }

    public DbSet<Pesquisa> Pesquisas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

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
        modelBuilder.Entity<Pesquisa>(e =>
        {
            e.HasKey(e => e.Id);
            e.OwnsMany(e => e.Perguntas, p =>
            {
                p.ToTable("Perguntas");
                p.Property<Guid>("Id").ValueGeneratedOnAdd();
                p.HasKey("Id");
                p.Property(x => x.Texto).IsRequired();
                
                // Mapeia a lista de strings para JSON
                p.Property(x => x.Opcoes).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                );
            });

            e.OwnsMany(e => e.Respostas, r =>
            {
                r.ToTable("Respostas");
                r.WithOwner(x => x.Pesquisa);
                r.Property<Guid>("Id").ValueGeneratedOnAdd();
                r.HasKey("Id");

                r.OwnsMany(x => x.Itens, i =>
                {
                    i.ToTable("RespostaItens");
                    i.WithOwner().HasForeignKey("RespostaId");
                    i.Property<Guid>("Id").ValueGeneratedOnAdd();
                    i.HasKey("Id");
                    i.Property(x => x.OpcaoSelecionada);
                });
            });

            e.HasOne(e => e.Gestor);
            e.Property(e => e.Status).HasConversion<string>();

            // Alterado de OwnsOne para HasOne para permitir relacionamento N:N
            e.HasOne(e => e.ResultadoSumarizado)
                .WithOne(r => r.Pesquisa)
                .HasForeignKey<ResultadoSumarizado>("PesquisaId");
        });

        // Mapeamento de ResultadoSumarizado como Entidade
        modelBuilder.Entity<ResultadoSumarizado>(r =>
        {
            r.ToTable("ResultadosSumarizados");
            r.Property<Guid>("PesquisaId");
            r.HasKey("PesquisaId"); // PK é a mesma FK da Pesquisa (1:1)

            r.Property(x => x.DataApuracao);
            r.Property(x => x.ContagemVotos).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, Dictionary<string, int>>()
            );

            r.HasMany(x => x.Leitores)
                .WithMany()
                .UsingEntity("ResultadoLeitores");
        });
    }
}
