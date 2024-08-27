public class PrayerTime
{
    public DateTime Time { get; set; }
    public bool IsJamaahPending { get; set; }
    public DateTime JamaahTime { get; set; }
    public int Index { get; set; }
    public bool HasPassed { get; set; }
    public string Name { get; set; }
    public string When { get; set; }
    public int DstAdjust { get; set; }
    public bool IsNext { get; set; }

    public string Get24AdhanTime()
    {
        return Time.ToString("HH:mm");
    }

    public string Get24IgamaTime()
    {
        return JamaahTime.ToString("HH:mm");
    }
}

public class DayInfo
{
    public DateTime Now { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime Hijri { get; set; }
    public int DstAdjust { get; set; }
}

public class PrayerTimesResult
{
    public List<PrayerTime> Today { get; set; }
    public List<PrayerTime> Yesterday { get; set; }
    public List<PrayerTime> Tomorrow { get; set; }
    public PrayerTime Previous { get; set; }
    public PrayerTime Current { get; set; }
    public PrayerTime Next { get; set; }
    public DateTime Now { get; set; }
    public DateTime Hijri { get; set; }
    public bool IsAfterIsha { get; set; }
    public bool IsJamaahPending { get; set; }
    public PrayerTime Focus { get; set; }
    //public double Percentage { get; set; }

    public double CalcPercentage(bool showJamaah = true, DateTime? now = null)
    {
        if(now == null)
        {
            now = Now;
        }

        var countUp = new
        {
            Name = Current.IsJamaahPending || !showJamaah ? Current.Name : $"{Current.Name} jamaah",
            Time = Current.IsJamaahPending || !showJamaah ? Current.Time : Current.JamaahTime,
            Duration = (Current.IsJamaahPending || !showJamaah ? now - Current.Time : now - Current.JamaahTime).Value.TotalSeconds
        };

        var countDown = new
        {
            Name = Current.IsJamaahPending && showJamaah ? $"{Current.Name} jamaah" : Next.Name,
            Time = Current.IsJamaahPending && showJamaah ? Current.JamaahTime : Next.Time,
            Duration = (Current.IsJamaahPending && showJamaah ? Current.JamaahTime - now : Next.Time - now).Value.TotalSeconds + 1
        };

        var totalDuration = countUp.Duration + countDown.Duration;

        var percentage = Math.Floor((10000 - (countDown.Duration / totalDuration) * 10000) / 100);
        return percentage;
    }

}

public class PrayerSettings
{
    public int HijriOffset { get; set; }
    public List<string> JamaahMethods { get; set; }
    public List<List<int>> JamaahOffsets { get; set; }
}