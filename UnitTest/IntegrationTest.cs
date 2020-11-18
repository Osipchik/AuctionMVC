using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Web;
using Xunit;

namespace UnitTest
{
    public class IntegrationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public IntegrationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        
        // [Theory]
        // [InlineData("/")]
        // [InlineData("/Home/LoadLots/?search=&Category=&Show=2&SortBy=Date&take=20&skip=5")]
        [Fact]
        public async Task Index()
        {
            var response = await _client.GetAsync("/");
            
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            
            Assert.Equal("text/html; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }
        
        [Fact]
        public async Task LoadLotsDefault()
        {
            var url = "/Home/LoadLots/?search=&categoryId=&sortBy=0&ShowOptions=1&take=20&skip";
            var response = await _client.GetAsync(url);
            
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("test_lot_title: false - 6", responseString);
            Assert.Contains("test_lot_title: false - 9", responseString);
        }
    }
}