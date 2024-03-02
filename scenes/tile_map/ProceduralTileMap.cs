using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

public partial class ProceduralTileMap : TileMap
{
  FastNoiseLite noise = new FastNoiseLite();

  private Array<PackedScene> _trees = new Array<PackedScene> {
    GD.Load<PackedScene>("res://scenes/environment/pine_tree.tscn"),
		// GD.Load<PackedScene>("res://scenes/environment/tree.tscn"),
		// GD.Load<PackedScene>("res://scenes/environment/square_tree.tscn")
	};

  [Export]
  public WorldData worldData;

  public enum TerrainTypes
  {
    GRASSLAND = 0
  }

  public List<Vector2I> validSpawnPoints = new List<Vector2I>();

  public Array<Vector2I> treeGrid = new Array<Vector2I>();

  public Array<Tree> trees = new Array<Tree>();

  private bool[,] visited;


  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    visited = new bool[worldData.worldDimmensions.X, worldData.worldDimmensions.Y];

    // gets us a unique seed every time
    GD.Randomize();

    noise.Seed = (int)Math.Floor(GD.Randf() * 50);
    noise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
    noise.Frequency = (float)0.007;

    Generate();
    FindValidTownCenterSpawnPoints(TerrainTypes.GRASSLAND);

    PoissonDiscSampling pds = new PoissonDiscSampling(1.7, 50, worldData);
    treeGrid = pds.Process();

    foreach (Vector2I vector in treeGrid)
    {
      if (vector.X == -1)
      {
        continue;
      }

      TileData td = GetCellTileData(0, vector);
      if (td != null && !(bool)td.GetCustomDataByLayerId(0))
      {
        continue;
      }

      Node2D pineTreeInstance = _trees.PickRandom().Instantiate<Node2D>();
      trees.Add(pineTreeInstance as Tree);

      pineTreeInstance.Position = MapToLocal(vector);
      pineTreeInstance.AddToGroup("trees");
      AddChild(pineTreeInstance);
    }
  }


  public void FindValidTownCenterSpawnPoints(TerrainTypes terrainTypes)
  {
    Vector2I areaSize = new Vector2I(5, 5);

    for (int x = areaSize.X; x < worldData.worldDimmensions.X - areaSize.X; x++)
    {
      for (int y = areaSize.Y; y < worldData.worldDimmensions.Y - areaSize.Y; y++)
      {
        TileData tileData = GetCellTileData(0, new Vector2I(x, y));
        if ((bool)tileData.GetCustomDataByLayerId(1))
        {
          if (IsSpawnLocationValid(x, y, 0, 0, areaSize))
          {
            validSpawnPoints.Add(new Vector2I(x, y));
          }
        }
      }
    }
  }

  private bool IsSpawnLocationValid(int x, int y, int startX, int startY, Vector2I areaSize)
  {
    if (x < 0 || x > worldData.worldDimmensions.X - areaSize.X || y < 0 || y > worldData.worldDimmensions.Y || visited[x, y])
    {
      return false;
    }
    visited[x, y] = true;

    TileData tileData = GetCellTileData(0, new Vector2I(x, y));

    bool canSpawnBuilding = (bool)tileData.GetCustomDataByLayerId(1);
    if (!canSpawnBuilding)
    {
      return false;
    }

    bool result = true;

    if (startX + 1 < areaSize.X && x + 1 < worldData.worldDimmensions.X - areaSize.X && !visited[x + 1, y])
    {
      result = result && IsSpawnLocationValid(x + 1, y, startX + 1, startY, areaSize);
    }
    if (Mathf.Abs(startX - 1) < areaSize.X && x - 1 >= 0 && !visited[x - 1, y])
    {
      result = result && IsSpawnLocationValid(x - 1, y, startX - 1, startY, areaSize);
    }

    if (startY + 1 < areaSize.Y && y + 1 < worldData.worldDimmensions.Y - areaSize.Y && !visited[x, y + 1])
    {
      result = result && IsSpawnLocationValid(x, y + 1, startX, startY + 1, areaSize);
    }
    if (Mathf.Abs(startY - 1) < areaSize.Y && y - 1 >= 0 && !visited[x, y - 1])
    {
      result = result && IsSpawnLocationValid(x, y - 1, startX, startY - 1, areaSize);
    }

    return true && result;
  }

  // generates terrain
  public void Generate()
  {
    foreach (int x in GD.Range(worldData.worldDimmensions.X))
    {
      foreach (int y in GD.Range(worldData.worldDimmensions.Y))
      {
        var groundNoiseValue = noise.GetNoise2D(x, y);
        int randomGroundTile = Math.Clamp((int)Math.Floor(Math.Abs(groundNoiseValue * worldData.groundValue)), 0, 19);

        SetCell(0, new Vector2I(x, y), 1, new Vector2I(randomGroundTile, 0));
      }
    }
  }
}
