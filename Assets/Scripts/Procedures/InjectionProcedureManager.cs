using UnityEngine;
using UnityEngine.Events;

public class InjectionProcedureManager : MonoBehaviour
{
    public enum Step { None, PickAntiseptic, ApplyToSwab, ApplySwabToSkin, Inject, ApplyGauze, Complete, Failed }
    public Step currentStep = Step.None;

    public GameObject cottonSwab;
    public GameObject gauze;

    public UnityEvent onProcedureStart;
    public UnityEvent onProcedureFailed;
    public UnityEvent onProcedureComplete;

    public void StartProcedure()
    {
        currentStep = Step.PickAntiseptic;
        UINotificationManager.ShowInfo("Procedure started. Awaiting antiseptic pickup.");
        onProcedureStart.Invoke();
    }

    public void ReportAction(Step attemptedStep)
    {
        if (currentStep == Step.Failed || currentStep == Step.Complete) return;

        if (attemptedStep == currentStep)
        {
            AdvanceStep();
        }
        else
        {
            UINotificationManager.ShowError($"Procedure failed. Expected: {currentStep}, but got: {attemptedStep}");
            currentStep = Step.Failed;
            onProcedureFailed.Invoke();
        }
    }

    private bool skinSwabbed = false;

    public void OnSkinSwabbed()
    {
        if (skinSwabbed) return; // prevent re-swabbing

        if (!cottonSwab || !cottonSwab.GetComponent<CottonSwab>().isSoaked)
        {
            UINotificationManager.ShowWarning("Swabbing failed: Cotton swab is not soaked!");
            FailProcedure("Swabbing with dry cotton swab");
            return;
        }

        if (currentStep == Step.ApplySwabToSkin)
        {
            UINotificationManager.ShowInfo("Skin successfully swabbed.");
            currentStep = Step.Inject;
            skinSwabbed = true;
        }
        else
        {
            UINotificationManager.ShowWarning("Swabbing out of sequence.");
            FailProcedure("Swabbed out of order");
        }
    }

    private void AdvanceStep()
    {
        switch (currentStep)
        {
            case Step.PickAntiseptic:
                currentStep = Step.ApplyToSwab;
                UINotificationManager.ShowInfo("Antiseptic picked. Awaiting swab soaking.");
                break;

            case Step.ApplyToSwab:
                currentStep = Step.ApplySwabToSkin;
                UINotificationManager.ShowInfo("Swab soaked. Awaiting skin swab.");
                break;

            case Step.ApplySwabToSkin:
                currentStep = Step.Inject;
                UINotificationManager.ShowInfo("Swab applied. Awaiting injection.");
                break;

            case Step.Inject:
                currentStep = Step.Complete;
                UINotificationManager.ShowInfo("Injection complete. Procedure succeeded!");
                onProcedureComplete.Invoke();
                break;
        }
    }

    private void FailProcedure(string reason)
    {
        UINotificationManager.ShowError($"Procedure failed: {reason}");
        currentStep = Step.Failed;
        onProcedureFailed.Invoke();
    }
}
