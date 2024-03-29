using System;
using Godot;

public partial class Villager : CharacterBody2D
{
    [Export]
    public CreatureData creatureData;

    [Export]
    public NavigationAgent2D navigationAgent;

    [Export]
    public RayCast2D raycast;

    [Signal]
    public delegate void OnStateChangeEventHandler(Villager villager);

    public Main world;

    public Tree treeToChop;

    public Area2D hitDetectionArea;

    public int woodCount = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        world = GetParent<Main>();
        raycast.Enabled = true;
        Sprite2D sprite = GetNode<Sprite2D>("%Sprite2D");
        sprite.Texture = creatureData.sprite;

        (creatureData.creatureController as VillagerController)._OnSpawn();

        if (world == null)
        {
            GD.PushError("Villager needs world of type `Main` as a parent Node.");
        }

        world.OnWorldEnvironmentChange += (Node e) =>
        {
            if (e is Tree)
            {
                HandleOnTreeChanges();
            }
        };

        navigationAgent.PathDesiredDistance = (float)16.0;
    }

    public void HandleOnTreeChanges()
    {
        EmitSignal(SignalName.OnStateChange, this);
    }
}
