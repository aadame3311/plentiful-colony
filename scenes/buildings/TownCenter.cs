using Godot;
using System;
using System.Numerics;

public partial class TownCenter : StaticBody2D
{
	[Export]
	public BuildingData buildingData;

	public Sprite2D spriteNode;

	private TileMap _tileMap;

	private Main _mainScene;

	private PackedScene _maleVillagerScene = GD.Load<PackedScene>("res://scenes/creatures/male_villager.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spriteNode = GetChild<Sprite2D>(0);
		spriteNode.Texture = buildingData.texture;

		_mainScene = GetParent<Main>();

		buildingData.buildingDimmensions = new Vector2I((int)spriteNode.Texture.GetSize().X, (int)spriteNode.Texture.GetSize().Y);
	}

	public Villager spawnVillager()
	{
		Villager villager = _maleVillagerScene.Instantiate<Villager>();
		villager.Position = new Godot.Vector2(Position.X, Position.Y + 16);

		return villager;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
