using DetectiveAgency.Tests.Models.Requests;
using DetectiveAgency.Tests.Models.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectiveAgency.Tests.Clients
{
    public class AuthClient : BaseApiClient
    {
        public AuthClient(string baseUrl) : base(baseUrl) { }

        public async Task<AuthResponse> LoginAsync(LoginCredentials credentials)
        {
            var request = CreateRequest("/api/auth/login", Method.Post);
            request.AddJsonBody(credentials);

            return await ExecuteAsync<AuthResponse>(request);
        }

        public async Task<bool> LoginWithErrorHandling(LoginCredentials credentials)
        {
            try
            {
                var request = CreateRequest("/api/auth/login", Method.Post);
                request.AddJsonBody(credentials);

                var response = await _client.ExecuteAsync<AuthResponse>(request);
                return response.IsSuccessful;
            }
            catch
            {
                return false;
            }
        }
    }
}
