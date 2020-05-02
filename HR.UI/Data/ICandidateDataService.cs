using HR.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.UI.Data
{
    public interface ICandidateDataService
    {
        IEnumerable<Candidate> GetAll();
        Task<List<Candidate>> GetAllAsync();
    }
}