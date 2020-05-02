using HR.Model;
using HR.UI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        Candidate _selectedCandidate;
        private ICandidateDataService _candidateDataService;

        public MainViewModel(ICandidateDataService candidateDataService)
        {
            Candidates = new ObservableCollection<Candidate>();
            _candidateDataService = candidateDataService;
        }

        public void Load()
        {
            var candidates = _candidateDataService.GetAll();
            Candidates.Clear();

            foreach (var candidate in candidates)
            {
                Candidates.Add(candidate);
            }
        }

        public async Task LoadAsync()
        {
            var candidates = await _candidateDataService.GetAllAsync();
            Candidates.Clear();

            foreach (var candidate in candidates)
            {
                Candidates.Add(candidate);
            }
        }

        public ObservableCollection<Candidate> Candidates { get; set; }

        public Candidate SelectedCandidate
        {
            get { return _selectedCandidate; }
            set 
            { 
                _selectedCandidate = value;
                OnPropertyChanged();
            }
        }
    }
}
