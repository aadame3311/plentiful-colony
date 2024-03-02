using Godot;

public class Utils
{
  public static Tween createTween(Node parent)
  {
    Tween tween = parent.GetTree().CreateTween();
    return tween;
  }
}