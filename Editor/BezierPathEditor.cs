using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Bezier;
[CustomEditor(typeof(BezierPath), true), CanEditMultipleObjects]
public class BezierPathEditor : Editor {
    public BezierPath bezierPath;
    private void OnSceneGUI() {
        if (bezierPath == null)
            bezierPath = target as BezierPath;
        Path path = bezierPath.Nodes;
        if (path != null) {
            for (int i = 0; i < path.NumSegments; i++) {
                int k = i * 3;
                Handles.DrawBezier(path[k], path[k + 3], path[k + 1], path[k + 2], Color.green, null, 2);
            }
            if (bezierPath.EditLine) {
                float nodeSize = bezierPath.NodeSize;
                for (int i = 0; i < path.NumSegments; i++) {
                    int k = i * 3;
                    Handles.color = Color.red;
                    Vector2 movePos = Handles.FreeMoveHandle(path[k], Quaternion.identity, nodeSize, Vector2.zero, Handles.CylinderHandleCap);
                    path.MovePos(k, movePos);

                    Handles.color = Color.white;
                    movePos = Handles.FreeMoveHandle(path[k + 1], Quaternion.identity, nodeSize, Vector2.zero, Handles.CylinderHandleCap);
                    path.MovePos(k + 1, movePos);
                    Handles.DrawLine(path[k], path[k + 1]);

                    movePos = Handles.FreeMoveHandle(path[k + 2], Quaternion.identity, nodeSize, Vector2.zero, Handles.CylinderHandleCap);
                    path.MovePos(k + 2, movePos);
                    Handles.DrawLine(path[k + 2], path[k + 3]);
                    if (i == path.NumSegments - 1) {
                        Handles.color = Color.blue;
                        movePos = Handles.FreeMoveHandle(path[k + 3], Quaternion.identity, nodeSize, Vector2.zero, Handles.CylinderHandleCap);
                        path.MovePos(k + 3, movePos);
                    }
                }
                Event guiEvent = Event.current;
                if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.shift) {
                    for (int i = 0; i < bezierPath.Nodes.NumSegments + 1; i++) {
                        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
                        float distance = Vector2.Distance(mousePos, bezierPath.Nodes[i * 3]);
                        //Debug.Log(guiEvent.mousePosition);
                        Debug.Log(i + "번째 노드 " + distance);
                        if (distance <= nodeSize) {
                            bezierPath.Nodes.DeleteNode(i);
                        }
                    }
                    Debug.Log("우클릭");
                }
            }
        }
        if (body != null && !Application.isPlaying) {
            body.transform.position = path[0];
        }
        EditorUtility.SetDirty(bezierPath);
    }
    Transform body;
    private void OnEnable() {
        BezierPath bezierPath = target as BezierPath;
        var field = typeof(BezierPath).GetField("body", BindingFlags.NonPublic | BindingFlags.Instance);
        var data = (Transform)field.GetValue(bezierPath);
        body = data;
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (bezierPath == null)
            bezierPath = target as BezierPath;
        EditorGUILayout.HelpBox("파란 점 - 도착지점\n빨간 노드에 쉬프트 + 우클릭하면 노드 삭제", MessageType.Info);
        if (GUILayout.Button("Add Node")) {
            if (bezierPath.Nodes == null || bezierPath.Nodes.NumPoints == 0)
                bezierPath.Nodes = new Path(new Vector2(1, 1));
            Vector2 spawnPos = new Vector2(bezierPath.Nodes[bezierPath.Nodes.NumPoints - 1].x + 1, bezierPath.Nodes[bezierPath.Nodes.NumPoints - 1].y + 1);
            bezierPath.Nodes.AddSegment(new Vector2(spawnPos.x, spawnPos.y));
        }
    }
}