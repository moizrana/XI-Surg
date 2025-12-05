using UnityEngine;

public class WoundDressingProcedureManager : MonoBehaviour
{
    public enum Step
    {
        None,
        PickAntiseptic,
        ApplyAntiseptic,
        PickGauze,
        ApplyGauze,
        BandageWound,
        ProcedureComplete,
        ProcedureFailed
    }

    public Step currentStep = Step.None;

    public void StartProcedure()
    {
        currentStep = Step.PickAntiseptic;
        UINotificationManager.ShowInfo("Procedure started. Step: Pick up antiseptic.");
    }

    public void ReportAction(Step stepDone)
    {
        if (currentStep == Step.ProcedureFailed || currentStep == Step.ProcedureComplete)
            return;

        if (stepDone == currentStep)
        {
            AdvanceToNextStep();
        }
        else
        {
            UINotificationManager.ShowWarning($"Incorrect step! Tried to do {stepDone} but expected {currentStep}. Procedure failed.");
            currentStep = Step.ProcedureFailed;
            OnProcedureFailed();
        }
    }


    private void AdvanceToNextStep()
    {
        switch (currentStep)
        {
            case Step.PickAntiseptic:
                currentStep = Step.ApplyAntiseptic;
                UINotificationManager.ShowInfo("Antiseptic picked. Apply it to wound.");
                break;
            case Step.ApplyAntiseptic:
                currentStep = Step.PickGauze;
                UINotificationManager.ShowInfo("Antiseptic applied. Pick up gauze.");
                break;
            case Step.PickGauze:
                currentStep = Step.ApplyGauze;
                UINotificationManager.ShowInfo("Gauze picked. Apply it on wound.");
                break;
            case Step.ApplyGauze:
                currentStep = Step.BandageWound;
                UINotificationManager.ShowInfo("Gauze applied. Wrap the bandage.");
                break;
            case Step.BandageWound:
                currentStep = Step.ProcedureComplete;
                UINotificationManager.ShowInfo("Wound dressing procedure complete!");
                break;
        }
    }

    private void OnProcedureFailed()
    {
        UINotificationManager.ShowError("Wound dressing procedure failed.");
        // Optional: trigger UI feedback, sound, reset, etc.
    }
}
