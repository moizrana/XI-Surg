// using UnityEngine;
// using UnityEngine.UI;

// namespace Meducator.Authentication
// {
//     /// <summary>
//     /// Automatically sets up the Meducator authentication system
//     /// </summary>
//     public class MeducatorAuthSetup : MonoBehaviour
//     {
//         [Header("Setup Configuration")]
//         public bool autoSetupOnStart = true;
//         public bool useFastApiAuth = true; // Toggle between FastAPI and Firebase
        
//         [Header("Medical Theme Colors")]
//         public Color medicalBlue = new Color(0.2f, 0.4f, 0.8f, 1f);
//         public Color medicalGreen = new Color(0.3f, 0.7f, 0.4f, 1f);
//         public Color medicalBackground = new Color(0.05f, 0.1f, 0.2f, 0.95f);
        
//         private void Start()
//         {
//             if (autoSetupOnStart)
//             {
//                 Invoke(nameof(CreateAuthenticationSystem), 0.5f); // Small delay to ensure everything is loaded
//             }
//         }
        
//         [ContextMenu("Create Meducator Authentication")]
//         public void CreateAuthenticationSystem()
//         {
//             Debug.Log("🩺 Meducator: Creating authentication system...");
            
//             // Check if system already exists
//             if (useFastApiAuth)
//             {
//                 if (FindObjectOfType<FastApiAuthManager>() != null)
//                 {
//                     Debug.Log("Authentication system already exists!");
//                     return;
//                 }
//             }
//             else if (FindObjectOfType<FirebaseAuthManager>() != null)
//             {
//                 Debug.Log("Authentication system already exists!");
//                 return;
//             }
            
//             // Create the authentication UI
//             CreateMeducatorUI();
            
//             Debug.Log("✅ Meducator authentication system ready!");
//         }
        
//         private void CreateMeducatorUI()
//         {
//             // 1. Create Main Canvas
//             GameObject canvas = CreateCanvas();
            
//             // 2. Create Background Panel
//             GameObject backgroundPanel = CreateBackgroundPanel(canvas);
            
//             // 3. Create Login Form
//             GameObject loginForm = CreateLoginForm(backgroundPanel);
            
//             // 4. Create Input Fields
//             CreateEmailField(loginForm);
//             CreatePasswordField(loginForm);
            
//             // 5. Create Buttons
//             CreateLoginButton(loginForm);
//             CreateRegisterButton(loginForm);
            
//             // 6. Create Status Text
//             CreateStatusText(loginForm);
            
//             // 7. Add Managers
//             AddAuthenticationManagers(canvas);
            
//             // 8. Connect Everything
//             ConnectUIElements(canvas);
//         }
        
//         private GameObject CreateCanvas()
//         {
//             GameObject canvasGO = new GameObject("Meducator Auth Canvas");
//             Canvas canvas = canvasGO.AddComponent<Canvas>();
//             canvas.renderMode = RenderMode.ScreenSpaceOverlay;
//             canvas.sortingOrder = 100;
            
//             CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
//             scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//             scaler.referenceResolution = new Vector2(1920, 1080);
            
//             canvasGO.AddComponent<GraphicRaycaster>();
//             canvasGO.AddComponent<CanvasGroup>();
            
//             return canvasGO;
//         }
        
//         private GameObject CreateBackgroundPanel(GameObject canvas)
//         {
//             GameObject panel = new GameObject("Background Panel");
//             panel.transform.SetParent(canvas.transform, false);
            
//             RectTransform rect = panel.AddComponent<RectTransform>();
//             rect.anchorMin = Vector2.zero;
//             rect.anchorMax = Vector2.one;
//             rect.sizeDelta = Vector2.zero;
//             rect.anchoredPosition = Vector2.zero;
            
//             Image image = panel.AddComponent<Image>();
//             image.color = medicalBackground;
            
//             return panel;
//         }
        
//         private GameObject CreateLoginForm(GameObject parent) 
//         {
//             GameObject form = new GameObject("Login Form");
//             form.transform.SetParent(parent.transform, false);
            
//             RectTransform rect = form.AddComponent<RectTransform>();
//             rect.sizeDelta = new Vector2(500, 600);
//             rect.anchoredPosition = Vector2.zero;
            
//             VerticalLayoutGroup layout = form.AddComponent<VerticalLayoutGroup>();
//             layout.childAlignment = TextAnchor.UpperCenter;
//             layout.childControlHeight = false;
//             layout.childControlWidth = false;
//             layout.childForceExpandHeight = false;
//             layout.childForceExpandWidth = false;
//             layout.spacing = 20f;
//             layout.padding = new RectOffset(40, 40, 40, 40);
            
//             return form;
//         }
        
//         private void CreateEmailField(GameObject parent)
//         {
//             GameObject field = CreateInputField(parent, "Email", "Enter your email");
//             field.name = "Email Field";
//             InputField inputField = field.GetComponent<InputField>();
//             inputField.contentType = InputField.ContentType.EmailAddress;
//         }
        
//         private void CreatePasswordField(GameObject parent)
//         {
//             GameObject field = CreateInputField(parent, "Password", "Enter your password");
//             field.name = "Password Field";
//             InputField inputField = field.GetComponent<InputField>();
//             inputField.contentType = InputField.ContentType.Password;
//             inputField.inputType = InputField.InputType.Password;
//         }
        
//         private GameObject CreateInputField(GameObject parent, string label, string placeholder)
//         {
//             // Main container
//             GameObject container = new GameObject($"{label} Container");
//             container.AddComponent<RectTransform>();
//             container.transform.SetParent(parent.transform, false);
            
//             RectTransform containerRect = container.GetComponent<RectTransform>();
//             containerRect.sizeDelta = new Vector2(420, 80);
            
//             VerticalLayoutGroup containerLayout = container.AddComponent<VerticalLayoutGroup>();
//             containerLayout.spacing = 5f;
//             containerLayout.childControlHeight = false;
//             containerLayout.childControlWidth = true;
            
//             // Label
//             GameObject labelGO = new GameObject($"{label} Label");
//             labelGO.transform.SetParent(container.transform, false);
            
//             RectTransform labelRect = labelGO.AddComponent<RectTransform>();
//             labelRect.sizeDelta = new Vector2(420, 20);
            
            
//             Text labelText = labelGO.AddComponent<Text>();
//             labelText.text = label;
//             labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
//             labelText.fontSize = 14;
//             labelText.color = Color.white;
//             labelText.alignment = TextAnchor.MiddleLeft;
            
//             // Input field
//             GameObject inputFieldGO = new GameObject($"{label} Input");
//             inputFieldGO.transform.SetParent(container.transform, false);
            
//             RectTransform inputRect = inputFieldGO.AddComponent<RectTransform>();
//             inputRect.sizeDelta = new Vector2(420, 50);
            
//             Image inputImage = inputFieldGO.AddComponent<Image>();
//             inputImage.color = new Color(0.9f, 0.95f, 1f, 0.9f);
            
//             // Use legacy InputField for FirebaseAuthManager compatibility
//             InputField inputField = inputFieldGO.AddComponent<InputField>();
            
//             // Placeholder
//             GameObject placeholderGO = new GameObject("Placeholder");
//             placeholderGO.transform.SetParent(inputFieldGO.transform, false);
//             RectTransform placeholderRect = placeholderGO.AddComponent<RectTransform>();
//             placeholderRect.anchorMin = Vector2.zero;
//             placeholderRect.anchorMax = Vector2.one;
//             placeholderRect.sizeDelta = Vector2.zero;
//             placeholderRect.offsetMin = new Vector2(15, 0);
//             placeholderRect.offsetMax = new Vector2(-15, 0);
            
//             Text placeholderText = placeholderGO.AddComponent<Text>();
//             placeholderText.text = placeholder;
//             placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
//             placeholderText.fontSize = 16;
//             placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
//             placeholderText.alignment = TextAnchor.MiddleLeft;
            
//             // Text
//             GameObject textGO = new GameObject("Text");
//             textGO.transform.SetParent(inputFieldGO.transform, false);
//             RectTransform textRect = textGO.AddComponent<RectTransform>();
//             textRect.anchorMin = Vector2.zero;
//             textRect.anchorMax = Vector2.one;
//             textRect.sizeDelta = Vector2.zero;
//             textRect.offsetMin = new Vector2(15, 0);
//             textRect.offsetMax = new Vector2(-15, 0);
            
//             Text textComponent = textGO.AddComponent<Text>();
//             textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
//             textComponent.fontSize = 16;
//             textComponent.color = Color.black;
//             textComponent.alignment = TextAnchor.MiddleLeft;
            
//             // Configure input field
//             inputField.textComponent = textComponent;
//             inputField.placeholder = placeholderText;
//             inputField.targetGraphic = inputImage;
            
//             // Title
//             if (label == "Email")
//             {
//                 GameObject titleGO = new GameObject("Meducator Title");
//                 titleGO.transform.SetParent(parent.transform, false);
                
//                 RectTransform titleRect = titleGO.AddComponent<RectTransform>();
//                 titleRect.sizeDelta = new Vector2(420, 60);
                
//                 Text titleText = titleGO.AddComponent<Text>();
//                 titleText.text = "🩺 Meducator";
//                 titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
//                 titleText.fontSize = 36;
//                 titleText.color = Color.white;
//                 titleText.fontStyle = FontStyle.Bold;
//                 titleText.alignment = TextAnchor.MiddleCenter;
//             }
            
//             return inputFieldGO;
//         }
        
//         private void CreateLoginButton(GameObject parent)
//         {
//             GameObject button = CreateButton(parent, "Sign In to Meducator", medicalGreen);
//             button.name = "Login Button";
//         }
        
//         private void CreateRegisterButton(GameObject parent)
//         {
//             GameObject button = CreateButton(parent, "New User? Register Here", medicalBlue);
//             button.name = "Register Button";
//         }
        
//         private GameObject CreateButton(GameObject parent, string text, Color color)
//         {
//             GameObject buttonGO = new GameObject($"{text} Button");
//             buttonGO.transform.SetParent(parent.transform, false);
            
//             RectTransform rect = buttonGO.AddComponent<RectTransform>();
//             rect.sizeDelta = new Vector2(200, 50);
            
//             Image image = buttonGO.AddComponent<Image>();
//             image.color = color;
            
//             Button button = buttonGO.AddComponent<Button>();
//             button.targetGraphic = image;
            
//             // Button text
//             GameObject textGO = new GameObject("Text");
//             textGO.transform.SetParent(buttonGO.transform, false);
//             RectTransform textRect = textGO.AddComponent<RectTransform>();
//             textRect.anchorMin = Vector2.zero;
//             textRect.anchorMax = Vector2.one;
//             textRect.sizeDelta = Vector2.zero;
            
//             Text textComponent = textGO.AddComponent<Text>();
//             textComponent.text = text;
//             textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
//             textComponent.fontSize = 14;
//             textComponent.color = Color.white;
//             textComponent.fontStyle = FontStyle.Bold;
//             textComponent.alignment = TextAnchor.MiddleCenter;
            
//             return buttonGO;
//         }
        
//         private void CreateStatusText(GameObject parent)
//         {
//             GameObject statusGO = new GameObject("Status Text");
//             statusGO.transform.SetParent(parent.transform, false);
            
//             RectTransform rect = statusGO.AddComponent<RectTransform>();
//             rect.sizeDelta = new Vector2(420, 30);
            
//             Text statusText = statusGO.AddComponent<Text>();
//             statusText.text = "Enter your website credentials to continue";
//             statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
//             statusText.fontSize = 14;
//             statusText.color = new Color(0.8f, 0.8f, 0.9f, 1f);
//             statusText.alignment = TextAnchor.MiddleCenter;
            
//             Debug.Log("Created status text");
//         }
        
//         private void AddAuthenticationManagers(GameObject canvas)
//         {
//             if (useFastApiAuth)
//             {
//                 FastApiAuthManager fastApi = canvas.AddComponent<FastApiAuthManager>();
//                 fastApi.apiBaseUrl = "https://meducator.onrender.com";
//                 fastApi.registrationWebUrl = "https://meducator.vercel.app/auth/login";
//             }
//             else
//             {
//                 FirebaseAuthManager firebase = canvas.AddComponent<FirebaseAuthManager>();
//                 firebase.firebaseDatabaseUrl = "https://meducator-c188d-default-rtdb.firebaseio.com/";
//             }

//             // Add LandingPageManager
//             LandingPageManager landingManager = canvas.AddComponent<LandingPageManager>();
            
//             Debug.Log("Added authentication managers");
//         }
        
//         private void ConnectUIElements(GameObject canvas)
//         {
//             if (useFastApiAuth)
//             {
//                 FastApiAuthManager authManager = canvas.GetComponent<FastApiAuthManager>();
                
//                 if (authManager != null)
//                 {
//                     GameObject emailField = GameObject.Find("Email Field");
//                     GameObject passwordField = GameObject.Find("Password Field");
//                     GameObject loginButton = GameObject.Find("Login Button");
//                     GameObject registerButton = GameObject.Find("Register Button");
//                     GameObject statusText = GameObject.Find("Status Text");
                    
//                     if (emailField != null)
// 					{
// 						authManager.emailInput = emailField.GetComponent<InputField>();
// 						var tmp = emailField.GetComponent<TMPro.TMP_InputField>();
// 						if (tmp != null) authManager.emailTMPInput = tmp;
// 					}
                    
//                     if (passwordField != null)
// 					{
// 						authManager.passwordInput = passwordField.GetComponent<InputField>();
// 						var tmp = passwordField.GetComponent<TMPro.TMP_InputField>();
// 						if (tmp != null) authManager.passwordTMPInput = tmp;
// 					}
                    
//                     if (loginButton != null)
//                         authManager.loginButton = loginButton.GetComponent<Button>();
                    
//                     if (registerButton != null)
//                         authManager.registerButton = registerButton.GetComponent<Button>();
                    
//                     if (statusText != null)
//                         authManager.statusText = statusText.GetComponent<Text>();
                    
//                     Debug.Log("✅ Connected UI elements to FastApiAuthManager");
//                 }
//                 return;
//             }

//             FirebaseAuthManager firebaseAuth = canvas.GetComponent<FirebaseAuthManager>();
            
//             if (firebaseAuth != null)
//             {
//                 GameObject emailField = GameObject.Find("Email Field");
//                 GameObject passwordField = GameObject.Find("Password Field");
//                 GameObject loginButton = GameObject.Find("Login Button");
//                 GameObject registerButton = GameObject.Find("Register Button");
//                 GameObject statusText = GameObject.Find("Status Text");
                
//                 if (emailField != null)
//                     firebaseAuth.emailInput = emailField.GetComponent<InputField>();
                
//                 if (passwordField != null)
//                     firebaseAuth.passwordInput = passwordField.GetComponent<InputField>();
                
//                 if (loginButton != null)
//                     firebaseAuth.loginButton = loginButton.GetComponent<Button>();
                
//                 if (registerButton != null)
//                     firebaseAuth.registerWebButton = registerButton.GetComponent<Button>();
                
//                 if (statusText != null)
//                     firebaseAuth.statusText = statusText.GetComponent<Text>();
                
//                 Debug.Log("✅ Connected UI elements to FirebaseAuthManager");
//             }
//         }
        
//         [ContextMenu("Clear Authentication System")]
//         public void ClearAuthenticationSystem()
//         {
//             GameObject canvas = GameObject.Find("Meducator Auth Canvas");
//             if (canvas != null)
//             {
//                 DestroyImmediate(canvas);
//                 Debug.Log("Cleared authentication system");
//             }
//         }
//     }
// }

using UnityEngine;
using UnityEngine.UI;

namespace Meducator.Authentication
{
    /// <summary>
    /// Automatically sets up the Meducator authentication system with enhanced UI
    /// </summary>
    public class MeducatorAuthSetup : MonoBehaviour
    {
        [Header("Setup Configuration")]
        public bool autoSetupOnStart = true;
        public bool useFastApiAuth = true; // Toggle between FastAPI and Firebase
        
        [Header("Medical Theme Colors")]
        public Color medicalBlue = new Color(0.2f, 0.5f, 0.9f, 1f);
        public Color medicalGreen = new Color(0.2f, 0.8f, 0.5f, 1f);
        public Color medicalBackground = new Color(0.05f, 0.08f, 0.15f, 1f);
        public Color cardBackground = new Color(0.12f, 0.15f, 0.22f, 0.95f);
        public Color accentGlow = new Color(0.3f, 0.7f, 1f, 0.3f);
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                Invoke(nameof(CreateAuthenticationSystem), 0.5f);
            }
        }
        
        [ContextMenu("Create Meducator Authentication")]
        public void CreateAuthenticationSystem()
        {
            Debug.Log("🩺 Meducator: Creating enhanced authentication system...");
            
            // Check if system already exists
            if (useFastApiAuth)
            {
                if (FindObjectOfType<FastApiAuthManager>() != null)
                {
                    Debug.Log("Authentication system already exists!");
                    return;
                }
            }
            else if (FindObjectOfType<FirebaseAuthManager>() != null)
            {
                Debug.Log("Authentication system already exists!");
                return;
            }
            
            CreateMeducatorUI();
            Debug.Log("✅ Meducator authentication system ready!");
        }
        
        private void CreateMeducatorUI()
        {
            GameObject canvas = CreateCanvas();
            GameObject backgroundPanel = CreateBackgroundPanel(canvas);
            GameObject loginCard = CreateLoginCard(backgroundPanel);
            
            CreateTitle(loginCard);
            CreateSubtitle(loginCard);
            CreateEmailField(loginCard);
            CreatePasswordField(loginCard);
            CreateLoginButton(loginCard);
            CreateRegisterButton(loginCard);
            CreateStatusText(loginCard);
            CreateDecorativeElements(backgroundPanel);
            
            AddAuthenticationManagers(canvas);
            ConnectUIElements(canvas);
        }
        
        private GameObject CreateCanvas()
        {
            GameObject canvasGO = new GameObject("Meducator Auth Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasGO.AddComponent<GraphicRaycaster>();
            canvasGO.AddComponent<CanvasGroup>();
            
            return canvasGO;
        }
        
        private GameObject CreateBackgroundPanel(GameObject canvas)
        {
            GameObject panel = new GameObject("Background Panel");
            panel.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            Image image = panel.AddComponent<Image>();
            image.color = medicalBackground;
            
            return panel;
        }
        
        private GameObject CreateLoginCard(GameObject parent)
        {
            GameObject card = new GameObject("Login Card");
            card.transform.SetParent(parent.transform, false);
            
            RectTransform rect = card.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(550, 700);
            rect.anchoredPosition = Vector2.zero;
            
            Image cardImage = card.AddComponent<Image>();
            cardImage.color = cardBackground;
            
            Shadow shadow = card.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(0, -8);
            
            VerticalLayoutGroup layout = card.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            layout.spacing = 15f;
            layout.padding = new RectOffset(50, 50, 50, 40);
            
            return card;
        }
        
        private void CreateTitle(GameObject parent)
        {
            GameObject titleGO = new GameObject("Meducator Title");
            titleGO.transform.SetParent(parent.transform, false);
            
            RectTransform titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(450, 80);
            
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = "🩺 MEDUCATOR";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 48;
            titleText.color = Color.white;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            Outline outline = titleGO.AddComponent<Outline>();
            outline.effectColor = medicalBlue;
            outline.effectDistance = new Vector2(2, -2);
        }
        
        private void CreateSubtitle(GameObject parent)
        {
            GameObject subtitleGO = new GameObject("Subtitle");
            subtitleGO.transform.SetParent(parent.transform, false);
            
            RectTransform subtitleRect = subtitleGO.AddComponent<RectTransform>();
            subtitleRect.sizeDelta = new Vector2(450, 35);
            
            Text subtitleText = subtitleGO.AddComponent<Text>();
            subtitleText.text = "Medical Education Platform";
            subtitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            subtitleText.fontSize = 18;
            subtitleText.color = new Color(0.7f, 0.8f, 0.95f, 0.9f);
            subtitleText.alignment = TextAnchor.MiddleCenter;
            subtitleText.fontStyle = FontStyle.Italic;
        }
        
        private void CreateEmailField(GameObject parent)
        {
            GameObject container = CreateInputField(parent, "Email Address", "your.email@example.com", false);
            container.name = "Email Container";
            
            // Find the actual input field and rename it
            InputField inputField = container.GetComponentInChildren<InputField>();
            if (inputField != null)
            {
                inputField.gameObject.name = "Email Field";
                inputField.contentType = InputField.ContentType.EmailAddress;
            }
        }
        
        private void CreatePasswordField(GameObject parent)
        {
            GameObject container = CreateInputField(parent, "Password", "••••••••", true);
            container.name = "Password Container";
            
            // Find the actual input field and rename it
            InputField inputField = container.GetComponentInChildren<InputField>();
            if (inputField != null)
            {
                inputField.gameObject.name = "Password Field";
                inputField.contentType = InputField.ContentType.Password;
                inputField.inputType = InputField.InputType.Password;
            }
        }
        
        private GameObject CreateInputField(GameObject parent, string label, string placeholder, bool isPassword)
        {
            GameObject container = new GameObject($"{label} Container");
            container.AddComponent<RectTransform>();
            container.transform.SetParent(parent.transform, false);
            
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(450, 95);
            
            VerticalLayoutGroup containerLayout = container.AddComponent<VerticalLayoutGroup>();
            containerLayout.spacing = 8f;
            containerLayout.childControlHeight = false;
            containerLayout.childControlWidth = true;
            
            // Label with icon
            GameObject labelGO = new GameObject($"{label} Label");
            labelGO.transform.SetParent(container.transform, false);
            
            RectTransform labelRect = labelGO.AddComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(450, 25);
            
            Text labelText = labelGO.AddComponent<Text>();
            string icon = isPassword ? "🔒" : "📧";
            labelText.text = $"{icon} {label}";
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 16;
            labelText.color = new Color(0.85f, 0.9f, 1f, 1f);
            labelText.alignment = TextAnchor.MiddleLeft;
            labelText.fontStyle = FontStyle.Bold;
            
            // Input field with enhanced styling
            GameObject inputFieldGO = new GameObject($"{label} Input");
            inputFieldGO.transform.SetParent(container.transform, false);
            
            RectTransform inputRect = inputFieldGO.AddComponent<RectTransform>();
            inputRect.sizeDelta = new Vector2(450, 55);
            
            Image inputImage = inputFieldGO.AddComponent<Image>();
            inputImage.color = new Color(0.18f, 0.22f, 0.3f, 1f);
            
            Outline inputOutline = inputFieldGO.AddComponent<Outline>();
            inputOutline.effectColor = new Color(0.3f, 0.5f, 0.8f, 0.5f);
            inputOutline.effectDistance = new Vector2(0, 0);
            
            InputField inputField = inputFieldGO.AddComponent<InputField>();
            
            // Placeholder
            GameObject placeholderGO = new GameObject("Placeholder");
            placeholderGO.transform.SetParent(inputFieldGO.transform, false);
            RectTransform placeholderRect = placeholderGO.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.sizeDelta = Vector2.zero;
            placeholderRect.offsetMin = new Vector2(20, 0);
            placeholderRect.offsetMax = new Vector2(-20, 0);
            
            Text placeholderText = placeholderGO.AddComponent<Text>();
            placeholderText.text = placeholder;
            placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            placeholderText.fontSize = 16;
            placeholderText.color = new Color(0.45f, 0.5f, 0.6f, 1f);
            placeholderText.alignment = TextAnchor.MiddleLeft;
            placeholderText.fontStyle = FontStyle.Italic;
            
            // Text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(inputFieldGO.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(20, 0);
            textRect.offsetMax = new Vector2(-20, 0);
            
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = 17;
            textComponent.color = new Color(0.95f, 0.97f, 1f, 1f);
            textComponent.alignment = TextAnchor.MiddleLeft;
            
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderText;
            inputField.targetGraphic = inputImage;
            
            return inputFieldGO;
        }
        
        private void CreateLoginButton(GameObject parent)
        {
            GameObject button = CreateStyledButton(parent, "Sign In", medicalGreen, "Login Button");
            
            // Add subtle animation effect via Shadow component
            Shadow shadow = button.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.4f);
            shadow.effectDistance = new Vector2(0, -4);
        }
        
        private void CreateRegisterButton(GameObject parent)
        {
            GameObject button = CreateStyledButton(parent, "New User? Register Here", medicalBlue, "Register Button");
            
            Image btnImage = button.GetComponent<Image>();
            btnImage.color = new Color(medicalBlue.r * 0.5f, medicalBlue.g * 0.5f, medicalBlue.b * 0.5f, 0.8f);
            
            Outline outline = button.AddComponent<Outline>();
            outline.effectColor = medicalBlue;
            outline.effectDistance = new Vector2(0, 0);
        }
        
        private GameObject CreateStyledButton(GameObject parent, string text, Color color, string objectName)
        {
            GameObject buttonGO = new GameObject(objectName);
            buttonGO.transform.SetParent(parent.transform, false);
            
            RectTransform rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(450, 60);
            
            Image image = buttonGO.AddComponent<Image>();
            image.color = color;
            
            Button button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;
            
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            button.colors = colors;
            
            // Button text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.text = text.ToUpper();
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = 16;
            textComponent.color = Color.white;
            textComponent.fontStyle = FontStyle.Bold;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            Shadow textShadow = textGO.AddComponent<Shadow>();
            textShadow.effectColor = new Color(0, 0, 0, 0.5f);
            textShadow.effectDistance = new Vector2(1, -1);
            
            return buttonGO;
        }
        
        private void CreateStatusText(GameObject parent)
        {
            GameObject statusGO = new GameObject("Status Text");
            statusGO.transform.SetParent(parent.transform, false);
            
            RectTransform rect = statusGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(450, 40);
            
            Text statusText = statusGO.AddComponent<Text>();
            statusText.text = "Welcome to Meducator";
            statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            statusText.fontSize = 15;
            statusText.color = new Color(0.7f, 0.8f, 0.95f, 1f);
            statusText.alignment = TextAnchor.MiddleCenter;
            statusText.fontStyle = FontStyle.Italic;
            
            Debug.Log("Created enhanced status text");
        }
        
        private void CreateDecorativeElements(GameObject parent)
        {
            // Top left decorative circle
            CreateDecorativeCircle(parent, new Vector2(-700, 400), 150f, new Color(0.2f, 0.5f, 0.9f, 0.15f));
            
            // Top right decorative circle
            CreateDecorativeCircle(parent, new Vector2(700, 450), 200f, new Color(0.2f, 0.8f, 0.5f, 0.12f));
            
            // Bottom left decorative circle
            CreateDecorativeCircle(parent, new Vector2(-650, -350), 180f, new Color(0.3f, 0.7f, 1f, 0.1f));
            
            // Bottom right decorative circle
            CreateDecorativeCircle(parent, new Vector2(750, -400), 160f, new Color(0.5f, 0.3f, 0.9f, 0.13f));
        }
        
        private void CreateDecorativeCircle(GameObject parent, Vector2 position, float size, Color color)
        {
            GameObject circle = new GameObject("Decorative Circle");
            circle.transform.SetParent(parent.transform, false);
            
            RectTransform rect = circle.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = position;
            
            Image image = circle.AddComponent<Image>();
            image.color = color;
            
            // Create a simple circular sprite
            Texture2D tex = new Texture2D(128, 128);
            Color[] pixels = new Color[128 * 128];
            Vector2 center = new Vector2(64, 64);
            
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01(1f - (distance / 64f));
                    pixels[y * 128 + x] = new Color(1, 1, 1, alpha);
                }
            }
            
            tex.SetPixels(pixels);
            tex.Apply();
            
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
        }
        
        private void AddAuthenticationManagers(GameObject canvas)
        {
            if (useFastApiAuth)
            {
                FastApiAuthManager fastApi = canvas.AddComponent<FastApiAuthManager>();
                fastApi.apiBaseUrl = "https://meducator.onrender.com";
                fastApi.registrationWebUrl = "https://meducator.vercel.app/auth/login";
            }
            else
            {
                FirebaseAuthManager firebase = canvas.AddComponent<FirebaseAuthManager>();
                firebase.firebaseDatabaseUrl = "https://meducator-c188d-default-rtdb.firebaseio.com/";
            }

            LandingPageManager landingManager = canvas.AddComponent<LandingPageManager>();
            
            Debug.Log("Added authentication managers");
        }
        
        private void ConnectUIElements(GameObject canvas)
        {
            if (useFastApiAuth)
            {
                FastApiAuthManager authManager = canvas.GetComponent<FastApiAuthManager>();
                
                if (authManager != null)
                {
                    GameObject emailField = GameObject.Find("Email Field");
                    GameObject passwordField = GameObject.Find("Password Field");
                    GameObject loginButton = GameObject.Find("Login Button");
                    GameObject registerButton = GameObject.Find("Register Button");
                    GameObject statusText = GameObject.Find("Status Text");
                    
                    if (emailField != null)
                    {
                        authManager.emailInput = emailField.GetComponent<InputField>();
                        var tmp = emailField.GetComponent<TMPro.TMP_InputField>();
                        if (tmp != null) authManager.emailTMPInput = tmp;
                    }
                    
                    if (passwordField != null)
                    {
                        authManager.passwordInput = passwordField.GetComponent<InputField>();
                        var tmp = passwordField.GetComponent<TMPro.TMP_InputField>();
                        if (tmp != null) authManager.passwordTMPInput = tmp;
                    }
                    
                    if (loginButton != null)
                        authManager.loginButton = loginButton.GetComponent<Button>();
                    
                    if (registerButton != null)
                        authManager.registerButton = registerButton.GetComponent<Button>();
                    
                    if (statusText != null)
                        authManager.statusText = statusText.GetComponent<Text>();
                    
                    Debug.Log("✅ Connected UI elements to FastApiAuthManager");
                }
                return;
            }

            FirebaseAuthManager firebaseAuth = canvas.GetComponent<FirebaseAuthManager>();
            
            if (firebaseAuth != null)
            {
                GameObject emailField = GameObject.Find("Email Field");
                GameObject passwordField = GameObject.Find("Password Field");
                GameObject loginButton = GameObject.Find("Login Button");
                GameObject registerButton = GameObject.Find("Register Button");
                GameObject statusText = GameObject.Find("Status Text");
                
                if (emailField != null)
                    firebaseAuth.emailInput = emailField.GetComponent<InputField>();
                
                if (passwordField != null)
                    firebaseAuth.passwordInput = passwordField.GetComponent<InputField>();
                
                if (loginButton != null)
                    firebaseAuth.loginButton = loginButton.GetComponent<Button>();
                
                if (registerButton != null)
                    firebaseAuth.registerWebButton = registerButton.GetComponent<Button>();
                
                if (statusText != null)
                    firebaseAuth.statusText = statusText.GetComponent<Text>();
                
                Debug.Log("✅ Connected UI elements to FirebaseAuthManager");
            }
        }
        
        [ContextMenu("Clear Authentication System")]
        public void ClearAuthenticationSystem()
        {
            GameObject canvas = GameObject.Find("Meducator Auth Canvas");
            if (canvas != null)
            {
                DestroyImmediate(canvas);
                Debug.Log("Cleared authentication system");
            }
        }
    }
}