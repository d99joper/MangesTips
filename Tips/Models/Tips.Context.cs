using Microsoft.EntityFrameworkCore;

namespace Tipset.Models
{
    public partial class Tips_Entities : DbContext
    {
        public Tips_Entities(DbContextOptions<Tips_Entities> options)
            : base(options)
        {
        }

        // Current tournament entities
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Standing> Standings { get; set; }
        public virtual DbSet<TeamStats> TeamStats { get; set; }
        public virtual DbSet<TopScorer> TopScorers { get; set; }
        public virtual DbSet<BlogEntry> BlogEntry { get; set; }
        public virtual DbSet<BonusPoints> BonusPoints { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<UserMatch> UserMatches { get; set; }
        public virtual DbSet<UserBronzeTeam> UserBronzeTeam { get; set; }
        public virtual DbSet<UserFinalTeam> UserFinalTeam { get; set; }
        public virtual DbSet<UserGoldTeam> UserGoldTeam { get; set; }
        public virtual DbSet<UserPlayoffTeam> UserPlayoffTeam { get; set; }
        public virtual DbSet<UserQFTeam> UserQFTeam { get; set; }
        public virtual DbSet<UserSFTeam> UserSFTeam { get; set; }
        public virtual DbSet<UserSilverTeam> UserSilverTeam { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }

        // Historical tournament entities
        public virtual DbSet<User_2010> User_2010 { get; set; }
        public virtual DbSet<User_2012> User_2012 { get; set; }
        public virtual DbSet<User_2014> User_2014 { get; set; }
        public virtual DbSet<User_2016> User_2016 { get; set; }
        public virtual DbSet<User_2018> User_2018 { get; set; }
        public virtual DbSet<User_2021> User_2021 { get; set; }
        public virtual DbSet<User_2022> User_2022 { get; set; }
        public virtual DbSet<User_2024> User_2024 { get; set; }
        public virtual DbSet<Standings_2010> Standings_2010 { get; set; }
        public virtual DbSet<Standings_2012> Standings_2012 { get; set; }
        public virtual DbSet<Standings_2014> Standings_2014 { get; set; }
        public virtual DbSet<Standings_2016> Standings_2016 { get; set; }
        public virtual DbSet<Standings_2018> Standings_2018 { get; set; }
        public virtual DbSet<Standings_2021> Standings_2021 { get; set; }
        public virtual DbSet<Standings_2022> Standings_2022 { get; set; }
        public virtual DbSet<Standings_2024> Standings_2024 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ----- Current tournament table mappings -----

            // User -> User_2026
            modelBuilder.Entity<User>().ToTable("User_2026").HasKey(e => e.ID);
            modelBuilder.Entity<User>()
                .HasOne(e => e.TopScorer)
                .WithMany(t => t.Users)
                .HasForeignKey(e => e.TopScorerID)
                .IsRequired(false);

            // Match -> Match_2026
            modelBuilder.Entity<Match>().ToTable("Match_2026").HasKey(e => e.ID);
            modelBuilder.Entity<Match>()
                .HasOne(e => e.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(e => e.HomeTeamID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Match>()
                .HasOne(e => e.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(e => e.AwayTeamID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Team -> Team_2026
            modelBuilder.Entity<Team>().ToTable("Team_2026").HasKey(e => e.ID);

            // Standing -> Standings_2026
            modelBuilder.Entity<Standing>().ToTable("Standings_2026").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standing>()
                .HasOne(e => e.User)
                .WithMany(u => u.Standings)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // TeamStats -> TeamStats_2026
            modelBuilder.Entity<TeamStats>().ToTable("TeamStats_2026").HasKey(e => e.TeamID);
            modelBuilder.Entity<TeamStats>()
                .HasOne(e => e.Team)
                .WithOne(t => t.TeamStats)
                .HasForeignKey<TeamStats>(e => e.TeamID)
                .IsRequired();

            // TopScorer -> TopScorer_2026
            modelBuilder.Entity<TopScorer>().ToTable("TopScorer_2026").HasKey(e => e.ID);
            modelBuilder.Entity<TopScorer>()
                .HasOne(e => e.Team)
                .WithMany(t => t.TopScorer)
                .HasForeignKey(e => e.TeamID)
                .IsRequired(false);

            // BlogEntry -> BlogEntry_2026
            modelBuilder.Entity<BlogEntry>().ToTable("BlogEntry_2026").HasKey(e => e.ID);

            // BonusPoints -> BonusPoints_2026
            modelBuilder.Entity<BonusPoints>().ToTable("BonusPoints_2026").HasKey(e => e.ID);
            modelBuilder.Entity<BonusPoints>()
                .HasOne(e => e.User)
                .WithMany(u => u.BonusPoints)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Comment -> Comment_2026
            modelBuilder.Entity<Comment>().ToTable("Comment_2026").HasKey(e => e.ID);
            modelBuilder.Entity<Comment>()
                .HasOne(e => e.BlogEntry)
                .WithMany(b => b.Comments)
                .HasForeignKey(e => e.BlogEntryID)
                .IsRequired(false);

            // UserMatch -> UserMatch_2026 (composite key)
            modelBuilder.Entity<UserMatch>().ToTable("UserMatch_2026").HasKey(e => new { e.UserID, e.MatchID });
            modelBuilder.Entity<UserMatch>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserMatches)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserMatch>()
                .HasOne(e => e.Match)
                .WithMany(m => m.UserMatch)
                .HasForeignKey(e => e.MatchID)
                .IsRequired();

            // UserBronzeTeam -> UserBronzeTeam_2026 (composite key)
            modelBuilder.Entity<UserBronzeTeam>().ToTable("UserBronzeTeam_2026").HasKey(e => new { e.UserID, e.TeamID });
            modelBuilder.Entity<UserBronzeTeam>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserBronzeTeam)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserBronzeTeam>()
                .HasOne(e => e.Team)
                .WithMany(t => t.UserBronzeTeam)
                .HasForeignKey(e => e.TeamID)
                .IsRequired();

            // UserFinalTeam -> UserFinalTeams_2026 (composite key)
            modelBuilder.Entity<UserFinalTeam>().ToTable("UserFinalTeams_2026").HasKey(e => new { e.UserID, e.TeamID });
            modelBuilder.Entity<UserFinalTeam>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserFinalTeams)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserFinalTeam>()
                .HasOne(e => e.Team)
                .WithMany(t => t.UserFinalTeams)
                .HasForeignKey(e => e.TeamID)
                .IsRequired();

            // UserGoldTeam -> UserGoldTeam_2026 (composite key)
            modelBuilder.Entity<UserGoldTeam>().ToTable("UserGoldTeam_2026").HasKey(e => new { e.UserID, e.TeamID });
            modelBuilder.Entity<UserGoldTeam>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserGoldTeam)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserGoldTeam>()
                .HasOne(e => e.Team)
                .WithMany(t => t.UserGoldTeam)
                .HasForeignKey(e => e.TeamID)
                .IsRequired();

            // UserPlayoffTeam -> UserPlayoffTeams_2026 (composite key)
            modelBuilder.Entity<UserPlayoffTeam>().ToTable("UserPlayoffTeams_2026").HasKey(e => new { e.UserID, e.TeamID });
            modelBuilder.Entity<UserPlayoffTeam>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserPlayoffTeams)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserPlayoffTeam>()
                .HasOne(e => e.Team)
                .WithMany(t => t.UserPlayoffTeams)
                .HasForeignKey(e => e.TeamID)
                .IsRequired();

            // UserQFTeam -> UserQFTeams_2026 (composite key)
            modelBuilder.Entity<UserQFTeam>().ToTable("UserQFTeams_2026").HasKey(e => new { e.UserID, e.TeamID });
            modelBuilder.Entity<UserQFTeam>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserQFTeams)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserQFTeam>()
                .HasOne(e => e.Team)
                .WithMany(t => t.UserQFTeams)
                .HasForeignKey(e => e.TeamID)
                .IsRequired();

            // UserSFTeam -> UserSFTeams_2026 (composite key)
            modelBuilder.Entity<UserSFTeam>().ToTable("UserSFTeams_2026").HasKey(e => new { e.UserID, e.TeamID });
            modelBuilder.Entity<UserSFTeam>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserSFTeams)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserSFTeam>()
                .HasOne(e => e.Team)
                .WithMany(t => t.UserSFTeams)
                .HasForeignKey(e => e.TeamID)
                .IsRequired();

            // UserSilverTeam -> UserSilverTeam_2026 (composite key)
            modelBuilder.Entity<UserSilverTeam>().ToTable("UserSilverTeam_2026").HasKey(e => new { e.UserID, e.TeamID });
            modelBuilder.Entity<UserSilverTeam>()
                .HasOne(e => e.User)
                .WithMany(u => u.UserSilverTeam)
                .HasForeignKey(e => e.UserID)
                .IsRequired();
            modelBuilder.Entity<UserSilverTeam>()
                .HasOne(e => e.Team)
                .WithMany(t => t.UserSilverTeam)
                .HasForeignKey(e => e.TeamID)
                .IsRequired();

            // ----- Historical tournament table mappings -----

            modelBuilder.Entity<User_2010>().ToTable("User_2010").HasKey(e => e.ID);
            modelBuilder.Entity<User_2012>().ToTable("User_2012").HasKey(e => e.ID);
            modelBuilder.Entity<User_2014>().ToTable("User_2014").HasKey(e => e.ID);
            modelBuilder.Entity<User_2016>().ToTable("User_2016").HasKey(e => e.ID);
            modelBuilder.Entity<User_2018>().ToTable("User_2018").HasKey(e => e.ID);
            modelBuilder.Entity<User_2021>().ToTable("User_2021").HasKey(e => e.ID);
            modelBuilder.Entity<User_2022>().ToTable("User_2022").HasKey(e => e.ID);
            modelBuilder.Entity<User_2024>().ToTable("User_2024").HasKey(e => e.ID);

            // Standings_2010 (composite key)
            modelBuilder.Entity<Standings_2010>().ToTable("Standings_2010").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2010>()
                .HasOne(e => e.User_2010)
                .WithMany(u => u.Standings_2010)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Standings_2012 (composite key)
            modelBuilder.Entity<Standings_2012>().ToTable("Standings_2012").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2012>()
                .HasOne(e => e.User_2012)
                .WithMany(u => u.Standings_2012)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Standings_2014 (composite key)
            modelBuilder.Entity<Standings_2014>().ToTable("Standings_2014").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2014>()
                .HasOne(e => e.User_2014)
                .WithMany(u => u.Standings_2014)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Standings_2016 (composite key)
            modelBuilder.Entity<Standings_2016>().ToTable("Standings_2016").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2016>()
                .HasOne(e => e.User_2016)
                .WithMany(u => u.Standings_2016)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Standings_2018 (composite key)
            modelBuilder.Entity<Standings_2018>().ToTable("Standings_2018").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2018>()
                .HasOne(e => e.User_2018)
                .WithMany(u => u.Standings_2018)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Standings_2021 (composite key)
            modelBuilder.Entity<Standings_2021>().ToTable("Standings_2021").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2021>()
                .HasOne(e => e.User_2021)
                .WithMany(u => u.Standings_2021)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Standings_2022 (composite key)
            modelBuilder.Entity<Standings_2022>().ToTable("Standings_2022").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2022>()
                .HasOne(e => e.User_2022)
                .WithMany(u => u.Standings_2022)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            // Standings_2024 (composite key)
            modelBuilder.Entity<Standings_2024>().ToTable("Standings_2024").HasKey(e => new { e.UserID, e.UpdateDate });
            modelBuilder.Entity<Standings_2024>()
                .HasOne(e => e.User_2024)
                .WithMany(u => u.Standings_2024)
                .HasForeignKey(e => e.UserID)
                .IsRequired();

            modelBuilder.Entity<AppSetting>()
                .ToTable("AppSettings")
                .HasKey(e => new { e.Year, e.Key });
        }
    }
}
