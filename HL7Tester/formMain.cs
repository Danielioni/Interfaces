using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

using motInboundLib;

namespace HL7Tester
{
    public partial class formMain : Form
    {
        HL7Message __msg;
        XmlDocument __xdoc;

        public formMain()
        {
            InitializeComponent();

            __msg = new HL7Message();
        }

        private void __update_tree()
        {
            try
            {
                BeginInvoke(new Action(() =>
                {
                    treeView.Nodes.Clear();
                    treeView.Nodes.Add(new TreeNode(__xdoc.DocumentElement.Name));
                    TreeNode tNode = new TreeNode();
                    tNode = treeView.Nodes[0];
                    
                    AddNode(__xdoc.DocumentElement, tNode);
                    treeView.ExpandAll();
                }));
            }
            catch (XmlException xmlEx)
            {
                MessageBox.Show(xmlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;

            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        private void __start()
        {
            while (true)
            {
                Thread.Yield();
                __xdoc = __msg.__wait();
                __update_tree();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // This will start the listener and call the callback 
            Thread __worker = new Thread(new ThreadStart(__start));
            __worker.Name = "waiter";
            __worker.Start();
          
            // Now get on with your life ...
        }

        public bool __get_drug_record()
        {
            Dictionary<string, int> __roman = new Dictionary<string, int>();

            __roman.Add("I", 1);
            __roman.Add("II", 2);
            __roman.Add("III", 3);
            __roman.Add("IV", 4);
            __roman.Add("V", 5);
            __roman.Add("VI", 6);
            __roman.Add("VII", 7);
            __roman.Add("VIII", 8);
            __roman.Add("IX", 9);
            __roman.Add("X", 10);

            try
            {
                motDrugRecord d = new motDrugRecord("Add");

                d.RxSys_DrugID =  __msg.__get_field_data(__msg.__mot_fields["RxSys_DrugID"]);
                d.LabelCode =  __msg.__get_field_data(__msg.__mot_fields["LblCode"]);
                d.ProductCode = __msg.__get_field_data(__msg.__mot_fields["ProdCode"]);
                d.TradeName = __msg.__get_field_data(__msg.__mot_fields["TradeName"]);
                d.Strength = Convert.ToInt32(__msg.__get_field_data(__msg.__mot_fields["Strength"]));
                d.Unit = __msg.__get_field_data(__msg.__mot_fields["Unit"]);
                d.RxOTC = __msg.__get_field_data(__msg.__mot_fields["RxOtc"]);
                d.DoseForm = __msg.__get_field_data(__msg.__mot_fields["DoseForm"]);
                d.Route = __msg.__get_field_data(__msg.__mot_fields["Route"]);

                string __tmp = __msg.__get_field_data(__msg.__mot_fields["DrugSchedule"]);
                if(__tmp.ToUpper().Contains("I") || __tmp.ToUpper().Contains("V") || __tmp.ToUpper().Contains("X"))
                {
                    d.DrugSchedule = __roman[__tmp];
                }
                else
                {
                    d.DrugSchedule = Convert.ToInt32(__tmp);
                }
               

                d.VisualDescription = __msg.__get_field_data(__msg.__mot_fields["VisualDescription"]);
                d.DrugName = __msg.__get_field_data(__msg.__mot_fields["DrugName"]);
                d.ShortName = __msg.__get_field_data(__msg.__mot_fields["ShortName"]);
                d.NDCNum = __msg.__get_field_data(__msg.__mot_fields["NDCNum"]);
                d.SizeFactor = Convert.ToInt32(__msg.__get_field_data(__msg.__mot_fields["SizeFactor"]));
                d.Template = __msg.__get_field_data(__msg.__mot_fields["Template"]);
                d.DefaultIsolate = Convert.ToInt32(__msg.__get_field_data(__msg.__mot_fields["DefaultIsolate"]));
                d.ConsultMsg = __msg.__get_field_data(__msg.__mot_fields["ConsultMsg"]);
                d.GenericFor = __msg.__get_field_data(__msg.__mot_fields["GenericFor"]);
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to populate drug record {0}", e.Message);
                return false;
            }



            return true;
        }

        public bool __get_prescription_record()
        {
            return true;
        } 


    }
}
