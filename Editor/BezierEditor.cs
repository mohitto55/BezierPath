using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Bezier), true), CanEditMultipleObjects]
public class BezierEditor : Editor
{
    Bezier Script;
    private void OnSceneGUI()
    {
        if(Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector2 MousePoint = ray.origin;
            BezierPoint point = new BezierPoint(MousePoint, MousePoint + Vector2.up);
            if (Script.Points.Count > 1)
            {
                BezierPoint LastPoint = Script.Points[Script.Points.Count - 1];
                Vector2 dir = LastPoint.Point - LastPoint.ControlPoints[0];
                LastPoint.ControlPoints.Add(LastPoint.Point + dir);
            }
            Script.Points.Add(point);
            BezierPoint NewPoint = Script.Points[Script.Points.Count - 1];
        }
        if(Script.Points.Count > 0)
        {
            for (int i = 0; i < Script.Points.Count; i++)
            {
                Script.Points[i].Point = Handles.FreeMoveHandle(Script.Points[i].Point, Quaternion.identity, 0.4f, Vector2.one, Handles.CircleHandleCap);
                for (int j = 0; j < Script.Points[i].ControlPoints.Count; j++)
                {
                    Script.Points[i].ControlPoints[j] = Handles.FreeMoveHandle(Script.Points[i].ControlPoints[j], Quaternion.identity, 0.2f, Vector2.one, Handles.CubeHandleCap);
                    Vector2 Pos = Script.Points[i].ControlPoints[j];
                    Handles.DrawLine(Script.Points[i].Point, Script.Points[i].ControlPoints[j]);
                    if(Script.Points[i].ControlPoints.Count > 1)
                    {
                        int a;
                        if (j == 0)
                            a = 1;
                        else
                            a = 0;
                        Vector2 Dir = (Script.Points[i].Point - Script.Points[i].ControlPoints[j]).normalized;
                        float Distance = Vector2.Distance(Script.Points[i].Point, Script.Points[i].ControlPoints[a]);
                        Script.Points[i].ControlPoints[a] = Handles.FreeMoveHandle(Script.Points[i].Point + Dir * Distance, Quaternion.identity, 0.2f, Vector2.one, Handles.CubeHandleCap);
                    }
                }
                if (i != 0)
                {
                    //Handles.DrawLine(Script.Points[i - 1].Point, Script.Points[i].Point);
                    Handles.DrawBezier(Script.Points[i - 1].Point, Script.Points[i].Point, 
                    Script.Points[i - 1].ControlPoints[Script.Points[i - 1].ControlPoints.Count - 1],
                    Script.Points[i].ControlPoints[0], Color.green, null, 2);
                }
            }
        }
    }
    int DeletePoint = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DeletePoint = EditorGUILayout.IntSlider(DeletePoint, 0 , Script.Points.Count - 1);
        if (GUILayout.Button("선택 라인 삭제"))
        {
            Script.Points.RemoveAt(DeletePoint);
            if (Script.Points.Count > 0)
            {
                if (Script.Points[0].ControlPoints.Count > 1)
                    Script.Points[0].ControlPoints.RemoveAt(1);
                if(Script.Points[Script.Points.Count - 1].ControlPoints.Count > 1)
                Script.Points[Script.Points.Count - 1].ControlPoints.RemoveAt(1);
            }
        }
    }
    private void OnEnable()
    {
        Script = target as Bezier;
    }
}
