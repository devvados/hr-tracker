using HR.Model;
using HR.UI.Data;
using HR.UI.Event;
using HR.UI.Wrapper;
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
        private CandidateWrapper _candidate;

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

        public async Task LoadAsync(int candidateId)
        {
            var candidate = await _dataService.GetByIdAsync(candidateId);

            //subscribe save command availability to object state change
            Candidate = new CandidateWrapper(candidate);
            Candidate.PropertyChanged += (s, e) =>
              {
                  if (e.PropertyName == nameof(Candidate.HasErrors))
                  {
                      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                  }
              };
        }

        public CandidateWrapper Candidate
        {
            get { return _candidate; }
            private set
            {
                _candidate = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute()
        {
            //check if candidate has changes
            return Candidate!=null  && !Candidate.HasErrors;
        }

        private async void OnSaveExecute()
        {
            await _dataService.SaveAsync(Candidate.Model);
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
    }
}
