using HR.UI.Event;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HR.UI.ViewModel
{  
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMember;
        private IEventAggregator _eventAggregator;

        public NavigationItemViewModel(int id, string displayMember,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Id = id;
            DisplayMember = displayMember;
            OpenCandidateDetailViewCommand = new DelegateCommand(OnOpenCandidateDetailView);
        }
     
        public int Id { get; }

        public string DisplayMember
        {
            get
            {
                return _displayMember;
            }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenCandidateDetailViewCommand { get; }

        private void OnOpenCandidateDetailView()
        {
            _eventAggregator.GetEvent<OpenCandidateDetailViewEvent>()
                        .Publish(Id);
        }
    }
}
