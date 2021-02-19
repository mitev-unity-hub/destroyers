using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class BallHandler : MonoBehaviour
{
  [SerializeField] private Rigidbody2D currentBallRigidbody; // SerializeField makes it available in the Unity inspektor1
  [SerializeField] private SpringJoint2D currentSPringJoint;

  [SerializeField] private float delayInSeconds;
  private Camera mainCamera; // declaring a variable mainCamera with type Camera
  private bool isDragging; // are we dragging the ball 
  // Start is called before the first frame update
  void Start()
  {
    mainCamera = Camera.main; // reference to the camera object
  }


  // static - you can NOT move the object at all
  // dynamic - you can move it and physics are active (act like a normal physics object)
  // kinematic - you can move it but physics are not active(acting on the object)
  // Update is called once per frame
  void Update()
  {
    if (currentBallRigidbody == null) { return; } // if there is no current ball return

    if (!Touchscreen.current.primaryTouch.press.isPressed) // if the player not touching the screen return;
    {

      if (isDragging)
      {
        LaunchBall();
      }

      isDragging = false;
      return;
    }

    isDragging = true;
    currentBallRigidbody.isKinematic = true; // while is in kinematic phisics like spring joint and gravity wont be acting on the object

    Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue(); // return a Vector 2 value

    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition); // convert pixels to unity(world) units

    // Debug.Log(worldPosition);

    currentBallRigidbody.position = worldPosition;
  }

  private void LaunchBall()
  {
    currentBallRigidbody.isKinematic = false; // return it back to dinamic so the gravity and spring joint can be active again
    currentBallRigidbody = null; // clear that entire reference compleately 

    //! the invoke method is needed so it can launch the DetachBall method after sertan amount of time in order the springjoint to launch the ball 
    //? nameof makes a reference to the method so you get an error if you misspelled it 
    Invoke(nameof(DetachBall), delayInSeconds);

  }

  private void DetachBall()
  {
    currentSPringJoint.enabled = false;
    currentBallRigidbody = null;
  }
}
