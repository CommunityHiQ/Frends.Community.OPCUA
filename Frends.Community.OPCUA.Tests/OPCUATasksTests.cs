using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Frends.Community.OPCUA.Tests
{
    [TestClass]
    public class OPCUATasksTests
    {
        [TestMethod]
        public async Task ReadTags()
        {
            var result = await OPCUATasks.ReadTags(
                new OPCUAConnectionProperties
                {
                    AcceptUntrustedCertificates = true,
                    URL = "opc.tcp://127.0.0.1:49320",
                    Username = "opcguru2",
                    Password = "opcgurupassword"
                },
                new ReadTagsParameters
                {
                    TagsToRead = new []
                    {
                        new TagToRead { NodeId = "ns=3;i=1001", ExpectedType = typeof(double) },
                        new TagToRead { NodeId = "ns=3;i=1002", ExpectedType = typeof(double) },
                        new TagToRead { NodeId = "ns=3;i=1003", ExpectedType = typeof(double) },
                        new TagToRead { NodeId = "ns=3;i=1004", ExpectedType = typeof(double) },
                        new TagToRead { NodeId = "ns=3;i=1005", ExpectedType = typeof(double) },
                        new TagToRead { NodeId = "ns=2;s=Channel1.Device1.Tag1", ExpectedType = typeof(double) },
                    }
                });
            Assert.AreEqual(6, result.TagValues.Count);
        }

        [TestMethod]
        public async Task WriteTag()
        {
            await OPCUATasks.WriteTag(
                new OPCUAConnectionProperties
                {
                    URL = ""
                },
                new WriteTagParameters
                {
                    NodeId = "ns=3;i=1009",
                    Value = (double)69
                });
            //Assert.AreEqual(6, result.TagValues.Count);
        }
    }
}
