using UnityEngine;
using UnityEngine.UI;

namespace Meducator.Selection
{
    /// <summary>
    /// Automatically creates the Selection Scene UI
    /// </summary>
    public class SelectionSceneUISetup : MonoBehaviour
    {
        [Header("Setup Configuration")]
        public bool autoSetupOnStart = true;
        
        [Header("Medical Theme Colors")]
        public Color medicalBlue = new Color(0.2f, 0.5f, 0.9f, 1f);
        public Color medicalGreen = new Color(0.2f, 0.8f, 0.5f, 1f);
        public Color medicalPurple = new Color(0.6f, 0.3f, 0.9f, 1f);
        public Color medicalOrange = new Color(1f, 0.5f, 0.2f, 1f);
        public Color darkBackground = new Color(0.05f, 0.08f, 0.15f, 1f);
        public Color cardBackground = new Color(0.12f, 0.15f, 0.22f, 0.95f);
        public Color buttonHover = new Color(0.18f, 0.22f, 0.32f, 1f);
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                Invoke("CreateSelectionUI", 0.5f);
            }
        }
        
        [ContextMenu("Create Selection Scene UI")]
        public void CreateSelectionUI()
        {
            Debug.Log("🩺 Meducator: Creating Selection Scene UI...");
            
            if (FindObjectOfType<SelectionSceneManager>() != null)
            {
                Debug.Log("Selection Scene UI already exists!");
                return;
            }
            
            CreateCompleteUI();
            Debug.Log("✅ Selection Scene UI ready!");
        }
        
        private void CreateCompleteUI()
        {
            GameObject canvas = CreateCanvas();
            GameObject background = CreateBackground(canvas);
            CreateMeducatorLogo(canvas);
            GameObject userInfoPanel = CreateUserInfoPanel(canvas);
            GameObject modePanel = CreateModeSelectionPanel(canvas);
            GameObject surgeryPanel = CreateSurgerySelectionPanel(canvas);
            AddManagerAndConnect(canvas, modePanel, surgeryPanel, userInfoPanel);
        }
        
        private GameObject CreateCanvas()
{
    GameObject canvasGO = new GameObject("Selection Scene Canvas");
    Canvas canvas = canvasGO.AddComponent<Canvas>();
    
    // ✅ CRITICAL: Set to WorldSpace for VR
    canvas.renderMode = RenderMode.WorldSpace;
    canvas.sortingOrder = 100;
    
    // Configure RectTransform
    RectTransform rt = canvasGO.GetComponent<RectTransform>();
    rt.sizeDelta = new Vector2(1920f, 1080f);
    
    // Don't set position/scale here - you'll do it manually
    
    CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
    scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
    scaler.physicalUnit = CanvasScaler.Unit.Millimeters;
    scaler.referencePixelsPerUnit = 100;
    
    // ✅ Add GraphicRaycaster for standard UI interaction
    canvasGO.AddComponent<GraphicRaycaster>();
    
    // ✅ Add CanvasGroup for fading
    canvasGO.AddComponent<CanvasGroup>();
    
    Debug.Log("✅ Canvas created in World Space mode");
    
    return canvasGO;
}
        
        private GameObject CreateBackground(GameObject canvas)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = bg.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            Image image = bg.AddComponent<Image>();
            image.color = darkBackground;
            
            CreateDecorativeCircle(bg, new Vector2(-700, 400), 180f, new Color(0.2f, 0.5f, 0.9f, 0.1f));
            CreateDecorativeCircle(bg, new Vector2(750, -350), 200f, new Color(0.2f, 0.8f, 0.5f, 0.08f));
            CreateDecorativeCircle(bg, new Vector2(-600, -400), 150f, new Color(0.6f, 0.3f, 0.9f, 0.1f));
            
            return bg;
        }
        
        private void CreateMeducatorLogo(GameObject canvas)
        {
            GameObject logoPanel = new GameObject("Meducator Logo Panel");
            logoPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = logoPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(30, -30);
            rect.sizeDelta = new Vector2(350, 80);
            
            Image image = logoPanel.AddComponent<Image>();
            image.color = cardBackground;
            
            Shadow shadow = logoPanel.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(0, -4);
            
            // Logo Text
            GameObject logoTextGO = new GameObject("Logo Text");
            logoTextGO.transform.SetParent(logoPanel.transform, false);
            
            RectTransform logoRect = logoTextGO.AddComponent<RectTransform>();
            logoRect.anchorMin = Vector2.zero;
            logoRect.anchorMax = Vector2.one;
            logoRect.sizeDelta = Vector2.zero;
            
            Text logoText = logoTextGO.AddComponent<Text>();
            logoText.text = "🩺 MEDUCATOR";
            logoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            logoText.fontSize = 32;
            logoText.color = Color.white;
            logoText.fontStyle = FontStyle.Bold;
            logoText.alignment = TextAnchor.MiddleCenter;
            
            Outline outline = logoTextGO.AddComponent<Outline>();
            outline.effectColor = medicalBlue;
            outline.effectDistance = new Vector2(2, -2);
        }
        
        private GameObject CreateUserInfoPanel(GameObject canvas)
        {
            GameObject panel = new GameObject("User Info Panel");
            panel.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-30, -30);
            rect.sizeDelta = new Vector2(350, 80);
            
            Image image = panel.AddComponent<Image>();
            image.color = cardBackground;
            
            Shadow shadow = panel.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(0, -4);
            
            HorizontalLayoutGroup layout = panel.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 10, 10);
            layout.spacing = 15f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            GameObject userNameGO = new GameObject("User Name Text");
            userNameGO.transform.SetParent(panel.transform, false);
            
            RectTransform userNameRect = userNameGO.AddComponent<RectTransform>();
            userNameRect.sizeDelta = new Vector2(200, 60);
            
            Text userNameText = userNameGO.AddComponent<Text>();
            userNameText.text = "Welcome, User";
            userNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            userNameText.fontSize = 18;
            userNameText.color = Color.white;
            userNameText.fontStyle = FontStyle.Bold;
            userNameText.alignment = TextAnchor.MiddleLeft;
            
            GameObject logoutBtn = CreateSmallButton(panel, "Logout", new Color(0.8f, 0.3f, 0.3f, 1f));
            
            return panel;
        }
        
        private GameObject CreateModeSelectionPanel(GameObject canvas)
        {
            GameObject panel = new GameObject("Mode Selection Panel");
            panel.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(800, 600);
            
            Image image = panel.AddComponent<Image>();
            image.color = cardBackground;
            
            Shadow shadow = panel.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.6f);
            shadow.effectDistance = new Vector2(0, -8);
            
            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(60, 60, 60, 60);
            layout.spacing = 40f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            panel.AddComponent<CanvasGroup>();
            
            CreateTitle(panel, "Select Training Mode", 42);
            CreateSubtitle(panel, "Choose how you want to learn");
            CreateModeButton(panel, "TRAINING MODE", "Practice freely without guidance", medicalGreen);
            CreateModeButton(panel, "GUIDED MODE", "Step-by-step instructions and assistance", medicalBlue);
            
            return panel;
        }
        
        private GameObject CreateSurgerySelectionPanel(GameObject canvas)
        {
            GameObject panel = new GameObject("Surgery Selection Panel");
            panel.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(1400, 800);
            
            Image image = panel.AddComponent<Image>();
            image.color = cardBackground;
            
            Shadow shadow = panel.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.6f);
            shadow.effectDistance = new Vector2(0, -8);
            
            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(50, 50, 50, 50);
            layout.spacing = 30f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            panel.AddComponent<CanvasGroup>();
            
            CreateTitle(panel, "Select Surgery", 42);
            CreateCategorySelection(panel);
            
            GameObject cardsContainer = new GameObject("Surgery Cards Container");
            cardsContainer.transform.SetParent(panel.transform, false);
            RectTransform cardsRect = cardsContainer.AddComponent<RectTransform>();
            cardsRect.sizeDelta = new Vector2(1300, 550);
            
            CreateBasicSurgeriesPanel(cardsContainer);
            CreateAdvancedSurgeriesPanel(cardsContainer);
            
            return panel;
        }
        
        private GameObject CreateCategorySelection(GameObject parent)
        {
            GameObject categoryPanel = new GameObject("Category Selection");
            categoryPanel.transform.SetParent(parent.transform, false);
            
            RectTransform rect = categoryPanel.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 80);
            
            HorizontalLayoutGroup layout = categoryPanel.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 30f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            CreateCategoryButton(categoryPanel, "📚 BASIC SURGERIES", medicalGreen);
            CreateCategoryButton(categoryPanel, "🔬 ADVANCED SURGERIES", medicalPurple);
            
            return categoryPanel;
        }
        
        private GameObject CreateBasicSurgeriesPanel(GameObject parent)
        {
            GameObject panel = new GameObject("Basic Surgeries Panel");
            panel.transform.SetParent(parent.transform, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            HorizontalLayoutGroup layout = panel.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 40f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            CreateSurgeryCard(panel, "Suturing", "Basic wound closure technique", "suturing", medicalGreen);
            CreateSurgeryCard(panel, "Injection", "Intramuscular and IV techniques", "injection", medicalBlue);
            
            return panel;
        }
        
        private GameObject CreateAdvancedSurgeriesPanel(GameObject parent)
        {
            GameObject panel = new GameObject("Advanced Surgeries Panel");
            panel.transform.SetParent(parent.transform, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            GridLayoutGroup layout = panel.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(380, 450);
            layout.spacing = new Vector2(40, 40);
            layout.childAlignment = TextAnchor.UpperCenter;
            
            CreateSurgeryCard(panel, "Laparoscopic Appendectomy", "Minimally invasive appendix removal", "laparoscopic", medicalPurple);
            CreateSurgeryCard(panel, "Intramedullary Tibial Nailing", "Tibial fracture fixation procedure", "tibial", medicalOrange);
            CreateSurgeryCard(panel, "Pacemaker Implantation", "Cardiac rhythm device insertion", "pacemaker", new Color(0.9f, 0.2f, 0.4f, 1f));
            
            return panel;
        }
        
        private void CreateSurgeryCard(GameObject parent, string surgeryName, string description, string icon, Color accentColor)
        {
            GameObject card = new GameObject(surgeryName + " Button");
            card.transform.SetParent(parent.transform, false);
            
            RectTransform rect = card.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(380, 450);
            
            Image cardImage = card.AddComponent<Image>();
            cardImage.color = new Color(0.15f, 0.18f, 0.25f, 1f);
            
            Button button = card.AddComponent<Button>();
            button.targetGraphic = cardImage;
            
            Outline outline = card.AddComponent<Outline>();
            outline.effectColor = accentColor;
            outline.effectDistance = new Vector2(0, -3);
            
            Shadow shadow = card.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(0, -6);
            
            VerticalLayoutGroup layout = card.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(30, 30, 30, 30);
            layout.spacing = 20f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            // Icon container with background circle
            GameObject iconContainer = new GameObject("Icon Container");
            iconContainer.transform.SetParent(card.transform, false);
            RectTransform iconContainerRect = iconContainer.AddComponent<RectTransform>();
            iconContainerRect.sizeDelta = new Vector2(140, 140);
            
            Image iconBg = iconContainer.AddComponent<Image>();
            iconBg.color = new Color(accentColor.r * 0.3f, accentColor.g * 0.3f, accentColor.b * 0.3f, 0.5f);
            
            // Create circular sprite for icon background
            Texture2D circleTex = new Texture2D(128, 128);
            Color[] circlePixels = new Color[128 * 128];
            Vector2 center = new Vector2(64, 64);
            
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= 62f)
                        circlePixels[y * 128 + x] = Color.white;
                    else
                        circlePixels[y * 128 + x] = Color.clear;
                }
            }
            
            circleTex.SetPixels(circlePixels);
            circleTex.Apply();
            Sprite circleSprite = Sprite.Create(circleTex, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
            iconBg.sprite = circleSprite;
            
            // Icon Image instead of Text for better emoji support
            GameObject iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(iconContainer.transform, false);
            RectTransform iconRect = iconGO.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.sizeDelta = new Vector2(-20, -20); // Slight padding
            
            // Create icon graphic
            Image iconImage = iconGO.AddComponent<Image>();
            iconImage.sprite = CreateIconSprite(icon, accentColor);
            iconImage.preserveAspect = true;
            
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(card.transform, false);
            RectTransform titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(320, 80);
            
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = surgeryName.ToUpper();
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 20;
            titleText.color = Color.white;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.UpperCenter;
            
            GameObject descGO = new GameObject("Description");
            descGO.transform.SetParent(card.transform, false);
            RectTransform descRect = descGO.AddComponent<RectTransform>();
            descRect.sizeDelta = new Vector2(320, 100);
            
            Text descText = descGO.AddComponent<Text>();
            descText.text = description;
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 16;
            descText.color = new Color(0.7f, 0.75f, 0.85f, 1f);
            descText.alignment = TextAnchor.UpperCenter;
            descText.horizontalOverflow = HorizontalWrapMode.Wrap;
            descText.verticalOverflow = VerticalWrapMode.Overflow;
            
            GameObject accentBar = new GameObject("Accent Bar");
            accentBar.transform.SetParent(card.transform, false);
            RectTransform barRect = accentBar.AddComponent<RectTransform>();
            barRect.sizeDelta = new Vector2(320, 8);
            
            Image barImage = accentBar.AddComponent<Image>();
            barImage.color = accentColor;
        }
        
        private GameObject CreateModeButton(GameObject parent, string title, string description, Color color)
        {
            GameObject button = new GameObject(title + " Button");
            button.transform.SetParent(parent.transform, false);
            
            RectTransform rect = button.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(680, 140);
            
            Image image = button.AddComponent<Image>();
            image.color = color;
            
            Button btn = button.AddComponent<Button>();
            btn.targetGraphic = image;
            
            Shadow shadow = button.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(0, -5);
            
            VerticalLayoutGroup layout = button.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(30, 30, 20, 20);
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(button.transform, false);
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = title;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 28;
            titleText.color = Color.white;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            GameObject descGO = new GameObject("Description");
            descGO.transform.SetParent(button.transform, false);
            Text descText = descGO.AddComponent<Text>();
            descText.text = description;
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 16;
            descText.color = new Color(0.9f, 0.95f, 1f, 0.9f);
            descText.alignment = TextAnchor.MiddleCenter;
            descText.fontStyle = FontStyle.Italic;
            
            return button;
        }
        
        private GameObject CreateCategoryButton(GameObject parent, string text, Color color)
        {
            GameObject button = new GameObject(text + " Button");
            button.transform.SetParent(parent.transform, false);
            
            RectTransform rect = button.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(320, 70);
            
            Image image = button.AddComponent<Image>();
            image.color = color;
            
            Button btn = button.AddComponent<Button>();
            btn.targetGraphic = image;
            
            Shadow shadow = button.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.4f);
            shadow.effectDistance = new Vector2(0, -4);
            
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(button.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = 18;
            textComponent.color = Color.white;
            textComponent.fontStyle = FontStyle.Bold;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            return button;
        }
        
        private GameObject CreateSmallButton(GameObject parent, string text, Color color)
        {
            GameObject button = new GameObject(text + " Button");
            button.transform.SetParent(parent.transform, false);
            
            RectTransform rect = button.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 60);
            
            Image image = button.AddComponent<Image>();
            image.color = color;
            
            Button btn = button.AddComponent<Button>();
            btn.targetGraphic = image;
            
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(button.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.fontSize = 16;
            textComponent.color = Color.white;
            textComponent.fontStyle = FontStyle.Bold;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            return button;
        }
        
        private void CreateTitle(GameObject parent, string text, int fontSize)
        {
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(parent.transform, false);
            
            RectTransform rect = titleGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 70);
            
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = text.ToUpper();
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = fontSize;
            titleText.color = Color.white;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            Outline outline = titleGO.AddComponent<Outline>();
            outline.effectColor = medicalBlue;
            outline.effectDistance = new Vector2(2, -2);
        }
        
        private void CreateSubtitle(GameObject parent, string text)
        {
            GameObject subtitleGO = new GameObject("Subtitle");
            subtitleGO.transform.SetParent(parent.transform, false);
            
            RectTransform rect = subtitleGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 40);
            
            Text subtitleText = subtitleGO.AddComponent<Text>();
            subtitleText.text = text;
            subtitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            subtitleText.fontSize = 18;
            subtitleText.color = new Color(0.7f, 0.8f, 0.95f, 0.9f);
            subtitleText.alignment = TextAnchor.MiddleCenter;
            subtitleText.fontStyle = FontStyle.Italic;
        }
        
        private Sprite CreateIconSprite(string iconType, Color accentColor)
        {
            int size = 256;
            Texture2D tex = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];
            
            // Initialize transparent
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.clear;
            
            Vector2 center = new Vector2(size / 2f, size / 2f);
            
            // Draw different icons based on type
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
            // Draw scissors (two crossed lines forming X)
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = i - center.x;
                    float y = j - center.y;
                    
                    // Scissor blade 1 (diagonal line)
                    if (Mathf.Abs(x - y) < 8 && x > -60 && x < 60 && y > -60 && y < 60)
                        pixels[j * size + i] = color;
                    
                    // Scissor blade 2 (other diagonal)
                    if (Mathf.Abs(x + y) < 8 && x > -60 && x < 60 && y > -60 && y < 60)
                        pixels[j * size + i] = color;
                    
                    // Handle circles
                    float dist1 = Vector2.Distance(new Vector2(i, j), center + new Vector2(-40, -40));
                    float dist2 = Vector2.Distance(new Vector2(i, j), center + new Vector2(40, -40));
                    
                    if ((dist1 < 15 && dist1 > 10) || (dist2 < 15 && dist2 > 10))
                        pixels[j * size + i] = color;
                }
            }
        }
        
        private void DrawInjectionIcon(Color[] pixels, int size, Vector2 center, Color color)
        {
            // Draw syringe shape
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = i - center.x;
                    float y = j - center.y;
                    
                    // Needle (thin line at angle)
                    if (x > 30 && x < 80 && Mathf.Abs(y - (x - 55)) < 3)
                        pixels[j * size + i] = color;
                    
                    // Barrel (rectangle)
                    if (x > -50 && x < 40 && y > -15 && y < 15)
                        pixels[j * size + i] = color;
                    
                    // Plunger (line inside barrel)
                    if (x > -45 && x < -20 && Mathf.Abs(y) < 8)
                        pixels[j * size + i] = new Color(color.r * 0.6f, color.g * 0.6f, color.b * 0.6f, color.a);
                    
                    // Plunger handle
                    if (x > -60 && x < -45 && Mathf.Abs(y) < 20)
                        pixels[j * size + i] = color;
                }
            }
        }
        
        private void DrawLaparoscopicIcon(Color[] pixels, int size, Vector2 center, Color color)
        {
            // Draw laparoscope (circle with lines - representing scope view)
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float dist = Vector2.Distance(new Vector2(i, j), center);
                    
                    // Outer circle
                    if (dist < 70 && dist > 60)
                        pixels[j * size + i] = color;
                    
                    // Inner circle
                    if (dist < 50 && dist > 40)
                        pixels[j * size + i] = color;
                    
                    // Center dot
                    if (dist < 15)
                        pixels[j * size + i] = color;
                    
                    // Crosshair lines
                    float x = i - center.x;
                    float y = j - center.y;
                    if ((Mathf.Abs(x) < 4 || Mathf.Abs(y) < 4) && dist < 65)
                        pixels[j * size + i] = color;
                }
            }
        }
        
        private void DrawTibialIcon(Color[] pixels, int size, Vector2 center, Color color)
        {
            // Draw bone shape with nail
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = i - center.x;
                    float y = j - center.y;
                    
                    // Bone outline (two bulges at ends)
                    bool topBulge = y < -30 && y > -70 && Mathf.Abs(x) < 35;
                    bool bottomBulge = y > 30 && y < 70 && Mathf.Abs(x) < 35;
                    bool shaft = y > -30 && y < 30 && Mathf.Abs(x) < 20;
                    
                    if (topBulge || bottomBulge || shaft)
                    {
                        // Create hollow bone effect
                        if (Mathf.Abs(x) > 12 || y < -35 || y > 35)
                            pixels[j * size + i] = new Color(color.r, color.g, color.b, color.a * 0.4f);
                    }
                    
                    // Nail through center (darker line)
                    if (Mathf.Abs(x) < 6 && y > -60 && y < 60)
                        pixels[j * size + i] = color;
                    
                    // Screws (small horizontal lines)
                    if (Mathf.Abs(y + 40) < 4 && x > -25 && x < 25)
                        pixels[j * size + i] = color;
                    if (Mathf.Abs(y - 40) < 4 && x > -25 && x < 25)
                        pixels[j * size + i] = color;
                }
            }
        }
        
        private void DrawPacemakerIcon(Color[] pixels, int size, Vector2 center, Color color)
        {
            // Draw a proper heart shape with pulse line (pointed bottom)
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float x = i - center.x;
                    float y = j - center.y;
                    
                    // Create heart shape - flipped (pointed at bottom)
                    // Left and right bumps of the heart at the TOP
                    float leftCircleX = -30;
                    float rightCircleX = 30;
                    float circleY = 25;  // Moved down (positive Y is down)
                    float circleRadius = 35;
                    
                    float leftDist = Mathf.Sqrt(Mathf.Pow(x - leftCircleX, 2) + Mathf.Pow(y - circleY, 2));
                    float rightDist = Mathf.Sqrt(Mathf.Pow(x - rightCircleX, 2) + Mathf.Pow(y - circleY, 2));
                    
                    bool inLeftCircle = leftDist < circleRadius;
                    bool inRightCircle = rightDist < circleRadius;
                    
                    // Bottom point of heart (triangle pointing DOWN)
                    float triangleTop = 5;  // Near the circles
                    float triangleBottom = -55;  // Point at bottom (negative Y)
                    float triangleWidth = 65;
                    
                    bool inTriangle = false;
                    if (y < triangleTop && y > triangleBottom)
                    {
                        float widthAtY = triangleWidth * (y - triangleBottom) / (triangleTop - triangleBottom);
                        inTriangle = Mathf.Abs(x) < widthAtY;
                    }
                    
                    // Fill the heart
                    if ((inLeftCircle || inRightCircle || inTriangle) && y > triangleBottom)
                    {
                        pixels[j * size + i] = color;
                    }
                    
                    // Add ECG/Pulse line across the heart
                    float baselineY = -10;
                    
                    // Flat line before spike
                    if (Mathf.Abs(y - baselineY) < 4 && x > -70 && x < -30)
                        pixels[j * size + i] = new Color(1f, 1f, 1f, 1f);
                    
                    // Upward spike
                    if (x > -30 && x < -20)
                    {
                        float spikeY = baselineY - (x + 30) * 4;
                        if (Mathf.Abs(y - spikeY) < 4)
                            pixels[j * size + i] = new Color(1f, 1f, 1f, 1f);
                    }
                    
                    // Peak
                    if (Mathf.Abs(y - (baselineY - 40)) < 4 && x > -20 && x < -15)
                        pixels[j * size + i] = new Color(1f, 1f, 1f, 1f);
                    
                    // Downward spike
                    if (x > -15 && x < -5)
                    {
                        float spikeY = baselineY - 40 + (x + 15) * 6;
                        if (Mathf.Abs(y - spikeY) < 4)
                            pixels[j * size + i] = new Color(1f, 1f, 1f, 1f);
                    }
                    
                    // Small dip
                    if (x > -5 && x < 5)
                    {
                        float dipY = baselineY + 20 - Mathf.Abs(x) * 2;
                        if (Mathf.Abs(y - dipY) < 4)
                            pixels[j * size + i] = new Color(1f, 1f, 1f, 1f);
                    }
                    
                    // Return to baseline
                    if (x > 5 && x < 15)
                    {
                        float returnY = baselineY + 10 - (x - 5);
                        if (Mathf.Abs(y - returnY) < 4)
                            pixels[j * size + i] = new Color(1f, 1f, 1f, 1f);
                    }
                    
                    // Flat line after spike
                    if (Mathf.Abs(y - baselineY) < 4 && x > 15 && x < 70)
                        pixels[j * size + i] = new Color(1f, 1f, 1f, 1f);
                }
            }
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
        
        private void AddManagerAndConnect(GameObject canvas, GameObject modePanel, GameObject surgeryPanel, GameObject userInfoPanel)
        {
            SelectionSceneManager manager = canvas.AddComponent<SelectionSceneManager>();
            
            manager.modeSelectionPanel = modePanel;
            manager.surgerySelectionPanel = surgeryPanel;
            manager.userInfoPanel = userInfoPanel;
            
            GameObject trainingBtn = GameObject.Find("TRAINING MODE Button");
            GameObject guidedBtn = GameObject.Find("GUIDED MODE Button");
            
            if (trainingBtn != null)
                manager.trainingModeButton = trainingBtn.GetComponent<Button>();
            if (guidedBtn != null)
                manager.guidedModeButton = guidedBtn.GetComponent<Button>();
            
            GameObject basicCatBtn = GameObject.Find("📚 BASIC SURGERIES Button");
            GameObject advancedCatBtn = GameObject.Find("🔬 ADVANCED SURGERIES Button");
            
            if (basicCatBtn != null)
                manager.basicCategoryButton = basicCatBtn.GetComponent<Button>();
            if (advancedCatBtn != null)
                manager.advancedCategoryButton = advancedCatBtn.GetComponent<Button>();
            
            GameObject basicPanel = GameObject.Find("Basic Surgeries Panel");
            GameObject advancedPanel = GameObject.Find("Advanced Surgeries Panel");
            
            if (basicPanel != null)
                manager.basicSurgeriesPanel = basicPanel;
            if (advancedPanel != null)
                manager.advancedSurgeriesPanel = advancedPanel;
            
            GameObject suturingBtn = GameObject.Find("Suturing Button");
            GameObject injectionBtn = GameObject.Find("Injection Button");
            GameObject laparoscopicBtn = GameObject.Find("Laparoscopic Appendectomy Button");
            GameObject tibialBtn = GameObject.Find("Intramedullary Tibial Nailing Button");
            GameObject pacemakerBtn = GameObject.Find("Pacemaker Implantation Button");
            
            if (suturingBtn != null)
                manager.suturingButton = suturingBtn.GetComponent<Button>();
            if (injectionBtn != null)
                manager.injectionButton = injectionBtn.GetComponent<Button>();
            if (laparoscopicBtn != null)
                manager.laparoscopicButton = laparoscopicBtn.GetComponent<Button>();
            if (tibialBtn != null)
                manager.tibialNailingButton = tibialBtn.GetComponent<Button>();
            if (pacemakerBtn != null)
                manager.pacemakerButton = pacemakerBtn.GetComponent<Button>();
            
            GameObject userNameText = GameObject.Find("User Name Text");
            GameObject logoutBtn = GameObject.Find("Logout Button");
            
            if (userNameText != null)
                manager.userNameText = userNameText.GetComponent<Text>();
            if (logoutBtn != null)
                manager.logoutButton = logoutBtn.GetComponent<Button>();
            
            Debug.Log("✅ Connected all UI elements to SelectionSceneManager");
        }
        
        [ContextMenu("Clear Selection Scene UI")]
        public void ClearSelectionUI()
        {
            GameObject canvas = GameObject.Find("Selection Scene Canvas");
            if (canvas != null)
            {
                DestroyImmediate(canvas);
                Debug.Log("Cleared Selection Scene UI");
            }
        }
    }
}