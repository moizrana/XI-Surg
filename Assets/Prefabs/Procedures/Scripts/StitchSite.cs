using UnityEngine;
using System.Collections;

/// <summary>
/// Streamlined stitch site with two connection points and skin deformation animation
/// Handles proximity detection and thread creation when a VR tool comes near
/// Now includes StitchVisualizer integration to show final stitch appearance
/// </summary>
public class StitchSite : MonoBehaviour
{
    [Header("Stitch Points")]
    public Transform firstStitch;
    public Transform secondStitch;

    [Header("Detection Settings")]
    public float detectionRadius = 0.05f; // 5cm detection radius
    public LayerMask toolLayerMask = -1; // What layers count as tools

    [Header("Thread Settings")]
    public Material threadMaterial;
    public float threadWidth = 0.002f; // 2mm thread width
    public float threadHeightOffset = 0.01f; // 1cm above the stitch points
    public AnimationCurve threadCreationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float threadCreationDuration = 1.0f;

    [Header("Skin Deformation Settings")]
    public Transform leftSkinObject; // Left skin segment
    public Transform rightSkinObject; // Right skin segment

    [Header("Skin Initial Positioning")]
    [Tooltip("Offset from the first stitch where left skin starts")]
    public Vector3 leftSkinStartOffset = new Vector3(-0.02f, 0, 0); // 2cm to the left
    [Tooltip("Offset from the second stitch where right skin starts")]
    public Vector3 rightSkinStartOffset = new Vector3(0.02f, 0, 0); // 2cm to the right
    [Tooltip("Make skins invisible at start")]
    public bool hideSkinInitially = true;

    [Header("Skin Slide Animation")]
    public float skinSlideToStitchDuration = 0.8f; // Duration for sliding to stitch
    public AnimationCurve skinSlideToStitchCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector3 skinToStitchOffset = new Vector3(0, -0.005f, 0); // Slightly below the stitch

    [Header("Skin Animation")]
    public float skinAnimationDuration = 0.5f; // Duration for skin movement animation
    public AnimationCurve skinMovementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Range(0f, 1f)]
    public float skinPullStrength = 0.3f; // How much the skins get pulled together (0-1)

    [Header("Stitch Visualizer")]
    [Tooltip("The StitchVisualizer object that shows the final stitch appearance")]
    public GameObject stitchVisualizer;
    [Tooltip("Delay before showing the stitch visualizer after skin animation completes")]
    public float visualizerShowDelay = 0.1f;
    [Tooltip("Duration for fading in the stitch visualizer")]
    public float visualizerFadeInDuration = 0.5f;
    [Tooltip("Animation curve for visualizer fade-in")]
    public AnimationCurve visualizerFadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Timing Settings")]
    public float stitchCooldownTime = 2.0f; // 2 second cooldown between stitches

    [Header("Audio")]
    public AudioClip stitchCompleteSound;

    // Private variables
    private LineRenderer threadLineRenderer;
    private bool isStitched = false;
    private bool isCreatingThread = false;
    private AudioSource audioSource;
    private StitchManager stitchManager;

    // Skin positioning tracking
    private Vector3 leftSkinFinalPosition; // Where left skin should end up (current Inspector position)
    private Vector3 rightSkinFinalPosition; // Where right skin should end up (current Inspector position)
    private Vector3 leftSkinStartPosition; // Calculated start position
    private Vector3 rightSkinStartPosition; // Calculated start position

    // Track which skins have been activated
    private bool leftSkinActivated = false;
    private bool rightSkinActivated = false;
    private bool isSliding = false; // Track if skins are currently sliding

    // StitchVisualizer management
    private Renderer[] visualizerRenderers; // Cache renderers for fade effect
    private bool visualizerShown = false;

    // Events
    public System.Action<StitchSite> OnStitchCompleted;

    private void Awake()
    {
        // Validate stitch points
        if (firstStitch == null || secondStitch == null)
        {
            Debug.LogError($"StitchSite {gameObject.name} is missing stitch point references!");
            return;
        }

        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Find stitch manager
        stitchManager = FindObjectOfType<StitchManager>();

        // Store the current positions as the final positions (where they should end up)
        if (leftSkinObject != null)
            leftSkinFinalPosition = leftSkinObject.position;
        if (rightSkinObject != null)
            rightSkinFinalPosition = rightSkinObject.position;

        // Calculate start positions based on offsets from stitch points
        leftSkinStartPosition = firstStitch.position + leftSkinStartOffset;
        rightSkinStartPosition = secondStitch.position + rightSkinStartOffset;

        // Check if skins are manually disabled and update hideSkinInitially accordingly
        bool leftSkinDisabled = leftSkinObject != null && !leftSkinObject.gameObject.activeInHierarchy;
        bool rightSkinDisabled = rightSkinObject != null && !rightSkinObject.gameObject.activeInHierarchy;

        if (leftSkinDisabled || rightSkinDisabled)
        {
            hideSkinInitially = true;
            Debug.Log($"{gameObject.name}: Detected manually disabled skins. Set hideSkinInitially to true.");
        }

        // Initialize stitch visualizer
        InitializeStitchVisualizer();

        // Initialize skin positions and visibility
        InitializeSkinPositions();
    }

    private void InitializeStitchVisualizer()
    {
        if (stitchVisualizer == null)
        {
            Debug.LogWarning($"{gameObject.name}: No StitchVisualizer assigned!");
            return;
        }

        // Cache all renderers in the visualizer for fade effects
        visualizerRenderers = stitchVisualizer.GetComponentsInChildren<Renderer>();

        // Hide the visualizer initially
        stitchVisualizer.SetActive(false);
        visualizerShown = false;

        Debug.Log($"{gameObject.name}: StitchVisualizer initialized with {visualizerRenderers.Length} renderers");
    }

    private void InitializeSkinPositions()
    {
        // Move skins to their starting positions
        if (leftSkinObject != null)
        {
            leftSkinObject.position = leftSkinStartPosition;
        }
        if (rightSkinObject != null)
        {
            rightSkinObject.position = rightSkinStartPosition;
        }

        // Reset activation flags
        leftSkinActivated = false;
        rightSkinActivated = false;
        isSliding = false;

        // Handle initial visibility
        if (hideSkinInitially)
        {
            if (leftSkinObject != null)
                leftSkinObject.gameObject.SetActive(false);
            if (rightSkinObject != null)
                rightSkinObject.gameObject.SetActive(false);
        }

        Debug.Log($"{gameObject.name}: Skins initialized to start positions. Hidden: {hideSkinInitially}");
    }

    private void Start()
    {
        // Create sphere colliders for detection if they don't exist
        CreateDetectionColliders();
    }

    private void CreateDetectionColliders()
    {
        // Create detection collider at FirstStitch
        CreateDetectionColliderAt(firstStitch, "FirstStitchDetector");

        // Create detection collider at SecondStitch  
        CreateDetectionColliderAt(secondStitch, "SecondStitchDetector");
    }

    private void CreateDetectionColliderAt(Transform stitchPoint, string colliderName)
    {
        GameObject detectorObj = new GameObject(colliderName);
        detectorObj.transform.SetParent(stitchPoint);
        detectorObj.transform.localPosition = Vector3.zero;

        SphereCollider detector = detectorObj.AddComponent<SphereCollider>();
        detector.radius = detectionRadius;
        detector.isTrigger = true;

        // Add the detector component
        StitchDetector detectorScript = detectorObj.AddComponent<StitchDetector>();
        detectorScript.parentStitchSite = this;
        detectorScript.toolLayerMask = toolLayerMask;
    }

    /// <summary>
    /// Called by StitchDetector when a tool enters the detection zone
    /// </summary>
    public void OnToolDetected(GameObject tool)
    {
        // Check if tool is valid (has VRStitchTool component)
        VRStitchTool stitchTool = tool.GetComponent<VRStitchTool>();
        if (stitchTool == null)
            return;

        // Determine which stitch was detected based on tool position
        Transform detectedStitch = GetClosestStitch(tool.transform.position);

        // Show skin for the detected stitch
        ShowSkinForStitch(detectedStitch);

        // Check if both skins are activated and not currently sliding
        if (leftSkinActivated && rightSkinActivated && !isSliding)
        {
            StartStitchingProcess();
        }
    }

    private Transform GetClosestStitch(Vector3 toolPosition)
    {
        float distanceToFirst = Vector3.Distance(toolPosition, firstStitch.position);
        float distanceToSecond = Vector3.Distance(toolPosition, secondStitch.position);

        return distanceToFirst <= distanceToSecond ? firstStitch : secondStitch;
    }

    private void ShowSkinForStitch(Transform detectedStitch)
    {
        if (detectedStitch == firstStitch && leftSkinObject != null && !leftSkinActivated)
        {
            if (hideSkinInitially && !leftSkinObject.gameObject.activeInHierarchy)
            {
                leftSkinObject.gameObject.SetActive(true);
            }
            leftSkinActivated = true;
            Vector3 targetPos = firstStitch.position + skinToStitchOffset;
            StartCoroutine(SlideSkinToStitch(leftSkinObject, leftSkinStartPosition, targetPos, true));
            Debug.Log($"Sliding left skin object to first stitch for {gameObject.name}");
        }
        else if (detectedStitch == secondStitch && rightSkinObject != null && !rightSkinActivated)
        {
            if (hideSkinInitially && !rightSkinObject.gameObject.activeInHierarchy)
            {
                rightSkinObject.gameObject.SetActive(true);
            }
            rightSkinActivated = true;
            Vector3 targetPos = secondStitch.position + skinToStitchOffset;
            StartCoroutine(SlideSkinToStitch(rightSkinObject, rightSkinStartPosition, targetPos, false));
            Debug.Log($"Sliding right skin object to second stitch for {gameObject.name}");
        }
    }

    private System.Collections.IEnumerator SlideSkinToStitch(Transform skinObject, Vector3 startPos, Vector3 targetPos, bool isLeftSkin)
    {
        if (skinObject == null) yield break;

        isSliding = true;

        // Ensure skin starts at the correct position
        skinObject.position = startPos;

        float elapsedTime = 0f;

        while (elapsedTime < skinSlideToStitchDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = skinSlideToStitchCurve.Evaluate(elapsedTime / skinSlideToStitchDuration);

            if (skinObject != null)
            {
                skinObject.position = Vector3.Lerp(startPos, targetPos, progress);
            }

            yield return null;
        }

        // Ensure exact final position
        if (skinObject != null)
        {
            skinObject.position = targetPos;
        }

        // Check if both skins have finished sliding
        if (leftSkinActivated && rightSkinActivated)
        {
            isSliding = false;
            // Small delay before starting stitching process
            yield return new WaitForSeconds(0.2f);

            // Check if we should start stitching (both skins activated and not already stitching)
            if (!isStitched && !isCreatingThread)
            {
                StartStitchingProcess();
            }
        }
        else
        {
            isSliding = false;
        }
    }

    private void StartStitchingProcess()
    {
        // First check: if already stitched, never allow stitching again
        if (isStitched)
        {
            Debug.Log($"{gameObject.name} is already stitched. Ignoring tool detection.");
            return;
        }

        // Second check: if currently creating thread, don't allow another
        if (isCreatingThread)
        {
            Debug.Log($"{gameObject.name} is currently being stitched. Ignoring tool detection.");
            return;
        }

        // Check if stitching is allowed (cooldown check)
        if (stitchManager != null && !stitchManager.IsStitchingAllowed())
        {
            float remainingCooldown = stitchManager.GetRemainingCooldown();
            Debug.Log($"Stitching blocked. Wait {remainingCooldown:F1} more seconds or another stitch is in progress.");
            return;
        }

        // Mark stitching as in progress to prevent simultaneous stitches
        if (stitchManager != null)
        {
            stitchManager.SetStitchingInProgress(true);
        }

        // Start thread creation with skin animation
        StartCoroutine(CreateThreadWithSkinAnimation());
    }

    private System.Collections.IEnumerator CreateThreadWithSkinAnimation()
    {
        isCreatingThread = true;

        // Create LineRenderer for the thread
        if (threadLineRenderer == null)
        {
            GameObject threadObj = new GameObject($"Thread_{gameObject.name}");
            threadObj.transform.SetParent(transform);
            threadLineRenderer = threadObj.AddComponent<LineRenderer>();

            // Configure LineRenderer
            threadLineRenderer.material = threadMaterial;
            threadLineRenderer.startWidth = threadWidth;
            threadLineRenderer.endWidth = threadWidth;
            threadLineRenderer.positionCount = 2;
            threadLineRenderer.useWorldSpace = true;
        }

        // Validate that we have all required components before proceeding
        if (threadLineRenderer == null || firstStitch == null || secondStitch == null)
        {
            Debug.LogError($"StitchSite {gameObject.name} is missing required components for thread creation!");
            CleanupOnError();
            yield break;
        }

        // Start skin animation simultaneously with thread creation
        Coroutine skinCoroutine = StartCoroutine(AnimateSkinDeformation());

        // Animate thread creation - growing from first stitch to second stitch
        float elapsedTime = 0f;
        Vector3 startPos = firstStitch.position + Vector3.up * threadHeightOffset;
        Vector3 endPos = secondStitch.position + Vector3.up * threadHeightOffset;

        // Initially, both points start at the first stitch (zero-length line)
        threadLineRenderer.SetPosition(0, startPos);
        threadLineRenderer.SetPosition(1, startPos);

        while (elapsedTime < threadCreationDuration)
        {
            // Safety check - if components were destroyed during animation
            if (threadLineRenderer == null)
            {
                Debug.LogWarning($"Thread renderer was destroyed during animation on {gameObject.name}");
                CleanupOnError();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float progress = threadCreationCurve.Evaluate(elapsedTime / threadCreationDuration);

            // First position always stays at first stitch
            // Second position grows from first stitch toward second stitch
            threadLineRenderer.SetPosition(0, startPos);
            threadLineRenderer.SetPosition(1, Vector3.Lerp(startPos, endPos, progress));

            yield return null;
        }

        // Ensure final position is exact
        if (threadLineRenderer != null)
        {
            threadLineRenderer.SetPosition(0, startPos);
            threadLineRenderer.SetPosition(1, endPos);
        }

        // Wait for skin animation to complete
        yield return skinCoroutine;

        // Show the StitchVisualizer after skin animation completes
        yield return StartCoroutine(ShowStitchVisualizer());

        // Mark as stitched
        isStitched = true;
        isCreatingThread = false;

        // Disable detection colliders to prevent further stitching
        DisableDetectionColliders();

        // Play completion sound
        if (stitchCompleteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(stitchCompleteSound);
        }

        // Clear stitching in progress
        if (stitchManager != null)
        {
            stitchManager.SetStitchingInProgress(false);
        }

        // Notify completion
        OnStitchCompleted?.Invoke(this);
        stitchManager?.OnStitchSiteCompleted(this);
    }

    private System.Collections.IEnumerator AnimateSkinDeformation()
    {
        if (leftSkinObject == null || rightSkinObject == null)
            yield break;

        // Use current positions as starting points (after the slide animation)
        Vector3 leftStartPos = leftSkinObject.position;
        Vector3 rightStartPos = rightSkinObject.position;

        // Calculate the center point between the two stitch points
        Vector3 centerPoint = (firstStitch.position + secondStitch.position) * 0.5f;

        // Calculate target positions - skins move toward each other
        Vector3 leftTargetPos = Vector3.Lerp(leftStartPos, centerPoint, skinPullStrength);
        Vector3 rightTargetPos = Vector3.Lerp(rightStartPos, centerPoint, skinPullStrength);

        // First phase: Pull skins toward each other (simulating tissue being pulled together)
        float elapsedTime = 0f;
        while (elapsedTime < skinAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = skinMovementCurve.Evaluate(elapsedTime / skinAnimationDuration);

            if (leftSkinObject != null)
                leftSkinObject.position = Vector3.Lerp(leftStartPos, leftTargetPos, progress);

            if (rightSkinObject != null)
                rightSkinObject.position = Vector3.Lerp(rightStartPos, rightTargetPos, progress);

            yield return null;
        }

        // Ensure exact positions
        if (leftSkinObject != null)
            leftSkinObject.position = leftTargetPos;
        if (rightSkinObject != null)
            rightSkinObject.position = rightTargetPos;

        // Small delay before final positioning
        yield return new WaitForSeconds(0.2f);

        // Second phase: Move skins to their final positions (where they were originally placed in Inspector)
        elapsedTime = 0f;
        Vector3 leftCurrentPos = leftSkinObject.position;
        Vector3 rightCurrentPos = rightSkinObject.position;

        while (elapsedTime < skinAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = skinMovementCurve.Evaluate(elapsedTime / skinAnimationDuration);

            if (leftSkinObject != null)
                leftSkinObject.position = Vector3.Lerp(leftCurrentPos, leftSkinFinalPosition, progress);

            if (rightSkinObject != null)
                rightSkinObject.position = Vector3.Lerp(rightCurrentPos, rightSkinFinalPosition, progress);

            yield return null;
        }

        // Ensure final positions are exact
        if (leftSkinObject != null)
            leftSkinObject.position = leftSkinFinalPosition;
        if (rightSkinObject != null)
            rightSkinObject.position = rightSkinFinalPosition;
    }

    /// <summary>
    /// Show the StitchVisualizer with optional fade-in animation
    /// </summary>
    private System.Collections.IEnumerator ShowStitchVisualizer()
    {
        if (stitchVisualizer == null || visualizerShown)
            yield break;

        // Wait for the specified delay
        yield return new WaitForSeconds(visualizerShowDelay);

        // Activate the visualizer
        stitchVisualizer.SetActive(true);
        visualizerShown = true;

        Debug.Log($"{gameObject.name}: StitchVisualizer activated");

        // If we have cached renderers and want a fade-in effect
        if (visualizerRenderers != null && visualizerRenderers.Length > 0 && visualizerFadeInDuration > 0)
        {
            // Start with transparent materials
            foreach (var renderer in visualizerRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    // Check if material supports transparency
                    if (renderer.material.HasProperty("_Color"))
                    {
                        Color color = renderer.material.color;
                        color.a = 0f;
                        renderer.material.color = color;
                    }
                }
            }

            // Fade in over time
            float elapsedTime = 0f;
            while (elapsedTime < visualizerFadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = visualizerFadeInCurve.Evaluate(elapsedTime / visualizerFadeInDuration);

                foreach (var renderer in visualizerRenderers)
                {
                    if (renderer != null && renderer.material != null && renderer.material.HasProperty("_Color"))
                    {
                        Color color = renderer.material.color;
                        color.a = progress;
                        renderer.material.color = color;
                    }
                }

                yield return null;
            }

            // Ensure full opacity
            foreach (var renderer in visualizerRenderers)
            {
                if (renderer != null && renderer.material != null && renderer.material.HasProperty("_Color"))
                {
                    Color color = renderer.material.color;
                    color.a = 1f;
                    renderer.material.color = color;
                }
            }
        }

        Debug.Log($"{gameObject.name}: StitchVisualizer fade-in completed");
    }

    /// <summary>
    /// Hide the StitchVisualizer (used when resetting)
    /// </summary>
    private void HideStitchVisualizer()
    {
        if (stitchVisualizer != null)
        {
            stitchVisualizer.SetActive(false);
            visualizerShown = false;
            Debug.Log($"{gameObject.name}: StitchVisualizer hidden");
        }
    }

    /// <summary>
    /// Clean up when an error occurs during thread creation
    /// </summary>
    private void CleanupOnError()
    {
        isCreatingThread = false;
        if (stitchManager != null)
        {
            stitchManager.SetStitchingInProgress(false);
        }
    }

    /// <summary>
    /// Disable detection colliders to prevent further stitching
    /// </summary>
    private void DisableDetectionColliders()
    {
        // Find and disable all StitchDetector components in children
        StitchDetector[] detectors = GetComponentsInChildren<StitchDetector>();
        foreach (StitchDetector detector in detectors)
        {
            if (detector != null && detector.gameObject != null)
            {
                detector.gameObject.SetActive(false);
                Debug.Log($"Disabled detection collider for {gameObject.name}");
            }
        }
    }

    /// <summary>
    /// Enable detection colliders (used when resetting)
    /// </summary>
    private void EnableDetectionColliders()
    {
        // Find and enable all StitchDetector components in children
        StitchDetector[] detectors = GetComponentsInChildren<StitchDetector>(true); // Include inactive
        foreach (StitchDetector detector in detectors)
        {
            if (detector != null && detector.gameObject != null)
            {
                detector.gameObject.SetActive(true);
                Debug.Log($"Enabled detection collider for {gameObject.name}");
            }
        }
    }

    /// <summary>
    /// Check if this stitch site has been completed
    /// </summary>
    public bool IsStitched => isStitched;

    /// <summary>
    /// Check if the StitchVisualizer is currently shown
    /// </summary>
    public bool IsVisualizerShown => visualizerShown;

    /// <summary>
    /// Reset the stitch site (for restarting the procedure)
    /// </summary>
    public void ResetStitch()
    {
        isStitched = false;
        isCreatingThread = false;

        // Re-enable detection colliders
        EnableDetectionColliders();

        // Hide the StitchVisualizer
        HideStitchVisualizer();

        // Reset skin positions to start positions and hide them again
        InitializeSkinPositions();

        if (threadLineRenderer != null)
        {
            DestroyImmediate(threadLineRenderer.gameObject);
            threadLineRenderer = null;
        }

        Debug.Log($"{gameObject.name} reset to initial state");
    }

    // Helper methods for editor use
    [ContextMenu("Preview Start Positions")]
    private void PreviewStartPositions()
    {
        if (Application.isPlaying) return;

        if (firstStitch != null && secondStitch != null)
        {
            leftSkinStartPosition = firstStitch.position + leftSkinStartOffset;
            rightSkinStartPosition = secondStitch.position + rightSkinStartOffset;

            if (leftSkinObject != null)
                leftSkinObject.position = leftSkinStartPosition;
            if (rightSkinObject != null)
                rightSkinObject.position = rightSkinStartPosition;

            Debug.Log($"Preview: Moved skins to start positions");
        }
    }

    [ContextMenu("Reset To Final Positions")]
    private void ResetToFinalPositions()
    {
        if (Application.isPlaying) return;

        if (leftSkinObject != null && rightSkinObject != null)
        {
            // Store current positions as final positions
            leftSkinFinalPosition = leftSkinObject.position;
            rightSkinFinalPosition = rightSkinObject.position;
            Debug.Log($"Final positions stored from current skin positions");
        }
    }

    [ContextMenu("Test Show Visualizer")]
    private void TestShowVisualizer()
    {
        if (Application.isPlaying && stitchVisualizer != null)
        {
            StartCoroutine(ShowStitchVisualizer());
        }
    }

    [ContextMenu("Test Hide Visualizer")]
    private void TestHideVisualizer()
    {
        if (Application.isPlaying)
        {
            HideStitchVisualizer();
        }
    }

    // Visualization in Scene view
    private void OnDrawGizmosSelected()
    {
        if (firstStitch == null || secondStitch == null)
            return;

        // Draw detection spheres
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(firstStitch.position, detectionRadius);
        Gizmos.DrawWireSphere(secondStitch.position, detectionRadius);

        // Draw connection line
        Gizmos.color = isStitched ? Color.green : Color.red;
        Gizmos.DrawLine(firstStitch.position, secondStitch.position);

        // Draw visualizer indicator
        if (stitchVisualizer != null)
        {
            Gizmos.color = visualizerShown ? Color.blue : Color.gray;
            Vector3 center = (firstStitch.position + secondStitch.position) * 0.5f;
            Gizmos.DrawWireCube(center + Vector3.up * 0.01f, Vector3.one * 0.008f);
        }

        // Draw start position indicators for skins
        if (firstStitch != null && secondStitch != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 leftStart = firstStitch.position + leftSkinStartOffset;
            Vector3 rightStart = secondStitch.position + rightSkinStartOffset;
            Gizmos.DrawWireCube(leftStart, Vector3.one * 0.005f);
            Gizmos.DrawWireCube(rightStart, Vector3.one * 0.005f);

            // Draw center point
            Gizmos.color = Color.magenta;
            Vector3 center = (firstStitch.position + secondStitch.position) * 0.5f;
            Gizmos.DrawWireSphere(center, 0.003f);

            // Draw slide target positions
            Gizmos.color = Color.white;
            Vector3 leftTarget = firstStitch.position + skinToStitchOffset;
            Vector3 rightTarget = secondStitch.position + skinToStitchOffset;
            Gizmos.DrawWireCube(leftTarget, Vector3.one * 0.003f);
            Gizmos.DrawWireCube(rightTarget, Vector3.one * 0.003f);
        }
    }
}