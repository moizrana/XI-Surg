using UnityEngine;


public class HapticFeedbackTrigger : MonoBehaviour
{
    [Range(0, 1)]
    public float intensity = 0.5f;
    public float duration = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        var interactor = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        if (interactor != null)
        {
            SendHaptics(interactor);
        }
    }

    private void SendHaptics(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
    {
        if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor direct)
        {
            direct.SendHapticImpulse(intensity, duration);
        }
        else if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor ray)
        {
            ray.SendHapticImpulse(intensity, duration);
        }
    }
}
