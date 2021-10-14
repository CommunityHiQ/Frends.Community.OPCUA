using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frends.Community.OPCUA
{

    public class ReadTagsOutput
    {
        public Dictionary<string, object> TagValues { get; set; }
    }

    public class OPCUAConnectionProperties
    {
        public string URL { get; set; }
    }

    public class ReadTagsParameters
    {
        public TagToRead[] TagsToRead { get; set; }
    }

    public class TagToRead
    {
        public string NodeId { get; set; }

        public Type ExpectedType { get; set; }
    }

    public class WriteTagParameters
    {
        public string NodeId { get; set; }

        public double Value { get; set; }
    }

    public static class OPCUATasks
    {
        public static async Task WriteTag(OPCUAConnectionProperties connectionProperties, WriteTagParameters parameters)
        {
            using (var session = await CreateOpcSession(connectionProperties))
            {
                var writeValueColl = new WriteValueCollection();
                writeValueColl.Add(new WriteValue { NodeId = new NodeId(parameters.NodeId), AttributeId = Attributes.Value, Value = new DataValue(new Variant((double)parameters.Value)) });

                var diagInfoColl = new DiagnosticInfoCollection();
                var statusColl = new StatusCodeCollection();
                var responseHeader = session.Write(
                    null,
                    writeValueColl,
                    out statusColl,
                    out diagInfoColl);
            }
        }

        public static async Task<ReadTagsOutput> ReadTags(OPCUAConnectionProperties connectionProperties, ReadTagsParameters parameters)
        {
            using (var session = await CreateOpcSession(connectionProperties))
            {
                var nodes = parameters.TagsToRead.Select(o => new NodeId(o.NodeId)).ToList();
                var expectedTypes = parameters.TagsToRead.Select(o => o.ExpectedType).ToList();
                var listValues = new List<object>();
                var listErrors = new List<ServiceResult>();
                session.ReadValues(nodes, expectedTypes, out listValues, out listErrors);

                var result = new Dictionary<string, object>();
                for (var i = 0; i < parameters.TagsToRead.Length; i++)
                {
                    result.Add(parameters.TagsToRead[i].NodeId, listValues[i]);
                }

                return new ReadTagsOutput { TagValues = result };
            }
        }

        private static async Task<Session> CreateOpcSession(OPCUAConnectionProperties connectionProperties)
        {
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(connectionProperties.URL, false);

            ApplicationInstance application = new ApplicationInstance
            {
                ApplicationName = "FRENDS",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "FRENDS"
            };

            //ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);
            application.ApplicationConfiguration = new ApplicationConfiguration
            {
                ClientConfiguration = new ClientConfiguration
                {
                    WellKnownDiscoveryUrls = new[] { "opc.tcp://{0}:4840/UADiscovery" },
                    DiscoveryServers = new EndpointDescriptionCollection()
                }
            };

            var endpointConfiguration = EndpointConfiguration.Create(application.ApplicationConfiguration);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            var session = await Session.Create(application.ApplicationConfiguration, endpoint, false, "FRENDS", 60000, new UserIdentity(new AnonymousIdentityToken()), null);
            return session;
        }
    }
}
