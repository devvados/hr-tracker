using HR.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.UI.Data.Repositories
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    { 
        void RemovePhoneNumber(CandidatePhoneNumber model);
        void RemoveMeeting(Meeting model);
        Task<bool> HasMeetingsAsync(int candidateId);
    }
}