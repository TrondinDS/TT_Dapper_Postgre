using Dapper;
using Microsoft.AspNetCore.Connections;
using Test.DB.Models;
using Test.Repositories.Generic;
using Test.Repositories.Interfaces;
using Test1.DB.DbConnectionFactory;

namespace Test.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => nameof(Company);

        protected override string KeyName => nameof(Company.Id);
    }
}
