using HR.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HR.UI.Wrapper
{
    public class CandidateWrapper : ModelWrapper<Candidate>
    {
        public CandidateWrapper(Candidate model) : base(model) { }

        public int Id {  get { return Model.Id; } }

        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
  
        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Name):
                    if (string.Equals(Name, "Robot", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Robots are not valid!";
                    }
                    break;
                case nameof(LastName):
                    if (string.Equals(Name, "Robot", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Robots are not valid!";
                    }
                    break;
                case nameof(Email):
                    //TODO: implement Email validation logic
                    break;
            }
        }
    }
}
