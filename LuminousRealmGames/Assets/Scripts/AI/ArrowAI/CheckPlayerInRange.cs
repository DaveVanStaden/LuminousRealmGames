using BehaviorTree;
using UnityEngine;

public class CheckPlayerInRange : Node
{
    private static LayerMask _playerLayerMask = 6;
    private Transform _transform;

    public CheckPlayerInRange(Transform transform)
    {
        _transform = transform;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(
                   _transform.position,ArrowBT.targetRange , _playerLayerMask);

            if (colliders.Length > 0)
            {
                parent.parent.SetData("target", colliders[0].transform);
                
                state = NodeState.SUCCES;
                return state;
            }
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCES;
        return state;
    }
}
