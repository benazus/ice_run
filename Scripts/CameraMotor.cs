using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour {

    public Transform lookAt;
    public Vector3 offset;
    public Vector3 rotation = new Vector3(35f, 0, 0);

    public bool IsMoving { set; get; }

    private void LateUpdate() {
        if (IsMoving == false)
            return;

        Vector3 desiredPosition = lookAt.position + offset;
        desiredPosition.x = 0;
        //this.transform.position = Vector3.Lerp(this.transform.position, desiredPosition, Time.deltaTime);
        this.transform.position = desiredPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), 0.05f);
    }
}
