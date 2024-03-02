using Godot;

[GlobalClass]
public partial class BuildingData : Resource
{
    [Export]
    public BuildingType buildingType;

    [Export]
    public AtlasTexture texture;

    [Export]
    public BuildingController buildingBehavior;

    // set on _Ready
    public Vector2I buildingDimmensions;
}