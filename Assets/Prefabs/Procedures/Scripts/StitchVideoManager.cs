using UnityEngine;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Manages video playback progression during the stitching procedure
/// Changes video clips based on stitch completion progress
/// Enhanced with smooth transitions to eliminate white flash
/// </summary>
public class StitchVideoManager : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer primaryVideoPlayer;
    public VideoPlayer secondaryVideoPlayer; // For crossfading
    public VideoClip[] stepVideos = new VideoClip[5]; // step1.mp4 through step5.mp4

    [Header("Transition Settings")]
    public bool useCrossfade = true;
    public float crossfadeDuration = 0.5f;
    public bool preloadNextVideo = true;

    [Header("Alternative: Single Player with Frame Hold")]
    public bool useFrameHold = false; // Alternative method if you can't use dual players
    public float frameHoldDuration = 0.1f;

    [Header("Auto-Find Settings")]
    public bool autoFindVideoPlayer = true;
    public string videoPlayerObjectName = "InteriorVisual";

    [Header("Debug")]
    public bool showDebugLogs = true;

    // Private variables
    private StitchManager stitchManager;
    private int currentStep = 0;
    private bool isTransitioning = false;
    private VideoPlayer currentActivePlayer;
    private Renderer videoRenderer;
    private Material videoMaterial;
    private RenderTexture lastFrame;

    private void Awake()
    {
        // Auto-find video player if not assigned
        if (primaryVideoPlayer == null && autoFindVideoPlayer)
        {
            FindVideoPlayer();
        }

        // Setup secondary player for crossfading
        SetupSecondaryVideoPlayer();

        // Find stitch manager
        stitchManager = FindObjectOfType<StitchManager>();
        if (stitchManager == null)
        {
            Debug.LogError("StitchVideoManager: No StitchManager found in scene!");
        }

        // Get renderer and material for manual texture control
        if (primaryVideoPlayer != null)
        {
            videoRenderer = primaryVideoPlayer.GetComponent<Renderer>();
            if (videoRenderer != null)
            {
                videoMaterial = videoRenderer.material;
            }
        }
    }

    private void Start()
    {
        // Subscribe to stitch completion events
        if (stitchManager != null)
        {
            stitchManager.OnStitchCompleted += OnStitchCompleted;
            stitchManager.OnProcedureStarted += OnProcedureStarted;
            stitchManager.OnProcedureCompleted += OnProcedureCompleted;
        }

        // Validate setup
        ValidateSetup();

        // Initialize current active player
        currentActivePlayer = primaryVideoPlayer;

        // Start with step 1 video
        SetVideoStep(1);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (stitchManager != null)
        {
            stitchManager.OnStitchCompleted -= OnStitchCompleted;
            stitchManager.OnProcedureStarted -= OnProcedureStarted;
            stitchManager.OnProcedureCompleted -= OnProcedureCompleted;
        }

        // Cleanup render texture
        if (lastFrame != null)
        {
            lastFrame.Release();
            DestroyImmediate(lastFrame);
        }
    }

    /// <summary>
    /// Setup secondary video player for smooth transitions
    /// </summary>
    private void SetupSecondaryVideoPlayer()
    {
        if (useCrossfade && primaryVideoPlayer != null && secondaryVideoPlayer == null)
        {
            // Create a duplicate video player for crossfading
            GameObject secondaryObj = new GameObject("SecondaryVideoPlayer");
            secondaryObj.transform.SetParent(primaryVideoPlayer.transform.parent);
            secondaryObj.transform.localPosition = primaryVideoPlayer.transform.localPosition;
            secondaryObj.transform.localRotation = primaryVideoPlayer.transform.localRotation;
            secondaryObj.transform.localScale = primaryVideoPlayer.transform.localScale;

            // Copy video player component
            secondaryVideoPlayer = secondaryObj.AddComponent<VideoPlayer>();

            // Copy renderer component
            if (primaryVideoPlayer.GetComponent<Renderer>() != null)
            {
                Renderer secondaryRenderer = secondaryObj.AddComponent<Renderer>();
                secondaryRenderer.material = new Material(primaryVideoPlayer.GetComponent<Renderer>().material);
            }

            // Configure secondary player
            secondaryVideoPlayer.playOnAwake = false;
            secondaryVideoPlayer.isLooping = primaryVideoPlayer.isLooping;
            secondaryVideoPlayer.renderMode = primaryVideoPlayer.renderMode;

            // Start with secondary player invisible
            secondaryObj.SetActive(false);

            if (showDebugLogs)
                Debug.Log("StitchVideoManager: Created secondary video player for crossfading");
        }
    }

    /// <summary>
    /// Auto-find the video player in the scene
    /// </summary>
    private void FindVideoPlayer()
    {
        // First try to find by object name
        GameObject videoObj = GameObject.Find(videoPlayerObjectName);
        if (videoObj != null)
        {
            primaryVideoPlayer = videoObj.GetComponent<VideoPlayer>();
        }

        // If not found, try to find any VideoPlayer in the scene
        if (primaryVideoPlayer == null)
        {
            primaryVideoPlayer = FindObjectOfType<VideoPlayer>();
        }

        if (primaryVideoPlayer != null && showDebugLogs)
        {
            Debug.Log($"StitchVideoManager: Found VideoPlayer on {primaryVideoPlayer.gameObject.name}");
        }
    }

    /// <summary>
    /// Validate that everything is set up correctly
    /// </summary>
    private void ValidateSetup()
    {
        if (primaryVideoPlayer == null)
        {
            Debug.LogError("StitchVideoManager: No VideoPlayer assigned! Please assign one in the inspector or ensure autoFindVideoPlayer is enabled.");
            return;
        }

        // Check if videos are assigned
        int assignedVideos = 0;
        for (int i = 0; i < stepVideos.Length; i++)
        {
            if (stepVideos[i] != null)
                assignedVideos++;
        }

        if (assignedVideos == 0)
        {
            Debug.LogWarning("StitchVideoManager: No step videos assigned! Please assign step1.mp4 through step5.mp4 in the inspector.");
        }
        else if (showDebugLogs)
        {
            Debug.Log($"StitchVideoManager: {assignedVideos}/5 step videos assigned.");
        }
    }

    /// <summary>
    /// Called when the procedure starts
    /// </summary>
    private void OnProcedureStarted()
    {
        if (showDebugLogs)
            Debug.Log("StitchVideoManager: Procedure started - Setting video to step 1");

        SetVideoStep(1);
    }

    /// <summary>
    /// Called when a stitch is completed
    /// </summary>
    private void OnStitchCompleted(StitchSite completedSite)
    {
        // Get current progress from stitch manager
        int completedStitches = stitchManager.GetCompletedStitches();

        // Calculate next video step (completed stitches + 1)
        int nextStep = completedStitches + 1;

        if (showDebugLogs)
            Debug.Log($"StitchVideoManager: Stitch {completedStitches} completed. Advancing to step {nextStep}");

        SetVideoStep(nextStep);
    }

    /// <summary>
    /// Called when the entire procedure is completed
    /// </summary>
    private void OnProcedureCompleted()
    {
        if (showDebugLogs)
            Debug.Log("StitchVideoManager: Procedure completed - Final video step");

        // Ensure we're on the final step (step 5 for 4 completed stitches)
        SetVideoStep(5);
    }

    /// <summary>
    /// Set the video to a specific step with smooth transition
    /// </summary>
    public void SetVideoStep(int step)
    {
        if (primaryVideoPlayer == null)
        {
            Debug.LogError("StitchVideoManager: Cannot set video step - no VideoPlayer assigned!");
            return;
        }

        // Don't transition if we're already on this step or currently transitioning
        if (step == currentStep || isTransitioning)
            return;

        // Clamp step to valid range (1-5 for 4 stitches + final step)
        step = Mathf.Clamp(step, 1, 5);

        // Get the video clip for this step (step 1 = index 0)
        int videoIndex = step - 1;

        if (videoIndex >= 0 && videoIndex < stepVideos.Length && stepVideos[videoIndex] != null)
        {
            if (useCrossfade && secondaryVideoPlayer != null && currentStep > 0)
            {
                // Use crossfade transition
                StartCoroutine(CrossfadeToNewVideo(stepVideos[videoIndex], step));
            }
            else if (useFrameHold)
            {
                // Use frame hold method
                StartCoroutine(TransitionWithFrameHold(stepVideos[videoIndex], step));
            }
            else
            {
                // Direct transition (original method)
                DirectVideoTransition(stepVideos[videoIndex], step);
            }
        }
        else
        {
            Debug.LogWarning($"StitchVideoManager: step{step}.mp4 is not assigned in the stepVideos array!");
        }
    }

    /// <summary>
    /// Direct video transition (original method)
    /// </summary>
    private void DirectVideoTransition(VideoClip newClip, int step)
    {
        currentStep = step;
        primaryVideoPlayer.clip = newClip;
        primaryVideoPlayer.Play();

        if (showDebugLogs)
            Debug.Log($"StitchVideoManager: Playing step{step}.mp4");
    }

    /// <summary>
    /// Crossfade between two video players for smooth transition
    /// </summary>
    private IEnumerator CrossfadeToNewVideo(VideoClip newClip, int step)
    {
        isTransitioning = true;

        // Prepare secondary player with new video
        secondaryVideoPlayer.clip = newClip;
        secondaryVideoPlayer.Prepare();

        // Wait for secondary video to be prepared
        while (!secondaryVideoPlayer.isPrepared)
        {
            yield return null;
        }

        // Start playing secondary video
        secondaryVideoPlayer.Play();
        secondaryVideoPlayer.gameObject.SetActive(true);

        // Get renderers for crossfading
        Renderer primaryRenderer = primaryVideoPlayer.GetComponent<Renderer>();
        Renderer secondaryRenderer = secondaryVideoPlayer.GetComponent<Renderer>();

        if (primaryRenderer != null && secondaryRenderer != null)
        {
            Material primaryMat = primaryRenderer.material;
            Material secondaryMat = secondaryRenderer.material;

            // Crossfade
            float elapsed = 0f;
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / crossfadeDuration;

                // Fade out primary, fade in secondary
                Color primaryColor = primaryMat.color;
                Color secondaryColor = secondaryMat.color;

                primaryColor.a = 1f - t;
                secondaryColor.a = t;

                primaryMat.color = primaryColor;
                secondaryMat.color = secondaryColor;

                yield return null;
            }

            // Ensure final alpha values
            Color finalPrimaryColor = primaryMat.color;
            Color finalSecondaryColor = secondaryMat.color;
            finalPrimaryColor.a = 0f;
            finalSecondaryColor.a = 1f;
            primaryMat.color = finalPrimaryColor;
            secondaryMat.color = finalSecondaryColor;
        }

        // Switch players
        primaryVideoPlayer.Stop();
        primaryVideoPlayer.gameObject.SetActive(false);

        // Swap references
        VideoPlayer temp = primaryVideoPlayer;
        primaryVideoPlayer = secondaryVideoPlayer;
        secondaryVideoPlayer = temp;
        currentActivePlayer = primaryVideoPlayer;

        currentStep = step;
        isTransitioning = false;

        if (showDebugLogs)
            Debug.Log($"StitchVideoManager: Crossfaded to step{step}.mp4");
    }

    /// <summary>
    /// Transition with frame hold to avoid white flash
    /// </summary>
    private IEnumerator TransitionWithFrameHold(VideoClip newClip, int step)
    {
        isTransitioning = true;

        // Capture current frame if possible
        if (primaryVideoPlayer.texture != null)
        {
            // Create a render texture to hold the last frame
            if (lastFrame == null || lastFrame.width != primaryVideoPlayer.texture.width || lastFrame.height != primaryVideoPlayer.texture.height)
            {
                if (lastFrame != null) lastFrame.Release();
                lastFrame = new RenderTexture(primaryVideoPlayer.texture.width, primaryVideoPlayer.texture.height, 0);
            }

            // Copy current frame
            Graphics.Blit(primaryVideoPlayer.texture as RenderTexture, lastFrame);

            // Apply the held frame to the renderer
            if (videoRenderer != null && videoMaterial != null)
            {
                videoMaterial.mainTexture = lastFrame;
            }
        }

        // Wait a brief moment to ensure frame is held
        yield return new WaitForSeconds(frameHoldDuration);

        // Prepare new video
        primaryVideoPlayer.clip = newClip;
        primaryVideoPlayer.Prepare();

        // Wait for video to be prepared
        while (!primaryVideoPlayer.isPrepared)
        {
            yield return null;
        }

        // Start playing new video
        primaryVideoPlayer.Play();

        // Wait for first frame of new video to be available
        yield return new WaitForSeconds(0.1f);

        // Restore normal texture assignment (video player will take over)
        if (videoRenderer != null && videoMaterial != null)
        {
            videoMaterial.mainTexture = primaryVideoPlayer.texture;
        }

        currentStep = step;
        isTransitioning = false;

        if (showDebugLogs)
            Debug.Log($"StitchVideoManager: Transitioned with frame hold to step{step}.mp4");
    }

    /// <summary>
    /// Preload the next video for faster transitions
    /// </summary>
    private void PreloadNextVideo()
    {
        if (!preloadNextVideo || secondaryVideoPlayer == null)
            return;

        int nextStep = currentStep + 1;
        if (nextStep <= stepVideos.Length && stepVideos[nextStep - 1] != null)
        {
            secondaryVideoPlayer.clip = stepVideos[nextStep - 1];
            secondaryVideoPlayer.Prepare();
        }
    }

    /// <summary>
    /// Get the current video step
    /// </summary>
    public int GetCurrentStep()
    {
        return currentStep;
    }

    /// <summary>
    /// Manually advance to the next video step
    /// </summary>
    public void AdvanceToNextStep()
    {
        SetVideoStep(currentStep + 1);
    }

    /// <summary>
    /// Reset video to step 1 (called when procedure is reset)
    /// </summary>
    public void ResetToFirstStep()
    {
        if (showDebugLogs)
            Debug.Log("StitchVideoManager: Resetting to step 1");

        SetVideoStep(1);
    }

    /// <summary>
    /// Set whether to show debug logs
    /// </summary>
    public void SetDebugMode(bool enabled)
    {
        showDebugLogs = enabled;
    }
}