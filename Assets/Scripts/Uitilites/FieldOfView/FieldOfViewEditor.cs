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
            FieldOfView fov = (FieldOfView)target; Vector3 fowPos = fov.transform.position + Vector3.up;

            Handles.color = Color.white;
            Vector3 viewAngleA = fov.DirectionFromAngle(-fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirectionFromAngle(fov.viewAngle / 2, false);
            Handles.DrawWireArc(fowPos, Vector3.up, viewAngleA, fov.viewAngle, fov.viewRadius);

            if (fov.viewAngle != 0)
            {
                Handles.color = Color.green;
            }

            else Handles.color = Color.yellow;

            Handles.DrawLine(fowPos, fowPos + viewAngleA * fov.viewRadius);
            Handles.DrawLine(fowPos, fowPos + viewAngleB * fov.viewRadius);

            Handles.color = new Color(255, 128, 0);
            foreach (Transform visibleTarget in fov.visibleTargets)
            {
                if (visibleTarget == fov.closestTarget) continue;

                Handles.DrawLine(fowPos, visibleTarget.position);
            }

            if (fov.closestTarget)
            {
                Handles.color = Color.red;
                Handles.DrawLine(fowPos, fov.closestTarget.position);
            }

        }
    }
}