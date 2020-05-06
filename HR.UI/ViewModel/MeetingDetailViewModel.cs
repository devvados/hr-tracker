using HR.Model;
using HR.UI.Data.Repositories;
using HR.UI.View.Services;
using HR.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HR.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMeetingRepository _meetingRepository;
        private MeetingWrapper _meeting;
        private IMessageDialogService _messageDialogService;

        private Candidate _selectedAvailableCandidate;
        private Candidate _selectedAddedCandidate;
        private IEnumerable<Candidate> _allCandidates;

        public MeetingDetailViewModel(IEventAggregator eventAggregator, 
            IMessageDialogService messageDialogService,
            IMeetingRepository meetingRepository)
            : base(eventAggregator)
        {
            _meetingRepository = meetingRepository;           
            _messageDialogService = messageDialogService;

            AddedCandidates = new ObservableCollection<Candidate>();
            AvailableCandidates = new ObservableCollection<Candidate>();

            AddCandidateCommand = new DelegateCommand(OnAddCandidateExecute, OnAddCandidateCanExecute);
            RemoveCandidateCommand = new DelegateCommand(OnRemoveCandidateExecute, OnRemoveCandidateCanExecute);
        }

        private bool OnRemoveCandidateCanExecute()
        {
            return SelectedAddedCandidate != null;
        }

        private void OnRemoveCandidateExecute()
        {
            var candidateToRemove = SelectedAddedCandidate;

            Meeting.Model.Candidate = null;
            AddedCandidates.Remove(candidateToRemove);
            AvailableCandidates.Add(candidateToRemove);

            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnAddCandidateCanExecute()
        {
            return SelectedAvailableCandidate != null;
        }

        private void OnAddCandidateExecute()
        {
            var addedCandidate = Meeting.Model.Candidate;
            var candidateToAdd = SelectedAvailableCandidate;

            Meeting.Model.Candidate = candidateToAdd;
            AddedCandidates.Clear();
            AddedCandidates.Add(candidateToAdd);
            AvailableCandidates.Remove(candidateToAdd);
            AvailableCandidates.Add(addedCandidate);

            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public MeetingWrapper Meeting
        {
            get { return _meeting; }
            set
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Candidate> AddedCandidates { get; }

        public ObservableCollection<Candidate> AvailableCandidates { get; }
        
        public ICommand AddCandidateCommand { get; }
        
        public ICommand RemoveCandidateCommand { get; }

        public Candidate SelectedAvailableCandidate
        {
            get { return _selectedAvailableCandidate; }
            set
            {
                _selectedAvailableCandidate = value;
                OnPropertyChanged();
                ((DelegateCommand)AddCandidateCommand).RaiseCanExecuteChanged();
            }
        }

        public Candidate SelectedAddedCandidate
        {
            get { return _selectedAddedCandidate; }
            set
            {
                _selectedAddedCandidate = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCandidateCommand).RaiseCanExecuteChanged();
            }
        }

        public override async Task LoadAsync(int? meetingId)
        {
            var meeting = meetingId.HasValue
                ? await _meetingRepository.GetByIdAsync(meetingId.Value)
                : CreateNewMeeting();

            InitializeMeeting(meeting);

            _allCandidates = await _meetingRepository.GetAllCandidatesAsync();
            SetupPicklist();
        }

        private void SetupPicklist()
        {
            var addedCandidate = Meeting.Model.Candidate;
            var availableCandidates = _allCandidates
                .Except(new List<Candidate> { addedCandidate })
                .OrderBy(c => c.LastName);

            AddedCandidates.Clear();
            AvailableCandidates.Clear();

            AddedCandidates.Add(addedCandidate);

            foreach (var availableCandidate in availableCandidates)
            {
                AvailableCandidates.Add(availableCandidate);
            }
        }

        protected override void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog(
                $"Do you really want to delete the meeting {Meeting.Title}?",
                "Question");
            if (result == MessageDialogResult.OK)
            {
                _meetingRepository.Remove(Meeting.Model);
                _meetingRepository.SaveAsync();
                RaiseDetailDeletedEvent(Meeting.Id);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null 
                && !Meeting.HasErrors 
                && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Title);
        }
        private Meeting CreateNewMeeting()
        {
            var meeting = new Meeting
            {
                Date = DateTime.Now.Date
            };
            _meetingRepository.Add(meeting);
            return meeting;
        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _meetingRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if(Meeting.Id == 0)
            {
                Meeting.Title = "";
            }
        }
    }
}
