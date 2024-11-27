using UnityEngine;
using BehaviorTree;

public class TaskGoToPlayer : Node
{
    private Transform _transform;
    private ArrowBT _arrowBT;
    private float _maxDistanceFromWaypoint = 50.0f; // Maximum allowed distance from the current waypoint

    public TaskGoToPlayer(Transform transform, ArrowBT arrowBT)
    {
        _transform = transform;
        _arrowBT = arrowBT;
    }

    public override NodeState Evaluate()
    {
        Transform player = GetPlayerTransform();
        if (player == null)
        {
            return NodeState.FAILURE;
        }

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

        _transform.position = Vector3.MoveTowards(_transform.position, new Vector3(player.position.x, _transform.position.y, player.position.z), ArrowBT.followSpeed * Time.deltaTime);

        if (Vector3.Distance(_transform.position, player.position) < 0.1f)
        {
            return NodeState.SUCCES;
        }

        return NodeState.RUNNING;
    }

    private Transform GetPlayerTransform()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        return playerObject != null ? playerObject.transform : null;
    }
}