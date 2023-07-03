namespace MoviesApi.Config
{
	public class CharcaterConfig : IEntityTypeConfiguration<Character>
	{
		public void Configure(EntityTypeBuilder<Character> builder)
		{
			builder.HasKey(C => C.ID);

			builder.OwnsOne(C => C.CharacterName, name =>
			{
				name.Property(n => n.FirstName).IsRequired();
				name.Property(n => n.LastName).IsRequired(false);
			});

			builder
				.HasMany(C => C.CharacterActInMovies)
				.WithOne(CM => CM.Character);


		}
	}
}
