using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class VillagerStateMachine : Node
{
    [Export]
    State initialState;

    [Export]
    public Villager villager;

    State currentState;
    Dictionary<string, State> states;

    public override void _Ready()
    {
        base._Ready();

        states = new Dictionary<string, State>();

        foreach (Node child in GetChildren())
        {
            if (child is State)
            {
                states[child.Name.ToString().ToLower()] = child as State;
                (child as State).OnTransition += (State state, string newStateName) =>
                {
                    OnChildTransition(state, newStateName);
                };
            }
        }

        if (initialState != null)
        {
            initialState.Enter();
            currentState = initialState;
        }

        villager.OnStateChange += (Villager v) =>
       {
           if (currentState == states["villagermovetotree"])
           {
               currentState.Exit();
               currentState.Enter();
           }
       };
    }

    public void OnChildTransition(State state, string newStateName)
    {
        if (state != currentState)
        {
            return;
        }


        State newState = states[newStateName.ToLower()];
        if (newState != null)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            newState.Enter();
            currentState = newState;
        }


    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (currentState != null)
        {
            currentState.Update(delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (currentState != null)
        {
            currentState.PhysicsUpdate(delta);
        }
    }
}