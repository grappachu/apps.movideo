using System.Windows.Input;
using Grappachu.Apps.Movideo.ViewModels.Commands;

namespace Grappachu.Apps.Movideo.ViewModels
{
    public class NavBarPresenter : ObservableObject
    {
        private NavBarAction _currentProduct;
        private ICommand _navigateCatalogCommand;
        private ICommand _navigateSearchCommand;

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
                if (_navigateSearchCommand == null)
                    _navigateSearchCommand = new RelayCommand(
                        param => GoToSearch(),
                        param => CurrentNavAction != null && CurrentNavAction.Title != "Search"
                    );
                return _navigateSearchCommand;
            }
        }

        public ICommand OpenSettingsCommand => OpenSettings.GetCommand();


        private void GoToSearch()
        {
            var p = new NavBarAction {Title = "Search"};
            CurrentNavAction = p;
        }

        private void GoToCatalog()
        {
            var p = new NavBarAction {Title = "Catalog"};
            CurrentNavAction = p;
        }
    }
}