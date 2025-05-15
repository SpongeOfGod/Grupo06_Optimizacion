using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
public class AssetsManager
{
    public static AssetsManager Instance { get; private set; }
    public Dictionary<string, GameObject> loadedAssetsGameObjects;
    public Dictionary<string, Texture2D> loadedAssetsTextures;
    public event Action OnLoadComplete;
    public bool useRemoteAssets = true;
    public String localURL = "http://localhost:3000/";
    public String cloudURL = "https://myserver.com/";
    public bool assetsLoaded = false;
    public void Initialize()
    {
        if (Instance == null) { Instance = this; }
        if (useRemoteAssets)
        {
            UnityEngine.AddressableAssets.Addressables.ResourceManager.InternalIdTransformFunc +=
            ChangeAssetUrlToPrivateServer;
        }
        loadedAssetsGameObjects = new Dictionary<string, GameObject>();
        loadedAssetsTextures = new Dictionary<string, Texture2D>();
        LoadAssets();
    }

    protected string ChangeAssetUrlToPrivateServer(IResourceLocation location)
    {
        String assetURL = location.InternalId;
        if (location.InternalId.IndexOf(localURL) != -1)
        {
            assetURL = location.InternalId.Replace(localURL, cloudURL);
        }
        return location.InternalId;
    }
    private void LoadAssets()
    {
        GameManager.Instance.LoadAssets();
    }

    public void ExecuteEvent()
    {
        OnLoadComplete?.Invoke();
    }
    public void SubscribeOnLoadComplete(Action callback)
    {
        OnLoadComplete += callback;
    }
}
