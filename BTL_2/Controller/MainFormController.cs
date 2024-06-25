using BTL_2.Model;
using BTL_2.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_2.Controller
{
    public class MainFormController
    {
        private DatabaseDataContext dataContext = new DatabaseDataContext();
        
        public MainForm mainForm { get; private set; }
        public ToolStripMenuItem AccountRoleMenuItem { get; private set; }
        public ToolStripMenuItem LoginMenuItem { get; private set; }
        public ToolStripMenuItem AccountManagerMenuItem { get; private set; }
        public ToolStripMenuItem DataManagerMenuItem { get; private set; }
        public ToolStripMenuItem LogOutMenuItem { get; private set; }
        public Panel pnView { get; private set; }

        public MainFormController(MainForm mainForm, ToolStripMenuItem accountRoleMenuItem, ToolStripMenuItem loginMenuItem, ToolStripMenuItem accountManagerMenuItem, ToolStripMenuItem dataManagerMenuItem, ToolStripMenuItem logOutMenuItem, Panel pnView)
        {
            this.mainForm = mainForm;
            AccountRoleMenuItem = accountRoleMenuItem;
            LoginMenuItem = loginMenuItem;
            AccountManagerMenuItem = accountManagerMenuItem;
            DataManagerMenuItem = dataManagerMenuItem;
            LogOutMenuItem = logOutMenuItem;
            this.pnView = pnView;
        }
        public void SetMenuItem()
        {
            if (Constant.User.RoleID == 1)
            {
                AccountManagerMenuItem.Visible = true;
            }
        }
        public void SetEvent()
        {
            mainForm.Load += ShowLoginForm;

            //AccountManagerMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            //{
            //    LoadData();
            //});

            LoginMenuItem.Click += ShowLoginForm;

            LogOutMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Constant.User = null;
                UpdateMenuItems();
            });       
        }

        private void ShowLoginForm(object sender, EventArgs e)
        {
            if (Constant.User == null)
            {
                using (LoginForm login = new LoginForm())
                {
                    login.ShowDialog();
                }

                if (Constant.User != null)
                {
                    LoginMenuItem.Visible = false;
                    //AccountManagerMenuItem.Visible = true;
                    LogOutMenuItem.Visible = true;
                    SetMenuItem();
                    ViewLoad();
                    //LoadData();
                }
            }
        }

        private void UpdateMenuItems()
        {
            pnView.Controls.Clear();
            LoginMenuItem.Visible = true;
            AccountManagerMenuItem.Visible = false;
            LogOutMenuItem.Visible = false;
            
        }
        private void ViewLoad()
        {
            if (Constant.User.RoleID == 1)
            {
                AccountManagerForm childForm = new AccountManagerForm();

                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                pnView.Controls.Add(childForm);
                pnView.Tag = childForm;

                childForm.Show();
            }
        }
        private void LoadData()
        {
            //AccountDataGridView.DataSource = dataContext.Users.ToList();

            //cbxRole.DataSource = dataContext.Roles.ToList();
            //cbxRole.DisplayMember = "RoleName";
            //cbxRole.ValueMember = "RoleId";

        }
    }
}
