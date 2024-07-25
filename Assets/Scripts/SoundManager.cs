using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton dont destroy on load
    public static SoundManager Instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    
    private Dictionary<string, AudioClip> _sounds = new Dictionary<string, AudioClip>();

    [SerializeField] private string _resourcePath = "Sounds/";
    void Start()
    {   
        foreach (var sound in Resources.LoadAll<AudioClip>(_resourcePath))
            _sounds.Add(sound.name, sound);
        
    }

    public void Play(string name)
    {
        if (_sounds.ContainsKey(name))
            AudioSource.PlayClipAtPoint(_sounds[name], Camera.main.transform.position);

    }

}
