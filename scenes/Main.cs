using Godot;

public partial class Main : Node2D
{

	[Signal]
	public delegate void OnWorldEnvironmentChangeEventHandler(Node instance);

	private PackedScene _starterTownCenter = GD.Load<PackedScene>("res://scenes/buildings/town_center.tscn");
	private PackedScene _world = GD.Load<PackedScene>("res://scenes/tile_map/tile_map.tscn");

	private Camera _camera;
	private TownCenter _townCenter;
	public ProceduralTileMap proceduralTileMap;

	// debugging displaysv
	Label coordsLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitDebugging();

		proceduralTileMap = _world.Instantiate<ProceduralTileMap>();

		GD.Print("Loading - Generating world");

		// SubscribeToTreeOnDestroy has to happen after adding proc map into scene
		AddChild(proceduralTileMap);
		SubscribeToTreeOnDestroy();

		_camera = GetNode<Camera>("%Camera2D");

		_townCenter = (TownCenter)_starterTownCenter.Instantiate();

		GD.Print("Loading - Evaluating town center spawn location");
		EvaluateTownCenterPosition();
		AddChild(_townCenter);

		// ! for testing only, should replace with actual spawn logic
		Villager villager = _townCenter.spawnVillager();
		AddChild(villager);

		Villager villager2 = _townCenter.spawnVillager();
		villager2.Position = new Vector2(villager2.Position.X + 32, villager.Position.Y);
		AddChild(villager2);
		// !

	}

	public override void _Process(double delta)
	{
		_camera.SetCameraLimits(this);
		if (coordsLabel != null)
		{
			UpdateDebugUI();
		}
	}

	public void SubscribeToTreeOnDestroy()
	{
		foreach (Tree tree in proceduralTileMap.trees)
		{
			tree.OnTreeDestroyed += (Node t) =>
			{
				EmitSignal(SignalName.OnWorldEnvironmentChange, t);
			};
		}

	}

	public void UpdateDebugUI()
	{

		coordsLabel.Text = $"Coords: ({_camera.Position.X}, {_camera.Position.Y})";
		coordsLabel.Text += $"\nCamera Zoom: {_camera.Zoom}";
		coordsLabel.Text += $"\nCamera Viewport: {_camera.GetViewportRect().Size.X}";
		coordsLabel.Text += $"\nTileMap Used Rect: {proceduralTileMap.GetUsedRect().Size}";
	}

	public void EvaluateTownCenterPosition()
	{
		var spawnPoints = proceduralTileMap.validSpawnPoints;
		var randomSpawnPoint = spawnPoints[GD.RandRange(0, spawnPoints.Count - 1)];

		_townCenter.Position = proceduralTileMap.MapToLocal(randomSpawnPoint);
		_camera.Position = proceduralTileMap.MapToLocal(randomSpawnPoint);
	}

	public void InitDebugging()
	{
		coordsLabel = GetNode<Label>("%CoordsLabel");
		coordsLabel.Text = "Coords: ()";
	}
}
