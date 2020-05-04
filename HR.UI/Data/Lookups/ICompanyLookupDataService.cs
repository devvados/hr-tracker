using HR.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.UI.Data.Lookups
{
    public interface ICompanyLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetCompanyAsync();
    }
}