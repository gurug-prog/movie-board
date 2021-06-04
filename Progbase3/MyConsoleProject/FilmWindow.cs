using Terminal.Gui;
using System.Collections.Generic;
using System;
using NStack;

namespace MyConsoleProject
{
    public class FilmWindow : Window
    {
        private Label emptyDBLbl;
        private ListView filmsListView;
        private FilmRepository filmRepo;
        private Label totalPagesLbl;
        private FrameView frameView;
        private Label pageLbl;
        private TextField searchInput;
        private Button prevPageBtn, nextPageBtn;
        private int pageLength = 10;
        private int page = 1;
        private string searchValue = "";

        public FilmWindow()
        {
            this.Title = "Film Database";
            emptyDBLbl = new Label(2, 14, "Nothing found");
            this.Add(emptyDBLbl);

            var frame = new Rect(0, 0, 200, 10);
            filmsListView = new ListView(frame, new List<Film>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            filmsListView.OpenSelectedItem += OnOpenFilm;

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

            frameView = new FrameView("Films")
            {
                X = 2,
                Y = 8,
                Width = Dim.Percent(62),
                Height = pageLength + 2,
            };
            frameView.Add(filmsListView);
            this.Add(frameView);

            var createFilmBtn = new Button(2, 2, "Create new film");
            createFilmBtn.Clicked += OnCreateButtonClicked;
            this.Add(createFilmBtn);

            searchInput = new TextField(2, 4, 20, "");
            searchInput.TextChanged += OnSearchChanging;
            this.Add(searchInput);

            var radioGroup = new RadioGroup(new NStack.ustring[]{ "film", "user", "review", "actor" })
            {
                X = Pos.AnchorEnd() - Pos.Percent(20),
                Y = Pos.AnchorEnd() - Pos.Percent(60),
            };
            this.Add(radioGroup);
        }

        private void OnSearchChanging(ustring args)
        {
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentPage();
        }

        public void SetRepository(FilmRepository filmRepo)
        {
            this.filmRepo = filmRepo;
            this.ShowCurrentPage();
        }

        private void ShowCurrentPage()
        {
            int pages = filmRepo.GetSearchPagesCount(searchValue);
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
                this.filmsListView.SetSource(filmRepo.GetSearchPages(searchValue, page));
            }
            catch (Exception)
            {
                MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
            }
        }


        public void OnCreateButtonClicked()
        {
            var dialog = new CreateFilmDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                var film = new Film();
                try
                {
                    film = dialog.GetFilm();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                film.id = filmRepo.Insert(film);

                ShowCurrentPage();
                try
                {
                    this.filmsListView.SetSource(filmRepo.GetPage(page));
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
                }
                OpenFilm(film);
            }
        }

        private void OpenFilm(Film film)
        {
            var dialog = new OpenFilmDialog();
            dialog.SetFilm(film);

            Application.Run(dialog);

            if (dialog.isUpdated)
            {
                bool result = filmRepo.Update(film.id, dialog.GetFilm());
                if (result)
                {
                    filmsListView.SetSource(filmRepo.GetPage(page));
                }
                else
                {
                    MessageBox.ErrorQuery("Update film", "Cannot update film", "OK");
                }
            }

            if (dialog.isDeleted)
            {
                bool result = filmRepo.DeleteById(film.id);
                if (result == true)
                {
                    int pages = (int)filmRepo.GetTotalPages();
                    if (page > pages && page > 1)
                    {
                        page--;
                        try
                        {
                            this.filmsListView.SetSource(filmRepo.GetPage(page));
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
                    MessageBox.ErrorQuery("Delete film", "Cannot delete film", "OK");
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
            int totalPages = filmRepo.GetTotalPages();
            if (page >= totalPages)
            {
                return;
            }

            this.page++;
            ShowCurrentPage();
        }

        public void OnOpenFilm(ListViewItemEventArgs args)
        {
            var film = (Film)args.Value;
            OpenFilm(film);
        }
    }
}