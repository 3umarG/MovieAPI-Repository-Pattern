namespace MoviesApi.Config
{
	public class CharactersInMoviesConfig : IEntityTypeConfiguration<CharacterInMovie>
	{
		public void Configure(EntityTypeBuilder<CharacterInMovie> builder)
		{
			builder.HasKey(CM => new { CM.MovieID, CM.CharacterID });
		}
	}
}
