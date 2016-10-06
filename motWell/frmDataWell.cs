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
    public partial class frmDataWell : Form
    {
        motFileSystemListener __fsw;

        public frmDataWell()
        {
            InitializeComponent();
        }

        public void startWatching()
        {
            __fsw = new motFileSystemListener("C:/MOT_IO", "127.0.0.1", "24041");
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
