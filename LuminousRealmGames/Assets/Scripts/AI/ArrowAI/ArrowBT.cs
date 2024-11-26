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

    public static float speed = 5f;
    public Transform CurrentWaypoint { get; set; }
    public static float followSpeed = 10f;
    public static float targetRange = 15f;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInRange(transform, this),
                new TaskGoToPlayer(transform, this),
                new TaskAttack(transform, shootingPosition, arrowPrefab, attackRadius, attackCooldown),
            }),
            new TaskScanArea(transform, waypoints, this),
        });
        return root;
    }
}
