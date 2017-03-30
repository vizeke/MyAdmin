namespace MyAdmin.Application.Models
{
    public class AuditoriaModel
    {
        public string user { get; set; }
        public int userId { get; set; }
        public string ip { get; set; }
        public string url { get; set; }
        public string className { get; set; }
        public string methodName { get; set; }
        public string jsonParameters { get; set; }
        public string jsonObject { get; set; }
        public string obs { get; set; }
    }
}
