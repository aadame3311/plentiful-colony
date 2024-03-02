using Godot;

public partial class Tree : Node2D
{
	[Signal]
	public delegate void OnTreeDestroyedEventHandler(Node tree);

	public bool isInteractedWith = false;

	// if an object collides with this, despawns tree
	Area2D spawnConflictArea;
	ProceduralTileMap tileMap;

	public override void _Ready()
	{
		tileMap = GetParent<ProceduralTileMap>();

		spawnConflictArea = GetNode<Area2D>("%SpawnConflictArea");
		spawnConflictArea.BodyEntered += (Node2D node) => { OnConflictDetectionEntered(node); };
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

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

	public void OnHitDetected(Node2D node)
	{
		GD.Print("Tree hit!!");
	}
}
