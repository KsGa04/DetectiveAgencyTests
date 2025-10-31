using RestSharp;
using DetectiveAgency.Tests.Models.Responses;

namespace DetectiveAgency.Tests.Clients;

public class AbilitiesClient : BaseApiClient
{
    public AbilitiesClient(string baseUrl) : base(baseUrl) { }

    public async Task<List<AbilityResponse>> GetAllAbilitiesAsync()
    {
        var request = CreateRequest("/api/abilities", Method.Get);
        return await ExecuteAsync<List<AbilityResponse>>(request);
    }

    public async Task<AbilityResponse> GetAbilityByIdAsync(string id)
    {
        var request = CreateRequest($"/api/abilities/{id}", Method.Get);
        return await ExecuteAsync<AbilityResponse>(request);
    }

    public async Task<AbilityResponse> CreateAbilityAsync(AbilityResponse ability)
    {
        var request = CreateRequest("/api/abilities", Method.Post);
        request.AddJsonBody(ability);
        return await ExecuteAsync<AbilityResponse>(request);
    }

    public async Task<AbilityResponse> UpdateAbilityAsync(string id, object updates)
    {
        var request = CreateRequest($"/api/abilities/{id}", Method.Put);
        request.AddJsonBody(updates);
        return await ExecuteAsync<AbilityResponse>(request);
    }

    public async Task<bool> DeleteAbilityAsync(string id)
    {
        var request = CreateRequest($"/api/abilities/{id}", Method.Delete);
        var response = await _client.ExecuteAsync(request);
        return response.IsSuccessful;
    }
}