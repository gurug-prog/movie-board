using Terminal.Gui;

namespace MyConsoleProject
{
    public class CreateActorDialog : Dialog
    {
        public bool canceled;
        protected TextField actorFullNameInput;
        protected TextField actorAgeInput;
        protected TextField actorRolePlanInput;

        public CreateActorDialog()
        {
            this.Title = "Create actor";

            var okButton = new Button("OK");
            okButton.Clicked += OnCreateDialogSubmit;

            var cancelButton = new Button("Cancel");
            cancelButton.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelButton);
            this.AddButton(okButton);

            int rightColumnX = 20;

            var actorFullNameInputLbl = new Label(2, 2, "Fullname:");
            actorFullNameInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(actorFullNameInputLbl),
                Width = 40,
            };
            this.Add(actorFullNameInputLbl, actorFullNameInput);

            var actorAgeInpurLbl = new Label(2, 4, "Age:");
            actorAgeInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(actorAgeInpurLbl),
                Width = 40,
            };
            this.Add(actorAgeInpurLbl, actorAgeInput);

            var actorRolePlanLbl = new Label(2, 6, "Roleplan:");
            actorRolePlanInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(actorRolePlanLbl),
                Width = 40,
            };
            this.Add(actorRolePlanLbl, actorRolePlanInput);
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

        public Actor GetActor()
        {
            return new Actor()
            {
                fullName = actorFullNameInput.Text.ToString(),
                age = int.Parse(actorAgeInput.Text.ToString()),
                rolePlan = actorRolePlanInput.Text.ToString(),
            };
        }
    }
}