using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoidsController))]
[CanEditMultipleObjects]

public class BoidsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        BoidsController controller = (BoidsController)target;
        if (GUILayout.Button("Lazy Flight"))
        {
            controller.UpdateTarget(0);
        }
        if (GUILayout.Button("Circle Tree"))
        {
            controller.UpdateTarget(1);
        }
        if (GUILayout.Button("Follow Leader"))
        {
            controller.UpdateTarget(2);
        }
    }
}
