using UnityEngine;

public class InjectPatient : MonoBehaviour
{
    public InjectionProcedureManager procedureManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SyringeTip"))
        {
            if (procedureManager.currentStep == InjectionProcedureManager.Step.Inject)
            {
                UINotificationManager.ShowInfo("Injection performed via tip.");
                procedureManager.ReportAction(InjectionProcedureManager.Step.Inject);
            }
            else
            {
                UINotificationManager.ShowWarning("Injection out of order.");
                procedureManager.ReportAction(InjectionProcedureManager.Step.PickAntiseptic);
            }
        }
    }

}
