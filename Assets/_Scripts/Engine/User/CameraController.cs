namespace Adventure.Engine.User
{
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {

        [SerializeField] float sensitivity = 1.0f; 
        [SerializeField] Vector3 offset;

        [Header("Pitch Settings")]
        [SerializeField] float maxPitch;
        [SerializeField] AnimationCurve pitchHeightCurve;
        [SerializeField] AnimationCurve pitchForwardCurve;

        [Header("Camera Rig Transforms")]
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
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity, 0f, maxPitch);
            rotationArm.rotation = Quaternion.Euler(pitch, rotationArm.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivity, 0f);
           
        }
    }
}