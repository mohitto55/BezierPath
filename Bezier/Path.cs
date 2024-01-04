using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path {
    [SerializeField]
    List<Vector2> Points;

    public Path(Vector2 centre) {
        Points = new List<Vector2> {
            centre + Vector2.left,
            centre + (Vector2.left + Vector2.up) * 0.5f,
            centre + (Vector2.right + Vector2.down) * 0.5f,
            centre + Vector2.right
        };
    }
    public Vector2 this[int i] {
        get { return Points[i]; }
    }
    public int NumPoints {
        get { return Points.Count; }
    }
    public int NumSegments {
        get { return (Points.Count - 4) / 3 + 1; }
    }
    public void AddSegment(Vector2 anchorPos) {
        Debug.Log("길이 " + Points.Count);
        Points.Add(Points[Points.Count - 1] * 2 - Points[Points.Count - 2]);
        Points.Add((Points[Points.Count - 1] + anchorPos) * 0.5f);
        Points.Add(anchorPos);
    }
    public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1) {
        List<Vector2> evenlySpacedPoints = new List<Vector2>();
        evenlySpacedPoints.Add(Points[0]);
        Vector2 previousPoint = Points[0];
        float dstSinceLastEvenPoint = 0;

        for (int segmentIndex = 0; segmentIndex < NumSegments; segmentIndex++) {
            Vector2[] p = GetSegment(segmentIndex);
            float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength / 2f;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            while (t <= 1) {
                t += 1f / divisions;
                Vector2 pointOnCurve = Bezier.BezierCurve.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                dstSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                while (dstSinceLastEvenPoint >= spacing) {
                    float overshootDst = dstSinceLastEvenPoint - spacing;
                    Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    dstSinceLastEvenPoint = overshootDst;
                    previousPoint = newEvenlySpacedPoint;
                }

                previousPoint = pointOnCurve;
            }
        }

        return evenlySpacedPoints.ToArray();
    }
    public Vector2[] GetSegment(int i) {
        return new Vector2[] { Points[i * 3], Points[i * 3 + 1], 
            Points[i * 3 + 2], Points[i * 3 + 3] }; 
    }
    public void MovePos(int i, Vector2 pos) {
        Vector2 deltaPos = pos - Points[i];
        Points[i] = pos;
        if(i % 3 == 0) {
            if(i + 1 < Points.Count) {
                Points[i + 1] += deltaPos;
            }
            if(i - 1 >= 0) {
                Points[i - 1] += deltaPos;
            }
        } else if(i > 1 && i < Points.Count - 2){
            int crossControlIndex = i % 3 == 1 ? i - 2 : i + 2;
            crossControlIndex = i % 3 == 2 ? i + 2 : crossControlIndex;
            int anchorIndex = i % 3 == 1 ? i - 1 : i + 1;
            float ControlDst = Vector2.Distance(Points[anchorIndex] , Points[crossControlIndex]);
            Vector2 ControlDir = (Points[anchorIndex] - Points[i]).normalized;

            Vector2 ControlVec = Points[anchorIndex] + ControlDir * ControlDst;
            Points[crossControlIndex] = ControlVec;
        }
    }
    public void DeleteSegment(int anchorIndex) {
    }
    public void DeleteNode(int anchorIndex) {
        if(anchorIndex == 0) {
            Points.RemoveRange(0, 3);
        }
        else
        if (anchorIndex == NumSegments) {
            Points.RemoveRange(Points.Count - 1 - 2, 3);
        }
        else
            if(anchorIndex > 0 && anchorIndex < NumSegments) {
            Points.RemoveRange(anchorIndex * 3 - 1, 3);
        }
    }
    //public void MoveAnco(int i)
}


