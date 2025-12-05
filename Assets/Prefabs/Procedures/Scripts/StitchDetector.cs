using UnityEngine;

/// <summary>
/// Detector component that handles trigger events for stitch sites
/// Attached to the detection colliders created by StitchSite
/// </summary>
public class StitchDetector : MonoBehaviour
{
    [HideInInspector]
    public StitchSite parentStitchSite;
    
    [HideInInspector]
    public LayerMask toolLayerMask = -1;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is on the correct layer
        if ((toolLayerMask.value & (1 << other.gameObject.layer)) == 0)
            return;
            
        // Notify parent stitch site
        if (parentStitchSite != null)
        {
            parentStitchSite.OnToolDetected(other.gameObject);
        }
    }
}
