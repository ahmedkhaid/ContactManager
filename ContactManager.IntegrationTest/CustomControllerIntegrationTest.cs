using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
namespace TestProject2
{
    public class CustomControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private HttpClient _httpClient;
        public CustomControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient=factory.CreateClient();
        }
        [Fact]
        #region index

        public async Task index_returnView()
        {
            //adding the test for the end point point by creatign http client object 
            HttpResponseMessage httpResponse = await _httpClient.GetAsync("Persons/Index");
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
            string responseString = await httpResponse.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument = new HtmlDocument();
             htmlDocument.LoadHtml(responseString);
            var document = htmlDocument.DocumentNode;
            document.QuerySelectorAll("table.Persons").Should().NotBeNull();
        }
        #endregion
        #region Create
        [Fact]
        public async Task Create_ToReturnView()
        {
            //Arrange

            //Act
            HttpResponseMessage response = await _httpClient.GetAsync("/Persons/Index");

            //Assert
            response.IsSuccessStatusCode.Should().BeTrue(); //2xx

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);
            var document = html.DocumentNode;

            document.QuerySelectorAll("input.form-input").Should().NotBeNull();
        }
        #endregion
    }
}
