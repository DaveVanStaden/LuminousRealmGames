using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class TaskScanArea : Node
{
    private Transform _transform;
    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;
    private ArrowBT _arrowBT;

    public TaskScanArea(Transform transform, Transform[] waypoints, ArrowBT arrowBT)
    {
        _transform = transform;
        _waypoints = waypoints;
        _arrowBT = arrowBT;
    }

    public override NodeState Evaluate()
    {
        if (_waypoints.Length == 0)
        {
            return NodeState.FAILURE;
        }

        Transform targetWaypoint = _waypoints[_currentWaypointIndex];
        _transform.position = Vector3.MoveTowards(_transform.position, targetWaypoint.position, ArrowBT.speed * Time.deltaTime);

        // Update the current waypoint in the ArrowBT script
        _arrowBT.CurrentWaypoint = targetWaypoint;

        if (Vector3.Distance(_transform.position, targetWaypoint.position) < 0.1f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            return NodeState.SUCCES;
        }

        return NodeState.RUNNING;
    }

    public Transform CurrentWaypoint()
    {
        return _waypoints[_currentWaypointIndex];
    }
}
