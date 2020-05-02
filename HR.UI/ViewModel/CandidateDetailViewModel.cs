using HR.Model;
using HR.UI.Data;
using System.Threading.Tasks;

namespace HR.UI.ViewModel
{
    class CandidateDetailViewModel : ViewModelBase, ICandidateDetailViewModel
    {
        private ICandidateDataService _dataService;

        public CandidateDetailViewModel(ICandidateDataService dataService)
        {
            _dataService = dataService;
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
