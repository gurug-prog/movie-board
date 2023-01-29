using Terminal.Gui;
using System;

namespace MyConsoleProject
{
    public class OpenUserDialog : Dialog
    {
        protected User user;
        public bool isDeleted;
        public bool isUpdated;
        private TextField userLoginInput;
        private TextField userPasswordInput;
        private TextField userRoleInput;
        private TextField userSignUpDateInput;

        public OpenUserDialog()
        {
            this.Title = "User";

            var backButton = new Button("Back");
            backButton.Clicked += OnCreateDialogSubmit;
            this.AddButton(backButton);

            int rightColumnX = 20;

            var userLoginLbl = new Label(2, 2, "Login:");
            userLoginInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(userLoginLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(userLoginLbl, userLoginInput);

            var userPasswordLbl = new Label(2, 4, "Password:");
            userPasswordInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(userPasswordLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(userPasswordLbl, userPasswordInput);

            var userRoleLbl = new Label(2, 6, "Role:");
            userRoleInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(userRoleLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(userRoleLbl, userRoleInput);

            var userSingUpDateLbl = new Label(2, 8, "Last seen:");
            userSignUpDateInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(userSingUpDateLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(userSingUpDateLbl, userSignUpDateInput);

            var editButton = new Button(2, 10, "Edit");
            editButton.Clicked += OnUserEdit;
            this.AddButton(editButton);

            var deleteButton = new Button(rightColumnX, 10, "Delete")
            {
                X = Pos.Right(editButton) + 2,
                Y = Pos.Top(editButton),
            };
            deleteButton.Clicked += OnUserDelete;
            this.AddButton(deleteButton);
        }

        private void OnUserEdit()
        {
            var editUserDlg = new EditUserDialog();
            editUserDlg.SetUser(this.user);

            Application.Run(editUserDlg);

            if (editUserDlg.canceled == false)
            {
                var updatedUser = new User();
                try
                {
                    updatedUser = editUserDlg.GetUser();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                this.isUpdated = true;
                this.SetUser(updatedUser);
            }
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }

        private void OnUserDelete()
        {
            var index = MessageBox.Query("Delete user", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.isDeleted = true;
                Application.RequestStop();
            }
        }

        public void SetUser(User user)
        {
            this.user = user;
            this.userLoginInput.Text = user.login;
            this.userPasswordInput.Text = user.password;
            this.userRoleInput.Text = user.role.ToString();
            this.userSignUpDateInput.Text = user.signUpDate.ToString();
        }

        public User GetUser()
        {
            return user;
        }
    }
}