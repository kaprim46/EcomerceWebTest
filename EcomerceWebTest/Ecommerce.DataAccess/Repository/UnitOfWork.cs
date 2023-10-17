﻿using System;
using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;

namespace Ecommerce.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
        private readonly ApplicationDbContext _db;
        public ICategoryRepository category { get; private set; }
        public IProductRepository product { get; private set; }
        public ICompanyRepository company { get; private set; }
        public IShoppingCartRepository shoppingCart { get; private set; }
        public IApplicationUserRepository applicationUser { get; private set; }
        public IOrderDetailRepository orderDetail { get; private set; }
        public IOrderHeaderRepository orderHeader { get; private set; }
        public IProductImageRepository productImage { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            category = new CategoryRepository(_db);
            product = new ProductRepository(_db);
            company = new CompanyRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            applicationUser = new ApplicationUserRepository(_db);
            orderHeader = new OrderHeaderRepository(_db);
            orderDetail = new OrderDetailRepository(_db);
            productImage = new ProductImageRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

