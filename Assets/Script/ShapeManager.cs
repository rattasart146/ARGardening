using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;

public class ShapeManager : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject parentObject;
    public Vector3 size;
    public float area;
    public MeshRenderer renderer;
    private LineRenderer lineRender;
    private int Index = 0;
    List<Shape> shapes = new List<Shape>();
    bool shapeChangedSinceLastRepaint;

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;


    private List<GameObject> areaCal = new List<GameObject>();
    private List<GameObject> areaCal2 = new List<GameObject>();
    Ray myRay;
    RaycastHit hit;
    GameObject barPanel, planeDetectPanel;

    // Start is called before the first frame update
    private void Awake()
    {
        lineRender = GetComponent<LineRenderer>();
        CreateNewShape();
        barPanel = GameObject.Find("BarPanel");
        planeDetectPanel = GameObject.Find("PlaneDetectPanel");
    }
    // Update is called once per frame
    void Update()
    {

        size = renderer.bounds.size;
        area = (size.x) * (size.z);
        if (Input.GetMouseButtonDown(0))
        {
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(myRay, out hit, 100))
            {

                Draw(hit);
            }
        }
        //calAreaText.text = "areaCal length :" + areaCal.Count;
        if (areaCal.Count > 2)
        {
            SuperficieIrregularPolygon();
        }
    }

    private void Draw(RaycastHit hit)
    {
        Shape shapeToDraw = shapes[0];

        Index = shapeToDraw.points.Count;
        if (Index == 0)
        {
            shapeToDraw.points.Add(hit.point);
        }
        else
        {
            shapeToDraw.points.Add(new Vector3(hit.point.x, shapeToDraw.points[0].y, hit.point.z));
            //Debug.Log("V3 point :" + shapeToDraw.points[0].y.ToString("F4"));
        }

        GameObject markedClone = Instantiate(originalObject, shapeToDraw.points[Index], Quaternion.identity);
        areaCal.Add(markedClone);
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

    private void SuperficieIrregularPolygon()
    {
        float temp = 0;
        for (int i = 0; i < areaCal.Count; i++)
        {
            //Debug.Log($"Pos Point {i} : {areaCal[i].transform.position.x} ,{areaCal[i].transform.position.z}");
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
        //Debug.Log("Area : " + Mathf.Round(temp * 100f) / 100f);

    }

}
