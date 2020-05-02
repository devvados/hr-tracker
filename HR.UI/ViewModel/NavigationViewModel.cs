using HR.Model;
using HR.UI.Data;
using HR.UI.Event;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.UI.ViewModel
{
    class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private ICandidateLookupDataService _candidateLookupService;
        private IEventAggregator _eventAggregator; 

        public NavigationViewModel(ICandidateLookupDataService candidateLookupService,
            IEventAggregator eventAggregator)
        {
            _candidateLookupService = candidateLookupService;
            _eventAggregator = eventAggregator;
            Candidates = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterCandidateSavedEvent>()
                .Subscribe(AfterCandidateSaved);
        }

        private void AfterCandidateSaved(AfterCandidateSavedEventArgs obj)
        {
            var lookupItem = Candidates.Single(l => l.Id == obj.Id);
            lookupItem.DisplayMember = obj.DisplayMember; 
        }

        public async Task LoadAsync()
        {
            var lookup = await _candidateLookupService.GetCandidateAsync();
            Candidates.Clear();
            foreach (var item in lookup)
            {
                Candidates.Add(new NavigationItemViewModel(item.Id,item.DisplayMember));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Candidates { get; }

        private NavigationItemViewModel _selectedCandidate;

        public NavigationItemViewModel SelectedCandidate
        {
            get { return _selectedCandidate; }
            set { 
                _selectedCandidate = value;
                OnPropertyChanged();

                if(_selectedCandidate !=null)
                {
                    _eventAggregator.GetEvent<OpenCandidateDetailViewEvent>()
                        .Publish(_selectedCandidate.Id);
                }
            }
        }

    }
}
