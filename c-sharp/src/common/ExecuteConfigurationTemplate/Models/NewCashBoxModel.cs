using System;

namespace ExecuteConfigurationTemplate.Models
{
    public class NewCashBoxModel
    {
        public Guid CashBoxId { get; set; }
        public string AccessToken { get; set; }
        public string Configuration { get; set; }

    }
}
