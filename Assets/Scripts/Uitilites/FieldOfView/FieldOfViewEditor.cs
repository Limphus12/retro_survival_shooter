using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace com.limphus.utilities
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target; 
            
            if (!fov.eyes) return; Vector3 fovPos = fov.eyes.position;

            Vector3 viewAngleA = fov.DirectionFromAngle(-fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirectionFromAngle(fov.viewAngle / 2, false);

            Handles.color = Color.red;
            Handles.DrawWireArc(fov.transform.position + Vector3.up, Vector3.up, Vector3.forward, 360, fov.minViewRadius);

            Handles.color = Color.white;
            Handles.DrawWireArc(fovPos, Vector3.up, viewAngleA, fov.viewAngle, fov.viewRadius);

            if (fov.viewAngle != 0) Handles.color = Color.green;
            else Handles.color = Color.yellow;

            Handles.DrawLine(fovPos, fovPos + viewAngleA * fov.viewRadius);
            Handles.DrawLine(fovPos, fovPos + viewAngleB * fov.viewRadius);

            Handles.color = new Color(255, 128, 0);

            if (fov.VisibleTargets == null) return;

            if (fov.VisibleTargets.Count > 0)
            {
                foreach (Transform visibleTarget in fov.VisibleTargets)
                {
                    Handles.DrawLine(fovPos, visibleTarget.position);
                }
            }
        }
    }
}