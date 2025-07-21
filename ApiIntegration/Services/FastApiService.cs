using ApiIntegration.Models;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ApiIntegration.Services;

public class FastApiService
{
    private readonly HttpClient _httpClient;

    public FastApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreateSessionAsync()
    {
        var url = "http://asl.serveblog.net:8000/apps/manager/users/k8k_id/sessions";

        var requestBody = new
        {
            state = new { additionalProp1 = new { } },
            events = new object[] { }
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseBody);

        return result?.id;
    }

    public async Task<List<string>> GetFastApiResponseAsync(string sessionId)
    {
        var url = "http://asl.serveblog.net:8000/run_sse";

        var requestPayload = new FastApiRequest
        {
            app_name = "manager",
            user_id = "k8k_id",
            session_id = sessionId,
            streaming = false,
            new_message = new NewMessage
            {
                role = "user",
                parts = new List<RequestPart>
                {
                    new RequestPart
                    {
                        text = "The patients health log: Andre is 82 years old. He has medical conditions including Type 2 Diabetes and Hypertension. He takes Metformin 500mg and Lisinopril 10mg. His mobility status indicates that he walks with a cane indoors but had no outdoor activity today. For meals, he had breakfast and dinner, but skipped lunch. Regarding medications, he took his morning dose but missed the evening one. There were no social interactions today—no calls or visits. A note from the evening mentions that he seemed withdrawn during dinner."
                    }
                }
            }
        };

        var json = JsonConvert.SerializeObject(requestPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var messages = new List<string>();

        var parts = responseString.Split(new[] { "data: " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in parts)
        {
            var line = raw.Trim().Split('\n')[0];

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
                continue;
            }
        }

        return messages;
    }
}
