using Prism.Events;

namespace HR.UI.Event
{
    public class AfterCandidateSavedEvent : PubSubEvent<AfterCandidateSavedEventArgs>
    {
    }

    public class AfterCandidateSavedEventArgs
    {
        public int Id { get; set; }
        public string DisplayMember { get; set; }
    }
}
