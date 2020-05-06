namespace HR.UI.View.Services
{
    public interface IMessageDialogService
    {
        void ShowInfoDialog(string text);
        MessageDialogResult ShowOkCancelDialog(string text, string title);
    }
}