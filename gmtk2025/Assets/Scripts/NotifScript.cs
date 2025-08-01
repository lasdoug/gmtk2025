using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class NotifScript : MonoBehaviour
{
    public TMP_Text firstMessage, old1, old2, old3;
    string recent = "";
    Queue messages = new Queue();
    float timer = 0f;
    float maxWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstMessage.text = "...";
        maxWidth = firstMessage.rectTransform.rect.width;
        old1.text = "";
        old2.text = "";
        old3.text = "";
        QueueMessage("Testing this thing");
        QueueMessage("Another message");
        QueueMessage("Lets do a long one to see what happens how long can we go");
        QueueMessage("One more thanks");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0) timer -= Time.deltaTime;

        if (messages.Count > 0)
        {
            if (timer <= 0)
            {
                DisplayNewMessage((string)messages.Dequeue());
            }
        }
    }

    public void QueueMessage(string str)
    {
        if (firstMessage.GetPreferredValues(str).x <= maxWidth)
        {
            messages.Enqueue(str);
            return; 
        }
        string[] tokens = str.Split(' ');
        string segment = "";

        foreach (string t in tokens)
        {
            if (segment == "")
            {
                segment = t;
                continue;
            }
            if (firstMessage.GetPreferredValues(segment + " " + t).x > maxWidth)
            {
                messages.Enqueue(segment);
                segment = t;
            }
            else
            {
                segment = segment + " " + t;
            }
        }
        messages.Enqueue(segment);
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
