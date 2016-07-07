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

namespace motWell
{
    public partial class Form1 : Form
    {
        fileSystemWatcher __fsw;

        public Form1()
        {
            InitializeComponent();
        }

        public void startWatching()
        {
            __fsw = new fileSystemWatcher("C:/MOT_IO", "127.0.0.1", "24042");
        }

        public void stopWatching()
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            startWatching();
        }
    }
}
