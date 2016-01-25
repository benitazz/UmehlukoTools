#region

using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Dapper;

#endregion

namespace Umehluko.Tools.DataModel.DataAccessHelper
{
    /// <summary>
    /// The dapper access helper.
    /// </summary>
    public class DapperAccessHelper
    {
        /// <summary>
        /// The db.
        /// </summary>
        private static readonly IDbConnection db =
            new SqlConnection(ConfigurationManager.ConnectionStrings["UmehlukoEntities1"].ConnectionString);

       /// <summary>
        /// The get medical item.
        /// </summary>
        /// <returns>
        /// The <see cref="MedicalItem"/>.
        /// </returns>
        public static MedicalItem GetMedicalItem()
       {
           var query = @"SELECT * FROM Medical.MedicalItem WHERE ItemCode = '560'";

            foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
            {
                string name = css.Name;
                string connString = css.ConnectionString;
                string provider = css.ProviderName;
            }
            
            return (MedicalItem)db.Query<MedicalItem>(query);

        }
    }
}