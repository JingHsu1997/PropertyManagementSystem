using System.Data;

namespace PropertyManagementSystem.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
