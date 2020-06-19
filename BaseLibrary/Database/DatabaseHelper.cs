using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Storage;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace BaseLibrary.Database
{
    public class DatabaseHelper
    {
        string _connectionString = string.Empty;
        Microsoft.Practices.EnterpriseLibrary.Data.Database db = null;
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
        private DatabaseHelper()
        {

        }

        public IConfiguration Configuration { get; set; }
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

        public DatabaseHelper(string ConnectionString, bool isSQLConnectionString)
        {
            _connectionString = ConnectionString;
            _providesqlconnectionstring = isSQLConnectionString;

            if (!_connectionString.ToLower().Contains("initial"))
            {
                _connectionString = EncryptDecrypt.EncryptDecrypt.DecryptString(_connectionString);//.Replace("\u0006", "").Replace("\u0010", ""); 
            }

            if (db == null)
                db = GetDatabase();
        }
        #region	Database Helper Methods

        /// <summary>
        /// Takes a command object and returns a sqldatareader object
        /// </summary>
        /// <param name="sqlCommand">SQLCommand</param>
        /// <returns>SqlDataReader</returns>
        public IDataReader ExecuteReader(SqlCommand command)
        {

            if (command == null) { throw new ArgumentNullException("command"); }
            int timeout = CommandTimeOut;
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }
            //Microsoft.Practices.EnterpriseLibrary.Data.Database db = GetDatabase();            
            return db.ExecuteReader((DbCommand)command);
        }

        /// <summary>
        /// Takes a command object and returns a sqldatareader object
        /// </summary>
        /// <param name="sqlCommand">SQLCommand</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteSQLReader(SqlCommand command)
        {

            if (command == null) { throw new ArgumentNullException("command"); }
            int timeout = CommandTimeOut;
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }
            RefCountingDataReader refdr = (RefCountingDataReader)db.ExecuteReader((DbCommand)command);
            //Database db = DatabaseFactory.CreateDatabase(_connectionString);
            return (SqlDataReader)refdr.InnerReader;


        }


        /// <summary>
        /// Takes a command object and returns an xmlreader object
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public XmlReader ExecuteXMLReader(SqlCommand command)
        {
            if (command == null) { throw new ArgumentNullException("command"); }
            int timeout = CommandTimeOut;
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }
            SqlDatabase dbSQL = db as SqlDatabase; //GetDatabase() as SqlDatabase;            
            return (XmlReader)dbSQL.ExecuteXmlReader((DbCommand)command);
        }
        /// <summary>
        /// Takes a command object and returns the first column of the first row
        /// </summary>
        /// <param name="sqlCommand">SQLCommand</param>
        /// <returns>SqlDataReader</returns>
        public int ExecuteScalar(SqlCommand command)
        {
            if (command == null) { throw new ArgumentNullException("command"); }
            int timeout = CommandTimeOut;
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }
            //Microsoft.Practices.EnterpriseLibrary.Data.Database db = GetDatabase();            
            return Convert.ToInt32(db.ExecuteScalar((DbCommand)command));
        }

        /// <summary>
        /// Takes a command object and returns the first column of the first row as object
        /// </summary>
        /// <param name="sqlCommand">SQLCommand</param>
        /// <returns>SqlDataReader</returns>
        public object ExecuteScalarAsObject(SqlCommand command)
        {
            if (command == null) { throw new ArgumentNullException("command"); }
            int timeout = CommandTimeOut;
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }
            //Microsoft.Practices.EnterpriseLibrary.Data.Database db = GetDatabase();            
            return db.ExecuteScalar((DbCommand)command);
        }


        /// <summary>
        /// Takes a command object and returns an integer for the number of rows affected
        /// </summary>
        /// <param name="sqlCommand">SQLCommand</param>
        /// <returns>SqlDataReader</returns>
        public int ExecuteNonQuery(SqlCommand command)
        {
            if (command == null) { throw new ArgumentNullException("command"); }
            int timeout = CommandTimeOut;
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }
            //Microsoft.Practices.EnterpriseLibrary.Data.Database db = GetDatabase();            
            return db.ExecuteNonQuery((DbCommand)command);
        }

        /// <summary>
        /// Takes a command object and returns a dataset
        /// </summary>
        /// <param name="sqlCommand">SQLCommand</param>
        /// <returns>SqlDataReader</returns>
        public DataSet ExecuteDataSet(SqlCommand command)
        {
            if (command == null) { throw new ArgumentNullException("command"); }
            int timeout = CommandTimeOut;
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }
            //Microsoft.Practices.EnterpriseLibrary.Data.Database db = GetDatabase();            
            return db.ExecuteDataSet((DbCommand)command);

        }

        #endregion

        #region	Utility Methods
        /// <summary>
        /// Add stored proc parameter in sql command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        public void AddInParameter(SqlCommand command, string name, DbType dbType)
        {
            SqlParameter param = command.CreateParameter();
            param.DbType = dbType;
            param.ParameterName = name;
            param.Direction = ParameterDirection.Input;
            param.SourceColumn = String.Empty;
            param.IsNullable = false;
            param.Size = 0;
            param.Precision = 0;
            param.SourceVersion = DataRowVersion.Default;
            command.Parameters.Add(param);
        }
        /// <summary>
        /// Add stored proc parameter in sql command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        public void AddInParameter(SqlCommand command, string name, SqlDbType dbType)
        {
            SqlParameter param = command.CreateParameter();
            param.SqlDbType = dbType;
            param.ParameterName = name;
            param.Direction = ParameterDirection.Input;
            param.SourceColumn = String.Empty;
            param.IsNullable = false;
            param.Size = 0;
            param.Precision = 0;
            param.SourceVersion = DataRowVersion.Default;
            command.Parameters.Add(param);
        }
        /// <summary>
        /// Add stored proc parameter in sql command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        public void AddOutParameter(SqlCommand command, string name, DbType dbType)
        {
            SqlParameter param = command.CreateParameter();
            param.DbType = dbType;
            param.ParameterName = name;
            param.Direction = ParameterDirection.Output;
            param.SourceColumn = String.Empty;
            param.IsNullable = false;
            param.Size = 0;
            param.Precision = 0;
            param.SourceVersion = DataRowVersion.Default;
            command.Parameters.Add(param);
        }
        /// <summary>
        /// Add stored proc parameter in sql command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        public void AddOutParameter(SqlCommand command, string name, SqlDbType dbType)
        {
            SqlParameter param = command.CreateParameter();
            param.SqlDbType = dbType;
            param.ParameterName = name;
            param.Direction = ParameterDirection.Output;
            param.SourceColumn = String.Empty;
            param.IsNullable = false;
            param.Size = 0;
            param.Precision = 0;
            param.SourceVersion = DataRowVersion.Default;
            command.Parameters.Add(param);
        }
        private bool _providesqlconnectionstring = false;
        public bool ProvideSQLConnectionString
        {
            get { return this._providesqlconnectionstring; }
            set { this._providesqlconnectionstring = value; }
        }


        /// <summary>
        /// Returns the database object
        /// </summary>
        /// <returns></returns>
        private Microsoft.Practices.EnterpriseLibrary.Data.Database GetDatabase()
        {
            if (!_providesqlconnectionstring)
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    return DatabaseFactory.CreateDatabase();
                }
                else
                {
                    return DatabaseFactory.CreateDatabase(_connectionString);
                }
            }
            else
            {
                return new SqlDatabase(_connectionString);
            }
        }

        #endregion
    }
}
