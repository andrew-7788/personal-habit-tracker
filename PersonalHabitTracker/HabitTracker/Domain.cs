namespace HabitTracker;

public class UserAccount
{
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string SecretHash { get; set; } = "";
    public int FailedLoginAttempts { get; set; }
    public bool IsLocked { get; set; }
}

public class Habit
{
    public int HabitId { get; set; }
    public string Username { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; }
}

public class HabitEntry
{
    public int EntryId { get; set; }
    public int HabitId { get; set; }
    public DateTime EntryDate { get; set; }
    public int Value { get; set; }
}

public class HabitSchedule
{
    public int HabitId { get; set; }
    public string FrequencyType { get; set; } = "";
    public int TargetPerPeriod { get; set; }
}

public class Reminder
{
    public int ReminderId { get; set; }
    public int HabitId { get; set; }
    public DateTime TimeOfDay { get; set; }
    public bool IsEnabled { get; set; }
}