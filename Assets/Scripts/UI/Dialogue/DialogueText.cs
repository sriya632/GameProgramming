using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueText : MonoBehaviour
{
    public TMP_Text nameBox;
    public TMP_Text textBox;

    private TextAsset file;
    private List<string> lines;
    private float textSpeed;
    private int index;

    void Start()
    {
        file = gameObject.transform.parent.gameObject.GetComponent<Dialogue>().file;
        lines = new List<string>();
        lines.AddRange(file.text.Split(Environment.NewLine));

        index = 0;
        nameBox.text = lines[0];

        textSpeed = 2;

        NextLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
        {
            if (textBox.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textBox.text = lines[index];
            }
        }
    }

    void NextLine()
    {
        if (index < lines.Count - 1)
        {
            index++;
            textBox.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Clock.Resume();
            UI.Toggle(true);
            Player.Toggle(true);

            Dialogue.isActive = false;
            Destroy(GameObject.FindWithTag("Dialogue"));
        }
    }

    IEnumerator TypeLine()
    {
        //type character 1 by 1
        foreach (char c in lines[index].ToCharArray())
        {
            textBox.text += c;
            yield return new WaitForSeconds(textSpeed * 0.01f);
        }
    }
}
