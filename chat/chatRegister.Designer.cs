namespace chat
{
    partial class formChatRegister
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
            this.textBoxUserNameRegister = new System.Windows.Forms.TextBox();
            this.textBoxEmailRegister = new System.Windows.Forms.TextBox();
            this.textBoxPasswordRegistor = new System.Windows.Forms.TextBox();
            this.buttonChatRegister = new System.Windows.Forms.Button();
            this.textBoxConfirmPasswordRegister = new System.Windows.Forms.TextBox();
            this.labelErrorShower = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(314, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Register";
            // 
            // textBoxUserNameRegister
            // 
            this.textBoxUserNameRegister.Location = new System.Drawing.Point(319, 156);
            this.textBoxUserNameRegister.Name = "textBoxUserNameRegister";
            this.textBoxUserNameRegister.Size = new System.Drawing.Size(160, 20);
            this.textBoxUserNameRegister.TabIndex = 1;
            this.textBoxUserNameRegister.Text = "Username";
            this.textBoxUserNameRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUserNameRegister_KeyPress);
            this.textBoxUserNameRegister.Leave += new System.EventHandler(this.textBoxUserNameRegister_Leave);
            // 
            // textBoxEmailRegister
            // 
            this.textBoxEmailRegister.Location = new System.Drawing.Point(319, 182);
            this.textBoxEmailRegister.Name = "textBoxEmailRegister";
            this.textBoxEmailRegister.Size = new System.Drawing.Size(160, 20);
            this.textBoxEmailRegister.TabIndex = 2;
            this.textBoxEmailRegister.Text = "Email";
            this.textBoxEmailRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEmailRegister_KeyPress);
            this.textBoxEmailRegister.Leave += new System.EventHandler(this.textBoxEmailRegister_Leave);
            // 
            // textBoxPasswordRegistor
            // 
            this.textBoxPasswordRegistor.Location = new System.Drawing.Point(319, 208);
            this.textBoxPasswordRegistor.Name = "textBoxPasswordRegistor";
            this.textBoxPasswordRegistor.Size = new System.Drawing.Size(160, 20);
            this.textBoxPasswordRegistor.TabIndex = 3;
            this.textBoxPasswordRegistor.Text = "Password";
            this.textBoxPasswordRegistor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPasswordRegistor_KeyPress);
            this.textBoxPasswordRegistor.Leave += new System.EventHandler(this.textBoxPasswordRegistor_Leave);
            // 
            // buttonChatRegister
            // 
            this.buttonChatRegister.Enabled = false;
            this.buttonChatRegister.Location = new System.Drawing.Point(319, 277);
            this.buttonChatRegister.Name = "buttonChatRegister";
            this.buttonChatRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonChatRegister.TabIndex = 4;
            this.buttonChatRegister.Text = "Register";
            this.buttonChatRegister.UseVisualStyleBackColor = true;
            // 
            // textBoxConfirmPasswordRegister
            // 
            this.textBoxConfirmPasswordRegister.Location = new System.Drawing.Point(319, 234);
            this.textBoxConfirmPasswordRegister.Name = "textBoxConfirmPasswordRegister";
            this.textBoxConfirmPasswordRegister.Size = new System.Drawing.Size(160, 20);
            this.textBoxConfirmPasswordRegister.TabIndex = 5;
            this.textBoxConfirmPasswordRegister.Text = "Confirm Password";
            this.textBoxConfirmPasswordRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxConfirmPasswordRegistor_KeyPress);
            this.textBoxConfirmPasswordRegister.Leave += new System.EventHandler(this.textBoxConfirmPasswordRegistor_Leave);
            // 
            // labelErrorShower
            // 
            this.labelErrorShower.AutoSize = true;
            this.labelErrorShower.Enabled = false;
            this.labelErrorShower.Location = new System.Drawing.Point(319, 261);
            this.labelErrorShower.Name = "labelErrorShower";
            this.labelErrorShower.Size = new System.Drawing.Size(87, 13);
            this.labelErrorShower.TabIndex = 6;
            this.labelErrorShower.Text = "labelErrorShower";
            // 
            // buttonBack
            // 
            this.buttonBack.Location = new System.Drawing.Point(404, 277);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(75, 23);
            this.buttonBack.TabIndex = 7;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // formChatRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.labelErrorShower);
            this.Controls.Add(this.textBoxConfirmPasswordRegister);
            this.Controls.Add(this.buttonChatRegister);
            this.Controls.Add(this.textBoxPasswordRegistor);
            this.Controls.Add(this.textBoxEmailRegister);
            this.Controls.Add(this.textBoxUserNameRegister);
            this.Controls.Add(this.label1);
            this.Name = "formChatRegister";
            this.Text = "Chat register";
            this.Load += new System.EventHandler(this.chatRegister_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUserNameRegister;
        private System.Windows.Forms.TextBox textBoxEmailRegister;
        private System.Windows.Forms.TextBox textBoxPasswordRegistor;
        private System.Windows.Forms.Button buttonChatRegister;
        private System.Windows.Forms.TextBox textBoxConfirmPasswordRegister;
        private System.Windows.Forms.Label labelErrorShower;
        private System.Windows.Forms.Button buttonBack;
    }
}