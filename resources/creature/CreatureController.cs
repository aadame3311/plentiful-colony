using Godot;

public partial class CreatureController : Resource
{
    [Export]
    public int health;
    [Export]
    public int speed = 10;

    [Signal]
    public delegate void OnSpawnEventHandler();

    public virtual void _OnSpawn()
    {
        EmitSignal(SignalName.OnSpawn, this);
    }
}