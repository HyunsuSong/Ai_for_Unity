using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//노드 이외의 부분은 '게임 상에서 정상적인 루트가 아님'이기 때문에 길찾기를 할 경우 무조건 노드를 거치게 되는 것인가?
//아주 짧은 거리나 객체 위치와 내가 클릭한 곳의 위치 거리가 가장 가까운 노드보다 짧다면 바로 가게 하는 것은 옳은 것인가?

public class PlayerMgr : MonoBehaviour
{
    public GameObject Graph;

    public float _maxSpeed = 1.0f;
    public float speed = 1.0f;
    public float Mass = 15;

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _pickPos;
    private RaycastHit hit;

    public int startNode = 1;
    public int endNode = 1;

    public Node[] Node_my;
    public Node[] target_Node;

    public int count = 1;

    private int temp;
    private int[] d;
    public int[] d_path;

    private int current;
    private int distance;
    private int t_count=0;
    private int t_count_2 = 0;

    private float start_Length; // 시작노드 길이 값
    private float end_Length; // 끝날노드 길이 값
    private float short_Length;

    private int INF = 1000000000;

    private void Start()
    {
        Node_my = new Node[30];
        target_Node = new Node[30];
        d = new int[30];
        d_path = new int[30];

        Node_my = Graph.GetComponent<AiMgr>().Node;

        _pickPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //클릭시마다 초기화 해주어야 하는 ㅂ분
            t_count = 0;
            t_count_2 = 0;
            startNode = 0;
            endNode = 0;

            for (int i = 0; i < d_path.Length; i++)
            {
                d_path[i] = 0;
            }

            for(int i=0;i<target_Node.Length;i++)
            {
                target_Node[i] = null;
            }

            Vector3 mouse_pos = Input.mousePosition;

            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), -Vector3.up, out hit, 1000))
            {
                if(hit.collider.tag != "Wall")
                _pickPos = hit.point;
            }

            dijkstra();
        }

        //해당 타겟노드가 비지 않았다면 Seek를 통해서 해당 노드로 가고, 해당 노드에 다다르면 벡터를0으로 바꾼 후
        //다음 타겟노드로 이동하는 방식으로 했는데, translate로 해도 될까?
        if (target_Node[t_count_2] != null)
        {
            _velocity = _velocity + (Seek(target_Node[t_count_2].transform.position) * Time.deltaTime);

            if ((target_Node[t_count_2].transform.position - transform.position).magnitude <= 0.2f)
            {
                //해당 노드를 향한 벡터를 0으로 만들어 주고 새로운 벡터값을 주는 방식
                _velocity = Vector3.zero;

                t_count_2++;
            }
        }
        //마지막 노드까지 이동을 했다면 그때부터는 클릭한 위치까지 arrive를 해주었음
        else
        {
            _velocity = _velocity + (Arrive(_pickPos) * Time.deltaTime);
        }

        if (_velocity != Vector3.zero)
        {
            transform.position = transform.position + _velocity;
            transform.forward = _velocity.normalized;
        }
    }

    private void FindNode()
    {
        short_Length = (_pickPos - transform.position).magnitude; //비교할 대상

        for (int i = 1; i < Node_my.Length; i++)
        {
            start_Length = (_pickPos - Node_my[i].transform.position).magnitude;

            //내가 찍은 위치와 객체사이의 거리보다 찍은 위치와 노드 사이의 거리가 더 가까우면 그 중 가장 짧은 노드가 시작노드(역순이므로)
            if (start_Length <= short_Length)
            {
                short_Length = start_Length;
                startNode = i; //마우스 위치와 가장 가까운 노드가 엔드 노드 > 역으로 계산하기 때문
            }
        }

        short_Length = (_pickPos - transform.position).magnitude;

        for (int i=1;i<Node_my.Length;i++)
        {
            end_Length = (transform.position - Node_my[i].transform.position).magnitude;

            //내가 찍은 위치와 객체사이의 거리보다 객체의 위치와 노드 사이의 거리가 더 가까우면 그중 가장 짧은 노드가 끝나는 노드
            if(end_Length <= short_Length)
            {
                short_Length = end_Length;
                endNode = i;
            }
        }
    }

    private Vector3 Seek(Vector3 target_pos)
    {
        Vector3 desired_velocity_1 = ((target_pos - transform.position).normalized) * _maxSpeed;

        desired_velocity_1.y = 0.0f;

        return (desired_velocity_1 - _velocity);
    }

    private Vector3 Arrive(Vector3 target_pos)
    {
        Vector3 targetVelocity = target_pos - transform.position;

        float dist = targetVelocity.magnitude;

        if (dist > 40.0f)
        {
            speed = _maxSpeed;
        }
        else
        {
            speed = _maxSpeed * (dist / 40.0f);
        }

        targetVelocity.Normalize();
        targetVelocity *= speed;

        Vector3 acceleration = targetVelocity - _velocity;

        acceleration *= 1 / 0.1f;

        if (acceleration.magnitude > 15.0f)
        {
            acceleration.Normalize();
            acceleration *= 5.0f;
        }

        speed = Mathf.Min(speed, _maxSpeed);

        return acceleration;
    }

    void dijkstra()
    {
        FindNode();

        for (int i = 1; i < Node_my.Length; i++)
        {
            d[i] = INF;
        }

        d[startNode] = 0;

        List<Tuple<int, int>> pq = new List<Tuple<int, int>>();

        if (startNode != 0)
        {
            pq.Add(new Tuple<int, int>(startNode, 0));
        }

        while (pq.Count != 0)
        {

            pq.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            foreach (var q in pq)
            {
                current = q.Item1;

                distance = q.Item2;
                pq.RemoveAt(0);
                break;
            }

            if (d[current] < distance)
            {
                continue;
            }

            for (int i = 1; i <= Node_my[current]._count; i++)
            {
                int next = Node_my[current]._Connected_node[i];

                int nextDistance = distance + Node_my[current]._node_Weight[i];

                if (nextDistance < d[next])
                {
                    d_path[next] = current;
                    d[next] = nextDistance;

                    pq.Add(new Tuple<int, int>(next, nextDistance));
                }
            }
        }
        temp = endNode; //객체와 가장 가까운 노드가 시작 노드

        while (temp != 0)
        {
            //리스트 같은걸 하나 만들어서 노드 순서를 저장(역순으로 뽑기 때문에 뽑은 순서대로 저장하면 됨)
            target_Node[t_count] = Node_my[temp];
            t_count++;
            temp = d_path[temp];
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pickPos, 1.0f);
    }
}
