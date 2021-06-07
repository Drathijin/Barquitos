using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instace_;


    private void Awake()
    {
        if (instace_)
        {
            Destroy(gameObject);
            return;
        }
        instace_ = this;
    }
}
