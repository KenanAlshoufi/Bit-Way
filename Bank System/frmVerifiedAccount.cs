using Bank_System.Global;
using Bank_System.Properties;
using BankSystemBusinessLayar;
using Driving_License_Management;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bank_System
{
    public partial class frmVerifiedAccount : Form
    {
        public frmVerifiedAccount()
        {
            InitializeComponent();
        }

        private bool _HandlePersonImage(PictureBox picture)
        {
            if (picture.ImageLocation != null)
            {
                string SourseImageFile = picture.ImageLocation.ToString();
                if (clsUtil.CopyImageToProjectImagesFolder(ref SourseImageFile))
                {
                    picture.ImageLocation = SourseImageFile;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                return;
            }
            if (pbFront.ImageLocation == null || pbBack.ImageLocation == null)
            {
                MessageBox.Show("You Must upload Image ID .","Erorr",MessageBoxButtons.OK);
                return;
            }
            if (!(_HandlePersonImage(pbBack) && _HandlePersonImage(pbFront)))
            {
                return;
            }

            DocumentationInformation documentation =new DocumentationInformation();
            documentation.BackImageoftheID = pbBack.ImageLocation.ToString();
            documentation.FrontImageoftheID = pbFront.ImageLocation.ToString();
            documentation.Address=txtAddress.Text.Trim();
            documentation.AccountID = clsGlobal.CurrentUser.AccountID;


           
            if (documentation.AddNewDocumentationInformation())
            {
                txtAddress.Enabled = false;
                pbBack.Enabled = false;
                pbFront.Enabled = false;

                timer1.Start();
                MessageBox.Show("Done","Save",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            else
            {
                MessageBox.Show("There is an error.", "error", MessageBoxButtons.OK);
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void txtAddress_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddress.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtAddress, "This Filed Is requir !");
            }
            else
            {
                errorProvider1.SetError(txtAddress, null);
            }
        }

        private void pbFront_DoubleClick(object sender, EventArgs e)
        {
            ofdSelectImage.Filter = "Image Files | *.jpg; *.jpeg; *.png; *.gif; *.bmp";
            ofdSelectImage.FilterIndex = 1;
            ofdSelectImage.RestoreDirectory = true;

            if (ofdSelectImage.ShowDialog() == DialogResult.OK)
            {
                // Process the selected file 
                string selectedFilePath = ofdSelectImage.FileName;
                //MessageBox.Show("Selected Image is:" + selectedFilePath); 

                pbFront.ImageLocation = selectedFilePath;
            }
        }

        private void pbBack_DoubleClick(object sender, EventArgs e)
        {
            ofdSelectImage.Filter = "Image Files | *.jpg; *.jpeg; *.png; *.gif; *.bmp";
            ofdSelectImage.FilterIndex = 1;
            ofdSelectImage.RestoreDirectory = true;

            if (ofdSelectImage.ShowDialog() == DialogResult.OK)
            {
                // Process the selected file 
                string selectedFilePath = ofdSelectImage.FileName;
                //MessageBox.Show("Selected Image is:" + selectedFilePath); 

                pbBack.ImageLocation = selectedFilePath;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            lblTimer.Text = (clsGlobal.Durationofaccountverification.TotalHours).ToString("00") + ":" +
               (clsGlobal.Durationofaccountverification.TotalMinutes).ToString("00") + ":" +
               (clsGlobal.Durationofaccountverification.TotalSeconds).ToString("00");

            clsGlobal.Durationofaccountverification= clsGlobal.Durationofaccountverification
                .Subtract(new TimeSpan(0, 0, 1));
           

            if (clsGlobal.Durationofaccountverification == new TimeSpan(0, 0, 0))
            {
                timer1.Stop();
                if (AccountInformation.AccountVerified(clsGlobal.CurrentUser.AccountID))
                {
                    MessageBox.Show("Done Verified your Account", "Done", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erorr", "Error", MessageBoxButtons.OK,
                       MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }

        private void LoadInfoDocument()
        {
            DocumentationInformation documentationInformation =  DocumentationInformation.
                FindDocumentationInformationByAccountID(clsGlobal.CurrentUser.AccountID);
            txtAddress.Text =documentationInformation.Address;
            pbFront.ImageLocation=documentationInformation.FrontImageoftheID; 
            pbBack.ImageLocation=documentationInformation.BackImageoftheID; 

            txtAddress.Enabled = false;
            pbBack.Enabled = false;
            pbFront.Enabled = false;
        }

        private void frmVerifiedAccount_Load(object sender, EventArgs e)
        {
            if (DocumentationInformation.IsDocumentationInformationExist(clsGlobal.CurrentUser.AccountID))
            {
               LoadInfoDocument();
            }
        }
    }
}
