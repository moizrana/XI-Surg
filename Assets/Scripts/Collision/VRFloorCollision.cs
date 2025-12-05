using UnityEngine;
using UnityEngine.XR;

public class VRPlayerCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    public float gravity = -9.81f;
    public LayerMask collisionLayers = -1; // All layers
    public bool enableWallPush = true;
    public float wallPushForce = 2f;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 lastPosition;
    private Transform cameraTransform;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        lastPosition = transform.position;

        // Find the VR camera
        cameraTransform = GetComponentInChildren<Camera>().transform;

        // Ensure we start above ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 5f, Vector3.down, out hit, 50f, collisionLayers))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
        }
    }

    void Update()
    {
        HandleGroundCollision();
        HandleCameraMovement();
        lastPosition = transform.position;
    }

    void HandleGroundCollision()
    {
        // Check if grounded
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to maintain ground contact
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move the character controller (this handles collision)
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleCameraMovement()
    {
        // This handles when player physically moves their head/body in VR
        if (cameraTransform != null)
        {
            Vector3 cameraOffset = cameraTransform.localPosition;
            cameraOffset.y = 0; // Don't move vertically with head movement

            // Try to move the character controller to follow camera movement
            CollisionFlags flags = characterController.Move(cameraOffset);

            // Reset camera position after moving character controller
            cameraTransform.localPosition = new Vector3(0, cameraTransform.localPosition.y, 0);

            // If we hit a wall, push back slightly
            if (enableWallPush && flags != CollisionFlags.None && flags != CollisionFlags.Below)
            {
                Vector3 pushDirection = (transform.position - lastPosition).normalized;
                if (pushDirection.magnitude > 0.1f)
                {
                    characterController.Move(-pushDirection * wallPushForce * Time.deltaTime);
                }
            }
        }
    }

    // This is called when Character Controller hits something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (showDebugInfo)
        {
            Debug.Log($"VR Player collided with: {hit.gameObject.name}");
        }

        // Optional: Add haptic feedback when hitting walls
        // XRController.SendHapticImpulse(0.3f, 0.1f); // Requires XR setup
    }

    void OnDrawGizmosSelected()
    {
        if (characterController != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 center = transform.position + characterController.center;

            // Simple wire cube to show collision bounds
            Vector3 size = new Vector3(characterController.radius * 2,
                                      characterController.height,
                                      characterController.radius * 2);
            Gizmos.DrawWireCube(center, size);

            // Show movement direction
            Gizmos.color = Color.blue;
            Vector3 movement = transform.position - lastPosition;
            if (movement.magnitude > 0.01f)
            {
                Gizmos.DrawRay(center, movement * 10f);
            }
        }
    }
}