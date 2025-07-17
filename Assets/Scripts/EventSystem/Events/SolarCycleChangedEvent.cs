public readonly struct SolarCycleChangedEvent : IEvent
{
    public SolarCycleModel Model { get; }

    public SolarCycleChangedEvent(SolarCycleModel model)
    {
        Model = model;
    }
}