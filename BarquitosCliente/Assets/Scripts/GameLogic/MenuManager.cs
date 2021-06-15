using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject mainGroup, onlineGroup, aiGroup;
    [SerializeField]
    AIDataGroup dataGroup_;

    public void Start()
    {
        Back();
    }

    public void Back()
    {
        mainGroup.SetActive(true);
        onlineGroup.SetActive(false);
        aiGroup.SetActive(false);
    }

    public void AI()
    {
        mainGroup.SetActive(false);
        aiGroup.SetActive(true);
    }

    public void Online()
    {
        mainGroup.SetActive(false);
        onlineGroup.SetActive(true);
    }

    public void LoadAILevel()
    {
        GameManager.Instance().SetGameType(GameManager.GameType.AI);
        GameManager.Instance().SetAISetup(dataGroup_.data_);
        GameManager.Instance().LoadLevel("Game");
    }

    public void Exit()
    {
        GameManager.Instance().Exit();
    }

    private void SetGroup(List<GameObject> group, bool set)
    {
        foreach(GameObject g in group)
        {
            g.SetActive(set);
        }
    }

}
