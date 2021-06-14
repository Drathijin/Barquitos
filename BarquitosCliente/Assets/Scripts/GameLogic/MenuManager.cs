using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject mainGroup, aiGroup, onlineGroup;


    public void Start()
    {
        Back();
    }

    public void Back()
    {
        mainGroup.SetActive(true);
        aiGroup.SetActive(false);
        onlineGroup.SetActive(false);
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

    public void LoadAILevel(bool br)
    {
        GameManager.Instance().SetGameType(br ? GameManager.GameType.BRAI : GameManager.GameType.SPAI);
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
