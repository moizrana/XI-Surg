using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Meducator.Authentication
{
    public class LandingPageManager : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject authenticationPanel;
        public GameObject loadingPanel;
        
        [Header("Authentication UI")]
        public InputField emailInputField;
        public InputField passwordInputField;
        public Button loginButton;
        public Button registerButton;
        public Text statusText;
        public GameObject loadingIndicator;
        
        [Header("Password Toggle")]
        public Button togglePasswordButton;
        public Text togglePasswordLabel; // optional: shows "Show"/"Hide"
        
        [Header("Visual Elements")]
        public Image backgroundImage;
        public CanvasGroup canvasGroup;
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Settings")]
        public float transitionDuration = 1f;
        public string mainSceneName = "SelectionScene";
        
        private FastApiAuthManager authManager;
        private bool isTransitioning = false;
        
        private void Start()
        {
            InitializeComponents();
            SetupUI();
            StartCoroutine(FadeInAnimation());
        }
        
        private void InitializeComponents()
        {
            authManager = FindObjectOfType<FastApiAuthManager>();
            
            if (authManager == null)
            {
                Debug.LogError("FastApiAuthManager not found! Please add it to the scene.");
                return;
            }
            
            // Connect UI elements to the auth manager (only if provided locally)
            if (loginButton != null) authManager.loginButton = loginButton;
            if (registerButton != null) authManager.registerButton = registerButton;
            if (emailInputField != null) authManager.emailInput = emailInputField;
            if (passwordInputField != null) authManager.passwordInput = passwordInputField;
            if (statusText != null) authManager.statusText = statusText;
            if (loadingIndicator != null) authManager.loadingIndicator = loadingIndicator;
            if (!string.IsNullOrEmpty(mainSceneName)) authManager.mainSceneName = mainSceneName;
            authManager.RebindUI();
            
            // Subscribe to authentication events
            FastApiAuthManager.OnAuthenticationChanged += HandleAuthenticationChanged;
            FastApiAuthManager.OnAuthStatusChanged += HandleAuthStatusChanged;
        }
        
        private void SetupUI()
        {
            // Configure input fields
            if (emailInputField != null)
            {
                emailInputField.placeholder.GetComponent<Text>().text = "Enter your email";
                emailInputField.contentType = InputField.ContentType.EmailAddress;
            }
            
            if (passwordInputField != null)
            {
                passwordInputField.placeholder.GetComponent<Text>().text = "Enter your password";
                passwordInputField.contentType = InputField.ContentType.Password;
                passwordInputField.inputType = InputField.InputType.Password;
            }
            
            // Toggle password visibility button
            if (togglePasswordButton != null)
            {
                togglePasswordButton.onClick.RemoveAllListeners();
                togglePasswordButton.onClick.AddListener(TogglePasswordVisibility);
                UpdateTogglePasswordLabel();
            }
            else if (passwordInputField != null)
            {
                // Auto-create a small Show/Hide button if not assigned
                AutoCreatePasswordToggleButton();
                UpdateTogglePasswordLabel();
            }
            
            // Configure buttons
            if (loginButton != null)
            {
                var buttonText = loginButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = "Sign In";
            }
            
            if (registerButton != null)
            {
                var buttonText = registerButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = "New User? Register Here";
            }
            
            // Setup canvas group for fading
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }
        
        private IEnumerator FadeInAnimation()
        {
            yield return new WaitForSeconds(0.5f);
            
            float elapsedTime = 0f;
            while (elapsedTime < transitionDuration)
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = fadeCurve.Evaluate(elapsedTime / transitionDuration);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }
        
        private void HandleAuthenticationChanged(bool isAuthenticated)
        {
            if (isAuthenticated && !isTransitioning)
            {
                StartCoroutine(TransitionToMainScene());
            }
        }
        
        private void HandleAuthStatusChanged(string message)
        {
            // Handle status updates from the auth manager
            Debug.Log($"Auth Status: {message}");
        }
        
        private IEnumerator TransitionToMainScene()
        {
            if (isTransitioning) yield break;
            
            isTransitioning = true;
            
            // Fade out animation
            float elapsedTime = 0f;
            while (elapsedTime < transitionDuration)
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - fadeCurve.Evaluate(elapsedTime / transitionDuration);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
            
            // Load the main scene
            SceneManager.LoadScene(mainSceneName);
        }
        
        public void OnEmailFieldSubmit()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (passwordInputField != null)
                    passwordInputField.Select();
                else if (loginButton != null && loginButton.interactable)
                    loginButton.onClick.Invoke();
            }
        }
        
        public void OnPasswordFieldSubmit()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (loginButton != null && loginButton.interactable)
                    loginButton.onClick.Invoke();
            }
        }
        
        private bool isPasswordVisible = false;
        
        private void TogglePasswordVisibility()
        {
            if (passwordInputField == null) return;
            isPasswordVisible = !isPasswordVisible;
            
            if (isPasswordVisible)
            {
                passwordInputField.inputType = InputField.InputType.Standard;
                passwordInputField.contentType = InputField.ContentType.Standard;
            }
            else
            {
                passwordInputField.inputType = InputField.InputType.Password;
                passwordInputField.contentType = InputField.ContentType.Password;
            }
            
            // Refresh shown text
            var current = passwordInputField.text;
            passwordInputField.text = string.Empty;
            passwordInputField.text = current;
            UpdateTogglePasswordLabel();
        }
        
        private void UpdateTogglePasswordLabel()
        {
            if (togglePasswordLabel == null) return;
            togglePasswordLabel.text = isPasswordVisible ? "Hide" : "Show";
        }

        private void AutoCreatePasswordToggleButton()
        {
            if (passwordInputField == null)
            {
                Debug.LogWarning("LandingPageManager: passwordInputField is null, cannot create toggle button.");
                return;
            }
            
            RectTransform passwordRT = passwordInputField.GetComponent<RectTransform>();
            if (passwordRT == null)
            {
                Debug.LogWarning("LandingPageManager: passwordInputField has no RectTransform.");
                return;
            }
            
            // Find the Canvas to ensure proper rendering order
            Canvas canvas = passwordRT.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("LandingPageManager: Could not find Canvas.");
                return;
            }
            
            // Create button as a sibling of the password field (same parent)
            // This ensures it renders on top and is clickable
            Transform parentTransform = passwordRT.parent;
            if (parentTransform == null) parentTransform = passwordRT.root;
            
            GameObject btnGO = new GameObject("Show Password Button");
            btnGO.transform.SetParent(parentTransform, false);
            
            // Place it right after the password field in the hierarchy (renders on top)
            int passwordIndex = passwordRT.GetSiblingIndex();
            btnGO.transform.SetSiblingIndex(passwordIndex + 1);
            
            RectTransform rt = btnGO.AddComponent<RectTransform>();
            
            // Copy password field's anchors to match its layout group behavior
            rt.anchorMin = passwordRT.anchorMin;
            rt.anchorMax = passwordRT.anchorMax;
            rt.pivot = new Vector2(1f, 0.5f);
            
            // Position: right edge of password field + small offset for spacing
            Vector2 passwordPos = passwordRT.anchoredPosition;
            float buttonWidth = 80f;
            float buttonX = passwordPos.x + (passwordRT.sizeDelta.x / 2f) + (buttonWidth / 2f) + 5f;
            
            rt.anchoredPosition = new Vector2(buttonX, passwordPos.y);
            rt.sizeDelta = new Vector2(buttonWidth, passwordRT.sizeDelta.y);
            
            // Make sure button is active and visible
            btnGO.SetActive(true);
            
            // Add Image component for button background
            Image img = btnGO.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.9f, 0.9f);
            
            // Add Button component
            Button btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(TogglePasswordVisibility);
            
            // Add hover color transition
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(0.3f, 0.6f, 1f, 1f);
            colors.pressedColor = new Color(0.1f, 0.3f, 0.7f, 1f);
            btn.colors = colors;
            
            // Create label text
            GameObject labelGO = new GameObject("Text");
            labelGO.transform.SetParent(btnGO.transform, false);
            RectTransform lrt = labelGO.AddComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.sizeDelta = Vector2.zero;
            lrt.anchoredPosition = Vector2.zero;
            
            Text label = labelGO.AddComponent<Text>();
            label.text = "Show";
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 14;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.fontStyle = FontStyle.Bold;
            
            // Assign references
            togglePasswordButton = btn;
            togglePasswordLabel = label;
            
            Debug.Log("LandingPageManager: Auto-created Show/Hide password button at position: " + rt.position);
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            FastApiAuthManager.OnAuthenticationChanged -= HandleAuthenticationChanged;
            FastApiAuthManager.OnAuthStatusChanged -= HandleAuthStatusChanged;
        }
        
        private void Update()
        {
            // Handle escape key to quit
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}