using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    private Vector3 startPos;
    private float minSwipeDistX, minSwipeDistY;
    private bool isJump = false;
    private bool isSwipe = false;
    private bool isTouch = false;
    private bool enable = false;
    public delegate void OnSwipe(int direction);//up:1 right:2 down:3 left:0
    private OnSwipe onSwipe;

    private bool buttonTof = true;

    public void initialize(OnSwipe onSwipe)
    {
        this.onSwipe = onSwipe;
    }

    // Use this for initialization
    void Start () {
        minSwipeDistX = minSwipeDistY = Screen.width / 6;
    }
	
	// Update is called once per frame
    void Update()
    {
        if (enable)
        {
            if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            startPos = touch.position;
                            break;
                        case TouchPhase.Moved:
                            isTouch = true;
                            float swipeDistHorizontal =
                                (new Vector3(touch.position.x, 0, 0) - new Vector3(startPos.x, 0, 0)).magnitude;
                            float swipeDistVertical =
                                (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
                            if (swipeDistHorizontal > minSwipeDistX)
                            {
                                float swipeValue = Mathf.Sign(touch.position.x - startPos.x);
                                if (swipeValue > 0 && !isSwipe) //to right swipe
                                {
                                    isTouch = false;
                                    isSwipe = true;
                                    if (onSwipe != null) onSwipe(2);
                                    Debug.Log("Right");
                                }
                                else if (swipeValue < 0 && !isSwipe) //to left swipe
                                {
                                    isTouch = false;
                                    isSwipe = true;
                                    if (onSwipe != null) onSwipe(0);
                                    Debug.Log("Left");
                                }
                            }
                            // add swipe to up
                            if (swipeDistVertical > minSwipeDistY)
                            {
                                float swipeValueY = Mathf.Sign(touch.position.y - startPos.y);
                                if (swipeValueY > 0 && !isSwipe)
                                {
                                    isTouch = false;
                                    isSwipe = true;
                                    if (onSwipe != null) onSwipe(1);
                                    Debug.Log("Up");
                                }
                                if (swipeValueY < 0 && !isSwipe)
                                {
                                    isTouch = false;
                                    isSwipe = true;
                                    if (onSwipe != null) onSwipe(3);
                                    Debug.Log("Down");
                                }
                            }
                            break;
                        case TouchPhase.Stationary:
                            isJump = true;
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            isTouch = false;
                            isSwipe = false;
                            break;
                    }
                }
            }
            else
            {
                if (buttonTof)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        Debug.Log("Up");
                        if (onSwipe != null) onSwipe(1);
                        buttonTof = false;
                        StartCoroutine(stopForSomeWhile());
                    }

                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        Debug.Log("Down");
                        if (onSwipe != null) onSwipe(3);
                        buttonTof = false;
                        StartCoroutine(stopForSomeWhile());
                    }

                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        Debug.Log("Left");
                        if (onSwipe != null) onSwipe(0);
                        buttonTof = false;
                        StartCoroutine(stopForSomeWhile());
                    }

                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        Debug.Log("Right");
                        if (onSwipe != null) onSwipe(2);
                        buttonTof = false;
                        StartCoroutine(stopForSomeWhile());
                    }
                }

            }
        }
    }

    public IEnumerator stopForSomeWhile()
    {
        yield return new WaitForSeconds(0.1f);
        buttonTof = true;
    }

    public void setEnable(bool val)
    {
        enable = val;
    }
}
