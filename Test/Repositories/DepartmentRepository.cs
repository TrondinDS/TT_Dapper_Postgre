using Dapper;
using Test.DB.Models;
using Test.Repositories.Generic;
using Test.Repositories.Interfaces;
using Test1.DB.DbConnectionFactory;
using Test1.DB.Models;

namespace Test.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => nameof(Department);

        protected override string KeyName => nameof(Department.Id);
    }
}
