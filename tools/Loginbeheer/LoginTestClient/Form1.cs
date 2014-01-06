/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
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