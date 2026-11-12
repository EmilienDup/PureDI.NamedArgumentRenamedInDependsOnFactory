# Pure.DI: a named argument in a factory of another setup file is renamed with the local variable it passes

Minimal repro for Pure.DI master (e3d91a1) and 2.5.7. The composition depends on a setup declared in another file. A factory of that setup passes a local variable by a named argument with the same name, `options: options`. The generated code renames the label with the variable, `localOptions: localOptions`, which does not compile (CS1739).

[`ISSUE.md`](ISSUE.md) is the text to paste into a Pure.DI GitHub issue.

```
dotnet build                                                                                   # 2.5.7 from nuget.org: CS1739
dotnet build -p:PureDIVersion=<master build version> -p:RestoreAdditionalProjectSources=<folder with the package>
```

| version | result |
|---|---|
| 2.5.7 | CS1739 |
| master e3d91a1 | CS1739 |
| master e3d91a1 + the change in ISSUE.md | compiles, prints `service True` |
| the factory in `Program.cs`, or `options: value` | compiles, prints `service True` |
