using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;

public class ShapeManager : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject parentObject;

    private LineRenderer lineRender;
    private int Index = 0;
    List<Shape> shapes = new List<Shape>();
    bool shapeChangedSinceLastRepaint;

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;


    Ray myRay;
    RaycastHit hit;

    // Start is called before the first frame update
    private void Awake()
    {
        lineRender = GetComponent<LineRenderer>();
        CreateNewShape();
    }


    // Update is called once per frame
    void Update()
    {
        myRay = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(myRay, out hit, 100))
        {

            if (Input.GetMouseButtonDown(0))
            {
                Draw(hit);

            }
        }
    }

    private void Draw(RaycastHit hit)
    {
        Shape shapeToDraw = shapes[0];

        Index = shapeToDraw.points.Count;
        Debug.Log("Index :" + Index);
        if (Index == 0)
        {
            shapeToDraw.points.Add(hit.point);
        }
        else
        {
            shapeToDraw.points.Add(new Vector3(hit.point.x, shapeToDraw.points[0].y, hit.point.z));
            Debug.Log("V3 point :" + shapeToDraw.points[0].y.ToString("F4"));
        }

        GameObject markedClone = Instantiate(originalObject, shapeToDraw.points[Index], Quaternion.identity);
        markedClone.transform.parent = parentObject.transform;
        markedClone.name = "MarkedPoint" + Index;

        lineRender.positionCount = Index + 1;
        lineRender.SetPosition(Index, shapeToDraw.points[Index]);

        

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

}
