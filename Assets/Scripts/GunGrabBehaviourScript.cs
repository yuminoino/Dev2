using UnityEngine;
using UnityEngine.InputSystem;

public class GunGrabBehaviourScript : MonoBehaviour

{
    public InputAction GrabAction;
    public GameObject TemporaryGunObject;
    public Transform GunDummyTransform;

    Transform grabbedGunObject;

    bool grabOnce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GrabAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (TemporaryGunObject != null && GrabAction.IsPressed())
        {
            if (!grabOnce)
            {


              grabbedGunObject = TemporaryGunObject.transform;

              grabbedGunObject.parent = transform;
              grabbedGunObject.GetComponent<Rigidbody>().isKinematic = true;
              grabbedGunObject.localPosition = GunDummyTransform.localPosition;
                grabbedGunObject.localRotation = GunDummyTransform.localRotation;

                grabOnce = true;
              

            }
        }

        if (grabbedGunObject != null && !GrabAction.IsPressed())
        {
            if (!grabOnce)
            {

             grabbedGunObject.GetComponent<Rigidbody>().isKinematic = true;
             grabbedGunObject.parent = null;
             grabbedGunObject = null;

             grabOnce = false;

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GunTag")
        {
            TemporaryGunObject = other.gameObject.transform.parent.gameObject;
            
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GunTag")
        {
            TemporaryGunObject = null;
        }
    }
}
