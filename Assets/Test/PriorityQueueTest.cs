using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class PriorityQueueTestNode : FastPriorityQueueNode
{
    private string m_name = "node";
    private int m_cost = 1;

    public PriorityQueueTestNode(string name, int cost)
    {
        m_name = name;
        m_cost = cost;
    }

    public string name { get { return m_name; } }
    public int cost { get { return m_cost; } }
}

public class PriorityQueueTest : MonoBehaviour
{
    private FastPriorityQueue<PriorityQueueTestNode> m_lstNode = new FastPriorityQueue<PriorityQueueTestNode>(100);
    // Use this for initialization
    void Start () {

        PriorityQueueTestNode node1 = new PriorityQueueTestNode("one", 10);
        PriorityQueueTestNode node2 = new PriorityQueueTestNode("two", 3);
        PriorityQueueTestNode node3 = new PriorityQueueTestNode("three", 8);

        m_lstNode.Enqueue(node1, node1.cost);
        m_lstNode.Enqueue(node2, node2.cost);
        m_lstNode.Enqueue(node3, node3.cost);

        while(m_lstNode.Count != 0)
        {
            PriorityQueueTestNode node = m_lstNode.Dequeue();
            Debug.Log("name:" + node.name + " cost: " + node.cost);
        }
    }
}
