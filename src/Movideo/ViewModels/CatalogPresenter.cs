using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

        private async void LoadCatalogAsync()
        {
            await new TaskFactory().StartNew(LoadCatalog);
        }
    }
}