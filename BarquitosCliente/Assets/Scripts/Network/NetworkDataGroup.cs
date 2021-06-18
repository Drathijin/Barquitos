using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        data_.playerName = name;
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