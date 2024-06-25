using BTL_1.Controller;
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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            MainFormController mainFormController = new MainFormController(menuStrip1, UserRole, login, AccountManager, DataManager, dataGridView, logout, this, lbId, txtName, txtPass, txtEmail, txtPhone, cbxRole, btnInsert, btnDelete, btnUpdate, txtFullName);
            mainFormController.SetEvent();
            if (Constant.User != null)
            {
                AccountManager.Visible = true;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
