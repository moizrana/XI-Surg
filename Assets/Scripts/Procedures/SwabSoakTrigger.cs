using UnityEngine;

public class SwabSoakTrigger : MonoBehaviour
{
    public InjectionProcedureManager procedureManager;
    private bool soaked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (soaked) return;

        if (other.CompareTag("AntisepticVial"))
        {
            if (procedureManager.currentStep == InjectionProcedureManager.Step.ApplyToSwab)
            {
                soaked = true;

                //Set swab as soaked (important!)
                GetComponent<CottonSwab>()?.SetSoaked();

                UINotificationManager.ShowInfo("Swab soaked in antiseptic.");
                procedureManager.ReportAction(InjectionProcedureManager.Step.ApplyToSwab);
            }
            else
            {
                UINotificationManager.ShowError("Swab soaked at the wrong time.");
                procedureManager.ReportAction(InjectionProcedureManager.Step.PickAntiseptic); // force-fail
            }
        }
    }

}
