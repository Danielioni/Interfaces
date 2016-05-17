using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using motInboundLib;

namespace CPRPlusInterface
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Port __port;
        public string __DSN;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            txtResponse.Clear();

            try
            {
                cprPlus cpr;

                switch (cbDBType.SelectedIndex)
                {
                    // @"server=127.0.0.1;port=5432;userid=mot;password=mot!cool;database=Mot");
                    case 0:  // ODBC
                        cpr = new cprPlus(dbType.ODBCServer,
                                            @"server=" + txtDSNAddress.Text + ";" +
                                            @"port=" + txtDSNPort.Text + ";" +
                                            @"userid=" + txtUname.Text + ";" +
                                            @"password=" + txtDBPassword.Text + ";" +
                                            @"database=" + txtDatabase.Text);
                        break;

                    case 1:
                        cpr = new cprPlus(dbType.SQLServer,
                                            @"server=" + txtDSNAddress.Text + ";" +
                                            @"port=" + txtDSNPort.Text + ";" +
                                            @"userid=" + txtUname.Text + ";" +
                                            @"password=" + txtDBPassword.Text + ";" +
                                            @"database=" + txtDatabase.Text);
                        break;

                    case 2:
                        cpr = new cprPlus(dbType.NPGServer,
                                            @"server=" + txtDSNAddress.Text + ";" +
                                            @"port=" + txtDSNPort.Text + ";" +
                                            @"userid=" + txtUname.Text + ";" +
                                            @"password=" + txtDBPassword.Text + ";" +
                                            @"database=" + txtDatabase.Text);
                        break;
                }

                txtResponse.AppendText(@"DSN Is Good To Go!");

            }
            catch (Exception err)
            {
                txtResponse.AppendText("Failed to open Database for input " + err.Message);
            }
        }

        private void btnTestPort_Click(object sender, RoutedEventArgs e)
        {
            txtResponse.Clear();

            try
            {
                Port p = new Port(txtAddress.Text, txtPort.Text);
                txtResponse.AppendText(@"Address Is Good To Go!");
            }
            catch(Exception err)
            {
                txtResponse.AppendText( @"Address Test Error: " + err.Message);
            }
        }

        private void btnKeep_Click(object sender, RoutedEventArgs e)
        {
            __DSN = @"server=" + txtDSNAddress.Text + ";" +
                    @"port=" + txtDSNPort.Text + ";" +
                    @"userid=" + txtUname.Text + ";" +
                    @"password=" + txtDBPassword.Text + ";" +
                    @"database=" + txtDatabase.Text;

            __port = new Port(txtAddress.Text, txtPort.Text);
        }
    }

    class cprPlus : databaseInputSource
    {
        public cprPlus(dbType __type, string DSN) : base(__type, DSN)
        { }

        // Find all new Drug Records and add them to the system
        public override motPrescriptionRecord getPrescriptionRecord()
        {
            try
            {
                motPrescriberRecord __scrip = new motPrescriberRecord();
                Dictionary<string, string> __xTable = new Dictionary<string, string>();
                Port p = new Port("127.0.0.1", "24042");

                // Load the translaton table -- Database Column Name to Gateway Tag Name 
                              
                __xTable.Add("RxSys_RxNum", "");
                __xTable.Add("RxSys_PatID", "");
                __xTable.Add("RxSys_DocID", "");
                __xTable.Add("RxSys_DrugID", "");
                __xTable.Add("Sig", "");
                __xTable.Add("RxStartDate", "");
                __xTable.Add("RxStopDate", "");
                __xTable.Add("DiscontinueDate", "");
                __xTable.Add("DoseScheduleName", "");
                __xTable.Add("Comments", "");
                __xTable.Add("Refills", "");
                __xTable.Add("RxSys_NewRxNum", "");
                __xTable.Add("Isolate", "");
                __xTable.Add("RxType", "");
                __xTable.Add("MDOMStart", "");
                __xTable.Add("MDOMEnd", "");
                __xTable.Add("QtyPerDose", "");
                __xTable.Add("QtyDispensed", "");
                __xTable.Add("Status", "");
                __xTable.Add("DoW", "");
                __xTable.Add("SpecialDoses", "");
                __xTable.Add("DoseTimeQtys", "");
                __xTable.Add("ChartOnly", "");
                __xTable.Add("AnchorDate", "");
                

                List<IDataRecord> __recordSet = db.executeQuery("SELECT * from Rxes");

                foreach (IDataRecord __record in __recordSet)
                {
                    for (int i = 0; i < __record.FieldCount; i++)
                    {
                        __scrip.setField(__xTable[__record.GetName(i).ToString()],  // Column
                                                  __record.GetValue(i).ToString()); // Value
                    }

                    __scrip.Write(p);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Drug Record " + e.Message);
            }

            return base.getPrescriptionRecord();
        }
    }
}

