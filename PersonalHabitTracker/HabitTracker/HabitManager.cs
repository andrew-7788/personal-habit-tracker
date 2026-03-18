using System;
using System.Collections.Generic;
using System.Linq;

namespace HabitTracker;

public class HabitManager
{
    private readonly DataManager _dataManager;

    public HabitManager(DataManager dataManager)
    {
        _dataManager = dataManager;
    }

    public List<Habit> GetHabitsForUser(string username)
    {
        return _dataManager
            .LoadHabits()
            .Where(h => h.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
            .OrderBy(h => h.HabitId)
            .ToList();
    }

    public List<Habit> GetActiveHabitsForUser(string username)
    {
        return GetHabitsForUser(username)
            .Where(h => h.IsActive)
            .ToList();
    }

    public Habit? GetHabitById(string username, int habitId)
    {
        return _dataManager
            .LoadHabits()
            .FirstOrDefault(h =>
                h.HabitId == habitId &&
                h.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public HabitSchedule? GetScheduleByHabitId(int habitId)
    {
        return _dataManager
            .LoadHabitSchedules()
            .FirstOrDefault(s => s.HabitId == habitId);
    }

    public bool CreateHabit(string username, string name, string frequencyType, int targetPerPeriod, out string message)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            message = "Habit name cannot be blank.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(frequencyType))
        {
            frequencyType = "daily";
        }

        if (targetPerPeriod <= 0)
        {
            message = "Target per period must be greater than zero.";
            return false;
        }

        List<Habit> habits = _dataManager.LoadHabits();
        List<HabitSchedule> schedules = _dataManager.LoadHabitSchedules();

        int newId = _dataManager.GetNextHabitId();

        Habit habit = new()
        {
            HabitId = newId,
            Username = username,
            Name = name.Trim(),
            IsActive = true,
            CreatedOn = DateTime.Now
        };

        HabitSchedule schedule = new()
        {
            HabitId = newId,
            FrequencyType = frequencyType.Trim(),
            TargetPerPeriod = targetPerPeriod
        };

        habits.Add(habit);
        schedules.Add(schedule);

        _dataManager.SaveHabits(habits);
        _dataManager.SaveHabitSchedules(schedules);

        message = "Habit created.";
        return true;
    }

    public bool EditHabit(string username, int habitId, string newName, string newFrequencyType, int newTargetPerPeriod, out string message)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            message = "Habit name cannot be blank.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(newFrequencyType))
        {
            newFrequencyType = "daily";
        }

        if (newTargetPerPeriod <= 0)
        {
            message = "Target per period must be greater than zero.";
            return false;
        }

        List<Habit> habits = _dataManager.LoadHabits();
        List<HabitSchedule> schedules = _dataManager.LoadHabitSchedules();

        Habit? habit = habits.FirstOrDefault(h =>
            h.HabitId == habitId &&
            h.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        HabitSchedule? schedule = schedules.FirstOrDefault(s => s.HabitId == habitId);

        if (habit == null || schedule == null)
        {
            message = "Habit not found.";
            return false;
        }

        habit.Name = newName.Trim();
        schedule.FrequencyType = newFrequencyType.Trim();
        schedule.TargetPerPeriod = newTargetPerPeriod;

        _dataManager.SaveHabits(habits);
        _dataManager.SaveHabitSchedules(schedules);

        message = "Habit updated.";
        return true;
    }

    public bool DeleteHabit(string username, int habitId, out string message)
    {
        List<Habit> habits = _dataManager.LoadHabits();
        List<HabitSchedule> schedules = _dataManager.LoadHabitSchedules();
        List<HabitEntry> entries = _dataManager.LoadHabitEntries();
        List<Reminder> reminders = _dataManager.LoadReminders();

        Habit? habit = habits.FirstOrDefault(h =>
            h.HabitId == habitId &&
            h.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (habit == null)
        {
            message = "Habit not found.";
            return false;
        }

        habits.Remove(habit);
        schedules.RemoveAll(s => s.HabitId == habitId);
        entries.RemoveAll(e => e.HabitId == habitId);
        reminders.RemoveAll(r => r.HabitId == habitId);

        _dataManager.SaveHabits(habits);
        _dataManager.SaveHabitSchedules(schedules);
        _dataManager.SaveHabitEntries(entries);
        _dataManager.SaveReminders(reminders);

        message = "Habit deleted.";
        return true;
    }

    public bool LogHabitEntry(string username, int habitId, int value, out string message)
    {
        if (value <= 0)
        {
            message = "Value must be greater than zero.";
            return false;
        }

        Habit? habit = GetHabitById(username, habitId);

        if (habit == null)
        {
            message = "Habit not found.";
            return false;
        }

        if (!habit.IsActive)
        {
            message = "Habit is inactive.";
            return false;
        }

        List<HabitEntry> entries = _dataManager.LoadHabitEntries();

        HabitEntry entry = new()
        {
            EntryId = _dataManager.GetNextHabitEntryId(),
            HabitId = habitId,
            EntryDate = DateTime.Today,
            Value = value
        };

        entries.Add(entry);
        _dataManager.SaveHabitEntries(entries);

        message = "Habit entry logged.";
        return true;
    }

    public int GetTodayLoggedAmount(int habitId)
    {
        return _dataManager
            .LoadHabitEntries()
            .Where(e => e.HabitId == habitId && e.EntryDate.Date == DateTime.Today)
            .Sum(e => e.Value);
    }

    public int GetTargetPerPeriod(int habitId)
    {
        HabitSchedule? schedule = GetScheduleByHabitId(habitId);
        return schedule?.TargetPerPeriod ?? 0;
    }

    public string GetStatusText(int habitId)
    {
        int logged = GetTodayLoggedAmount(habitId);
        int target = GetTargetPerPeriod(habitId);

        if (target <= 0)
        {
            return "Unknown";
        }

        return logged >= target ? "Complete" : "Pending";
    }

    public List<Habit> GetPendingHabits(string username)
    {
        List<Habit> habits = GetActiveHabitsForUser(username);
        List<Habit> pending = new();

        foreach (Habit habit in habits)
        {
            int logged = GetTodayLoggedAmount(habit.HabitId);
            int target = GetTargetPerPeriod(habit.HabitId);

            if (logged < target)
            {
                pending.Add(habit);
            }
        }

        return pending;
    }

    public List<Reminder> GetRemindersForUser(string username)
    {
        List<Habit> habits = GetHabitsForUser(username);
        HashSet<int> habitIds = habits.Select(h => h.HabitId).ToHashSet();

        return _dataManager
            .LoadReminders()
            .Where(r => habitIds.Contains(r.HabitId))
            .OrderBy(r => r.TimeOfDay.TimeOfDay)
            .ThenBy(r => r.ReminderId)
            .ToList();
    }

    public List<Reminder> GetEnabledRemindersForUser(string username)
    {
        return GetRemindersForUser(username)
            .Where(r => r.IsEnabled)
            .OrderBy(r => r.TimeOfDay.TimeOfDay)
            .ToList();
    }

    public bool CreateReminder(string username, int habitId, DateTime timeOfDay, out string message)
    {
        Habit? habit = GetHabitById(username, habitId);

        if (habit == null)
        {
            message = "Habit not found.";
            return false;
        }

        List<Reminder> reminders = _dataManager.LoadReminders();

        Reminder reminder = new()
        {
            ReminderId = _dataManager.GetNextReminderId(),
            HabitId = habitId,
            TimeOfDay = timeOfDay,
            IsEnabled = true
        };

        reminders.Add(reminder);
        _dataManager.SaveReminders(reminders);

        message = "Reminder created.";
        return true;
    }

    public bool ToggleReminder(string username, int reminderId, out string message)
    {
        List<Habit> habits = GetHabitsForUser(username);
        HashSet<int> habitIds = habits.Select(h => h.HabitId).ToHashSet();

        List<Reminder> reminders = _dataManager.LoadReminders();
        Reminder? reminder = reminders.FirstOrDefault(r =>
            r.ReminderId == reminderId &&
            habitIds.Contains(r.HabitId));

        if (reminder == null)
        {
            message = "Reminder not found.";
            return false;
        }

        reminder.IsEnabled = !reminder.IsEnabled;
        _dataManager.SaveReminders(reminders);

        message = reminder.IsEnabled ? "Reminder enabled." : "Reminder disabled.";
        return true;
    }
}