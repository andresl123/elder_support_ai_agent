using ApiIntegration.Models;

namespace ApiIntegration.Services;

using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

public class FastApiService
{
    private readonly HttpClient _httpClient;

    public FastApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<string>> GetFastApiResponseAsync()
    {
        var url = "http://asl.serveblog.net:8000/run_sse";

        var requestPayload = new FastApiRequest
        {
            app_name = "manager",
            user_id = "k8k_id",
            session_id = "42ab7b41-362e-4073-a46c-155367b827b0",
            streaming = false,
            new_message = new NewMessage
            {
                role = "user",
                parts = new List<RequestPart>
                {
                    new RequestPart
                    {
                        text = "The patients health log: Andre is 82 years old. He has medical conditions including Type 2 Diabetes and Hypertension..."
                    }
                }
            }
        };

        var json = JsonConvert.SerializeObject(requestPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var messages = new List<string>();

        // Split response into multiple "data:" sections
        var parts = responseString.Split(new[] { "data: " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in parts)
        {
            var line = raw.Trim().Split('\n')[0]; // grab only the first line of JSON for each block

            try
            {
                var result = JsonConvert.DeserializeObject<FastApiResponse>(line);
                var text = result?.content?.parts?.FirstOrDefault()?.text;
                if (!string.IsNullOrEmpty(text))
                {
                    messages.Add(text);
                }
            }
            catch
            {
                // You can log this if needed
                continue;
            }
        }

        return messages;
    }

}
