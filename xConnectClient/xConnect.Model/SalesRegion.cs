using Sitecore.XConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConnectClient.Model
{
    [FacetKey(FacetName)]
    public class SalesRegion : Facet
    {
        public const string FacetName = "SalesRegion";

        public string Region { get; set; }
    }
}
