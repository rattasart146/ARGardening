﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sebastian.Geometry;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARShapeBuilder : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject parentObject;
    public GameObject markerPointIndicator;
    public Camera arCamera;

    private GameObject spawn;
    private LineRenderer lineRender;
    private int Index = 0;
    List<Shape> shapes = new List<Shape>();
    private List<GameObject> areaCal = new List<GameObject>();
    bool shapeChangedSinceLastRepaint;

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    Vector2 touchPosition;
    private ARRaycastManager _arRaycastManager;
    private Ray mpIndicatorRay;
    private RaycastHit mpIndicatorHit;
    private Quaternion markerPointRotaion;
    private Pose markerPointPose;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    Text calAreaText;
    // Start is called before the first frame update
    private void Awake()
    {
        calAreaText = GameObject.Find("CalAreaText").GetComponent<Text>();
        _arRaycastManager = GetComponent<ARRaycastManager>();
        lineRender = GetComponent<LineRenderer>();
        Debug.Log("Index :" + shapes.Count);
        CreateNewShape();
        markerPointIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMarkerPointIndicator();
        //calAreaText.text = "areaCal length :" + areaCal.Count;
        if (areaCal.Count > 2)
        {
            SuperficieIrregularPolygon();
        }
    }

    private void UpdateMarkerPointIndicator()
    {
        var s_Hits = new List<ARRaycastHit>();

        mpIndicatorRay = arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var cameraForward = arCamera.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;


        if (_arRaycastManager.Raycast(mpIndicatorRay, s_Hits, TrackableType.Planes))
        {
            var hitPose = s_Hits[0].pose;

            markerPointPose = hitPose;
            markerPointRotaion = Quaternion.LookRotation(cameraBearing);

            markerPointIndicator.SetActive(true);
            markerPointIndicator.transform.SetPositionAndRotation(markerPointPose.position, markerPointRotaion);
        }
    }

    public void Draw()
    {
        var hit = markerPointPose;
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
        areaCal.Add(markedClone);
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


    private void SuperficieIrregularPolygon() //Calculate Area
    {
        float temp = 0;
        for (int i = 0; i < areaCal.Count; i++)
        {
            Debug.Log($"Pos Point {i} : {areaCal[i].transform.position.x} ,{areaCal[i].transform.position.z}");
            if (i != areaCal.Count - 1)
            {
                float mulA = areaCal[i].transform.position.x * areaCal[i + 1].transform.position.z;
                float mulB = areaCal[i + 1].transform.position.x * areaCal[i].transform.position.z;
                temp += (mulA - mulB);
            }
            else
            {
                float mulA = areaCal[i].transform.position.x * areaCal[0].transform.position.z;
                float mulB = areaCal[0].transform.position.x * areaCal[i].transform.position.z;
                temp += (mulA - mulB);
            }
        }
        temp *= 0.5f;
        temp = Mathf.Abs(temp);
        calAreaText.text = ("Area : " + temp);

    }

    public void TogglePlanDetection()
    {

    }

}
