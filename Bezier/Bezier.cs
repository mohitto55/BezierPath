using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bezier {
    public static class BezierCurve {
        public static Vector2 GetPointOnBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
            Vector3 a = Vector2.Lerp(p0, p1, t);
            Vector3 b = Vector2.Lerp(p1, p2, t);
            Vector3 c = Vector2.Lerp(p2, p3, t);

            Vector3 d = Vector2.Lerp(a, b, t);
            Vector3 e = Vector2.Lerp(b, c, t);

            Vector2 pointOnCurve = Vector2.Lerp(d, e, t);

            return pointOnCurve;
        }
        // 2차
        public static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t) {

            Vector2 p0 = Vector2.Lerp(a, b, t);
            Vector2 p1 = Vector2.Lerp(b, c, t);
            return Vector2.Lerp(p0, p1, t);
        }
        // 3차
        public static Vector2 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t) {
            Vector2 p0 = EvaluateQuadratic(a, b, c, t);
            Vector2 p1 = EvaluateQuadratic(b, c, d, t);
            return Vector2.Lerp(p0, p1, t);
        }
    }
}
