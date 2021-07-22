using System;
using System.Collections.Generic;
using System.Linq;

public class AIBrain<T> where T : Enum
{
    private T _currentState;

    private Dictionary<T, Action> _statesBehaviour;

    private List<Transition<T>> _transitions;
     
    public AIBrain()
    {
        _statesBehaviour = new Dictionary<T, Action>();
        _transitions = new List<Transition<T>>();
    }

    public void AddState(T state, Action action)
    {
        if (_statesBehaviour.ContainsKey(state))
        {
            UnityEngine.Debug.LogError("State Already exists in state machine");
            return;
        }
        
        _statesBehaviour.Add(state, action);
    }

    public void AddTransition(T from, T to, Func<bool> condition)
    {
        var transition = new Transition<T>(from, to, condition);
        _transitions.Add(transition);
    }
    
    private void ChooseState()
    {
        foreach (var transition in _transitions)
        {
            if (transition.From.Equals(_currentState))
            {
                if (transition.Condition())
                {
                    _currentState = transition.To;
                }
            }
        }
    }

    public void Tick()
    {
        if (_statesBehaviour.ContainsKey(_currentState))
        {
            _statesBehaviour[_currentState]?.Invoke();
        }
        
        ChooseState();
    }

    private class Transition<TEnum> where TEnum : Enum
    {
        public TEnum From;
        public TEnum To { get; }
        public Func<bool> Condition { get; }

        public Transition(TEnum @from, TEnum to, Func<bool> condition)
        {
            From = @from;
            To = to;
            Condition = condition;
        }
    }
}
