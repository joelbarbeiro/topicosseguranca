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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formChatRegister));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUserNameRegister = new System.Windows.Forms.TextBox();
            this.textBoxEmailRegister = new System.Windows.Forms.TextBox();
            this.textBoxPasswordRegister = new System.Windows.Forms.TextBox();
            this.buttonChatRegister = new System.Windows.Forms.Button();
            this.textBoxConfirmPasswordRegister = new System.Windows.Forms.TextBox();
            this.labelErrorShower = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(69, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "Register";
            // 
            // textBoxUserNameRegister
            // 
            this.textBoxUserNameRegister.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxUserNameRegister.Location = new System.Drawing.Point(74, 79);
            this.textBoxUserNameRegister.Name = "textBoxUserNameRegister";
            this.textBoxUserNameRegister.Size = new System.Drawing.Size(160, 24);
            this.textBoxUserNameRegister.TabIndex = 1;
            this.textBoxUserNameRegister.Text = "Username";
            this.textBoxUserNameRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUserNameRegister_KeyPress);
            this.textBoxUserNameRegister.Leave += new System.EventHandler(this.textBoxUserNameRegister_Leave);
            // 
            // textBoxEmailRegister
            // 
            this.textBoxEmailRegister.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEmailRegister.Location = new System.Drawing.Point(74, 103);
            this.textBoxEmailRegister.Name = "textBoxEmailRegister";
            this.textBoxEmailRegister.Size = new System.Drawing.Size(160, 24);
            this.textBoxEmailRegister.TabIndex = 2;
            this.textBoxEmailRegister.Text = "Email";
            this.textBoxEmailRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxEmailRegister_KeyPress);
            this.textBoxEmailRegister.Leave += new System.EventHandler(this.textBoxEmailRegister_Leave);
            // 
            // textBoxPasswordRegister
            // 
            this.textBoxPasswordRegister.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPasswordRegister.Location = new System.Drawing.Point(74, 127);
            this.textBoxPasswordRegister.Name = "textBoxPasswordRegister";
            this.textBoxPasswordRegister.Size = new System.Drawing.Size(160, 24);
            this.textBoxPasswordRegister.TabIndex = 3;
            this.textBoxPasswordRegister.Text = "Password";
            this.textBoxPasswordRegister.UseSystemPasswordChar = true;
            this.textBoxPasswordRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPasswordRegistor_KeyPress);
            this.textBoxPasswordRegister.Leave += new System.EventHandler(this.textBoxPasswordRegistor_Leave);
            // 
            // buttonChatRegister
            // 
            this.buttonChatRegister.Enabled = false;
            this.buttonChatRegister.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonChatRegister.Location = new System.Drawing.Point(74, 188);
            this.buttonChatRegister.Name = "buttonChatRegister";
            this.buttonChatRegister.Size = new System.Drawing.Size(75, 23);
            this.buttonChatRegister.TabIndex = 5;
            this.buttonChatRegister.Text = "Register";
            this.buttonChatRegister.UseVisualStyleBackColor = true;
            this.buttonChatRegister.Click += new System.EventHandler(this.buttonChatRegister_Click_1);
            // 
            // textBoxConfirmPasswordRegister
            // 
            this.textBoxConfirmPasswordRegister.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxConfirmPasswordRegister.Location = new System.Drawing.Point(74, 152);
            this.textBoxConfirmPasswordRegister.Name = "textBoxConfirmPasswordRegister";
            this.textBoxConfirmPasswordRegister.Size = new System.Drawing.Size(160, 24);
            this.textBoxConfirmPasswordRegister.TabIndex = 4;
            this.textBoxConfirmPasswordRegister.Text = "Password";
            this.textBoxConfirmPasswordRegister.UseSystemPasswordChar = true;
            this.textBoxConfirmPasswordRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxConfirmPasswordRegistor_KeyPress);
            this.textBoxConfirmPasswordRegister.Leave += new System.EventHandler(this.textBoxConfirmPasswordRegistor_Leave);
            // 
            // labelErrorShower
            // 
            this.labelErrorShower.AutoSize = true;
            this.labelErrorShower.BackColor = System.Drawing.Color.Transparent;
            this.labelErrorShower.Enabled = false;
            this.labelErrorShower.Location = new System.Drawing.Point(72, 173);
            this.labelErrorShower.Name = "labelErrorShower";
            this.labelErrorShower.Size = new System.Drawing.Size(111, 17);
            this.labelErrorShower.TabIndex = 6;
            this.labelErrorShower.Text = "labelErrorShower";
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBack.Location = new System.Drawing.Point(158, 188);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(75, 23);
            this.buttonBack.TabIndex = 6;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // formChatRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::chat.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(308, 274);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelErrorShower);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.textBoxPasswordRegister);
            this.Controls.Add(this.textBoxUserNameRegister);
            this.Controls.Add(this.textBoxConfirmPasswordRegister);
            this.Controls.Add(this.textBoxEmailRegister);
            this.Controls.Add(this.buttonChatRegister);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formChatRegister";
            this.Text = "Chat Stream";
            this.Load += new System.EventHandler(this.chatRegister_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUserNameRegister;
        private System.Windows.Forms.TextBox textBoxEmailRegister;
        private System.Windows.Forms.TextBox textBoxPasswordRegister;
        private System.Windows.Forms.Button buttonChatRegister;
        private System.Windows.Forms.TextBox textBoxConfirmPasswordRegister;
        private System.Windows.Forms.Label labelErrorShower;
        private System.Windows.Forms.Button buttonBack;
    }
}