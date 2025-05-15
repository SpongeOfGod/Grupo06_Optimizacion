using UnityEngine;
public class LoaderController : MonoBehaviour
{
    [SerializeField]
    private GameObject loader;
    private void Start()
    {
        AssetsManager.Instance.SubscribeOnLoadComplete(OnAssetsLoadComplete);
    }
    private void OnAssetsLoadComplete()
    {
        HideLoader();
    }
    private void HideLoader()
    {
        loader.SetActive(false);
        // Ejemplo de instanciación de un objeto
        //GameObject instance =
        //AssetsManager.Instance.GetInstance("Ball");
    }
}
