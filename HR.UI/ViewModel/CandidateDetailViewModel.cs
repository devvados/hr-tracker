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
    class CandidateDetailViewModel : DetailViewModelBase, ICandidateDetailViewModel
    {
        private ICandidateRepository _candidateRepository;
        private IMessageDialogService _messageDialogService;
        private IPositionLookupDataService _positionLookupDataService;
        private ICompanyLookupDataService _companyLookupDataService;
        private CandidateWrapper _candidate;
        private bool _hasChanges;
        private CandidatePhoneNumberWrapper _selectedPhoneNumber;
        private MeetingWrapper _selectedMeeting;

        public CandidateDetailViewModel(ICandidateRepository candidateRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IPositionLookupDataService positionLookupDataService,
            ICompanyLookupDataService companyLookupDataService)
            : base(eventAggregator)
        {
            _candidateRepository = candidateRepository;
            _messageDialogService = messageDialogService;
            _positionLookupDataService = positionLookupDataService;
            _companyLookupDataService = companyLookupDataService;

            //parameter can be added as modifying method to generic
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute,
                OnRemovePhoneNumberCanExecute);
            AddMeetingCommand = new DelegateCommand(OnAddMeetingExecute);
            RemoveMeetingCommand = new DelegateCommand(OnRemoveMeetingExecute,
                OnRemoveMeetingCanExecute);


            Companies = new ObservableCollection<LookupItem>();
            Positions = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<CandidatePhoneNumberWrapper>();
            Meetings = new ObservableCollection<MeetingWrapper>();
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

        public MeetingWrapper SelectedMeeting
        {
            get { return _selectedMeeting; }
            set
            {
                _selectedMeeting = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveMeetingCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddPhoneNumberCommand { get; }

        public ICommand RemovePhoneNumberCommand { get; }

        public ICommand AddMeetingCommand { get; }

        public ICommand RemoveMeetingCommand { get; }

        public ObservableCollection<LookupItem> Companies { get; }

        public ObservableCollection<LookupItem> Positions { get; }

        public ObservableCollection<CandidatePhoneNumberWrapper> PhoneNumbers { get; }

        public ObservableCollection<MeetingWrapper> Meetings { get; }

        public override async Task LoadAsync(int? candidateId)
        {
            var candidate = candidateId.HasValue
                ? await _candidateRepository.GetByIdAsync(candidateId.Value)
                : CreateNewCandidate();

            InitializeCandidate(candidate);
            InitializeCandidatePhoneNumbers(candidate.PhoneNumbers);
            InitializeMeetings(candidate.Meetings);

            await LoadCompaniesLookupAsync();
            await LoadPositionsLookupAsync();
        }
        
        protected override bool OnSaveCanExecute()
        {
            return Candidate != null
                && !Candidate.HasErrors
                && PhoneNumbers.All(pn => !pn.HasErrors)
                && Meetings.All(m => !m.HasErrors)
                && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await _candidateRepository.SaveAsync();

            HasChanges = _candidateRepository.HasChanges();
            RaiseDetailSavedEvent(Candidate.Id, $"{Candidate.Name} {Candidate.LastName}");
        }

        protected override async void OnDeleteExecute()
        {
            if(await _candidateRepository.HasMeetingsAsync(Candidate.Id))
            {
                _messageDialogService.ShowInfoDialog(
                    $"{Candidate.Name} {Candidate.LastName} can't be deleted, " +
                    $"as this candidate is a part of at least one meeting!");
                return;
            }
            var result = _messageDialogService.ShowOkCancelDialog(
                $"Do you really want to delete candidate {Candidate.Name} {Candidate.LastName}?",
                "Question");
            if (result == MessageDialogResult.OK)
            {
                _candidateRepository.Remove(Candidate.Model);
                await _candidateRepository.SaveAsync();

                RaiseDetailDeletedEvent(Candidate.Id);
            }
        }

        private void InitializeMeetings(ICollection<Meeting> meetings)
        {
            foreach (var wrapper in Meetings)
            {
                wrapper.PropertyChanged -= CandidateMeetingWrapper_PropertyChanged;
            }
            Meetings.Clear();
            foreach (var candidateMeeting in meetings)
            {
                var wrapper = new MeetingWrapper(candidateMeeting);
                Meetings.Add(wrapper);
                wrapper.PropertyChanged += CandidateMeetingWrapper_PropertyChanged;
            }
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
        
        private void CandidateMeetingWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _candidateRepository.HasChanges();
            }
            if (e.PropertyName == nameof(MeetingWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private async Task LoadPositionsLookupAsync()
        {
            Positions.Clear();
            Positions.Add(new NullLookupItem { DisplayMember = " - " });

            var positionLookup = await _positionLookupDataService.GetPositionLookupAsync();
            foreach (var lookupItem in positionLookup)
            {
                Positions.Add(lookupItem);
            }
        }

        private async Task LoadCompaniesLookupAsync()
        {
            Companies.Clear();
            Companies.Add(new NullLookupItem { DisplayMember = " - "});

            var companyLookup = await _companyLookupDataService.GetCompanyLookupAsync();
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

        private bool OnRemoveMeetingCanExecute()
        {
            return SelectedMeeting != null;
        }

        private void OnRemoveMeetingExecute()
        {
            SelectedMeeting.PropertyChanged -= CandidateMeetingWrapper_PropertyChanged;

            //remove record from database
            _candidateRepository.RemoveMeeting(SelectedMeeting.Model);

            //removing model from collections
            Meetings.Remove(SelectedMeeting);

            SelectedMeeting = null;

            //raise event that changes were made
            HasChanges = _candidateRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddMeetingExecute()
        {
            //subscribe to event that changes were made
            var newMeeting = new MeetingWrapper(new Meeting());
            newMeeting.PropertyChanged += CandidateMeetingWrapper_PropertyChanged;

            //adding model to collections
            Meetings.Add(newMeeting);
            Candidate.Model.Meetings.Add(newMeeting.Model);

            //trigger validation
            newMeeting.Title = "";
        }
    }
}
