using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject objectToFollow;

    public float speed = 0;

    void Update() {
        speed = Constants.instance.CAMERA_SPEED;

        if (Constants.instance.CAMERA_CONTROL_STYLE == CameraControlStyle.FOLLOW) {
            float interpolation = speed * Time.deltaTime;

            Vector3 position = this.transform.position;
            position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y, interpolation);
            position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);

            this.transform.position = position;
        } else if (Constants.instance.CAMERA_CONTROL_STYLE == CameraControlStyle.PAN_WASD) {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            float camSize = GetComponent<Camera>().orthographicSize;
            Vector3 direction = new Vector3(h, v, 0);
            direction = direction.normalized * speed * camSize * Time.deltaTime;
            transform.position += direction;
        }
    }
}