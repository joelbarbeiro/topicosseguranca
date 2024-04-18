namespace chat
{
    partial class FormChatLogin
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLoginUsername = new System.Windows.Forms.TextBox();
            this.textBoxLoginPassword = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonChatRegister = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(312, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sign In";
            // 
            // textBoxLoginUsername
            // 
            this.textBoxLoginUsername.Location = new System.Drawing.Point(317, 179);
            this.textBoxLoginUsername.Name = "textBoxLoginUsername";
            this.textBoxLoginUsername.Size = new System.Drawing.Size(145, 20);
            this.textBoxLoginUsername.TabIndex = 1;
            this.textBoxLoginUsername.Text = "Username:";
            this.textBoxLoginUsername.Enter += new System.EventHandler(this.textBoxLoginUsername_Enter);
            this.textBoxLoginUsername.Leave += new System.EventHandler(this.textBoxLoginUsername_Leave);
            // 
            // textBoxLoginPassword
            // 
            this.textBoxLoginPassword.Location = new System.Drawing.Point(317, 207);
            this.textBoxLoginPassword.Name = "textBoxLoginPassword";
            this.textBoxLoginPassword.PasswordChar = '*';
            this.textBoxLoginPassword.Size = new System.Drawing.Size(145, 20);
            this.textBoxLoginPassword.TabIndex = 2;
            this.textBoxLoginPassword.Text = "Password:";
            this.textBoxLoginPassword.Enter += new System.EventHandler(this.textBoxLoginPassword_Enter);
            this.textBoxLoginPassword.Leave += new System.EventHandler(this.textBoxLoginPassword_Leave);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(317, 233);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(75, 23);
            this.buttonLogin.TabIndex = 3;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // buttonChatRegister
            // 
            this.buttonChatRegister.Location = new System.Drawing.Point(387, 233);
            this.buttonChatRegister.Name = "buttonChatRegister";
            this.buttonChatRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonChatRegister.TabIndex = 4;
            this.buttonChatRegister.Text = "Register";
            this.buttonChatRegister.UseVisualStyleBackColor = true;
            this.buttonChatRegister.Click += new System.EventHandler(this.buttonChatRegister_Click_1);
            // 
            // FormChatLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonChatRegister);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxLoginPassword);
            this.Controls.Add(this.textBoxLoginUsername);
            this.Controls.Add(this.label1);
            this.Name = "FormChatLogin";
            this.Text = "Chat Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLoginUsername;
        private System.Windows.Forms.TextBox textBoxLoginPassword;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonChatRegister;
    }
}

