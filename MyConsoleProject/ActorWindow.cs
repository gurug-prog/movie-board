using Terminal.Gui;
using System.Collections.Generic;
using System;
using NStack;

namespace MyConsoleProject
{
    public class ActorWindow : Window
    {
        private Label emptyDBLbl;
        private ListView actorsListView;
        private ActorRepository actorRepo;
        private Label totalPagesLbl;
        private FrameView frameView;
        private Label pageLbl;
        private TextField searchInput;
        private Button prevPageBtn, nextPageBtn;
        private int pageLength = 10;
        private int page = 1;
        private string searchValue = "";

        public ActorWindow()
        {
            this.Title = "Actor Database";
            emptyDBLbl = new Label(2, 14, "Nothing found");
            this.Add(emptyDBLbl);

            var frame = new Rect(0, 0, 200, 10);
            actorsListView = new ListView(frame, new List<Actor>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            actorsListView.OpenSelectedItem += OnOpenActor;

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

            frameView = new FrameView("Actors")
            {
                X = 2,
                Y = 8,
                Width = Dim.Percent(62),
                Height = pageLength + 2,
            };
            frameView.Add(actorsListView);
            this.Add(frameView);

            var createActorBtn = new Button(2, 2, "Create new actor");
            createActorBtn.Clicked += OnCreateButtonClicked;
            this.Add(createActorBtn);

            searchInput = new TextField(2, 4, 20, "");
            searchInput.TextChanged += OnSearchChanging;
            this.Add(searchInput);

            var radioGroup = new RadioGroup(new NStack.ustring[]{ "film", "user", "review", "actor" })
            {
                X = Pos.AnchorEnd() - Pos.Percent(20),
                Y = Pos.AnchorEnd() - Pos.Percent(20),
            };
            this.Add(radioGroup);
        }

        private void OnSearchChanging(ustring args)
        {
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentPage();
        }

        public void SetRepository(ActorRepository actorRepo)
        {
            this.actorRepo = actorRepo;
            this.ShowCurrentPage();
        }

        private void ShowCurrentPage()
        {
            int pages = actorRepo.GetSearchPagesCount(searchValue);
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
                this.actorsListView.SetSource(actorRepo.GetSearchPages(searchValue, page));
            }
            catch (Exception)
            {
                MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
            }
        }


        public void OnCreateButtonClicked()
        {
            var dialog = new CreateActorDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                var actor = new Actor();
                try
                {
                    actor = dialog.GetActor();
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Got wrong input", "OK");
                    return;
                }
                actor.id = actorRepo.Insert(actor);

                ShowCurrentPage();
                try
                {
                    this.actorsListView.SetSource(actorRepo.GetPage(page));
                }
                catch (Exception)
                {
                    MessageBox.ErrorQuery("Error", "Unable to get empty data", "OK");
                }
                OpenActor(actor);
            }
        }

        private void OpenActor(Actor actor)
        {
            var dialog = new OpenActorDialog();
            dialog.SetActor(actor);

            Application.Run(dialog);

            if (dialog.isUpdated)
            {
                bool result = actorRepo.Update(actor.id, dialog.GetActor());
                if (result)
                {
                    actorsListView.SetSource(actorRepo.GetPage(page));
                }
                else
                {
                    MessageBox.ErrorQuery("Update actor", "Cannot update actor", "OK");
                }
            }

            if (dialog.isDeleted)
            {
                bool result = actorRepo.DeleteById(actor.id);
                if (result == true)
                {
                    int pages = (int)actorRepo.GetTotalPages();
                    if (page > pages && page > 1)
                    {
                        page--;
                        try
                        {
                            this.actorsListView.SetSource(actorRepo.GetPage(page));
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
                    MessageBox.ErrorQuery("Delete actor", "Cannot delete actor", "OK");
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
            int totalPages = actorRepo.GetTotalPages();
            if (page >= totalPages)
            {
                return;
            }

            this.page++;
            ShowCurrentPage();
        }

        public void OnOpenActor(ListViewItemEventArgs args)
        {
            var actor = (Actor)args.Value;
            OpenActor(actor);
        }
    }
}