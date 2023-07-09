﻿

using Movies.EF.Config;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Models;

namespace FirstWebApi.Models
{
	public class ApplicationDbContext : DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

		public ApplicationDbContext() { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
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
