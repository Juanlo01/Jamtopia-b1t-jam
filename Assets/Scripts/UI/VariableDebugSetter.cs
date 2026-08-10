using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

// Attach to the same GameObject as DialogueRunner (the "Dialogue System Variant" prefab's root).
// Toggle with the backquote key; lets you punch in any variable name/value/type and push it
// straight into VariableStorage, same as any other SetValue caller (PlayerController3D, etc).
[RequireComponent(typeof(DialogueRunner))]
public class VariableDebugSetter : MonoBehaviour
{
    [SerializeField] Key toggleKey = Key.Backquote;

    private enum ValueType { Bool, String, Float }

    private DialogueRunner dialogueRunner;
    private bool isOpen;
    private string nameInput = "$";
    private string valueInput = "";
    private ValueType selectedType = ValueType.Bool;

    private void Awake()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
    }

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            isOpen = !isOpen;
        }
#endif
    }

    private void OnGUI()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!isOpen)
        {
            return;
        }

        GUILayout.BeginArea(new Rect(10, 10, 320, 150), GUI.skin.box);
        GUILayout.Label("Yarn Variable Debug Setter");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(50));
        nameInput = GUILayout.TextField(nameInput);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Value", GUILayout.Width(50));
        valueInput = GUILayout.TextField(valueInput);
        GUILayout.EndHorizontal();

        selectedType = (ValueType)GUILayout.SelectionGrid((int)selectedType, new[] { "Bool", "String", "Float" }, 3);

        if (GUILayout.Button("Set"))
        {
            SetVariable();
        }

        GUILayout.EndArea();
#endif
    }

    private void SetVariable()
    {
        if (dialogueRunner == null || string.IsNullOrWhiteSpace(nameInput))
        {
            return;
        }

        string variableName = nameInput.StartsWith("$") ? nameInput : $"${nameInput}";

        switch (selectedType)
        {
            case ValueType.Bool:
                if (bool.TryParse(valueInput, out bool boolValue))
                {
                    dialogueRunner.VariableStorage.SetValue(variableName, boolValue);
                }
                else
                {
                    Debug.LogWarning($"{nameof(VariableDebugSetter)}: \"{valueInput}\" is not a valid bool (use true/false).", this);
                }
                break;

            case ValueType.String:
                dialogueRunner.VariableStorage.SetValue(variableName, valueInput);
                break;

            case ValueType.Float:
                if (float.TryParse(valueInput, out float floatValue))
                {
                    dialogueRunner.VariableStorage.SetValue(variableName, floatValue);
                }
                else
                {
                    Debug.LogWarning($"{nameof(VariableDebugSetter)}: \"{valueInput}\" is not a valid float.", this);
                }
                break;
        }
    }
}
