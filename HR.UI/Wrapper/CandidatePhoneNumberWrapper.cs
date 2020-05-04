using HR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.UI.Wrapper
{
    public class CandidatePhoneNumberWrapper : ModelWrapper<CandidatePhoneNumber>
    {
        public CandidatePhoneNumberWrapper(CandidatePhoneNumber phoneNumber)
            :base(phoneNumber)
        {

        }

        public string Number
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
    }
}
