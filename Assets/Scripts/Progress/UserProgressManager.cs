using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Meducator.Progress
{
    /// <summary>
    /// Manages user progress and session data
    /// </summary>
    public class UserProgressManager : MonoBehaviour
    {
        [Header("Progress Settings")]
        public bool autoSaveProgress = true;
        public float saveInterval = 30f; // Save every 30 seconds
        
        [Header("API Configuration")]
        // Note: Server functionality disabled to avoid 404 errors
		// public string apiBaseUrl = "https://meducator.onrender.com";
        
        private UserProgressData currentProgress;
        private Coroutine autoSaveCoroutine;
        
        public static UserProgressManager Instance { get; private set; }
        
        public static event Action<UserProgressData> OnProgressUpdated;
        public static event Action<string> OnProgressSaved;
        public static event Action<string> OnProgressLoadError;
        
        [Serializable]
        public class UserProgressData
        {
            public string userId;
            public string sessionId;
            public string selectedMode;
            public string selectedCategory;
            public string selectedSurgery;
            public DateTime sessionStartTime;
            public DateTime lastActivityTime;
            public List<SurgeryAttempt> surgeryAttempts;
            public Dictionary<string, object> customData;
            
            public UserProgressData()
            {
                surgeryAttempts = new List<SurgeryAttempt>();
                customData = new Dictionary<string, object>();
                sessionStartTime = DateTime.Now;
                lastActivityTime = DateTime.Now;
            }
        }
        
        [Serializable]
        public class SurgeryAttempt
        {
            public string surgeryType;
            public string mode;
            public DateTime startTime;
            public DateTime? endTime;
            public float completionPercentage;
            public List<string> completedSteps;
            public List<string> failedSteps;
            public Dictionary<string, object> metrics;
            
            public SurgeryAttempt()
            {
                completedSteps = new List<string>();
                failedSteps = new List<string>();
                metrics = new Dictionary<string, object>();
                startTime = DateTime.Now;
            }
        }
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeProgressManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            LoadUserProgress();
            
            if (autoSaveProgress)
            {
                StartAutoSave();
            }
        }
        
        private void InitializeProgressManager()
        {
            currentProgress = new UserProgressData();
            
            // Get user ID from PlayerPrefs
            string userId = PlayerPrefs.GetString("UserId", "");
            if (!string.IsNullOrEmpty(userId))
            {
                currentProgress.userId = userId;
            }
            
            // Generate session ID
            currentProgress.sessionId = Guid.NewGuid().ToString();
            
            Debug.Log($"🩺 UserProgressManager initialized for user: {currentProgress.userId}");
        }
        
        public void StartSurgerySession(string surgeryType, string mode, string category)
        {
            if (currentProgress == null)
            {
                InitializeProgressManager();
            }
            
            // Update current session data
            currentProgress.selectedSurgery = surgeryType;
            currentProgress.selectedMode = mode;
            currentProgress.selectedCategory = category;
            currentProgress.lastActivityTime = DateTime.Now;
            
            // Create new surgery attempt
            SurgeryAttempt attempt = new SurgeryAttempt
            {
                surgeryType = surgeryType,
                mode = mode,
                startTime = DateTime.Now
            };
            
            currentProgress.surgeryAttempts.Add(attempt);
            
            // Save to PlayerPrefs for immediate access
            SaveProgressToPlayerPrefs();
            
            Debug.Log($"🩺 Started surgery session: {surgeryType} in {mode} mode");
            
            OnProgressUpdated?.Invoke(currentProgress);
        }
        
        public void UpdateSurgeryProgress(string stepName, bool completed, Dictionary<string, object> metrics = null)
        {
            if (currentProgress == null || currentProgress.surgeryAttempts.Count == 0)
                return;
            
            SurgeryAttempt currentAttempt = currentProgress.surgeryAttempts[currentProgress.surgeryAttempts.Count - 1];
            
            if (completed)
            {
                if (!currentAttempt.completedSteps.Contains(stepName))
                {
                    currentAttempt.completedSteps.Add(stepName);
                }
                currentAttempt.failedSteps.Remove(stepName);
            }
            else
            {
                if (!currentAttempt.failedSteps.Contains(stepName))
                {
                    currentAttempt.failedSteps.Add(stepName);
                }
                currentAttempt.completedSteps.Remove(stepName);
            }
            
            // Update metrics
            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    currentAttempt.metrics[metric.Key] = metric.Value;
                }
            }
            
            // Calculate completion percentage
            int totalSteps = currentAttempt.completedSteps.Count + currentAttempt.failedSteps.Count;
            if (totalSteps > 0)
            {
                currentAttempt.completionPercentage = (float)currentAttempt.completedSteps.Count / totalSteps * 100f;
            }
            
            currentProgress.lastActivityTime = DateTime.Now;
            
            Debug.Log($"🩺 Updated surgery progress: {stepName} - {(completed ? "Completed" : "Failed")}");
            
            OnProgressUpdated?.Invoke(currentProgress);
        }
        
        public void CompleteSurgerySession()
        {
            if (currentProgress == null || currentProgress.surgeryAttempts.Count == 0)
                return;
            
            SurgeryAttempt currentAttempt = currentProgress.surgeryAttempts[currentProgress.surgeryAttempts.Count - 1];
            currentAttempt.endTime = DateTime.Now;
            
            Debug.Log($"🩺 Completed surgery session: {currentAttempt.surgeryType} - {currentAttempt.completionPercentage:F1}% completion");
            
            // Save progress
            SaveProgress();
        }
        
public void SaveProgress()
		{
			if (currentProgress == null) return;
			
			// Save to PlayerPrefs for immediate access
			SaveProgressToPlayerPrefs();
			
			// Note: Server save functionality removed to avoid 404 errors
			// Progress is now saved locally only
			
			OnProgressSaved?.Invoke("Progress saved locally");
			
			Debug.Log("🩺 Progress saved locally (server save disabled)");
		}
        
        private void SaveProgressToPlayerPrefs()
        {
            if (currentProgress == null) return;
            
            try
            {
                string progressJson = JsonUtility.ToJson(currentProgress, true);
                PlayerPrefs.SetString("UserProgress", progressJson);
                PlayerPrefs.SetString("SessionId", currentProgress.sessionId);
                PlayerPrefs.SetString("SelectedMode", currentProgress.selectedMode ?? "");
                PlayerPrefs.SetString("SelectedCategory", currentProgress.selectedCategory ?? "");
                PlayerPrefs.SetString("SelectedSurgery", currentProgress.selectedSurgery ?? "");
                PlayerPrefs.SetString("SessionStartTime", currentProgress.sessionStartTime.ToString());
                PlayerPrefs.SetString("LastActivityTime", currentProgress.lastActivityTime.ToString());
                PlayerPrefs.Save();
                
                Debug.Log("🩺 Progress saved to PlayerPrefs");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save progress to PlayerPrefs: {e.Message}");
            }
        }
        

        
        public void LoadUserProgress()
        {
            try
            {
                string progressJson = PlayerPrefs.GetString("UserProgress", "");
                if (!string.IsNullOrEmpty(progressJson))
                {
                    currentProgress = JsonUtility.FromJson<UserProgressData>(progressJson);
                    Debug.Log($"🩺 Loaded user progress for session: {currentProgress.sessionId}");
                }
                else
                {
                    InitializeProgressManager();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load user progress: {e.Message}");
                OnProgressLoadError?.Invoke($"Load failed: {e.Message}");
                InitializeProgressManager();
            }
        }
        
        private void StartAutoSave()
        {
            if (autoSaveCoroutine != null)
            {
                StopCoroutine(autoSaveCoroutine);
            }
            
            autoSaveCoroutine = StartCoroutine(AutoSaveCoroutine());
        }
        
        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(saveInterval);
                
                if (currentProgress != null)
                {
                    SaveProgress();
                }
            }
        }
        
        public UserProgressData GetCurrentProgress()
        {
            return currentProgress;
        }
        
        public SurgeryAttempt GetCurrentSurgeryAttempt()
        {
            if (currentProgress == null || currentProgress.surgeryAttempts.Count == 0)
                return null;
            
            return currentProgress.surgeryAttempts[currentProgress.surgeryAttempts.Count - 1];
        }
        
        public void SetCustomData(string key, object value)
        {
            if (currentProgress == null)
            {
                InitializeProgressManager();
            }
            
            currentProgress.customData[key] = value;
            currentProgress.lastActivityTime = DateTime.Now;
        }
        
        public T GetCustomData<T>(string key, T defaultValue = default(T))
        {
            if (currentProgress == null || !currentProgress.customData.ContainsKey(key))
                return defaultValue;
            
            try
            {
                return (T)currentProgress.customData[key];
            }
            catch
            {
                return defaultValue;
            }
        }
        
        public void ClearProgress()
        {
            currentProgress = new UserProgressData();
            PlayerPrefs.DeleteKey("UserProgress");
            PlayerPrefs.DeleteKey("SessionId");
            PlayerPrefs.DeleteKey("SelectedMode");
            PlayerPrefs.DeleteKey("SelectedCategory");
            PlayerPrefs.DeleteKey("SelectedSurgery");
            PlayerPrefs.DeleteKey("SessionStartTime");
            PlayerPrefs.DeleteKey("LastActivityTime");
            PlayerPrefs.Save();
            
            Debug.Log("🩺 User progress cleared");
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentProgress != null)
            {
                SaveProgress();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && currentProgress != null)
            {
                SaveProgress();
            }
        }
        
        private void OnDestroy()
        {
            if (currentProgress != null)
            {
                SaveProgress();
            }
            
            if (autoSaveCoroutine != null)
            {
                StopCoroutine(autoSaveCoroutine);
            }
        }
    }
}