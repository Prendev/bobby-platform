using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using QvaDev.Data.Models;

namespace QvaDev.Data.Repositories
{
    public interface IMetaTraderPlatformRepository
    {
        void Add(MetaTraderPlatform entity);
        IEnumerable<MetaTraderPlatform> GetAll();
    }

    public class MetaTraderPlatformRepository : IMetaTraderPlatformRepository
    {
        public void Add(MetaTraderPlatform entity)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DuplicatContext"].ConnectionString))
            {
                conn.Open();
                conn.Insert(entity);
            }
        }

        public IEnumerable<MetaTraderPlatform> GetAll()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DuplicatContext"].ConnectionString))
            {
                conn.Open();
                return conn.GetAll<MetaTraderPlatform>();
            }
        }
    }
}
