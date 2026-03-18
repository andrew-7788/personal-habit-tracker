using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HabitTracker;

public class Reporter
{
    private readonly DataManager _dataManager;

    public Reporter(DataManager dataManager)
    {
        _dataManager = dataManager;
    }

    public List<HabitEntry> GetHabitLog(string username)
    {
        List<Habit> habits = _dataManager.LoadHabits()
            .Where(h => h.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            .ToList();

        HashSet<int> habitIds = habits.Select(h => h.HabitId).ToHashSet();

        return _dataManager.LoadHabitEntries()
            .Where(e => habitIds.Contains(e.HabitId))
            .OrderByDescending(e => e.EntryDate)
            .ThenByDescending(e => e.EntryId)
            .ToList();
    }

    public string BuildProgressSummary(string username)
    {
        List<Habit> habits = _dataManager.LoadHabits()
            .Where(h => h.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            .OrderBy(h => h.Name)
            .ToList();

        List<HabitSchedule> schedules = _dataManager.LoadHabitSchedules();
        List<HabitEntry> entries = _dataManager.LoadHabitEntries();

        StringBuilder sb = new();

        sb.AppendLine("View Progress");
        sb.AppendLine("Habit\t\tDaily\t\tWeekly\t\tMonthly");

        foreach (Habit habit in habits)
        {
            HabitSchedule? schedule = schedules.FirstOrDefault(s => s.HabitId == habit.HabitId);

            if (schedule == null)
            {
                continue;
            }

            string daily = GetDailyProgressText(habit.HabitId, schedule.TargetPerPeriod, entries);
            string weekly = GetWeeklyProgressText(habit.HabitId, schedule.TargetPerPeriod, entries);
            string monthly = GetMonthlyProgressText(habit.HabitId, schedule.TargetPerPeriod, entries);

            sb.AppendLine($"{habit.Name}\t\t{daily}\t\t{weekly}\t\t{monthly}");
        }

        return sb.ToString();
    }

    public string BuildHabitLogReport(string username)
    {
        List<Habit> habits = _dataManager.LoadHabits()
            .Where(h => h.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Dictionary<int, string> habitNames = habits.ToDictionary(h => h.HabitId, h => h.Name);

        List<HabitEntry> entries = GetHabitLog(username);

        StringBuilder sb = new();

        sb.AppendLine("Habit Log");
        sb.AppendLine("Date\t\tHabit\t\tValue");

        foreach (HabitEntry entry in entries)
        {
            string habitName = habitNames.ContainsKey(entry.HabitId)
                ? habitNames[entry.HabitId]
                : $"Habit {entry.HabitId}";

            sb.AppendLine($"{entry.EntryDate:yyyy-MM-dd}\t{habitName}\t\t{entry.Value}");
        }

        return sb.ToString();
    }

    private string GetDailyProgressText(int habitId, int targetPerPeriod, List<HabitEntry> entries)
    {
        int total = entries
            .Where(e => e.HabitId == habitId && e.EntryDate.Date == DateTime.Today)
            .Sum(e => e.Value);

        int percent = CalculatePercent(total, targetPerPeriod);

        return $"{total}/{targetPerPeriod} ({percent}%)";
    }

    private string GetWeeklyProgressText(int habitId, int targetPerPeriod, List<HabitEntry> entries)
    {
        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        DateTime endOfWeek = startOfWeek.AddDays(7);

        int total = entries
            .Where(e => e.HabitId == habitId && e.EntryDate.Date >= startOfWeek && e.EntryDate.Date < endOfWeek)
            .Sum(e => e.Value);

        int weeklyTarget = targetPerPeriod * 7;
        int percent = CalculatePercent(total, weeklyTarget);

        return $"{total}/{weeklyTarget} ({percent}%)";
    }

    private string GetMonthlyProgressText(int habitId, int targetPerPeriod, List<HabitEntry> entries)
    {
        DateTime startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime startOfNextMonth = startOfMonth.AddMonths(1);
        int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);

        int total = entries
            .Where(e => e.HabitId == habitId && e.EntryDate.Date >= startOfMonth && e.EntryDate.Date < startOfNextMonth)
            .Sum(e => e.Value);

        int monthlyTarget = targetPerPeriod * daysInMonth;
        int percent = CalculatePercent(total, monthlyTarget);

        return $"{total}/{monthlyTarget} ({percent}%)";
    }

    private int CalculatePercent(int actual, int target)
    {
        if (target <= 0)
        {
            return 0;
        }

        return (int)Math.Round((double)actual / target * 100);
    }
}