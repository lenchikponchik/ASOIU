// =============================================================================
// I — Interface Segregation Principle
// Клиент не должен зависеть от методов, которыми не пользуется
// =============================================================================

// ---- ПЛОХО: «толстый» интерфейс ----
public interface IWorker_Dirty
{
    void Work();
    void Eat();
    void Sleep();
    void AttendMeeting();
}

// Робот вынужден реализовывать Eat, Sleep, AttendMeeting — бессмысленно
public class Robot_Dirty : IWorker_Dirty
{
    public void Work()           => Console.WriteLine("Робот работает");
    public void Eat()            => throw new NotSupportedException("Роботы не едят");
    public void Sleep()          => throw new NotSupportedException("Роботы не спят");
    public void AttendMeeting()  => throw new NotSupportedException("Роботы не ходят на совещания");
}

// ---- ХОРОШО: разбиваем на узкие интерфейсы ----
public interface IWorkable   { void Work(); }
public interface IEatable    { void Eat();  }
public interface ISleepable  { void Sleep(); }
public interface IMeetable   { void AttendMeeting(); }

/// <summary>Робот реализует только то, что умеет.</summary>
public class Robot : IWorkable
{
    public void Work() => Console.WriteLine("Робот работает");
}

/// <summary>Человек реализует все нужные интерфейсы.</summary>
public class HumanWorker : IWorkable, IEatable, ISleepable, IMeetable
{
    public void Work()          => Console.WriteLine("Человек работает");
    public void Eat()           => Console.WriteLine("Человек обедает");
    public void Sleep()         => Console.WriteLine("Человек спит");
    public void AttendMeeting() => Console.WriteLine("Человек идёт на совещание");
}

// Клиент зависит только от нужного интерфейса:
public class WorkScheduler(IWorkable worker)
{
    public void ScheduleWork() => worker.Work();
}
