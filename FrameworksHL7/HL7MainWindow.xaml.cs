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
using System.Windows.Shapes;
using System.Xml;
using motInboundLib;

namespace FrameworksHL7
{


    /// <summary>
    /// Interaction logic for HL7MainWindow.xaml
    /// </summary>
    public partial class HL7MainWindow : Window
    {
        HL7Message __msg;
        XmlDocument __xdoc;

        public HL7MainWindow()
        {
            InitializeComponent();

            __msg = new HL7Message();

        }

        private void __start()
        {        
           while (true)
           {
               __xdoc = __msg.__wait();
               __update_tree();

           }           
        }
        public void __update_tree()
        {
                      
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
                    {
                        __start();                    
                    });

            button.IsEnabled = false;
                    
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            __start();
        }
    }
}
