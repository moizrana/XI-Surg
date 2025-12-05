using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Text;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class ChatSystem : MonoBehaviour
{
    [Header("Chat UI Components")]
    public GameObject chatPanel;
    public TMP_InputField messageInputField;
    public Button sendButton;
    public TextMeshProUGUI placeholderText;

    [Header("Response UI Components")]
    public GameObject responsePanel;
    public TextMeshProUGUI responseText;

    [Header("Chat Settings")]
    public string apiUrl = "https://dt-agent-api.onrender.com/chat";
    public KeyCode chatToggleKey = KeyCode.T;

    [Header("VR Settings")]
    public float chatPanelDistance = 1.0f;
    public float chatPanelHeight = -0.2f;

    [Header("Visual Settings")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.2f;

    private bool isChatOpen = false;
    private CanvasGroup chatCanvasGroup;
    private CanvasGroup responseCanvasGroup;
    private Coroutine fadeCoroutine;
    private Transform playerHead;
    private bool isInVRMode = false;
    private XRBaseInteractor[] rayInteractors;
    private bool prevPrimaryRight, prevPrimaryLeft;

    void Start()
    {
        InitializeChatSystem();
        CheckVRStatus();
        FindRayInteractors();
    }

    void InitializeChatSystem()
    {
        // Get player head transform
        playerHead = Camera.main.transform;

        // Chat panel setup
        if (chatPanel != null)
        {
            chatPanel.SetActive(false);
            chatCanvasGroup = chatPanel.GetComponent<CanvasGroup>() ?? chatPanel.AddComponent<CanvasGroup>();

            // Convert to VR-ready canvas if in VR
            if (isInVRMode)
            {
                ConvertToVRCanvas(chatPanel);
            }
        }

        // Response panel setup
        if (responsePanel != null)
        {
            responsePanel.SetActive(false);
            responseCanvasGroup = responsePanel.GetComponent<CanvasGroup>() ?? responsePanel.AddComponent<CanvasGroup>();

            // Convert to VR-ready canvas if in VR
            if (isInVRMode)
            {
                ConvertToVRCanvas(responsePanel);
            }
        }

        // Input field setup
        if (messageInputField != null)
        {
            messageInputField.onSubmit.AddListener(OnSubmitMessage);
            messageInputField.characterLimit = 500;
        }

        // Send button setup
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(OnSendButtonClicked);
        }

        // Placeholder
        if (placeholderText != null)
        {
            placeholderText.text = "Type your message here...";
        }
    }

    void CheckVRStatus()
    {
        isInVRMode = XRSettings.isDeviceActive;
        Debug.Log("Running in " + (isInVRMode ? "VR Mode" : "Desktop Mode"));
    }

    void FindRayInteractors()
    {
        if (!isInVRMode) return;

        // Find all interactors in the scene
        rayInteractors = FindObjectsOfType<XRBaseInteractor>()
            .Where(i => i is XRRayInteractor || i is NearFarInteractor)
            .ToArray();

        Debug.Log($"Found {rayInteractors.Length} ray interactors");
    }

    void ConvertToVRCanvas(GameObject panel)
    {
        Canvas canvas = panel.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            // Add VR interaction components
            panel.AddComponent<TrackedDeviceGraphicRaycaster>();

            // Set appropriate scale
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(800, 400);
            rt.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
        }

        // Add colliders to interactive elements
        foreach (Button btn in panel.GetComponentsInChildren<Button>())
        {
            BoxCollider col = btn.gameObject.GetComponent<BoxCollider>() ?? btn.gameObject.AddComponent<BoxCollider>();
            RectTransform btnRect = btn.GetComponent<RectTransform>();
            col.size = new Vector3(btnRect.rect.width, btnRect.rect.height, 0.01f);
            col.center = new Vector3(0, 0, 0.005f);
        }

        if (messageInputField != null)
        {
            BoxCollider inputCol = messageInputField.gameObject.GetComponent<BoxCollider>() ??
                                  messageInputField.gameObject.AddComponent<BoxCollider>();
            RectTransform inputRect = messageInputField.GetComponent<RectTransform>();
            inputCol.size = new Vector3(inputRect.rect.width, inputRect.rect.height, 0.01f);
            inputCol.center = new Vector3(0, 0, 0.005f);
        }
    }

    void Update()
    {
        // Handle desktop input
        if (Input.GetKeyDown(chatToggleKey) )
        {
            if (!isChatOpen) OpenChat();
        }

        if (isChatOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseChat();
        }

        // Handle VR controller input
        if (isInVRMode)
        {
            HandleVRInput();
        }
    }

    void HandleVRInput()
    {
        // Check for primary button press on either controller
        bool primaryButtonRight, primaryButtonLeft;

        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonRight);
        leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonLeft);

        // Detect button down (not held)
        bool rightPressed = primaryButtonRight && !prevPrimaryRight;
        bool leftPressed = primaryButtonLeft && !prevPrimaryLeft;

        if ((rightPressed || leftPressed) && !isChatOpen)
        {
            OpenChat();
        }

        // Store previous state for edge detection
        prevPrimaryRight = primaryButtonRight;
        prevPrimaryLeft = primaryButtonLeft;
    }

    public void OpenChat()
    {
        if (isChatOpen) return;
        isChatOpen = true;

        // Position chat panel in VR space
        if (isInVRMode)
        {
            PositionVRPanel();
            SetRayInteractors(false);
        }

        chatPanel.SetActive(true);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(chatCanvasGroup, fadeInDuration, 1f));

        messageInputField.Select();
        messageInputField.ActivateInputField();
    }

    void PositionVRPanel()
    {
        Vector3 position = playerHead.position +
                         playerHead.forward * chatPanelDistance +
                         Vector3.up * chatPanelHeight;

        chatPanel.transform.position = position;

        // Make panel face player but keep it upright
        Vector3 lookDirection = playerHead.position - chatPanel.transform.position;
        lookDirection.y = 0;
        chatPanel.transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    void SetRayInteractors(bool enabled)
    {
        if (rayInteractors == null || rayInteractors.Length == 0) return;

        foreach (var interactor in rayInteractors)
        {
            if (interactor == null) continue;
            interactor.enabled = enabled;
        }
    }

    public void CloseChat()
    {
        if (!isChatOpen) return;
        isChatOpen = false;

        if (isInVRMode)
        {
            SetRayInteractors(true);
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(chatCanvasGroup, fadeOutDuration, 0f, () => chatPanel.SetActive(false)));
        messageInputField.text = string.Empty;
        messageInputField.DeactivateInputField();
    }

    IEnumerator Fade(CanvasGroup cg, float duration, float targetAlpha, System.Action onComplete = null)
    {
        float startAlpha = cg.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }
        cg.alpha = targetAlpha;
        onComplete?.Invoke();
    }

    void OnSubmitMessage(string msg)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            SendMessageToAPI();
    }

    void OnSendButtonClicked() => SendMessageToAPI();

    void SendMessageToAPI()
    {
        string msg = messageInputField.text.Trim();
        if (string.IsNullOrEmpty(msg)) { CloseChat(); return; }
        StartCoroutine(PostMessageCoroutine(msg));
        CloseChat();
    }

    IEnumerator PostMessageCoroutine(string message)
    {
        MessagePayload payload = new MessagePayload { message = message };
        string json = JsonUtility.ToJson(payload);
        Debug.Log("Sending JSON: " + json);

        using (UnityWebRequest req = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            Debug.Log("Response Code: " + req.responseCode);
            Debug.Log("Raw Response: " + req.downloadHandler.text);

            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    ResponseData data = JsonUtility.FromJson<ResponseData>(req.downloadHandler.text);
                    ShowResponse(data.response);
                }
                catch
                {
                    ShowResponse("Failed to parse response.");
                }
            }
            else
            {
                ShowResponse($"Error: {req.error}");
            }
        }
    }

    void ShowResponse(string text)
    {
        if (responsePanel == null || responseText == null) return;
        responseText.text = text;
        responsePanel.SetActive(true);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(responseCanvasGroup, fadeInDuration, 1f));

        // Position response panel in VR space
        if (isInVRMode)
        {
            PositionResponsePanel();
        }
    }

    void PositionResponsePanel()
    {
        Vector3 position = playerHead.position +
                         playerHead.forward * chatPanelDistance +
                         Vector3.up * (chatPanelHeight + 0.3f);

        responsePanel.transform.position = position;

        // Make panel face player but keep it upright
        Vector3 lookDirection = playerHead.position - responsePanel.transform.position;
        lookDirection.y = 0;
        responsePanel.transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    [System.Serializable]
    public class MessagePayload
    {
        public string message;
    }

    [System.Serializable]
    public class ResponseData
    {
        public string response;
    }
}