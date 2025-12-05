using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class APIManager : MonoBehaviour
{
    [Header("API Configuration")]
    public string apiUrl = "https://smarthospitalbackend.onrender.com/iotData/monitor_1/vitals/latest";
    public float updateInterval = 5f; // Update every 5 seconds
    
    // Events
    public static event Action<VitalSignsData> OnVitalSignsUpdated;
    
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
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
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
                    
                    Debug.Log("Vital signs updated successfully");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing API response: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"API request failed: {request.error}");
            }
        }
    }
    
    public VitalSignsData GetCurrentVitalSigns()
    {
        return currentVitalSigns;
    }
}