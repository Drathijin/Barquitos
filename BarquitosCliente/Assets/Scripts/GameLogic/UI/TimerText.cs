using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
  private Text text_;

  void Start()
  {
    text_ = GetComponent<Text>();
    GameManager.Instance().SetTimerText(this);
  }

  public void SetTime(float time)
  {
    text_.text = time.ToString("0.00");
  }
}
