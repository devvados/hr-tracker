using HR.Model;
using HR.UI.Data;
using HR.UI.Data.Repositories;
using HR.UI.Event;
using HR.UI.View.Services;
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
        private IMessageDialogService _messageDialogService;
        private CandidateWrapper _candidate;
        private bool _hasChanges;

        public CandidateDetailViewModel(ICandidateRepository candidateRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _candidateRepository = candidateRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;

            //parameter can be added as modifying method to generic
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
        }

        public async Task LoadAsync(int? candidateId)
        {
            var candidate = candidateId.HasValue
                ? await _candidateRepository.GetByIdAsync(candidateId.Value)
                : CreateNewCandidate();

            //subscribe save command availability to object state change
            Candidate = new CandidateWrapper(candidate);
            Candidate.PropertyChanged += (s, e) =>
              {
                  if(!HasChanges)
                  {
                      HasChanges = _candidateRepository.HasChanges();
                  }
                  if (e.PropertyName == nameof(Candidate.HasErrors))
                  {
                      ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                  }
              };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (candidate.Id == 0)
            {   
                //trigger validation on new entity creation
                Candidate.Name = "";
            }
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

        public ICommand DeleteCommand { get; }

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

        private async void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog(
                $"Do you really want to delete candidate {Candidate.Name} {Candidate.LastName}?",
                "Question");
            if(result==MessageDialogResult.OK)
            {
                _candidateRepository.Remove(Candidate.Model);
                await _candidateRepository.SaveAsync();

                _eventAggregator.GetEvent<AfterCandidateDeletedEvent>()
                    .Publish(Candidate.Id);
            }          
        }

        private Candidate CreateNewCandidate()
        {
            var candidate = new Candidate();
            _candidateRepository.Add(candidate);
            return candidate;
        }
    }
}
