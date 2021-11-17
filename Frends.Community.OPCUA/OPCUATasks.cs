using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frends.Community.OPCUA
{

    public class ReadTagsOutput
    {
        public Dictionary<string, TagReadResult> TagValues { get; set; }
    }

    public class TagReadResult
    {
        public object Value { get; set; }

        public string Error { get; set; }
    }

    public class OPCUAConnectionProperties
    {
        /// <summary>
        /// Allows untrusted / self-issued certificates
        /// </summary>
        public bool AcceptUntrustedCertificates { get; set; }

        /// <summary>
        /// OPC UA URL
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("opc.tcp://127.0.0.1:49320")]
        public string URL { get; set; }

        /// <summary>
        /// Username to use for connecting. Leave empty for anonymous login
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password to use for connecting. Leave empty for anonymous login
        /// </summary>
        [PasswordPropertyText(true)]
        public string Password { get; set; }
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

        /// <summary>
        /// Read tag values from the OPC UA server.
        /// </summary>
        /// <param name="connectionProperties">Connection properties.</param>
        /// <param name="parameters">Parameters for reading tags values.</param>
        /// <returns>Data of read tags with possible errors included.</returns>
        public static async Task<ReadTagsOutput> ReadTags(OPCUAConnectionProperties connectionProperties, ReadTagsParameters parameters)
        {
            using (var session = await CreateOpcSession(connectionProperties))
            {
                var nodes = parameters.TagsToRead.Select(o => new NodeId(o.NodeId)).ToList();
                var expectedTypes = parameters.TagsToRead.Select(o => o.ExpectedType).ToList();
                var listValues = new List<object>();
                var listErrors = new List<ServiceResult>();
                session.ReadValues(nodes, expectedTypes, out listValues, out listErrors);

                var result = new Dictionary<string, TagReadResult>();
                for (var i = 0; i < parameters.TagsToRead.Length; i++)
                {
                    var tagReadResult = new TagReadResult { Value = listValues[i], Error = listErrors[i]?.ToLongString() };
                    result.Add(parameters.TagsToRead[i].NodeId, tagReadResult);
                }

                return new ReadTagsOutput { TagValues = result };
            }
        }

        private static async Task<Session> CreateOpcSession(OPCUAConnectionProperties connectionProperties)
        {
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(connectionProperties.URL, false);

            //ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);
            var applicationConfiguration = new ApplicationConfiguration
            {
                ClientConfiguration = new ClientConfiguration
                {
                    WellKnownDiscoveryUrls = new[] { "opc.tcp://{0}:4840/UADiscovery" },
                    DiscoveryServers = new EndpointDescriptionCollection()
                },
                CertificateValidator = new CertificateValidator
                {
                    AutoAcceptUntrustedCertificates = connectionProperties.AcceptUntrustedCertificates
                }
            };

            //application.ApplicationConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates = true;
            //application.Server.CertificateValidator.AutoAcceptUntrustedCertificates = true;

            var endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

            var userIdentity = new UserIdentity(new AnonymousIdentityToken());
            if (!string.IsNullOrWhiteSpace(connectionProperties.Username))
            {
                userIdentity = new UserIdentity(connectionProperties.Username, connectionProperties.Password);
            }

            var session = await Session.Create(
                applicationConfiguration,
                endpoint,
                false,
                "FRENDS",
                60000,
                userIdentity,
                null);
            return session;
        }
    }
}
