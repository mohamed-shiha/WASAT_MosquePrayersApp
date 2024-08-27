
public class PrayerTimeService
{

    public PrayerTimeService()
    {
    }

    private bool IsDST(DateTime date)
    {
        var jan = TimeZoneInfo.Local.GetUtcOffset(new DateTime(date.Year, 1, 1));
        var jul = TimeZoneInfo.Local.GetUtcOffset(new DateTime(date.Year, 7, 1));
        return jan != jul;
    }

    private DayInfo CalculateDayInfo(int offsetDay = 0, int offsetHour = 0, int hijriOffset = 0, DateTime? nowDate = null)
    {
        var now = nowDate ?? DateTime.Now;
        now = now.AddHours(offsetHour + offsetDay * 24);
        var start = now.Date;
        var end = start.AddDays(1).AddTicks(-1);
        var hijri = now.AddDays(hijriOffset);
        var dstAdjust = IsDST(now) ? 1 : 0;

        return new DayInfo
        {
            Now = now,
            Month = now.Month,
            Day = now.Day,
            Start = start,
            End = end,
            Hijri = hijri,
            DstAdjust = dstAdjust
        };
    }

    private PrayerTime CalculatePrayer(
        int[] hourMinute,
        int[] hourMinuteNext,
        int index,
        DateTime now,
        string when,
        List<string> jamaahMethods,
        List<List<int>> jamaahOffsets,
        int dstAdjust)
    {
        var time = new DateTime(now.Year, now.Month, now.Day, hourMinute[0], hourMinute[1], 0).AddHours(dstAdjust);
        DateTime jamaahTime;

        var hourOffset = jamaahOffsets[index][0];
        var minuteOffset = jamaahOffsets[index][1];
        var jamaahMethod = string.Empty; //jamaahMethods[index];

        switch (jamaahMethod)
        {
            case "afterthis":
                jamaahTime = time.AddMinutes(hourOffset * 60 + minuteOffset);
                break;
            case "fixed":
                jamaahTime = new DateTime(now.Year, now.Month, now.Day, hourOffset, minuteOffset, 0);
                break;
            case "beforenext":
                var rawJamaahTime = new DateTime(now.Year, now.Month, now.Day, hourMinuteNext[0], hourMinuteNext[1], 0);
                jamaahTime = rawJamaahTime.AddMinutes(-hourOffset * 60 - minuteOffset + dstAdjust * 60);
                break;
            default:
                jamaahTime = time;
                break;
        }

        if (jamaahTime < time)
        {
            time = jamaahTime;
        }

        var names = new[] { "Fajr", "Shurooq", "Dhuhr", "Asr", "Maghrib", "Isha" };
        var name = names[index];
        var hasPassed = now > time;
        var isJamaahPending = now >= time && now <= jamaahTime;

        return new PrayerTime
        {
            Time = time,
            IsJamaahPending = isJamaahPending,
            JamaahTime = jamaahTime,
            Index = index,
            HasPassed = hasPassed,
            Name = name,
            When = when,
            DstAdjust = dstAdjust,
            IsNext = false
        };
    }

    public PrayerTimesResult CalculatePrayers(PrayerSettings settings, bool showJamaah = true, DateTime? nowDate = null)
    {
        // Load timetable from the Data folder
        var timetable = TimeTable.LocalTimeTable;

        var dayInfo = CalculateDayInfo(0, 0, settings.HijriOffset, nowDate);
        var yesterdayInfo = CalculateDayInfo(-1, 0, settings.HijriOffset, nowDate);
        var tomorrowInfo = CalculateDayInfo(1, 0, settings.HijriOffset, nowDate);

        var prayersToday = timetable[dayInfo.Month][dayInfo.Day]
            .Select((t, index) => CalculatePrayer(t, index < 5 ? timetable[dayInfo.Month][dayInfo.Day][index + 1] : new[] { 24, 0 }, index, dayInfo.Now, "today", settings.JamaahMethods, settings.JamaahOffsets, dayInfo.DstAdjust))
            .ToList();

        var prayersYesterday = timetable[yesterdayInfo.Month][yesterdayInfo.Day]
            .Select((t, index) => CalculatePrayer(t, index < 5 ? timetable[dayInfo.Month][dayInfo.Day][index + 1] : new[] { 24, 0 }, index, yesterdayInfo.Now, "yesterday", settings.JamaahMethods, settings.JamaahOffsets, yesterdayInfo.DstAdjust))
            .ToList();

        var prayersTomorrow = timetable[tomorrowInfo.Month][tomorrowInfo.Day]
            .Select((t, index) => CalculatePrayer(t, index < 5 ? timetable[dayInfo.Month][dayInfo.Day][index + 1] : new[] { 24, 0 }, index, tomorrowInfo.Now, "tomorrow", settings.JamaahMethods, settings.JamaahOffsets, tomorrowInfo.DstAdjust))
            .ToList();

        // Calculate previous, current, and next prayers
        PrayerTime previous = null, current = null, next = null;

        foreach (var prayer in prayersToday)
        {
            if (dayInfo.Now >= prayer.Time && dayInfo.Now <= prayer.JamaahTime)
            {
                current = prayer;
            }
            else if (dayInfo.Now < prayer.Time)
            {
                next = prayer;
                break;
            }

            previous = prayer;
        }

        if (previous == null)
        {
            previous = prayersYesterday.LastOrDefault();
        }
        if (current == null)
        {
            current = previous;
            previous = prayersYesterday.LastOrDefault();
        }

        if (next == null)
        {
            next = prayersTomorrow.FirstOrDefault();
        }

        var isAfterIsha = dayInfo.Now > prayersToday.Last().JamaahTime;

        //double percentage = CalcPercentage(showJamaah, dayInfo, current, next);

        return new PrayerTimesResult
        {
            Today = prayersToday,
            Yesterday = prayersYesterday,
            Tomorrow = prayersTomorrow,
            Previous = previous,
            Current = current,
            Next = next,
            Now = dayInfo.Now,
            Hijri = dayInfo.Hijri,
            //Percentage = percentage,
            IsAfterIsha = isAfterIsha,
            IsJamaahPending = current.IsJamaahPending,
            Focus = showJamaah && current.IsJamaahPending ? next : current
        };
    }
}