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
using System.Text.RegularExpressions;

/// <summary>
/// Medicine-On-Time - motDatabase
/// 
///     A base class that abstracts SQL databases and provides query support and returns a DataSet for use by the caller.
///     
///     Usage:
///     
///         motDatabase __db = new motDatabase( __connect,  __dbtype);
///         
///         Where:
///         
///             __connect is a formated connection string for dbType
///             __dbTyype is an enumeration for the set of supported DBMS {SQLServer, NPGServer, ODBCServer, ...}
///             
///         bool __success = __db.Query(@"<<Query String Here>>");
///         
///             On __success, the query pupulates a public DataSet named __recordSet containing all returned data             
///         
///         Errors output to System.Console
///         
/// Medicine-On-Time - motDatabaseInputSource
/// 
///     A class for collecting specific data from system databases.  It provides a virtual method for each Medicine-On-Time
///     Legacy Interface Record type. Each is overridden to execute queries and process data to fill and write records to the
///     MOT database.
///     
///     Example:
///     
///         public override motTimeQtysRecord getTimeQtyRecord()
///         {
///             try
///                {
///                     string __query = "{Mumble::Mumble}";   // System Specific Query or View
///                     
///                     motTimeQtysRecord __tq = new motTimeQtysRecord();
///                     Dictionary<string, string> __xTable = new Dictionary<string, string>();
///
///                     // Load the translaton table -- Database Column Name to Gateway Tag Name                
///                     __xTable.Add("StoreLocation", "RxSys_LocID");
///                     __xTable.Add("ScheduleName", "DoseScheduleName");
///                     __xTable.Add("DoseTime", "DoseTimeQtys");
///
///                     string __tag;
///                     string __val;
///                     string __tmp;
///
///                     if ((__query.Length > 0) && db.executeQuery(__query))
///                     {
///                         foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
///                         {
///                             DataTable table = __record.Table;
///
///                             // Print the DataType of each column in the table. 
///                             foreach (DataColumn column in table.Columns)
///                             {
///                                 if (__xTable.TryGetValue(column.ColumnName, out __tmp))
///                                 {
///                                     __tag = __tmp;
///                                     __val = __record[column.ColumnName].ToString();
///
///                                     // Process Data Managabement Rules 
///                                     //if (__tag == "ZipCode")
///                                     //{
///                                     //    if (__xTable.TryGetValue("ZipPlus4", out __tmp))
///                                     //    {
///                                     //        __val += __record["ZipCode"].ToString();
///                                     //    }
///                                     //}
///
///                                     // Conversion rules
///                                     while (__val.Contains("-"))
///                                     {
///                                         __val = __val.Remove(__val.IndexOf("-"), 1);
///                                     }
/// 
///                                     // Update the local drug record
///                                     __tq.setField(__tag, __val, __override_length_checking);
///                                 }
///                             }
///
///                             // Write the record to the gateway
///                             __tq.Write(__port);
///                         }
/// 
///                         return __tq;
///                     }
///                 }
///                 catch (Exception e)
///                 {
///                     throw new Exception("Failed to get Drug Record " + e.Message);
///                 }
///
///                 return base.getTimeQtyRecord();
///             }
/// 
/// 
/// Connection String Examples
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

    public enum actionType
    {
        Add,
        Change,
        Delete
    };

    public class motPostgreSQLServer
    {
        private NpgsqlConnection connection;
        private NpgsqlDataAdapter adapter;

        public void executeNonQuery(string strQuery)
        {

        }

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

        public void executeNonQuery(string strQuery)
        {
            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;

                    string[] scripts = Regex.Split(strQuery, @"^\w+GO$", RegexOptions.Multiline);

                    //string[] scripts = strQuery.Split("GO");

                    foreach (string splitScript in scripts)
                    {
                        command.CommandText = splitScript.Substring(0, splitScript.ToLower().IndexOf("go"));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to execute nonQuery {0}", e.Message);
            }
        }

        public bool executeQuery(string strQuery, DataSet __recordSet_)
        {

            try
            {
                __recordSet_.Clear();

                adapter = new SqlDataAdapter(strQuery, connection);
                adapter.Fill(__recordSet_, "__table");
                /*
                 * 		e	{"Incorrect syntax near 'GO'."}	System.Exception {System.Data.SqlClient.SqlException}
                 * 		Procedure	"vMOTPatient"	string
                 		Server	"DMPCPR"	string
                        StackTrace	"   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)\r\n   
                                at System.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)\r\n  
                                at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)\r\n   
                                at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)\r\n   
                                at System.Data.SqlClient.SqlDataReader.TryConsumeMetaData()\r\n   at System.Data.SqlClient.SqlDataReader.get_MetaData()\r\n   at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString)\r\n   
                                at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async, Int32 timeout, Task& task, Boolean asyncWrite, SqlDataReader ds, Boolean describeParameterEncryptionRequest)\r\n   
                                at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean asyncWrite)\r\n   
                                at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method)\r\n   
                                at System.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior, String method)\r\n   at System.Data.SqlClient.SqlCommand.ExecuteDbDataReader(CommandBehavior behavior)\r\n   at System.Data.Common.DbCommand.System.Data.IDbCommand.ExecuteReader(CommandBehavior behavior)\r\n   
                                at System.Data.Common.DbDataAdapter.FillInternal(DataSet dataset, DataTable[] datatables, Int32 startRecord, Int32 maxRecords, String srcTable, IDbCommand command, CommandBehavior behavior)\r\n   
                                at System.Data.Common.DbDataAdapter.Fill(DataSet dataSet, Int32 startRecord, Int32 maxRecords, String srcTable, IDbCommand command, CommandBehavior behavior)\r\n   at System.Data.Common.DbDataAdapter.Fill(DataSet dataSet, String srcTable)\r\n  
                                at motInboundLib.motSQLServer.executeQuery(String strQuery, DataSet __recordSet_) in C:\\Users\\pjenney\\Source\\Repos\\Interfaces\\motClassLibrary\\databaseInput.cs:line 218"	string


                 */

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

        public void executeNonQuery(string strQuery)
        {
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
                        return odbcServer.executeQuery(q, __recordSet);

                    case dbType.SQLServer:
                        return sqlServer.executeQuery(q, __recordSet);

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

        public bool executeNonQuery(string q)
        {
            try
            {
                switch (__wereA)
                {
                    case dbType.NPGServer:
                        npgServer.executeNonQuery(q);
                        break;

                    case dbType.ODBCServer:
                        odbcServer.executeNonQuery(q);
                        break;

                    case dbType.SQLServer:
                        sqlServer.executeNonQuery(q);
                        break;

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

        public bool setView(string __view)
        {
            try
            {
                switch (__wereA)
                {
                    case dbType.NPGServer:
                        return npgServer.executeQuery(__view, __recordSet);

                    case dbType.ODBCServer:
                        return odbcServer.executeQuery(__view, __recordSet);

                    case dbType.SQLServer:
                        return sqlServer.executeQuery(__view, __recordSet);

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

        // Setters (used for adds, changes and deletes) 
        public virtual bool setDrugRecord(actionType __action)
        {
            throw new NotImplementedException();
        }
        public virtual bool setLocationRecord(actionType __action)
        {
            throw new NotImplementedException();
            return false;
        }
        public virtual bool setPatientRecord(actionType __action)
        {
            throw new NotImplementedException();
        }
        public virtual bool setPrescriptionRecord(actionType __action)
        {
            throw new NotImplementedException();
        }
        public virtual bool setPrescriberRecord(actionType __action)
        {
            throw new NotImplementedException();
        }
        public virtual bool setStoreRecord(actionType __action)
        {
            throw new NotImplementedException();
        }
        public virtual bool setTimeQtyRecord(actionType __action)
        {
            throw new NotImplementedException();
        }

        // Getters
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
