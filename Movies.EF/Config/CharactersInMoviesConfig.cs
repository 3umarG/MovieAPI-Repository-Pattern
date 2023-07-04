using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Models;

namespace Movies.EF.Config
{
	public class CharactersInMoviesConfig : IEntityTypeConfiguration<CharacterInMovie>
	{
		public void Configure(EntityTypeBuilder<CharacterInMovie> builder)
		{
			builder.HasKey(CM => new { CM.MovieID, CM.CharacterID });
		}
	}
}
