using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CardStateMachine : Node
{
    [Export]
    public CardState initialState;

    public CardState currentState;
    public readonly Dictionary<CardState.State, CardState> states = new();

    public void Init(CardUI card)
    {
        foreach (Node child in GetChildren())
        {
            if (child is CardState state)
            {
                states[state.state] = state;
                state.TransitionRequested += _OnTransitionRequested;
                state.card_ui = card;
            }
        }

        if (initialState != null)
        {
            initialState.Enter();
            currentState = initialState;
        }
    }

    public void OnInput(InputEvent @event)
    {
        currentState?.OnInput(@event);
    }

    public void OnGuiInput(InputEvent @event)
    {
        currentState?.OnGuiInput(@event);
    }

    public void OnMouseEntered()
    {
        currentState?.OnMouseEntered();
    }

    public void OnMouseExited()
    {
        currentState?.OnMouseExited();
    }

    private void _OnTransitionRequested(CardState from, CardState.State to)
    {
        if (from != currentState)
            return;

        if (!states.TryGetValue(to, out var newState))
            return;

        currentState?.Exit();
        newState.Enter();
        currentState = newState;
    }
}
