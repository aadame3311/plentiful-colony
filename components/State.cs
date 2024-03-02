using System.Reflection.Metadata.Ecma335;
using Godot;
public partial class State : Node
{
    public VillagerStateMachine villagerStateMachine;
    public Villager villager;

    [Signal]
    public delegate void OnTransitionEventHandler(State state, string newStateName);
    public virtual void Enter()
    {
        villagerStateMachine = GetParent<VillagerStateMachine>();
        villager = villagerStateMachine.villager;
    }
    public virtual void Exit() { }
    public virtual void Update(double delta) { }
    public virtual void PhysicsUpdate(double delta) { }
}