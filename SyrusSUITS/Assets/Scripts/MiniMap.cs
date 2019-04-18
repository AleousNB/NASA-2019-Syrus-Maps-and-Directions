using System;
using Assets.Scripts;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {
    public LineRenderer line;
    GameObject map;
    float scaleFactor = 1.0f / 1.0f; // The scale of the minimap relative to the world scale.
    Vector3 originalSize;

    // Use this for initialization
    void Start () {
        line = GetComponent<LineRenderer>();
        DrawNodes(NavigationService.nodeMap);
    }

    void Awake() {
        NavigationService.Instance.MapLoaded += OnMapLoad;
    }
	
	// Update is called once per frame
	void Update () {
        DrawRoute(NavigationService.route);
        //testing Resize
        ResizeMiniMap(20);
    }

    void OnMapLoad()
    {
        string modelName = NavigationService.Instance.modelName;
        Debug.Log("Loading map: " + modelName);

        map = Instantiate((GameObject)Resources.Load(modelName), Vector3.zero, Quaternion.Euler(0, 0, 90), transform);
        map.transform.localScale = scaleFactor * map.transform.localScale;
        originalSize = map.transform.localScale;
    }

    //Resizes Minimap Model/Route based on given scale multiplier (minimap:real world)
    public void ResizeMiniMap(float scale)
    {
        scaleFactor = scale / 1.0f; // The scale of the minimap relative to the world scale.
        map.transform.localScale = scaleFactor * originalSize;

        //Redraw Routes according to new scale
        DrawRoute(NavigationService.route);
    }

    public void DrawRoute(List<Node> route)
    {   
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.positionCount = route.Count;

        Vector3[] positions = new Vector3[route.Count];
        line.startWidth = scaleFactor * 0.03f;
        line.endWidth = scaleFactor * 0.03f;

        for (int i = 0; i < route.Count; i++)
        {
            line.SetPosition(i, scaleFactor * route[i].position);
        }
    }

    public void DrawNodes(List<Node> nodeMap)
    {
        for(int i = 0; i < nodeMap.Count; i++)
        {
            //GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //node.name = "Node " + nodeMap[i].id;
            //node.transform.position = nodeMap[i].position;
            //node.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        }
    }
}
