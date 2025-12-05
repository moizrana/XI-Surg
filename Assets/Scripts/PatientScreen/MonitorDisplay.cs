using UnityEngine;
using TMPro;
using System;

public class LocalMonitorDisplay : MonoBehaviour
{
    [Header("API Manager Reference")]
    public ConfigurableAPIManager apiManager; // Drag the API manager for this monitor here
    
    [Header("UI Text References")]
    public TMP_Text hrText;
    public TMP_Text oxygenText;
    public TMP_Text tempText;
    public TMP_Text bpText;
    public TMP_Text timeText;
    
    [Header("Display Settings")]
    public string hrPrefix = "HR: ";
    public string hrSuffix = " bpm";
    public string oxygenPrefix = "Oxygen: ";
    public string oxygenSuffix = "%";
    public string tempPrefix = "Temp: ";
    public string tempSuffix = "°C";
    public string bpPrefix = "BP: ";
    public string bpSuffix = " mmHg";
    
    [Header("Time Display Settings")]
    public bool showCurrentTime = true;
    public string timeFormat = "HH:mm:ss";
    public string dateFormat = "MM/dd/yy";
    public bool showDate = false;
    
    private void Start()
    {
        // Connect to the specific API manager
        if (apiManager != null)
        {
            apiManager.OnVitalSignsUpdated.AddListener(UpdateDisplay);
        }
        else
        {
            Debug.LogError("API Manager not assigned to LocalMonitorDisplay!");
        }
        
        // Initialize time display
        if (timeText != null)
        {
            UpdateTimeDisplay();
        }
    }
    
    private void Update()
    {
        // Update time every frame
        if (showCurrentTime && timeText != null)
        {
            UpdateTimeDisplay();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up the listener
        if (apiManager != null)
        {
            apiManager.OnVitalSignsUpdated.RemoveListener(UpdateDisplay);
        }
    }
    
    private void UpdateDisplay(VitalSignsData vitalSigns)
    {
        if (vitalSigns == null) return;
        
        // Update Heart Rate
        if (hrText != null)
            hrText.text = hrPrefix + vitalSigns.heartRate.ToString() + hrSuffix;
        
        // Update Oxygen Level
        if (oxygenText != null)
            oxygenText.text = oxygenPrefix + vitalSigns.oxygenLevel.ToString() + oxygenSuffix;
        
        // Update Temperature
        if (tempText != null)
            tempText.text = tempPrefix + vitalSigns.temperature.ToString("F1") + tempSuffix;
        
        // Update Blood Pressure
        if (bpText != null)
            bpText.text = bpPrefix + vitalSigns.bloodPressure.systolic.ToString() + "/" + 
                         vitalSigns.bloodPressure.diastolic.ToString() + bpSuffix;
    }
    
    private void UpdateTimeDisplay()
    {
        if (timeText == null) return;
        
        DateTime currentTime = DateTime.Now;
        string displayText = "";
        
        if (showDate)
        {
            displayText = currentTime.ToString(dateFormat) + "\n";
        }
        
        displayText += currentTime.ToString(timeFormat);
        
        timeText.text = displayText;
    }
}