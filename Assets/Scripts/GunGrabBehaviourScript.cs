using UnityEngine;
using UnityEngine.InputSystem;

public class GunGrabBehaviourScript : MonoBehaviour

{
    [Header("Manipulations:")]
    public InputAction GrabAction;
    public Transform GunDummyTransform;

    [Header("Shoot System:")]
    public InputAction ShootAction;
    public Rigidbody BulletPrefab;
    public Transform GunTip;
    public float ShootForce = 1000;
    bool shootOnce;
    

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

     private void FixedUpdate()
    {
        if (grabbedGunObject != null && ShootAction.IsPressed()) {
            if (!shootOnce)
            {
                ShootBullet();
                shootOnce = true;
            }
        }
        else
        {
            shootOnce = false;
        }

        void ShootBullet()
            
                
                {
                    GameObject spawnedBullet = Instantiate(BulletPrefab.gameObject);
                    spawnedBullet.transform.position = GunTip.position; //GunTip.position bc GunTip is alr a transform;
                    spawnedBullet.transform.rotation = GunTip.rotation;
                    spawnedBullet.GetComponent<Rigidbody>().AddForce(GunTip.forward * ShootForce);
                    
                }
            
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gun")
        {
            temporaryGunObject = other.gameObject.transform.gameObject;
            
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Gun")
        {
            temporaryGunObject = null;
        }
    }
}

}
