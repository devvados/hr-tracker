using HR.UI.Event;
using HR.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HR.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ICandidateDetailViewModel _candidateDetailViewModel;
        private IEventAggregator _eventAggregator;
        private Func<ICandidateDetailViewModel> _candidateDetailViewModelCreator;
        private IMessageDialogService _messageDialogService;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<ICandidateDetailViewModel> candidateDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _eventAggregator = eventAggregator;
            _candidateDetailViewModelCreator = candidateDetailViewModelCreator;
            _messageDialogService = messageDialogService;

            _eventAggregator.GetEvent<OpenCandidateDetailViewEvent>()
                .Subscribe(OnOpenCandidateDetailView);
            _eventAggregator.GetEvent<AfterCandidateDeletedEvent>()
                .Subscribe(AfterCandidateDeleted);

            CreateNewCandidateCommand = new DelegateCommand(OnCreateNewCandidateExecute);

            NavigationViewModel = navigationViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewCandidateCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public ICandidateDetailViewModel CandidateDetailViewModel
        {
            get { return _candidateDetailViewModel; }
            private set 
            { 
                _candidateDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        private async void OnOpenCandidateDetailView(int? candidateId)
        {
            if(CandidateDetailViewModel!=null && CandidateDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You have made changes! Navigate away?", "Question");
                if(result  == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            CandidateDetailViewModel = _candidateDetailViewModelCreator();
            await CandidateDetailViewModel.LoadAsync(candidateId);
        }

        private void OnCreateNewCandidateExecute()
        {
            OnOpenCandidateDetailView(null);
        }

        private void AfterCandidateDeleted(int candidateId)
        {
            CandidateDetailViewModel = null;
        }
    }
}
