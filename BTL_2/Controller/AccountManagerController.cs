using BTL_2.Model;
using BTL_2.View;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BTL_2.Controller
{
    public class AccountManagerController
    {
        private DatabaseDataContext dataContext = new DatabaseDataContext();
        public AccountManagerForm AccountManagerForm { get; private set; }
        public TextBox txtFullName { get; private set; }
        public Label lbID { get; private set; }
        public Button btnDelete { get; private set; }
        public Button btnUpdate { get; private set; }
        public Button btnInsert { get; private set; }
        public ComboBox cbxRole { get; private set; }
        public TextBox txtPhoneNumber { get; private set; }
        public TextBox txtUserName { get; private set; }
        public TextBox txtPassword { get; private set; }
        public TextBox txtEmail { get; private set; }
        public DataGridView AccountDataGridView { get; private set; }

        public AccountManagerController(AccountManagerForm accountManagerForm, TextBox txtFullName, Label lbID, Button btnDelete, Button btnUpdate, Button btnInsert, ComboBox cbxRole, TextBox txtPhoneNumber, TextBox txtUserName, TextBox txtPassword, TextBox txtEmail, DataGridView accountDataGridView)
        {
            this.AccountManagerForm = accountManagerForm;
            this.txtFullName = txtFullName;
            this.lbID = lbID;
            this.btnDelete = btnDelete;
            this.btnUpdate = btnUpdate;
            this.btnInsert = btnInsert;
            this.cbxRole = cbxRole;
            this.txtPhoneNumber = txtPhoneNumber;
            this.txtUserName = txtUserName;
            this.txtPassword = txtPassword;
            this.txtEmail = txtEmail;
            AccountDataGridView = accountDataGridView;
        }

        public void SetEvent()
        {
            AccountManagerForm.Load += new EventHandler((object sender, EventArgs e) => LoadData());
            btnInsert.Click += InsertUser;
            AccountDataGridView.SelectionChanged += DataGridView_SelectionChanged;
            btnUpdate.Click += UpdateUser;
            btnDelete.Click += DeleteUser;
        }

        private void DeleteUser(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lbID.Text))
            {
                MessageBox.Show("No user selected for deletion.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = int.Parse(lbID.Text);
            var userToDelete = dataContext.Users.FirstOrDefault(u => u.UserID == id);

            if (userToDelete != null && ShowConfirmationMessage())
            {
                BackupUserData(userToDelete);

                dataContext.Users.DeleteOnSubmit(userToDelete);
                dataContext.SubmitChanges();
                MessageBox.Show("User deleted successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
                ClearInputs();
            }
            if(userToDelete == null)
            {
                MessageBox.Show("User not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackupUserData(User user)
        {
            string backupData = $"ID: {user.UserID}, Username: {user.Username}, FullName: {user.FullName}, Email: {user.Email}, Phone: {user.PhoneNumber}, RoleID: {user.RoleID}";
            System.IO.File.AppendAllText("backupUserData.txt", backupData + Environment.NewLine);
        }

        private void ClearInputs()
        {
            lbID.Text = "";
            txtUserName.Text = "";
            txtPassword.Text = "";
            txtEmail.Text = "";
            txtPhoneNumber.Text = "";
            cbxRole.SelectedIndex = -1;
            txtFullName.Text = "";
            btnInsert.Enabled = true;
            btnDelete.Enabled = false;
            btnUpdate.Enabled = false;
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (AccountDataGridView.SelectedRows.Count > 0)
            {
                btnInsert.Enabled = false;
                btnDelete.Enabled = true;
                btnUpdate.Enabled = true;

                DataGridViewRow row = AccountDataGridView.SelectedRows[0];

                lbID.Text = row.Cells[0].Value.ToString();
                txtUserName.Text = row.Cells[1].Value.ToString();
                txtPassword.Text = row.Cells[2].Value.ToString();
                txtEmail.Text = row.Cells[4].Value.ToString();
                txtPhoneNumber.Text = row.Cells[5].Value.ToString() ?? "";
                cbxRole.SelectedValue = row.Cells[6].Value;
                txtFullName.Text = row.Cells[3].Value.ToString();
            }
        }

        private void UpdateUser(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            int id = int.Parse(lbID.Text);
            var name = txtUserName.Text;
            var pass = txtPassword.Text;
            var phone = txtPhoneNumber.Text;
            var email = txtEmail.Text;
            var roleid = (int)cbxRole.SelectedValue;

            var userToUpdate = dataContext.Users.FirstOrDefault(u => u.UserID == id);

            if (userToUpdate != null)
            {
                userToUpdate.Username = name;
                userToUpdate.PasswordHash = pass;
                userToUpdate.Email = email;
                userToUpdate.PhoneNumber = phone;
                userToUpdate.FullName = txtFullName.Text;

                if (userToUpdate.RoleID != roleid)
                {
                    userToUpdate.RoleID = roleid;
                }

                dataContext.SubmitChanges();
                MessageBox.Show("User updated successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
            }
            else
            {
                MessageBox.Show("User not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void InsertUser(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            var name = txtUserName.Text;
            var pass = txtPassword.Text;
            var phone = txtPhoneNumber.Text;
            var email = txtEmail.Text;
            var roleid = (int)cbxRole.SelectedValue;

            User user = new User();
            user.Username = name;
            user.PasswordHash = pass;
            user.Email = email;
            user.PhoneNumber = phone;
            user.RoleID = roleid;
            user.FullName = txtFullName.Text;
            dataContext.Users.InsertOnSubmit(user);
            dataContext.SubmitChanges();

            LoadData();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("User Name is required.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("A valid Email is required.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text) || !IsValidPhone(txtPhoneNumber.Text))
            {
                MessageBox.Show("A valid Phone Number is required.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cbxRole.SelectedIndex == -1)
            {
                MessageBox.Show("Role must be selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\d{10}$");
        }

        private void LoadData()
        {
            AccountDataGridView.DataSource = dataContext.Users.ToList();

            cbxRole.DataSource = dataContext.Roles.ToList();
            cbxRole.DisplayMember = "RoleName";
            cbxRole.ValueMember = "RoleId";
        }

        private bool ShowConfirmationMessage()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return result == DialogResult.Yes;
        }
    }
}
