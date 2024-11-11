/* using BehaviorTree;
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
*/
using UnityEngine;
using BehaviorTree;

public class TaskGoToPlayer : Node
{
    private Transform _transform;

    public TaskGoToPlayer(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        Transform player = GetPlayerTransform();
        if (player == null)
        {
            return NodeState.FAILURE;
        }

        _transform.position = Vector3.MoveTowards(_transform.position, new Vector3(player.position.x,_transform.position.y, player.position.z), ArrowBT.followSpeed * Time.deltaTime);

        if (Vector3.Distance(_transform.position, player.position) < 0.1f)
        {
            return NodeState.SUCCES;
        }

        return NodeState.RUNNING;
    }

    private Transform GetPlayerTransform()
    {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        return playerTransform;
    }
}