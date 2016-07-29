using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using motInboundLib;

namespace motHL7
{
    public partial class Form1 : Form
    {
        HL7SocketListener __listener;

        public Form1()
        {
            InitializeComponent();
        }

     
        private void button1_Click(object sender, EventArgs e)
        {
            __listener = new HL7SocketListener(21110);
            __listener.start();

            //MSH __msh = new MSH(@"MSH|^~\&|ADT1|MCM|LABADT|MCM|198808181126|SECURITY|ADT^A01|MSG00001-|P|2.6");
        }

        
    }
}