using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.StateMachine
{
public class StateMachine
{
    public IState Current { get; private set; }
    public IState Previous { get; private set; }

    /// <summary>How long current state has been active.</summary>
    public float TimeInState { get; private set; }

    /// <summary>Event that can be watched outside the class (Exit old → Enter new has completed).</summary>
    public event Action<IState, IState> StateChanged;

    // Lookup table: current state => states you can go to.
    private readonly Dictionary<Type, List<Transition>> _map = new();
    // States you can go to from any state.
    private readonly List<Transition> _any = new();
    // Cached list of transitions you can go to from current state.
    private List<Transition> _currentTransitions = s_Empty;
    // Used as a quasi null value
    private static readonly List<Transition> s_Empty = new();

    // To avoid re-entrant transitions within the same Tick
    private bool _isTransitioning;

    public void Initialize(IState initialState)
    {
        Current = null;
        Previous = null;
        TimeInState = 0f;
        _isTransitioning = false;
        _currentTransitions = s_Empty;

        SetState(initialState);
    }

    public void Tick(float deltaTime = 0f)
    {
        TimeInState += deltaTime;

        var transition = SelectTransition();
        if (transition != null)
            SetState(transition.To);

        // do whatever the state does on tick
        Current?.Tick();
    }

    public void LateTick()
    {
        // do whatever the state does on fixed tick
        Current?.LateTick();
    }

    public void FixedTick()
    {
        // do whatever the state does on fixed tick
        Current?.FixedTick();
    }

    private Transition SelectTransition() {
        // priority: global > current state
        // priority within each group: Automatically ordered by priority value in AddState (higher first)

        // 1) Global transitions
        for (int i = 0; i < _any.Count; i++)
        {
            var t = _any[i];
            if (t.To == null || t.To == Current) continue;

            bool shouldTransition = false;
            try { shouldTransition = t.Condition?.Invoke() ?? false; }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (shouldTransition) return t;
        }

        // 2) Transitions valid from the CURRENT state's type (highest priority first)
        for (int i = 0; i < _currentTransitions.Count; i++)
        {
            var t = _currentTransitions[i];
            if (t.To == null || t.To == Current) continue;

            bool shouldTransition = false;
            try { shouldTransition = t.Condition?.Invoke() ?? false; }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (shouldTransition) return t;
        }

        return null;
    }

    private void SetState(IState newState) {
        if (newState == null) return;
        if (Current == newState) return; // no-op

        if (_isTransitioning) return; // prevent re-entrant loops

        _isTransitioning = true;

        try {
            // Exit old state
            Current?.Exit();
            // Switch to new state
            Previous = Current;
            Current = newState;
            // Refresh per-state transition cache
            _currentTransitions = _map.GetValueOrDefault(Current.GetType(), s_Empty);
            // Enter new state
            TimeInState = 0f;
            Current.Enter();
            // Notify outside world
            StateChanged?.Invoke(Previous, Current);
        }
        finally {
            _isTransitioning = false;
        }
    }

    /// <summary>Add a transition usable only from the specified state, ordered by priority.</summary>
    public void AddTransition(IState from, IState to, Func<bool> condition, int priority = 0)
    {
        // I don't think I'll ever pass null there, but just to be sure
        if (from == null || to == null || condition == null) return;

        var key = from.GetType();

        if (!_map.TryGetValue(key, out var list))
        {
            list = new List<Transition>();
            _map[key] = list;
        }

        list.Add(new Transition(to, condition, priority));
        list.Sort(Transition.CompareByPriorityDesc);

        // If we're currently in `from`, keep the cache pointing at the (now sorted) list
        if (Current != null && Current.GetType() == key)
            _currentTransitions = list;
    }

    /// <summary>Add a global (any-state) transition, ordered by priority.</summary>
    public void AddAnyTransition(IState to, Func<bool> condition, int priority = 0)
    {
        if (to == null || condition == null) return;

        _any.Add(new Transition(to, condition, priority));
        _any.Sort(Transition.CompareByPriorityDesc);
    }

    private sealed class Transition
    {
        public readonly IState To;
        public readonly Func<bool> Condition;
        public readonly int Priority;

        public Transition(IState to, Func<bool> condition, int priority)
        {
            To = to;
            Condition = condition;
            Priority = priority;
        }

        public static int CompareByPriorityDesc(Transition a, Transition b) =>
            b.Priority.CompareTo(a.Priority);
    }
}
}