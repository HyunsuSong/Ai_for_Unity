using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isVisited = false;
    public int nodeindex;
    public int[] _Connected_node = new int[30];
    public int[] _node_Weight = new int[30];
    public int _count;

    public void setVisited(bool state)
    {
        isVisited = state;
    }

    public bool getVisited()
    {
        return isVisited;
    }

    public void setNodeIndex(int node_index)
    {
        {
            nodeindex = node_index;
        }
    }

    public int nodeWeight(int a)
    {
        return _node_Weight[a];
    }

    public int getConnectedNode(int a)
    {
        return _Connected_node[a];
    }

    public void connectNodeIndex(int count_num, int connected_node, int node_Weight)
    {
        _count = count_num;
        _Connected_node[_count] = connected_node;
        _node_Weight[_count] = node_Weight;
    }
    
}
