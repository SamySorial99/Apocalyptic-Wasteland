using UnityEngine;

public class hr_FakeParent : MonoBehaviour
{
    [SerializeField] private Transform fakeParent;
    [SerializeField] private float interpolationSpeed = 10.0f; // Adjust as needed

    private Vector3 pos, fw, up;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        pos = fakeParent.transform.InverseTransformPoint(transform.position);
        fw = fakeParent.transform.InverseTransformDirection(transform.forward);
        up = fakeParent.transform.InverseTransformDirection(transform.up);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>

    private void Update()
    {
        // Calculate the interpolation factor based on time (frame rate independent).
        float t = Mathf.Clamp01(Time.deltaTime * interpolationSpeed);

        // Interpolate the position and rotation.
        var newpos = Vector3.Lerp(transform.position, fakeParent.transform.TransformPoint(pos), t);
        var newfw = Vector3.Lerp(transform.forward, fakeParent.transform.TransformDirection(fw), t);
        var newup = Vector3.Lerp(transform.up, fakeParent.transform.TransformDirection(up), t);
        var newrot = Quaternion.LookRotation(newfw, newup);

        // Update the position and rotation.
        transform.position = newpos;
        transform.rotation = newrot;
    }

}
