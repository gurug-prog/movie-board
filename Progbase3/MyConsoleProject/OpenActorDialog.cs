using Terminal.Gui;
using System;

namespace MyConsoleProject
{
    public class OpenActorDialog : Dialog
    {
        protected Actor actor;
        public bool isDeleted;
        public bool isUpdated;
        private TextField actorFullNameInput;
        private TextField actorAgeInput;
        private TextField actorRolePlanInput;

        public OpenActorDialog()
        {
            this.Title = "Actor";

            var backButton = new Button("Back");
            backButton.Clicked += OnCreateDialogSubmit;
            this.AddButton(backButton);

            int rightColumnX = 20;

            var actorFullNameLbl = new Label(2, 2, "Fullname:");
            actorFullNameInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(actorFullNameLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(actorFullNameLbl, actorFullNameInput);

            var actorAgeLbl = new Label(2, 4, "Age:");
            actorAgeInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(actorAgeLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(actorAgeLbl, actorAgeInput);

            var actorRolePalanLbl = new Label(2, 6, "Roleplan:");
            actorRolePlanInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(actorRolePalanLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(actorRolePalanLbl, actorRolePlanInput);

            var editButton = new Button(2, 8, "Edit");
            editButton.Clicked += OnActorEdit;
            this.AddButton(editButton);

            var deleteButton = new Button(rightColumnX, 8, "Delete")
            {
                X = Pos.Right(editButton) + 2,
                Y = Pos.Top(editButton),
            };
            deleteButton.Clicked += OnActorDelete;
            this.AddButton(deleteButton);
        }

        private void OnActorEdit()
        {
            var editActorDlg = new EditActorDialog();
            editActorDlg.SetActor(this.actor);

            Application.Run(editActorDlg);

            if (editActorDlg.canceled == false)
            {
                var updatedActor = new Actor();
                try
                {
                    updatedActor = editActorDlg.GetActor();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                this.isUpdated = true;
                this.SetActor(updatedActor);
            }
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }

        private void OnActorDelete()
        {
            var index = MessageBox.Query("Delete actor", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.isDeleted = true;
                Application.RequestStop();
            }
        }

        public void SetActor(Actor actor)
        {
            this.actor = actor;
            this.actorFullNameInput.Text = actor.fullName;
            this.actorAgeInput.Text = actor.age.ToString();
            this.actorRolePlanInput.Text = actor.rolePlan;
        }

        public Actor GetActor()
        {
            return actor;
        }
    }
}