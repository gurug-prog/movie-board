namespace MyConsoleProject
{
    public class EditUserDialog : CreateUserDialog
    {
        public EditUserDialog()
        {
            this.Title = "Edit user";
        }

        public void SetUser(User user)
        {
            this.userLoginInput.Text = user.login;
            this.userPasswordInput.Text = user.password;
            this.userRoleInput.Text = user.role;
        }
    }
}