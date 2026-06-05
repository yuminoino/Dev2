using UnityEngine;

public class OggettoSulTavoloBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Singleton.OggettiSulTavolo.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
