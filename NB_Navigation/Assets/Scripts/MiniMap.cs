using System;
using Assets.Scripts;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    public List<Node> nodeMap = new List<Node>();
    public List<Node> route = new List<Node>();

    float scaleFactor = 1 / 25.0f; // The scale of the minimap relative to the world scale.
                                   // Use this when you add the LineRenderer

    // Use this for initialization
    void Start () {
        LoadFromJson("/NodeMaps/MLK205.json");

        List<Node> route = GetRoute(GetNodeByID(1), GetNodeByID(8));

        LogRoute(route);

        DrawNodeMap(nodeMap);
        DrawRoute(route);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    //Visualy Displays Nodes and Edges of nodeMap
    public void DrawNodeMap(List<Node> nodeMap)
    {
        //Used to make sure no edge is not accidently redrawn multiple times
        List<Tuple<int, int>> drawnIDPairs = new List<Tuple<int, int>>();
        foreach (Node node in nodeMap)
        {
            //Drawing Nodes
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = node.position;
            sphere.name = "node_" + node.id;


            //Drawing Edges
            foreach (int adjNodeID in node.adjacentNodeIDs)
            {
                Node adjNode = GetNodeByID(adjNodeID);

                if (EdgeHasBeenDrawn(drawnIDPairs, node.id, adjNodeID) == false)
                {
                    LineRenderer line = new GameObject().AddComponent<LineRenderer>();
                    line.useWorldSpace = true;
                    line.startWidth = 0.05f;
                    line.endWidth = 0.05f;
                    //starting point
                    line.SetPosition(0, node.position);
                    //ending point
                    line.SetPosition(1, adjNode.position);
                    line.name = "edge_" + node.id + "_to_" + adjNode.id;

                    drawnIDPairs.Add(new Tuple<int, int>(node.id, adjNode.id));
                }
            }
        }
    }

    //Visualy Displays Nodes and Edges of route
    public void DrawRoute(List<Node> route)
    {
        int posIndex = 0;

        LineRenderer line = new GameObject().AddComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.startWidth = 0.25f;
        line.endWidth = 0.25f;
        line.material.color = Color.blue;
        line.name = "route_path";
        line.positionCount = route.Count;

        foreach (Node node in route)
        {
            line.SetPosition(posIndex, node.position);
            posIndex++;
        }
    }

    //helper function for DrawRoute
    public bool DoesConnectToRouteNode(Node adjNode)
    {
        bool doesConnect = true;

        foreach(Node node in route)
        {
            if(adjNode.id == node.id)
            {
                doesConnect = true;
            }
        }
        return doesConnect;
    }

    //helper function for DrawNodeMap to avoid redrawing same edges
    public bool EdgeHasBeenDrawn(List<Tuple<int, int>> drawnIDPairs, int curID, int adjID)
    {
        bool hasBeenDrawn = false;

        foreach(Tuple<int,int> IDPair in drawnIDPairs)
        {
            if( (IDPair.Item1 == curID) && (IDPair.Item2 == adjID) )
            {
                hasBeenDrawn = true;
            }
            else if( (IDPair.Item1 == adjID) && (IDPair.Item2 == curID) )
            {
                hasBeenDrawn = true;
            }
        }
        //if not true then hasBeenDrawn remains false
        return hasBeenDrawn;
    }

    public List<Node> GetRoute(Node source, Node destination)
    {
        Debug.Log("Source: " + source.id + " / Destination: " + destination.id);
        Pathfinder pathFinder = new Pathfinder(nodeMap, source, destination);
        pathFinder.Execute();

        return pathFinder.GetShortestPath(); 
    }

    public void LogRoute(List<Node> route)
    {
        string output = "Route: ";

        for(int i = 0; i < route.Count; i++)
        {
            output += route[i].id;

            if (i < route.Count - 1) output += " -> ";
        }

        Debug.Log(output);
    }

    public Node GetNodeByID(int id)
    {
        return nodeMap.Find(x => x.id.Equals(id));
    }

    void LogNodes()
    {
        foreach (Node node in nodeMap)
        {
            Debug.Log(node.id + " (" + node.position.x + ", " + node.position.y + ", " + node.position.z + ")");
        }
    }

    void LoadFromJson(string directory)
    {
        string filePath = Application.streamingAssetsPath + directory;

        Debug.Log(filePath);

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            NodeMap jsonNodeMap = JsonUtility.FromJson<NodeMap>(jsonContent);

            nodeMap = ConvertedJsonNodeMap(jsonNodeMap);
        }
        else
        {
            Debug.LogError("Cannot load data from " + filePath);
        }
    }

    public List<Node> ConvertedJsonNodeMap(NodeMap jsonNodeMap)
    {
        List<Node> nodeMap = new List<Node>();

        foreach(JsonNode jsonNode in jsonNodeMap.nodes)
        {
            Node node = new Node();

            node.id = jsonNode.id;
            node.position = new Vector3(jsonNode.position.x, jsonNode.position.y, jsonNode.position.z);
            node.adjacentNodeIDs = jsonNode.adjacentNodeIDs;

            nodeMap.Add(node);
        }

        return nodeMap;
    }

    [Serializable]
    public class NodeMap
    {
        public string title;
        public List<JsonNode> nodes;
    }

    [Serializable]
    public class JsonNode
    {
        public int id;
        public Positition position;
        public List<int> adjacentNodeIDs;

    }

    [Serializable]
    public class Positition
    {
        public float x;
        public float y;
        public float z;
    }
}
