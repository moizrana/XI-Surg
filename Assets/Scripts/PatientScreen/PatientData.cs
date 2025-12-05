using System;

[Serializable]
public class PatientData
{
    public string name;
    public int age;
    public CurrentStatus currentStatus;
    public Baseline baseline;

    [Serializable]
    public class CurrentStatus
    {
        public string diagnosis;
        public string status;
        public string mobility;
        public string consciousness;
    }

    //[Serializable]
    //public class MedicalHistory
    //{
    //    public string admissionReason;
    //    public string[] conditions;
    //    public string lastCheckup;
    //}

        //[Serializable]
    [System.Serializable]
    public class Baseline
    {
        public float temperature;
        public int heartRate;
        public int oxygenLevel;
        public int systolicBP;
        public int diastolicBP;
        public int glucose;
        public int respiratoryRate;
    }
}
