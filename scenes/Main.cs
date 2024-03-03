using Godot;
using Godot.Collections;

public partial class Main : Node2D
{
    [Signal]
    public delegate void OnWorldEnvironmentChangeEventHandler(Node instance);

    private PackedScene _starterTownCenter = GD.Load<PackedScene>(
        "res://scenes/buildings/town_center.tscn"
    );
    private PackedScene _world = GD.Load<PackedScene>("res://scenes/tile_map/tile_map.tscn");
    private Array<PackedScene> _trees = new Array<PackedScene>
    {
        GD.Load<PackedScene>("res://scenes/environment/pine_tree.tscn"),
        // GD.Load<PackedScene>("res://scenes/environment/tree.tscn"),
        // GD.Load<PackedScene>("res://scenes/environment/square_tree.tscn")
    };
    private Camera _camera;
    private TownCenter _townCenter;
    public ProceduralTileMap proceduralTileMap;
    public Array<Node2D> trees = new Array<Node2D>();
    public Array<Node2D> depositCenters = new Array<Node2D>();

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
        depositCenters.Add(_townCenter);

        PoissonDiscSampling pds = new PoissonDiscSampling(1.7, 50, proceduralTileMap.worldData);
        Array<Vector2I> treeGrid = pds.Process();

        foreach (Vector2I vector in treeGrid)
        {
            if (vector.X == -1)
            {
                continue;
            }

            TileData td = proceduralTileMap.GetCellTileData(0, vector);
            if (td != null && !(bool)td.GetCustomDataByLayerId(0))
            {
                continue;
            }

            Node2D pineTreeInstance = _trees.PickRandom().Instantiate<Node2D>();
            pineTreeInstance.Position = proceduralTileMap.MapToLocal(vector);

            GD.Print($"Distance to: {pineTreeInstance.Position.DistanceTo(_townCenter.Position)}");
            if (pineTreeInstance.Position.DistanceTo(_townCenter.Position) > 100f)
            {
                trees.Add(pineTreeInstance);
                pineTreeInstance.AddToGroup("trees");
                proceduralTileMap.AddChild(pineTreeInstance);
            }
        }

        CallDeferred("SpawnVillagers");
    }

    public void SpawnVillagers()
    {
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
        // foreach (Tree tree in trees)
        // {
        //     tree.OnTreeDestroyed += (Tree t) =>
        //     {
        //         EmitSignal(SignalName.OnWorldEnvironmentChange, t);
        //     };
        // }
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
