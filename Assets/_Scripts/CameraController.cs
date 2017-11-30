using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Vector3 offset;
    public float maxPitch;
    public AnimationCurve pitchHeightCurve;

    Transform cameraTransform;
    float pitch = 0.0f;
    
	// Use this for initialization
	void Start ()
    {
        cameraTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Rotate();
        Follow();
    }

    void Follow()
    {
        Vector3 rotatedOffset = (this.transform.forward * offset.z) + (this.transform.right * offset.x) + (this.transform.up * offset.y) + (this.transform.up * PitchHeight(pitch));

        cameraTransform.position = this.transform.position + rotatedOffset;

    }

    void Rotate()
    {
        //Rotate player
        this.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));

        //Pitch camera
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y"), 0f, maxPitch);

        //Set camera rotation
        cameraTransform.rotation = Quaternion.Euler(pitch, this.transform.rotation.eulerAngles.y, 0f);
    }

    float PitchHeight(float currentPitch)
    {
        return pitchHeightCurve.Evaluate(currentPitch / maxPitch);
    }

}
