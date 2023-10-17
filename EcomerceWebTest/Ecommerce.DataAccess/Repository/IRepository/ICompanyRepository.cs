using System;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository.IRepository
{
	public interface ICompanyRepository : IRepository<Company>
	{
		void Update(Company model);
	}
}

