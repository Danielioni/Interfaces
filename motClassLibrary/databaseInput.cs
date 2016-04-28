using System;
using System.Data.Odbc;

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
    class databaseInput
    {
        motDrugRecord getDrugRecord()
        {
            throw new NotImplementedException();
            //return new motDrugRecord();
        }

        motLocationRecord getLocationRecord()
        {
            throw new NotImplementedException();
            //return new motLocationRecord();
        }

        motPatientRecord getPatientRecord()
        {
            throw new NotImplementedException();
            //return new motPatientRecord();
        }

        motPrescriptionRecord getPrescriptionRecord()
        {
            throw new NotImplementedException();
            //return new motPrescriptionRecord();
        }

        motPrescriberRecord getPrescriberRecord()
        {
            throw new NotImplementedException();
            //return new motPrescriberRecord();
        }

        motStoreRecord getStoreRecord()
        {
            throw new NotImplementedException();
            //return new motStoreRecord();
        }

        motTimeQtysRecord getTimeQtyRecord()
        {
            throw new NotImplementedException();
            //return new motTimeQtysRecord();
        }

        void connectToDB(string conectString)
        {
            throw new NotImplementedException();
        }

        void openDB()
        {
            string connectionString = "Driver={Microsoft Access Driver (*.mdb)};DBQ=C:\\Samples\\Northwind.mdb"; //Mumble, default connection goes here ...

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();
            }

            throw new NotImplementedException();
        }

        void openDB(string DSN)
        {
            try {
                using (OdbcConnection connection = new OdbcConnection(DSN))
                {
                    connection.Open();
                }
            }
            catch(Exception e)
            {
                throw e;
            }

            throw new NotImplementedException();
        }

        void closeDB()
        {
            throw new NotImplementedException();
        }

        Object queryDB(string __query)
        {
            throw new NotImplementedException();
        }

        // Catchall function for things that the database does.  Should return a motRecord object.  Not sure how to return something derefereneable,
        // might need to be something like a motREcord base class or something.  Smart folks, step in here ...
        Object waitForNewData()
        {
            throw new NotImplementedException();
            //return false;
        }

        databaseInput() { }

        ~databaseInput() { }


 
    }
}
