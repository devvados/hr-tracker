using HR.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.UI.Data
{
    public interface ICandidateDataService
    {
        Task<Candidate> GetByIdAsync(int candidateId);
        Task SaveAsync(Candidate candidate);
    }
}