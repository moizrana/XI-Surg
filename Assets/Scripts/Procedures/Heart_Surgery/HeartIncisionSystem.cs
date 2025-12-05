using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class HeartIncisionSystem : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoPlayer videoPlayer;
    public VideoClip step1Video; // Surface level - skin
    public VideoClip step2Video; // Deeper level - muscle/tissue
    public VideoClip step3Video; // Deepest level - heart

    [Header("Skin Layer Setup")]
    public GameObject skinLayer; // The skin layer object to be disabled after first incision
    public GameObject interiorVisualGameObject; // The interior visual GameObject to enable after first incision
    public float skinLayerRemovalDelay = 0.5f; // Delay before swapping skin layer and interior visual

    [Header("Transition Settings")]
    public float transitionDuration = 2f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Interaction Settings")]
    public string scalpelTag = "Scalpel";

    [Header("Display Settings")]
    public Transform interiorVisualObject; // Reference to the InteriorVisual object

    [System.Serializable]
    public class VideoDisplaySettings
    {
        [Header("Transform Settings")]
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;

        [Header("Material Settings")]
        public Color tintColor = Color.white;
        [Range(0f, 1f)]
        public float transparency = 1f;

        [Header("UV Settings")]
        public Vector2 uvOffset = Vector2.zero;
        public Vector2 uvTiling = Vector2.one;

        [Header("Animation Settings")]
        public bool animateOnTransition = false;
        public float animationDuration = 1f;
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    [Header("Individual Video Display Settings")]
    public VideoDisplaySettings step1DisplaySettings = new VideoDisplaySettings();
    public VideoDisplaySettings step2DisplaySettings = new VideoDisplaySettings();
    public VideoDisplaySettings step3DisplaySettings = new VideoDisplaySettings();

    private enum IncisionDepth
    {
        Initial,    // No incision yet - skin layer present
        Step1,      // First incision - skin level
        Step2,      // Second incision - deeper tissue
        Step3       // Final incision - heart level
    }

    private IncisionDepth currentDepth = IncisionDepth.Initial;
    private bool isTransitioning = false;
    private Material videoMaterial;
    private RenderTexture transitionTexture;
    private Camera transitionCamera;

    // Store original transform values for InteriorVisual
    private Vector3 originalInteriorPosition;
    private Quaternion originalInteriorRotation;
    private Vector3 originalInteriorScale;

    void Start()
    {
        InitializeSystem();
        SetupSkinLayer();
        SetupInitialVisibility();
        DebugSetup();
    }

    void SetupInitialVisibility()
    {
        Debug.Log("=== SETTING UP INITIAL VISIBILITY ===");

        // Store original InteriorVisual transform values BEFORE disabling it
        if (interiorVisualGameObject != null)
        {
            originalInteriorPosition = interiorVisualGameObject.transform.position;
            originalInteriorRotation = interiorVisualGameObject.transform.rotation;
            originalInteriorScale = interiorVisualGameObject.transform.localScale;

            Debug.Log($"Stored original InteriorVisual transform:");
            Debug.Log($"Position: {originalInteriorPosition}");
            Debug.Log($"Rotation: {originalInteriorRotation.eulerAngles}");
            Debug.Log($"Scale: {originalInteriorScale}");
        }
        else if (interiorVisualObject != null)
        {
            Debug.Log("Using Transform reference for InteriorVisual");
            // Fallback: if user assigned the Transform instead of GameObject
            interiorVisualGameObject = interiorVisualObject.gameObject;
            originalInteriorPosition = interiorVisualGameObject.transform.position;
            originalInteriorRotation = interiorVisualGameObject.transform.rotation;
            originalInteriorScale = interiorVisualGameObject.transform.localScale;
        }

        // At start, show skin layer and hide interior visual
        if (skinLayer != null)
        {
            skinLayer.SetActive(true);
            Debug.Log($"SkinLayer activated at start: {skinLayer.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("SkinLayer is null in SetupInitialVisibility!");
        }

        if (interiorVisualGameObject != null)
        {
            Debug.Log($"InteriorVisual before disable: {interiorVisualGameObject.activeInHierarchy}");
            interiorVisualGameObject.SetActive(false);
            Debug.Log($"InteriorVisual after disable: {interiorVisualGameObject.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("Both interiorVisualGameObject and interiorVisualObject are null!");
        }

        Debug.Log("=== INITIAL VISIBILITY SETUP COMPLETE ===");
    }

    void SetupSkinLayer()
    {
        if (skinLayer != null)
        {
            // Check if the skin layer has a collider (user should add this manually)
            Collider skinCollider = skinLayer.GetComponent<Collider>();
            if (skinCollider == null)
            {
                Debug.LogError("SkinLayer does not have a collider! Please add a BoxCollider (or other collider) and set it as a trigger.");
                return;
            }
            else
            {
                Debug.Log($"SkinLayer collider found: {skinCollider.GetType().Name}, IsTrigger: {skinCollider.isTrigger}");
                if (!skinCollider.isTrigger)
                {
                    Debug.LogWarning("SkinLayer collider is not set as trigger! Please check 'Is Trigger' in the collider component.");
                }
            }

            // Add the trigger detection script to the skin layer
            SkinLayerTrigger skinTrigger = skinLayer.GetComponent<SkinLayerTrigger>();
            if (skinTrigger == null)
            {
                skinTrigger = skinLayer.AddComponent<SkinLayerTrigger>();
            }
            skinTrigger.Initialize(this);
        }
        else
        {
            Debug.LogWarning("SkinLayer not assigned in inspector!");
        }
    }

    void DebugSetup()
    {
        Debug.Log($"HeartIncisionSystem attached to: {gameObject.name}");
        Debug.Log($"Looking for scalpel with tag: {scalpelTag}");
        Debug.Log($"Current depth: {currentDepth}");
        Debug.Log($"SkinLayer assigned: {(skinLayer != null ? skinLayer.name + " (Active: " + skinLayer.activeInHierarchy + ")" : "None")}");
        Debug.Log($"InteriorVisual assigned: {(interiorVisualGameObject != null ? interiorVisualGameObject.name + " (Active: " + interiorVisualGameObject.activeInHierarchy + ")" : "None")}");

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Debug.Log($"Main collider found: {col.GetType().Name}, IsTrigger: {col.isTrigger}");
        }
        else
        {
            Debug.LogError("No collider found on this GameObject!");
        }

        // Additional hierarchy debugging
        Debug.Log($"This script is on: {transform.name}");
        Debug.Log($"Parent: {(transform.parent != null ? transform.parent.name : "None")}");

        if (interiorVisualGameObject != null)
        {
            Debug.Log($"InteriorVisual parent: {(interiorVisualGameObject.transform.parent != null ? interiorVisualGameObject.transform.parent.name : "None")}");
            Debug.Log($"Is this script child of InteriorVisual? {transform.IsChildOf(interiorVisualGameObject.transform)}");
        }
    }

    void InitializeSystem()
    {
        // Set initial video player
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Start with no video playing initially (skin layer is visible)
        videoPlayer.isLooping = true;
        videoPlayer.Stop(); // Don't play until first incision

        // Get the material that displays the video
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            videoMaterial = renderer.material;
        }

        // Create render texture for transition effects
        CreateTransitionTexture();
    }

    void CreateTransitionTexture()
    {
        transitionTexture = new RenderTexture(1920, 1080, 0);

        // Create a camera for transition effects
        GameObject cameraObject = new GameObject("TransitionCamera");
        cameraObject.transform.SetParent(transform);
        transitionCamera = cameraObject.AddComponent<Camera>();
        transitionCamera.enabled = false;
        transitionCamera.targetTexture = transitionTexture;
    }

    // This will be called by the SkinLayerTrigger component
    public void OnSkinLayerIncision()
    {
        Debug.Log($"=== OnSkinLayerIncision called ===");
        Debug.Log($"TouchSwabHere active: {gameObject.activeInHierarchy}");
        Debug.Log($"TouchSwabHere enabled: {gameObject.activeSelf}");
        Debug.Log($"Current depth: {currentDepth}");
        Debug.Log($"Is transitioning: {isTransitioning}");

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("Cannot start coroutine - TouchSwabHere is inactive!");
            Debug.Log($"Parent chain: {GetParentChain()}");
            return;
        }

        if (currentDepth == IncisionDepth.Initial && !isTransitioning)
        {
            Debug.Log("Skin layer incision detected! Starting coroutine...");
            StartCoroutine(ProcessSkinLayerIncision());
        }
        else
        {
            Debug.LogWarning($"Cannot process incision - Depth: {currentDepth}, Transitioning: {isTransitioning}");
        }
    }

    string GetParentChain()
    {
        string chain = gameObject.name;
        Transform current = transform.parent;
        while (current != null)
        {
            chain = current.name + " -> " + chain;
            current = current.parent;
        }
        return chain;
    }

    IEnumerator ProcessSkinLayerIncision()
    {
        isTransitioning = true;

        // Start playing the first video
        if (step1Video != null)
        {
            videoPlayer.clip = step1Video;
            videoPlayer.Play();
        }

        // Wait for the specified delay
        yield return new WaitForSeconds(skinLayerRemovalDelay);

        // Swap visibility: disable skin layer, enable interior visual
        yield return StartCoroutine(SwapSkinLayerAndInterior());

        // Update the current depth
        currentDepth = IncisionDepth.Step1;

        // Apply display settings for step 1
        VideoDisplaySettings settings = GetDisplaySettingsForDepth(IncisionDepth.Step1);
        if (settings.animateOnTransition)
        {
            yield return StartCoroutine(AnimateDisplaySettings(settings));
        }
        else
        {
            ApplyDisplaySettings(settings);
        }

        isTransitioning = false;
        Debug.Log("Skin layer disabled, interior visual enabled, now at Step1 depth");
    }

    IEnumerator SwapSkinLayerAndInterior()
    {
        Debug.Log("=== STARTING SWAP PROCESS ===");

        // Disable skin layer
        if (skinLayer != null)
        {
            Debug.Log($"Before disable - SkinLayer active: {skinLayer.activeInHierarchy}");
            skinLayer.SetActive(false);
            Debug.Log($"After disable - SkinLayer active: {skinLayer.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("SkinLayer is null!");
        }

        // Enable interior visual and restore its transform
        if (interiorVisualGameObject != null)
        {
            Debug.Log($"Before enable - InteriorVisual active: {interiorVisualGameObject.activeInHierarchy}");
            Debug.Log($"Current transform before enable - Pos: {interiorVisualGameObject.transform.position}, Rot: {interiorVisualGameObject.transform.rotation.eulerAngles}, Scale: {interiorVisualGameObject.transform.localScale}");

            // Enable the GameObject first
            interiorVisualGameObject.SetActive(true);

            // Then restore the original transform values
            interiorVisualGameObject.transform.position = originalInteriorPosition;
            interiorVisualGameObject.transform.rotation = originalInteriorRotation;
            interiorVisualGameObject.transform.localScale = originalInteriorScale;

            Debug.Log($"After enable and restore - InteriorVisual active: {interiorVisualGameObject.activeInHierarchy}");
            Debug.Log($"Restored transform - Pos: {interiorVisualGameObject.transform.position}, Rot: {interiorVisualGameObject.transform.rotation.eulerAngles}, Scale: {interiorVisualGameObject.transform.localScale}");
        }
        else
        {
            Debug.LogError("InteriorVisual GameObject is null!");
        }

        Debug.Log("=== SWAP PROCESS COMPLETE ===");

        // Optional: Add a small delay for the transition effect
        yield return new WaitForSeconds(0.1f);
    }

    // Regular trigger detection for deeper incisions (after skin layer is removed)
    void OnTriggerEnter(Collider other)
    {
        // Debug logging to help troubleshoot
        Debug.Log($"Main trigger entered by: {other.name} with tag: {other.tag}");

        // Only process if we're past the initial skin layer stage
        if (currentDepth != IncisionDepth.Initial && other.CompareTag(scalpelTag) && !isTransitioning)
        {
            Debug.Log("Scalpel detected for deeper incision! Processing...");
            ProcessDeeperIncision();
        }
        else if (currentDepth == IncisionDepth.Initial)
        {
            Debug.Log("Still at initial depth - skin layer should handle this incision");
        }
        else if (!other.CompareTag(scalpelTag))
        {
            Debug.Log($"Object has wrong tag. Expected: {scalpelTag}, Got: {other.tag}");
        }
        else if (isTransitioning)
        {
            Debug.Log("Currently transitioning, ignoring input");
        }
    }

    void ProcessDeeperIncision()
    {
        switch (currentDepth)
        {
            case IncisionDepth.Step1:
                if (step2Video != null)
                {
                    StartCoroutine(TransitionToDepth(step2Video, IncisionDepth.Step2));
                }
                break;

            case IncisionDepth.Step2:
                if (step3Video != null)
                {
                    StartCoroutine(TransitionToDepth(step3Video, IncisionDepth.Step3));
                }
                break;

            case IncisionDepth.Step3:
                // Already at maximum depth
                Debug.Log("Maximum incision depth reached - heart level exposed!");
                break;
        }
    }

    IEnumerator TransitionToDepth(VideoClip targetVideo, IncisionDepth targetDepth)
    {
        if (isTransitioning) yield break;

        isTransitioning = true;

        Debug.Log($"Making incision to {targetDepth}");

        // Store current video
        VideoClip currentVideo = videoPlayer.clip;

        // Start playing the target video on a secondary video player
        GameObject tempVideoPlayerObject = new GameObject("TempVideoPlayer");
        VideoPlayer tempVideoPlayer = tempVideoPlayerObject.AddComponent<VideoPlayer>();
        tempVideoPlayer.clip = targetVideo;
        tempVideoPlayer.isLooping = true;
        tempVideoPlayer.Play();

        // Create render textures for both videos
        RenderTexture currentTexture = new RenderTexture(1920, 1080, 0);
        RenderTexture targetTexture = new RenderTexture(1920, 1080, 0);

        if (videoPlayer.clip != null)
        {
            videoPlayer.targetTexture = currentTexture;
        }
        tempVideoPlayer.targetTexture = targetTexture;

        // Wait a frame for video players to initialize
        yield return new WaitForEndOfFrame();

        // Perform incision transition (center-outward cut effect)
        yield return StartCoroutine(PerformIncisionTransition(currentTexture, targetTexture));

        // Switch to the new video
        videoPlayer.targetTexture = null;
        videoPlayer.clip = targetVideo;
        currentDepth = targetDepth;

        // Apply display settings for the new depth
        VideoDisplaySettings settings = GetDisplaySettingsForDepth(targetDepth);
        if (settings.animateOnTransition)
        {
            yield return StartCoroutine(AnimateDisplaySettings(settings));
        }
        else
        {
            ApplyDisplaySettings(settings);
        }

        // Cleanup
        Destroy(tempVideoPlayerObject);
        if (currentTexture != null) currentTexture.Release();
        if (targetTexture != null) targetTexture.Release();

        isTransitioning = false;
    }

    IEnumerator PerformIncisionTransition(RenderTexture fromTexture, RenderTexture toTexture)
    {
        // Create a material for the incision effect
        // For now, we'll use a simple top-to-bottom transition
        // You can replace this with a custom shader for a more realistic incision effect
        Material transitionMaterial = new Material(Shader.Find("Custom/IncisionTransition"));

        if (transitionMaterial.shader.name == "Custom/IncisionTransition")
        {
            transitionMaterial.SetTexture("_FromTex", fromTexture);
            transitionMaterial.SetTexture("_ToTex", toTexture);
        }
        else
        {
            // Fallback to standard shader if custom shader not found
            Debug.LogWarning("Custom incision shader not found, using fallback");
            transitionMaterial = new Material(Shader.Find("Unlit/Texture"));
        }

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float progress = elapsedTime / transitionDuration;
            float curvedProgress = transitionCurve.Evaluate(progress);

            // Set the transition progress (0 = all from texture, 1 = all to texture)
            if (transitionMaterial.HasProperty("_Progress"))
            {
                transitionMaterial.SetFloat("_Progress", curvedProgress);
                Graphics.Blit(null, transitionTexture, transitionMaterial);
            }
            else
            {
                // Simple blend fallback
                Graphics.Blit(curvedProgress < 0.5f ? fromTexture : toTexture, transitionTexture);
            }

            videoMaterial.mainTexture = transitionTexture;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final state
        if (transitionMaterial.HasProperty("_Progress"))
        {
            transitionMaterial.SetFloat("_Progress", 1f);
            Graphics.Blit(null, transitionTexture, transitionMaterial);
        }
        else
        {
            Graphics.Blit(toTexture, transitionTexture);
        }

        Destroy(transitionMaterial);
    }

    void OnDestroy()
    {
        if (transitionTexture != null)
            transitionTexture.Release();
    }

    // Method to get display settings for a specific depth
    VideoDisplaySettings GetDisplaySettingsForDepth(IncisionDepth depth)
    {
        switch (depth)
        {
            case IncisionDepth.Step1: return step1DisplaySettings;
            case IncisionDepth.Step2: return step2DisplaySettings;
            case IncisionDepth.Step3: return step3DisplaySettings;
            default: return step1DisplaySettings;
        }
    }

    // Method to apply display settings
    void ApplyDisplaySettings(VideoDisplaySettings settings)
    {
        if (interiorVisualObject != null)
        {
            // Apply transform settings
            interiorVisualObject.localPosition = settings.position;
            interiorVisualObject.localEulerAngles = settings.rotation;
            interiorVisualObject.localScale = settings.scale;
        }

        if (videoMaterial != null)
        {
            // Apply material settings
            videoMaterial.color = new Color(settings.tintColor.r, settings.tintColor.g, settings.tintColor.b, settings.transparency);

            // Apply UV settings
            videoMaterial.mainTextureOffset = settings.uvOffset;
            videoMaterial.mainTextureScale = settings.uvTiling;
        }
    }

    // Method to animate display settings
    IEnumerator AnimateDisplaySettings(VideoDisplaySettings targetSettings)
    {
        if (interiorVisualObject == null) yield break;

        // Store current settings
        Vector3 startPosition = interiorVisualObject.localPosition;
        Vector3 startRotation = interiorVisualObject.localEulerAngles;
        Vector3 startScale = interiorVisualObject.localScale;

        Color startColor = videoMaterial != null ? videoMaterial.color : Color.white;
        Vector2 startUVOffset = videoMaterial != null ? videoMaterial.mainTextureOffset : Vector2.zero;
        Vector2 startUVTiling = videoMaterial != null ? videoMaterial.mainTextureScale : Vector2.one;

        float elapsedTime = 0f;

        while (elapsedTime < targetSettings.animationDuration)
        {
            float progress = elapsedTime / targetSettings.animationDuration;
            float curvedProgress = targetSettings.animationCurve.Evaluate(progress);

            // Animate transform
            interiorVisualObject.localPosition = Vector3.Lerp(startPosition, targetSettings.position, curvedProgress);
            interiorVisualObject.localEulerAngles = Vector3.Lerp(startRotation, targetSettings.rotation, curvedProgress);
            interiorVisualObject.localScale = Vector3.Lerp(startScale, targetSettings.scale, curvedProgress);

            // Animate material properties
            if (videoMaterial != null)
            {
                Color targetColor = new Color(targetSettings.tintColor.r, targetSettings.tintColor.g, targetSettings.tintColor.b, targetSettings.transparency);
                videoMaterial.color = Color.Lerp(startColor, targetColor, curvedProgress);
                videoMaterial.mainTextureOffset = Vector2.Lerp(startUVOffset, targetSettings.uvOffset, curvedProgress);
                videoMaterial.mainTextureScale = Vector2.Lerp(startUVTiling, targetSettings.uvTiling, curvedProgress);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        ApplyDisplaySettings(targetSettings);
    }

    // Public method to reset the system
    public void ResetToInitialState()
    {
        if (!isTransitioning)
        {
            currentDepth = IncisionDepth.Initial;
            videoPlayer.Stop();
            videoPlayer.clip = null;

            // Restore skin layer visibility and hide interior visual
            if (skinLayer != null)
            {
                skinLayer.SetActive(true);
            }
            if (interiorVisualGameObject != null)
            {
                interiorVisualGameObject.SetActive(false);
                // Also restore the original transform when hiding
                interiorVisualGameObject.transform.position = originalInteriorPosition;
                interiorVisualGameObject.transform.rotation = originalInteriorRotation;
                interiorVisualGameObject.transform.localScale = originalInteriorScale;
            }
        }
    }

    // Public method to go to a specific depth
    public void GoToDepth(int depth)
    {
        if (isTransitioning) return;

        // Handle going to initial state (skin layer visible)
        if (depth == 0)
        {
            ResetToInitialState();
            return;
        }

        VideoClip targetVideo = null;
        IncisionDepth targetDepth = IncisionDepth.Initial;

        switch (depth)
        {
            case 1:
                targetVideo = step1Video;
                targetDepth = IncisionDepth.Step1;
                break;
            case 2:
                targetVideo = step2Video;
                targetDepth = IncisionDepth.Step2;
                break;
            case 3:
                targetVideo = step3Video;
                targetDepth = IncisionDepth.Step3;
                break;
        }

        if (targetVideo != null)
        {
            // If going to depth 1 and we're at initial, swap skin layer first
            if (depth == 1 && currentDepth == IncisionDepth.Initial)
            {
                StartCoroutine(ProcessSkinLayerIncision());
            }
            else
            {
                StartCoroutine(TransitionToDepth(targetVideo, targetDepth));
            }
        }
    }

    // Public method to apply display settings immediately
    public void ApplyDisplaySettingsForCurrentDepth()
    {
        VideoDisplaySettings settings = GetDisplaySettingsForDepth(currentDepth);
        ApplyDisplaySettings(settings);
    }

    // Public method to set display settings for a specific depth
    public void SetDisplaySettings(int depth, VideoDisplaySettings newSettings)
    {
        switch (depth)
        {
            case 1:
                step1DisplaySettings = newSettings;
                break;
            case 2:
                step2DisplaySettings = newSettings;
                break;
            case 3:
                step3DisplaySettings = newSettings;
                break;
        }

        // If this is the current depth, apply immediately
        if (GetCurrentDepthLevel() == depth)
        {
            ApplyDisplaySettings(newSettings);
        }
    }

    // Public method to check current depth
    public string GetCurrentDepth()
    {
        return currentDepth.ToString();
    }

    // Public method to get current depth as integer
    public int GetCurrentDepthLevel()
    {
        switch (currentDepth)
        {
            case IncisionDepth.Initial: return 0;
            case IncisionDepth.Step1: return 1;
            case IncisionDepth.Step2: return 2;
            case IncisionDepth.Step3: return 3;
            default: return 0;
        }
    }

    // Public method to manually swap skin layer (for testing)
    public void SwapSkinLayerManually()
    {
        if (currentDepth == IncisionDepth.Initial)
        {
            StartCoroutine(ProcessSkinLayerIncision());
        }
    }

    // Public method to check if skin layer is present and active
    public bool IsSkinLayerActive()
    {
        return skinLayer != null && skinLayer.activeInHierarchy && currentDepth == IncisionDepth.Initial;
    }
}