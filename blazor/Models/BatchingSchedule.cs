using System.Globalization;

namespace ClientImplementation.Models;

public enum BatchFrequency
{
    /// <summary>Monday–Friday.</summary>
    Daily,
    Weekly,
    TwiceAWeek,
    /// <summary>One day of the month, 1st–28th, weekends included.</summary>
    Monthly
}

/// <summary>
/// API order batching schedule for standard orders. Rush orders are fixed (daily, Monday–Friday) and not stored here.
/// A new institution has no Frequency ("Not set").
/// The draft keeps the day fields of other frequencies, so switching Weekly ↔ Twice a week keeps the chosen day.
/// Compare with Normalized() to ignore those hidden values.
/// </summary>
public sealed record BatchingSchedule
{
    public BatchFrequency? Frequency { get; init; }

    /// <summary>Weekly: one of Monday–Friday.</summary>
    public DayOfWeek? WeeklyDay { get; init; }

    /// <summary>Twice a week: exactly 2 of Monday–Friday, in the order they were picked.</summary>
    public IReadOnlyList<DayOfWeek> TwiceDays { get; init; } = [];

    /// <summary>Monthly: 1–28.</summary>
    public int? MonthDay { get; init; }

    /// <summary>Only the values the chosen frequency uses. This is what gets saved and compared.</summary>
    public BatchingSchedule Normalized() => Frequency switch
    {
        BatchFrequency.Daily => new() { Frequency = BatchFrequency.Daily },
        BatchFrequency.Weekly => new() { Frequency = BatchFrequency.Weekly, WeeklyDay = WeeklyDay },
        BatchFrequency.TwiceAWeek => new() { Frequency = BatchFrequency.TwiceAWeek, TwiceDays = TwiceDays.Order().ToList() },
        BatchFrequency.Monthly => new() { Frequency = BatchFrequency.Monthly, MonthDay = MonthDay },
        _ => new()
    };

    // Value equality for TwiceDays (records compare lists by reference). Pick order doesn't matter.
    public bool Equals(BatchingSchedule? other) =>
        other is not null
        && Frequency == other.Frequency
        && WeeklyDay == other.WeeklyDay
        && MonthDay == other.MonthDay
        && TwiceDays.Order().SequenceEqual(other.TwiceDays.Order());

    public override int GetHashCode() => HashCode.Combine(Frequency, WeeklyDay, MonthDay, TwiceDays.Count);
}

/// <summary>Schedule text and dates. Pure functions, so they can be unit tested.</summary>
public static class BatchingScheduleText
{
    /// <summary>Monday–Friday, the days a weekly or twice-a-week batch can run.</summary>
    public static readonly IReadOnlyList<DayOfWeek> Weekdays =
        [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];

    public const int LastMonthDay = 28;

    /// <summary>True when the chosen frequency has all the days it needs.</summary>
    public static bool IsComplete(BatchingSchedule s) => s.Frequency switch
    {
        BatchFrequency.Daily => true,
        BatchFrequency.Weekly => s.WeeklyDay is not null,
        BatchFrequency.TwiceAWeek => s.TwiceDays.Count == 2,
        BatchFrequency.Monthly => s.MonthDay is not null,
        _ => false
    };

    /// <summary>"Daily, Monday–Friday", "Weekly on Tuesday", "Twice a week on Monday and Thursday", "Monthly on the 15th", or "Not set".</summary>
    public static string Describe(BatchingSchedule s) => s.Frequency switch
    {
        BatchFrequency.Daily => "Daily, Monday–Friday",
        BatchFrequency.Weekly when s.WeeklyDay is { } day => $"Weekly on {day}",
        BatchFrequency.TwiceAWeek when s.TwiceDays.Count == 2 => "Twice a week on " + string.Join(" and ", s.TwiceDays.Order()),
        BatchFrequency.Monthly when s.MonthDay is { } m => $"Monthly on the {Ordinal(m)}",
        _ => "Not set"
    };

    /// <summary>1st, 2nd, 3rd, 4th … 11th, 12th, 13th … 21st, 22nd.</summary>
    public static string Ordinal(int n)
    {
        var v = n % 100;
        if (v is >= 11 and <= 13) return n + "th";
        return (n % 10) switch
        {
            1 => n + "st",
            2 => n + "nd",
            3 => n + "rd",
            _ => n + "th"
        };
    }

    /// <summary>"Wed, Oct 21".</summary>
    public static string FormatDate(DateOnly date) => date.ToString("ddd, MMM d", CultureInfo.InvariantCulture);

    /// <summary>
    /// The next 3 batch dates after today (today itself is not included). Empty while the schedule is incomplete.
    /// Daily, weekly and twice a week: Monday–Friday only, looking 59 days ahead.
    /// Monthly: that day in this month or the next 5 months, weekends included.
    /// </summary>
    public static IReadOnlyList<DateOnly> NextBatches(BatchingSchedule s, DateOnly today, int count = 3)
    {
        var result = new List<DateOnly>();
        if (!IsComplete(s)) return result;

        if (s.Frequency == BatchFrequency.Monthly)
        {
            var firstOfMonth = new DateOnly(today.Year, today.Month, 1);
            for (var m = 0; result.Count < count && m < 6; m++)
            {
                var month = firstOfMonth.AddMonths(m);
                var date = new DateOnly(month.Year, month.Month, s.MonthDay!.Value);
                if (date > today) result.Add(date);
            }
            return result;
        }

        for (var i = 1; result.Count < count && i < 60; i++)
        {
            var date = today.AddDays(i);
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;

            var runs = s.Frequency switch
            {
                BatchFrequency.Daily => true,
                BatchFrequency.Weekly => s.WeeklyDay == date.DayOfWeek,
                BatchFrequency.TwiceAWeek => s.TwiceDays.Contains(date.DayOfWeek),
                _ => false
            };
            if (runs) result.Add(date);
        }
        return result;
    }
}
