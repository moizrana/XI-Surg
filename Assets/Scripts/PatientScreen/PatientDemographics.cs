// PatientData.cs - Data models for patient information
using System;
using System.Collections.Generic;

[Serializable]
public class PatientInfoResponse
{
    public CurrentStatus currentStatus;
    public MedicalHistory medicalHistory;
    public PersonalInfo personalInfo;
    public Predictions predictions;
}

[Serializable]
public class CurrentStatus
{
    public string consciousness;
    public string diagnosis;
    public string lastUpdated;
    public string mobility;
    public string status;
}

[Serializable]
public class MedicalHistory
{
    public string admissionReason;
    public List<string> allergies;
    public List<string> conditions;
    public string lastCheckup;
    public List<Medication> medications;
}

[Serializable]
public class Medication
{
    public string dosage;
    public string frequency;
    public string name;
    public string startDate;
}

[Serializable]
public class PersonalInfo
{
    public string admissionDate;
    public int age;
    public string bedId;
    public string gender;
    public string name;
    public string roomId;
    public string ward;
}

[Serializable]
public class Predictions
{
    public string alertId;
    public float confidence;
    public List<string> factors;
    public string nextPrediction;
    public string predictedAt;
    public float processingTime;
    public List<string> recommendations;
    public string riskLevel;
    public int riskScore;
    public VitalsUsed vitalsUsed;
}

[Serializable]
public class VitalsUsed
{
    public BloodPressureInfo bloodPressure;
    public int glucose;
    public int heartRate;
    public int oxygenLevel;
    public int respiratoryRate;
    public float temperature;
}

[Serializable]
public class BloodPressureInfo
{
    public int diastolic;
    public int systolic;
}