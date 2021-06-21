using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    Button button_;
    Text text_;
    Image imgCmp_;

    void Start()
    {

        imgCmp_ = GetComponent<Image>();
        text_ = GetComponentInChildren<Text>();
        text_.text = "READY";

        button_ = GetComponent<Button>();
        if (GameManager.Instance())
        {
            GameManager.Instance().SetReadyButton(this);
            button_.onClick.AddListener(OnReadyClick);
            OnStateChanged(GameManager.Instance().state_);
        }
    }

    void OnReadyClick()
    {
        button_.interactable = false;
        GameManager.Instance().OnReadyClick();
    }

    public void OnStateChanged(GameManager.GameState state)
    {
        button_.interactable = true;
        gameObject.SetActive(true);
        imgCmp_.color = Color.white;
        switch (state)
        {
            case GameManager.GameState.WAITINGFORPLAYERS:
                gameObject.SetActive(false);
                break;
            case GameManager.GameState.PREPARING:
                text_.text = "READY";
                break;
            case GameManager.GameState.SELECTING:
                text_.text = "ATTACK";
                break;
            case GameManager.GameState.ATTACKING:
                text_.text = "ATTACKING";
                button_.interactable = false;
                break;
            case GameManager.GameState.END:
                text_.text = "return";
                break;
            default:
                break;
        }
    }
}
