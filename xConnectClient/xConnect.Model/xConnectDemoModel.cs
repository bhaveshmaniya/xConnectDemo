﻿using Sitecore.XConnect;
using Sitecore.XConnect.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xConnectClient.Event;

namespace xConnectClient.Model
{
    public class xConnectDemoModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            var builder = new XdbModelBuilder("xConnectDemoModel", new XdbModelVersion(1, 0));
            builder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
            builder.DefineFacet<Contact, SalesRegion>(SalesRegion.FacetName);
            builder.DefineEventType<LeadCaptured>(true);

            return builder.BuildModel();
        }
    }
}
