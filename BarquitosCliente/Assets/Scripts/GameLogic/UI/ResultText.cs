using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultText : MonoBehaviour
{
    Text text_;
    void Start()
    {
        GameManager.Instance().SetResultText(this);
        text_ = GetComponent<Text>();
    }

    public void OnEnd(bool result)
    {
        gameObject.SetActive(true);
        if (result)
        {
            text_.text = "you win";
            text_.color = Color.green;
        }
        else
        {
            text_.text = "you lose";
            text_.color = Color.red;
        }
    }
}
