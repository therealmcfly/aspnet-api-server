using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet_api_server.Dtos.Account
{
	public class UpdateUserDto
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
	}
}