using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Models;

namespace Movies.EF.Config
{
	public class GenreConfig : IEntityTypeConfiguration<Genre>
	{
		public void Configure(EntityTypeBuilder<Genre> builder)
		{
			builder.HasKey(G => G.ID);

			builder.Property(G => G.Name).IsRequired().HasMaxLength(100);

			builder.Property(G => G.ID).ValueGeneratedOnAdd();
		}
	}
}
