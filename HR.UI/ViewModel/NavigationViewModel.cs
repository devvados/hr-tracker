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
    class NavigationViewModel : INavigationViewModel
    {
        private ICandidateLookupDataService _candidateLookupService;

        public NavigationViewModel(ICandidateLookupDataService candidateLookupService)
        {
            _candidateLookupService = candidateLookupService;
            Candidates = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync()
        {
            var lookup = await _candidateLookupService.GetCandidateAsync();
            Candidates.Clear();
            foreach (var item in lookup)
            {
                Candidates.Add(item);
            }
        }

        public ObservableCollection<LookupItem> Candidates { get; }
    }
}
