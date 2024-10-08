﻿namespace chat
{
    partial class FormChat
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
            this.labelUsersConnected = new System.Windows.Forms.Label();
            this.listBoxUserList = new System.Windows.Forms.ListBox();
            this.listBoxMessage = new System.Windows.Forms.ListBox();
            this.buttonSendMessage = new System.Windows.Forms.Button();
            this.labelUserName = new System.Windows.Forms.Label();
            this.buttonChatLogout = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelUsersConnected
            // 
            this.labelUsersConnected.AutoSize = true;
            this.labelUsersConnected.Location = new System.Drawing.Point(13, 27);
            this.labelUsersConnected.Name = "labelUsersConnected";
            this.labelUsersConnected.Size = new System.Drawing.Size(89, 13);
            this.labelUsersConnected.TabIndex = 0;
            this.labelUsersConnected.Text = "Users Connected";
            // 
            // listBoxUserList
            // 
            this.listBoxUserList.FormattingEnabled = true;
            this.listBoxUserList.Location = new System.Drawing.Point(16, 43);
            this.listBoxUserList.Name = "listBoxUserList";
            this.listBoxUserList.Size = new System.Drawing.Size(120, 342);
            this.listBoxUserList.TabIndex = 1;
            // 
            // listBoxMessage
            // 
            this.listBoxMessage.FormattingEnabled = true;
            this.listBoxMessage.Location = new System.Drawing.Point(143, 43);
            this.listBoxMessage.Name = "listBoxMessage";
            this.listBoxMessage.Size = new System.Drawing.Size(645, 342);
            this.listBoxMessage.TabIndex = 2;
            // 
            // buttonSendMessage
            // 
            this.buttonSendMessage.Location = new System.Drawing.Point(713, 391);
            this.buttonSendMessage.Name = "buttonSendMessage";
            this.buttonSendMessage.Size = new System.Drawing.Size(75, 47);
            this.buttonSendMessage.TabIndex = 3;
            this.buttonSendMessage.Text = "Send";
            this.buttonSendMessage.UseVisualStyleBackColor = true;
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(142, 27);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(35, 13);
            this.labelUserName.TabIndex = 4;
            this.labelUserName.Text = "label1";
            // 
            // buttonChatLogout
            // 
            this.buttonChatLogout.Location = new System.Drawing.Point(713, 17);
            this.buttonChatLogout.Name = "buttonChatLogout";
            this.buttonChatLogout.Size = new System.Drawing.Size(75, 23);
            this.buttonChatLogout.TabIndex = 5;
            this.buttonChatLogout.Text = "Logout";
            this.buttonChatLogout.UseVisualStyleBackColor = true;
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(16, 391);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessage.Size = new System.Drawing.Size(691, 47);
            this.textBoxMessage.TabIndex = 6;
            // 
            // FormChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.buttonChatLogout);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.buttonSendMessage);
            this.Controls.Add(this.listBoxMessage);
            this.Controls.Add(this.listBoxUserList);
            this.Controls.Add(this.labelUsersConnected);
            this.Name = "FormChat";
            this.Text = "Chat";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUsersConnected;
        private System.Windows.Forms.ListBox listBoxUserList;
        private System.Windows.Forms.ListBox listBoxMessage;
        private System.Windows.Forms.Button buttonSendMessage;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Button buttonChatLogout;
        private System.Windows.Forms.TextBox textBoxMessage;
    }
}