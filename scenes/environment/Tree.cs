using Godot;

public partial class Tree : Interactable
{
    [Signal]
    public delegate void OnTreeDestroyedEventHandler(Tree tree);

    public bool isInteractedWith = false;

    // if an object collides with this, despawns tree
    Area2D spawnConflictArea;
    Main world;
    AnimationPlayer treeShakeAnimation;
    Health healthComponent;

    public override void _Ready()
    {
        world = GetParent<ProceduralTileMap>().GetParent<Main>();
        treeShakeAnimation = GetNode<AnimationPlayer>("%TreeShakeAnimation");
        spawnConflictArea = GetNode<Area2D>("%SpawnConflictArea");
        // spawnConflictArea.BodyEntered += (Node2D node) =>
        // {
        //     OnConflictDetectionEntered(node);
        // };
        healthComponent = GetNode<Health>("Health");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    // removes tree from scene as well as stored tree locations for navigation purposes
    public void RemoveTree()
    {
        if (!IsQueuedForDeletion())
        {
            EmitSignal(SignalName.OnTreeDestroyed, this);
            QueueFree();

            // remove tree from tree grid list
            world.trees.Remove(this);
        }
    }

    // public void OnConflictDetectionEntered(Node2D node)
    // {
    //     if (node is TownCenter)
    //     {
    //         RemoveTree();
    //         EmitSignal(SignalName.OnTreeDestroyed, this);
    //     }
    // }

    public void SignalVillagerInventoryUpdate(Villager villagerNode)
    {
        int currentWoodCount = villagerNode.woodCount;
        int newWoodCount = currentWoodCount + 1;
        int woodLimit = villagerNode.creatureData.creatureInventoryData.woodLimit;

        if (newWoodCount > woodLimit)
        {
            return;
        }

        villagerNode.woodCount = newWoodCount;
    }

    public void TreeHit(Node2D node)
    {
        if (!IsInstanceValid(treeShakeAnimation))
        {
            return;
        }

        treeShakeAnimation.Play("tree_shake");
        EmitSignal(SignalName.OnHitDetected, node);

        bool treeIsBeingHitByPlayer = node is Villager;
        if (treeIsBeingHitByPlayer)
        {
            SignalVillagerInventoryUpdate(node as Villager);
        }
    }
}
