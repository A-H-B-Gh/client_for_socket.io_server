using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManagerScript;
    [SerializeField] private NetworkManager networkManagerScript;
    [SerializeField] private ViewManager viewManager;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject disconnectButton;
    [SerializeField] private Text nameText;
    [SerializeField] private Text connecting;
    private GameData gameData;

    #region DATA

    public class GameData
    {
        public string myId;
        public int[,] map;
        public List<UserInfo> users;

        public GameData()
        {
            users = new List<UserInfo>();
            map = new int[12, 12]
            {
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            };
        }
        
        public UserInfo giveMyData()
        {
            foreach (UserInfo userInfo in users)
            {
                if (userInfo.id == myId)
                {
                    return userInfo;
                }
            }
            return null;
        }
    }

    public class UserInfo
    {

        public string id;
        public string name;
        public Vector2 position;
        public GameObject gameObject;

        public UserInfo(string id, string name, Vector2 position , GameObject gameObject)
        {
            this.id = id;
            this.name = name;
            this.position = position;
            this.gameObject = gameObject;
        }

    }

    #endregion

    #region NETWORK

    public void userConnectedCallBack(string id, string name, Vector2 position,Color color)
    {
        viewManager.createNewPlayer(id, name, position, color);
    }

    public void playCallBack(string id, string name, Vector2 position , Color color)
    {
        gameData.myId = id;
        inputManagerScript.setEnable(true);
        playPanel.SetActive(false);
        disconnectButton.SetActive(true);
        viewManager.createNewPlayer(id, name, position , color);
    }

    public void moveCallBack(string id, Vector2 position)
    {
        viewManager.changePlayerPosition(id , position);
    }

    public void userDisconnectedCallBack(string id)
    {
        viewManager.deletePlayer(id);
        playPanel.SetActive(true);
        disconnectButton.SetActive(false);
        inputManagerScript.setEnable(false);
        gameData.myId = null;
    }


    #endregion

    #region INPUT

    private void onSwipe(int dir)
    {
        Vector2 temp = new Vector2(gameData.giveMyData().position.x , gameData.giveMyData().position.y);
        switch (dir)
        {
            case 0:
                temp.y--;
                break;
            case 1:
                temp.x--;
                break;
            case 2:
                temp.y++;
                break;
            case 3:
                temp.x++;
                break;
        }
        if (temp.x < 1 || temp.x >gameData.map.GetLength(0)-2) return;
        if (temp.y < 1 || temp.y > gameData.map.GetLength(1) - 2) return;
        networkManagerScript.moveSend(temp);
//        viewManager.changePlayerPosition(gameData.myId, gameData.giveMyData().position);
    }

    public void playButtonClicked()
    {
        networkManagerScript.playSend(nameText.text,new Vector2(1,1));
//        inputManagerScript.setEnable(true);
//        playPanel.SetActive(false);
    }

    public void disconnectButtonClicked()
    {
        networkManagerScript.disconnnectSend();
        userDisconnectedCallBack(gameData.myId);
    }

    #endregion


    void Start ()
    {
        gameData = new GameData();
        viewManager.initialize(gameData.map , gameData.users);
        inputManagerScript.initialize(onSwipe);
        inputManagerScript.setEnable(false);
        networkManagerScript.initialize(userConnectedCallBack,playCallBack,moveCallBack,userDisconnectedCallBack);
        networkManagerScript.userConnectSend();
    }

    void Update()
    {
        if(!networkManagerScript.isConnected()) connecting.text = ":(((";
        else connecting.text = ":)))";
    }

    void OnApplicationQuit()
    {
        networkManagerScript.disconnnectSend();
    }

}
