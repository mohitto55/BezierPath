using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using Bezier;

public class BezierPath : MonoBehaviour
{
    [SerializeField] Transform body = null;

    public bool EditLine = false;
    public float NodeSize = 0.5f;
    public Path Nodes;
    public float Speed;
    List<Vector2> Paths = new List<Vector2>();

    private void Start() {
        SearchPath();
        StartCoroutine(MoveBody(body));
    }
    void SearchPath() {
        Vector2[] NodesVec = Nodes.CalculateEvenlySpacedPoints(.1f, 1);
        foreach (Vector2 path in NodesVec)
            Paths.Add(path);
    }
    IEnumerator MoveBody(Transform body) {
        body.transform.position = Paths[0];
        for (int i = 0; i < Paths.Count; i++) {
                Vector2 nextNode = Paths[i];
                while (true) {
                yield return null;
                body.position = Vector2.MoveTowards(body.position, nextNode, Speed * Time.deltaTime);
                Vector2 dirvec = (nextNode - (Vector2)body.position).normalized;
                if ((Vector2)body.position == nextNode)
                        break;
                float dir = Mathf.Atan2(dirvec.y, dirvec.x) * Mathf.Rad2Deg;
                //body.rotation = Quaternion.Euler(0, 0, dir);
                float dst = Vector2.Distance(nextNode, (Vector2)body.position);
                body.DOKill();
                body.DORotate(new Vector3(0, 0, dir), dst/Speed);
            }

            
        }
    }

    private void OnDrawGizmos() {
        if(Paths != null)
        for (int i = 0; i < Paths.Count; i++) {
            Gizmos.DrawSphere(Paths[i], 0.2f);
        }
    }


   
    private void OnDisable() {
        body.DOKill();
    }
}