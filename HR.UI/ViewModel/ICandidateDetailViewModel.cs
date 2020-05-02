using System.Threading.Tasks;

namespace HR.UI.ViewModel
{
    public interface ICandidateDetailViewModel
    {
        Task LoadAsync(int candidateId);
    }
}