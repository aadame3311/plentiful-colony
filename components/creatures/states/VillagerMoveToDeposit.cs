using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class VillagerMoveToDeposit : State
{
    public override void Enter()
    {
        base.Enter();

        MakePathToClosestDeposit();
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);

        if (villager.navigationAgent.IsNavigationFinished())
        {
            villager.woodCount = 0;
            EmitSignal(SignalName.OnTransition, this, "villagermovetotree");

            return;
        }

        // move to deposit
        if (villager.navigationAgent.IsTargetReachable())
        {
            Utils.MoveVillagerAlongNavigationPath(villager);
        }
    }

    public void MakePathToClosestDeposit()
    {
        Node2D depositCenterTarget = Utils.FindClosestDistanceToTarget(
            villager,
            villager.world.depositCenters
        );

        villager.navigationAgent.TargetPosition = depositCenterTarget.Position;
    }
}
