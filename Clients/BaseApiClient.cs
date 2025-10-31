using Allure.Net.Commons;
using DetectiveAgency.Tests.Models.Responses;
using DetectiveAgency.Tests.Utilities;
using RestSharp;

namespace DetectiveAgency.Tests.Clients;

public abstract class BaseApiClient
{
    protected readonly RestClient _client;
    protected readonly string _baseUrl;

    protected BaseApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _client = new RestClient(baseUrl);
    }

    protected async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
    {
        var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value;
        TestLogger.LogRequest(request.Method.ToString(), _baseUrl + request.Resource, body);

        var response = await _client.ExecuteAsync<T>(request);

        TestLogger.LogResponse(response.StatusCode.ToString(), response.Data);

        // Безопасное добавление вложений в Allure
        try
        {
            if (body != null)
            {
                var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(body, Newtonsoft.Json.Formatting.Indented);
                AllureApi.AddAttachment("Request Body", "application/json", requestJson);
            }

            if (response.Data != null)
            {
                var responseJson = Newtonsoft.Json.JsonConvert.SerializeObject(response.Data, Newtonsoft.Json.Formatting.Indented);
                AllureApi.AddAttachment("Response Body", "application/json", responseJson);
            }

            AllureApi.AddAttachment("Status Code", "text/plain", $"{(int)response.StatusCode} {response.StatusCode}");
        }
        catch (Exception ex)
        {
            TestLogger.LogError($"Allure attachment failed: {ex.Message}");
        }

        if (!response.IsSuccessful)
        {
            TestLogger.LogError($"Request failed: {response.StatusCode} - {response.Content}");
            throw new HttpRequestException($"Request failed: {response.StatusCode} - {response.Content}");
        }

        return response.Data ?? new T();
    }

    protected RestRequest CreateRequest(string resource, Method method)
    {
        return new RestRequest(resource, method);
    }
}