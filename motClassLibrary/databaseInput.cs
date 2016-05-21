// 
// MIT license
//
// Copyright (c) 2016 by Peter H. Jenney and Medicine-On-Time, LLC.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 

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

    public class motPostgreSQLServer
    {
        private NpgsqlConnection connection;
        private NpgsqlDataAdapter adapter;
        
        public bool executeQuery(string strQuery, DataSet __recordSet_)
        {
            try
            {
                __recordSet_.Clear();

                adapter = new NpgsqlDataAdapter(strQuery, connection);
                adapter.Fill(__recordSet_, "__table");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public motPostgreSQLServer(string DSN)
        {
            try
            {
                connection = new NpgsqlConnection(DSN);
                connection.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ~motPostgreSQLServer()
        {
            try
            {
                connection.Close();
            }
            catch (Exception e)
            {
                e = null;
            }
        }
    }

    public class motSQLServer
    {
        private SqlConnection connection;
        private SqlDataAdapter adapter;

        public bool executeQuery(string strQuery, DataSet __recordSet_)
        {

            try
            {
                __recordSet_.Clear();

                adapter = new SqlDataAdapter(strQuery, connection);
                adapter.Fill(__recordSet_, "__table");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public motSQLServer(string DSN)
        {
            try
            {
                connection = new SqlConnection(DSN);
                connection.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ~motSQLServer()
        {
            try
            {
                connection.Close();
            }
            catch (Exception e)
            {
                e = null;
            }
        }
    }

    public class motODBCServer
    {
        private OdbcConnection connection;
        private OdbcDataAdapter adapter;

        public motODBCServer(string DSN)
        {
            try
            {
                connection = new OdbcConnection(DSN);
                connection.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool executeQuery(string strQuery, DataSet __recordSet_)
        {
            __recordSet_.Clear();

            try
            {
                __recordSet_.Clear();

                adapter = new OdbcDataAdapter(strQuery, connection);
                adapter.Fill(__recordSet_, "__table");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }

    public class motDatabase
    {
        private dbType __wereA = dbType.NULLServer;
        private motSQLServer sqlServer;
        private motPostgreSQLServer npgServer;
        private motODBCServer odbcServer;
        public DataSet __recordSet;

        public bool executeQuery(string q)
        {
            try
            {
                switch (__wereA)
                {
                    case dbType.NPGServer:
                        return npgServer.executeQuery(q, __recordSet);

                    case dbType.ODBCServer:
                        return  odbcServer.executeQuery(q, __recordSet);

                    case dbType.SQLServer:
                        return  sqlServer.executeQuery(q, __recordSet);

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return false;
        }

        public motDatabase() { }

        public motDatabase(string __connect, dbType __dbtype)
        {
            try
            {
                __recordSet = new DataSet("__table");

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
            catch (Exception e)
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
            catch (Exception e)
            {
                throw new Exception("failed to create database object " + e.Message);
            }
        }

        ~databaseInputSource() { }
    }
}
