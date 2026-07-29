using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace SimpleAudioSystem.Editor
{
    [CustomEditor(typeof(AudioDatabase))]
    public class AudioDatabaseEditor : UnityEditor.Editor
    {
        private SerializedProperty entriesProperty;

        private void OnEnable()
        {
            entriesProperty = serializedObject.FindProperty("entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(
                "Entry Count",
                entriesProperty.arraySize.ToString());

            EditorGUILayout.HelpBox(
                "Use Music entries with PlayMusic and SFX entries with PlayOneShot.",
                MessageType.Info);

            if (GUILayout.Button("Add Entry"))
            {
                AddEntry();
                return;
            }

            EditorGUILayout.Space();

            HashSet<string> duplicateIds = FindDuplicateIds();

            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                SerializedProperty entryProperty =
                    entriesProperty.GetArrayElementAtIndex(i);
                SerializedProperty idProperty =
                    entryProperty.FindPropertyRelative("id");
                SerializedProperty categoryProperty =
                    entryProperty.FindPropertyRelative("category");
                SerializedProperty eventProperty =
                    entryProperty.FindPropertyRelative("eventReference");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"Entry {i + 1}", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(idProperty, new GUIContent("ID"));
                EditorGUILayout.PropertyField(
                    categoryProperty,
                    new GUIContent("Category"));
                EditorGUILayout.PropertyField(
                    eventProperty,
                    new GUIContent("FMOD Event"));

                if (string.IsNullOrWhiteSpace(idProperty.stringValue))
                {
                    EditorGUILayout.HelpBox("ID cannot be blank.", MessageType.Error);
                }
                else if (duplicateIds.Contains(idProperty.stringValue))
                {
                    EditorGUILayout.HelpBox(
                        $"Duplicate ID '{idProperty.stringValue}'. IDs are case-sensitive.",
                        MessageType.Error);
                }

                EventReference eventReference =
                    (EventReference)eventProperty.boxedValue;
                if (eventReference.IsNull)
                {
                    EditorGUILayout.HelpBox(
                        "An FMOD event must be assigned.",
                        MessageType.Error);
                }

                if (GUILayout.Button("Remove"))
                {
                    RemoveEntry(i);
                    return;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddEntry()
        {
            Undo.RecordObject(target, "Add Audio Entry");

            string uniqueId = GetUniqueDefaultId();
            int newIndex = entriesProperty.arraySize;
            entriesProperty.InsertArrayElementAtIndex(newIndex);

            SerializedProperty newEntry =
                entriesProperty.GetArrayElementAtIndex(newIndex);
            newEntry.FindPropertyRelative("id").stringValue = uniqueId;
            newEntry.FindPropertyRelative("category").enumValueIndex =
                (int)AudioCategory.SFX;
            newEntry.FindPropertyRelative("eventReference").boxedValue =
                default(EventReference);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void RemoveEntry(int index)
        {
            Undo.RecordObject(target, "Remove Audio Entry");
            entriesProperty.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            GUIUtility.ExitGUI();
        }

        private string GetUniqueDefaultId()
        {
            const string baseId = "new.audio.entry";

            if (!ContainsId(baseId))
            {
                return baseId;
            }

            int suffix = 2;
            while (ContainsId($"{baseId}.{suffix}"))
            {
                suffix++;
            }

            return $"{baseId}.{suffix}";
        }

        private bool ContainsId(string id)
        {
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                SerializedProperty entry =
                    entriesProperty.GetArrayElementAtIndex(i);
                SerializedProperty idProperty =
                    entry.FindPropertyRelative("id");

                if (string.Equals(
                    idProperty.stringValue,
                    id,
                    StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private HashSet<string> FindDuplicateIds()
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> duplicates =
                new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                SerializedProperty entry =
                    entriesProperty.GetArrayElementAtIndex(i);
                string id =
                    entry.FindPropertyRelative("id").stringValue;

                if (!string.IsNullOrWhiteSpace(id) && !seen.Add(id))
                {
                    duplicates.Add(id);
                }
            }

            return duplicates;
        }
    }
}
