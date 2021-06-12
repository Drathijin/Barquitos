using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    List<AttackData> attacks_;

    void Start()
    {
        attacks_ = new List<AttackData>();
        GameManager.Instance().SetAIManager(this);
    }

    public void ManageTurn()    // Tomar la decision de ataque en el turno y guardarla para el ResolveTurn
    {
        attacks_.Clear();
    }
    public void ResolveTurn()   // Ejecutar la decisión de ataque tomada en el ManageTurn
    {

    }
}
