using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnet_api_server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace aspnet_api_server.Data
{
	public class ApplicationDBContext : IdentityDbContext<AppUser>
	{
		public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			List<IdentityRole> roles = new List<IdentityRole>
			{
				new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
				new IdentityRole { Name = "User", NormalizedName = "USER" }
			};
			modelBuilder.Entity<IdentityRole>().HasData(roles);
		}

	}
}

