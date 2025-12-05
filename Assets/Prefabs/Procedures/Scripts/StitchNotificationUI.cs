using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages UI notifications for the stitching procedure
/// Shows messages for stitch completions and procedure completion
/// </summary>
public class StitchNotificationUI : MonoBehaviour
{
    [Header("UI References")]
    public Text notificationText;
    public GameObject notificationPanel;
    public CanvasGroup notificationCanvasGroup;
    
    [Header("Auto-Setup")]
    public bool autoCreateUI = true;
    public Canvas targetCanvas;
    
    [Header("Animation Settings")]
    public float displayDuration = 2.0f;
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.5f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Messages")]
    public string[] stitchMessages = new string[]
    {
        "First stitch done!",
        "Second stitch done!",
        "Third stitch done!",
        "Fourth stitch done!",
        "Fifth stitch done!",
        "Sixth stitch done!",
        "Seventh stitch done!",
        "Final stitch done!"
    };
    
    public string procedureCompleteMessage = "Procedure completed successfully!";
    public string procedureStartMessage = "Stitching procedure started";
    
    [Header("Styling")]
    public Color normalTextColor = Color.white;
    public Color completionTextColor = Color.green;
    public int fontSize = 24;
    public FontStyle fontStyle = FontStyle.Bold;
    
    // Private variables
    private StitchManager stitchManager;
    private Coroutine currentNotificationCoroutine;
    private bool isUISetup = false;
    
    private void Awake()
    {
        // Find stitch manager
        stitchManager = FindObjectOfType<StitchManager>();
        if (stitchManager == null)
        {
            Debug.LogError("StitchNotificationUI: No StitchManager found in scene!");
        }
        
        // Auto-create UI if needed
        if (autoCreateUI && !isUISetup)
        {
            CreateNotificationUI();
        }
    }
    
    private void Start()
    {
        // Subscribe to events
        if (stitchManager != null)
        {
            stitchManager.OnStitchCompleted += OnStitchCompleted;
            stitchManager.OnProcedureStarted += OnProcedureStarted;
            stitchManager.OnProcedureCompleted += OnProcedureCompleted;
        }
        
        // Initially hide notification
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
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
    }
    
    /// <summary>
    /// Auto-create the notification UI
    /// </summary>
    private void CreateNotificationUI()
    {
        // Find or create canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
        }
        
        if (targetCanvas == null)
        {
            // Create new canvas
            GameObject canvasObj = new GameObject("NotificationCanvas");
            targetCanvas = canvasObj.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            targetCanvas.sortingOrder = 100; // High priority
            
            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add Graphic Raycaster
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create notification panel
        GameObject panelObj = new GameObject("NotificationPanel");
        panelObj.transform.SetParent(targetCanvas.transform, false);
        
        // Setup RectTransform
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.8f);
        panelRect.anchorMax = new Vector2(0.5f, 0.8f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 80);
        
        // Add background image
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        
        // Add CanvasGroup for fading
        notificationCanvasGroup = panelObj.AddComponent<CanvasGroup>();
        
        // Create text object
        GameObject textObj = new GameObject("NotificationText");
        textObj.transform.SetParent(panelObj.transform, false);
        
        // Setup text RectTransform
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);
        
        // Setup text component
        notificationText = textObj.AddComponent<Text>();
        notificationText.text = "";
        notificationText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        notificationText.fontSize = fontSize;
        notificationText.fontStyle = fontStyle;
        notificationText.color = normalTextColor;
        notificationText.alignment = TextAnchor.MiddleCenter;
        
        // Set references
        notificationPanel = panelObj;
        isUISetup = true;
        
        Debug.Log("StitchNotificationUI: Auto-created notification UI");
    }
    
    /// <summary>
    /// Called when procedure starts
    /// </summary>
    private void OnProcedureStarted()
    {
        ShowNotification(procedureStartMessage, normalTextColor);
    }
    
    /// <summary>
    /// Called when a stitch is completed
    /// </summary>
    private void OnStitchCompleted(StitchSite completedSite)
    {
        int completedStitches = stitchManager.GetCompletedStitches();
        
        // Get appropriate message
        string message;
        if (completedStitches > 0 && completedStitches <= stitchMessages.Length)
        {
            message = stitchMessages[completedStitches - 1];
        }
        else
        {
            message = $"Stitch {completedStitches} completed!";
        }
        
        ShowNotification(message, normalTextColor);
    }
    
    /// <summary>
    /// Called when procedure is completed
    /// </summary>
    private void OnProcedureCompleted()
    {
        ShowNotification(procedureCompleteMessage, completionTextColor);
    }
    
    /// <summary>
    /// Show a notification with the given message
    /// </summary>
    public void ShowNotification(string message, Color textColor)
    {
        if (notificationText == null || notificationPanel == null)
        {
            Debug.LogWarning("StitchNotificationUI: UI components not set up!");
            return;
        }
        
        // Stop any existing notification
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }
        
        // Start new notification
        currentNotificationCoroutine = StartCoroutine(ShowNotificationCoroutine(message, textColor));
    }
    
    /// <summary>
    /// Coroutine to handle notification display and animation
    /// </summary>
    private System.Collections.IEnumerator ShowNotificationCoroutine(string message, Color textColor)
    {
        // Set message and color
        notificationText.text = message;
        notificationText.color = textColor;
        
        // Show panel
        notificationPanel.SetActive(true);
        
        // Fade in
        yield return StartCoroutine(FadeCanvasGroup(notificationCanvasGroup, 0f, 1f, fadeInDuration));
        
        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(notificationCanvasGroup, 1f, 0f, fadeOutDuration));
        
        // Hide panel
        notificationPanel.SetActive(false);
        
        currentNotificationCoroutine = null;
    }
    
    /// <summary>
    /// Fade CanvasGroup alpha
    /// </summary>
    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float curveValue = fadeCurve.Evaluate(progress);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
    
    /// <summary>
    /// Manually show a custom notification
    /// </summary>
    public void ShowCustomNotification(string message)
    {
        ShowNotification(message, normalTextColor);
    }
    
    /// <summary>
    /// Update stitch messages (useful for different languages or custom messages)
    /// </summary>
    public void SetStitchMessages(string[] newMessages)
    {
        stitchMessages = newMessages;
    }
    
    /// <summary>
    /// Set the completion message
    /// </summary>
    public void SetCompletionMessage(string message)
    {
        procedureCompleteMessage = message;
    }
}
