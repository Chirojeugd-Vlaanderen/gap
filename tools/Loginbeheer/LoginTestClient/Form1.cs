using System;
using System.ServiceModel;
using System.Windows.Forms;
using Chiro.Ad.ServiceContracts;
using Chiro.Adf.ServiceModel;
using LoginTestClient.Properties;

namespace LoginTestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            AdNrTextBox.Text = string.Empty;
            VoornaamTextBox.Text = string.Empty;
            NaamTextBox.Text = string.Empty;
            MailadresTextBox.Text = string.Empty;
            LoginLabel.Text = Resources.NogGeenLoginGemaakt_Feedback;
        }

        private void GapLoginButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            LoginLabel.Text = Resources.NogGeenLoginGemaakt_Feedback;

            try
            {
                string login = ServiceHelper.CallService<IAdService, string>(client => client.GapLoginAanvragen(Int32.Parse(AdNrTextBox.Text), VoornaamTextBox.Text, NaamTextBox.Text, MailadresTextBox.Text));
                LoginLabel.Text = string.Concat("Login: ", login);
            }
            catch (FaultException<ArgumentException> fault)
            {
                MessageBox.Show(fault.Detail.Message, Resources.Foutje_Venstertitel, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FaultException<InvalidOperationException> fault)
            {
                FoutboodschapTonen(fault.Detail.Message);
            }
            catch (FaultException<FormatException> fault)
            {
                FoutboodschapTonen(fault.Detail.Message);
            }
            catch (FaultException fault)
            {
                FoutboodschapTonen(fault.Message);
            }
 
            Cursor.Current = Cursors.Default;
        }

        private static void FoutboodschapTonen(string boodschap)
        {
            MessageBox.Show(boodschap, Resources.Foutje_Venstertitel, MessageBoxButtons.OK, MessageBoxIcon.Error);   
        }
    }
}