using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    public partial class formChatRegister : Form
    {
        public formChatRegister()
        {
            InitializeComponent();
        }

        private void chatRegister_Load(object sender, EventArgs e)
        {
            labelErrorShower.Text = "";
        }

        private void textBoxUserNameRegister_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxUserNameRegister.Text == "Username")
            {
                textBoxUserNameRegister.Text = "";
                registerButtonControl();
            }
        }
        private void registerButtonControl()
        {
            if (textBoxUserNameRegister.Text != "Username" && textBoxEmailRegister.Text != "Email" && textBoxPasswordRegistor.Text != "Password" && textBoxConfirmPasswordRegister.Text != "Confirm Password")
            {
                buttonChatRegister.Enabled = true;
            }
            else
            {
                buttonChatRegister.Enabled = false;
            }
        }

        private void textBoxUserNameRegister_Leave(object sender, EventArgs e)
        {
            if (textBoxUserNameRegister.Text == "") {
                textBoxUserNameRegister.Text = "Username";
            }
        }

        private void textBoxEmailRegister_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxEmailRegister.Text == "Email")
            {
                textBoxEmailRegister.Text = "";
                registerButtonControl();
            }
        }

        private void textBoxEmailRegister_Leave(object sender, EventArgs e)
        {
            if (textBoxEmailRegister.Text == "")
            {
                textBoxEmailRegister.Text = "Email";
            }
        }

        private void textBoxPasswordRegistor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxPasswordRegistor.Text == "Password")
            {
                textBoxPasswordRegistor.Text = "";
                registerButtonControl();
            }
        }

        private void textBoxPasswordRegistor_Leave(object sender, EventArgs e)
        {
            if (textBoxPasswordRegistor.Text == "")
            {
                textBoxPasswordRegistor.Text = "Password";
            }
        }

        private void textBoxConfirmPasswordRegistor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBoxConfirmPasswordRegister.Text == "Confirm Password")
            {
                textBoxConfirmPasswordRegister.Text = "";
                registerButtonControl();
            }
        }

        private void textBoxConfirmPasswordRegistor_Leave(object sender, EventArgs e)
        {
            if (textBoxConfirmPasswordRegister.Text == "")
            {
                textBoxConfirmPasswordRegister.Text = "Confirm Password";
            }
        }
        
        private void buttonChatRegister_Click(object sender, EventArgs e)
        {
            if(textBoxPasswordRegistor.Text != textBoxConfirmPasswordRegister.Text)
            {
                labelErrorShower.Text = "Passwords do not match!";
            }
            else
            {
                labelErrorShower.Text = "";
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            FormChatLogin formchatlogin = new FormChatLogin();
            formchatlogin.Show();
            this.Hide();
        }

        
    }
}
