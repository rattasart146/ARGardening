using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;

public class Gameplay : MonoBehaviour
{
    public GameObject originalCube;
    public GameObject parentCube;
    private LineRenderer lineRender;
    private int Index = 0;
    List<Vector3> positions = new List<Vector3>();

    Ray myRay;
    RaycastHit hit;

    // Start is called before the first frame update
    private void Awake()
    {
        lineRender = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        myRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        
        if (Physics.Raycast (myRay, out hit, 100))
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.name == "MarkedPoint0")
                {
                    Debug.Log("Done");
                    FinishMapping(hit);
                }
                else
                {
                    AddPrefabs(hit);
                }
            }
        }
    }
    public void AddPrefabs(RaycastHit hits)
    {
        Index = positions.Count;
        positions.Add(hits.point);
        GameObject markedClone = Instantiate(originalCube, hits.point, Quaternion.identity);
        markedClone.transform.parent = parentCube.transform;
        markedClone.name = "MarkedPoint" + Index;
        lineRender.positionCount = positions.Count;
        lineRender.SetPosition(Index, positions[Index]);
        Debug.Log("Index :" + Index);
    }

    public void FinishMapping(RaycastHit hits)
    {
        Index = positions.Count;
        positions.Add(hits.point);
        lineRender.positionCount = positions.Count;
        lineRender.SetPosition(Index, positions[0]);
    }

    //Shape Editor
    public MeshFilter meshFilter;

    [HideInInspector]
    public List<Shape> shapes = new List<Shape>();

    [HideInInspector]
    public bool showShapesList;

    public float handleRadius = .5f;

    public void UpdateMeshDisplay()
    {
        CompositeShape compShape = new CompositeShape(shapes, handleRadius);
        meshFilter.mesh = compShape.GetMesh();
    }


    SelectionInfo selectionInfo;
    bool shapeChangedSinceLastRepaint;


    public class SelectionInfo
    {
        public int selectedShapeIndex;
        public int mouseOverShapeIndex;

        public int pointIndex = -1;
        public bool mouseIsOverPoint;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine;
    }
}
