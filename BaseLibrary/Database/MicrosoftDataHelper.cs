using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.Database
{  
    public class MicrosoftDataHelper
    {
        string _connectionString = string.Empty;
        public string ConnectionString
        {
            get 
            {                
                return _connectionString;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(_connectionString))
                    _connectionString = EncryptDecrypt.EncryptDecrypt.DecryptString(value);

            }

        }

        public IConfiguration Configuration { get; set; }
        
        /// <summary>
        /// Command time out
        /// </summary>
        private int CommandTimeOut
        {
            get
            {
                int commtimeout = 0;
                try
                {
                    Int32.TryParse(Configuration.GetSection("AppSettings").GetSection("CommandTimeOut").Value,
                        out commtimeout);
                    if (commtimeout <= 0)
                        commtimeout = 300000;
                }
                catch (Exception)
                {
                    commtimeout = 300000;
                }
                return commtimeout;
            }
        }
        
        /// <summary>
        /// Execute query. Pass isscalar when running identity queries
        /// </summary>
        /// <param name="query"></param>
        /// <param name="isScalar"></param>
        /// <returns></returns>
        public async Task<int> ExecuteQuery(string query, bool isScalar)
        {
            using(Microsoft.Data.SqlClient.SqlConnection cn = new Microsoft.Data.SqlClient.SqlConnection(ConnectionString))
            {
                await cn.OpenAsync();
                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query))
                {
                    cmd.CommandTimeout = CommandTimeOut;
                    cmd.Connection = cn;
                    if(!isScalar)
                    {
                        var result = await cmd.ExecuteNonQueryAsync();
                        return result;
                    }
                    else
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }        

        }

        


    }
}

