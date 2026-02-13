using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    [Export] public NodePath initialState;

    private Dictionary<string, State> _states;
    private State _current_state;

   public override void _Ready()
    {
        _states = new Dictionary<string, State>();
        foreach (Node node in GetChildren())
        {
            if (node is State s)
            {
                _states[node.Name] = s;
                s.stateMachine = this;
                s.Ready();
                s.Exit();
            }
        }

        // If initialState is empty or not found, pick the first available state as fallback.
        State firstState = null;
        foreach (var s in _states.Values) { firstState = s; break; }

        if (initialState == null || string.IsNullOrEmpty(initialState.ToString()) || !HasNode(initialState))
        {
            if (firstState == null)
            {
                GD.PrintErr("StateMachine: no initialState specified and no child states found.");
                return;
            }
            _current_state = firstState;
        }
        else
        {
            _current_state = GetNode<State>(initialState);
        }

        _current_state.Enter();
    }

    public override void _Process(double delta)
    {
        _current_state.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _current_state.UpdatePhysics(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _current_state.HandleInput(@event);
    }

    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key) || _current_state == _states[key])
        {
            return;
        }

        _current_state.Exit();
        _current_state = _states[key];
        _current_state.Enter();
    }
}
