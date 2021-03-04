using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace Webapp.Tests
{
    public class ExampleIntegrationTest : AbstractTest
    {
        [Test]
        public async Task getLogin_loginPageExists_receiveOK()
        {
            var response = await Host.GetTestClient().GetAsync("/login");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task getLogin_loginPageExists_receivedDocumentContainsLoginHeader()
        {
            var response = await Host.GetTestClient().GetAsync("/login");
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(body.Contains("Login"));
        }
    }
}