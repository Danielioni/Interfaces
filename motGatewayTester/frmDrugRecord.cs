﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using motCommonLib;


namespace motGatewayTester
{
    public partial class frmDrugRecord : Form
    {
        Execute __execute;
        string __action = "Add";
        motDrugRecord Drug;

        public frmDrugRecord()
        {
            InitializeComponent();
            Drug = new motDrugRecord(__action, motErrorlLevel.Info, false);
            __execute = frmMainRecordTester.__execute;
        }

        private void txtRxSys_DrugID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_DrugID.Text = txtRxSys_DrugID.Text.Length.ToString();
        }

        private void txtLblCode_TextChanged(object sender, EventArgs e)
        {
            valLblCode.Text = txtLblCode.Text.Length.ToString();
        }

        private void txtProdCode_TextChanged(object sender, EventArgs e)
        {
            valProdCode.Text = txtProdCode.Text.Length.ToString();
        }

        private void txtTradename_TextChanged(object sender, EventArgs e)
        {
            valTradename.Text = txtTradename.Text.Length.ToString();
        }

        private void txtStrength_TextChanged(object sender, EventArgs e)
        {
            valStrength.Text = txtStrength.Text.Length.ToString();
        }

        private void txtUnit_TextChanged(object sender, EventArgs e)
        {
            valUnit.Text = txtUnit.Text.Length.ToString();
        }

        private void txtRxOtc_TextChanged(object sender, EventArgs e)
        {
            valRxOtc.Text = txtRxOtc.Text.Length.ToString();
        }

        private void txtDoseForm_TextChanged(object sender, EventArgs e)
        {
            valDoseForm.Text = txtDoseForm.Text.Length.ToString();
        }

        private void txtRoute_TextChanged(object sender, EventArgs e)
        {
            valRoute.Text = txtRoute.Text.Length.ToString();
        }

        private void txtDrugSchedule_TextChanged(object sender, EventArgs e)
        {
            valDrugSchedule.Text = txtDrugSchedule.Text.Length.ToString();
        }

        private void txtVisualDescription_TextChanged(object sender, EventArgs e)
        {
            valVisualDescription.Text = txtVisualDescription.Text.Length.ToString();
        }

        private void txtDrugName_TextChanged(object sender, EventArgs e)
        {
            valDrugName.Text = txtDrugName.Text.Length.ToString();
        }

        private void txtShortName_TextChanged(object sender, EventArgs e)
        {
            valShortName.Text = txtShortName.Text.Length.ToString();
        }

        private void txtNDCNum_TextChanged(object sender, EventArgs e)
        {
            valNDCNum.Text = txtNDCNum.Text.Length.ToString();
        }

        private void txtSizeFactor_TextChanged(object sender, EventArgs e)
        {
            valSizeFactor.Text = txtSizeFactor.Text.Length.ToString();
        }

        private void txtTemplate_TextChanged(object sender, EventArgs e)
        {
            valTemplate.Text = txtTemplate.Text.Length.ToString();
        }

        private void txtDefaultIsolate_TextChanged(object sender, EventArgs e)
        {
            valDefaultIsolate.Text = txtDefaultIsolate.Text.Length.ToString();
        }

        private void txtConsultMsg_TextChanged(object sender, EventArgs e)
        {
            valConsultMsg.Text = txtConsultMsg.Text.Length.ToString();
        }

        private void txtGenericFor_TextChanged(object sender, EventArgs e)
        {
            valGenericFor.Text = txtGenericFor.Text.Length.ToString();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                Drug.RxSys_DrugID = txtRxSys_DrugID.Text;
                Drug.LabelCode = txtLblCode.Text;
                Drug.ProductCode = txtProdCode.Text;
                Drug.TradeName = txtTradename.Text;
                Drug.Strength = Convert.ToInt32(txtStrength.Text);
                Drug.Unit = txtUnit.Text;
                Drug.RxOTC = txtRxOtc.Text;
                Drug.DoseForm = txtDoseForm.Text;
                Drug.Route = txtRoute.Text;
                Drug.DrugSchedule = Convert.ToInt32(txtDrugSchedule.Text);
                Drug.VisualDescription = txtVisualDescription.Text;
                Drug.DrugName = txtDrugName.Text;
                Drug.ShortName = txtShortName.Text;
                Drug.NDCNum = txtNDCNum.Text;
                Drug.SizeFactor = Convert.ToInt32(txtSizeFactor.Text);
                Drug.Template = txtTemplate.Text;
                Drug.DefaultIsolate = Convert.ToInt32(txtDefaultIsolate.Text);
                Drug.ConsultMsg = txtConsultMsg.Text;
                Drug.GenericFor = txtGenericFor.Text;

                __execute.__update_event_ui("Drug field assignment complete ...");
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Drug record field assignment error: {0}", ex.Message));
                return;
            }

            try
            {
                Drug.Write(__execute.__p);
                __execute.__update_event_ui("Drug write record complete ...");
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Drug record write error: {0}", ex.Message));
                return;
            }
        }

        private void rbChange_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Change";
        }

        private void rbDelete_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Delete";
        }

        private void rbAdd_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Add";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            __execute.__auto_truncate = checkBox1.Checked;
        }
    }
}