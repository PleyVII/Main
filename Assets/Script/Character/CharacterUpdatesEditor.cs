using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterUpdates))]
public class CharacterUpdatesEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI(); // Draw the default inspector

        CharacterUpdates script = (CharacterUpdates)target;
        if (script.allInfo == null) {
            EditorGUILayout.HelpBox("objectInformation must be assigned.", MessageType.Warning);
        }
    }
}
