using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Tree = BehaviorTree.Tree;


public class ArrowBT : Tree
{
    public Transform[] waypoints;
    
    public static float speed = 2f;
    public static float targetRange = 10f;
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform),
                new TaskGoToPlayer(transform),
            }),
            new TaskScanArea(transform, waypoints),
        });
        return root;
    }
}
