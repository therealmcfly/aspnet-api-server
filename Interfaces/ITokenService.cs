using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnet_api_server.Models;

namespace aspnet_api_server.Interfaces
{
	public interface ITokenService
	{
		string CreateToken(AppUser user);
	}
}