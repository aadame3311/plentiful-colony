using Godot;

public partial class Health : Node
{
    [Export]
    public double health = 100;
    public Interactable parent;

    [Signal]
    public delegate void OnHealthHitEventHandler();

    [Signal]
    public delegate void OnHealthDepleteEventHandler();


    public override void _Ready()
    {
        base._Ready();
        parent = GetParent<Interactable>();
        parent.OnHitDetected += (Node2D node) => { HealthHitHandler(node); };
    }

    public void HealthHitHandler(Node2D node)
    {
        if (node is Villager)
        {
            Villager villager = node as Villager;

            // safety check to ensure interactor is the same as the tree the villager is hitting
            if (villager.treeToChop == parent)
            {
                health -= villager.creatureData.creatureController.treeDamage;

                if (health <= 0)
                {
                    HealthDepleteHandler();
                }
            }
        }
    }

    public void HealthDepleteHandler()
    {
        if (health <= 0)
        {
            if (parent is Tree)
            {
                Tree treeNode = parent as Tree;
                treeNode.RemoveTree();
            }
        }
    }
}