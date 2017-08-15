using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Controller : MonoBehaviour
{

    public SocketIOComponent socket;

    // Use this for initialization
	void Start ()
	{
	    StartCoroutine(ConnectToServer());
        socket.On("USER_CONNECTED",onUSERConnected);
        socket.On("PLAY", onGameStated);
    }

    IEnumerator ConnectToServer()
    {
        yield return new WaitForSeconds(0.5f);
        socket.Emit("USER_CONNECT");
        yield return new WaitForSeconds(1f);
        Dictionary<string,string> data = new Dictionary<string, string>();
        data["name"] = "sahand";
        data["position"] = "0,0";
        socket.Emit("PLAY", new JSONObject(data));
    }

    private void onUSERConnected(SocketIOEvent evt)
    {
        Debug.Log(("Get The message from server : " + evt.data));
    }

    private void onGameStated(SocketIOEvent evt)
    {
        Debug.Log(("Game Started and your data is : " + evt.data));
    }

}
