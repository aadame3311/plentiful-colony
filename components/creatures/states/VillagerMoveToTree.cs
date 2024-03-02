
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class VillagerMoveToTree : State
{
  public override void Enter()
  {
    base.Enter();

    CallDeferred("MakePathToClosestTree");
  }

  public override void Update(double delta)
  {
    base.Update(delta);
  }

  public override void Exit()
  {
    base.Exit();
  }

  public override void PhysicsUpdate(double delta)
  {
    // TODO edge case needs to be fixed..
    // TODO when nearest tree is on the other side of an unreachable destination

    base.PhysicsUpdate(delta);


    if (villager.navigationAgent.IsNavigationFinished())
    {
      GD.Print("Villager finished navigation");
      EmitSignal(SignalName.OnTransition, this, "villagerchoptree");

      return;
    }

    // move to tree
    if (IsInstanceValid(villager.treeToChop) && villager.navigationAgent.IsTargetReachable())
    {
      Vector2 direction = villager.navigationAgent.GetNextPathPosition();
      if (villager != null)
      {
        Vector2 newVelocity = direction - villager.GlobalPosition;
        newVelocity = newVelocity.Normalized();
        newVelocity *= villager.creatureData.creatureController.speed;
        villager.Velocity = newVelocity;

        villager.MoveAndSlide();
      }
    }
    else
    {
      MakePathToClosestTree();
    }
  }

  public bool CheckIfTargetReachable(Vector2 targetPosition)
  {
    villager.navigationAgent.TargetPosition = targetPosition;
    return villager.navigationAgent.IsTargetReachable();
  }

  public Tree FindClosestTree(out Vector2 closestDistance)
  {
    Main world = villager.GetParent<Main>();
    Array<Tree> treeGrid = world.proceduralTileMap.trees;

    Tree closestTree = treeGrid[0];
    Vector2 closestTreeVector = closestTree.Position;
    float closestTreeDistance = villager.Position.DistanceTo(closestTreeVector);

    foreach (Tree tree in treeGrid)
    {
      Vector2 vector = tree.Position;
      float currentDistance = villager.Position.DistanceTo(vector);

      if (currentDistance < closestTreeDistance && IsInstanceValid(tree))
      {
        closestTreeDistance = currentDistance;
        closestTreeVector = vector;
        closestTree = tree;
      }
    }

    closestDistance = closestTreeVector;

    return closestTree;
  }

  public void MakePathToClosestTree()
  {
    Vector2 treeWorldPosition;
    villager.treeToChop = FindClosestTree(out treeWorldPosition);

    villager.navigationAgent.TargetPosition = treeWorldPosition;
  }
}
