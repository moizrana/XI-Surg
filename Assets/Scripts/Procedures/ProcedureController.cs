using UnityEngine;

public class ProcedureController : MonoBehaviour
{
    public enum ProcedureType { None, Injection, WoundDressing }
    public static ProcedureType ActiveProcedure { get; private set; } = ProcedureType.None;

    public InjectionProcedureManager injectionManager;
    public WoundDressingProcedureManager woundDressingManager;

    public void StartInjectionProcedure()
    {
        SetActiveProcedure(ProcedureType.Injection);
        injectionManager.StartProcedure();
    }

    public void StartWoundDressingProcedure()
    {
        SetActiveProcedure(ProcedureType.WoundDressing);
        woundDressingManager.StartProcedure();
    }

    private void SetActiveProcedure(ProcedureType type)
    {
        ActiveProcedure = type;

        // Enable only the relevant manager
        injectionManager.enabled = (type == ProcedureType.Injection);
        woundDressingManager.enabled = (type == ProcedureType.WoundDressing);
    }
}
