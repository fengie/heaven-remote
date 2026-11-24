namespace PhoneSeat.Logic;

public sealed class TurboDetector
{
    private readonly Queue<DateTime>[] _edges;
    private readonly bool[] _lastPressed;
    private readonly TimeSpan _window;
    private readonly int _edgeThreshold;

    public TurboDetector(int controllerSlots = 4, TimeSpan? window = null, int edgeThreshold = 8)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(controllerSlots);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(edgeThreshold);
        _window = window ?? TimeSpan.FromSeconds(1);
        _edgeThreshold = edgeThreshold;
        _edges = Enumerable.Range(0, controllerSlots).Select(_ => new Queue<DateTime>()).ToArray();
        _lastPressed = new bool[controllerSlots];
    }

    public bool Observe(int controllerIndex, bool pressed, DateTime utcNow)
    {
        Validate(controllerIndex);
        if (pressed != _lastPressed[controllerIndex])
        {
            _lastPressed[controllerIndex] = pressed;
            _edges[controllerIndex].Enqueue(utcNow);
        }

        while (_edges[controllerIndex].Count > 0 && utcNow - _edges[controllerIndex].Peek() > _window)
            _edges[controllerIndex].Dequeue();

        return _edges[controllerIndex].Count >= _edgeThreshold;
    }

    public void Reset(int controllerIndex)
    {
        Validate(controllerIndex);
        _edges[controllerIndex].Clear();
        _lastPressed[controllerIndex] = false;
    }

    private void Validate(int controllerIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(controllerIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(controllerIndex, _edges.Length);
    }
}
