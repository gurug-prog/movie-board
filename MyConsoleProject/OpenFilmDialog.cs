using Terminal.Gui;
using System;

namespace MyConsoleProject
{
    public class OpenFilmDialog : Dialog
    {
        protected Film film;
        public bool isDeleted;
        public bool isUpdated;
        private TextField filmTitleInput;
        private TextField filmDirectorInput;
        private TextField filmCountryInput;
        private TextField filmReleaseYearInput;
        private TimeField filmDurationInput;

        public OpenFilmDialog()
        {
            this.Title = "Film";

            var backButton = new Button("Back");
            backButton.Clicked += OnCreateDialogSubmit;
            this.AddButton(backButton);

            int rightColumnX = 20;

            var filmTitleLbl = new Label(2, 2, "Title:");
            filmTitleInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmTitleLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(filmTitleLbl, filmTitleInput);

            var filmDirectorLbl = new Label(2, 4, "Director:");
            filmDirectorInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmDirectorLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(filmDirectorLbl, filmDirectorInput);

            var filmCountryLbl = new Label(2, 6, "Country:");
            filmCountryInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmCountryLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(filmCountryLbl, filmCountryInput);

            var filmReleaseYearLbl = new Label(2, 8, "Release year:");
            filmReleaseYearInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmReleaseYearLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(filmReleaseYearLbl, filmReleaseYearInput);

            var filmDurationLbl = new Label(2, 10, "Duration:");
            filmDurationInput = new TimeField()
            {
                X = rightColumnX,
                Y = Pos.Top(filmDurationLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(filmDurationLbl, filmDurationInput);

            var editButton = new Button(2, 12, "Edit");
            editButton.Clicked += OnFilmEdit;
            this.AddButton(editButton);

            var deleteButton = new Button(rightColumnX, 12, "Delete")
            {
                X = Pos.Right(editButton) + 2,
                Y = Pos.Top(editButton),
            };
            deleteButton.Clicked += OnFilmDelete;
            this.AddButton(deleteButton);
        }

        private void OnFilmEdit()
        {
            var editFilmDlg = new EditFilmDialog();
            editFilmDlg.SetFilm(this.film);

            Application.Run(editFilmDlg);

            if (editFilmDlg.canceled == false)
            {
                var updatedFilm = new Film();
                try
                {
                    updatedFilm = editFilmDlg.GetFilm();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                this.isUpdated = true;
                this.SetFilm(updatedFilm);
            }
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }

        private void OnFilmDelete()
        {
            var index = MessageBox.Query("Delete film", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.isDeleted = true;
                Application.RequestStop();
            }
        }

        public void SetFilm(Film film)
        {
            this.film = film;
            this.filmTitleInput.Text = film.title;
            this.filmDirectorInput.Text = film.director;
            this.filmCountryInput.Text = film.country;
            this.filmReleaseYearInput.Text = film.releaseYear.ToString();
            this.filmDurationInput.Text = film.duration.ToString();
        }

        public Film GetFilm()
        {
            return film;
        }
    }
}