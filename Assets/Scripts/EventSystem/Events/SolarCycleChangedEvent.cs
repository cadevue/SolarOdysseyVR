public readonly struct SolarCycleChangedEvent : IEvent
{
    public float CycleProgress { get; }

    public SolarCycleChangedEvent(float cycleProgress)
    {
        CycleProgress = cycleProgress;
    }
}