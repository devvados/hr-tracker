using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HR.DataAccess;
using HR.Model;

namespace HR.UI.Data
{
    public class CandidateDataService : ICandidateDataService
    {
        Func<HrDbContext> _contextCreator;

        public CandidateDataService(Func<HrDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public IEnumerable<Candidate> GetAll()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Candidates.AsNoTracking().ToList();
            }    
        }

        public async Task<List<Candidate>> GetAllAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Candidates.AsNoTracking().ToListAsync();

                /* Visual representation of async work
                 * 
                 * var candidates = await ctx.Candidates.AsNoTracking().ToListAsync();
                 * await Task.Delay(5000);
                 * return candidates;
                 */
            }
        }
    }
}
