using System;
using Terminal.Gui;

namespace MyConsoleProject
{
    public class CreateFilmDialog : Dialog
    {
        public bool canceled;
        protected TextField filmTitleInput;
        protected TextField filmDirectorInput;
        protected TextField filmCountryInput;
        protected TextField filmReleaseYearInput;
        protected TextField filmDurationInput;

        public CreateFilmDialog()
        {
            this.Title = "Create film";

            var okButton = new Button("OK");
            okButton.Clicked += OnCreateDialogSubmit;

            var cancelButton = new Button("Cancel");
            cancelButton.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelButton);
            this.AddButton(okButton);

            int rightColumnX = 20;

            var filmTitleLbl = new Label(2, 2, "Title:");
            filmTitleInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmTitleLbl),
                Width = 40,
            };
            this.Add(filmTitleLbl, filmTitleInput);

            var filmDirectorLbl = new Label(2, 4, "Director:");
            filmDirectorInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmDirectorLbl),
                Width = 40,
            };
            this.Add(filmDirectorLbl, filmDirectorInput);

            var filmCountryLbl = new Label(2, 6, "Country:");
            filmCountryInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmCountryLbl),
                Width = 40,
            };
            this.Add(filmCountryLbl, filmCountryInput);

            var filmReleaseYearLbl = new Label(2, 8, "Release year:");
            filmReleaseYearInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmReleaseYearLbl),
                Width = 40,
            };
            this.Add(filmReleaseYearLbl, filmReleaseYearInput);

            var filmDurationLbl = new Label(2, 10, "Duration:");
            filmDurationInput = new TimeField()
            {
                X = rightColumnX,
                Y = Pos.Top(filmDurationLbl),
                Width = 40,
            };
            this.Add(filmDurationLbl, filmDurationInput);
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

        public Film GetFilm()
        {
            return new Film()
            {
                title = filmTitleInput.Text.ToString(),
                director = filmDirectorInput.Text.ToString(),
                country = filmCountryInput.Text.ToString(),
                releaseYear = int.Parse(filmReleaseYearInput.Text.ToString()),
                duration = TimeSpan.Parse(filmDurationInput.Text.ToString()),
            };
        }
    }
}