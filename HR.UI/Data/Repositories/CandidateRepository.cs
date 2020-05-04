using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HR.DataAccess;
using HR.Model;

namespace HR.UI.Data.Repositories
{
    public class CandidateRepository : GenericRepository<Candidate, HrDbContext>, ICandidateRepository
    {
        public CandidateRepository(HrDbContext context) : base(context) { }

        public override async Task<Candidate> GetByIdAsync(int candidateId)
        {
            return await Context.Candidates
                .Include(c => c.PhoneNumbers)
                .SingleAsync(c => c.Id == candidateId);

            /* Visual representation of async work
            * 
            * var candidates = await ctx.Candidates.AsNoTracking().ToListAsync();
            * await Task.Delay(5000);
            * return candidates;
            */
        }

        public void RemovePhoneNumber(CandidatePhoneNumber model)
        {
            Context.CandidatePhoneNumbers.Remove(model);
        }
    }
}
