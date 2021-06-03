namespace MyConsoleProject
{
    public class EditReviewDialog : CreateReviewDialog
    {
        public EditReviewDialog()
        {
            this.Title = "Edit review";
        }

        public void SetReview(Review review)
        {
            this.reviewHeaderInput.Text = review.header;
            this.reviewOverviewInput.Text = review.overview;
            this.reviewRatingInput.Text = review.rating.ToString();
            this.reviewFilmIdInput.Text = review.filmId.ToString();
            this.reviewUserIdInput.Text = review.userId.ToString();
        }
    }
}