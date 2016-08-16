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
using motInboundLib;

namespace motHttpInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        motHttpListener _listener;
        volatile string _targetUri = string.Empty;


        public MainWindow()
        {
            InitializeComponent();
            tbxURI.Text = Properties.Settings.Default.TargetHttp;
            _listener = new motHttpListener();
            //the event handler uses the following format to allow the updates to UI from threads other than the calling thread
            _listener.OutputTextChanged += (s, e) => Dispatcher.Invoke(() => HandlerHttpOutputChange(s, e));
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void ListenerStart_Click(object sender, RoutedEventArgs e)
        {
            

            try
            {
                _targetUri = tbxURI.Text;
                _listener.StartListener(_targetUri);
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;


            }
            catch (Exception ex)
            {
                string _err = string.Format("An error occurred while attempting to start the Http listener: {0}\nExiting ...", ex.Message);
               
               
            }

        }

        private void ListenerStop_Click(object sender, RoutedEventArgs e)
        {
            _listener.StopListener();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;

        }

       

        private void HandlerHttpOutputChange(object sender, motHttpEventArgs e)
        {
            tBoxOutput.AppendText(string.Format("{0} : {1} \r\n",  e._timeStamp, e._outputMessage));
            tBoxOutput.UpdateLayout();
        }
    }
}
