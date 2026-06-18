using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float LifeTime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroyBullet", LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        //lancio animation of the hit object
        // distruggo in ritardo l'oggetto colpito per far partire l'animazione
        if (collision.gameObject.tag == "ZombieTag")
        {
            collision.gameObject.GetComponent<ZombieBehaviour>().ZombieColpito();
        }
    }
    
}
