using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINotificationManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject notificationPanel;
    public Text notificationText;
    
    [Header("Display Settings")]
    public float displayDuration = 1f;
    public Color infoColor = Color.white;
    public Color warningColor = Color.yellow;
    public Color errorColor = Color.red;
    
    private static UINotificationManager instance;
    private Coroutine currentNotificationCoroutine;
    
    public static UINotificationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UINotificationManager>();
                if (instance == null)
                {
                    Debug.LogError("UINotificationManager not found in scene!");
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        // Ensure panel starts hidden
        if (notificationPanel != null)
            notificationPanel.SetActive(false);
    }
    
    public void ShowNotification(string message, NotificationType type = NotificationType.Info)
    {
        if (notificationPanel == null || notificationText == null)
        {
            Debug.LogError("Notification UI components not assigned!");
            return;
        }
        
        // Stop any existing notification
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }
        
        // Set text and color based on type
        notificationText.text = message;
        notificationText.color = GetColorForType(type);
        
        // Start new notification display
        currentNotificationCoroutine = StartCoroutine(DisplayNotificationCoroutine());
    }
    
    private Color GetColorForType(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Warning:
                return warningColor;
            case NotificationType.Error:
                return errorColor;
            default:
                return infoColor;
        }
    }
    
    private IEnumerator DisplayNotificationCoroutine()
    {
        notificationPanel.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        notificationPanel.SetActive(false);
        currentNotificationCoroutine = null;
    }
    
    // Static methods for easy access
    public static void ShowInfo(string message)
    {
        Instance?.ShowNotification(message, NotificationType.Info);
    }
    
    public static void ShowWarning(string message)
    {
        Instance?.ShowNotification(message, NotificationType.Warning);
    }
    
    public static void ShowError(string message)
    {
        Instance?.ShowNotification(message, NotificationType.Error);
    }
}

public enum NotificationType
{
    Info,
    Warning,
    Error
}
