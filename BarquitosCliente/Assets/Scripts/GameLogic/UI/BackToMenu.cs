using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMenu : MonoBehaviour
{
  public void Menu()
  {
    GameManager.Instance().LoadLevel("Menu");
  }
}
