using UnityEngine;
using UnityEngine.UI;

public class LocalECGWaveGenerator : MonoBehaviour
{
    [Header("API Manager Reference")]
    public ConfigurableAPIManager apiManager; // Drag the API manager for this monitor here
    
    [Header("ECG Display Settings")]
    public RawImage ecgDisplay;
    public int textureWidth = 512;
    public int textureHeight = 256;
    public Color waveColor = Color.green;
    public Color backgroundColor = Color.black;
    public int lineThickness = 3;
    
    [Header("Wave Parameters")]
    public float waveSpeed = 2f;
    public float amplitude = 0.3f;
    public float baselineY = 0.5f;
    
    [Header("Heart Rate Sync")]
    public bool syncWithHeartRate = true;
    public float defaultBPM = 75f;
    
    private Texture2D ecgTexture;
    private float[] waveData;
    private int currentPosition = 0;
    private float currentBPM;
    
    // ECG wave pattern (same as before)
    private readonly float[] ecgPattern = new float[]
    {
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0.1f, 0.2f, 0.15f, 0.05f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        -0.1f, -0.15f,
        1.0f, 0.8f, 0.4f,
        -0.3f, -0.2f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0.15f, 0.25f, 0.2f, 0.1f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f
    };
    
    private int patternIndex = 0;
    private float timeSinceLastBeat = 0f;
    
    private void Start()
    {
        InitializeECGDisplay();
        currentBPM = defaultBPM;
        
        // Connect to the specific API manager
        if (apiManager != null)
        {
            apiManager.OnVitalSignsUpdated.AddListener(OnVitalSignsUpdated);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up the listener
        if (apiManager != null)
        {
            apiManager.OnVitalSignsUpdated.RemoveListener(OnVitalSignsUpdated);
        }
    }
    
    private void OnVitalSignsUpdated(VitalSignsData vitalSigns)
    {
        if (syncWithHeartRate && vitalSigns != null)
        {
            currentBPM = vitalSigns.heartRate;
        }
    }
    
    private void InitializeECGDisplay()
    {
        ecgTexture = new Texture2D(textureWidth, textureHeight);
        waveData = new float[textureWidth];
        
        for (int i = 0; i < textureWidth; i++)
        {
            waveData[i] = baselineY;
        }
        
        if (ecgDisplay != null)
        {
            ecgDisplay.texture = ecgTexture;
        }
        
        ClearTexture();
    }
    
    private void Update()
    {
        GenerateECGWave();
        UpdateTexture();
    }
    
    private void GenerateECGWave()
    {
        float beatInterval = 60f / currentBPM;
        timeSinceLastBeat += Time.deltaTime;
        
        currentPosition = (currentPosition + 1) % textureWidth;
        
        float waveValue = baselineY;
        
        if (timeSinceLastBeat >= beatInterval)
        {
            timeSinceLastBeat = 0f;
            patternIndex = 0;
        }
        
        if (patternIndex < ecgPattern.Length)
        {
            float patternValue = ecgPattern[patternIndex] * amplitude;
            waveValue = baselineY + patternValue;
            patternIndex++;
            waveValue += Random.Range(-0.01f, 0.01f);
        }
        else
        {
            waveValue = baselineY + Random.Range(-0.005f, 0.005f);
        }
        
        waveValue = Mathf.Clamp(waveValue, 0.1f, 0.9f);
        waveData[currentPosition] = waveValue;
    }
    
    private void UpdateTexture()
    {
        ClearTexture();
        
        for (int x = 0; x < textureWidth; x++)
        {
            int dataIndex = (currentPosition + x) % textureWidth;
            float waveHeight = waveData[dataIndex];
            int pixelY = Mathf.RoundToInt(waveHeight * (textureHeight - 1));
            
            int halfThickness = lineThickness / 2;
            for (int thickness = -halfThickness; thickness <= halfThickness; thickness++)
            {
                int y = pixelY + thickness;
                if (y >= 0 && y < textureHeight)
                {
                    ecgTexture.SetPixel(x, y, waveColor);
                }
            }
            
            if (lineThickness % 2 == 0)
            {
                int y = pixelY + halfThickness + 1;
                if (y >= 0 && y < textureHeight)
                {
                    ecgTexture.SetPixel(x, y, waveColor);
                }
            }
        }
        
        // Grid lines
        for (int x = 0; x < textureWidth; x += 25)
        {
            for (int y = 0; y < textureHeight; y += 2)
            {
                ecgTexture.SetPixel(x, y, Color.gray * 0.2f);
            }
        }
        
        for (int y = 0; y < textureHeight; y += 25)
        {
            for (int x = 0; x < textureWidth; x += 2)
            {
                ecgTexture.SetPixel(x, y, Color.gray * 0.2f);
            }
        }
        
        ecgTexture.Apply();
    }
    
    private void ClearTexture()
    {
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        ecgTexture.SetPixels(pixels);
    }
    
    public void SetBPM(float bpm)
    {
        currentBPM = Mathf.Clamp(bpm, 30f, 200f);
    }
}