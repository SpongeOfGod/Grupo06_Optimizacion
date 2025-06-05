using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : ManagedUpdateBehaviourNoMono
{
    public int index = 0;
    public int numberOfButtons = 5;
    public override void UpdateMe()
    {
        base.UpdateMe();

        numberOfButtons = GameManager.Instance.PauseButtonsList.Count;

        if (GameManager.Instance.PauseObject == null || GameManager.Instance.PauseObject.activeSelf == false) return;

        var pos = GameManager.Instance.PointerPause.transform.position;

        GameManager.Instance.PointerPause.transform.position = new Vector3(pos.x, GameManager.Instance.PauseButtonsList[index].transform.position.y, pos.z);

        if (Input.GetKeyDown(KeyCode.DownArrow) && index + 1 < numberOfButtons) 
        {
            index++;
            GameManager.Instance.PlayAudio(GameManager.Instance.SelectClip);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && index - 1 >= 0) 
        {
            index--;
            GameManager.Instance.PlayAudio(GameManager.Instance.SelectClip);
        }

        switch (index) 
        {
            case 0:
                GameManager.Instance.Global.audioMixer.GetFloat("MasterVol", out float Globalvol);

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    Globalvol -= 10;

                if (Input.GetKeyDown(KeyCode.RightArrow))
                    Globalvol += 10;

                Globalvol = Mathf.Clamp(Globalvol, -80, 20);
                    GameManager.Instance.Global.audioMixer.SetFloat("MasterVol", Globalvol);

                break;

            case 1:
                GameManager.Instance.BGM.audioMixer.GetFloat("BGMVol", out float BGMvol);
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    BGMvol -= 10;

                if (Input.GetKeyDown(KeyCode.RightArrow))
                    BGMvol += 10;

                BGMvol = Mathf.Clamp(BGMvol, -80, 20);
                GameManager.Instance.BGM.audioMixer.SetFloat("BGMVol", BGMvol);

                break;

            case 2:
                GameManager.Instance.SFX.audioMixer.GetFloat("BGSVol", out float SFXvol);

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    SFXvol -= 10;

                if (Input.GetKeyDown(KeyCode.RightArrow))
                    SFXvol += 10;

                SFXvol = Mathf.Clamp(SFXvol, -80, 20);
                GameManager.Instance.SFX.audioMixer.SetFloat("BGSVol", SFXvol);

                break;

            case 3:

                if (Input.GetKeyDown(KeyCode.Return)) 
                    GameManager.Instance.PauseTrigger();

            break;

            case 4:

                if (Input.GetKeyDown(KeyCode.Return)) 
                {
                    GameManager.Instance.assetsManager.UnloadAssets();
                    SceneManager.LoadScene("MainMenu");
                }
                break;
        }
    }
}
