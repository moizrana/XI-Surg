using UnityEngine;

/// <summary>
/// Component for VR tools that can perform stitching
/// Attach this to your needle or stitching tool object
/// </summary>
public class VRStitchTool : MonoBehaviour
{
    [Header("Tool Settings")]
    public ToolType toolType = ToolType.Needle;
    public bool isActivelyHeld = false;
    
    [Header("Haptic Feedback")]
    public bool enableHaptics = true;
    public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.1f;
    
    [Header("Visual Feedback")]
    public GameObject stitchingIndicator; // Optional visual indicator when near stitch site
    public Color readyToStitchColor = Color.green;
    public Color normalColor = Color.white;
    
    // Private variables
    private Renderer toolRenderer;
    private bool isNearStitchSite = false;
    
    // Events
    public System.Action<StitchSite> OnStitchPerformed;
    
    public enum ToolType
    {
        Needle,
        Suture,
        Other
    }
    
    private void Awake()
    {
        toolRenderer = GetComponent<Renderer>();
    }
    
    private void Start()
    {
        // Set initial color
        if (toolRenderer != null)
        {
            toolRenderer.material.color = normalColor;
        }
    }
    
    /// <summary>
    /// Called when the tool is grabbed by the player
    /// </summary>
    public void OnGrabbed()
    {
        isActivelyHeld = true;
        
        // Enable any visual indicators
        if (stitchingIndicator != null)
        {
            stitchingIndicator.SetActive(true);
        }
    }
    
    /// <summary>
    /// Called when the tool is released by the player
    /// </summary>
    public void OnReleased()
    {
        isActivelyHeld = false;
        
        // Disable visual indicators
        if (stitchingIndicator != null)
        {
            stitchingIndicator.SetActive(false);
        }
        
        // Reset color
        if (toolRenderer != null)
        {
            toolRenderer.material.color = normalColor;
        }
    }
    
    /// <summary>
    /// Called when near a stitch site
    /// </summary>
    public void SetNearStitchSite(bool near)
    {
        isNearStitchSite = near;
        
        if (toolRenderer != null)
        {
            toolRenderer.material.color = near ? readyToStitchColor : normalColor;
        }
        
        // Provide haptic feedback if enabled
        if (near && enableHaptics && isActivelyHeld)
        {
            TriggerHapticFeedback();
        }
    }
    
    /// <summary>
    /// Trigger haptic feedback (requires XR controller integration)
    /// </summary>
    private void TriggerHapticFeedback()
    {
        // This would need to be implemented based on your VR framework
        // For example, with XR Interaction Toolkit:
        /*
        if (isActivelyHeld)
        {
            var controller = GetComponentInParent<XRBaseController>();
            if (controller != null)
            {
                controller.SendHapticImpulse(hapticIntensity, hapticDuration);
            }
        }
        */
        
        Debug.Log($"Haptic feedback triggered on {gameObject.name}");
    }
    
    /// <summary>
    /// Check if this tool can perform stitching
    /// </summary>
    public bool CanStitch()
    {
        return isActivelyHeld && toolType == ToolType.Needle;
    }
}
