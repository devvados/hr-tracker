using HR.Model;
using HR.UI.Data;
using HR.UI.Data.Lookups;
using HR.UI.Data.Repositories;
using HR.UI.Event;
using HR.UI.View.Services;
using HR.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HR.UI.ViewModel
{
    class CandidateDetailViewModel : ViewModelBase, ICandidateDetailViewModel
    {
        private ICandidateRepository _candidateRepository;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IPositionLookupDataService _positionLookupDataService;
        private ICompanyLookupDataService _companyLookupDataService;
        private CandidateWrapper _candidate;
        private bool _hasChanges;
        private CandidatePhoneNumberWrapper _selectedPhoneNumber;

        public CandidateDetailViewModel(ICandidateRepository candidateRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPositionLookupDataService positionLookupDataService,
            ICompanyLookupDataService companyLookupDataService)
        {
            _candidateRepository = candidateRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _positionLookupDataService = positionLookupDataService;
            _companyLookupDataService = companyLookupDataService;

            //parameter can be added as modifying method to generic
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute,
                OnRemovePhoneNumberCanExecute);

            Companies = new ObservableCollection<LookupItem>();
            Positions = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<CandidatePhoneNumberWrapper>();
        }

        public async Task LoadAsync(int? candidateId)
        {
            var candidate = candidateId.HasValue
                ? await _candidateRepository.GetByIdAsync(candidateId.Value)
                : CreateNewCandidate();

            InitializeCandidate(candidate);
            InitializeCandidatePhoneNumbers(candidate.PhoneNumbers);

            await LoadCompaniesLookupAsync();
            await LoadPositionsLookupAsync();
        }

        private void InitializeCandidatePhoneNumbers(ICollection<CandidatePhoneNumber> phoneNumbers)
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= CandidatePhoneNumberWrapper_PropertyChanged;
            }
            PhoneNumbers.Clear();
            foreach (var candidatePhoneNumber in phoneNumbers)
            {
                var wrapper = new CandidatePhoneNumberWrapper(candidatePhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += CandidatePhoneNumberWrapper_PropertyChanged;
            }
        }

        private void CandidatePhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!HasChanges)
            {
                HasChanges = _candidateRepository.HasChanges();
            }
            if(e.PropertyName == nameof(CandidatePhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private async Task LoadPositionsLookupAsync()
        {
            Positions.Clear();
            Positions.Add(new NullLookupItem { DisplayMember = " - " });

            var positionLookup = await _positionLookupDataService.GetPositionAsync();
            foreach (var lookupItem in positionLookup)
            {
                Positions.Add(lookupItem);
            }
        }

        private async Task LoadCompaniesLookupAsync()
        {
            Companies.Clear();
            Companies.Add(new NullLookupItem { DisplayMember = " - "});

            var companyLookup = await _companyLookupDataService.GetCompanyAsync();
            foreach (var lookupItem in companyLookup)
            {
                Companies.Add(lookupItem);
            }
        }

        private void InitializeCandidate(Candidate candidate)
        {
            //subscribe save command availability to object state change
            Candidate = new CandidateWrapper(candidate);
            Candidate.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
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

        public CandidatePhoneNumberWrapper SelectedPhoneNumber
        {
            get { return _selectedPhoneNumber; }
            set
            {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
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

        public ICommand AddPhoneNumberCommand { get; }

        public ICommand RemovePhoneNumberCommand { get; }

        public ObservableCollection<LookupItem> Companies { get; }

        public ObservableCollection<LookupItem> Positions { get; }

        public ObservableCollection<CandidatePhoneNumberWrapper> PhoneNumbers { get; }

        private bool OnSaveCanExecute()
        {
            return Candidate != null
                && !Candidate.HasErrors
                && PhoneNumbers.All(pn => !pn.HasErrors)
                && HasChanges;
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

        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }

        private void OnRemovePhoneNumberExecute()
        {
            SelectedPhoneNumber.PropertyChanged -= CandidatePhoneNumberWrapper_PropertyChanged;

            //remove record from database
            _candidateRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);

            //removing model from collections
            PhoneNumbers.Remove(SelectedPhoneNumber);

            SelectedPhoneNumber = null;

            //raise event that changes were made
            HasChanges = _candidateRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPhoneNumberExecute()
        {
            //subscribe to event that changes were made
            var newNumber = new CandidatePhoneNumberWrapper(new CandidatePhoneNumber());
            newNumber.PropertyChanged += CandidatePhoneNumberWrapper_PropertyChanged;

            //adding model to collections
            PhoneNumbers.Add(newNumber);
            Candidate.Model.PhoneNumbers.Add(newNumber.Model);

            //trigger validation
            newNumber.Number = "";
        }
    }
}
