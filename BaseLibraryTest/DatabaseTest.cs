using BaseLibrary.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;

namespace BaseLibraryTest
{
    [TestClass]
    public class DatabaseTest
    {
        private IConfiguration configuration = InitConfiguration();
        private DatabaseHelper _dbHelper = null;
        private string _connectionString = string.Empty;

        public string ConnectionString
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_connectionString))
                {
                    _connectionString = configuration.GetConnectionString("EmployeeDB");                    
                }
                return _connectionString;
            }
        }
        public DatabaseHelper DbHelper 
        { 
            get
            {
                if(_dbHelper == null)
                {                  
                    _dbHelper = new DatabaseHelper(this.ConnectionString, true) { Configuration = configuration };                    
                }
                return _dbHelper;
            }
            
        }

        [TestMethod]
        public void TestSQLReader()
        {              
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandText = "SELECT * FROM Employees";
            SqlDataReader dr = DbHelper.ExecuteSQLReader(sqlCommand);
            Assert.IsNotNull(dr);
            Assert.AreNotEqual(dr.HasRows, false);
        }

        
        [TestMethod]
        public void TestExecuteNonQuery()
        {
            TimeSpan ts = new TimeSpan(0, 10, 0);

            using (System.Transactions.TransactionScope scope =
                new System.Transactions.TransactionScope(
                    System.Transactions.TransactionScopeOption.Required, ts))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = "INSERT INTO Employees VALUES('TEST')";
                    int numRows = DbHelper.ExecuteNonQuery(sqlCommand);
                    Assert.AreNotEqual(numRows, 0);
                }
                catch
                {
                    throw;
                }
            }
        }

        [TestMethod]
        public void TestExecuteQueryMsft()
        {
            TimeSpan ts = new TimeSpan(0, 10, 0);

            MicrosoftDataHelper microsoftDataHelper = new MicrosoftDataHelper() { 
                ConnectionString = this.ConnectionString,
                Configuration = this.configuration
            };
            using (System.Transactions.TransactionScope scope =
                new System.Transactions.TransactionScope(
                    System.Transactions.TransactionScopeOption.Required, ts))
            {
                try
                {                   
                    var returnVal = microsoftDataHelper.ExecuteQuery("INSERT INTO Employees VALUES('TEST')",false).Result;
                 
                    Assert.AreNotEqual(returnVal, 0);
                }
                catch
                {
                    throw;
                }
            }
        }

        //[TestMethod]
        //public void TestSQLReaderMsft()
        //{
        //    MicrosoftDataHelper microsoftDataHelper = new MicrosoftDataHelper()
        //    {
        //        ConnectionString = this.ConnectionString,
        //        Configuration = this.configuration
        //    };
        //    Microsoft.Data.SqlClient.SqlCommand sqlCommand = new Microsoft.Data.SqlClient.SqlCommand();
        //    sqlCommand.CommandText = "SELECT * FROM Attributes";
        //    Microsoft.Data.SqlClient.SqlDataReader dr = microsoftDataHelper.ExecuteDataReader(sqlCommand).Result;
        //    Assert.IsNotNull(dr);
        //    Assert.AreNotEqual(dr.HasRows, false);
        //    while (dr.Read())
        //    {
        //        Assert.AreNotEqual(dr.HasRows, false);
        //    }
            
        //}

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return config;
        }
    }
}
