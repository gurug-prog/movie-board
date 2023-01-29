using Terminal.Gui;
using System;

namespace MyConsoleProject
{
    public class OpenReviewDialog : Dialog
    {
        protected Review review;
        public bool isDeleted;
        public bool isUpdated;
        private TextField reviewHeaderInput;
        private TextField reviewOverviewInput;
        private TextField reviewRatingInput;
        private TextField reviewFilmIdInput;
        private TextField reviewUserIdInput;

        public OpenReviewDialog()
        {
            this.Title = "Review";

            var backButton = new Button("Back");
            backButton.Clicked += OnCreateDialogSubmit;
            this.AddButton(backButton);

            int rightColumnX = 20;

            var reviewHeaderLbl = new Label(2, 2, "Header:");
            reviewHeaderInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewHeaderLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(reviewHeaderLbl, reviewHeaderInput);

            var reviewOverviewLbl = new Label(2, 4, "Overview:");
            reviewOverviewInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewOverviewLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(reviewOverviewLbl, reviewOverviewInput);

            var reviewRatingLbl = new Label(2, 6, "Rating:");
            reviewRatingInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewRatingLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(reviewRatingLbl, reviewRatingInput);

            var reviewFilmIdLbl = new Label(2, 8, "FilmId:");
            reviewFilmIdInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewFilmIdLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(reviewFilmIdLbl, reviewFilmIdInput);

            var reviewUserIdLbl = new Label(2, 10, "UserId:");
            reviewUserIdInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(reviewUserIdLbl),
                Width = 40,
                ReadOnly = true,
            };
            this.Add(reviewUserIdLbl, reviewUserIdInput);

            var editButton = new Button(2, 12, "Edit");
            editButton.Clicked += OnReviewEdit;
            this.AddButton(editButton);

            var deleteButton = new Button(rightColumnX, 12, "Delete")
            {
                X = Pos.Right(editButton) + 2,
                Y = Pos.Top(editButton),
            };
            deleteButton.Clicked += OnReviewDelete;
            this.AddButton(deleteButton);
        }

        private void OnReviewEdit()
        {
            var editReviewDlg = new EditReviewDialog();
            editReviewDlg.SetReview(this.review);

            Application.Run(editReviewDlg);

            if (editReviewDlg.canceled == false)
            {
                var updatedReview = new Review();
                try
                {
                    updatedReview = editReviewDlg.GetReview();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                this.isUpdated = true;
                this.SetReview(updatedReview);
            }
        }

        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }

        private void OnReviewDelete()
        {
            var index = MessageBox.Query("Delete review", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.isDeleted = true;
                Application.RequestStop();
            }
        }

        public void SetReview(Review review)
        {
            this.review = review;
            this.reviewHeaderInput.Text = review.header;
            this.reviewOverviewInput.Text = review.overview;
            this.reviewRatingInput.Text = review.rating.ToString();
            this.reviewFilmIdInput.Text = review.filmId.ToString();
            this.reviewUserIdInput.Text = review.userId.ToString();
        }

        public Review GetReview()
        {
            return review;
        }
    }
}