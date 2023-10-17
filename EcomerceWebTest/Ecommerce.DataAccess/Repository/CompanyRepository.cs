using System;
using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository
{
	public class CompanyRepository : Repository<Company>, ICompanyRepository
	{
        private readonly ApplicationDbContext _db;
		public CompanyRepository(ApplicationDbContext db) : base(db)
		{
            _db = db;
		}

        public void Update(Company model)
        {
            var dataFromDb = _db.Companies.FirstOrDefault(q => q.CompanyId == model.CompanyId);
            if (dataFromDb != null)
            {
                dataFromDb.Name = model.Name;
                dataFromDb.City = model.City;
                dataFromDb.PhoneNumber = model.PhoneNumber;
                dataFromDb.PostalCode = model.PostalCode;
                dataFromDb.State = model.State;
                dataFromDb.StreetAdress = model.StreetAdress;
            }
        }
    }
}

