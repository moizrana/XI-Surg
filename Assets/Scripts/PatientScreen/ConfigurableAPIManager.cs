// ConfigurableAPIManager.cs - Handles API communication for any monitor
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ConfigurableAPIManager : MonoBehaviour
{
    [Header("API Configuration")]
    public string monitorId = "monitor_1"; // Change this for different monitors
    public float updateInterval = 5f; // Update every 5 seconds
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent<VitalSignsData> OnVitalSignsUpdated;
    
    private string baseUrl = "https://smarthospitalbackend.onrender.com/iotData/";
    private VitalSignsData currentVitalSigns;
    
    private void Start()
    {
        StartCoroutine(FetchVitalSignsRoutine());
    }
    
    private IEnumerator FetchVitalSignsRoutine()
    {
        while (true)
        {
            yield return FetchVitalSigns();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private IEnumerator FetchVitalSigns()
    {
        string fullUrl = baseUrl + monitorId + "/vitals/latest";
        
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            request.SetRequestHeader("accept", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    VitalSignsResponse response = JsonUtility.FromJson<VitalSignsResponse>(jsonResponse);
                    
                    currentVitalSigns = response.data;
                    OnVitalSignsUpdated?.Invoke(currentVitalSigns);
                    
                    Debug.Log($"Vital signs updated for {monitorId}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing API response for {monitorId}: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"API request failed for {monitorId}: {request.error}");
            }
        }
    }
    
    public VitalSignsData GetCurrentVitalSigns()
    {
        return currentVitalSigns;
    }
    
    // Method to change monitor ID at runtime
    public void SetMonitorId(string newMonitorId)
    {
        monitorId = newMonitorId;
        Debug.Log($"Monitor ID changed to: {monitorId}");
    }
}