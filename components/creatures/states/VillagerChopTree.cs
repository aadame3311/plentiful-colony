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

  }


  public override void PhysicsUpdate(double delta)
  {
    base.PhysicsUpdate(delta);

    var space = villager.GetViewport().World2D.DirectSpaceState;

    if (IsInstanceValid(villager.treeToChop))
    {
      villager.raycast.TargetPosition = villager.ToLocal(villager.treeToChop.Position);
      if (!villager.raycast.IsColliding())
      {
        return;
      }

      var collider = villager.raycast.GetCollider();
      if (collider == villager.treeToChop)
      {
        villager.treeToChop.RemoveTree();
      }
    }

    EmitSignal(SignalName.OnTransition, this, "villagermovetotree");
  }
}