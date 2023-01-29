using Terminal.Gui;
using System.Collections.Generic;
using System;
using NStack;

namespace MyConsoleProject
{
    public class ReviewWindow : Window
    {
        private Label emptyDBLbl;
        private ListView reviewsListView;
        private ReviewRepository reviewRepo;
        private Label totalPagesLbl;
        private FrameView frameView;
        private Label pageLbl;
        private TextField searchInput;
        private Button prevPageBtn, nextPageBtn;
        private int pageLength = 10;
        private int page = 1;
        private string searchValue = "";

        public ReviewWindow()
        {
            this.Title = "Review Database";
            emptyDBLbl = new Label(2, 14, "Nothing found");
            this.Add(emptyDBLbl);

            var frame = new Rect(0, 0, 200, 10);
            reviewsListView = new ListView(frame, new List<Review>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            reviewsListView.OpenSelectedItem += OnOpenReview;

            prevPageBtn = new Button(2, 6, "Prev");
            prevPageBtn.Clicked += OnPrevPage;

            pageLbl = new Label("?")
            {
                X = Pos.Right(prevPageBtn) + 5,
                Y = Pos.Top(prevPageBtn),
                Width = 7,
            };

            totalPagesLbl = new Label("?")
            {
                X = Pos.Right(pageLbl) - 3,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            nextPageBtn = new Button("Next")
            {
                X = Pos.Right(totalPagesLbl) + 2,
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextPage;

            this.Add(prevPageBtn, pageLbl, totalPagesLbl, nextPageBtn);

            frameView = new FrameView("Reviews")
            {
                X = 2,
                Y = 8,
                Width = Dim.Percent(80),
                Height = pageLength + 2,
            };
            frameView.Add(reviewsListView);
            this.Add(frameView);

            var createReviewBtn = new Button(2, 2, "Create new review");
            createReviewBtn.Clicked += OnCreateButtonClicked;
            this.Add(createReviewBtn);

            searchInput = new TextField(2, 4, 20, "");
            searchInput.TextChanged += OnSearchChanging;
            this.Add(searchInput);
        }

        private void OnSearchChanging(ustring args)
        {
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentPage();
        }

        public void SetRepository(ReviewRepository reviewRepo)
        {
            this.reviewRepo = reviewRepo;
            this.ShowCurrentPage();
        }

        private void ShowCurrentPage()
        {
            int pages = reviewRepo.GetSearchPagesCount(searchValue);
            if (pages != 0)
            {
                emptyDBLbl.Visible = false;
            }

            if (page > pages && page >= 1 && pages != 0)
            {
                page = pages;
            }

            this.pageLbl.Text = pages == 0 ? "1 /" : page.ToString() + " /";
            this.totalPagesLbl.Text = pages == 0 ? "1" : pages.ToString();

            if (pages == 0)
            {
                prevPageBtn.Visible = false;
                nextPageBtn.Visible = false;

                frameView.Visible = false;
                emptyDBLbl.Visible = true;
            }
            else
            {
                prevPageBtn.Visible = true;
                nextPageBtn.Visible = true;

                frameView.Visible = true;
            }

            if (System.Math.Min(pages, page) == 1)
            {
                prevPageBtn.Visible = false;
            }

            if (System.Math.Min(pages, page) == int.Parse(totalPagesLbl.Text.ToString()))
            {
                nextPageBtn.Visible = false;
            }

            try
            {
                this.reviewsListView.SetSource(reviewRepo.GetSearchPages(searchValue, page));
            }
            catch (Exception)
            {
                MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
            }
        }


        public void OnCreateButtonClicked()
        {
            var dialog = new CreateReviewDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                var review = new Review();
                try
                {
                    review = dialog.GetReview();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                review.id = reviewRepo.Insert(review);

                ShowCurrentPage();
                try
                {
                    this.reviewsListView.SetSource(reviewRepo.GetPage(page));
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
                }
                OpenReview(review);
            }
        }

        private void OpenReview(Review review)
        {
            var dialog = new OpenReviewDialog();
            dialog.SetReview(review);

            Application.Run(dialog);

            if (dialog.isUpdated)
            {
                bool result = reviewRepo.Update(review.id, dialog.GetReview());
                if (result)
                {
                    reviewsListView.SetSource(reviewRepo.GetPage(page));
                }
                else
                {
                    MessageBox.ErrorQuery("Update review", "Cannot update review", "OK");
                }
            }

            if (dialog.isDeleted)
            {
                bool result = reviewRepo.DeleteById(review.id);
                if (result == true)
                {
                    int pages = (int)reviewRepo.GetTotalPages();
                    if (page > pages && page > 1)
                    {
                        page--;
                        try
                        {
                            this.reviewsListView.SetSource(reviewRepo.GetPage(page));
                        }
                        catch (Exception)
                        {
                            MessageBox.ErrorQuery("Error", "Cannot get null data from database", "OK");
                        }
                    }
                    ShowCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete review", "Cannot delete review", "OK");
                }
            }
        }

        private void OnPrevPage()
        {
            if (page == 1)
            {
                return;
            }

            this.page--;
            ShowCurrentPage();
        }

        private void OnNextPage()
        {
            int totalPages = reviewRepo.GetTotalPages();
            if (page >= totalPages)
            {
                return;
            }

            this.page++;
            ShowCurrentPage();
        }

        public void OnOpenReview(ListViewItemEventArgs args)
        {
            var review = (Review)args.Value;
            OpenReview(review);
        }
    }
}