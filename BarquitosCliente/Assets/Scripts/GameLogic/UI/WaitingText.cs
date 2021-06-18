using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingText : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance().GetGameType() == GameManager.GameType.AI)
            gameObject.SetActive(false);
        GameManager.Instance().SetWaitingText(this);
    }
}
