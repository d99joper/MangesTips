using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace VMTips_2022.Models
{
    public partial class Tips_Entities : DbContext
    {
        public Tips_Entities()
        {
        }

        public Tips_Entities(DbContextOptions<Tips_Entities> options)
            : base(options)
        {
        }

        // Current tournament entities
        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Standing> Standings { get; set; }
        public DbSet<TeamStats> TeamStats { get; set; }
        public DbSet<TopScorer> TopScorers { get; set; }
        public DbSet<BlogEntry> BlogEntry { get; set; }
        public DbSet<BonusPoints> BonusPoints { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<UserMatch> UserMatches { get; set; }
        public DbSet<UserBronzeTeam> UserBronzeTeam { get; set; }
        public DbSet<UserFinalTeam> UserFinalTeam { get; set; }
        public DbSet<UserGoldTeam> UserGoldTeam { get; set; }
        public DbSet<UserPlayoffTeam> UserPlayoffTeam { get; set; }
        public DbSet<UserQFTeam> UserQFTeam { get; set; }
        public DbSet<UserSFTeam> UserSFTeam { get; set; }
        public DbSet<UserSilverTeam> UserSilverTeam { get; set; }

        // Historical tournament entities
        public DbSet<User_2010> User_2010 { get; set; }
        public DbSet<User_2012> User_2012 { get; set; }
        public DbSet<User_2014> User_2014 { get; set; }
        public DbSet<User_2016> User_2016 { get; set; }
        public DbSet<User_2018> User_2018 { get; set; }
        public DbSet<User_2021> User_2021 { get; set; }
        public DbSet<Standings_2010> Standings_2010 { get; set; }
        public DbSet<Standings_2012> Standings_2012 { get; set; }
        public DbSet<Standings_2014> Standings_2014 { get; set; }
        public DbSet<Standings_2016> Standings_2016 { get; set; }
        public DbSet<Standings_2018> Standings_2018 { get; set; }
        public DbSet<Standings_2021> Standings_2021 { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Tips_Entities"]?.ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseSqlServer(connectionString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ----- Current tournament table mappings -----

            // User -> User_2022
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User_2022");
                entity.HasKey(e => e.ID);

                entity.HasOne(e => e.TopScorer)
                    .WithMany(t => t.Users)
                    .HasForeignKey(e => e.TopScorerID)
                    .IsRequired(false);
            });

            // Match -> Match_2022
            modelBuilder.Entity<Match>(entity =>
            {
                entity.ToTable("Match_2022");
                entity.HasKey(e => e.ID);

                entity.HasOne(e => e.HomeTeam)
                    .WithMany(t => t.Match)
                    .HasForeignKey(e => e.HomeTeamID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.AwayTeam)
                    .WithMany(t => t.Match_2022)
                    .HasForeignKey(e => e.AwayTeamID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Team -> Team_2022
            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team_2022");
                entity.HasKey(e => e.ID);
            });

            // Standing -> Standings_2022
            modelBuilder.Entity<Standing>(entity =>
            {
                entity.ToTable("Standings_2022");
                entity.HasKey(e => new { e.UserID, e.UpdateDate });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Standings)
                    .HasForeignKey(e => e.UserID);
            });

            // TeamStats -> TeamStats_2022
            modelBuilder.Entity<TeamStats>(entity =>
            {
                entity.ToTable("TeamStats_2022");
                entity.HasKey(e => e.TeamID);

                entity.HasOne(e => e.Team)
                    .WithOne(t => t.TeamStats)
                    .HasForeignKey<TeamStats>(e => e.TeamID);
            });

            // TopScorer -> TopScorer_2022
            modelBuilder.Entity<TopScorer>(entity =>
            {
                entity.ToTable("TopScorer_2022");
                entity.HasKey(e => e.ID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.TopScorer)
                    .HasForeignKey(e => e.TeamID)
                    .IsRequired(false);
            });

            // BlogEntry -> BlogEntry_2022
            modelBuilder.Entity<BlogEntry>(entity =>
            {
                entity.ToTable("BlogEntry_2022");
                entity.HasKey(e => e.ID);
            });

            // BonusPoints -> BonusPoints_2022
            modelBuilder.Entity<BonusPoints>(entity =>
            {
                entity.ToTable("BonusPoints_2022");
                entity.HasKey(e => e.ID);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.BonusPoints)
                    .HasForeignKey(e => e.UserID);
            });

            // Comment -> Comment_2022
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment_2022");
                entity.HasKey(e => e.ID);

                entity.HasOne(e => e.BlogEntry_2022)
                    .WithMany(b => b.Comments)
                    .HasForeignKey(e => e.BlogEntryID)
                    .IsRequired(false);
            });

            // UserMatch -> UserMatch_2022 (composite key)
            modelBuilder.Entity<UserMatch>(entity =>
            {
                entity.ToTable("UserMatch_2022");
                entity.HasKey(e => new { e.UserID, e.MatchID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserMatches)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Match)
                    .WithMany(m => m.UserMatch)
                    .HasForeignKey(e => e.MatchID);
            });

            // UserBronzeTeam -> UserBronzeTeam_2022 (composite key)
            modelBuilder.Entity<UserBronzeTeam>(entity =>
            {
                entity.ToTable("UserBronzeTeam_2022");
                entity.HasKey(e => new { e.UserID, e.TeamID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserBronzeTeam)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.UserBronzeTeam)
                    .HasForeignKey(e => e.TeamID);
            });

            // UserFinalTeam -> UserFinalTeams_2022 (composite key)
            modelBuilder.Entity<UserFinalTeam>(entity =>
            {
                entity.ToTable("UserFinalTeams_2022");
                entity.HasKey(e => new { e.UserID, e.TeamID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserFinalTeams)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.UserFinalTeams)
                    .HasForeignKey(e => e.TeamID);
            });

            // UserGoldTeam -> UserGoldTeam_2022 (composite key)
            modelBuilder.Entity<UserGoldTeam>(entity =>
            {
                entity.ToTable("UserGoldTeam_2022");
                entity.HasKey(e => new { e.UserID, e.TeamID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserGoldTeam)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.UserGoldTeam)
                    .HasForeignKey(e => e.TeamID);
            });

            // UserPlayoffTeam -> UserPlayoffTeams_2022 (composite key)
            modelBuilder.Entity<UserPlayoffTeam>(entity =>
            {
                entity.ToTable("UserPlayoffTeams_2022");
                entity.HasKey(e => new { e.UserID, e.TeamID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserPlayoffTeams)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.UserPlayoffTeams)
                    .HasForeignKey(e => e.TeamID);
            });

            // UserQFTeam -> UserQFTeams_2022 (composite key)
            modelBuilder.Entity<UserQFTeam>(entity =>
            {
                entity.ToTable("UserQFTeams_2022");
                entity.HasKey(e => new { e.UserID, e.TeamID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserQFTeams)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.UserQFTeams)
                    .HasForeignKey(e => e.TeamID);
            });

            // UserSFTeam -> UserSFTeams_2022 (composite key)
            modelBuilder.Entity<UserSFTeam>(entity =>
            {
                entity.ToTable("UserSFTeams_2022");
                entity.HasKey(e => new { e.UserID, e.TeamID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserSFTeams)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.UserSFTeams)
                    .HasForeignKey(e => e.TeamID);
            });

            // UserSilverTeam -> UserSilverTeam_2022 (composite key)
            modelBuilder.Entity<UserSilverTeam>(entity =>
            {
                entity.ToTable("UserSilverTeam_2022");
                entity.HasKey(e => new { e.UserID, e.TeamID });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserSilverTeam)
                    .HasForeignKey(e => e.UserID);

                entity.HasOne(e => e.Team)
                    .WithMany(t => t.UserSilverTeam)
                    .HasForeignKey(e => e.TeamID);
            });

            // ----- Historical tournament table mappings -----

            // User_2010
            modelBuilder.Entity<User_2010>(entity =>
            {
                entity.ToTable("User_2010");
                entity.HasKey(e => e.ID);
            });

            // User_2012
            modelBuilder.Entity<User_2012>(entity =>
            {
                entity.ToTable("User_2012");
                entity.HasKey(e => e.ID);
            });

            // User_2014
            modelBuilder.Entity<User_2014>(entity =>
            {
                entity.ToTable("User_2014");
                entity.HasKey(e => e.ID);
            });

            // User_2016
            modelBuilder.Entity<User_2016>(entity =>
            {
                entity.ToTable("User_2016");
                entity.HasKey(e => e.ID);
            });

            // User_2018
            modelBuilder.Entity<User_2018>(entity =>
            {
                entity.ToTable("User_2018");
                entity.HasKey(e => e.ID);
            });

            // User_2021
            modelBuilder.Entity<User_2021>(entity =>
            {
                entity.ToTable("User_2021");
                entity.HasKey(e => e.ID);
            });

            // Standings_2010 (composite key)
            modelBuilder.Entity<Standings_2010>(entity =>
            {
                entity.ToTable("Standings_2010");
                entity.HasKey(e => new { e.UserID, e.UpdateDate });

                entity.HasOne(e => e.User_2010)
                    .WithMany(u => u.Standings_2010)
                    .HasForeignKey(e => e.UserID);
            });

            // Standings_2012 (composite key)
            modelBuilder.Entity<Standings_2012>(entity =>
            {
                entity.ToTable("Standings_2012");
                entity.HasKey(e => new { e.UserID, e.UpdateDate });

                entity.HasOne(e => e.User_2012)
                    .WithMany(u => u.Standings_2012)
                    .HasForeignKey(e => e.UserID);
            });

            // Standings_2014 (composite key)
            modelBuilder.Entity<Standings_2014>(entity =>
            {
                entity.ToTable("Standings_2014");
                entity.HasKey(e => new { e.UserID, e.UpdateDate });

                entity.HasOne(e => e.User_2014)
                    .WithMany(u => u.Standings_2014)
                    .HasForeignKey(e => e.UserID);
            });

            // Standings_2016 (composite key)
            modelBuilder.Entity<Standings_2016>(entity =>
            {
                entity.ToTable("Standings_2016");
                entity.HasKey(e => new { e.UserID, e.UpdateDate });

                entity.HasOne(e => e.User_2016)
                    .WithMany(u => u.Standings_2016)
                    .HasForeignKey(e => e.UserID);
            });

            // Standings_2018 (composite key)
            modelBuilder.Entity<Standings_2018>(entity =>
            {
                entity.ToTable("Standings_2018");
                entity.HasKey(e => new { e.UserID, e.UpdateDate });

                entity.HasOne(e => e.User_2018)
                    .WithMany(u => u.Standings_2018)
                    .HasForeignKey(e => e.UserID);
            });

            // Standings_2021 (composite key)
            modelBuilder.Entity<Standings_2021>(entity =>
            {
                entity.ToTable("Standings_2021");
                entity.HasKey(e => new { e.UserID, e.UpdateDate });

                entity.HasOne(e => e.User_2021)
                    .WithMany(u => u.Standings_2021)
                    .HasForeignKey(e => e.UserID);
            });
        }
    }
}
