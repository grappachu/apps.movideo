using System.Windows.Input;
using Grappachu.Apps.Movideo.Common;
using Grappachu.Apps.Movideo.Views.Dialogs;

namespace Grappachu.Apps.Movideo.ViewModels
{
    public class NavBarPresenter : ObservableObject
    {
        private NavBarAction _currentProduct;
        private ICommand _navigateCatalogCommand;
        private ICommand _navigateSearchCommand;
        private ICommand _openSettingsCommand;

        public NavBarAction CurrentNavAction
        {
            get { return _currentProduct; }
            set
            {
                if (value != _currentProduct)
                {
                    _currentProduct = value;
                    RaisePropertyChangedEvent(nameof(CurrentNavAction));
                }
            }
        }


        public ICommand NavigateCatalogCommand
        {
            get
            {
                if (_navigateCatalogCommand == null)
                    _navigateCatalogCommand = new RelayCommand(
                        param => GoToCatalog(),
                        param => CurrentNavAction != null && CurrentNavAction.Title != "Catalog"
                    );
                return _navigateCatalogCommand;
            }
        }

        public ICommand NavigateSearchCommand
        {
            get
            {
                return _navigateSearchCommand ?? (_navigateSearchCommand = new RelayCommand(
                           param => GoToSearch(),
                           param => CurrentNavAction != null && CurrentNavAction.Title != "Search"
                       ));
            }
        }

        public ICommand OpenSettingsCommand
        {
            get
            {
                return _openSettingsCommand ?? (_openSettingsCommand = new RelayCommand(
                           param =>
                           {
                               var dlg = new ScanSettingsDialog();
                               dlg.ShowDialog();
                           },
                           param => true
                       ));
            }
        }


        private void GoToSearch()
        {
            var p = new NavBarAction { Title = "Search" };
            CurrentNavAction = p;
        }

        private void GoToCatalog()
        {
            var p = new NavBarAction { Title = "Catalog" };
            CurrentNavAction = p;
        }
    }
}