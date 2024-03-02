using Godot;
using System;

public partial class Camera : Camera2D
{
	public Main mainScene;

	private string _missingTileMapWarningMessage = "Camera2D needs a TileMap sibling node of type `ProceduralTileMap`";

	public override void _Ready()
	{
		base._Ready();

		LimitLeft = 0;
		LimitTop = 0;
	}

	public void SetCameraLimits(Main parentScene)
	{
		mainScene = parentScene;

		LimitRight = parentScene.proceduralTileMap.worldData.worldDimmensions.X * 16;
		LimitBottom = parentScene.proceduralTileMap.worldData.worldDimmensions.Y * 16;
	}
}
