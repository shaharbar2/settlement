using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject objectToFollow;

    public float speed = 2.0f;

    void Update() {
        if (Constants.instance.PLAYER_CONTROL_STYLE != PlayerControlStyle.POINTCLICK_FREECAMERA) {
            float interpolation = speed * Time.deltaTime;

            Vector3 position = this.transform.position;
            position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y, interpolation);
            position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);

            this.transform.position = position;
        }
    }
}