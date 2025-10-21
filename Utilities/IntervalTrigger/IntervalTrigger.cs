using Godot;
using System;

public partial class IntervalTrigger : RefCounted
{
    // Repeats infinitely if set to 0
    public int IntervalCount { get; private set; } = 1;
    public float Interval { get; private set; } = 1f;
    public bool IsCompleted { get; private set; } = false;
    private int _currentIntervalCount = 0;
    private float _currentTimeElapsed = 0;
    private float _totalTimeElapsed = 0;
    private Action _onIntervalAction = null;
    private Action _onCompletedAction = null;
    private float _duration = 0f;
    private bool _isValid = true;
    public IntervalTrigger(int intervalCount, float interval, float duration, bool triggerOnStart, Action onIntervalAction, Action onCompletedAction = null)
    {
        if (intervalCount < 0 || interval <= 0f)
        {
            _isValid = false;
            GD.PushError("IntervalTrigger initialized with invalid parameters.");
            return;
        }
        IntervalCount = intervalCount;
        Interval = interval;
        _onIntervalAction = onIntervalAction;
        _onCompletedAction = onCompletedAction;
        _duration = duration;
        if (triggerOnStart && intervalCount > 0)
            _currentTimeElapsed = interval;
    }
    public void Tick(double delta)
    {
        if (IsCompleted || !_isValid)
            return;
        _currentTimeElapsed += (float)delta;
        _totalTimeElapsed += (float)delta;
        if (_currentTimeElapsed > Interval || Mathf.IsEqualApprox(_currentTimeElapsed, Interval))
        {
            _currentTimeElapsed -= Interval;
            _currentIntervalCount++;
            _onIntervalAction?.Invoke();
        }
        if ((_currentIntervalCount >= IntervalCount && IntervalCount != 0) ||
            (_duration > 0f && _totalTimeElapsed >= _duration))
        {
            _onCompletedAction?.Invoke();
            IsCompleted = true;
        }
    }
}
