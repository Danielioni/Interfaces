using System;
using System.Collections.Generic;
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
    class motSQLServer
    {
        protected SqlConnection connection;
        protected List<Field> __record;

        public List<Field> executeQuery(string strQuery)
        {
            
            int __fieldNo = 0;

            try
            {
                __record.Clear();

                SqlDataReader reader = null;
                SqlCommand command = new SqlCommand(strQuery,  connection);
                reader = command.ExecuteReader();

                while( reader.Read() )
                {
                    __record.Add( new Field(reader.GetName(__fieldNo).ToString(),
                                            reader.GetValue(__fieldNo).ToString(), 
                                            0, 
                                            false, 
                                            'z'));

                    __fieldNo++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return __record;
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
        protected List<Field> __record;

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

        public List<Field> executeQuery(string strQuery)
        {
            int __fieldNo = 0;

            try
            {
                __record.Clear();

                OdbcDataReader reader = null;
                OdbcCommand command = new OdbcCommand(strQuery, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    __record.Add(new Field(reader.GetName(__fieldNo).ToString(),
                                            reader.GetValue(__fieldNo).ToString(),
                                            0,
                                            false,
                                            'z'));

                    __fieldNo++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return __record;
        }
    }

    class motDatabase
    {
        private bool isODBC;
        private motSQLServer sqlServer;
        private motODBCServer odbcServer;

        public List<Field> executeQuery(string q)
        {
            try
            {
                if (isODBC)
                {
                    return odbcServer.executeQuery(q);
                }
                else
                {
                    return sqlServer.executeQuery(q);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public motDatabase() { }

        public motDatabase(string __connect, bool isSQLServer)
        {
            try
            {
                if (isSQLServer)
                {
                    sqlServer = new motSQLServer(__connect);
                    isODBC = false;
                    isSQLServer = true;
                }
                else
                {
                    odbcServer = new motODBCServer(__connect);
                    isODBC = true;
                    isSQLServer = false;
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
        motDatabase db;

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

        public databaseInputSource(string __type, string DSN)
        {
            if(__type.ToUpper() == "ODBC" )
            {
                db = new motDatabase(DSN, false);
            }
            else
            {
                db = new motDatabase(DSN, true);
            }
        }

        ~databaseInputSource() { }
    }
}
