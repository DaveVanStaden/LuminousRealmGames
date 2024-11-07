using BehaviorTree;
using UnityEngine;

public class TaskScanArea : Node
{
   private Transform _transform;
   private Transform[] _waypoints;

   private int currentWaypointIndex = 0;

   private float waitTime = 2f;
   private float waitCounter = 0f;
   private bool waiting = false;
   public TaskScanArea(Transform transform, Transform[] waypoints)
   {
      _transform = transform;
      _waypoints = waypoints;
   }

   public override NodeState Evaluate()
   {
      if (waiting)
      {
         waitCounter += Time.deltaTime;
         if (waitCounter >= waitTime)
            waiting = false;
      }
      else
      {
         Transform wp = _waypoints[currentWaypointIndex];
         if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
         {
            _transform.position = wp.position;
            waitCounter = 0f;
            waiting = true;

            currentWaypointIndex = (currentWaypointIndex + 1) % _waypoints.Length;
         }
         else
         {
            _transform.position = Vector3.MoveTowards(_transform.position, wp.position, ArrowBT.speed * Time.deltaTime);
            _transform.LookAt(wp.position);
         }
      }

      state = NodeState.RUNNING;
      return state;
   }
}
