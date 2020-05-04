using HR.Model;
using HR.UI.Data;
using HR.UI.Data.Lookups;
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
            _eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);
        }

        public async Task LoadAsync()
        {
            var lookup = await _candidateLookupService.GetCandidateAsync();
            Candidates.Clear();
            foreach (var item in lookup)
            {
                Candidates.Add(new NavigationItemViewModel(item.Id,item.DisplayMember, 
                    _eventAggregator,
                    nameof(CandidateDetailViewModel)));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Candidates { get; }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            //switch between view models
            switch(args.ViewModelName)
            {
                case nameof(CandidateDetailViewModel):
                    var candidate = Candidates
                        .SingleOrDefault(c => c.Id == args.Id);
                    if (candidate != null)
                    {
                        Candidates.Remove(candidate);
                    }
                    break;
            }    
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs obj)
        {
            //switch between view models
            switch (obj.ViewModelName)
            {
                case nameof(CandidateDetailViewModel):
                    var lookupItem = Candidates.SingleOrDefault(l => l.Id == obj.Id);
                    if (lookupItem == null)
                    {
                        Candidates.Add(new NavigationItemViewModel(obj.Id, obj.DisplayMember,
                            _eventAggregator,
                            nameof(CandidateDetailViewModel)));
                    }
                    else
                    {
                        lookupItem.DisplayMember = obj.DisplayMember;
                    }
                    break;
            }
            
        }
    }
}
