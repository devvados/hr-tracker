using HR.Model;
using HR.UI.Data;
using HR.UI.Event;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace HR.UI.ViewModel
{
    class CandidateDetailViewModel : ViewModelBase, ICandidateDetailViewModel
    {
        private ICandidateDataService _dataService;
        private IEventAggregator _eventAggregator;

        public CandidateDetailViewModel(ICandidateDataService dataService,
            IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenCandidateDetailViewEvent>()
                .Subscribe(OnOpenCandidateDetailView);

        }

        private async void OnOpenCandidateDetailView(int candidateId)
        {
            await LoadAsync(candidateId);
        }

        public async Task LoadAsync(int candidateId)
        {
            Candidate = await _dataService.GetByIdAsync(candidateId);
        }

        private Candidate _candidate;

        public Candidate Candidate
        {
            get { return _candidate; }
            private set
            {
                _candidate = value;
                OnPropertyChanged();
            }
        }
    }
}
