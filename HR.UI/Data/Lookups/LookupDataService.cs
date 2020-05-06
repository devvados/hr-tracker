using HR.DataAccess;
using HR.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.UI.Data.Lookups
{
    public class LookupDataService : ICandidateLookupDataService, 
        IPositionLookupDataService, 
        ICompanyLookupDataService,
        IMeetingLookupDataService
    {
        private Func<HrDbContext> _contextCreator;

        public LookupDataService(Func<HrDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetCandidateLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Candidates.AsNoTracking()
                    .Select(c =>
                    new LookupItem
                    {
                        Id = c.Id,
                        DisplayMember = c.Name + " " + c.LastName
                    })
                .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetPositionLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Positions.AsNoTracking()
                    .Select(c =>
                    new LookupItem
                    {
                        Id = c.Id,
                        DisplayMember = c.Name
                    })
                .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetCompanyLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Companies.AsNoTracking()
                    .Select(c =>
                    new LookupItem
                    {
                        Id = c.Id,
                        DisplayMember = c.Name
                    })
                .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetMeetingLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                var items = await ctx.Meetings.AsNoTracking()
                    .Select(m =>
                    new LookupItem
                    {
                        Id = m.Id,
                        DisplayMember = m.Title
                    }).ToListAsync();
                return items;
            }
        }
    }
}
