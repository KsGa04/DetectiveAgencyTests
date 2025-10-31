using RestSharp;
using DetectiveAgency.Tests.Models.Responses;

namespace DetectiveAgency.Tests.Clients;

public class CasesClient : BaseApiClient
{
    public CasesClient(string baseUrl) : base(baseUrl) { }

    public async Task<List<CaseResponse>> GetAllCasesAsync()
    {
        var request = CreateRequest("/api/cases", Method.Get);
        return await ExecuteAsync<List<CaseResponse>>(request);
    }

    public async Task<CaseResponse> GetCaseByIdAsync(string id)
    {
        var request = CreateRequest($"/api/cases/{id}", Method.Get);
        return await ExecuteAsync<CaseResponse>(request);
    }

    public async Task<CaseResponse> CreateCaseAsync(CaseResponse caseEntity)
    {
        var request = CreateRequest("/api/cases", Method.Post);
        request.AddJsonBody(caseEntity);
        return await ExecuteAsync<CaseResponse>(request);
    }

    public async Task<CaseResponse> UpdateCaseAsync(string id, object updates)
    {
        var request = CreateRequest($"/api/cases/{id}", Method.Put);
        request.AddJsonBody(updates);
        return await ExecuteAsync<CaseResponse>(request);
    }

    public async Task<bool> DeleteCaseAsync(string id)
    {
        var request = CreateRequest($"/api/cases/{id}", Method.Delete);
        var response = await _client.ExecuteAsync(request);
        return response.IsSuccessful;
    }
}