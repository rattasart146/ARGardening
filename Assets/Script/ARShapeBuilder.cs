using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARShapeBuilder : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject parentObject;

    private GameObject spawn;
    private LineRenderer lineRender;
    private int Index = 0;
    List<Shape> shapes = new List<Shape>();
    bool shapeChangedSinceLastRepaint;

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    Vector2 touchPosition;
    private ARRaycastManager _arRaycastManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        lineRender = GetComponent<LineRenderer>();
        Debug.Log("Index :" + shapes.Count);
        CreateNewShape();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        var s_Hits = new List<ARRaycastHit>();
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (_arRaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinBounds))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            Draw(hitPose);

        }
    }

    private void Draw(Pose hit)
    {
        Shape shapeToDraw = shapes[0];

        Index = shapeToDraw.points.Count;
        if (Index == 0)
        {
            shapeToDraw.points.Add(hit.position);
            Debug.Log("V3 point :" + hit.position.ToString("F4"));
        }
        else
        {
            shapeToDraw.points.Add(new Vector3(hit.position.x, shapeToDraw.points[0].y, hit.position.z));
            Debug.Log("V3 point :" + new Vector3(hit.position.x, shapeToDraw.points[0].y, hit.position.z));
        }

        GameObject markedClone = Instantiate(originalObject, shapeToDraw.points[Index], Quaternion.identity);
        markedClone.transform.parent = parentObject.transform;
        markedClone.name = "MarkedPoint" + Index;

        lineRender.positionCount = Index + 1;
        lineRender.SetPosition(Index, shapeToDraw.points[Index]);

        Debug.Log("Index :" + Index);

        UpdateMeshDisplay();
    }

    void CreateNewShape()
    {
        shapes.Add(new Shape());
    }

    private void OnEnable()
    {
        shapeChangedSinceLastRepaint = true;
    }

    public void UpdateMeshDisplay()
    {
        CompositeShape compShape = new CompositeShape(shapes, shapes[0].points[0].y);
        meshFilter.mesh = compShape.GetMesh();
        meshCollider.sharedMesh = compShape.GetMesh();
    }

    public void TogglePlanDetection()
    {

    }

}
