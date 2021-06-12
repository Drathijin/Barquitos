using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance().SetNetworkManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public abstract void ManageTurn();                  // Tomar la decision de ataque en el turno y guardarla para el ResolveTurn
    //public abstract void ResolveTurn();                 // Ejecutar la decisión de ataque tomada en el ManageTurn
}
