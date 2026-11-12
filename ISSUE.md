# A named argument in a factory of a setup in another file is renamed with the local variable it passes (CS1739)

**Pure.DI master (e3d91a1)** and **2.5.7**, .NET SDK 10.0.403, macOS. Repro project attached (`dotnet build`).

## Minimal code

`Program.cs`:

```csharp
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
```

`Services.cs`:

```csharp
using Pure.DI;

static class Services
{
    private static void Setup() => DI.Setup("Services", CompositionKind.Internal)
        .Bind<Options>().To<Options>()
        .Bind<Service>().To(ctx =>
        {
            ctx.Inject(out Options options);
            return new Service("service", options: options);
        });
}
```

## Expected

The program prints `service True`.

## Actual

```
error CS1739: The best overload for 'Service' does not have a parameter named 'localOptions'
```

The generated code renames the name of the argument along with the variable:

```csharp
new Service("service", localOptions: localOptions)
```

It compiles when the factory is in `Program.cs`, or when the variable has another name than the parameter (`ctx.Inject(out Options value); … options: value`).

## Cause

`FactoryCodeBuilder` renames the locals of a factory with the semantic model of the setup of the composition:

```csharp
var factoryExpression = (LambdaExpressionSyntax)localVariableRenamingRewriter.Rewrite(setup.SemanticModel, false, originalLambda);
```

When the factory is in another file, `LocalVariableRenamingRewriter.VisitToken` cannot look up its symbols in that semantic model (`_semanticModel?.SyntaxTree != parent.SyntaxTree`), so it renames every identifier token whose text is the name of a renamed local, including the name of a named argument.

Passing the semantic model of the factory fixes the repro:

```csharp
var factoryExpression = (LambdaExpressionSyntax)localVariableRenamingRewriter.Rewrite(factory.Source.SemanticModel, false, originalLambda);
```

With this change, the Pure.DI repro projects I have give the same results as before. I have not run the Pure.DI test suite with it.
