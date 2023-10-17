using System;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository.IRepository
{
	public interface IApplicationUserRepository : IRepository<ApplicationUser>
	{
		void Update(ApplicationUser model);
	}
}

