using HR.Model;
using HR.UI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationViewModel navigationViewModel,
            ICandidateDetailViewModel candidateDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            CandidateDetailViewModel = candidateDetailViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public INavigationViewModel NavigationViewModel { get; }
        public ICandidateDetailViewModel CandidateDetailViewModel { get; }
    }
}
