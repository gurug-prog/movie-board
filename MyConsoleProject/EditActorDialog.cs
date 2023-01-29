namespace MyConsoleProject
{
    public class EditActorDialog : CreateActorDialog
    {
        public EditActorDialog()
        {
            this.Title = "Edit actor";
        }

        public void SetActor(Actor actor)
        {
            this.actorFullNameInput.Text = actor.fullName;
            this.actorAgeInput.Text = actor.age.ToString();
            this.actorRolePlanInput.Text = actor.rolePlan;
        }
    }
}