using AutoMapper;

namespace WebAPI.Dashboard.Models.ViewModels.Keys
{
    public class ApiKeyViewModel
    {
        public string Id { get; set; }
        public string ApiKey { get; set; }
        public string LastUsed { get; set; }
        public string Type { get; set; }
        public int UsageCount { get; set; }
        public string AccountId { get; set; }
        public string Pattern { get; set; }
        public bool Active { get; set; }
        public bool Development { get; set; }

        public string ButtonCssClass
        {
            get { return Active ? "btn-warning" : "btn-success"; }
        }

        public string StutusIconCssClass
        {
            get { return Active ? "glyphicon glyphicon-pause" : "glyphicon glyphicon-play"; }
        }

        public string StatusIconText
        {
            get { return Active ? "Deactivate" : "Activate"; }
        }

        public string TableRowCssClass
        {
            get { return Active ? "active" : "inactive"; }
        }

        public string UseTypeIconCssClass
        {
            get { return Development ? "label label-default" : ""; }
        }

        public string UseType
        {
            get { return Development ? "dev" : ""; }
        }

        public string[] ToArray()
        {
            return Mapper.Map<ApiKeyViewModel, string[]>(this);
        }
    }
}