using HR.Model;
using HR.UI.Data;
using HR.UI.Data.Repositories;
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
        private ICandidateRepository _candidateRepository;
        private IEventAggregator _eventAggregator;
        private CandidateWrapper _candidate;
        private bool _hasChanges;

        public CandidateDetailViewModel(ICandidateRepository candidateRepository,
            IEventAggregator eventAggregator)
        {
            _candidateRepository = candidateRepository;
            _eventAggregator = eventAggregator;

            //parameter can be added as modifying method to generic
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(int candidateId)
        {
            var candidate = await _candidateRepository.GetByIdAsync(candidateId);

            //subscribe save command availability to object state change
            Candidate = new CandidateWrapper(candidate);
            Candidate.PropertyChanged += (s, e) =>
              {
                  if(HasChanges)
                  {
                      HasChanges = _candidateRepository.HasChanges();
                  }
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

        public bool HasChanges
        {
            get { return _hasChanges; }
            set {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }


        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute()
        {
            return Candidate!=null  && !Candidate.HasErrors && HasChanges;
        }

        private async void OnSaveExecute()
        {
            await _candidateRepository.SaveAsync();

            HasChanges = _candidateRepository.HasChanges();
            _eventAggregator.GetEvent<AfterCandidateSavedEvent>()
                .Publish(new AfterCandidateSavedEventArgs
                {
                    Id = Candidate.Id,
                    DisplayMember = $"{Candidate.Name} {Candidate.LastName}"
                });
        }
    }
}
