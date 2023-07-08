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

		}
	}
}
