using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

namespace Meducator.Selection
{
    public class SelectionSceneManager : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject modeSelectionPanel;
        public GameObject surgerySelectionPanel;
        public GameObject userInfoPanel;
        
        [Header("Mode Selection UI")]
        public Button trainingModeButton;
        public Button guidedModeButton;
        public Text modeTitle;
        
        [Header("Surgery Category UI")]
        public GameObject basicSurgeriesPanel;
        public GameObject advancedSurgeriesPanel;
        public Button basicCategoryButton;
        public Button advancedCategoryButton;
        
        [Header("Basic Surgery Buttons")]
        public Button suturingButton;
        public Button injectionButton;
        
        [Header("Advanced Surgery Buttons")]
        public Button laparoscopicButton;
        public Button tibialNailingButton;
        public Button pacemakerButton;
        
        [Header("User Info UI")]
        public Text userNameText;
        public Button logoutButton;
        
        [Header("Visual Elements")]
        public CanvasGroup canvasGroup;
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Settings")]
        public float transitionDuration = 0.5f;
        public string sampleSceneName = "SampleScene";
        
        private string selectedMode = "";
        private bool isTransitioning = false;
        
        private void Start()
        {
            ConfigureCanvasForVR();
            InitializeUI();
			AutoFindCategoryButtonsIfMissing();
            LoadUserInfo();
            EnsureEmojiCategoryLabels();
			ApplyEmojiFontIfAvailable(basicCategoryButton);
			ApplyEmojiFontIfAvailable(advancedCategoryButton);
			EnsureSurgeryCardIcons();
            StartCoroutine(FadeInAnimation());
        }
        
        private void InitializeUI()
        {
            // Show mode selection first, hide surgery selection
            if (modeSelectionPanel != null)
                modeSelectionPanel.SetActive(true);
            
            if (surgerySelectionPanel != null)
                surgerySelectionPanel.SetActive(false);
            
            // Setup button listeners
            if (trainingModeButton != null)
                trainingModeButton.onClick.AddListener(() => SelectMode("Training"));
            
            if (guidedModeButton != null)
                guidedModeButton.onClick.AddListener(() => SelectMode("Guided"));
            
            if (basicCategoryButton != null)
                basicCategoryButton.onClick.AddListener(() => ShowCategory("Basic"));
            
            if (advancedCategoryButton != null)
                advancedCategoryButton.onClick.AddListener(() => ShowCategory("Advanced"));
            
            // Basic surgery buttons
            if (suturingButton != null)
                suturingButton.onClick.AddListener(() => SelectSurgery("Suturing"));
            
            if (injectionButton != null)
                injectionButton.onClick.AddListener(() => SelectSurgery("Injection"));
            
            // Advanced surgery buttons
            if (laparoscopicButton != null)
                laparoscopicButton.onClick.AddListener(() => SelectSurgery("Laparoscopic Appendectomy"));
            
            if (tibialNailingButton != null)
                tibialNailingButton.onClick.AddListener(() => SelectSurgery("Intramedullary Tibial Nailing"));
            
            if (pacemakerButton != null)
                pacemakerButton.onClick.AddListener(() => SelectSurgery("Pacemaker Implantation"));
            
            // Logout button
            if (logoutButton != null)
                logoutButton.onClick.AddListener(Logout);
            
            // Initially hide both surgery category panels
            if (basicSurgeriesPanel != null)
                basicSurgeriesPanel.SetActive(false);
            
            if (advancedSurgeriesPanel != null)
                advancedSurgeriesPanel.SetActive(false);
            
            // Setup canvas group for fading
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }
        
        private void LoadUserInfo()
        {
            string userEmail = PlayerPrefs.GetString("UserEmail", "User");
            string firstName = "";
            
            // Try to get user data from PlayerPrefs
            string userData = PlayerPrefs.GetString("UserData", "");
            if (!string.IsNullOrEmpty(userData))
            {
                try
                {
                    var user = JsonUtility.FromJson<UserData>(userData);
                    if (!string.IsNullOrEmpty(user.first_name))
                        firstName = user.first_name;
                    else if (!string.IsNullOrEmpty(user.email))
                        firstName = user.email.Split('@')[0];
                }
                catch
                {
                    firstName = userEmail.Split('@')[0];
                }
            }
            else
            {
                firstName = userEmail.Split('@')[0];
            }
            
            // Capitalize first letter
            if (!string.IsNullOrEmpty(firstName))
                firstName = char.ToUpper(firstName[0]) + firstName.Substring(1);
            
            if (userNameText != null)
                userNameText.text = $"Welcome, {firstName}";
        }
        
        private void SelectMode(string mode)
        {
            selectedMode = mode;
            PlayerPrefs.SetString("SelectedMode", mode);
            PlayerPrefs.Save();
            
            Debug.Log($"Selected Mode: {mode}");
            
            // Transition to surgery selection
            StartCoroutine(TransitionToSurgerySelection());
        }
        
        private IEnumerator TransitionToSurgerySelection()
        {
            if (isTransitioning) yield break;
            isTransitioning = true;
            
            // Fade out mode selection
            if (modeSelectionPanel != null)
            {
                CanvasGroup modeCanvasGroup = modeSelectionPanel.GetComponent<CanvasGroup>();
                if (modeCanvasGroup == null)
                    modeCanvasGroup = modeSelectionPanel.AddComponent<CanvasGroup>();
                
                float elapsedTime = 0f;
                while (elapsedTime < transitionDuration)
                {
                    modeCanvasGroup.alpha = 1f - (elapsedTime / transitionDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                
                modeSelectionPanel.SetActive(false);
            }
            
            // Show surgery selection
            if (surgerySelectionPanel != null)
            {
                surgerySelectionPanel.SetActive(true);
                
                CanvasGroup surgeryCanvasGroup = surgerySelectionPanel.GetComponent<CanvasGroup>();
                if (surgeryCanvasGroup == null)
                    surgeryCanvasGroup = surgerySelectionPanel.AddComponent<CanvasGroup>();
                
                surgeryCanvasGroup.alpha = 0f;
                
                float elapsedTime = 0f;
                while (elapsedTime < transitionDuration)
                {
                    surgeryCanvasGroup.alpha = elapsedTime / transitionDuration;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                
                surgeryCanvasGroup.alpha = 1f;
            }
            
            isTransitioning = false;
        }
        
        private void ShowCategory(string category)
        {
            if (category == "Basic")
            {
                if (basicSurgeriesPanel != null)
                    basicSurgeriesPanel.SetActive(true);
                
                if (advancedSurgeriesPanel != null)
                    advancedSurgeriesPanel.SetActive(false);
            }
            else if (category == "Advanced")
            {
                if (basicSurgeriesPanel != null)
                    basicSurgeriesPanel.SetActive(false);
                
                if (advancedSurgeriesPanel != null)
                    advancedSurgeriesPanel.SetActive(true);
            }
        }
        
        private void SelectSurgery(string surgeryName)
        {
            PlayerPrefs.SetString("SelectedSurgery", surgeryName);
            PlayerPrefs.Save();
            
            Debug.Log($"Selected Surgery: {surgeryName} in {selectedMode} mode");
            
            // For Suturing, load SampleScene
            if (surgeryName == "Suturing")
            {
                StartCoroutine(TransitionToScene(sampleSceneName));
            }
            else
            {
                // For other surgeries, show a message (you can implement their scenes later)
                Debug.Log($"{surgeryName} scene not yet implemented");
                // You can add a status text to show this message to the user
            }
        }
        
        private IEnumerator TransitionToScene(string sceneName)
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
            
            // Load the scene
            SceneManager.LoadScene(sceneName);
        }
        
        private IEnumerator FadeInAnimation()
        {
            yield return new WaitForSeconds(0.2f);
            
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
        
        private void Logout()
        {
            PlayerPrefs.DeleteKey("JwtToken");
            PlayerPrefs.DeleteKey("UserEmail");
            PlayerPrefs.DeleteKey("UserId");
            PlayerPrefs.DeleteKey("UserData");
            PlayerPrefs.DeleteKey("SelectedMode");
            PlayerPrefs.DeleteKey("SelectedSurgery");
            PlayerPrefs.Save();
            
            SceneManager.LoadScene("LandingPageScene");
        }
        
        private void EnsureEmojiCategoryLabels()
        {
            // Ensure category buttons display emoji-prefixed labels regardless of manual scene setup
            SetButtonLabelText(basicCategoryButton, "📚 BASIC SURGERIES");
            SetButtonLabelText(advancedCategoryButton, "🔬 ADVANCED SURGERIES");
        }
        
        private void SetButtonLabelText(Button targetButton, string label)
        {
            if (targetButton == null) return;
            
            // Legacy UI Text
            Text legacyText = targetButton.GetComponentInChildren<Text>(true);
            if (legacyText != null)
            {
                legacyText.text = label;
            }
			
			// TextMeshProUGUI (access via reflection if TMP exists without compile symbol)
			Type tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
			if (tmpType != null)
			{
				var tmp = targetButton.GetComponentInChildren(tmpType, true);
				if (tmp != null)
				{
					var textProp = tmpType.GetProperty("text");
					if (textProp != null && textProp.CanWrite)
					{
						textProp.SetValue(tmp, label, null);
					}
				}
			}
        }

		private void AutoFindCategoryButtonsIfMissing()
		{
			// If buttons were not wired in the inspector (manual canvas), try to locate them by name/text
			if (basicCategoryButton != null && advancedCategoryButton != null) return;
			
			Button[] allButtons = GameObject.FindObjectsOfType<Button>(true);
			foreach (var btn in allButtons)
			{
				string nameUpper = btn.gameObject.name.ToUpperInvariant();
				string labelUpper = GetButtonCurrentLabelUpper(btn);
				
				if (basicCategoryButton == null)
				{
					if (nameUpper.Contains("BASIC") || labelUpper.Contains("BASIC"))
					{
						basicCategoryButton = btn;
					}
				}
				if (advancedCategoryButton == null)
				{
					if (nameUpper.Contains("ADVANCED") || labelUpper.Contains("ADVANCED"))
					{
						advancedCategoryButton = btn;
					}
				}
				if (basicCategoryButton != null && advancedCategoryButton != null) break;
			}
			
			// Re-wire listeners if we just found them
			if (basicCategoryButton != null && basicCategoryButton.onClick != null)
			{
				basicCategoryButton.onClick.RemoveAllListeners();
				basicCategoryButton.onClick.AddListener(() => ShowCategory("Basic"));
			}
			if (advancedCategoryButton != null && advancedCategoryButton.onClick != null)
			{
				advancedCategoryButton.onClick.RemoveAllListeners();
				advancedCategoryButton.onClick.AddListener(() => ShowCategory("Advanced"));
			}
		}

		private void ApplyEmojiFontIfAvailable(Button targetButton)
		{
			if (targetButton == null) return;
			// Try legacy Text first
			Text legacyText = targetButton.GetComponentInChildren<Text>(true);
			if (legacyText != null)
			{
				Font emojiFont = CreateEmojiSystemFont();
				if (emojiFont != null)
				{
					legacyText.font = emojiFont;
				}
			}
			
			// Try TMP via reflection (no hard dependency)
			Type tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
			Type tmpFontAssetType = Type.GetType("TMPro.TMP_FontAsset, Unity.TextMeshPro");
			if (tmpType != null && tmpFontAssetType != null)
			{
				var tmp = targetButton.GetComponentInChildren(tmpType, true);
				if (tmp != null)
				{
					var currentFontProp = tmpType.GetProperty("font");
					Font emojiFont = CreateEmojiSystemFont();
					if (currentFontProp != null && emojiFont != null)
					{
						// Create TMP font asset from OS font
						var createMethod = tmpFontAssetType.GetMethod("CreateFontAsset", new Type[] { typeof(Font) });
						if (createMethod != null)
						{
							var emojiTmpFont = createMethod.Invoke(null, new object[] { emojiFont });
							if (emojiTmpFont != null)
							{
								currentFontProp.SetValue(tmp, emojiTmpFont, null);
							}
						}
					}
				}
			}
		}

		private Font CreateEmojiSystemFont()
		{
			try
			{
				// Try common emoji-capable fonts on Windows and cross-platform fallbacks
				string[] candidates = new string[] {
					"Segoe UI Emoji", // Windows
					"Segoe UI Symbol",
					"Noto Color Emoji", // Android/Linux if present
					"Apple Color Emoji" // macOS/iOS
				};
				Font font = Font.CreateDynamicFontFromOSFont(candidates, 18);
				return font;
			}
			catch
			{
				return null;
			}
		}

		private void EnsureSurgeryCardIcons()
		{
			TryEnsureIconForCard(suturingButton, "suturing");
			TryEnsureIconForCard(injectionButton, "injection");
			TryEnsureIconForCard(laparoscopicButton, "laparoscopic");
			TryEnsureIconForCard(tibialNailingButton, "tibial");
			TryEnsureIconForCard(pacemakerButton, "pacemaker");
		}

		private void TryEnsureIconForCard(Button cardButton, string iconKey)
		{
			if (cardButton == null) return;
			Transform iconTransform = FindDeepChild(cardButton.transform, "Icon");
			if (iconTransform == null) return;
			Image iconImage = iconTransform.GetComponent<Image>();
			if (iconImage == null) return;
			if (iconImage.sprite != null) return; // already set
			
			// Use the card's outline color as accent if available
			Color accent = new Color(0.2f, 0.8f, 0.5f, 1f);
			Outline outline = cardButton.GetComponent<Outline>();
			if (outline != null) accent = outline.effectColor;
			
			iconImage.sprite = CreateIconSprite(iconKey, accent);
			iconImage.preserveAspect = true;
		}

		private Transform FindDeepChild(Transform parent, string name)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (child.name == name) return child;
				Transform result = FindDeepChild(child, name);
				if (result != null) return result;
			}
			return null;
		}

		private Sprite CreateIconSprite(string iconType, Color accentColor)
		{
			int size = 256;
			Texture2D tex = new Texture2D(size, size);
			Color[] pixels = new Color[size * size];
			for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
			Vector2 center = new Vector2(size / 2f, size / 2f);
			switch (iconType)
			{
				case "suturing":
					DrawSuturingIcon(pixels, size, center, accentColor);
					break;
				case "injection":
					DrawInjectionIcon(pixels, size, center, accentColor);
					break;
				case "laparoscopic":
					DrawLaparoscopicIcon(pixels, size, center, accentColor);
					break;
				case "tibial":
					DrawTibialIcon(pixels, size, center, accentColor);
					break;
				case "pacemaker":
					DrawPacemakerIcon(pixels, size, center, accentColor);
					break;
			}
			tex.SetPixels(pixels);
			tex.Apply();
			return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
		}

		private void DrawSuturingIcon(Color[] pixels, int size, Vector2 center, Color color)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					float x = i - center.x;
					float y = j - center.y;
					if (Mathf.Abs(x - y) < 8 && x > -60 && x < 60 && y > -60 && y < 60) pixels[j * size + i] = color;
					if (Mathf.Abs(x + y) < 8 && x > -60 && x < 60 && y > -60 && y < 60) pixels[j * size + i] = color;
					float dist1 = Vector2.Distance(new Vector2(i, j), center + new Vector2(-40, -40));
					float dist2 = Vector2.Distance(new Vector2(i, j), center + new Vector2(40, -40));
					if ((dist1 < 15 && dist1 > 10) || (dist2 < 15 && dist2 > 10)) pixels[j * size + i] = color;
				}
			}
		}

		private void DrawInjectionIcon(Color[] pixels, int size, Vector2 center, Color color)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					float x = i - center.x;
					float y = j - center.y;
					if (x > 30 && x < 80 && Mathf.Abs(y - (x - 55)) < 3) pixels[j * size + i] = color;
					if (x > -50 && x < 40 && y > -15 && y < 15) pixels[j * size + i] = color;
					if (x > -45 && x < -20 && Mathf.Abs(y) < 8) pixels[j * size + i] = new Color(color.r * 0.6f, color.g * 0.6f, color.b * 0.6f, color.a);
					if (x > -60 && x < -45 && Mathf.Abs(y) < 20) pixels[j * size + i] = color;
				}
			}
		}

		private void DrawLaparoscopicIcon(Color[] pixels, int size, Vector2 center, Color color)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					float dist = Vector2.Distance(new Vector2(i, j), center);
					if (dist < 70 && dist > 60) pixels[j * size + i] = color;
					if (dist < 50 && dist > 40) pixels[j * size + i] = color;
					if (dist < 15) pixels[j * size + i] = color;
					float x = i - center.x;
					float y = j - center.y;
					if ((Mathf.Abs(x) < 4 || Mathf.Abs(y) < 4) && dist < 65) pixels[j * size + i] = color;
				}
			}
		}

		private void DrawTibialIcon(Color[] pixels, int size, Vector2 center, Color color)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					float x = i - center.x;
					float y = j - center.y;
					bool topBulge = y < -30 && y > -70 && Mathf.Abs(x) < 35;
					bool bottomBulge = y > 30 && y < 70 && Mathf.Abs(x) < 35;
					bool shaft = y > -30 && y < 30 && Mathf.Abs(x) < 20;
					if (topBulge || bottomBulge || shaft)
					{
						if (Mathf.Abs(x) > 12 || y < -35 || y > 35) pixels[j * size + i] = new Color(color.r, color.g, color.b, color.a * 0.4f);
					}
					if (Mathf.Abs(x) < 6 && y > -60 && y < 60) pixels[j * size + i] = color;
					if (Mathf.Abs(y + 40) < 4 && x > -25 && x < 25) pixels[j * size + i] = color;
					if (Mathf.Abs(y - 40) < 4 && x > -25 && x < 25) pixels[j * size + i] = color;
				}
			}
		}

		private void DrawPacemakerIcon(Color[] pixels, int size, Vector2 center, Color color)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					float x = i - center.x;
					float y = j - center.y;
					float leftCircleX = -30;
					float rightCircleX = 30;
					float circleY = 25;
					float circleRadius = 35;
					float leftDist = Mathf.Sqrt(Mathf.Pow(x - leftCircleX, 2) + Mathf.Pow(y - circleY, 2));
					float rightDist = Mathf.Sqrt(Mathf.Pow(x - rightCircleX, 2) + Mathf.Pow(y - circleY, 2));
					bool inLeftCircle = leftDist < circleRadius;
					bool inRightCircle = rightDist < circleRadius;
					float triangleTop = 5;
					float triangleBottom = -55;
					float triangleWidth = 65;
					bool inTriangle = false;
					if (y < triangleTop && y > triangleBottom)
					{
						float widthAtY = triangleWidth * (y - triangleBottom) / (triangleTop - triangleBottom);
						inTriangle = Mathf.Abs(x) < widthAtY;
					}
					if ((inLeftCircle || inRightCircle || inTriangle) && y > triangleBottom) pixels[j * size + i] = color;
					float baselineY = -10;
					if (Mathf.Abs(y - baselineY) < 4 && x > -70 && x < -30) pixels[j * size + i] = Color.white;
					if (x > -30 && x < -20)
					{
						float spikeY = baselineY - (x + 30) * 4;
						if (Mathf.Abs(y - spikeY) < 4) pixels[j * size + i] = Color.white;
					}
					if (Mathf.Abs(y - (baselineY - 40)) < 4 && x > -20 && x < -15) pixels[j * size + i] = Color.white;
					if (x > -15 && x < -5)
					{
						float spikeY = baselineY - 40 + (x + 15) * 6;
						if (Mathf.Abs(y - spikeY) < 4) pixels[j * size + i] = Color.white;
					}
					if (x > -5 && x < 5)
					{
						float dipY = baselineY + 20 - Mathf.Abs(x) * 2;
						if (Mathf.Abs(y - dipY) < 4) pixels[j * size + i] = Color.white;
					}
					if (x > 5 && x < 15)
					{
						float returnY = baselineY + 10 - (x - 5);
						if (Mathf.Abs(y - returnY) < 4) pixels[j * size + i] = Color.white;
					}
					if (Mathf.Abs(y - baselineY) < 4 && x > 15 && x < 70) pixels[j * size + i] = Color.white;
				}
			}
		}

		private string GetButtonCurrentLabelUpper(Button btn)
		{
			if (btn == null) return string.Empty;
			string text = string.Empty;
			var legacy = btn.GetComponentInChildren<Text>(true);
			if (legacy != null && !string.IsNullOrEmpty(legacy.text)) text = legacy.text;
			else
			{
				Type tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
				if (tmpType != null)
				{
					var tmp = btn.GetComponentInChildren(tmpType, true);
					if (tmp != null)
					{
						var textProp = tmpType.GetProperty("text");
						if (textProp != null)
						{
							object val = textProp.GetValue(tmp, null);
							if (val != null) text = val.ToString();
						}
					}
				}
			}
			return string.IsNullOrEmpty(text) ? string.Empty : text.ToUpperInvariant();
		}
        
        private void ConfigureCanvasForVR()
        {
            // Find the Canvas component
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
                canvas = FindObjectOfType<Canvas>();
            
            if (canvas == null)
            {
                Debug.LogWarning("SelectionSceneManager: No Canvas found!");
                return;
            }
            
            // Set to World Space if not already
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.Log("SelectionSceneManager: Converting canvas to World Space for VR");
                canvas.renderMode = RenderMode.WorldSpace;
            }
            
            // Find and assign HMD camera
            if (canvas.worldCamera == null)
            {
                Camera hmdCam = FindHMDCamera();
                if (hmdCam != null)
                {
                    canvas.worldCamera = hmdCam;
                    Debug.Log("SelectionSceneManager: Assigned HMD camera to canvas");
                }
            }
            
            // Ensure GraphicRaycaster exists (needed for ISDK/OVR raycasting)
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log("SelectionSceneManager: Added GraphicRaycaster");
            }
            
            // For Oculus/Meta, ensure OVRRaycaster if available
            EnsureOVRRaycaster(canvas);
            
            // Position canvas in front of HMD
            PositionCanvasForVR(canvas);
        }
        
        private Camera FindHMDCamera()
        {
            // Try to find camera in VR_Character hierarchy
            GameObject vrChar = GameObject.Find("VR_Character");
            if (vrChar != null)
            {
                Camera cam = vrChar.GetComponentInChildren<Camera>();
                if (cam != null) return cam;
            }
            
            // Fallback to Main Camera or first enabled camera
            if (Camera.main != null) return Camera.main;
            return FindObjectOfType<Camera>();
        }
        
        private void EnsureOVRRaycaster(Canvas canvas)
        {
            // Check if OVRRaycaster component type exists
            Type ovrRayType = Type.GetType("OVRRaycaster, Assembly-CSharp-firstpass") ??
                              Type.GetType("OVRRaycaster, Oculus.VR") ??
                              Type.GetType("OVRRaycaster");
            
            if (ovrRayType != null)
            {
                // Check if already has OVRRaycaster
                Component existing = canvas.GetComponent(ovrRayType);
                if (existing == null)
                {
                    canvas.gameObject.AddComponent(ovrRayType);
                    Debug.Log("SelectionSceneManager: Added OVRRaycaster");
                }
            }
        }
        
        private void PositionCanvasForVR(Canvas canvas)
        {
            if (canvas.worldCamera == null) return;
            
            Camera cam = canvas.worldCamera;
            Vector3 forward = cam.transform.forward;
            Vector3 position = cam.transform.position + forward * 1.8f;
            
            canvas.transform.position = position;
            canvas.transform.rotation = Quaternion.LookRotation(-forward, Vector3.up);
            
            // Only set scale if it hasn't been customized (scale of 1.0 usually means not set for VR)
            if (canvas.transform.localScale == Vector3.one)
            {
                canvas.transform.localScale = Vector3.one * 0.001f; // Typical VR UI scale
            }
        }
        
        [System.Serializable]
        private class UserData
        {
            public string id;
            public string email;
            public string first_name;
            public string last_name;
            public string role;
        }
    }
}