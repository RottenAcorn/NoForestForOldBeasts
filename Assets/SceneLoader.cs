using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SceneLoader : MonoBehaviour
{
    #region Singleton
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    void Start()
    {
        DontDestroyOnLoad(this);

        //on the start of the application load MainMenu

        LoadScene("MainMenu");
    }

    public async void LoadScene(string sceneName)
    {
        if (NetworkManager.Singleton != null)
        {
            await Task.Delay(500);
            SceneManager.LoadScene(sceneName);
        }
    }
}
