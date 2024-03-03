using Godot;

[GlobalClass]
public partial class VillagerChopTree : State
{
    double initialChopTimer = 1;

    float timer = 0;

    public override void Enter()
    {
        base.Enter();
        timer = villager.creatureData.creatureController.treeChopRate;

        villager.treeToChop.OnTreeDestroyed += (Tree tree) =>
        {
            villager.treeToChop = null;
            if (villager.woodCount >= villager.creatureData.creatureInventoryData.woodLimit)
            {
                EmitSignal(SignalName.OnTransition, this, "villagermovetodeposit");
                return;
            }

            EmitSignal(SignalName.OnTransition, this, "villagermovetotree");
        };

        OnEnter();
    }

    public void OnEnter()
    {
        if (villager.treeToChop == null || villager.treeToChop.IsQueuedForDeletion())
        {
            EmitSignal(SignalName.OnTransition, this, "villagermovetotree");
        }
    }

    // signal villager to deposit resource if inventory full
    public void CheckVillagerInventoryLimits()
    {
        int woodCount = villager.woodCount;
        int woodLimit = villager.creatureData.creatureInventoryData.woodLimit;

        if (woodCount >= woodLimit)
        {
            EmitSignal(SignalName.OnTransition, this, "villagermovetodeposit");
        }
    }

    public void CheckForCollisions()
    {
        if (!villager.raycast.IsColliding())
        {
            timer = villager.creatureData.creatureController.treeChopRate;
            EmitSignal(SignalName.OnTransition, "villagermovetotree");
            return;
        }

        var collider = villager.raycast.GetCollider();
        if (collider != villager.treeToChop)
        {
            timer = villager.creatureData.creatureController.treeChopRate;
            EmitSignal(SignalName.OnTransition, "villagermovetotree");
            return;
        }

        villager.treeToChop.TreeHit(villager);
        timer = villager.creatureData.creatureController.treeChopRate;

        CheckVillagerInventoryLimits();
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);
        timer -= 1f * (float)delta;

        if (timer > 0)
        {
            return;
        }

        if (!IsInstanceValid(villager.treeToChop))
        {
            return;
        }

        villager.raycast.TargetPosition = villager.ToLocal(villager.treeToChop.Position);
        CallDeferred("CheckForCollisions");
    }
}
