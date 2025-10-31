using DetectiveAgency.Tests.Models.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectiveAgency.Tests.Clients
{
    public class DetectivesClient : BaseApiClient
    {
        public DetectivesClient(string baseUrl) : base(baseUrl) { }

        public async Task<List<DetectiveResponse>> GetAllDetectivesAsync()
        {
            var request = CreateRequest("/api/detectives", Method.Get);
            return await ExecuteAsync<List<DetectiveResponse>>(request);
        }

        public async Task<DetectiveResponse> GetDetectiveByIdAsync(string id)
        {
            var request = CreateRequest($"/api/detectives/{id}", Method.Get);
            return await ExecuteAsync<DetectiveResponse>(request);
        }

        public async Task<DetectiveResponse> CreateDetectiveAsync(DetectiveResponse detective)
        {
            var request = CreateRequest("/api/detectives", Method.Post);
            request.AddJsonBody(detective);
            return await ExecuteAsync<DetectiveResponse>(request);
        }

        public async Task<DetectiveResponse> UpdateDetectiveAsync(string id, object updates)
        {
            var request = CreateRequest($"/api/detectives/{id}", Method.Put);
            request.AddJsonBody(updates);
            return await ExecuteAsync<DetectiveResponse>(request);
        }

        public async Task<bool> DeleteDetectiveAsync(string id)
        {
            var request = CreateRequest($"/api/detectives/{id}", Method.Delete);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
    }
}
