namespace MyConsoleProject
{
    public class EditFilmDialog : CreateFilmDialog
    {
        public EditFilmDialog()
        {
            this.Title = "Edit film";
        }

        public void SetFilm(Film film)
        {
            this.filmTitleInput.Text = film.title;
            this.filmDirectorInput.Text = film.director;
            this.filmCountryInput.Text = film.country;
            this.filmReleaseYearInput.Text = film.releaseYear.ToString();
            this.filmDurationInput.Text = film.duration.ToString();
        }
    }
}