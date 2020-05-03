using HR.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.UI.Data.Repositories
{
    public interface ICandidateRepository
    {
        Task<Candidate> GetByIdAsync(int candidateId);
        Task SaveAsync();
        bool HasChanges();
        void Add(Candidate candidate);
        void Remove(Candidate candidate);
    }
}