using System;
using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
		{
            _db = db;
        }

        public void Update(Product model)
        {
            var dataFromDb = _db.Products.FirstOrDefault(q => q.ProductId == model.ProductId);
            if (dataFromDb != null)
            {
                dataFromDb.Title = model.Title;
                dataFromDb.ISBN = model.ISBN;
                dataFromDb.Price = model.Price;
                dataFromDb.Price100 = model.Price100;
                dataFromDb.Price50 = model.Price50;
                dataFromDb.ListPrice = model.ListPrice;
                dataFromDb.Author = model.Author;
                dataFromDb.CategoryId = model.CategoryId;
                dataFromDb.Description = model.Description;
                dataFromDb.ProductImages = model.ProductImages;
                //if (model.ImageUrl != null)
               // {
                  //  dataFromDb.ImageUrl = model.ImageUrl;
               // }
            }
        }
    }
}

