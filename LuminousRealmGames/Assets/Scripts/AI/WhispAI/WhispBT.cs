using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Tree = BehaviorTree.Tree;

public class WhispBT : Tree
{
    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new TaskGuidePlayer(),
            }),
            new TaskIdle(),
        });
        return root;
    }
}
