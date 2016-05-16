using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Data.Odbc;
using System.Data.SqlClient;

/// <summary>
/// 
/// connection-string ::= empty-string[;] | attribute[;] | attribute; connection-string
/// empty-string ::=
/// attribute ::= attribute-keyword=attribute-value | DRIVER=[{]attribute-value[}]
/// attribute-keyword ::= DSN | UID | PWD | driver-defined-attribute-keyword
/// attribute-value ::= character-string
/// driver-defined-attribute-keyword ::= identifier
/// 
/// Windows Example: "Driver={Microsoft Access Driver (*.mdb)};DBQ=C:\\Samples\\Northwind.mdb"
/// 
/// 
/// PostgreSQL Example:  string szConnect = "DSN=dsnname;" + "UID=postgres;" + "PWD=********";
///                      OdbcConnection cnDB = new OdbcConnection(szConnect);
///                      
/// </summary>


namespace motInboundLib
{
    public enum dbType
    {
        NULLServer,
        SQLServer,
        NPGServer,
        ODBCServer
    };

    class motPostgreSQLServer
    {
        protected NpgsqlConnection connection;
        protected List<IDataRecord> __recordSet;

        public List<IDataRecord> executeQuery(string strQuery)
        {
            try
            {
                __recordSet.Clear();

                NpgsqlDataReader reader = null;
                NpgsqlCommand command = new NpgsqlCommand(strQuery, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    while (reader.Read())
                    {
                        __recordSet.Add((IDataRecord)reader);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return (List<IDataRecord>) null;
            }

            return __recordSet;
        }

        public motPostgreSQLServer(string DSN)
        {
            try
            {
                using (connection = new NpgsqlConnection(DSN))
                {
                    connection.Open();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ~motPostgreSQLServer()
        {
            connection.Close();
        }
    }

    class motSQLServer
    {
        protected SqlConnection connection;    
        protected List<IDataRecord> __recordSet;

        public List<IDataRecord> executeQuery(string strQuery)
        {
            
            //int __fieldNo = 0;

            try
            {
                __recordSet.Clear();

                SqlDataReader reader = null;
                SqlCommand command = new SqlCommand(strQuery,  connection);
                reader = command.ExecuteReader();

                while( reader.Read() )
                {
                    __recordSet.Add((IDataRecord)reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return (List<IDataRecord>)null;
            }

            return __recordSet;
        }

        public motSQLServer(string DSN)
        {
            try
            {
                using (connection = new SqlConnection(DSN))
                {
                    connection.Open();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        ~motSQLServer()
        {
            connection.Close();
        }
    }

    class motODBCServer
    {
        protected OdbcConnection connection;
        protected List<IDataRecord> __recordSet;

        public motODBCServer(string DSN)
        {
            try
            {
                using (connection = new OdbcConnection(DSN))
                {
                    connection.Open();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<IDataRecord> executeQuery(string strQuery)
        {

            try
            {
                __recordSet.Clear();

                OdbcDataReader reader = null;
                OdbcCommand command = new OdbcCommand(strQuery, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    while (reader.Read())
                    {
                        __recordSet.Add((IDataRecord)reader);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return (List<IDataRecord>) null;
            }

            return __recordSet;
        }
    }

   public class motDatabase
    {
        private dbType __wereA = dbType.NULLServer;
        private motSQLServer sqlServer;
        private motPostgreSQLServer npgServer;
        private motODBCServer odbcServer;

        public List<IDataRecord> executeQuery(string q)
        {
            try
            {
                switch(__wereA)
                {
                    case dbType.NPGServer:
                        return npgServer.executeQuery(q);

                    case dbType.ODBCServer:
                        return odbcServer.executeQuery(q);

                    case dbType.SQLServer:
                        return sqlServer.executeQuery(q);

                    default:
                        break;
                }
            }
            catch(Exception e)
            {
                throw e;
            }

            return (List<IDataRecord>)null;
        }

        public motDatabase() { }

        public motDatabase(string __connect, dbType __dbtype)
        {
            try
            {
                switch (__dbtype)
                {
                    case dbType.SQLServer:
                        sqlServer = new motSQLServer(__connect);
                        __wereA = dbType.SQLServer;
                        break;
                
                    case dbType.ODBCServer:
                        odbcServer = new motODBCServer(__connect);
                        __wereA = dbType.ODBCServer;
                        break;

                    case dbType.NPGServer:
                        npgServer = new motPostgreSQLServer(__connect);
                        __wereA = dbType.NPGServer;
                        break;

                    default:
                        break;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }

    public class databaseInputSource
    {
        protected motDatabase db;

        public virtual motDrugRecord getDrugRecord()
        {
            throw new NotImplementedException();
        }

        public virtual motLocationRecord getLocationRecord()
        {
            throw new NotImplementedException();
        }

        public virtual motPatientRecord getPatientRecord()
        {
            throw new NotImplementedException();
        }

        public virtual motPrescriptionRecord getPrescriptionRecord()
        {
            throw new NotImplementedException();
        }

        public virtual motPrescriberRecord getPrescriberRecord()
        {
            throw new NotImplementedException();
        }

        public virtual motStoreRecord getStoreRecord()
        {
            throw new NotImplementedException();
        }
        public virtual motTimeQtysRecord getTimeQtyRecord()
        {
            throw new NotImplementedException();
        }

        public databaseInputSource(dbType __type, string DSN)
        {
            try
            {
                db = new motDatabase(DSN, __type);
            }
            catch(Exception e)
            {
                throw new Exception("failed to create database object " + e.Message);
            }  
        }

        ~databaseInputSource() { }
    }
}
