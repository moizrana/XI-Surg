using System;

[Serializable]
public class VitalSignsResponse
{
    public string timestamp;
    public VitalSignsData data;
    public string patientId;
}

[Serializable]
public class VitalSignsData
{
    public int batteryLevel;
    public bool bedOccupancy;
    public BloodPressure bloodPressure;
    public string deviceStatus;
    public int glucose;
    public int heartRate;
    public int oxygenLevel;
    public string patientId;
    public int respiratoryRate;
    public int signalStrength;
    public float temperature;
    public string timestamp;
}

[Serializable]
public class BloodPressure
{
    public int diastolic;
    public int systolic;
}