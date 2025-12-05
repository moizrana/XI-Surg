using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    [Tooltip("Assign your VR Camera (Main Camera).")]
    public Transform target;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 dir = transform.position - target.position;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
