using UnityEngine;

/// <summary>
/// This script should be attached to the skinLayer GameObject to handle scalpel collision detection
/// </summary>
public class SkinLayerTrigger : MonoBehaviour
{
    private HeartIncisionSystem incisionSystem;

    [Header("Debug Settings")]
    public bool enableDebugLogs = true;

    /// <summary>
    /// Initialize the trigger with reference to the main incision system
    /// </summary>
    /// <param name="system">Reference to the HeartIncisionSystem</param>
    public void Initialize(HeartIncisionSystem system)
    {
        incisionSystem = system;

        if (enableDebugLogs)
        {
            Debug.Log($"SkinLayerTrigger initialized on {gameObject.name}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"SkinLayer trigger entered by: {other.name} with tag: {other.tag}");
        }

        // Check if it's the scalpel
        if (other.CompareTag("Scalpel") && incisionSystem != null)
        {
            if (enableDebugLogs)
            {
                Debug.Log("Scalpel detected on skin layer! Initiating skin incision...");
            }

            // Notify the main system about the skin layer incision
            incisionSystem.OnSkinLayerIncision();
        }
        else if (!other.CompareTag("Scalpel"))
        {
            if (enableDebugLogs)
            {
                Debug.Log($"Non-scalpel object detected: {other.tag}");
            }
        }
        else if (incisionSystem == null)
        {
            Debug.LogError("SkinLayerTrigger: incisionSystem reference is null!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (enableDebugLogs && other.CompareTag("Scalpel"))
        {
            Debug.Log("Scalpel exited skin layer trigger");
        }
    }

    void OnDestroy()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"SkinLayerTrigger on {gameObject.name} is being destroyed");
        }
    }
}