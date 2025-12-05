// PatientInfoDisplay.cs - Displays patient information on the tablet monitor
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class PatientInfoDisplay : MonoBehaviour
{
    [Header("Patient API Manager Reference")]
    public PatientAPIManager patientAPIManager;
    
    [Header("Patient Info UI References")]
    public TMP_Text patientNameText;
    public TMP_Text patientDetailsText; // Age, Gender, Room, Ward
    public TMP_Text diagnosisText;
    public TMP_Text statusText;
    public TMP_Text riskLevelText;
    public TMP_Text allergiesText;
    public TMP_Text medicationsText;
    public TMP_Text recommendationsText;
    
    [Header("Display Colors")]
    public Color lowRiskColor = Color.green;
    public Color mediumRiskColor = Color.yellow;
    public Color highRiskColor = Color.red;
    
    private void Start()
    {
        if (patientAPIManager != null)
        {
            patientAPIManager.OnPatientInfoUpdated.AddListener(UpdatePatientDisplay);
        }
        else
        {
            Debug.LogError("Patient API Manager not assigned!");
        }
    }
    
    private void OnDestroy()
    {
        if (patientAPIManager != null)
        {
            patientAPIManager.OnPatientInfoUpdated.RemoveListener(UpdatePatientDisplay);
        }
    }
    
    private void UpdatePatientDisplay(PatientInfoResponse patientInfo)
    {
        if (patientInfo == null) return;
        
        // Update Patient Name
        if (patientNameText != null && patientInfo.personalInfo != null)
        {
            patientNameText.text = patientInfo.personalInfo.name;
        }
        
        // Update Patient Details
        if (patientDetailsText != null && patientInfo.personalInfo != null)
        {
            patientDetailsText.text = $"Age: {patientInfo.personalInfo.age} | {patientInfo.personalInfo.gender}\n" +
                                    $"Room: {patientInfo.personalInfo.roomId} | Ward: {patientInfo.personalInfo.ward}\n" +
                                    $"Bed: {patientInfo.personalInfo.bedId}";
        }
        
        // Update Diagnosis
        if (diagnosisText != null && patientInfo.currentStatus != null)
        {
            diagnosisText.text = $"Diagnosis: {patientInfo.currentStatus.diagnosis}";
        }
        
        // Update Status
        if (statusText != null && patientInfo.currentStatus != null)
        {
            statusText.text = $"Status: {patientInfo.currentStatus.status.ToUpper()}\n" +
                            $"Consciousness: {patientInfo.currentStatus.consciousness}\n" +
                            $"Mobility: {patientInfo.currentStatus.mobility}";
        }
        
        // Update Risk Level with color coding
        if (riskLevelText != null && patientInfo.predictions != null)
        {
            riskLevelText.text = $"Risk Level: {patientInfo.predictions.riskLevel}\n" +
                               $"Risk Score: {patientInfo.predictions.riskScore}%";
            
            // Color code based on risk level
            switch (patientInfo.predictions.riskLevel.ToLower())
            {
                case "low":
                    riskLevelText.color = lowRiskColor;
                    break;
                case "medium":
                    riskLevelText.color = mediumRiskColor;
                    break;
                case "high":
                    riskLevelText.color = highRiskColor;
                    break;
                default:
                    riskLevelText.color = Color.white;
                    break;
            }
        }
        
        // Update Allergies
        if (allergiesText != null && patientInfo.medicalHistory != null && patientInfo.medicalHistory.allergies != null)
        {
            string allergiesStr = "Allergies: ";
            if (patientInfo.medicalHistory.allergies.Count > 0)
            {
                allergiesStr += string.Join(", ", patientInfo.medicalHistory.allergies);
            }
            else
            {
                allergiesStr += "None";
            }
            allergiesText.text = allergiesStr;
        }
        
        // Update Medications
        if (medicationsText != null && patientInfo.medicalHistory != null && patientInfo.medicalHistory.medications != null)
        {
            string medicationsStr = "Current Medications:\n";
            foreach (var med in patientInfo.medicalHistory.medications.Take(3)) // Show first 3 medications
            {
                medicationsStr += $"• {med.name} {med.dosage} - {med.frequency}\n";
            }
            if (patientInfo.medicalHistory.medications.Count > 3)
            {
                medicationsStr += $"... and {patientInfo.medicalHistory.medications.Count - 3} more";
            }
            medicationsText.text = medicationsStr;
        }
        
        // Update Recommendations
        if (recommendationsText != null && patientInfo.predictions != null && patientInfo.predictions.recommendations != null)
        {
            string recommendationsStr = "Recommendations:\n";
            foreach (var rec in patientInfo.predictions.recommendations)
            {
                recommendationsStr += $"• {rec}\n";
            }
            recommendationsText.text = recommendationsStr;
        }
    }
}