using Newtonsoft.Json;

namespace BlazorMosque.Client.Services
{
 // Ensure you have the Newtonsoft.Json package installed

    public class PrayerTimeWithIqamah
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Adhan")]
        public string Adhan { get; set; }

        [JsonProperty("Iqamah")]
        public string Iqamah { get; set; }

        [JsonProperty("Offset")]
        public string Offset { get; set; }
    }

    public class IqamahTimeList
    {
        public List<PrayerTimeWithIqamah> PrayerTimes { get; set; } = new List<PrayerTimeWithIqamah>();
    }
}
