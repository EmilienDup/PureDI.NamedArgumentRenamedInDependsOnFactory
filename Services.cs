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
