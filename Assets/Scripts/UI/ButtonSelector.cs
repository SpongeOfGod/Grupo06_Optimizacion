using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSelector : ManagedUpdateBehaviourNoMono
{
    int numberOfButtons = 3;
    int currentButton = 1;
    RectTransform rectTransform;
    public override void UpdateMe()
    {
        if (isPaused) return;

        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow)) && currentButton + 1 <= numberOfButtons)
        {
            currentButton++;
            GameManager.Instance.PlayAudio(GameManager.Instance.SelectClip);
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.UpArrow)) && currentButton - 1 > 0) 
        {
            currentButton--;
            GameManager.Instance.PlayAudio(GameManager.Instance.SelectClip);
        }

        if (rectTransform == null)
            rectTransform = gameObject.GetComponent<RectTransform>();

        switch (currentButton) 
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.Return))
                    SceneManager.LoadScene("Gameplay");
                
                rectTransform.anchoredPosition = new Vector3(219, 62, 0);

                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.Return))
                    GameManager.Instance.PauseTrigger();

                rectTransform.anchoredPosition = new Vector3(219, -62, 0);
                break;

            case 3:
                if (Input.GetKeyDown(KeyCode.Return))
                    Application.Quit();

                rectTransform.anchoredPosition = new Vector3(219, -186, 0);
                break;
        }
    }
}
