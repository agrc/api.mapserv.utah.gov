namespace WebAPI.Dashboard.Models.ViewModels
{
    public abstract class MainViewBase : IErrorMessageDisplayable, IMessageDisplayable
    {
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
    }
}