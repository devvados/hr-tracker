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
        private IMeetingLookupDataService _meetingLookupService;
        private IEventAggregator _eventAggregator; 

        public NavigationViewModel(ICandidateLookupDataService candidateLookupService,
            IMeetingLookupDataService meetingLookupService,
            IEventAggregator eventAggregator)
        {
            _candidateLookupService = candidateLookupService;
            _meetingLookupService = meetingLookupService;
            _eventAggregator = eventAggregator;

            Candidates = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();

            _eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);
        }

        public async Task LoadAsync()
        {
            var lookup = await _candidateLookupService.GetCandidateLookupAsync();
            Candidates.Clear();
            foreach (var item in lookup)
            {
                Candidates.Add(new NavigationItemViewModel(item.Id,item.DisplayMember, 
                    _eventAggregator,
                    nameof(CandidateDetailViewModel)));
            }

            lookup = await _meetingLookupService.GetMeetingLookupAsync();
            Meetings.Clear();
            foreach (var item in lookup)
            {
                Meetings.Add(new NavigationItemViewModel(item.Id, item.DisplayMember,
                    _eventAggregator,
                    nameof(MeetingDetailViewModel)));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Candidates { get; }

        public ObservableCollection<NavigationItemViewModel> Meetings { get; }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            //switch between view models
            switch(args.ViewModelName)
            {
                case nameof(CandidateDetailViewModel):
                    AfterDetailDeleted(Candidates, args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailDeleted(Meetings, args);
                    break;
            }    
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            //switch between view models
            switch (args.ViewModelName)
            {
                case nameof(CandidateDetailViewModel):
                    AfterDetailSaved(Candidates, args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailSaved(Meetings, args);
                    break;
            }
            
        }

        private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, 
            AfterDetailSavedEventArgs args)
        {
            var lookupItem = items.SingleOrDefault(l => l.Id == args.Id);
            if (lookupItem == null)
            {
                items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember,
                    _eventAggregator,
                    args.ViewModelName));
            }
            else
            {
                lookupItem.DisplayMember = args.DisplayMember;
            }
        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items,
            AfterDetailDeletedEventArgs args)
        {
            var item = items
                        .SingleOrDefault(c => c.Id == args.Id);
            if (item != null)
            {
                items.Remove(item);
            }
        }
    }
}
