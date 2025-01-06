using System.Net;
using ProblemSolver.Logic.DlServices.Interfaces;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class DlClientFactory : IDlClientFactory
    {
        public HttpClient CreateClient()
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://dl.gsu.by/")
            };

            return client;
        }
    }
}
