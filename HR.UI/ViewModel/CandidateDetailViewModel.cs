using HR.Model;
using HR.UI.Data;
using HR.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

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

            //parameter can be added as modifying method to generic
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        private bool OnSaveCanExecute()
        {
            //check if candidate is valid
            return true;
        }

        private async void OnSaveExecute()
        {
            await _dataService.SaveAsync(Candidate);
            _eventAggregator.GetEvent<AfterCandidateSavedEvent>()
                .Publish(new AfterCandidateSavedEventArgs
                {
                    Id = Candidate.Id,
                    DisplayMember = $"{Candidate.Name} {Candidate.LastName}"
                });
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

        public ICommand SaveCommand { get; }
    }
}
