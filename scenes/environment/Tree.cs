using Godot;

public partial class Tree : Interactable
{
	[Signal]
	public delegate void OnTreeDestroyedEventHandler(Node tree);

	public bool isInteractedWith = false;

	// if an object collides with this, despawns tree
	Area2D spawnConflictArea;
	ProceduralTileMap tileMap;
	AnimationPlayer treeShakeAnimation;
	Health healthComponent;

	public override void _Ready()
	{
		tileMap = GetParent<ProceduralTileMap>();
		treeShakeAnimation = GetNode<AnimationPlayer>("%TreeShakeAnimation");
		spawnConflictArea = GetNode<Area2D>("%SpawnConflictArea");
		spawnConflictArea.BodyEntered += (Node2D node) => { OnConflictDetectionEntered(node); };
		healthComponent = GetNode<Health>("%Health");
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
			QueueFree();

			// remove tree from tree grid list
			tileMap.trees.Remove(this);
		}

	}

	public void OnConflictDetectionEntered(Node2D node)
	{
		if (node is TownCenter)
		{
			RemoveTree();
			EmitSignal(SignalName.OnTreeDestroyed, this);
		}
	}

	public void TreeHit(Node2D node)
	{

		if (IsInstanceValid(treeShakeAnimation))
		{
			treeShakeAnimation.Play("tree_shake");
			EmitSignal(SignalName.OnHitDetected, node);
		}
	}
}
