// =============================================================================
// L — Liskov Substitution Principle
// Подтип должен быть полностью взаимозаменяем с базовым типом
// =============================================================================

// ---- ПЛОХО: Square нарушает контракт Rectangle ----
public class Rectangle_Dirty
{
    public virtual double Width  { get; set; }
    public virtual double Height { get; set; }

    public double Area() => Width * Height;
}

public class Square_Dirty : Rectangle_Dirty
{
    // Квадрат ломает ожидание: Width и Height всегда одинаковы
    public override double Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }
    }
    public override double Height
    {
        get => base.Height;
        set { base.Width = value; base.Height = value; }
    }
}

// Вот почему это плохо:
// Rectangle r = new Square_Dirty(); r.Width = 4; r.Height = 5;
// r.Area() вернёт 25, а не 20 — нарушено ожидание!

// ---- ХОРОШО: общий интерфейс без нарушения контракта ----

/// <summary>Фигура с площадью.</summary>
public interface IShape
{
    double Area();
}

public class Rectangle(double width, double height) : IShape
{
    public double Width  { get; } = width;
    public double Height { get; } = height;
    public double Area() => Width * Height;
}

public class Square(double side) : IShape
{
    public double Side { get; } = side;
    public double Area() => Side * Side;
}

// ---- Птицы: не все птицы летают ----

// ПЛОХО: Bird с методом Fly() — страусы не летают
public class Bird_Dirty
{
    public virtual void Fly() => Console.WriteLine("Птица летит");
}
public class Ostrich_Dirty : Bird_Dirty
{
    // Нарушаем LSP: страус «летит», но на деле — исключение или пустой метод
    public override void Fly() => throw new NotSupportedException("Страусы не летают!");
}

// ХОРОШО: разделить летающих и нелетающих
public abstract class BirdBase(string name)
{
    public string Name { get; } = name;
    public abstract void Eat();
}

public interface IFlyable
{
    void Fly();
}

public class Sparrow(string name) : BirdBase(name), IFlyable
{
    public override void Eat() => Console.WriteLine($"{Name} клюёт зёрна");
    public void Fly()          => Console.WriteLine($"{Name} летит");
}

public class Ostrich(string name) : BirdBase(name)
{
    public override void Eat() => Console.WriteLine($"{Name} щиплет траву");
    public void Run()          => Console.WriteLine($"{Name} бежит");
}
