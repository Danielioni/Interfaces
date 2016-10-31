using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using motCommonLib;
using motOutboundLib;

namespace motGatewayTester
{
    public partial class frmPatientRecord : Form
    {
        Execute __execute;
        string __action = "Add";

        motPatientRecord Patient;
        public frmPatientRecord()
        {
            InitializeComponent();
            Patient = new motPatientRecord(__action, false);
            __execute = frmMainRecordTester.__execute;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void txtAllergies_Click(object sender, EventArgs e)
        {                    
        }

        private void txtAllergies_Enter(object sender, EventArgs e)
        {
            txtAllergies.Height *= 5;
            txtAllergies.BringToFront();
        }

        private void txtAllergies_Leave(object sender, EventArgs e)
        {
            txtAllergies.Height /= 5;
            txtAllergies.SendToBack();
        }

        private void txtDiet_Enter(object sender, EventArgs e)
        {
            txtDiet.Height *= 5;
            txtDiet.BringToFront();
        }

        private void txtDiet_Leave(object sender, EventArgs e)
        {
            txtDiet.Height /= 5;
            txtDiet.SendToBack();
        }

        private void txtDxNote_Enter(object sender, EventArgs e)
        {
            txtDxNote.Height *= 5;
            txtDxNote.BringToFront();
        }

        private void txtDxNote_Leave(object sender, EventArgs e)
        {
            txtDxNote.Height /= 5;
            txtDxNote.SendToBack();
        }

        private void txtTreatmentNotes_Enter(object sender, EventArgs e)
        {
            txtTreatmentNotes.Height *= 5;
            txtTreatmentNotes.BringToFront();
        }

        private void txtTreatmentNotes_Leave(object sender, EventArgs e)
        {
            txtTreatmentNotes.Height /= 5;
            txtTreatmentNotes.SendToBack();
        }

        private void txtComments_Enter(object sender, EventArgs e)
        {
            txtComments.Height *= 5;
            txtComments.BringToFront();
        }

        private void txtComments_Leave(object sender, EventArgs e)
        {
            txtComments.Height /= 5;
            txtComments.SendToBack();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                if ((cbDelimitedTestRecord.Checked || cbTaggedTestRecord.Checked) && txtFileName.Text.Length > 0)
                {
                    List<KeyValuePair<string, string>> __key_data = new List<KeyValuePair<string, string>>();
                    var __output = new motFormattedFileOutput();

                    if (!Directory.Exists("C:/Records"))
                    {
                        Directory.CreateDirectory("C:/Records");
                    }

                    if (cbDelimitedTestRecord.Checked)
                    {
                        __key_data.Add(new KeyValuePair<string, string>("TableType", "A" + __action.ToUpper().Substring(0, 1)));
                    }

                    __key_data.Add(new KeyValuePair<string, string>("RxSys_PatID", txtRxSys_PatID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("LastName", txtLastName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("FirstName", txtFirstName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("MiddleInitial", txtMiddleInitial.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address1", txtAddress1.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address2", txtAddress2.Text));
                    __key_data.Add(new KeyValuePair<string, string>("City", txtCity.Text));
                    __key_data.Add(new KeyValuePair<string, string>("State", txtState.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Zip", txtZip.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Phone1", txtPhone1.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Phone2", txtPhone2.Text));
                    __key_data.Add(new KeyValuePair<string, string>("WorkPhone", txtWorkPhone.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_LocID", txtRxSys_LocID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Room", txtRoom.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Comments", txtComments.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Gender", txtGender.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Status", txtStatus.Text));
                    __key_data.Add(new KeyValuePair<string, string>("CycleDate", txtCycleDate.Text));
                    __key_data.Add(new KeyValuePair<string, string>("CycleDays", txtCycleDays.Text));
                    __key_data.Add(new KeyValuePair<string, string>("CycleType", txtCycleType.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Status", txtStatus.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_LastDoc", txtRxSys_LastDoc.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_PrimaryDoc", txtRxSys_PrimaryDoc.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_AltDoc", txtRxSys_AltDoc.Text));
                    __key_data.Add(new KeyValuePair<string, string>("SSN", txtSSN.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Allergies", txtAllergies.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Diet", txtDiet.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DxNotes", txtDxNote.Text));
                    __key_data.Add(new KeyValuePair<string, string>("TreatmentNotes", txtTreatmentNotes.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DOB", txtDOB.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Height", txtHeight.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Weight", txtWeight.Text));
                    __key_data.Add(new KeyValuePair<string, string>("ResponsibleName", txtResponsibleName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("InsName", txtInsName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("InsPNo", txtInsPNo.Text));
                    __key_data.Add(new KeyValuePair<string, string>("AltInsName", txtAltInsName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("AltInsPNo", txtAltInsPNo.Text));
                    __key_data.Add(new KeyValuePair<string, string>("MCareNum", txtMCareNum.Text));
                    __key_data.Add(new KeyValuePair<string, string>("MCaidNum", txtMCaidNum.Text));
                    __key_data.Add(new KeyValuePair<string, string>("AdmitDate", txtAdmitDate.Text));
                    __key_data.Add(new KeyValuePair<string, string>("ChartOnly", txtChartOnly.Text));

                    if (cbDelimitedTestRecord.Checked)
                    {
                        byte[] __record = __output.WriteDelimitedFile(@"C:\Records\" + txtFileName.Text, __key_data);

                        if (cbSendDelimitedRecord.Checked)
                        {
                            __execute.__p.write(__record);
                        }
                    }

                    if (cbTaggedTestRecord.Checked)
                    {
                        if (cbDelimitedTestRecord.Checked)
                        {
                            __key_data.RemoveAt(0);
                        }

                        string __record = __output.WriteTaggedFile(@"C:\Records\" + txtFileName.Text, __key_data, "Patient", __action);

                        if (cbSendTaggedRecord.Checked)
                        {
                            __execute.__p.write(__record);
                        }
                    }
                }
                else
                {
                    Patient.setField("Action", __action);

                    // Assign all the values and write
                    Patient.RxSys_PatID = txtRxSys_PatID.Text;
                    Patient.LastName = txtLastName.Text;
                    Patient.MiddleInitial = txtMiddleInitial.Text;
                    Patient.FirstName = txtFirstName.Text;
                    Patient.Address1 = txtAddress1.Text;
                    Patient.Address2 = txtAddress2.Text;
                    Patient.City = txtCity.Text;
                    Patient.PostalCode = txtZip.Text;
                    Patient.Phone1 = txtPhone1.Text;
                    Patient.Phone2 = txtPhone2.Text;
                    Patient.WorkPhone = txtWorkPhone.Text;
                    Patient.Room = txtRoom.Text;

                    Patient.Comments = txtComments.Text;
                    Patient.Allergies = txtAllergies.Text;
                    Patient.Diet = txtDiet.Text;
                    Patient.DxNotes = txtDxNote.Text;
                    Patient.TreatmentNotes = txtTreatmentNotes.Text;
                    Patient.ResponisbleName = txtResponsibleName.Text;

                    Patient.DOB = txtDOB.Text;
                    Patient.Height = Convert.ToInt32(txtHeight.Text);
                    Patient.Weight = Convert.ToInt32(txtWeight.Text);

                    Patient.InsName = txtInsName.Text;
                    Patient.AltInsName = txtAltInsName.Text;
                    Patient.InsPNo = txtAltInsName.Text;
                    Patient.AltInsPNo = txtAltInsPNo.Text;
                    Patient.MedicareNum = txtMCareNum.Text;
                    Patient.MedicaidNum = txtMCaidNum.Text;

                    Patient.CycleDate = txtCycleDate.Text;
                    Patient.CycleDays = Convert.ToInt32(txtCycleDays.Text);
                    Patient.CycleType = Convert.ToInt32(txtCycleType.Text);

                    Patient.Status = Convert.ToInt32(txtStatus.Text);
                    Patient.SSN = txtSSN.Text;

                    Patient.AdmitDate = txtAdmitDate.Text;
                    Patient.ChartOnly = txtChartOnly.Text;
                    Patient.Gender = txtGender.Text;

                    Patient.RxSys_PatID = txtRxSys_PatID.Text;
                    Patient.RxSys_LocID = txtRxSys_LocID.Text;
                    Patient.RxSys_PrimaryDoc = txtRxSys_PrimaryDoc.Text;
                    Patient.RxSys_LastDoc = txtRxSys_LastDoc.Text;
                    Patient.RxSys_AltDoc = txtRxSys_AltDoc.Text;

                    __execute.__update_event_ui("Patient field assignment complete ...");

                    try
                    {
                        Patient.Write(__execute.__p);
                        
                    }
                    catch (Exception ex)
                    {
                        __execute.__update_error_ui(string.Format("Patient record write error: {0}", ex.Message));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Patient record field assignment error: {0}", ex.Message));
                return;
            }

            __execute.__update_event_ui("Patient write record complete ...");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            __execute.__auto_truncate = checkBox1.Checked;
        }

        private void gbSwitches_Enter(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void txtRxSys_PatID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_PatID.Text = txtRxSys_PatID.Text.Length.ToString();
        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            valLastName.Text = txtLastName.Text.Length.ToString();
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            valFirstName.Text = txtFirstName.Text.Length.ToString();
        }

        private void txtMiddleInitial_TextChanged(object sender, EventArgs e)
        {
            valMiddleInitial.Text = txtMiddleInitial.Text.Length.ToString();
        }

        private void txtAddress1_TextChanged(object sender, EventArgs e)
        {
            valAddress1.Text = txtAddress1.Text.Length.ToString();
        }

        private void txtAddress2_TextChanged(object sender, EventArgs e)
        {
            valAddress2.Text = valAddress2.Text.Length.ToString();
        }

        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            valCity.Text = txtCity.Text.Length.ToString();
        }

        private void txtState_TextChanged(object sender, EventArgs e)
        {
            valState.Text = txtState.Text.Length.ToString();
        }

        private void txtZip_TextChanged(object sender, EventArgs e)
        {
            valZip.Text = txtZip.Text.Length.ToString();
        }

        private void txtPhone1_TextChanged(object sender, EventArgs e)
        {
            valPhone1.Text = txtPhone1.Text.Length.ToString();
        }

        private void txtPhone2_TextChanged(object sender, EventArgs e)
        {
            valPhone2.Text = txtPhone2.Text.Length.ToString();
        }

        private void txtWorkPhone_TextChanged(object sender, EventArgs e)
        {
            valWorkPhone.Text = txtWorkPhone.Text.Length.ToString();
        }

        private void txtRxSys_LocID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_LocID.Text = txtRxSys_LocID.Text.Length.ToString();
        }

        private void txtRoom_TextChanged(object sender, EventArgs e)
        {
            valRoom.Text = txtRoom.Text.Length.ToString();
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            valComments.Text = txtComments.Text.Length.ToString();
        }

        private void txtCycleDate_TextChanged(object sender, EventArgs e)
        {
            valCycleDate.Text = txtCycleDate.Text.Length.ToString();
        }

        private void txtCycleDays_TextChanged(object sender, EventArgs e)
        {
            valCycleDays.Text = txtCycleDays.Text.Length.ToString();
        }

        private void txtCycleType_TextChanged(object sender, EventArgs e)
        {
            valCycleType.Text = txtCycleType.Text.Length.ToString();
        }

        private void txtStatus_TextChanged(object sender, EventArgs e)
        {
            valStatus.Text = txtStatus.Text.Length.ToString();
        }

        private void txtRxSys_LastDoc_TextChanged(object sender, EventArgs e)
        {
            valRxSys_LastDoc.Text = txtRxSys_LastDoc.Text.Length.ToString();
        }

        private void valCycleDays_Click(object sender, EventArgs e)
        {

        }

        private void txtRxSys_PrimaryDoc_TextChanged(object sender, EventArgs e)
        {
            valRxSys_PrimaryDoc.Text = txtRxSys_PrimaryDoc.Text.Length.ToString();
        }

        private void txtRxSys_AltDoc_TextChanged(object sender, EventArgs e)
        {
            valRxSys_AltDoc.Text = txtRxSys_AltDoc.Text.Length.ToString();
        }

        private void txtSSN_TextChanged(object sender, EventArgs e)
        {
            valSSN.Text = txtSSN.Text.Length.ToString();
        }

        private void txtAllergies_TextChanged(object sender, EventArgs e)
        {
            valAllergies.Text = txtAllergies.Text.Length.ToString();
        }

        private void txtDiet_TextChanged(object sender, EventArgs e)
        {
            valDiet.Text = txtDiet.Text.Length.ToString();
        }

        private void txtDxNote_TextChanged(object sender, EventArgs e)
        {
            valDxNote.Text = txtDxNote.Text.Length.ToString();
        }

        private void txtTreatmentNotes_TextChanged(object sender, EventArgs e)
        {
            valTreatmentNotes.Text = txtTreatmentNotes.Text.Length.ToString();
        }

        private void txtDOB_TextChanged(object sender, EventArgs e)
        {
            valDOB.Text = txtDOB.Text.Length.ToString();
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {
            valHeight.Text = txtHeight.Text.Length.ToString();
        }

        private void txtWeight_TextChanged(object sender, EventArgs e)
        {
            valWeight.Text = txtWeight.Text.Length.ToString();
        }

        private void txtResponsibleName_TextChanged(object sender, EventArgs e)
        {
            valResponsibleName.Text = txtResponsibleName.Text.Length.ToString();
        }

        private void txtInsName_TextChanged(object sender, EventArgs e)
        {
            valInsName.Text = txtInsName.Text.Length.ToString();
        }

        private void txtInsPNo_TextChanged(object sender, EventArgs e)
        {
            valInsPNo.Text = txtInsPNo.Text.Length.ToString();
        }

        private void txtAltInsName_TextChanged(object sender, EventArgs e)
        {
            valAltInsName.Text = txtAltInsName.Text.Length.ToString();
        }

        private void txtAltInsPNo_TextChanged(object sender, EventArgs e)
        {
            valAltInsPNo.Text = txtAltInsPNo.Text.Length.ToString();
        }

        private void txtMCareNum_TextChanged(object sender, EventArgs e)
        {
            valMCareNum.Text = txtMCareNum.Text.Length.ToString();
        }

        private void txtMCaidNum_TextChanged(object sender, EventArgs e)
        {
            valMCaidNum.Text = txtMCaidNum.Text.Length.ToString();
        }

        private void txtAdmitDate_TextChanged(object sender, EventArgs e)
        {
            valAdmitDate.Text = txtAdmitDate.Text.Length.ToString();
        }

        private void txtChartOnly_TextChanged(object sender, EventArgs e)
        {
            valChartOnly.Text = txtChartOnly.Text.Length.ToString();
        }

        private void txtGender_TextChanged(object sender, EventArgs e)
        {
            valGender.Text = txtGender.Text.Length.ToString();
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
    }
}
