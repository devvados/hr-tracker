using HR.Model;
using System.Collections.Generic;

namespace HR.UI.Data.Repositories
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    { 
        void RemovePhoneNumber(CandidatePhoneNumber model);
    }
}