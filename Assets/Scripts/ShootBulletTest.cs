using UnityEngine;

public class ShootBulletTest : MonoBehaviour
{
    public Rigidbody BulletPrefab;
    public Transform GunTip;
    public float BulletSpeed = 1000f;
    public bool ShootBullet;
    bool shootOnce;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (ShootBullet)
        {
           GameObject spawnedBullet = Instantiate(BulletPrefab.gameObject);
            spawnedBullet.transform.position = GunTip.position; //GunTip.position bc GunTip is alr a transform;
            spawnedBullet.transform.rotation = GunTip.rotation;
            spawnedBullet.GetComponent<Rigidbody>().AddForce(GunTip.forward * BulletSpeed);
            ShootBullet = false;

        }
    }
}

