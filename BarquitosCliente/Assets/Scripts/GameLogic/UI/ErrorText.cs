using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorText : MonoBehaviour
{

  void Start()
  {
    if (GameManager.Instance() && GameManager.Instance().ErrorMessage != "")
      GetComponent<Text>().text = GameManager.Instance().ErrorMessage;
  }
}
