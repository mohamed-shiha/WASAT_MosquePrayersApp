namespace MudBlazorPages.Data
{
    public class FirebaseConfiguration
    {
        public string ApiKey { get; set; }
        public string AuthDomain { get; set; }
        public string ProjectId { get; set; }
        public string StorageBucket { get; set; }
        public string MessagingSenderId { get; set; }
        public string AppId { get; set; }
        public string MeasurementId { get; set; } // Optional, for Firebase Analytics

        // Constructor to initialize the configuration from a JSON file or other source
        public FirebaseConfiguration(string apiKey, string authDomain, string projectId, string storageBucket, string messagingSenderId, string appId, string measurementId = null)
        {
            ApiKey = apiKey;
            AuthDomain = authDomain;
            ProjectId = projectId;
            StorageBucket = storageBucket;
            MessagingSenderId = messagingSenderId;
            AppId = appId;
            MeasurementId = measurementId;
        }
    }
}
