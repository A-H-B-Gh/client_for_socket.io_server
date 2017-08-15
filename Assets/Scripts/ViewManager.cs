using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ViewManager : MonoBehaviour
{
    [SerializeField] private GameObject mapParent;
    [SerializeField] private GameObject playerParent;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject player;

    private GameObject[,] mapObjects;
    private List<GameManager.UserInfo> users;
    private double scale;
    private double size;
    private double startX;
    private double startY;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void initialize(int[,] map , List<GameManager.UserInfo> users)
    {
        this.users = users;
        mapObjects = new GameObject[map.GetLength(0),map.GetLength(1)];
        double xscale = (1f / map.GetLength(0)) /
                  (wall.GetComponent<RectTransform>().sizeDelta.x / mapParent.GetComponent<RectTransform>().sizeDelta.x);
        double yscale = (1f / map.GetLength(1)) /
                  (wall.GetComponent<RectTransform>().sizeDelta.x / mapParent.GetComponent<RectTransform>().sizeDelta.x);
        scale = (xscale<yscale)? xscale : yscale;
        size = scale * wall.GetComponent<RectTransform>().sizeDelta.x;
        startX = -((map.GetLength(1) / 2 - (1 - map.GetLength(1) % 2) * 0.5) * size);
        startY = ((map.GetLength(0) / 2 - (1 - map.GetLength(0) % 2) * 0.5) * size);
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                GameObject type;
                switch (map[i,j])
                {
                    case 0:
                        type = ground;
                        break;
                    case 1:
                        type = wall;
                        break;
                    default:
                        type = ground;
                        break;
                }
                mapObjects[i,j] = Instantiate(type,
                            new Vector3((float)(startX + j * size), (float)(startY - i * size) , 0),
                            Quaternion.identity,
                            mapParent.transform);
                mapObjects[i,j].GetComponent<RectTransform>().localScale = new Vector2((float)scale, (float)scale);

            }
        }

    }

    public bool createNewPlayer(string id , string name , Vector2 position,Color color)
    {
        if (position.x >= mapObjects.GetLength(0) || position.x < 0) return false;
        if (position.y >= mapObjects.GetLength(1) || position.x < 0) return false;
        Vector2 realPosition = new Vector2((float)(startX + position.y * size), (float)(startY - position.x * size));
        GameObject currentGameObject = Instantiate(player,
                            new Vector3(realPosition.x, realPosition.y, -1),
                            Quaternion.identity,
                            playerParent.transform);
        currentGameObject.GetComponent<SpriteRenderer>().color = color;
        currentGameObject.GetComponentInChildren<Text>().text = name;
        currentGameObject.GetComponent<RectTransform>().localScale = new Vector2((float)scale, (float)scale);
        GameManager.UserInfo current = new GameManager.UserInfo(id , name , position , currentGameObject);
        users.Add(current);
        return true;
    }

    public void changePlayerPosition(string id, Vector2 position)
    {
        foreach (GameManager.UserInfo userInfo in users)
        {
            if (userInfo.id == id)
            {
                Vector3 realPosition = new Vector3((float)(startX + position.y * size), (float)(startY - position.x * size),-1);
                userInfo.gameObject.GetComponent<RectTransform>().localPosition = realPosition;
                userInfo.position = position;
                break;
            }
        }
    }

    public void deletePlayer(string id)
    {
        foreach (GameManager.UserInfo userInfo in users)
        {
            if (userInfo.id == id)
            {
                Destroy(userInfo.gameObject);
                users.Remove(userInfo);
                break;
            }
        }
    }


}
