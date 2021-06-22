using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
  [SerializeField]
  GameObject mainGroup, onlineGroup, aiGroup;
  [SerializeField]
  AIDataGroup aiDataGroup_;
  [SerializeField]
  NetworkDataGroup netDataGroup_;
  [SerializeField]
  AudioManager audioManager_;

  public void Start()
  {
    //mainGroup.SetActive(true);
    //onlineGroup.SetActive(true);
    //aiGroup.SetActive(true);

    //Button[] buttons = FindObjectsOfType<Button>();
    //foreach (Button b in buttons)
    //{
    //  b.onClick.AddListener(Click);
    //}
    
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

  public void LoadLevel(bool ai)
  {
    if (ai)
    {
      GameManager.Instance().SetGameType(GameManager.GameType.AI);
      GameManager.Instance().SetAISetup(aiDataGroup_.data_);
    }
    else
    {
      if (netDataGroup_.wrongIp || netDataGroup_.wrongPort)
        return;
      GameManager.Instance().SetGameType(GameManager.GameType.ONLINE);
      GameManager.Instance().SetNetworkSetup(netDataGroup_.data_);
      GameManager.Instance().ip = netDataGroup_.ip;
      GameManager.Instance().port = netDataGroup_.port;
    }
    GameManager.Instance().LoadLevel("Game");
  }

  public void Exit()
  {
    GameManager.Instance().Exit();
  }

  private void SetGroup(List<GameObject> group, bool set)
  {
    foreach (GameObject g in group)
    {
      g.SetActive(set);
    }
  }
  private void Click()
  {
    audioManager_.PlayEffect(AudioManager.Effecs.Click);
  }

}
