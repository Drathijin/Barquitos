using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  public enum Effecs
  {
    Click,
    Shoot
  }

  public enum Music
  {
    Win,
    Loose,
    Menu,
    Game
  }

  [SerializeField]
  AudioSource effectSource, musicSource;
  [SerializeField]
  List<AudioClip> effectClips, winMusic, looseMusic, menuMusic, gameMusic;

  [SerializeField]
  List<List<AudioClip>> musicClips;

  private void Start()
  {
    musicClips = new List<List<AudioClip>>();
    musicClips.Add(winMusic);
    musicClips.Add(looseMusic);
    musicClips.Add(menuMusic);
    musicClips.Add(gameMusic);
    GameManager.Instance().SetAudioManager(this);
  }
  public void PlayEffect(Effecs ef)
  {
    if (effectSource.isPlaying)
      return;
    effectSource.clip = effectClips[(int)ef];
    effectSource.Play();
  }

  public void PlayMusic(Music mu, bool loop)
  {
    musicSource.loop = loop;
    musicSource.clip = musicClips[(int)mu][Random.Range(0, musicClips[(int)mu].Count)];
    musicSource.Play();
  }
}
