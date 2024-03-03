using Godot;

[GlobalClass]
public partial class VillagerChopTree : State
{
  double initialChopTimer = 1;
  Timer chopTimer;
  bool chopTree;

  public override void Enter()
  {
    base.Enter();

    // when timer runs out, villager will execute chop action on it's treeToChop
    chopTimer = Utils.createTimer(this, villager.treeChopRate, true);
    chopTimer.Timeout += () => { chopTree = true; };
    chopTimer.Start();

    villager.raycast.CollideWithBodies = true;
    if (villager.treeToChop == null || villager.treeToChop.IsQueuedForDeletion())
    {
      GD.Print("transitioning");
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
      if (collider == villager.treeToChop && chopTree)
      {
        chopTree = false;
        villager.treeToChop.TreeHit(villager);
      }
    }

    EmitSignal(SignalName.OnTransition, this, "villagermovetotree");
  }
}