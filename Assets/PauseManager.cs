using Unity.Mathematics;
using UnityEngine;

public class PauseManager : ManagedUpdateBehaviourNoMono
{
    public int index = 0;
    public int numberOfButtons = 5;
    public override void UpdateMe()
    {
        base.UpdateMe();

        if (GameManager.Instance.PauseObject == null || GameManager.Instance.PauseObject.activeSelf == false) return;


        if (Input.GetKeyDown(KeyCode.DownArrow) && index + 1 <= numberOfButtons)
            index++;

        if (Input.GetKeyDown(KeyCode.UpArrow) && index - 1 >= 0)
            index--;

        switch (index) 
        {
            case 0:
                float vol = -1;

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    vol = GameManager.Instance.GlobalVol - 10;

                    vol = math.remap(0, 100, -80, 20, vol);

                    Debug.Log(vol);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    vol = GameManager.Instance.GlobalVol + 10;

                    vol = math.remap(0, 100, -80, 20, vol);
                }


                if (vol >= 0)
                    GameManager.Instance.Global.audioMixer.SetFloat("MasterVol", vol);

                break;

            case 1:
                vol = -1;

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    vol = GameManager.Instance.BGMVol - 10;

                    vol = math.remap(0, 100, -80, 20, vol);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    vol = GameManager.Instance.BGMVol + 10;

                    vol = math.remap(0, 100, -80, 20, vol);
                }

                if (vol >= 0)
                    GameManager.Instance.BGM.audioMixer.SetFloat("BGMVol", vol);

                break;

            case 2:
                vol = -1;

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    vol = GameManager.Instance.SFXVol - 10;

                    vol = math.remap(0, 100, -80, 20, vol);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    vol = GameManager.Instance.SFXVol + 10;

                    vol = math.remap(0, 100, -80, 20, vol);
                }

                if (vol >= 0)
                    GameManager.Instance.SFX.audioMixer.SetFloat("BGSVol", vol);

                break;
        }
    }
}
