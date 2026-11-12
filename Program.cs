using Pure.DI;

var composition = new Composition();
Console.WriteLine(composition.Root);

public sealed class Options;

public sealed class Service(string name, Options? options = null)
{
    public override string ToString() => $"{name} {options is not null}";
}

partial class Composition
{
    private static void Setup() => DI.Setup(nameof(Composition))
        .DependsOn("Services")
        .Root<Service>("Root");
}
