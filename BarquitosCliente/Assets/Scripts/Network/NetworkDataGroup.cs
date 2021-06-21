using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class NetworkDataGroup : MonoBehaviour
{
  public NetworkData data_;
  public string ip;
  public string port;
  public bool wrongIp = false;
  public bool wrongPort = false;

  void Start()
  {
    data_ = new NetworkData();
  }

  public void SetPlayerName(string name)
  {
    if (name == "")
      name = "Player";

    var aux = System.Text.UnicodeEncoding.Unicode.GetBytes(name);
    Array.Resize<Byte>(ref aux, 24);

    data_.playerName = Encoding.Unicode.GetString(aux);
  }

  public void SetIp(string ip)
  {
    if (ip == "" || !CheckIp(ip))
      wrongIp = true;
    else
    {
      this.ip = ip;
      wrongIp = false;
    }
  }

  private bool CheckIp(string ip)
  {
    for (int i = 0; i < ip.Length; i++)
    {
      char c = ip[i];
      if (char.IsLetter(c))
        return false;
    }

    string[] splits = ip.Split('.');

    if (splits.Length != 4)
      return false;

    foreach (string split in splits)
    {
      int s = int.Parse(split);
      if (s > 255 || s < 0)
        return false;
    }

    return true;
  }

  public void SetPort(string port)
  {
    if (port == "" || !CheckPort(port))
      wrongPort = true;
    else
    {
      wrongPort = false;
      this.port = port;
    }

  }

  private bool CheckPort(string port)
  {
    for (int i = 0; i < port.Length; i++)
    {
      char c = port[i];
      if (!char.IsDigit(c))
        return false;
    }
    return true;
  }

  public void SetBr(bool br)
  {
    data_.battleRoyale = br;
  }
}