using Godot;

[GlobalClass]
public partial class VillagerChopTree : State
{
    double initialChopTimer = 1;
    double chopTimer;

    public override void Enter()
    {
        base.Enter();

        chopTimer = 0;
        villager.raycast.CollideWithBodies = true;
        if (villager.treeToChop == null || villager.treeToChop.IsQueuedForDeletion())
        {
            EmitSignal(SignalName.OnTransition, this, "villagermovetotree");

        }

        villager.treeToChop.OnTreeDestroyed += (Node n) =>
        {
            EmitSignal(SignalName.OnTransition, this, "villagermovetotree");
        };
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);

        var space = villager.GetViewport().World2D.DirectSpaceState;

        if (IsInstanceValid(villager.treeToChop))
        {
            PhysicsRayQueryParameters2D prq = new PhysicsRayQueryParameters2D
            {
                From = villager.Position,
                To = villager.treeToChop.Position
            };

            var results = space.IntersectRay(prq);

            Tree tree = (Tree)results["collider"];
            if (tree != null)
            {
                tree.RemoveTree();
            }

        }

        EmitSignal(SignalName.OnTransition, this, "villagermovetotree");
    }
}