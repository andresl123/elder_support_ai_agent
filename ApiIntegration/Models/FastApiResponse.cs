namespace ApiIntegration.Models;

public class FastApiResponse
{
    public Content content { get; set; }
}

public class Content
{
    public string role { get; set; }
    public List<Part> parts { get; set; }
}