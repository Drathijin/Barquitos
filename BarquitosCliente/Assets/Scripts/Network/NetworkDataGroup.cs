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
    if (ip == "")
      ip = "127.0.0.1";
    this.ip = ip;
  }

  public void SetPort(string port)
  {
    if (port == "")
      port = "8080";
    this.port = port;
  }

  public void SetBr(bool br)
  {
    data_.battleRoyale = br;
  }
}