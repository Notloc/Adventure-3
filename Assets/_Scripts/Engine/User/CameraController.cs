namespace Adventure.Engine.User
{
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {

        [SerializeField] Vector3 offset;
        [SerializeField] float maxPitch;
        [SerializeField] AnimationCurve pitchHeightCurve;
        [SerializeField] AnimationCurve pitchForwardCurve;

        [SerializeField] Transform cameraRig;
        [SerializeField] Transform rotationArm;
        [SerializeField] Transform cameraTransform;
        [SerializeField] Transform followTarget;
        float pitch = 0.0f;

        void Update()
        {
            Rotate();
            Follow();
        }

        void Follow()
        {
            Vector3 rotatedOffset = (this.transform.forward * offset.z * pitchForwardCurve.Evaluate(pitch / maxPitch))
                                        + (this.transform.right * offset.x)
                                            + (this.transform.up * offset.y * pitchHeightCurve.Evaluate(pitch / maxPitch));

            cameraRig.position = followTarget.position;
            cameraTransform.localPosition = rotatedOffset;

        }

        void Rotate()
        {
            if (!Input.GetButton("Rotate Camera"))
                return;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y"), 0f, maxPitch);
            rotationArm.rotation = Quaternion.Euler(pitch, rotationArm.rotation.eulerAngles.y + Input.GetAxis("Mouse X"), 0f);
           
        }
    }
}