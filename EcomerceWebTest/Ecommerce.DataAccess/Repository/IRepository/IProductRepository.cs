using System;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository.IRepository
{
	public interface IProductRepository : IRepository<Product>
	{
		void Update(Product model);
	}
}

