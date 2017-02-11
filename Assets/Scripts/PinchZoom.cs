using UnityEngine;
using System.Collections;

public class PinchZoom : MonoBehaviour {

    private float rotationSpeed = 50f;

	
	// Update is called once per frame
	void Update () {

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            //Debug.Log( transform.rotation.y + ":"+touchDeltaPosition.x );

            //rotate object
            transform.Rotate(Vector3.up * -touchDeltaPosition.x * Time.deltaTime * rotationSpeed);
        }else if (Input.touchCount == 2)
        {
            Vector3 initScale = transform.localScale;
            //get touches
            Touch touchOne = Input.GetTouch(0);
            Touch touchTwo = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
            float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = touchDeltaMag / prevTouchDeltaMag;


            transform.localScale = initScale * deltaMagnitudeDiff;
        }
	
	}
}
