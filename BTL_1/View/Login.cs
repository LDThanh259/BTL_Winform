﻿using BTL_1.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_1.View
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            LoginController loginController = new LoginController(txtUsername, txtPassword, msg, cbSavePass, btnLogin, this);
            loginController.setEvent();
        }
    }
}
