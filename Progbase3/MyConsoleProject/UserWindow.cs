using Terminal.Gui;
using System.Collections.Generic;
using System;
using NStack;

namespace MyConsoleProject
{
    public class UserWindow : Window
    {
        private Label emptyDBLbl;
        private ListView usersListView;
        private UserRepository userRepo;
        private Label totalPagesLbl;
        private FrameView frameView;
        private Label pageLbl;
        private TextField searchInput;
        private Button prevPageBtn, nextPageBtn;
        private int pageLength = 10;
        private int page = 1;
        private string searchValue = "";

        public UserWindow()
        {
            this.Title = "User Database";
            emptyDBLbl = new Label(2, 14, "Nothing found");
            this.Add(emptyDBLbl);

            var frame = new Rect(0, 0, 200, 10);
            usersListView = new ListView(frame, new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            usersListView.OpenSelectedItem += OnOpenUser;

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

            frameView = new FrameView("Users")
            {
                X = 2,
                Y = 8,
                Width = Dim.Percent(62),
                Height = pageLength + 2,
            };
            frameView.Add(usersListView);
            this.Add(frameView);

            var createUserBtn = new Button(2, 2, "Create new user");
            createUserBtn.Clicked += OnCreateButtonClicked;
            this.Add(createUserBtn);

            searchInput = new TextField(2, 4, 20, "");
            searchInput.TextChanged += OnSearchChanging;
            this.Add(searchInput);

            // var radioWidth = 10;
            var radioGroup = new RadioGroup(new NStack.ustring[]{ "film", "user", "review", "actor" })
            {
                X = Pos.AnchorEnd() - Pos.Percent(20),
                Y = Pos.AnchorEnd() - Pos.Percent(20),
            };
            this.Add(radioGroup);
            // var chooseUserButton = new Button("user")
            // {
            //     X = Pos.AnchorEnd() - Pos.Percent(20),
            //     Y = Pos.AnchorEnd() - Pos.Percent(70),
            // };
            // this.Add(chooseUserButton);

            // var chooseButton = new Button("films")
            // {
            //     X = Pos.AnchorEnd() - Pos.Percent(20),
            //     Y = Pos.AnchorEnd() - Pos.Percent(70),
            // };
            // this.Add(chooseUserButton);

            // var chooseUserButton = new Button("reviews")
            // {
            //     X = Pos.AnchorEnd() - Pos.Percent(20),
            //     Y = Pos.AnchorEnd() - Pos.Percent(70),
            // };
            // this.Add(chooseUserButton);

            // var chooseUserButton = new Button("actors")
            // {
            //     X = Pos.AnchorEnd() - Pos.Percent(20),
            //     Y = Pos.AnchorEnd() - Pos.Percent(70),
            // };
            // this.Add(chooseUserButton);
        }

        private void OnSearchChanging(ustring args)
        {
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentPage();
        }

        public void SetRepository(UserRepository userRepo)
        {
            this.userRepo = userRepo;
            this.ShowCurrentPage();
        }

        private void ShowCurrentPage()
        {
            int pages = userRepo.GetSearchPagesCount(searchValue);
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
                this.usersListView.SetSource(userRepo.GetSearchPages(searchValue, page));
            }
            catch (Exception)
            {
                MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
            }
        }


        public void OnCreateButtonClicked()
        {
            var dialog = new CreateUserDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                var user = new User();
                try
                {
                    user = dialog.GetUser();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                user.id = userRepo.Insert(user);

                ShowCurrentPage();
                try
                {
                    this.usersListView.SetSource(userRepo.GetPage(page));
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
                }
                OpenUser(user);
            }
        }

        private void OpenUser(User user)
        {
            OpenUserDialog dialog = new OpenUserDialog();
            dialog.SetUser(user);

            Application.Run(dialog);

            if (dialog.isUpdated)
            {
                bool result = userRepo.Update(user.id, dialog.GetUser());
                if (result)
                {
                    usersListView.SetSource(userRepo.GetPage(page));
                }
                else
                {
                    MessageBox.ErrorQuery("Update user", "Cannot update user", "OK");
                }
            }

            if (dialog.isDeleted)
            {
                bool result = userRepo.DeleteById(user.id);
                if (result == true)
                {
                    int pages = (int)userRepo.GetTotalPages();
                    if (page > pages && page > 1)
                    {
                        page--;
                        try
                        {
                            this.usersListView.SetSource(userRepo.GetPage(page));
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
                    MessageBox.ErrorQuery("Delete user", "Cannot delete user", "OK");
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
            int totalPages = userRepo.GetTotalPages();
            if (page >= totalPages)
            {
                return;
            }

            this.page++;
            ShowCurrentPage();
        }

        public void OnOpenUser(ListViewItemEventArgs args)
        {
            var user = (User)args.Value;
            OpenUser(user);
        }
    }
}