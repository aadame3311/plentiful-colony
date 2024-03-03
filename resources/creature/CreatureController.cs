using Godot;

public partial class CreatureController : Resource
{
    [Export]
    public int health;

    [Export]
    public int speed = 10;

    [Export]
    public int treeDamage = 33;

    [Export]
    public float treeChopRate = 1f;

    [Signal]
    public delegate void OnSpawnEventHandler();

    public virtual void _OnSpawn()
    {
        EmitSignal(SignalName.OnSpawn, this);
    }
}
