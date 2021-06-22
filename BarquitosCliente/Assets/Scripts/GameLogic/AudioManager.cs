using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  public enum Effecs
  {
    CLick,
    Shoot,
    Hit,
    Miss,
    Win
  }

  [SerializeField]
  AudioSource effectSource, musicSource;
  [SerializeField]
  List<AudioClip> effectClips, musicClips;

  void Start()
  {
    //effectSource.cl
  }

  // Update is called once per frame
  void Update()
  {

  }
}
