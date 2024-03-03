using System;
using Godot;

public class Utils
{
  public static Tween createTween(Node parent)
  {
    Tween tween = parent.GetTree().CreateTween();
    return tween;
  }

  public static Timer createTimer(Node parent, double waitTime, bool oneShot)
  {
    Timer timer = new Timer
    {
      OneShot = oneShot,
      WaitTime = waitTime
    };

    parent.AddChild(timer);
    return timer;
  }
}