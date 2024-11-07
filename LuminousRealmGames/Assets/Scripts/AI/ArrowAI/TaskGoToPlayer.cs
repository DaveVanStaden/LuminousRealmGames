using BehaviorTree;
using UnityEngine;

public class TaskGoToPlayer : Node
{
    private Transform _transform;

    public TaskGoToPlayer(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform) GetData("target");
        if (Vector3.Distance(_transform.position, target.position) > 0.01f)
        {
            _transform.position = Vector3.MoveTowards(
                _transform.position, target.position, ArrowBT.speed * Time.deltaTime);
            _transform.LookAt(target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
