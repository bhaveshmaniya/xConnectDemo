using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.WebApi;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using Sitecore.Xdb.Common.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xConnectClient.Event;
using xConnectClient.Model;

namespace xConnectClient
{
    // References:
    // - https://doc.sitecore.net/developers/xp/xconnect/xconnect-overview/index.html
    // - https://citizensitecore.com/2017/10/17/introducing-xconnect-for-sitecore-9/
    // - http://sitecoreart.martinrayenglish.com/2017/12/exploring-sitecore-xconnect-working.html
    
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // ==== xConnect Contact Basics ====

                //Console.WriteLine("Adding Contact: Start");
                //AddContact();
                //Console.WriteLine("Contact added successfully!");
                //Console.WriteLine("Adding Contact: End");

                //Console.WriteLine("Search Contacts: Start");
                //SearchContacts();
                //Console.WriteLine("Search Contacts: End");

                //Console.WriteLine("Get Contact[Fixed Identifiers]: Start");
                //GetContact();
                //Console.WriteLine("Get Contact[Fixed Identifiers]: End");

                //Console.WriteLine("Add Interaction: Start");
                //AddInteraction();
                //Console.WriteLine("Add Interaction: End");

                // ==== Adding Custom Facet ====

                Console.WriteLine("Adding Custom Facet: Start");
                AddCustomFacet();
                Console.WriteLine("Adding Custom Facet: End");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        #region xConnect Contact Basics
        // References:
        // - https://sitecore.stackexchange.com/questions/8817/exception-occured-while-initialize-the-xconnect-config
        // - https://doc.sitecore.net/developers/xp/xconnect/xconnect-client-api/xconnect-client-api-overview/build-runtime-model.html
        private static XConnectClient GetClient()
        {
            // Valid certificate thumbprint must be passed in
            CertificateWebRequestHandlerModifierOptions options =
                CertificateWebRequestHandlerModifierOptions.Parse("StoreName=My;StoreLocation=LocalMachine;FindType=FindByThumbprint;FindValue=5137B55085A667B3726F81FA71418E84C528C87B");

            // Optional timeout modifier
            var certificateModifier = new CertificateWebRequestHandlerModifier(options);

            List<IHttpClientModifier> clientModifiers = new List<IHttpClientModifier>();
            var timeoutClientModifier = new TimeoutHttpClientModifier(new TimeSpan(0, 0, 20));
            clientModifiers.Add(timeoutClientModifier);

            // This overload takes three client end points - collection, search, and configuration
            var collectionClient = new CollectionWebApiClient(new Uri("https://xp0.xconnect/odata"), clientModifiers, new[] { certificateModifier });
            var searchClient = new SearchWebApiClient(new Uri("https://xp0.xconnect/odata"), clientModifiers, new[] { certificateModifier });
            var configurationClient = new ConfigurationWebApiClient(new Uri("https://xp0.xconnect/configuration"), clientModifiers, new[] { certificateModifier });

            // ==== Build a run-time model: Start ====
            // ==== Getting Error ====
            //Sitecore.XConnect.Schema.DuplicateXdbModelNameException was unhandled
            //HResult = -2146233088
            //Message = Different models with the same names are referenced('Runtime')
            //ModelName = Runtime
            //Source = Sitecore.XConnect
            //StackTrace:
            //          at Sitecore.XConnect.Schema.XdbModel.AddAndValidateModelAndTypes(XdbModel model, HashSet`1 visited)
            //     at Sitecore.XConnect.Schema.XdbModel.AddAndValidateModelAndTypes(XdbModel model, HashSet`1 visited)
            //     at Sitecore.XConnect.Schema.XdbModel..ctor(String name, XdbModelVersion version, XdbNamedType[] types, XdbFacetDefinition[] facets, XdbModel[] referencedModels)
            //     at Sitecore.XConnect.Schema.XdbRuntimeModel..ctor(XdbModel[] models)
            //     at xConnectClient.Program.GetClient() in D:\Learning\Sample Projects\Sitecore - xConnect\xConnectDemo\xConnectClient\Program.cs:line 52
            //     at xConnectClient.Program.Main(String[] args) in D:\Learning\Sample Projects\Sitecore - xConnect\xConnectDemo\xConnectClient\Program.cs:line 19
            //     at System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)
            //     at System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)
            //     at Microsoft.VisualStudio.HostingProcess.HostProc.RunUsersAssembly()
            //     at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
            //     at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
            //     at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
            //     at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
            //     at System.Threading.ThreadHelper.ThreadStart()
            //InnerException:

            //var modelResolver = new ServiceLayerModelResolver("https://xp0.xconnect/configuration/models");
            //modelResolver.Refresh();
            //var model = new XdbRuntimeModel(modelResolver.KnownModels.ToArray());

            //var config = new XConnectClientConfiguration(
            //                   new XdbRuntimeModel(model),
            //                   collectionClient,
            //                   searchClient,
            //                   configurationClient,
            //                   true);
            // ==== Build a run-time model: End ====

            var config = new XConnectClientConfiguration(
                               new XdbRuntimeModel(CollectionModel.Model),
                               collectionClient,
                               searchClient,
                               configurationClient);

            try
            {
                config.Initialize();
            }
            catch (XdbModelConflictException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return new XConnectClient(config);
        }

        private static void AddContact()
        {
            using (var client = GetClient())
            {
                var identifiers = new ContactIdentifier[]
                {
                    new ContactIdentifier("twitter", "longhorntaco", ContactIdentifierType.Known ),
                    new ContactIdentifier("domain", "longhorn.taco", ContactIdentifierType.Known)
                };
                var contact = new Contact(identifiers);

                var personalInfoFacet = new PersonalInformation
                {
                    FirstName = "Longhorn",
                    LastName = "Taco"
                };
                client.SetFacet<PersonalInformation>(contact, PersonalInformation.DefaultFacetKey, personalInfoFacet);

                var emailFacet = new EmailAddressList(new EmailAddress("longhorn@taco.com", true), "twitter");
                client.SetFacet<EmailAddressList>(contact, EmailAddressList.DefaultFacetKey, emailFacet);

                client.AddContact(contact);
                client.Submit();
            }
        }

        private static async void SearchContacts()
        {
            using (var client = GetClient())
            {
                var queryable = client.Contacts
                    .Where(c => c.Interactions.Any(x => x.StartDateTime > DateTime.UtcNow.AddDays(-30)))
                    .WithExpandOptions(new ContactExpandOptions("Personal"));

                var results = await queryable.ToSearchResults();
                var contacts = await results.Results.Select(x => x.Item).ToList();

                foreach (var contact in contacts)
                {
                    Console.WriteLine($"{contact.Personal().FirstName} {contact.Personal().LastName}");
                }
            }
        }

        private static void GetContact()
        {
            using (var client = GetClient())
            {
                var contactReference = new IdentifiedContactReference("twitter", "longhorntaco");
                var contact = client.Get(contactReference, new ExpandOptions() { FacetKeys = { "Personal" } });
                if (contact != null)
                {
                    Console.WriteLine($"{contact.Personal().FirstName} {contact.Personal().LastName}");
                }
            }
        }

        private static void AddInteraction()
        {
            using (var client = GetClient())
            {
                var contactReference = new IdentifiedContactReference("twitter", "longhorntaco");
                var contact = client.Get(contactReference, new ExpandOptions() { FacetKeys = { "Personal" } });
                if (contact != null)
                {
                    // Item ID of the "Enter Store" Offline Channel at 
                    // /sitecore/system/Marketing Control Panel/Taxonomies/Channel/Offline/Store/Enter store
                    var enterStoreChannelId = Guid.Parse("{3FC61BB8-0D9F-48C7-9BBD-D739DCBBE032}");
                    var userAgent = "xConnectDemo Console App";

                    var interaction = new Interaction(contact, InteractionInitiator.Contact, enterStoreChannelId, userAgent);

                    // Item ID of the "Product Purchase" outcome at
                    // /sitecore/system/Marketing Control Panel/Outcomes/Product Purchase
                    var productPurchaseOutcomeId = Guid.Parse("{9016E456-95CB-42E9-AD58-997D6D77AE83}");
                    var outcome = new Outcome(productPurchaseOutcomeId, DateTime.UtcNow, "USD", 42.99m);

                    interaction.Events.Add(outcome);

                    client.AddInteraction(interaction);
                    client.Submit();
                }
            }
        } 
        #endregion

        private static void AddCustomFacet()
        {
            var model = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(xConnectDemoModel.Model);
            File.WriteAllText("c:\\Temp\\" + xConnectDemoModel.Model.FullName + ".json", model);
        }
    }
}
