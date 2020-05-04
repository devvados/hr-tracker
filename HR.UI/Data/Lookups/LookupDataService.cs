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
    public class LookupDataService : ICandidateLookupDataService, IPositionLookupDataService, ICompanyLookupDataService
    {
        private Func<HrDbContext> _contextCreator;

        public LookupDataService(Func<HrDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetCandidateAsync()
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

        public async Task<IEnumerable<LookupItem>> GetPositionAsync()
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

        public async Task<IEnumerable<LookupItem>> GetCompanyAsync()
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
    }
}
