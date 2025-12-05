using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class BloodCleaningSystem : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoPlayer videoPlayer;
    public VideoClip bloodFlowingState;
    public VideoClip intermediateState;
    public VideoClip finalState;

    [Header("Transition Settings")]
    public float transitionDuration = 2f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Interaction Settings")]
    public string cottonSwabTag = "CottonSwab";

    private enum CleaningState
    {
        BloodFlowing,
        Intermediate,
        Final
    }

    private CleaningState currentState = CleaningState.BloodFlowing;
    private bool isTransitioning = false;
    private Material videoMaterial;
    private RenderTexture transitionTexture;
    private Camera transitionCamera;

    void Start()
    {
        InitializeSystem();
    }

    void InitializeSystem()
    {
        // Set initial video
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.clip = bloodFlowingState;
        videoPlayer.isLooping = true;
        videoPlayer.Play();

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(cottonSwabTag) && !isTransitioning)
        {
            ProcessCleaning();
        }
    }

    void ProcessCleaning()
    {
        switch (currentState)
        {
            case CleaningState.BloodFlowing:
                StartCoroutine(TransitionToState(intermediateState, CleaningState.Intermediate));
                break;

            case CleaningState.Intermediate:
                StartCoroutine(TransitionToState(finalState, CleaningState.Final));
                break;

            case CleaningState.Final:
                // Already clean - could add feedback here
                Debug.Log("Already fully clean!");
                break;
        }
    }

    IEnumerator TransitionToState(VideoClip targetVideo, CleaningState targetState)
    {
        if (isTransitioning) yield break;

        isTransitioning = true;

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

        videoPlayer.targetTexture = currentTexture;
        tempVideoPlayer.targetTexture = targetTexture;

        // Wait a frame for video players to initialize
        yield return new WaitForEndOfFrame();

        // Perform top-to-bottom transition
        yield return StartCoroutine(PerformTopToBottomTransition(currentTexture, targetTexture));

        // Switch to the new video
        videoPlayer.targetTexture = null;
        videoPlayer.clip = targetVideo;
        currentState = targetState;

        // Cleanup
        Destroy(tempVideoPlayerObject);
        currentTexture.Release();
        targetTexture.Release();

        isTransitioning = false;
    }

    IEnumerator PerformTopToBottomTransition(RenderTexture fromTexture, RenderTexture toTexture)
    {
        // Create a material for the transition effect
        Material transitionMaterial = new Material(Shader.Find("Custom/TopToBottomTransition"));
        transitionMaterial.SetTexture("_FromTex", fromTexture);
        transitionMaterial.SetTexture("_ToTex", toTexture);

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float progress = elapsedTime / transitionDuration;
            float curvedProgress = transitionCurve.Evaluate(progress);

            // Set the transition progress (0 = all from texture, 1 = all to texture)
            transitionMaterial.SetFloat("_Progress", curvedProgress);

            // Render the transition
            Graphics.Blit(null, transitionTexture, transitionMaterial);
            videoMaterial.mainTexture = transitionTexture;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final state
        transitionMaterial.SetFloat("_Progress", 1f);
        Graphics.Blit(null, transitionTexture, transitionMaterial);

        Destroy(transitionMaterial);
    }

    // Alternative simpler approach using UV offset animation
    IEnumerator SimpleTopToBottomTransition(VideoClip targetVideo, CleaningState targetState)
    {
        if (isTransitioning) yield break;

        isTransitioning = true;

        // Create a quad that will act as a "wipe" mask
        GameObject wipeQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        wipeQuad.transform.SetParent(transform);
        wipeQuad.transform.localPosition = Vector3.zero;
        wipeQuad.transform.localScale = Vector3.one;

        // Set up the wipe material
        Material wipeMaterial = new Material(Shader.Find("Unlit/Texture"));
        Renderer wipeRenderer = wipeQuad.GetComponent<Renderer>();
        wipeRenderer.material = wipeMaterial;

        // Start playing the new video
        VideoClip oldVideo = videoPlayer.clip;
        videoPlayer.clip = targetVideo;

        // Animate the wipe from top to bottom
        float elapsedTime = 0f;
        Vector3 startScale = new Vector3(1, 0, 1);
        Vector3 endScale = Vector3.one;
        Vector3 startPosition = new Vector3(0, 0.5f, -0.01f);
        Vector3 endPosition = new Vector3(0, 0, -0.01f);

        while (elapsedTime < transitionDuration)
        {
            float progress = elapsedTime / transitionDuration;
            float curvedProgress = transitionCurve.Evaluate(progress);

            wipeQuad.transform.localScale = Vector3.Lerp(startScale, endScale, curvedProgress);
            wipeQuad.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curvedProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Cleanup
        Destroy(wipeQuad);
        currentState = targetState;
        isTransitioning = false;
    }

    void OnDestroy()
    {
        if (transitionTexture != null)
            transitionTexture.Release();
    }

    // Public method to reset the system
    public void ResetToInitialState()
    {
        if (!isTransitioning)
        {
            currentState = CleaningState.BloodFlowing;
            videoPlayer.clip = bloodFlowingState;
            videoPlayer.Play();
        }
    }

    // Public method to check current state
    public string GetCurrentState()
    {
        return currentState.ToString();
    }
}