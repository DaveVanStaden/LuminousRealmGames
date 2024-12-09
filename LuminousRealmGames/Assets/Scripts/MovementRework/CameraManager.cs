using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    private PlayerInputManager inputManager;
    public Transform targetTransform;
    public Transform cameraPivot;
    public Transform cameraTransform;
    public LayerMask collisionLayers;
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraCollisionRadius;
    public float minimumCollisionOffSet;
    public float cameraCollisionOffSet; //how much the camera will jump off colliding object
    public float cameraFollowSpeed;
    public float cameraLookSpeed;
    public float cameraPivotSpeed;

    public float minimumPivotAngle;
    public float maximumPivotAngle;

    public float lookAngle;
    public float pivotAngle;

    public float cameraDistance = 10f; // New field to set the camera distance

    private void Awake()
    {
        inputManager = FindAnyObjectByType<PlayerInputManager>();
        targetTransform = FindAnyObjectByType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = -cameraDistance; // Set the default position based on the camera distance
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reinitialize necessary components after the scene is loaded
        inputManager = FindAnyObjectByType<PlayerInputManager>();
        targetTransform = FindAnyObjectByType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = -cameraDistance; // Set the default position based on the camera distance
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle += (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle -= (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit,
                Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition -= minimumCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}