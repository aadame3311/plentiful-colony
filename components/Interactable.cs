using Godot;

public partial class Interactable : Node2D
{
    [Signal]
    public delegate void OnHitDetectedEventHandler(Node2D node);
}