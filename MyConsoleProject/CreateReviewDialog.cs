using Terminal.Gui;

namespace MyConsoleProject
{
    public class CreateReviewDialog : Dialog
    {
        public bool canceled;
        protected TextField reviewHeaderInput;
        protected TextField reviewOverviewInput;
        protected TextField reviewRatingInput;
        protected TextField reviewFilmIdInput;
        protected TextField reviewUserIdInput;

        public CreateReviewDialog()
        {
            this.Title = "Create review";

            var okButton = new Button("OK");
            okButton.Clicked += OnCreateDialogSubmit;

            var cancelButton = new Button("Cancel");
            cancelButton.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelButton);
            this.AddButton(okButton);

            int rightColumnX = 20;

            var reviewHeaderLbl = new Label(2, 2, "Header:");
            reviewHeaderInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewHeaderLbl),
                Width = 40,
            };
            this.Add(reviewHeaderLbl, reviewHeaderInput);

            var reviewOverviewLbl = new Label(2, 4, "Overview:");
            reviewOverviewInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewOverviewLbl),
                Width = 40,
            };
            this.Add(reviewOverviewLbl, reviewOverviewInput);

            var reviewRatingLbl = new Label(2, 6, "Rating:");
            reviewRatingInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewRatingLbl),
                Width = 40,
            };
            this.Add(reviewRatingLbl, reviewRatingInput);
            
            var reviewFilmIdLbl = new Label(2, 8, "FilmId:");
            reviewFilmIdInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewFilmIdLbl),
                Width = 40,
            };
            this.Add(reviewFilmIdLbl, reviewFilmIdInput);

            var reviewUserIdLbl = new Label(2, 10, "UserId:");
            reviewUserIdInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewUserIdLbl),
                Width = 40,
            };
            this.Add(reviewUserIdLbl, reviewUserIdInput);
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

        public Review GetReview()
        {
            return new Review()
            {
                header = reviewHeaderInput.Text.ToString(),
                overview = reviewOverviewInput.Text.ToString(),
                rating = int.Parse(reviewRatingInput.Text.ToString()),
                filmId = int.Parse(reviewFilmIdInput.Text.ToString()),
                userId = int.Parse(reviewUserIdInput.Text.ToString()),
            };
        }
    }
}