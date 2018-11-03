using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Grappachu.Apps.Movideo.Common;
using Grappachu.Apps.Movideo.Config;
using Grappachu.Core.Collections;
using Grappachu.Movideo.Core;
using Grappachu.Movideo.Core.Models;

namespace Grappachu.Apps.Movideo.ViewModels
{
    public class NavBarAction
    {
        public string Title { get; set; }
      
    }

    public class CatalogPresenter : ObservableObject
    {
        private readonly MovideoApp _movideoApp;
        private ICommand _deleteMovieCommand;


        public CatalogPresenter()
        {
            _movideoApp = AppFactory.GetInstance();
             
            LoadCatalogAsync();
        }

        public ObservableCollection<Movie> Movies { get; set; }

        private void LoadCatalog()
        {
            var movies = new ObservableCollection<Movie>();
            movies.AddRange(_movideoApp.GetCatalog());
            Movies = movies;
            RaisePropertyChangedEvent(nameof(Movies));
        }


        public ICommand DeleteMovieCommand
        {
            get
            {
                if (_deleteMovieCommand == null)
                    _deleteMovieCommand = new RelayCommand(
                        param =>
                        {
                            var movieId = Convert.ToInt32(param);
                        },
                        param => true
                    );
                return _deleteMovieCommand;
            }
        }

        private async void LoadCatalogAsync()
        {
            await new TaskFactory().StartNew(LoadCatalog);
        }
    }
}