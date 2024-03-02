using Godot;

[GlobalClass]
public partial class VillagerController : CreatureController
{
    [Export]
    public Gender gender;

    public override void _OnSpawn()
    {
        base._OnSpawn();
    }
}