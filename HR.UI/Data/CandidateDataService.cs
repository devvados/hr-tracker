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

        public async Task<Candidate> GetByIdAsync(int candidateId)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Candidates.AsNoTracking().SingleAsync(c => c.Id == candidateId);

                /* Visual representation of async work
                 * 
                 * var candidates = await ctx.Candidates.AsNoTracking().ToListAsync();
                 * await Task.Delay(5000);
                 * return candidates;
                 */
            }
        }

        public async Task SaveAsync(Candidate candidate)
        {
            using(var ctx = _contextCreator())
            {
                ctx.Candidates.Attach(candidate);
                ctx.Entry(candidate).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
            }
        }
    }
}
