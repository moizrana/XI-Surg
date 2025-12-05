using UnityEngine;

/// <summary>
/// Enhanced thread renderer with advanced visual effects
/// Provides realistic thread appearance with physics simulation
/// </summary>
public class ThreadRenderer : MonoBehaviour
{
    [Header("Thread Appearance")]
    public Material threadMaterial;
    public float threadWidth = 0.002f;
    public Color threadColor = Color.white;
    public int threadResolution = 20; // Number of points along the thread
    
    [Header("Physics Simulation")]
    public bool enablePhysics = true;
    public float gravity = -9.81f;
    public float damping = 0.95f;
    public float tension = 50f; // Thread tension
    
    [Header("Animation")]
    public float creationSpeed = 2f;
    public AnimationCurve creationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool animateCreation = true;
    
    // Private variables
    private LineRenderer lineRenderer;
    private Vector3[] threadPoints;
    private Vector3[] threadVelocities;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private bool isAnimating = false;
    private float animationProgress = 0f;
    
    private void Awake()
    {
        SetupLineRenderer();
        InitializeThreadPoints();
    }
    
    private void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        lineRenderer.material = threadMaterial;
        lineRenderer.startWidth = threadWidth;
        lineRenderer.endWidth = threadWidth;
        lineRenderer.positionCount = threadResolution;
        lineRenderer.useWorldSpace = true;
        
        // Set thread color using gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(threadColor, 0.0f), new GradientColorKey(threadColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(threadColor.a, 0.0f), new GradientAlphaKey(threadColor.a, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        
        // Set render settings for better appearance
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.textureMode = LineTextureMode.Tile;
    }
    
    private void InitializeThreadPoints()
    {
        threadPoints = new Vector3[threadResolution];
        threadVelocities = new Vector3[threadResolution];
        
        // Initialize velocities to zero
        for (int i = 0; i < threadResolution; i++)
        {
            threadVelocities[i] = Vector3.zero;
        }
    }
    
    /// <summary>
    /// Create a thread between two points
    /// </summary>
    public void CreateThread(Vector3 start, Vector3 end)
    {
        startPoint = start;
        endPoint = end;
        
        if (animateCreation)
        {
            StartCoroutine(AnimateThreadCreation());
        }
        else
        {
            SetupImmediateThread();
        }
    }
    
    private void SetupImmediateThread()
    {
        // Set up thread points in a straight line initially
        for (int i = 0; i < threadResolution; i++)
        {
            float t = (float)i / (threadResolution - 1);
            threadPoints[i] = Vector3.Lerp(startPoint, endPoint, t);
        }
        
        UpdateLineRenderer();
    }
    
    private System.Collections.IEnumerator AnimateThreadCreation()
    {
        isAnimating = true;
        animationProgress = 0f;
        
        while (animationProgress < 1f)
        {
            animationProgress += Time.deltaTime * creationSpeed;
            animationProgress = Mathf.Clamp01(animationProgress);
            
            float curveValue = creationCurve.Evaluate(animationProgress);
            
            // Update thread points based on animation progress
            for (int i = 0; i < threadResolution; i++)
            {
                float pointProgress = (float)i / (threadResolution - 1);
                
                if (pointProgress <= curveValue)
                {
                    threadPoints[i] = Vector3.Lerp(startPoint, endPoint, pointProgress);
                }
                else
                {
                    // Points beyond the animation progress stay at start
                    threadPoints[i] = startPoint;
                }
            }
            
            UpdateLineRenderer();
            yield return null;
        }
        
        // Ensure final state is correct
        SetupImmediateThread();
        isAnimating = false;
    }
    
    private void Update()
    {
        if (enablePhysics && !isAnimating)
        {
            SimulatePhysics();
            UpdateLineRenderer();
        }
    }
    
    private void SimulatePhysics()
    {
        float deltaTime = Time.deltaTime;
        
        // Apply physics to internal points (not endpoints)
        for (int i = 1; i < threadResolution - 1; i++)
        {
            Vector3 currentPoint = threadPoints[i];
            
            // Calculate tension forces from neighboring points
            Vector3 leftTension = (threadPoints[i - 1] - currentPoint) * tension;
            Vector3 rightTension = (threadPoints[i + 1] - currentPoint) * tension;
            Vector3 totalTension = leftTension + rightTension;
            
            // Apply gravity
            Vector3 gravityForce = Vector3.up * gravity;
            
            // Update velocity
            threadVelocities[i] += (totalTension + gravityForce) * deltaTime;
            threadVelocities[i] *= damping; // Apply damping
            
            // Update position
            threadPoints[i] += threadVelocities[i] * deltaTime;
        }
        
        // Keep endpoints fixed
        threadPoints[0] = startPoint;
        threadPoints[threadResolution - 1] = endPoint;
    }
    
    private void UpdateLineRenderer()
    {
        lineRenderer.SetPositions(threadPoints);
    }
    
    /// <summary>
    /// Update thread endpoints (useful for dynamic stitching)
    /// </summary>
    public void UpdateEndpoints(Vector3 newStart, Vector3 newEnd)
    {
        startPoint = newStart;
        endPoint = newEnd;
        
        // Update endpoint positions
        if (threadPoints != null && threadPoints.Length > 0)
        {
            threadPoints[0] = startPoint;
            threadPoints[threadResolution - 1] = endPoint;
        }
    }
    
    /// <summary>
    /// Set thread color
    /// </summary>
    public void SetThreadColor(Color color)
    {
        threadColor = color;
        if (lineRenderer != null)
        {
            // Set color using gradient
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(color.a, 0.0f), new GradientAlphaKey(color.a, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
        }
    }
    
    /// <summary>
    /// Set thread width
    /// </summary>
    public void SetThreadWidth(float width)
    {
        threadWidth = width;
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }
    
    /// <summary>
    /// Hide/show the thread
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
        }
    }
    
    /// <summary>
    /// Get the current length of the thread
    /// </summary>
    public float GetThreadLength()
    {
        float length = 0f;
        
        for (int i = 0; i < threadResolution - 1; i++)
        {
            length += Vector3.Distance(threadPoints[i], threadPoints[i + 1]);
        }
        
        return length;
    }
}
