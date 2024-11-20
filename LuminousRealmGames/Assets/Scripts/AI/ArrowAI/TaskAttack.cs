using UnityEngine;
using BehaviorTree;

public class TaskAttack : Node
{
    private Transform _transform;
    private Transform _shootingPosition;
    private GameObject _arrowPrefab;
    private float _attackRadius;
    private float _arrowSpeed = 2;
    private float _cooldownTime;
    private float _lastShotTime;
    private int _arrowCount = 6; // Number of arrows to shoot
    private float _spreadRadius = 1.0f; // Radius for spread around the target
    private float _speedVariation = 0.5f; // Variation in arrow speed

    public TaskAttack(Transform transform, Transform shootingPosition, GameObject arrowPrefab, float attackRadius, float cooldownTime)
    {
        _transform = transform;
        _shootingPosition = shootingPosition;
        _arrowPrefab = arrowPrefab;
        _attackRadius = attackRadius;
        _cooldownTime = cooldownTime;
        _lastShotTime = -cooldownTime; // Initialize to allow immediate first shot
    }

    public override NodeState Evaluate()
    {
        Transform player = GetPlayerTransform();
        if (player == null)
        {
            Debug.Log("Player not found");
            return NodeState.FAILURE;
        }

        float distanceToPlayer = Vector3.Distance(_transform.position, player.position);
        Debug.Log($"Distance to player: {distanceToPlayer}");

        if (distanceToPlayer <= _attackRadius)
        {
            if (Time.time - _lastShotTime >= _cooldownTime)
            {
                Debug.Log("Attacking player");
                AttackPlayer(player);
                _lastShotTime = Time.time;
                return NodeState.SUCCES;
            }
            Debug.Log("Cooldown in progress");
            return NodeState.RUNNING; // Cooldown in progress
        }

        Debug.Log("Player out of range");
        return NodeState.FAILURE;
    }

    private Transform GetPlayerTransform()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        return playerObject != null ? playerObject.transform : null;
    }

    private void AttackPlayer(Transform player)
    {
        for (int i = 0; i < _arrowCount; i++)
        {
            GameObject arrow = GameObject.Instantiate(_arrowPrefab, _shootingPosition.position, Quaternion.identity);
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            Vector3 targetPosition = GetPositionAroundPlayer(player.position, i, _arrowCount, _spreadRadius);
            Vector3 direction = (targetPosition - _shootingPosition.position).normalized;

            float distance = Vector3.Distance(_shootingPosition.position, targetPosition);
            float speed = (distance / 1.0f) * (_arrowSpeed + Random.Range(-_speedVariation, _speedVariation)); // Adjust the speed with variation

            arrow.GetComponent<RotateArrow>().target = player;

            rb.linearVelocity = direction * speed;
            rb.angularVelocity = Vector3.zero; // Ensure no initial rotation

            Debug.Log($"Arrow {i + 1} shot towards {targetPosition} with velocity {rb.linearVelocity}");
        }
    }

    private Vector3 GetPositionAroundPlayer(Vector3 playerPosition, int index, int totalArrows, float radius)
    {
        float angle = index * Mathf.PI * 2 / totalArrows;
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 offset = new Vector3(x, 0, z);
        return playerPosition + offset;
    }
}