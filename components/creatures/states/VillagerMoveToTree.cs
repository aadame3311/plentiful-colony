using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class VillagerMoveToTree : State
{
    public override void Enter()
    {
        base.Enter();
        // if (villager.treeToChop != null && IsInstanceValid(villager.treeToChop))
        // {
        //     villager.navigationAgent.TargetPosition = villager.treeToChop.Position;
        //     return;
        // }

        MakePathToClosestTree();
    }

    public override void PhysicsUpdate(double delta)
    {
        // TODO edge case needs to be fixed..
        // TODO when nearest tree is on the other side of an unreachable destination

        base.PhysicsUpdate(delta);
        if (villager.navigationAgent.IsNavigationFinished() && IsInstanceValid(villager.treeToChop))
        {
            EmitSignal(SignalName.OnTransition, this, "villagerchoptree");

            return;
        }

        // move to tree
        if (IsInstanceValid(villager.treeToChop) && villager.navigationAgent.IsTargetReachable())
        {
            Utils.MoveVillagerAlongNavigationPath(villager);
        }
        else
        {
            MakePathToClosestTree();
        }

        // villager.navigationAgent.TargetPosition = villager.treeToChop.Position;
    }

    public void MakePathToClosestTree()
    {
        Node2D treeWorldPosition = Utils.FindClosestDistanceToTarget(
            villager,
            villager.GetParent<Main>().trees
        );
        villager.treeToChop = treeWorldPosition as Tree;
        villager.navigationAgent.TargetPosition = treeWorldPosition.Position;
    }
}
