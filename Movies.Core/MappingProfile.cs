using AutoMapper;
using Movies.Core.DTOs;
using Movies.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			//CreateMap<Tsource, Tdestination>();

			/*
			 CharacterResponseDto

			public int Id { get; set; }

			public string FirstName { get; set; }

			public string? LastName { get; set; }

			public DateTime BirthDate { get; set; }
			 */

			/*
			Character

				public int ID { get; set; }

				public Name CharacterName { get; set; }

				public DateTime BirthDate { get; set; }

				public virtual ICollection<CharacterInMovie> CharacterActInMovies { get; set; } = new HashSet<CharacterInMovie>();

			 */


			CreateMap<Character, CharacterResponseDto>()
				.ForMember(dest => dest.Id, src => src.MapFrom(src => src.ID)) // Map the Ids
				.ForMember(des => des.FirstName, src => src.MapFrom(src => src.CharacterName.FirstName))
				.ForMember(des => des.LastName, src => src.MapFrom(src => src.CharacterName.LastName))
				.ReverseMap()
				;


			/*
			 * CharacterRequestDto
			 * 
				public string FirstName { get; set; }

				public string? LastName { get; set; }

				public DateTime BirthDate { get; set; }

			*/
			CreateMap<CharacterRequestDto, Character>()
				.IgnoreAllPropertiesWithAnInaccessibleSetter()
				.ForPath(d => d.CharacterName.FirstName, s => s.MapFrom(s => s.FirstName))
				.ForPath(d => d.CharacterName.LastName, s => s.MapFrom(s => s.LastName));



			/*
			 *  CharacterWithMovieResponseDto
	
					public int MovieId { get; set; }

					public int CharacterId { get; set; }

					public double Salary { get; set; }
			 * 
			 * 
			 */


			/*
			 * CharacterInMovie
	
				public int CharacterID { get; set; }

				public int MovieID { get; set; }

				public double Salary { get; set; }

				public virtual Character Character { get; set; }
				
				public virtual Movie Movie { get; set; }
			 * 
			 * 
			 * 
			 */

			CreateMap<CharacterInMovie, CharacterWithMovieResponseDto>()
				.ForMember(d => d.MovieId, s => s.MapFrom(s => s.MovieID))
				.ForMember(d => d.CharacterId, s => s.MapFrom(s => s.CharacterID));





			/*
			 * Movie
					public int ID { get; set; }

					public string Title { get; set; }

					public int Year { get; set; }

					public double Rate { get; set; }

					public string StoryLine { get; set; }

					public byte[] Poster { get; set; }

					public byte GenreID { get; set; }

					public Genre Genre { get; set; }

			 */


			/*
			 * 
			 
			MovieResponseDto
			  
					public int ID { get; set; }

					public string Title { get; set; }

					public int Year { get; set; }

					public double Rate { get; set; }

					public string StoryLine { get; set; }


					public GenreResponseDto Genre { get; set; }
			 * 
			 */

			CreateMap<Movie, MovieResponseDto>()
			.ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre));

			CreateMap<Genre, GenreResponseDto>();


			/*
			 * 
			 * CharacterWithAllMoviesResponseDto
	
				public int Id { get; set; }
	
				public string FirstName { get; set; }

				public string LastName { get; set; }

				public DateTime BirthDate { get; set; }

				public List<MovieResponseDto> Movies { get; set; }
			 */

			CreateMap<Character, CharacterWithAllMoviesResponseDto>()
				.ForMember(d => d.FirstName, s => s.MapFrom(s => s.CharacterName.FirstName))
				.ForMember(d => d.LastName, s => s.MapFrom(s => s.CharacterName.LastName))
				.ForMember(d => d.Movies, s => s.MapFrom(s => s.CharacterActInMovies.Select(cm => cm.Movie)))
				;
		}
	}
}
