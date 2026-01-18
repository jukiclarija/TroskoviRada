using Microsoft.EntityFrameworkCore;
using TroskoviRada.Models;

namespace TroskoviRada.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }

        // DbSet predstavlja tablice u bazi
        public DbSet<Zaposlenik> Zaposlenici { get; set; }
        public DbSet<RadnoMjesto> RadnaMjesta { get; set; }
        public DbSet<Zaposlenje> Zaposlenja { get; set; }
        public DbSet<Prisustvo> Prisustva { get; set; }
        public DbSet<Odsustvo> Odsustva { get; set; }
        public DbSet<Smjena> Smjene { get; set; }
        public DbSet<Obračun> Obračuni { get; set; }
        public DbSet<TipNaknade> TipoviNaknada { get; set; }
        public DbSet<TipOdsustva> TipoviOdsustva { get; set; }
        public DbSet<Naknada> Naknade { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL specifičnosti za decimalne tipove
            modelBuilder.Entity<RadnoMjesto>()
                .Property(r => r.Satnica)
                .HasColumnType("numeric(10,2)");

            modelBuilder.Entity<Obračun>()
                .Property(o => o.Osnovica)
                .HasColumnType("numeric(12,2)");

            modelBuilder.Entity<Obračun>()
                .Property(o => o.UkupneNaknade)
                .HasColumnType("numeric(12,2)");

            modelBuilder.Entity<Obračun>()
                .Property(o => o.NetoPlaca)
                .HasColumnType("numeric(12,2)");

            modelBuilder.Entity<Obračun>()
                .Property(o => o.Porez)
                .HasColumnType("numeric(12,2)");

            modelBuilder.Entity<Obračun>()
                .Property(o => o.BrutoPlaca)
                .HasColumnType("numeric(12,2)");

            modelBuilder.Entity<TipNaknade>()
                .Property(t => t.Iznos)
                .HasColumnType("numeric(10,2)");

            modelBuilder.Entity<Naknada>()
                .Property(n => n.Iznos)
                .HasColumnType("numeric(10,2)");

            modelBuilder.Entity<Prisustvo>()
                .Property(p => p.UkupnoOdradeno)
                .HasColumnType("numeric(5,2)");

            modelBuilder.Entity<Smjena>()
                .Property(s => s.KoeficijentPlacanja)
                .HasColumnType("numeric(4,2)");

            modelBuilder.Entity<TipOdsustva>()
                .Property(t => t.KoeficijentIsplate)
                .HasColumnType("numeric(3,2)");

            // PostgreSQL specifičnosti za DATE i TIME tipove
            modelBuilder.Entity<Prisustvo>()
                .Property(p => p.Datum)
                .HasColumnType("date");

            modelBuilder.Entity<Prisustvo>()
                .Property(p => p.VrijemeDolaska)
                .HasColumnType("time");

            modelBuilder.Entity<Prisustvo>()
                .Property(p => p.VrijemeOdlaska)
                .HasColumnType("time");

            modelBuilder.Entity<Odsustvo>()
                .Property(o => o.DatumOd)
                .HasColumnType("date");

            modelBuilder.Entity<Odsustvo>()
                .Property(o => o.DatumDo)
                .HasColumnType("date");

            modelBuilder.Entity<Zaposlenje>()
                .Property(z => z.DatumOd)
                .HasColumnType("date");

            modelBuilder.Entity<Zaposlenje>()
                .Property(z => z.DatumDo)
                .HasColumnType("date");

            modelBuilder.Entity<Obračun>()
                .Property(o => o.DatumObračuna)
                .HasColumnType("date");

            modelBuilder.Entity<Naknada>()
                .Property(n => n.Datum)
                .HasColumnType("date");

            // KONFIGURACIJA TABLICA
            modelBuilder.Entity<Zaposlenik>().ToTable("zaposlenik");
            modelBuilder.Entity<RadnoMjesto>().ToTable("radno_mjesto");
            modelBuilder.Entity<Zaposlenje>().ToTable("zaposlenje");
            modelBuilder.Entity<Prisustvo>().ToTable("evidencija_rada");
            modelBuilder.Entity<Odsustvo>().ToTable("odsustvo");
            modelBuilder.Entity<Smjena>().ToTable("smjena");
            modelBuilder.Entity<Obračun>().ToTable("obracun");
            modelBuilder.Entity<TipNaknade>().ToTable("tip_naknade");
            modelBuilder.Entity<TipOdsustva>().ToTable("tip_odsustva");
            modelBuilder.Entity<Naknada>().ToTable("naknada");

            // PostgreSQL koristi snake_case za imena ograničenja
            modelBuilder.Entity<Zaposlenje>()
                .HasOne(z => z.Zaposlenik)
                .WithMany(z => z.Zaposlenja)
                .HasForeignKey(z => z.IdZaposlenik)
                .HasConstraintName("fk_zaposlenje_zaposlenik")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Zaposlenje>()
                .HasOne(z => z.RadnoMjesto)
                .WithMany()
                .HasForeignKey(z => z.IdRadnoMjesto)
                .HasConstraintName("fk_zaposlenje_radno_mjesto")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prisustvo>()
                .HasOne(p => p.Zaposlenik)
                .WithMany(z => z.Prisustva)
                .HasForeignKey(p => p.IdZaposlenik)
                .HasConstraintName("fk_evidencija_rada_zaposlenik")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Odsustvo>()
                .HasOne(o => o.Zaposlenik)
                .WithMany()
                .HasForeignKey(o => o.IdZaposlenik)
                .HasConstraintName("fk_odsustvo_zaposlenik")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Odsustvo>()
                .HasOne(o => o.TipOdsustva)
                .WithMany()
                .HasForeignKey(o => o.IdTipOdsustva)
                .HasConstraintName("fk_odsustvo_tip_odsustva")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Obračun>()
                .HasOne(o => o.Zaposlenik)
                .WithMany(z => z.Obračuni)
                .HasForeignKey(o => o.IdZaposlenik)
                .HasConstraintName("fk_obracun_zaposlenik")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Naknada>()
                .HasOne(n => n.Zaposlenik)
                .WithMany()
                .HasForeignKey(n => n.IdZaposlenik)
                .HasConstraintName("fk_naknada_zaposlenik")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Naknada>()
                .HasOne(n => n.TipNaknade)
                .WithMany()
                .HasForeignKey(n => n.IdTipNaknade)
                .HasConstraintName("fk_naknada_tip_naknade")
                .OnDelete(DeleteBehavior.Restrict);

            // Kompozitni ključ za many-to-many vezu između Zaposlenik i TipNaknade
            // Ovo je zamjena za tablicu Zaposlenik_Naknada iz ERA dijagrama
            modelBuilder.Entity<Naknada>()
                .HasKey(n => n.IdNaknada);

            // OVO JE NOVO - za temporalne tablice
            // Konfiguracija za temporalne tablice (historijske verzije)
            modelBuilder.Entity<Zaposlenik>()
                .ToTable("zaposlenik", tb => tb.HasTrigger("trg_zaposlenik_history"));

            modelBuilder.Entity<RadnoMjesto>()
                .ToTable("radno_mjesto", tb => tb.HasTrigger("trg_radno_mjesto_history"));

            modelBuilder.Entity<Prisustvo>()
                .ToTable("evidencija_rada", tb => tb.HasTrigger("trg_evidencija_rada_history"));

            modelBuilder.Entity<Obračun>()
                .ToTable("obracun", tb => tb.HasTrigger("trg_obracun_history"));

            // Konfiguracija za automatsko generiranje ID-eva (PostgreSQL SERIAL)
            modelBuilder.Entity<Zaposlenik>()
                .Property(z => z.IdZaposlenik)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<RadnoMjesto>()
                .Property(r => r.IdRadnoMjesto)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<Zaposlenje>()
                .Property(z => z.IdZaposlenje)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<Prisustvo>()
                .Property(p => p.IdPrisustvo)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<Odsustvo>()
                .Property(o => o.IdOdsustvo)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<Smjena>()
                .Property(s => s.IdSmjena)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<Obračun>()
                .Property(o => o.IdObračun)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<TipNaknade>()
                .Property(t => t.IdTipNaknade)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<TipOdsustva>()
                .Property(t => t.IdTipOdsustva)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            modelBuilder.Entity<Naknada>()
                .Property(n => n.IdNaknada)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();
        }
    }
}