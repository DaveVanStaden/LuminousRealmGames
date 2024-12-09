using BehaviorTree;
using UnityEngine;

public class TaskGuidePlayer : Node
{
    public override NodeState Evaluate()
    {
        return NodeState.FAILURE;
    }
}
