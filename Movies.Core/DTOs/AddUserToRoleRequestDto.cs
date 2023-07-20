using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.DTOs
{
	public class AddUserToRoleRequestDto
	{
		[Required]
		public string EmailOrUserName { get; set; }

		[Required]
		public string RoleName { get; set; }
	}
}
