using Godot;

public partial class ShakeAnimation : Node
{
  [Export]
  public bool pivotBelow = true;
  double xMax = 0.5;
  double rMax = 10;

  const double STOP_THRESHOLD = 0.1;
  const double TWEEN_DURATION = 0.05;
  const double RECOVERY_FACTOR = 2.0 / 3;
  const Tween.TransitionType TRANSITION_TYPE = Tween.TransitionType.Sine;

  Sprite2D host;


  public override void _Ready()
  {
    base._Ready();

    Node2D parent = GetParent<Node2D>();
    if (IsInstanceValid(parent))
    {
      host = parent.GetNode<Sprite2D>("Sprite2D");
    }
  }
  public void Startt(double x, double r) { }
  private void _TiltRight(double x, double r) { }
  private void _TiltLeft(double x, double r) { }
}