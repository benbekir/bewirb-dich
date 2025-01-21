using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreepyApi.Database.Models;

public class DokumentConfiguration : IEntityTypeConfiguration<Dokument>
{
    public void Configure(EntityTypeBuilder<Dokument> builder)
    {
        builder.ToTable("dokumente")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(x => x.Uuid)
            .HasColumnName("uuid")
            .HasColumnType("BINARY")
            .IsRequired();

        builder.Property(x => x.Berechnungbasis)
            .HasColumnName("berechnungsbasis")
            .HasColumnType("decimal")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.InkludiereZusatzschutz)
            .HasColumnName("inkludiere_zusatzschutz")
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(x => x.ZusatzschutzAufschlag)
            .HasColumnName("zusatzschutz_aufschlag")
            .HasColumnType("float")
            .IsRequired();

        builder.Property(x => x.HatWebshop)
            .HasColumnName("hat_webshop")
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(x => x.Beitrag)
            .HasColumnName("beitrag")
            .HasColumnType("decimal")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.VersicherungsscheinAusgestellt)
            .HasColumnName("versicherungsschein_ausgestellt")
            .HasColumnType("boolean")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.Versicherungssumme)
            .HasColumnName("versicherungssumme")
            .HasColumnType("decimal")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.Typ)
            .HasColumnName("dokument_typ")
            .HasConversion(
                value => value.ToString(),
                value => Enum.Parse<Dokumenttyp>(value))
            .IsRequired();

        builder.Property(x => x.Berechnungsart)
            .HasColumnName("berechnungsart")
            .HasConversion(
                value => value.ToString(),
                value => Enum.Parse<Berechnungsart>(value))
            .IsRequired();

        builder.Property(x => x.Risiko)
            .HasColumnName("risiko")
            .HasConversion(
                value => value.ToString(),
                value => Enum.Parse<Risiko>(value))
            .IsRequired();
    }
}
