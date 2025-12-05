using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RetractorInsertionManager : MonoBehaviour
{
    public XRGrabInteractable leftRetractorInteractable;
    public XRGrabInteractable rightRetractorInteractable;

    public Transform leftSocket;
    public Transform rightSocket;

    public GameObject skinLayer;
    public GameObject interiorVisual;

    public VideoPlayer videoPlayer;

    private bool leftInserted = false;
    private bool rightInserted = false;

    private void Update()
    {
        if (!leftInserted && Vector3.Distance(leftRetractorInteractable.transform.position, leftSocket.position) < 0.05f)
        {
            SnapRetractor(leftRetractorInteractable, leftSocket);
            leftInserted = true;
            Debug.Log("Left retractor snapped in.");
            CheckBothInserted();
        }

        if (!rightInserted && Vector3.Distance(rightRetractorInteractable.transform.position, rightSocket.position) < 0.05f)
        {
            SnapRetractor(rightRetractorInteractable, rightSocket);
            rightInserted = true;
            Debug.Log("Right retractor snapped in.");
            CheckBothInserted();
        }
    }

    private void SnapRetractor(XRGrabInteractable retractor, Transform socket)
    {
        // Move and rotate to socket position
        retractor.transform.position = socket.position;
        retractor.transform.rotation = socket.rotation;

        // Disable grabbing so it stays fixed
        retractor.interactionLayers = new UnityEngine.XR.Interaction.Toolkit.InteractionLayerMask();

        // Make rigidbody kinematic so it stays fixed
        Rigidbody rb = retractor.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        // Optionally disable collider or set as trigger to avoid physics issues
        Collider col = retractor.GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void CheckBothInserted()
    {
        if (leftInserted && rightInserted)
        {
            if (skinLayer != null)
                skinLayer.SetActive(false);

            if (interiorVisual != null)
                interiorVisual.SetActive(true);

            if (videoPlayer != null && !videoPlayer.isPlaying)
                videoPlayer.Play();
        }
    }
}
