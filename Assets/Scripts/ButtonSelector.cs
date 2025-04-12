using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSelector : ManagedUpdateBehaviourNoMono
{
    int numberOfButtons = 2;
    int currentButton = 1;
    RectTransform rectTransform;
    public override void UpdateMe()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow)) && currentButton + 1 <= numberOfButtons) 
            currentButton++;

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.UpArrow)) && currentButton - 1 > 0)
            currentButton--;

        if (rectTransform == null)
            rectTransform = gameObject.GetComponent<RectTransform>();

        switch (currentButton) 
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.Space))
                    SceneManager.LoadScene("Gameplay");
                
                rectTransform.anchoredPosition = new Vector3(260, 0, 0);

                break;

            case 2:
                if (Input.GetKeyDown(KeyCode.Space))
                    Application.Quit();

                rectTransform.anchoredPosition = new Vector3(260, -124, 0);
                break;
        }
    }
}
