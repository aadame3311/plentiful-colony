using Godot;

[GlobalClass]
public partial class WorldData : Resource
{
    [Export]
    public Vector2I worldDimmensions; // tile based dimmensions

    [Export]
    public int groundValue;
}