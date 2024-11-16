using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVS.Otel.APIContagem.Data.Mapping
{
    public class HistoricoContagemMapping : IEntityTypeConfiguration<HistoricoContagem>
    {
        public void Configure(EntityTypeBuilder<HistoricoContagem> builder)
        {
            builder.ToTable("HistoricoContagem");
            builder.HasKey(c => c.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            // Coluna DataProcessamento do tipo TIMESTAMP
            builder.Property(e => e.DataProcessamento)
                .IsRequired()
                .HasColumnType("TIMESTAMP");

            // Coluna ValorAtual do tipo INT
            builder.Property(e => e.ValorAtual)
                .IsRequired();

            // Coluna Producer com tamanho máximo de 120 caracteres
            builder.Property(e => e.Producer)
                .IsRequired()
                .HasMaxLength(120);

            // Coluna Kernel com tamanho máximo de 80 caracteres
            builder.Property(e => e.Kernel)
                .IsRequired()
                .HasMaxLength(80);

            // Coluna Framework com tamanho máximo de 80 caracteres
            builder.Property(e => e.Framework)
                .IsRequired()
                .HasMaxLength(80);

            // Coluna Mensagem com tamanho máximo de 500 caracteres
            builder.Property(e => e.Mensagem)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
