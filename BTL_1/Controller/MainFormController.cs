using BTL_1.View;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BTL_1.Controller
{
    public class MainFormController
    {
        DataClassesDataContext dataContext = new DataClassesDataContext();
        public MenuStrip menuStrip1 { get; set; }
        public ToolStripMenuItem UserRole { get; set; }
        public ToolStripMenuItem Login { get; set; }
        public ToolStripMenuItem AccountManager { get; set; }
        public ToolStripMenuItem DataManager { get; set; }
        public DataGridView DataGridView { get; set; }
        public ToolStripMenuItem Logout { get; set; }
        public MainForm mainForm { get; set; }

        public Label ID { get; set; }
        public TextBox UserName { get; set; }
        public TextBox Password { get; set; }
        public TextBox Email { get; set; }
        public TextBox PhoneNumber { get; set; }
        public ComboBox Role { get; set; }

        public Button Insert { get; set; }
        public Button Delete { get; set; }
        public Button Update { get; set; }
        public TextBox txtFullName { get; set; }
        public MainFormController(MenuStrip menuStrip1, ToolStripMenuItem userRole, ToolStripMenuItem login, ToolStripMenuItem accountManager, ToolStripMenuItem dataManager, DataGridView dataGridView, ToolStripMenuItem logout, MainForm mainForm)
        {
            this.menuStrip1 = menuStrip1;
            UserRole = userRole;
            Login = login;
            AccountManager = accountManager;
            DataManager = dataManager;
            DataGridView = dataGridView;
            Logout = logout;
            this.mainForm = mainForm;
        }

        public MainFormController(MenuStrip menuStrip1, ToolStripMenuItem userRole, ToolStripMenuItem login, ToolStripMenuItem accountManager, ToolStripMenuItem dataManager, DataGridView dataGridView, ToolStripMenuItem logout, MainForm mainForm, Label iD, TextBox userName, TextBox password, TextBox email, TextBox phoneNumber, ComboBox role, Button insert, Button delete, Button update, TextBox txtFullName)
        {
            this.menuStrip1 = menuStrip1;
            UserRole = userRole;
            Login = login;
            AccountManager = accountManager;
            DataManager = dataManager;
            DataGridView = dataGridView;
            Logout = logout;
            this.mainForm = mainForm;
            ID = iD;
            UserName = userName;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;
            Insert = insert;
            Delete = delete;
            Update = update;
            this.txtFullName = txtFullName;
        }

        public void SetEvent()
        {
            mainForm.Load += new EventHandler((object sender, EventArgs e) =>
            {
                ShowLoginForm();
            });

            AccountManager.Click += new EventHandler((object sender, EventArgs e) =>
            {
                LoadData();
            });

            Login.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ShowLoginForm();
            });

            Logout.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Constant.User = null;
                UpdateMenuItems();
            });

            Insert.Click += new EventHandler((object sender, EventArgs e) =>
            {
                InsertUser();
            });

            DataGridView.SelectionChanged += DataGridView_SelectionChanged;
            Update.Click += UpdateUser;
        }

        private void UpdateUser(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            int id = int.Parse(ID.Text); // Lấy ID của người dùng từ control ID (Label, TextBox, hoặc phù hợp với thiết kế giao diện của bạn)
            var name = UserName.Text;
            var pass = Password.Text;
            var phone = PhoneNumber.Text;
            var email = Email.Text;
            var roleid = (int)Role.SelectedValue;

            // Tìm người dùng trong danh sách đã load từ DB
            var userToUpdate = dataContext.Users.FirstOrDefault(u => u.UserID == id);

            if (userToUpdate != null)
            {
                userToUpdate.Username = name;
                userToUpdate.PasswordHash = pass;
                userToUpdate.Email = email;
                userToUpdate.PhoneNumber = phone;
                userToUpdate.RoleID = roleid;
                userToUpdate.FullName = txtFullName.Text; // Cập nhật các thông tin khác tương ứng

                dataContext.SubmitChanges();
                MessageBox.Show("User updated successfully.");

                LoadData(); // Tải lại dữ liệu sau khi cập nhật
            }
            else
            {
                MessageBox.Show("User not found.");
            }
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn không
            if (DataGridView.SelectedRows.Count > 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow row = DataGridView.SelectedRows[0];

                // Hiển thị thông tin từ dòng được chọn lên các control phù hợp
                ID.Text = row.Cells[0].Value.ToString();
                UserName.Text = row.Cells[1].Value.ToString();
                Password.Text = row.Cells[2].Value.ToString();
                Email.Text = row.Cells[4].Value.ToString();
                PhoneNumber.Text = row.Cells[5].Value.ToString() == null ? "" : row.Cells[5].Value.ToString();
                Role.SelectedValue = row.Cells[6].Value; // Giả sử Role là ComboBox và có ValueMember là RoleID
                txtFullName.Text = row.Cells[3].Value.ToString();
                // Các xử lý khác tùy theo yêu cầu của bạn
            }
        }
        private void InsertUser()
        {
            if (!ValidateInputs())
            {
                return;
            }

            var name = UserName.Text;
            var pass = Password.Text;
            var phone = PhoneNumber.Text;
            var email = Email.Text;
            var roleid = (int)Role.SelectedValue;

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
            if (string.IsNullOrWhiteSpace(UserName.Text))
            {
                MessageBox.Show("User Name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Password is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email.Text) || !IsValidEmail(Email.Text))
            {
                MessageBox.Show("A valid Email is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber.Text) || !IsValidPhone(PhoneNumber.Text))
            {
                MessageBox.Show("A valid Phone Number is required.");
                return false;
            }

            if (Role.SelectedIndex == -1)
            {
                MessageBox.Show("Role must be selected.");
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

        private void ShowLoginForm()
        {
            if (Constant.User == null)
            {
                using (Login login = new Login())
                {
                    login.ShowDialog();
                }

                if (Constant.User != null)
                {
                    Login.Visible = false;
                    AccountManager.Visible = true;
                    Logout.Visible = true;
                    LoadData();
                }
            }
        }

        private void UpdateMenuItems()
        {
            Login.Visible = true;
            AccountManager.Visible = false;
            Logout.Visible = false;
            DataGridView.DataSource = null;
            DataGridView.Rows.Clear();
            DataGridView.Refresh();
        }

        private void LoadData()
        {
            DataGridView.DataSource = dataContext.Users.ToList();

            Role.DataSource = dataContext.Roles.ToList();
            Role.DisplayMember = "RoleName";
            Role.ValueMember = "RoleId";
        }
    }
}
