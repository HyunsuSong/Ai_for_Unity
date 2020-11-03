using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMgr : MonoBehaviour
{
    //기본 변수
    public int startNode = 1;
    public int endNode = 1;

    public Graph Graph;
    public Node[] Node;
    public Node[] set_Color = new Node[7];

    public int count = 1;

    private int temp;
    private int[] d = new int[7];
    private int[] d_path = new int[7];

    private int current;
    private int distance;

    private int INF = 1000000000;

    //BFS용 변수
    Queue<Node> BFS_Que = new Queue<Node>();
    Node first_node;

    private void Awake()
    {
        Node = new Node[30];

        for (int i = 1; i <= 29; i++)
        {
            Node[i] = GameObject.Find(i.ToString()).GetComponent<Node>();
        }
    }

    private void Update()
    {
        
    }

    void DFS(Node src)
    {
        if (src.getVisited() == true)
        {
            return;
        }

        src.setVisited(true);

        set_Color[count] = src;
        count++;

        for (int i = 1; i <= src._count; i++)
        {
            DFS(Graph.Connectednode(src, i));
        }
    }

    void BFS(Node src)
    {
        BFS_Que.Clear();

        BFS_Que.Enqueue(Node[startNode]);

        while (BFS_Que.Count != 0)
        {
            first_node = BFS_Que.Dequeue();

            for (int i = 0; i < first_node._count; i++)
            {
                if (!first_node.getVisited())
                {
                    first_node.setVisited(true);

                    set_Color[count] = first_node;
                    count++;

                    for (int j = 1; j <= first_node._count; j++)
                    {
                        BFS_Que.Enqueue(Graph.Connectednode(first_node, j));
                    }
                }
            }
        }
    }

    void dijkstra()
    {
        for (int i = 1; i < Node.Length; i++)
        {
            d[i] = INF;
        }

        d[startNode] = 0;

        List<Tuple<int, int>> pq = new List<Tuple<int, int>>(); //힙구조 유지

        pq.Add(new Tuple<int, int>(startNode, 0));

        // 가까운 순서대로 처리 -> 큐 사용
        while (pq.Count != 0)
        { // 우선순위 큐가 비어있지 않다면 

            pq.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            foreach (var q in pq)
            {
                // 큐의 가장 위에는 가장 적은 비용을 가진 node의 정보가 들어있다.
                current = q.Item1;
                // 짧은 것이 먼저 오도록
                distance = q.Item2;
                pq.RemoveAt(0);
                break;
            }

            // 현재 저장된 최단 거리가 가져온 큐의 거리보다 작을 경우 스킵
            if (d[current] < distance)
            {
                continue;
            }

            for (int i = 1; i <= Node[current]._count; i++)
            {
                // 선택된 노드의 인접노드를 담아줌, first는 int int의 첫번째 인자, 즉 노드 번호 / second는 두번째 인자, 즉 가중치를 의미함
                int next = Node[current]._Connected_node[i];
                // 선택된 노드를 거쳐서 인접노드로 가는 비용 계산
                int nextDistance = distance + Node[current]._node_Weight[i];

                // 기존의 비용과 비교
                if (nextDistance < d[next])
                {
                    d_path[next] = current;
                    d[next] = nextDistance;

                    pq.Add(new Tuple<int, int>(next, nextDistance));
                }
            }
        }
        for(int i=1;i<set_Color.Length;i++)
        {
            set_Color[i] = Node[i];
        }

        StartCoroutine(Color_Setting(3));
    }

    public void isDFS()
    {
        count = 1;

        for (int i = 1; i < Node.Length; i++)
        {
            Node[i].isVisited = false;
        }

        if (set_Color[1] != null)
        {
            for (int i = 1; i < set_Color.Length; i++)
            {
                set_Color[i].transform.GetComponent<MeshRenderer>().material.color = Color.white;
                set_Color[i] = null;
            }
        }

        Graph.set_ai = 1;
        StartCoroutine(Wait_Setting(Graph.set_ai));

        StartCoroutine(Color_Setting(Graph.set_ai));
    }

    public void isBFS()
    {
        count = 1;

        for (int i = 1; i < Node.Length; i++)
        {
            Node[i].isVisited = false;
        }

        if (set_Color[1] != null)
        {
            for (int i = 1; i < set_Color.Length; i++)
            {
                set_Color[i].transform.GetComponent<MeshRenderer>().material.color = Color.white;
                set_Color[i] = null;
            }
        }

        Graph.set_ai = 2;
        StartCoroutine(Wait_Setting(Graph.set_ai));

        StartCoroutine(Color_Setting(Graph.set_ai));
    }

    public void isDijkstra()
    {
        count = 1;

        for (int i = 1; i < Node.Length; i++)
        {
            Node[i].isVisited = false;
        }

        if (set_Color[1] != null)
        {
            for (int i = 1; i < set_Color.Length; i++)
            {
                set_Color[i].transform.GetComponent<MeshRenderer>().material.color = Color.white;
                set_Color[i] = null;
            }
        }

        Graph.set_ai = 3;

        StartCoroutine(Wait_Setting(Graph.set_ai));
    }

    IEnumerator Wait_Setting(int src)
    {
        switch (src)
        {
            case 1:
                yield return new WaitForSeconds(1.0f);
                DFS(Node[startNode]);
                break;

            case 2:
                yield return new WaitForSeconds(1.0f);
                BFS(Node[startNode]);
                break;

            case 3:
                yield return new WaitForSeconds(1.0f);
                dijkstra();
                break;
        }
    }

    IEnumerator Color_Setting(int src)
    {
        switch(src)
        {
            case 1:
                for (int i=1;i< set_Color.Length;i++)
                {
                    yield return new WaitForSeconds(1.0f);
                    set_Color[i].transform.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                break;

            case 2:
                for (int i = 1; i < set_Color.Length; i++)
                {
                    yield return new WaitForSeconds(1.0f);
                    set_Color[i].transform.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
                break;

            case 3:
                temp = endNode;

                while (temp != 0)
                {
                    set_Color[temp].transform.GetComponent<MeshRenderer>().material.color = Color.green;
                    yield return new WaitForSeconds(1.0f);

                    temp = d_path[temp];
                }

                break;
        }
    }
}
