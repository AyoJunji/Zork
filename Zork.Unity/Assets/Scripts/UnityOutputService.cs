using Zork.Common;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnityOutputService : MonoBehaviour, IOutputService
{
    [SerializeField]
    private TextMeshProUGUI TextLinePrefab;

    [SerializeField]
    private Image NewLinePrefab;
    
    [SerializeField]
    private Transform ContentTransform;

    [SerializeField]
    [Range(0, 100)]
    private int maxEntries;
   
    public void Write(object obj)
    {
        ParseWriteLine(obj.ToString());
    }

    public void Write(string message)
    {
        ParseWriteLine(message);
    }

    public void WriteLine(object obj)
    {
        ParseWriteLine(obj.ToString());
    }

    public void WriteLine(string message)
    {
        ParseWriteLine(message);
    }

    private void ParseWriteLine(string message)
    {
        if (_entries.Count >= maxEntries)
        {
            _entries.Dequeue();
        }

        char separator = '\n';
        string[] lineTokens = message.Split(separator);
        string firstLine;
        string secondLine;
        if (lineTokens.Length == 1)
        {
            firstLine = lineTokens[0];
            var textLine = Instantiate(TextLinePrefab, ContentTransform);
            textLine.text = firstLine;
            _entries.Enqueue(textLine.gameObject);
        }
        else
        {
            firstLine = lineTokens[0];
            if(string.IsNullOrWhiteSpace(firstLine))
            {
                var newLine = Instantiate(NewLinePrefab, ContentTransform);
                _entries.Enqueue(newLine.gameObject);
            }

            else
            {
                var firstTextLine = Instantiate(TextLinePrefab, ContentTransform);
                firstTextLine.text = firstLine;
                _entries.Enqueue(firstTextLine.gameObject);
            }

            secondLine = lineTokens[1];
            if (string.IsNullOrWhiteSpace(secondLine))
            {
                var newLine = Instantiate(NewLinePrefab, ContentTransform);
                _entries.Enqueue(newLine.gameObject);
            }
            else
            {
                var secondTextLine = Instantiate(TextLinePrefab, ContentTransform);
                secondTextLine.text = secondLine;
                _entries.Enqueue(secondTextLine.gameObject);
            }

        }
    }

    private Queue<GameObject> _entries = new Queue<GameObject>();
}
