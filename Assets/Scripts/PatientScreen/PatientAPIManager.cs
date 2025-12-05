// PatientAPIManager.cs - Handles patient information API calls
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class PatientAPIManager : MonoBehaviour
{
    [Header("Patient API Configuration")]
    public string patientId = "patient_1"; // Change this for different patients
    public float updateInterval = 10f; // Update every 10 seconds (less frequent than vitals)
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent<PatientInfoResponse> OnPatientInfoUpdated;
    
    private string baseUrl = "https://smarthospitalbackend.onrender.com/patients/";
    private PatientInfoResponse currentPatientInfo;
    
    private void Start()
    {
        StartCoroutine(FetchPatientInfoRoutine());
    }
    
    private IEnumerator FetchPatientInfoRoutine()
    {
        while (true)
        {
            yield return FetchPatientInfo();
            yield return new WaitForSeconds(updateInterval);
        }
    }
    
    private IEnumerator FetchPatientInfo()
    {
        string fullUrl = baseUrl + patientId;
        
        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            request.SetRequestHeader("accept", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    PatientInfoResponse response = JsonUtility.FromJson<PatientInfoResponse>(jsonResponse);
                    
                    currentPatientInfo = response;
                    OnPatientInfoUpdated?.Invoke(currentPatientInfo);
                    
                    Debug.Log($"Patient info updated for {patientId}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing patient API response for {patientId}: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Patient API request failed for {patientId}: {request.error}");
            }
        }
    }
    
    public PatientInfoResponse GetCurrentPatientInfo()
    {
        return currentPatientInfo;
    }
    
    public void SetPatientId(string newPatientId)
    {
        patientId = newPatientId;
        Debug.Log($"Patient ID changed to: {patientId}");
    }
}