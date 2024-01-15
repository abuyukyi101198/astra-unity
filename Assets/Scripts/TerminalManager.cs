using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public InputField terminalInput;
    public GameObject userInputLine;
    public GameObject commandList;

    private string _command;
    private float _lastReturnTime;
    private int _copyCommandIndex;

    private CommandManager _commandManager;

    private void Start()
    {
        _commandManager = new CommandManager(commandList);
        _lastReturnTime = Time.time;
        _copyCommandIndex = 1;
        FocusOnInputField();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && Event.current.type == EventType.KeyDown && GetCoolDown()) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                _command = terminalInput.text;

                ClearInputField();
                AddDirectoryLine();
                
                _commandManager.Execute(_command);
                
                FocusOnInputField();

                userInputLine.transform.SetAsLastSibling();

                _lastReturnTime = Time.time;
                _copyCommandIndex = 1;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CopyCommand(true);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CopyCommand(false);
            }
        }
    }

    private void FocusOnInputField() {
        terminalInput.ActivateInputField();
        terminalInput.Select();
    }

    private void ClearInputField() {
        terminalInput.text = "";
    }

    private void AddDirectoryLine() {
        Vector2 commandListSize = GetSizeDelta(commandList);
        GameObject msg = Instantiate(directoryLine, commandList.transform);

        msg.transform.SetSiblingIndex(commandList.transform.childCount - 1);

        GetCommandText(msg.transform).text = _command;

        Vector2 newCommandListSize = new Vector2(
            commandListSize.x, commandListSize.y + GetSizeDelta(directoryLine).y);
        Vector2 canvasSize = GetSizeDelta(commandList.transform.parent.transform.parent);

        if (newCommandListSize.y > canvasSize.y + 300f) {
            Destroy(commandList.transform.GetChild(0).gameObject);
        }
        else {
            commandList.GetComponent<RectTransform>().sizeDelta = newCommandListSize;
            
            if (commandList.transform.position.y > 36)
            {
                Vector3 position = commandList.transform.position;
                position = new Vector3(
                    position.x, 
                    position.y - GetSizeDelta(directoryLine).y, 
                    position.z
                    );
                commandList.transform.position = position;
            }
        }
    }

    private void CopyCommand(bool isUp)
    {
        if (_copyCommandIndex == 1)
        {
            _command = terminalInput.text;
        }
        
        switch (isUp)
        {
            case true when _copyCommandIndex != commandList.transform.childCount:
                _copyCommandIndex++;
                break;
            case false when _copyCommandIndex != 1:
                _copyCommandIndex--;
                break;
        }

        terminalInput.text = _copyCommandIndex == 1 ? 
            _command : GetCommandText(commandList.transform.GetChild(commandList.transform.childCount - _copyCommandIndex)).text;

        StartCoroutine(SetCaretPosition());
    }
    
    #region Utilities
    private Vector2 GetSizeDelta(Transform obj)
    {
        return obj.GetComponent<RectTransform>().sizeDelta;
    }
    
    private Vector2 GetSizeDelta(GameObject obj)
    {
        return GetSizeDelta(obj.transform);
    }

    private Text GetCommandText(Transform obj)
    {
        return obj.GetComponentsInChildren<Text>()[1];
    }
    
    private IEnumerator SetCaretPosition()
    {
        yield return new WaitForEndOfFrame();
        terminalInput.MoveTextEnd(false);
    }

    private bool GetCoolDown() {
        return Time.time - _lastReturnTime > 0.1;
    }
    #endregion
}
