

using Movies.EF.Config;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Movies.Core.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace FirstWebApi.Models
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

		public ApplicationDbContext() { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			modelBuilder.Entity<ApplicationUser>().ToTable("Users");
			modelBuilder.Entity<IdentityRole>().ToTable("Roles");
			modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
			modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
			modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
			modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
			modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

			modelBuilder.Entity<ApplicationUser>()
				.OwnsMany(
				u => u.RefreshTokens,
				t =>
				{
					t.WithOwner().HasForeignKey("UserId");
					t.Property<int>("Id");
					t.HasKey("Id");
				});
			

			modelBuilder.ApplyConfiguration(new GenreConfig());
			modelBuilder.ApplyConfiguration(new MovieConfig());
			modelBuilder.ApplyConfiguration(new CharcaterConfig());
			modelBuilder.ApplyConfiguration(new CharactersInMoviesConfig());
		}

		public virtual DbSet<Genre> Genres { get;set; }
		public virtual DbSet<Movie> Movies { get;set; }
		public virtual DbSet<Character> Characters { get;set; }
		public virtual DbSet<CharacterInMovie> CharactersInMovies { get;set; }
	}
}
