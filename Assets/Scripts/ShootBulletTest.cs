using UnityEngine;

public class ShootBulletTest : MonoBehaviour
{
    public Rigidbody BulletPrefab;
    public Transform GunTip;
    public float BulletSpeed = 100f;
    public bool ShootBullet;
    


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
            spawnedBullet.transform.position = GunTip.transform.position;
            spawnedBullet.transform.rotation = GunTip.transform.rotation;
            spawnedBullet.GetComponent<Rigidbody>().AddForce(GunTip.transform.forward * BulletSpeed, ForceMode.Impulse);
            ShootBullet = false;

        }
    }
}

