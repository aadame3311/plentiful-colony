using Godot;

[GlobalClass]
public partial class VillagerIdle : State
{
  Vector2 moveDirection;
  double wanderTime;

  void RandomizeWander()
  {
    moveDirection = new Vector2(GD.RandRange(-1, 1), GD.RandRange(-1, 1)).Normalized();
    wanderTime = GD.RandRange(1, 2);
  }

  public override void Enter()
  {
    base.Enter();
    villagerStateMachine = GetParent<VillagerStateMachine>();
    villager = villagerStateMachine.villager;

    RandomizeWander();
  }

  public override void Update(double delta)
  {
    base.Update(delta);
    if (wanderTime > 0)
    {
      wanderTime -= delta;
      return;
    }

    RandomizeWander();
  }

  public override void PhysicsUpdate(double delta)
  {
    base.PhysicsUpdate(delta);

    if (villager != null)
    {
      villager.Velocity = moveDirection * villager.creatureData.creatureController.speed;
      villager.MoveAndSlide();
    }
  }

}