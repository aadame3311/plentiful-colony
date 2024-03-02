using Godot;
using System;

[GlobalClass]
public partial class CreatureData : Resource
{
    [Export]
    public CreatureType type;
    [Export]
    public Texture2D sprite;
    [Export]
    public CreatureController creatureController;
}
