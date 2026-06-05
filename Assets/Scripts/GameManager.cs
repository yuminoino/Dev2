using Oculus.Platform;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    
    public List <GameObject> OggettiSulTavolo = new List<GameObject>();

    public int VitaGiocatore = 100;

    void OnEnable()
    {
        if (Singleton != null && Singleton != this)
        {
            Debug.LogError("NON METTERE  DUE SINGLETON");
            Destroy(this);
        }
        Singleton = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDestroy()
    {
        GameManager.Singleton.OggettiSulTavolo.Remove(gameObject);
    }
    public void SearchForTables()
        
        
    {
        GameObject currentTable = GameObject.Find("Table");
        NavMeshObstacle myObstacle = currentTable.AddComponent<NavMeshObstacle>();
        myObstacle.carving = true;
    }
}
