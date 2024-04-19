using Books.Service.Internal.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http.Headers;
using Books.Service.Internal.Api.Infrastructure;
using System.Net;
using Moq;
using Moq.Protected;
using AutoMapper;
using Books.Service.Internal.Api.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Books.Service.Api.Tests
{
    [TestFixture]
	public class InternalBooksTests
	{
        public static HttpClient _httpClient;
        public static string _jwtKey;
        public static string _token;

        [SetUp]
        public void Setup()
        {
            var applicationFactory = new TestWebApplicationFactory();
            _httpClient = applicationFactory.CreateClient();

            _jwtKey = ConfigurationHelper.Config["JWTSetting:securitykey"];
            _token = new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    "Sample_Auth_Server",
                    "Sample_Auth_Server",
                    new List<Claim> { new(ClaimTypes.Role, "Operator") },
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256)));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        [Test]
        public async Task Test_Success_Internal_BooksApi_Call()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Internal/books/prod");
            request.Headers.Add("X-Session-ID", "1");
            request.Headers.Add("X-User-ID", "test");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseBooksData = JsonConvert.DeserializeObject<PaginatedList<BookInfoModel>>(responseContent);
            Assert.IsNotNull(responseBooksData);
            Assert.That(responseBooksData.Items.Count==10);
        }

        [TestCase(5)]
        [TestCase(9)]
        public async Task Test_Success_Internal_BooksApi_With_Page_Size(int pageSize)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Internal/books/prod?pageSize={pageSize}");
            request.Headers.Add("X-Session-ID", "1");
            request.Headers.Add("X-User-ID", "test");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseBooksData = JsonConvert.DeserializeObject<PaginatedList<BookInfoModel>>(responseContent);
            Assert.IsNotNull(responseBooksData);
            Assert.That(responseBooksData.Items.Count == pageSize);
        }
        [Test]
        public async Task Test_Error_Invalid_BooksApi_Call()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Invalid/books/prod");
            request.Headers.Add("X-Session-ID", "1");
            request.Headers.Add("X-User-ID", "test");

            var response = await _httpClient.SendAsync(request);

            Assert.That(response.StatusCode == HttpStatusCode.NotFound);
        }


        [Test]
        public async Task Test_Internal_Error_Case_BooksApi_Call()
        {
            var responsemessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
            var mockHttpClient = GetMockedHttpClient(responsemessage);
            var mockLogger = new Mock<ILogger<BookDataService>>();

            var application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                ServiceDescriptor serviceDescriptor = new(typeof(IBookDataService),
                    typeof(BookDataService), ServiceLifetime.Transient);
                services.Remove(serviceDescriptor);
                services.AddScoped<IBookDataService>(s => new BookDataService(mockHttpClient, new Mock<ICacheService>().Object, new Mock<IMapper>().Object,
                    new Mock<IOptions<APISetting>>().Object, mockLogger.Object));
                });
            });

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Internal/books/prod");
            request.Headers.Add("X-Session-ID", "1");
            request.Headers.Add("X-User-ID", "test");

            var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.SendAsync(request);
            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();
            }
            catch(Exception ex)
            {

            }

            Assert.That(response.StatusCode == HttpStatusCode.InternalServerError);
            mockLogger.Verify(x => x.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t)=> v.ToString()!.Contains("Internal Error in Service")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            , Times.Once);
        }


        private static HttpClient GetMockedHttpClient(HttpResponseMessage httpResponseMessage)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpMessageHandlerMock.Protected().
                Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            return new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:9090/")
            };
        }
    }
}

