using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class BallHandler : MonoBehaviour
{

  [SerializeField] private GameObject ballPrefab; // make a ref to the prefab ball
  [SerializeField] private Rigidbody2D anchorPoint; // ref to the anchor point(spring joint)
  [SerializeField] private float delayInSeconds;
  [SerializeField] private float respawnDelay; // time needed to respawn the ball
                                               //! because we are spawning the ball now we dont need the bottom part to be serialized
                                               //! so we removed the serializedField part to remove them from the inspektor
                                               // [SerializeField] private Rigidbody2D currentBallRigidbody; // SerializeField makes it available in the Unity inspektor
                                               // [SerializeField] private SpringJoint2D currentSPringJoint;
                                               //! -------------------------------------------------------------------------------------
  private Rigidbody2D currentBallRigidbody;
  private SpringJoint2D currentSPringJoint;

  private Camera mainCamera; // declaring a variable mainCamera with type Camera
  private bool isDragging; // are we dragging the ball 
  // Start is called before the first frame update
  void Start()
  {
    mainCamera = Camera.main; // reference to the camera object
    SpawnNewBall();
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

  private void SpawnNewBall()
  {
    //* ballPrefab - the thing that we want to spawn
    //* anchorPoint.position - the position where we want to spawn the object
    //* Quaternion.identity - the rotation of the object, (that we don`t care about)
    //! rotations are stored in this datatype called Quaternion (which get all weird because it has x,y,z and w so to prevent the weird things??? we can use the .identity which means the default rotation(need to see the documentation!!!!!!))
    //! instantiate returns a game object which is the instance we just spawned in
    GameObject ballInstance = Instantiate(ballPrefab, anchorPoint.position, Quaternion.identity);

    // in the <> in the method GetComponent we put the type of component we want to get
    //! we are declaring the  variables and the type at line 17 - 18
    currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
    currentSPringJoint = ballInstance.GetComponent<SpringJoint2D>();

    // attached the rigid body to the anchorPoint
    //! we've done this in the inspector with drag and drop when we where serializing these fields but now when they are not serialized fields we must connect them here
    currentSPringJoint.connectedBody = anchorPoint;
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

    Invoke(nameof(SpawnNewBall), respawnDelay);
  }
}
