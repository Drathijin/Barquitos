using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WInnerText : MonoBehaviour
{
    Text text_;
    void Start()
    {
        GameManager.Instance().SetWinnerText(this);
        text_ = GetComponent<Text>();
    }

    public void OnEnd(string winner)
    {
        gameObject.SetActive(true);
        text_.text = winner + " wins";
    }
}
