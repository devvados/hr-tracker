using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HR.DataAccess;
using HR.Model;

namespace HR.UI.Data.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        HrDbContext _context;

        public CandidateRepository(HrDbContext context)
        {
            _context = context;
        }

        public void Add(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
        }

        public void Remove(Candidate candidate)
        {
            _context.Candidates.Remove(candidate);
        }

        public IEnumerable<Candidate> GetAll()
        {
            return _context.Candidates.ToList();
        }

        public async Task<Candidate> GetByIdAsync(int candidateId)
        {
            return await _context.Candidates
                .Include(c => c.PhoneNumbers)
                .SingleAsync(c => c.Id == candidateId);

            /* Visual representation of async work
            * 
            * var candidates = await ctx.Candidates.AsNoTracking().ToListAsync();
            * await Task.Delay(5000);
            * return candidates;
            */
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void RemovePhoneNumber(CandidatePhoneNumber model)
        {
            _context.CandidatePhoneNumbers.Remove(model);
        }
    }
}
