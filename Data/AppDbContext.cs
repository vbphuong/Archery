using Archery.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Archery.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ENTITY SETS
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Inbox> Inbox { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Archer> Archers { get; set; }
        public DbSet<ArcherEquipment> ArcherEquipments { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<CompetitionRound> CompetitionRounds { get; set; }
        public DbSet<EquivalentRound> EquivalentRounds { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<ArcheryRange> ArcheryRanges { get; set; }
        public DbSet<End> Ends { get; set; }
        public DbSet<Arrow> Arrows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // KEYS
            modelBuilder.Entity<ArcherEquipment>()
                .HasKey(ae => new { ae.EquipmentID, ae.ArcherID });

            modelBuilder.Entity<CompetitionRound>()
                .HasKey(cr => new { cr.RoundID, cr.CompetitionID });

            modelBuilder.Entity<Score>()
                .HasKey(s => new { s.RoundID, s.CompetitionID, s.ArcherID });

            // RELATIONSHIPS

            modelBuilder.Entity<State>()
               .HasOne(s => s.Country)
               .WithMany(c => c.States) 
               .HasForeignKey(s => s.CountryID)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<City>()
                .HasOne(c => c.State)
                .WithMany(s => s.Cities) 
                .HasForeignKey(c => c.StateID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.City)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CityID)
                .OnDelete(DeleteBehavior.Cascade);

            // Archer <=> Address 
            modelBuilder.Entity<Archer>()
                .HasOne(a => a.Address)
                .WithMany()
                .HasForeignKey(a => a.AddressID)
                .OnDelete(DeleteBehavior.Restrict);

            // Archer <=> User (1–1)
            modelBuilder.Entity<Archer>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<Archer>(a => a.UserID);

            // ArcherEquipment <=> Equipment, Archer
            modelBuilder.Entity<ArcherEquipment>()
                .HasKey(ae => new { ae.ArcherID, ae.EquipmentID });

            modelBuilder.Entity<ArcherEquipment>()
                .HasOne(ae => ae.Archer)
                .WithMany(a => a.ArcherEquipments)
                .HasForeignKey(ae => ae.ArcherID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArcherEquipment>()
                .HasOne(ae => ae.Equipment)
                .WithMany(e => e.ArcherEquipments)
                .HasForeignKey(ae => ae.EquipmentID)
                .OnDelete(DeleteBehavior.Cascade);

            // Round <=> Equipment
            modelBuilder.Entity<Round>()
                .HasOne(r => r.Equipment)
                .WithMany()
                .HasForeignKey(r => r.EquipmentID);

            // CompetitionRound <=> Competition, Round
            modelBuilder.Entity<CompetitionRound>()
                .HasOne(cr => cr.Competition)
                .WithMany()
                .HasForeignKey(cr => cr.CompetitionID);

            modelBuilder.Entity<CompetitionRound>()
                .HasOne(cr => cr.Round)
                .WithMany()
                .HasForeignKey(cr => cr.RoundID);

            // Score <=> CompetitionRound, Archer
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Archer)
                .WithMany()
                .HasForeignKey(s => s.ArcherID);

            modelBuilder.Entity<Score>()
                .HasOne<CompetitionRound>()
                .WithMany()
                .HasForeignKey(s => new { s.RoundID, s.CompetitionID });

            // ArcheryRange <=> Score
            modelBuilder.Entity<ArcheryRange>()
                .HasOne(r => r.Score)
                .WithMany()
                .HasForeignKey(r => new { r.RoundID, r.CompetitionID, r.ArcherID })
                .HasPrincipalKey(s => new { s.RoundID, s.CompetitionID, s.ArcherID });

            // End <=> ArcheryRange
            modelBuilder.Entity<End>()
                .HasOne(e => e.ArcheryRange)
                .WithMany(r => r.Ends)
                .HasForeignKey(e => e.RangeID)
                .OnDelete(DeleteBehavior.Cascade);

            // Arrow <=> End
            modelBuilder.Entity<Arrow>()
                .HasOne(a => a.End)
                .WithMany(e => e.Arrows)
                .HasForeignKey(a => a.EndID)
                .OnDelete(DeleteBehavior.Cascade);

            // EquivalentRound <=> Round
            modelBuilder.Entity<EquivalentRound>()
                .HasOne(er => er.BaseRound)
                .WithMany()
                .HasForeignKey(er => er.BaseRoundID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EquivalentRound>()
                .HasOne(er => er.EquivalentTo)
                .WithMany()
                .HasForeignKey(er => er.EquivalentRoundID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}