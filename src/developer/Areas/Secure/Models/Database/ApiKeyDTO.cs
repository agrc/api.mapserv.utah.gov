using System;
namespace developer.mapserv.utah.gov.Areas.Secure.Models.Database {
    public class ApiKeyDTO {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool Elevated { get; set; }
        public bool Enabled { get; set; }
        public string Type { get; set; }
        public string Configuration { get; set; }
        public long CreatedAtTicks { get; set; }
        public string Pattern { get; set; }
        public string Notes { get; set; }
        public int Usage { get; set; }
        public long LastUsed { get; set; }
    }
}
