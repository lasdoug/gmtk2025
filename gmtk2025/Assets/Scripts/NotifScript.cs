using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class NotifScript : MonoBehaviour
{
    public TMP_Text firstMessage, old1, old2, old3;
    string recent = "";
    Stack messages = new Stack();
    float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstMessage.text = "...";
        old1.text = "";
        old2.text = "";
        old3.text = "";
        PushMessage("Testing this thing");
        PushMessage("Another message");
        PushMessage("Lets do a long one to see what happens how long can we go");
        PushMessage("One more thanks");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0) timer -= Time.deltaTime;

        if (messages.Count > 0)
        {
            if (timer <= 0)
            {
                DisplayNewMessage((string)messages.Pop());
            }
        }
    }

    public void PushMessage(string str)
    {
        messages.Push(str);
    }

    void DisplayNewMessage(string str)
    {
        old3.DOText(old2.text, 0.2f);
        old2.DOText(old1.text,0.2f);
        old1.DOText(recent,0.2f);
        recent = str;
        firstMessage.DOKill();
        firstMessage.text = "";
        firstMessage.DOText(str, 1.2f);
        timer = 1.75f;
    }
}
