using System;
using System.Windows.Forms;
using UcmaBotDtmf.Common;
using UcmaBotDtmf.Models;
using UcmaBotDtmf.Repositories;

namespace UcmaBotDtmf
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }
        private LanguageRepository languageRep;
        private bool isFormLoaded;
        private void frmSettings_Load(object sender, EventArgs e)
        {
            languageRep = new LanguageRepository();

            cmbLanguages.DisplayMember = "Language";

            cmbLanguages.DataSource = languageRep.GetBotLanguages();

            isFormLoaded = true;

            if(cmbLanguages.Items.Count > 0)
            {
                cmbLanguages.SelectedIndex = 0;
            }
        }

        private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
           

            var cmb = (ComboBox)sender;

            if (cmb == null)
                return;

            var data = (BotLanguageModel)cmb.SelectedItem;

            if (data == null)
                return;

            txtLocal.Text = data.Local;

            txtSecret.Text = data.Secret;
        }

        private void btnUpdateBL_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLocal.Text)){
                MessageBox.Show("Local required", MyAppSettings.ProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtSecret.Text))
            {
                MessageBox.Show("Secret required", MyAppSettings.ProjectTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
