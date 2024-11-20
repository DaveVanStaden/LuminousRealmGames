using UnityEngine;
using BehaviorTree;

public class CheckPlayerInRange : Node
{
    private Transform _transform;
    private ArrowBT _arrowBT;
    private float _maxDistanceFromWaypoint = 50.0f; // Maximum allowed distance from the current waypoint

    public CheckPlayerInRange(Transform transform, ArrowBT arrowBT)
    {
        _transform = transform;
        _arrowBT = arrowBT;
    }

    public override NodeState Evaluate()
    {
        // Check if the enemy is too far from the current waypoint
        if (_arrowBT.CurrentWaypoint != null)
        {
            float distanceFromWaypoint = Vector3.Distance(_transform.position, _arrowBT.CurrentWaypoint.position);
            if (distanceFromWaypoint > _maxDistanceFromWaypoint)
            {
                Debug.Log("Too far from waypoint, returning to scan area");
                return NodeState.FAILURE;
            }
        }

        // Assuming you have a method to get the player's position
        Transform player = GetPlayerTransform();
        if (player == null)
        {
            return NodeState.FAILURE;
        }

        if (Vector3.Distance(_transform.position, player.position) < ArrowBT.targetRange)
        {
            return NodeState.SUCCES;
        }
        return NodeState.FAILURE;
    }

    private Transform GetPlayerTransform()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        return playerObject != null ? playerObject.transform : null;
    }
}