using UnityEditor;
using UnityEngine;

// Adds a named dropdown (instead of a raw array index) for picking CameraFocusDebug's selected
// focus point, plus a button to preview it immediately without entering Play mode.
[CustomEditor(typeof(CameraFocusDebug))]
public class CameraFocusDebugEditor : Editor
{
    private SerializedProperty focusPointsProperty;
    private SerializedProperty selectedFocusIndexProperty;

    private void OnEnable(){
        focusPointsProperty = serializedObject.FindProperty("focusPoints");
        selectedFocusIndexProperty = serializedObject.FindProperty("selectedFocusIndex");
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();

        EditorGUILayout.PropertyField(focusPointsProperty, new GUIContent("Focus Points"), true);

        int count = focusPointsProperty.arraySize;
        if(count == 0){
            EditorGUILayout.HelpBox("Add at least one focus point above to select from.", MessageType.Info);
        }
        else{
            string[] options = new string[count];
            for(int i = 0; i < count; i++){
                Transform focus = focusPointsProperty.GetArrayElementAtIndex(i).objectReferenceValue as Transform;
                options[i] = focus != null ? $"{i}: {focus.name}" : $"{i}: (empty)";
            }

            int clampedIndex = Mathf.Clamp(selectedFocusIndexProperty.intValue, 0, count - 1);
            selectedFocusIndexProperty.intValue = EditorGUILayout.Popup("Selected Focus", clampedIndex, options);
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUI.BeginDisabledGroup(count == 0);
        if(GUILayout.Button("Preview Selected Focus")){
            CameraFocusDebug debug = (CameraFocusDebug)target;
            Undo.RecordObject(debug.transform, "Preview Camera Focus");
            debug.ApplySelectedFocus();
        }
        EditorGUI.EndDisabledGroup();
    }
}
