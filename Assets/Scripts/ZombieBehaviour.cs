using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Processors;

public class ZombieBehaviour : MonoBehaviour

{

    public Transform TargetObject;
    public NavMeshAgent ZombieAgent;
    public Animator ZombieAnimator;
    public float MovementThreshold = 0.1f;
    public float AttackDistanceTreshold = 1f;
    public bool isDead = false;
    
    public int ZombieLifePoints = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ZombieAgent.SetDestination(TargetObject.position);
        if (ZombieAgent.velocity.magnitude > MovementThreshold)
            {
            ZombieAnimator.SetBool("isWalking", true);
        }
        else
        {
            ZombieAnimator.SetBool("isWalking", false);
        }


       Vector3 targetpositionAtZombieHeight = new Vector3(TargetObject.position.x, ZombieAgent.transform.position.y, TargetObject.position.z);


        if (Vector3.Distance(ZombieAgent.transform.position, targetpositionAtZombieHeight) < ZombieAgent.stoppingDistance + AttackDistanceTreshold)
        {
            ZombieAnimator.SetTrigger("isAttacking");
        }
      

    }
    public void Attacco()
    {
        GameManager.Singleton.VitaGiocatore --;
    }

    public void ZombieColpito()
    {
        ZombieLifePoints --; // Decrease the zombie's life points by 1
       if (ZombieLifePoints <= 0)
        {
            ZombieAnimator.SetTrigger("isDead");
            Invoke("DestroyZombie", 5f); // Call DestroyZombie after 2 seconds
            isDead = true;
            
        }
    }
    void DestroyZombie()
    {
     Destroy(gameObject);
    }
}
