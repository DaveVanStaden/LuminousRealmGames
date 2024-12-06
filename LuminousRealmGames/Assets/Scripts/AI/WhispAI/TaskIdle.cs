using BehaviorTree;
using UnityEngine;

public class TaskIdle : Node
{
    public override NodeState Evaluate()
    {
        return NodeState.FAILURE;
    }
}
