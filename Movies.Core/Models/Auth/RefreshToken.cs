using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Models.Auth
{
	public class RefreshToken
	{
        public string Token { get; set; }

        public DateTime ExpiresOn { get; set; }

        public bool IsExpired => DateTime.Now >= ExpiresOn;

        public DateTime CreatedOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        public bool IsActive => RevokedOn is null && !IsExpired;
    }
}
