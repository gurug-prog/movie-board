using System;
using Terminal.Gui;

namespace MyConsoleProject
{
    public class CreateUserDialog : Dialog
    {
        public bool canceled;
        protected TextField userLoginInput;
        protected TextField userPasswordInput;
        protected TextField userRoleInput;
        protected TextField userSignUpDateInput;

        public CreateUserDialog()
        {
            this.Title = "Create user";

            var okButton = new Button("OK");
            okButton.Clicked += OnCreateDialogSubmit;

            var cancelButton = new Button("Cancel");
            cancelButton.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelButton);
            this.AddButton(okButton);

            int rightColumnX = 20;

            var userLoginLbl = new Label(2, 2, "Login:");
            userLoginInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(userLoginLbl),
                Width = 40,
            };
            this.Add(userLoginLbl, userLoginInput);

            var userPasswordLbl = new Label(2, 4, "Password:");
            userPasswordInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(userPasswordLbl),
                Width = 40,
            };
            this.Add(userPasswordLbl, userPasswordInput);

            var userRoleLbl = new Label(2, 6, "Role:");
            userRoleInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(userRoleLbl),
                Width = 40,
            };
            this.Add(userRoleLbl, userRoleInput);
        }

        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }

        private void OnCreateDialogSubmit()
        {
            this.canceled = false;
            Application.RequestStop();
        }

        public User GetUser()
        {
            return new User()
            {
                login = userLoginInput.Text.ToString(),
                password = userPasswordInput.Text.ToString(),
                role = userRoleInput.Text.ToString(),
                signUpDate = DateTime.Now,
            };
        }
    }
}