using UnityEngine;
using UnityEngine.InputSystem;

public class GunGrabBehaviourScript : MonoBehaviour

{
    public InputAction GrabAction;
    
    public Transform GunDummyTransform;

    GameObject temporaryGunObject;

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
        if (temporaryGunObject != null && GrabAction.IsPressed())
        {
            if (!grabOnce)
            {


              grabbedGunObject = temporaryGunObject.transform;

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
            temporaryGunObject = other.gameObject.transform.parent.gameObject;
            
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GunTag")
        {
            temporaryGunObject = null;
        }
    }
}
