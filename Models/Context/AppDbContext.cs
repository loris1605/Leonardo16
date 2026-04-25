using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class AppDbContext : BaseContext
    {
        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        //{

        //}
        //Add-Migration AddGiornata -Project Models -Context AppDbContext

        public DbSet<DbSettings> Settings { get; set; } = null!;
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Socio> Soci { get; set; } = null!;
        public DbSet<Tessera> Tessere { get; set; } = null!;
        public DbSet<Operatore> Operatori { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
        public DbSet<TipoRientro> TipiRientro { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<Permesso> Permessi { get; set; } = null!;
        public DbSet<TipoSettore> TipiSettore { get; set; } = null!;
        public DbSet<Settore> Settori { get; set; } = null!;
        public DbSet<Reparto> Reparti { get; set; } = null!;
        public DbSet<Tariffa> Tariffe { get; set; } = null!;
        public DbSet<Listino> Listini { get; set; } = null!;
        public DbSet<Giornata> Giornate { get; set; } = null!;
        public DbSet<Scheda> Schede { get; set; } = null!;
        public DbSet<Strisciata> Strisciate { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SettingsConfig(modelBuilder);
            PeopleConfig(modelBuilder);
            SociConfig(modelBuilder);
            TessereConfig(modelBuilder);
            OperatoriConfig(modelBuilder);
            TipoPostazioneConfig(modelBuilder);
            TipoRientroConfig(modelBuilder);
            PostazioneConfig(modelBuilder);
            PermessoConfig(modelBuilder);
            TipoSettoreConfig(modelBuilder);
            SettoreConfig(modelBuilder);
            RepartoConfig(modelBuilder);
            TariffaConfig(modelBuilder);
            ListinoConfig(modelBuilder);
            GiornataConfig(modelBuilder);
            SchedaConfig(modelBuilder);
            StrisciateConfig(modelBuilder);
        }


        private static void SettingsConfig(ModelBuilder modelBuilder)
        {
            //Configurazione People

            modelBuilder.Entity<DbSettings>().HasData(
                        new DbSettings { Id = -1, Version = 1, UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0) });

        }
        private static void PeopleConfig(ModelBuilder modelBuilder)
        {
            //Configurazione People

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.UniqueParam)
                .IsUnique();

            // Configura lunghezze massime per evitare nvarchar(max) inefficienti
            modelBuilder.Entity<Person>().Property(p => p.FirstName).HasMaxLength(50);
            modelBuilder.Entity<Person>().Property(p => p.SurName).HasMaxLength(50);
            modelBuilder.Entity<Person>().Property(p => p.UniqueParam).HasMaxLength(14);

            ////Relazione con Operatore
            //modelBuilder.Entity<Person>()
            //            .HasOne(p => p.Operatore)
            //            .WithOne(s => s.Person)
            //            .HasForeignKey<Operatore>(v => v.PersonId);

            modelBuilder.Entity<Person>().HasData(
                    new Person
                    {
                        Id = -1,
                        FirstName = "SOCIO",
                        SurName = "VIRTUALE",
                        Natoil = 21000101,
                        UniqueParam = "VIRSOC21000101"
                    },
                    new Person
                    {
                        Id = -999,
                        FirstName = "Di Servizio",
                        SurName = "Tessera",
                        Natoil = 21000101,
                        UniqueParam = "TESSER21000101"
                    },
                    new Person
                    {
                        Id = -2,
                        FirstName = "SOCIO",
                        SurName = "AMMINISTRATORE",
                        Natoil = 21000101,
                        UniqueParam = "AMMSOC21000101"
                    },
                    new Person
                    {
                        Id = -3,
                        FirstName = "Non Importato",
                        SurName = "Socio",
                        Natoil = 21000101,
                        UniqueParam = "SCOSOC21000101"
                    });
        }
        private static void SociConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Socio>().Property(s => s.NumeroSocio).HasMaxLength(20);

            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Socio>()
                        .HasOne(s => s.Person)          // Un socio ha una persona
                        .WithMany(p => p.Soci)    // Una persona ha molti soci
                        .HasForeignKey(s => s.PersonId) // La chiave è PersonId
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Socio>()
                .HasIndex(p => p.NumeroSocio)
                .IsUnique();

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.SurName)
                .HasDatabaseName("IX_Person_SurName"); // Opzionale: dà un nome specifico all'indice

            modelBuilder.Entity<Person>()
                .HasIndex(p => p.FirstName)
                .HasDatabaseName("IX_Person_FirstName"); // Opzionale: dà un nome specifico all'indice


        }
        private static void TessereConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tessera>().Property(s => s.NumeroTessera).HasMaxLength(20);

            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Tessera>()
                        .HasOne(s => s.Socio)          // Una tessera ha un socio
                        .WithMany(p => p.Tessere)    // Un Socio ha molte tessera
                        .HasForeignKey(s => s.SocioId) // La chiave è SocioId
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tessera>()
                .HasIndex(p => p.NumeroTessera)
                .IsUnique();


        }
        private static void OperatoriConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operatore>().Property(s => s.Nome).HasMaxLength(50);
            modelBuilder.Entity<Operatore>().Property(s => s.Password).HasMaxLength(50);

            modelBuilder.Entity<Operatore>().HasData(
                    new Operatore
                    {
                        Id = -1,
                        Nome = "ADMIN",
                        Password = "ADMIN",
                        Abilitato = true,
                        Pass = 0,
                        PersonId = -2
                    });

        }
        private static void TipoPostazioneConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoPostazione>()
                .Property(s => s.Nome).HasMaxLength(25);

            modelBuilder.Entity<TipoPostazione>()
                        .Property(p => p.Id)
                        .ValueGeneratedNever(); // DISATTIVA IDENTITY

            modelBuilder.Entity<TipoPostazione>().HasData(
                    new TipoPostazione
                    {
                        Id = 1,
                        Nome = "Amministratore"
                    },
                    new TipoPostazione
                    {
                        Id = 2,
                        Nome = "Cassa"
                    },
                    new TipoPostazione
                    {
                        Id = 3,
                        Nome = "Bar"
                    },
                    new TipoPostazione
                    {
                        Id = 4,
                        Nome = "Guardaroba"
                    },
                    new TipoPostazione
                    {
                        Id = 5,
                        Nome = "Pulizie"
                    });

        }
        private static void TipoRientroConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoRientro>()
                .Property(s => s.Nome).HasMaxLength(25);

            modelBuilder.Entity<TipoRientro>().HasData(
                    new TipoRientro
                    {
                        Id = -1,
                        Nome = "Nessuno",
                        DurataOre = 0
                    },
                    new TipoRientro
                    {
                        Id = -2,
                        Nome = "Giornata",
                        DurataOre = 0

                    });

        }
        private static void PostazioneConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Postazione>()
                .Property(s => s.Nome).HasMaxLength(25);

            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Postazione>()
                        .HasOne(s => s.TipoPostazione)          // Una Postazione ha un TipoPostazione
                        .WithMany(p => p.Postazioni)    // Un TipoPostazione ha molte Postazioni
                        .HasForeignKey(s => s.TipoPostazioneId) // La chiave è TipoPostazioneId
                        .OnDelete(DeleteBehavior.NoAction);

            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Postazione>()
                        .HasOne(s => s.TipoRientro)          // Una Postazione ha un TipoRientro
                        .WithMany(p => p.Postazioni)    // Un TipoRientro ha molte Postazioni
                        .HasForeignKey(s => s.TipoRientroId) // La chiave è TipoRientroId
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Postazione>().HasData(
                new Postazione
                {
                    Id = -1,
                    Nome = "Amministratore base",
                    TipoPostazioneId = 1,
                    TipoRientroId = -1
                });
            //    new Postazione
            //    {
            //        Nome = "Cassa base",
            //        TipoPostazioneId = 1,
            //        TipoRientroId = 0
            //    },
            //    new Postazione
            //    {
            //        Nome = "Bar base",
            //        TipoPostazioneId = 2,
            //        TipoRientroId = 0
            //    },
            //    new Postazione
            //    {
            //        Nome = "Guardaroba base",
            //        TipoPostazioneId = 3,
            //        TipoRientroId = 0
            //    },
            //    new Postazione
            //    {
            //        Nome = "Pulizie base",
            //        TipoPostazioneId = 4,
            //        TipoRientroId = 0
            //    });

        }
        private static void PermessoConfig(ModelBuilder modelBuilder)
        {
            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Permesso>()
                        .HasOne(s => s.Postazione)          // Una Permesso ha una Postazione
                        .WithMany(p => p.Permessi)    // Una Postazione ha molti Permessi
                        .HasForeignKey(s => s.PostazioneId) // La chiave è TipoPostazioneId
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Permesso>()
                        .HasOne(s => s.Operatore)          // Una Permesso ha un Operatore
                        .WithMany(p => p.Permessi)    // Un Operatore ha molti Permessi
                        .HasForeignKey(s => s.OperatoreId) // La chiave è OperatoreId
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Permesso>().HasData(
                new Permesso
                {
                    Id = -1,
                    OperatoreId = -1,
                    PostazioneId = -1
                });
            //new Permesso
            //{
            //    OperatoreId = 1,
            //    PostazioneId = 2
            //},
            //new Permesso
            //{
            //    OperatoreId = 1,
            //    PostazioneId = 3
            //},
            //new Permesso
            //{
            //    OperatoreId = 1,
            //    PostazioneId = 4
            //},
            //new Permesso
            //{
            //    OperatoreId = 1,
            //    PostazioneId = 5
            //});


        }
        private static void TipoSettoreConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoSettore>()
                .Property(s => s.Nome).HasMaxLength(25);

            modelBuilder.Entity<TipoSettore>().HasData(
                new TipoSettore
                {
                    Id = -1,
                    Nome = "Ingressi"
                },
                new TipoSettore
                {
                    Id = -2,
                    Nome = "Standard"
                });
        }
        private static void SettoreConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settore>()
                .Property(s => s.Nome).HasMaxLength(25);

            modelBuilder.Entity<Settore>()
                .Property(s => s.Label).HasMaxLength(25);

            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Settore>()
                        .HasOne(s => s.TipoSettore)          // Un Settore ha un TipoSettore
                        .WithMany(p => p.Settori)    // Un TipoSettore ha molti Settori
                        .HasForeignKey(s => s.TipoSettoreId) // La chiave è TipoPostazioneId
                        .OnDelete(DeleteBehavior.NoAction);


        }
        private static void RepartoConfig(ModelBuilder modelBuilder)
        {
            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Reparto>()
                        .HasOne(s => s.Postazione)          // Una Reparto ha una Postazione
                        .WithMany(p => p.Reparti)    // Una Postazione ha molti Reparti
                        .HasForeignKey(s => s.PostazioneId) // La chiave è PostazioneId
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reparto>()
                        .HasOne(s => s.Settore)          // Una Reparto ha un Settore
                        .WithMany(p => p.Reparti)    // Un Settore ha molti Reparti
                        .HasForeignKey(s => s.SettoreId) // La chiave è SettoreId
                        .OnDelete(DeleteBehavior.NoAction);


        }
        private static void TariffaConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tariffa>()
                .Property(s => s.Nome).HasMaxLength(25);

            modelBuilder.Entity<Tariffa>()
                .Property(s => s.Label).HasMaxLength(25);


        }
        private static void ListinoConfig(ModelBuilder modelBuilder)
        {
            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Listino>()
                        .HasOne(s => s.Tariffa)          // Una Listino ha una Tariffa
                        .WithMany(p => p.Listini)    // Una Tariffa ha molti Listini
                        .HasForeignKey(s => s.TariffaId) // La chiave è TariffaId
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Listino>()
                        .HasOne(s => s.Settore)          // Una Listino ha un Settore
                        .WithMany(p => p.Listini)    // Un Settore ha molti Listini
                        .HasForeignKey(s => s.SettoreId) // La chiave è SettoreId
                        .OnDelete(DeleteBehavior.NoAction);


        }
        private static void GiornataConfig(ModelBuilder modelBuilder)
        {

        }
        private static void SchedaConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Scheda>()
                .Property(s => s.NumeroTessera).HasMaxLength(30);

            modelBuilder.Entity<Scheda>()
                .Property(s => s.Cognome).HasMaxLength(30);
            
            modelBuilder.Entity<Scheda>()
                .Property(s => s.Nome).HasMaxLength(30);



            // EF Core capisce da solo la relazione, ma puoi essere esplicito:
            modelBuilder.Entity<Scheda>()
                        .HasOne(s => s.Person)          // Una Scheda ha una Person
                        .WithMany(p => p.Schede)    // Una Person ha una Scheda
                        .HasForeignKey(s => s.PersonId) // La chiave è PersonId
                        .OnDelete(DeleteBehavior.NoAction);

        }

        private static void StrisciateConfig(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Strisciata>()
                .Property(s => s.CodiceSocio).HasMaxLength(25);

            modelBuilder.Entity<Strisciata>()
                .Property(s => s.NumeroTessera).HasMaxLength(25);

            modelBuilder.Entity<Strisciata>()
                .Property(s => s.Cognome).HasMaxLength(50);

            modelBuilder.Entity<Strisciata>()
                .Property(s => s.Nome).HasMaxLength(50);
        }
    }
}
