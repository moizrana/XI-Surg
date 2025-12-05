using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the overall stitching procedure for the medical operation
/// Tracks progress, handles completion, and coordinates between stitch sites
/// </summary>
public class StitchManager : MonoBehaviour
{
    [Header("Stitch Sites")]
    public List<StitchSite> stitchSites = new List<StitchSite>();
    
    [Header("Procedure Settings")]
    public bool requireSequentialStitching = false; // If true, stitches must be done in order
    public float procedureTimeLimit = 300f; // 5 minutes default
    public bool enableTimer = false;
    
    [Header("Completion Settings")]
    public GameObject procedureCompleteUI;
    public AudioClip procedureCompleteSound;
    public ParticleSystem completionEffect;
    
    [Header("Progress Tracking")]
    public UnityEngine.UI.Slider progressSlider;
    public UnityEngine.UI.Text progressText;
    public UnityEngine.UI.Text timerText;
    
    // Private variables
    private int completedStitches = 0;
    private float startTime;
    private bool procedureCompleted = false;
    private AudioSource audioSource;
    private float lastStitchTime = -999f; // Time when last stitch was completed
    private bool isStitchingInProgress = false; // Prevents multiple simultaneous stitches
    private HashSet<StitchSite> completedSites = new HashSet<StitchSite>(); // Track which sites have been completed
    
    // Events
    public System.Action OnProcedureStarted;
    public System.Action OnProcedureCompleted;
    public System.Action<StitchSite> OnStitchCompleted; // Event for individual stitch completions
    public System.Action<float> OnProgressUpdated; // Progress as percentage (0-1)
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    private void Start()
    {
        InitializeProcedure();
    }
    
    private void Update()
    {
        if (enableTimer && !procedureCompleted)
        {
            UpdateTimer();
        }
    }
    
    /// <summary>
    /// Initialize the stitching procedure
    /// </summary>
    private void InitializeProcedure()
    {
        // Auto-find stitch sites if not manually assigned
        if (stitchSites.Count == 0)
        {
            stitchSites = FindObjectsOfType<StitchSite>().ToList();
            
            // Sort by name for consistent ordering
            stitchSites = stitchSites.OrderBy(s => s.name).ToList();
        }
        
        // Subscribe to stitch completion events
        foreach (var stitchSite in stitchSites)
        {
            stitchSite.OnStitchCompleted += OnStitchSiteCompleted;
        }
        
        // Initialize UI
        UpdateProgressUI();
        
        // Record start time
        startTime = Time.time;
        
        // Trigger start event
        OnProcedureStarted?.Invoke();
        
        Debug.Log($"Stitching procedure initialized with {stitchSites.Count} stitch sites");
    }
    
    /// <summary>
    /// Check if stitching is currently allowed (cooldown check)
    /// </summary>
    public bool IsStitchingAllowed()
    {
        if (procedureCompleted || isStitchingInProgress)
            return false;
            
        // Check if enough time has passed since last stitch
        float timeSinceLastStitch = Time.time - lastStitchTime;
        float requiredCooldown = stitchSites.Count > 0 ? stitchSites[0].stitchCooldownTime : 2.0f;
        
        return timeSinceLastStitch >= requiredCooldown;
    }
    
    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    public float GetRemainingCooldown()
    {
        if (procedureCompleted)
            return 0f;
            
        float timeSinceLastStitch = Time.time - lastStitchTime;
        float requiredCooldown = stitchSites.Count > 0 ? stitchSites[0].stitchCooldownTime : 2.0f;
        
        return Mathf.Max(0f, requiredCooldown - timeSinceLastStitch);
    }
    
    /// <summary>
    /// Mark that a stitch is starting (prevents simultaneous stitches)
    /// </summary>
    public void SetStitchingInProgress(bool inProgress)
    {
        isStitchingInProgress = inProgress;
    }
    
    /// <summary>
    /// Called when a stitch site is completed
    /// </summary>
    public void OnStitchSiteCompleted(StitchSite completedSite)
    {
        if (procedureCompleted)
            return;
            
        // Check if this site has already been completed
        if (completedSites.Contains(completedSite))
        {
            Debug.LogWarning($"StitchSite {completedSite.name} was already completed! Ignoring duplicate completion.");
            return;
        }
        
        // Add to completed sites list
        completedSites.Add(completedSite);
        
        completedStitches++;
        lastStitchTime = Time.time; // Record when this stitch was completed
        isStitchingInProgress = false; // Clear the in-progress flag
        
        Debug.Log($"Stitch completed: {completedSite.name} ({completedStitches}/{stitchSites.Count})");
        
        // Trigger stitch completion event for other systems (like video manager)
        OnStitchCompleted?.Invoke(completedSite);
        
        // Update progress
        float progress = (float)completedStitches / stitchSites.Count;
        OnProgressUpdated?.Invoke(progress);
        UpdateProgressUI();
        
        // Check if procedure is complete
        if (completedStitches >= stitchSites.Count)
        {
            CompleteProcedure();
        }
        else if (requireSequentialStitching)
        {
            // Enable next stitch site if sequential stitching is required
            EnableNextStitchSite();
        }
    }
    
    /// <summary>
    /// Enable the next stitch site in sequence
    /// </summary>
    private void EnableNextStitchSite()
    {
        if (completedStitches < stitchSites.Count)
        {
            var nextSite = stitchSites[completedStitches];
            nextSite.gameObject.SetActive(true);
            Debug.Log($"Next stitch site enabled: {nextSite.name}");
        }
    }
    
    /// <summary>
    /// Complete the entire procedure
    /// </summary>
    private void CompleteProcedure()
    {
        procedureCompleted = true;
        float completionTime = Time.time - startTime;
        
        Debug.Log($"Procedure completed in {completionTime:F1} seconds!");
        
        // Show completion UI
        if (procedureCompleteUI != null)
        {
            procedureCompleteUI.SetActive(true);
        }
        
        // Play completion sound
        if (procedureCompleteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(procedureCompleteSound);
        }
        
        // Trigger completion effect
        if (completionEffect != null)
        {
            completionEffect.Play();
        }
        
        // Trigger completion event
        OnProcedureCompleted?.Invoke();
    }
    
    /// <summary>
    /// Update the progress UI elements
    /// </summary>
    private void UpdateProgressUI()
    {
        float progress = (float)completedStitches / stitchSites.Count;
        
        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }
        
        if (progressText != null)
        {
            progressText.text = $"Stitches: {completedStitches}/{stitchSites.Count}";
        }
    }
    
    /// <summary>
    /// Update the timer UI
    /// </summary>
    private void UpdateTimer()
    {
        float elapsedTime = Time.time - startTime;
        float remainingTime = Mathf.Max(0, procedureTimeLimit - elapsedTime);
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
        
        // Check if time limit exceeded
        if (remainingTime <= 0 && !procedureCompleted)
        {
            OnTimeUp();
        }
    }
    
    /// <summary>
    /// Called when time limit is reached
    /// </summary>
    private void OnTimeUp()
    {
        Debug.Log("Time limit reached!");
        // Handle time up scenario (restart, show message, etc.)
    }
    
    /// <summary>
    /// Reset the entire procedure
    /// </summary>
    public void ResetProcedure()
    {
        procedureCompleted = false;
        completedStitches = 0;
        
        // Reset all stitch sites
        foreach (var stitchSite in stitchSites)
        {
            stitchSite.ResetStitch();
        }
        
        // Hide completion UI
        if (procedureCompleteUI != null)
        {
            procedureCompleteUI.SetActive(false);
        }
        
        // Reset timer
        startTime = Time.time;
        lastStitchTime = -999f; // Reset cooldown timer
        isStitchingInProgress = false; // Clear in-progress flag
        completedSites.Clear(); // Clear completed sites list
        
        // Trigger procedure started event (for video manager reset)
        OnProcedureStarted?.Invoke();
        
        // Update UI
        UpdateProgressUI();
        
        Debug.Log("Procedure reset");
    }
    
    /// <summary>
    /// Get current progress as percentage
    /// </summary>
    public float GetProgress()
    {
        return (float)completedStitches / stitchSites.Count;
    }
    
    /// <summary>
    /// Get number of completed stitches
    /// </summary>
    public int GetCompletedStitches()
    {
        return completedStitches;
    }
    
    /// <summary>
    /// Get total number of stitch sites
    /// </summary>
    public int GetTotalStitches()
    {
        return stitchSites.Count;
    }
    
    /// <summary>
    /// Check if procedure is completed
    /// </summary>
    public bool IsProcedureCompleted()
    {
        return procedureCompleted;
    }
}
