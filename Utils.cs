using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public class Utils
{
    public static Tween createTween(Node parent)
    {
        Tween tween = parent.GetTree().CreateTween();
        return tween;
    }

    public static Timer createTimer(Node parent, double waitTime, bool oneShot)
    {
        Timer timer = new Timer { OneShot = oneShot, WaitTime = waitTime };

        parent.AddChild(timer);
        return timer;
    }

    public static Node2D FindClosestDistanceToTarget(Node2D source, Array<Node2D> nodes)
    {
        Node2D closestNode = nodes[0];
        Vector2 closestNodeDistanceVector = closestNode.Position;

        float closestNodeDistance = source.Position.DistanceTo(closestNodeDistanceVector);

        foreach (Node2D node in nodes)
        {
            Vector2 currentVector = node.Position;
            float currentVectorDistanceToSource = source.Position.DistanceTo(currentVector);

            if (
                currentVectorDistanceToSource < closestNodeDistance
                && Node.IsInstanceValid(node)
                && !node.IsQueuedForDeletion()
            )
            {
                closestNodeDistance = currentVectorDistanceToSource;
                closestNodeDistanceVector = currentVector;
                closestNode = node;
            }
        }

        return closestNode;
    }

    // Moves a villager along their current navigation path
    public static void MoveVillagerAlongNavigationPath(Villager villager)
    {
        Vector2 direction = villager.navigationAgent.GetNextPathPosition();
        Vector2 newVelocity = (direction - villager.GlobalPosition).Normalized();

        newVelocity *= villager.creatureData.creatureController.speed;
        villager.Velocity = newVelocity;

        villager.MoveAndSlide();
    }
}
