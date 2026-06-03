/// <summary>Марка автомобиля.</summary>
public class CarBrand(int id, string name)
{
    public int    Id   { get; } = id;
    public string Name { get; } = name;

    public override string ToString() => $"[{Id}] {Name}";
}

/// <summary>Автомобиль.</summary>
public class Car(int id, int brandId, string name, int horsepower)
{
    public int    Id         { get; }      = id;
    public int    BrandId    { get; set; } = brandId;
    public string Name       { get; set; } = name;
    public int    Horsepower { get; set; } = horsepower;

    public override string ToString() =>
        $"[{Id}] BrandId={BrandId}  {Name}  ({Horsepower} л.с.)";
}
