namespace ApiIntegration.Models
{
    public class RequestPart
    {
        public string text { get; set; }
    }

    public class NewMessage
    {
        public string role { get; set; }
        public List<RequestPart> parts { get; set; }
    }

    public class FastApiRequest
    {
        public string app_name { get; set; }
        public string user_id { get; set; }
        public string session_id { get; set; }
        public NewMessage new_message { get; set; }
        public bool streaming { get; set; }
    }
}