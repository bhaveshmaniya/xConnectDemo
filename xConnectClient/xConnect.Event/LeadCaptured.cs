using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectClient.Event
{
    public class LeadCaptured : Sitecore.XConnect.Event
    {
        public string ContactInfo { get; set; }

        public LeadCaptured(Guid definitionId, DateTime timestamp)
                : base(definitionId, timestamp)
        { }
    }
}
