using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CommandManager
{
    private GameObject _commandList;
    private Vector3 originalPosition;

    public CommandManager(GameObject commandList)
    {
        _commandList = commandList;
        originalPosition = _commandList.transform.position;
    }
    
    public void Execute(string command)
    {
        switch (command)
        {
            case TerminalCommand.Clear:
                Clear();
                break;
            default:
                Debug.Log("undefined");
                break;
        }
    }

    private void Clear()
    {
        foreach (Transform child in _commandList.transform)
        {
            if (child.name != "CommandInputLine")
            {
                Object.Destroy(child.gameObject);
            }
        }
        _commandList.transform.position = originalPosition;
    }
}
