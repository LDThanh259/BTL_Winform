﻿using BTL_2.Model;
using BTL_2.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_2.Controller
{
    public class LoginController
    {
        private DatabaseDataContext dataClassesDataContext;
        public TextBox txtUsername { get; set; }
        public TextBox txtPassword { get; set; }
        public Label Msg { get; set; }
        public CheckBox IsSavePass { get; set; }
        public Button btnLogin { get; set; }

        public LoginForm login { get; set; }

        public LoginController(TextBox txtUsername, TextBox txtPassword, Label msg, CheckBox isSavePass, Button btnLogin, LoginForm login)
        {
            this.txtUsername = txtUsername;
            this.txtPassword = txtPassword;
            Msg = msg;
            IsSavePass = isSavePass;
            this.btnLogin = btnLogin;
            dataClassesDataContext = new DatabaseDataContext();
            this.login = login;

            //setEvent();  // Đăng ký sự kiện tại đây
        }

        public void CheckLogin()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var tk = dataClassesDataContext.Users
                    .Where(u => u.Username == username && u.PasswordHash == password)
                    .FirstOrDefault();
                if (tk != null)
                {
                    Constant.User = tk;
                    isSavePass();
                    Msg.Visible = false;
                    login.Close();
                }
                else
                {
                    Msg.Text = "Invalid username or password.";
                    Msg.Visible = true;
                }
            }
            else
            {
                Msg.Text = "Empty username or password.";
                Msg.Visible = true;
            }
        }

        private void isSavePass()
        {
            if (IsSavePass.Checked)
            {
                Constant.IsSavePass = true;
            }
        }

        public void setEvent()
        {
            btnLogin.Click += new System.EventHandler((object sender, EventArgs e) =>
            {
                CheckLogin();
            });
            login.FormClosing += Login_FormClosing;
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            //hien thi lai MainForm
        }
    }
}
