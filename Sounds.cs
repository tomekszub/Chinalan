using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Sounds : MonoBehaviour
{
    public AudioClip defaultSound;    // domyslny dzwiek grany w przypadku podania nieprzewidzianej nazwy dzwieku
    //public AudioClip capturingSound;  // dzwiek bicia pionka
    //public AudioClip thunderSound;
    //public AudioClip arrowSound;
    [SerializeField]
    List<AudioClip> sounds = new List<AudioClip>();
    Dictionary<string, AudioClip> soundsDB = new Dictionary<string, AudioClip>();
    AudioSource source;
	// Use this for initialization
    void Awake()
    {
        source = GetComponent<AudioSource>();
        foreach (var item in sounds)
        {
            soundsDB.Add(item.name, item);
        }
    }
	public void PlaySound(string name)
    {
        source.clip = soundsDB.ContainsKey(name) ? soundsDB[name] : defaultSound;
        source.Play();
    }
}
