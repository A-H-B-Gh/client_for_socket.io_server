using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private SocketIOComponent io;

    #region send

    public void userConnectSend()
    {
        StartCoroutine(userConnectEnumerator());
    }

    public void playSend(string name, Vector2 position)
    {
        string positionString = position.x + "," + position.y;
        StartCoroutine(playEnumerator(name, positionString));
    }

    public void moveSend(Vector2 position)
    {
        string positionString = position.x + "," + position.y;
        StartCoroutine(moveEnumerator(positionString));
    }

    public void disconnnectSend()
    {
        StartCoroutine(disconnectEnumerator());
    }

    private IEnumerator userConnectEnumerator()
    {
        yield return new WaitForSeconds(2f);
        io.Emit("USER_CONNECT");
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator moveEnumerator(string position)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["position"] = position;
        io.Emit("MOVE", new JSONObject(data));
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator disconnectEnumerator()
    {
        io.Emit("DISCONNECT");
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator playEnumerator(string name, string position)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["position"] = position;
        data["name"] = name;
        io.Emit("PLAY", new JSONObject(data));
        yield return new WaitForEndOfFrame();
    }

    #endregion

    #region recieve

    public delegate void UserConnectedCallBack(string id , string name , Vector2 position , Color color);
    public delegate void PlayCallBack(string id, string name, Vector2 position , Color color);
    public delegate void MoveCallBack(string id , Vector2 position);
    public delegate void UserDisconnectedCallBack(string id);

    private UserConnectedCallBack userConnectedCallBack;
    private PlayCallBack playCallBack;
    private MoveCallBack moveCallBack;
    private UserDisconnectedCallBack userDisconnectedCallBack;

    public void initialize(UserConnectedCallBack userConnectedCallBack,
                           PlayCallBack playCallBack,
                           MoveCallBack moveCallBack,
                           UserDisconnectedCallBack userDisconnectedCallBack)
    {
        this.userConnectedCallBack = userConnectedCallBack;
        this.playCallBack = playCallBack;
        this.moveCallBack = moveCallBack;
        this.userDisconnectedCallBack = userDisconnectedCallBack;

        io.On("MOVE",moveRecieve);
        io.On("PLAY",playRecieve);
        io.On("USER_DISCONNECTED", userDisconnectedRecieve);
        io.On("USER_CONNECTED",userConnectedRecieve);
    }

    public void userConnectedRecieve(SocketIOEvent evt)
    {
        Debug.Log("user connected packet");
        string id = evt.data.GetField("id").ToString().Replace("\"", "");
        string name = evt.data.GetField("name").ToString().Replace("\"", "");
        string color = evt.data.GetField("color").ToString().Replace("\"", "");
        string position = evt.data.GetField("position").ToString().Replace("\"", "");
        Vector2 pos = new Vector2( Convert.ToInt32(position.Split(new char[] { ',' })[0]) ,
                                   Convert.ToInt32(position.Split(new char[] { ',' })[1]) );
        Color col = new Color((float)Convert.ToDouble(color.Split(new char[] { ',' })[0])/256f,
                              (float)Convert.ToDouble(color.Split(new char[] { ',' })[1])/256f,
                              (float)Convert.ToDouble(color.Split(new char[] { ',' })[2])/256f);
        userConnectedCallBack(id , name , pos , col);
    }

    public void moveRecieve(SocketIOEvent evt)
    {
        Debug.Log("move packet");
        string id = evt.data.GetField("id").ToString().Replace("\"", "");
        string position = evt.data.GetField("position").ToString().Replace("\"", ""); 
        Vector2 pos = new Vector2(Convert.ToInt32(position.Split(new char[] { ',' })[0]),
                                   Convert.ToInt32(position.Split(new char[] { ',' })[1]));
        moveCallBack(id , pos);
    }

    public void playRecieve(SocketIOEvent evt)
    {
        Debug.Log("play packet");
        string id = evt.data.GetField("id").ToString().Replace("\"", "");
        string name = evt.data.GetField("name").ToString().Replace("\"", "");
        string color = evt.data.GetField("color").ToString().Replace("\"", "");
        string position = evt.data.GetField("position").ToString().Replace("\"", "");
        Vector2 pos = new Vector2(Convert.ToInt32(position.Split(new char[] { ',' })[0]),
                                   Convert.ToInt32(position.Split(new char[] { ',' })[1]));
        Color col = new Color((float)Convert.ToDouble(color.Split(new char[] { ',' })[0])/256f,
                              (float)Convert.ToDouble(color.Split(new char[] { ',' })[1])/256f,
                              (float)Convert.ToDouble(color.Split(new char[] { ',' })[2])/256f);
        playCallBack(id , name , pos , col);
    }

    public void userDisconnectedRecieve(SocketIOEvent evt)
    {
        Debug.Log("disconnect packet");
        string id = evt.data.GetField("id").ToString();
        userDisconnectedCallBack(id);
    }

    #endregion

    void Start () {
	}
	
	void Update () {
		
	}

    public bool isConnected()
    {
        return io.IsConnected;
    }
}
