using System;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository.IRepository
{
	public interface IProductImageRepository : IRepository<ProductImage>
	{
		void Update(ProductImage model);
	}
}

