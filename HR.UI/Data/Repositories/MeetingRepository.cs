using HR.DataAccess;
using HR.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace HR.UI.Data.Repositories
{
    public class MeetingRepository : GenericRepository<Meeting, HrDbContext>, IMeetingRepository
    {
        public MeetingRepository(HrDbContext context) : base(context)
        {
        }

        public async override Task<Meeting> GetByIdAsync(int id)
        {
            return await Context.Meetings
                .Include(m => m.Candidate)
                .SingleAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
        {
            return await Context.Set<Candidate>()
                .ToListAsync();
        }
    }
}
