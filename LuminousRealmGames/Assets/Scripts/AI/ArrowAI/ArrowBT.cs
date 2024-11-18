using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Tree = BehaviorTree.Tree;

public class ArrowBT : Tree
{
    public Transform[] waypoints;
    public GameObject arrowPrefab;
    public Transform shootingPosition;
    public float attackRadius = 5f;
    public float attackCooldown = 2f;

    public static float speed = 4f;
    public static float followSpeed = 6f;
    public static float targetRange = 10f;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform),
                new TaskGoToPlayer(transform),
                new TaskAttack(transform, shootingPosition, arrowPrefab, attackRadius, attackCooldown),
            }),
            new TaskScanArea(transform, waypoints),
        });
        return root;
    }
}
