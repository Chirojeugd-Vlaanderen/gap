namespace LoginTestClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GapLoginButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.VoornaamTextBox = new System.Windows.Forms.TextBox();
            this.NaamTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MailadresTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AdNrTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.ResetButton = new System.Windows.Forms.Button();
            this.LoginLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // GapLoginButton
            // 
            this.GapLoginButton.Location = new System.Drawing.Point(208, 125);
            this.GapLoginButton.Name = "GapLoginButton";
            this.GapLoginButton.Size = new System.Drawing.Size(115, 23);
            this.GapLoginButton.TabIndex = 4;
            this.GapLoginButton.Text = "GapLogin maken";
            this.GapLoginButton.UseVisualStyleBackColor = true;
            this.GapLoginButton.Click += new System.EventHandler(this.GapLoginButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Voornaam";
            // 
            // VoornaamTextBox
            // 
            this.VoornaamTextBox.Location = new System.Drawing.Point(81, 38);
            this.VoornaamTextBox.Name = "VoornaamTextBox";
            this.VoornaamTextBox.Size = new System.Drawing.Size(304, 20);
            this.VoornaamTextBox.TabIndex = 1;
            this.VoornaamTextBox.Text = "Lamme";
            // 
            // NaamTextBox
            // 
            this.NaamTextBox.Location = new System.Drawing.Point(81, 64);
            this.NaamTextBox.Name = "NaamTextBox";
            this.NaamTextBox.Size = new System.Drawing.Size(304, 20);
            this.NaamTextBox.TabIndex = 2;
            this.NaamTextBox.Text = "Goedzak";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Naam";
            // 
            // MailadresTextBox
            // 
            this.MailadresTextBox.Location = new System.Drawing.Point(81, 90);
            this.MailadresTextBox.Name = "MailadresTextBox";
            this.MailadresTextBox.Size = new System.Drawing.Size(304, 20);
            this.MailadresTextBox.TabIndex = 3;
            this.MailadresTextBox.Text = "palmbwome@hotmail.com";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Mailadres";
            // 
            // AdNrTextBox
            // 
            this.AdNrTextBox.Location = new System.Drawing.Point(81, 12);
            this.AdNrTextBox.Name = "AdNrTextBox";
            this.AdNrTextBox.Size = new System.Drawing.Size(304, 20);
            this.AdNrTextBox.TabIndex = 0;
            this.AdNrTextBox.Text = "123456789";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "AD-nummer";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(81, 125);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Kipdorplogin maken";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(330, 125);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(55, 23);
            this.ResetButton.TabIndex = 5;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // LoginLabel
            // 
            this.LoginLabel.AutoSize = true;
            this.LoginLabel.Location = new System.Drawing.Point(81, 172);
            this.LoginLabel.Name = "LoginLabel";
            this.LoginLabel.Size = new System.Drawing.Size(132, 13);
            this.LoginLabel.TabIndex = 11;
            this.LoginLabel.Text = "Nog geen login gemaakt...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 262);
            this.Controls.Add(this.LoginLabel);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.AdNrTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MailadresTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NaamTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.VoornaamTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GapLoginButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GapLoginButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox VoornaamTextBox;
        private System.Windows.Forms.TextBox NaamTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MailadresTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AdNrTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Label LoginLabel;
    }
}

