using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeGraph : MonoBehaviour
{
    [SerializeField] GameObject node, canvas;

    [HideInInspector] public Dictionary<string, States> nodes = new Dictionary<string, States>();
    [HideInInspector] public Dictionary<string, GameObject> graphNodes = new Dictionary<string, GameObject>();
    [HideInInspector] public Dictionary<string, float> rowsDict = new Dictionary<string, float>();
    
    public void BuildTree(string key, States score)
    {
        //build dictonary of nodes and update their values while building the tree to build each node
        if (!nodes.ContainsKey(key)) nodes.Add(key, score);
        else if (nodes.ContainsKey(key)) nodes[key] = score;
    }

    public void BuildGraph(float depthSearch)
    {
        int right = 0;
        Vector3 pos = transform.position;
        GameObject root = new GameObject("root node");

        //get length of nodes in each row/depth
        GenerateNodeLength();

        //build nodes 
        foreach (KeyValuePair<string, States> dictNode in nodes)
        {
            string[] splitKey = dictNode.Key.Split(' ');

            //Set what will be displayed via each node
            string nodeName = "";
            if (splitKey[3] == "") nodeName = splitKey[4];
            else nodeName = splitKey[3] + "\n" + splitKey[4];

            //splitKey[0] = the depth of each row on the list
            //Make adjustments if node is either root or doing evaluation calucaltions
            if (float.Parse(splitKey[0]) == depthSearch)
            {
                right = (int)transform.position.x;
            }
            else //calculate spacing based amount of nodes per row
                right = 15 * ((int)rowsDict[splitKey[0]] / 15);

            //place node based on number of nodes per level
            GameObject nodeObject = Instantiate(node, new Vector3((pos.x + right), transform.position.y + (float.Parse(splitKey[0]) * 50), transform.position.z), Quaternion.identity);
            pos = nodeObject.transform.position;

            //Create root node for tree graph
            if (float.Parse(splitKey[0]) == (depthSearch + 1))
                root = nodeObject;

            //Make node a part of the canvas to keep them organised
            nodeObject.transform.SetParent(canvas.transform);
            nodeObject.GetComponentInChildren<Text>().text = nodeName + "\n" + dictNode.Value.score.ToString();
            
            graphNodes.Add(dictNode.Key, nodeObject);
        }

        root.transform.position = new Vector3(transform.position.x * (graphNodes.Count), transform.position.y + (4 * 50), transform.position.z);
    }

    public void GenerateNodeLength()
    {
        foreach (KeyValuePair<string, States> dictNode in nodes)
        {
            string[] splitKey = dictNode.Key.Split(' ');

            //caluclate lengths of each row based on number of nodes
            if (!rowsDict.ContainsKey(splitKey[0]))
                rowsDict.Add(splitKey[0], 1);
            else
                rowsDict[splitKey[0]]++;
        }
    }

    //remove previous nodes to prevent confusion
    public IEnumerator DestroyGraphNode()
    {
        yield return new WaitForSeconds(2.5f);
        foreach (KeyValuePair<string, GameObject> graphNode in graphNodes)
        {
            Destroy(graphNode.Value);
        }

        //Incase node gameobject isn't on the graphnode list
        Destroy(GameObject.Find("root node"));

        rowsDict.Clear();
        graphNodes.Clear();
    }
}
