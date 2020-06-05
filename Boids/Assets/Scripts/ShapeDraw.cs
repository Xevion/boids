﻿using UnityEngine;

/// <summary>
/// A simple static utility class that assists with drawing shapes using the <c>LineRenderer</c> class.
/// </summary>
public class ShapeDraw {
    public static float CircleWidth = 1f;
    public static float ArcWidth = 1f;
    public static int CircleVertexCount = 360;
    public static int ArcVertexCount = 77;
    
    
    /// <summary>
    /// Draw a Arc aimed straight up with a certain angle width and radius.
    /// Use <see cref="RotateLineRenderer"/> to point the Arc at a certain direction.
    /// </summary>
    /// <param name="lineRenderer">The LineRenderer to draw the Arc upon.</param>
    /// <param name="angle">Angle of the Arc</param>
    /// <param name="radius">Radius of the Arc</param>
    /// <seealso cref="RotateLineRenderer"/>
    public static void DrawArc(LineRenderer lineRenderer, float angle, float radius) {
        // Setup LineRenderer properties
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = ArcWidth;
        lineRenderer.endWidth = ArcWidth;
        lineRenderer.positionCount = ArcVertexCount + 1 + 2;

        // Calculate points for circle
        var pointCount = ArcVertexCount + 1;
        var points = new Vector3[pointCount + 2];
        
        for (int i = 0; i < pointCount; i++) {
            // Magic '180 - angle'
            var rad = Mathf.Deg2Rad * Mathf.LerpAngle(0, angle, i / (float) pointCount);
            points[i + 1] = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0) * radius;
        }

        points[0] = new Vector3(0, 0, 0);
        points[points.Length - 1] = points[0];

        // Add points to LineRenderer
        lineRenderer.SetPositions(points);
    }

    /// <summary>
    /// Draw a Circle with a specific radius and number of vertexes (detail level)
    /// </summary>
    /// <param name="lineRenderer"></param>
    public static void DrawCircle(LineRenderer lineRenderer) {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lineRenderer"></param>
    public static void RotateLineRenderer(LineRenderer lineRenderer) {
        
    }
}