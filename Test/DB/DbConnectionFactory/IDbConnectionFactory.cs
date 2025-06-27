using System.Data;

namespace Test1.DB.DbConnectionFactory
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateOpenConnectionAsync();
    }
}
