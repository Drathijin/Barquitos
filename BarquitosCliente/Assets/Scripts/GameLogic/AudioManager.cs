using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  public enum Effecs
  {
    Click,
    Shoot,
    Hit,
    Miss,
    Win
  }

  [SerializeField]
  AudioSource effectSource, musicSource;
  [SerializeField]
  List<AudioClip> effectClips, musicClips;

  private void Start() {
    GameManager.Instance().SetAudioManager(this);
  }
  public void PlayEffect(Effecs ef)
  {
    if(effectSource.isPlaying)
      return;
    effectSource.clip = effectClips[(int)ef];
    effectSource.Play();
  }
}
