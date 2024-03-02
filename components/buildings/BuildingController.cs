using Godot;

[GlobalClass]
public partial class BuildingController : Resource
{
    public virtual void _Spawn()
    {
        GD.Print("Spawning entity");
    }
}