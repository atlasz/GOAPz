using Priority_Queue;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
/// <summary>
/// 1.����һ���򵥵�goapϵͳ��ι�����˼������
/// 2.������ʲô�������ǵ�ǰ��WorldState��Goal
///     1��WorldState��a bunch of Key-Value �����ļ��ϣ������-���ţ���-����
///     2��Goal������һ������ļ��ϣ�Ҳ���Լ򵥵ر�����һ��WorldState��������
///         �����Ա���װ��һ��GoalType��һ��WorldState
/// 3.�����ʲô������Ǵӵ�ǰ��WorldState������Goal������WorldState�м��Action�ļ���
///     1)Action�Ǿ����AI��ִ�еĲ���
/// 4.Goap��������ʲô��������worldStateΪNode��ActionΪEdge������ͼ����ÿ��Action�����úõĴ��ۣ�Cost)�����ҳ���ǰNode��Ŀ��Node֮������·��
/// 中文测试
/// </summary>
public class Planer
{
    private FastPriorityQueue<PlanNode> m_lstNode = new FastPriorityQueue<PlanNode>(100);
    private Dictionary<int, List<Action>> m_dicPreconditions = new Dictionary<int, List<Action>>();
    //private HashSet<WorldState> m_allNodes = new HashSet<WorldState>();
    private List<PlanNode> m_testAction = new List<PlanNode>();

    public void Initialize()
    {
        BuildPreconditionsTable();
    }

    public void Plan(WorldState initState, WorldState goalState)
    {
        PlanNode startNode = new PlanNode(initState, new Action(ActionType.Invalid));
        m_lstNode.Enqueue(startNode, startNode.costTotal);
        //m_allNodes.Add(initState);
        int idx = 0;
        while(m_lstNode.Count != 0 && idx < 100)
        {
            PlanNode curNode = m_lstNode.Dequeue();
            if(curNode.curState.IsSatisfiedOtherState(goalState))
            {
                Debug.Log("find goal: " + idx);
                break;
            }
            List<PlanNode> expand = GetExpand(curNode, goalState);
            Debug.Log("expand count: " + expand.Count);
            if(expand.Count != 0)
            {
                m_testAction.Add(curNode);
            }
            for(int idxExpand = 0; idxExpand < expand.Count; ++idxExpand)
            {
                PlanNode tmpNode = expand[idxExpand];
                Debug.Log("node: " + tmpNode.action.actionType);
                m_lstNode.Enqueue(tmpNode, tmpNode.costTotal);
            }
            idx++;
        }
        Debug.Log("search times: " + idx + " plan num :" + m_testAction.Count);
        int i = 0;
        foreach(PlanNode plan in m_testAction)
        {
            i++;
            Debug.Log("step " + i + " :" + plan.action.actionType);
        }
    }

    public List<PlanNode> GetExpand(PlanNode node, WorldState goal)
    {
        List<PlanNode> ret = new List<PlanNode>();

        List<Action> actions = ActionManager.me.GetAllActions();
        int num = actions.Count;
        int wsPropertyNum = (int)WorldPropKey.World_Prop_Num;
        bool expandFlag = false;
        bool selfFlag = false;
        WorldState curState = node.curState;
        BitArray curFlag = curState.worldPropertySetFlag;

        for (int idxAction = 0; idxAction < num; ++idxAction)
        {
            Action tmpAction = actions[idxAction];
            WorldState precondition = tmpAction.precondictions;
            WorldState effects = tmpAction.effects;

            //if(m_allNodes.Contains(tmpAction.actionType) || tmpAction.actionType == node.action.actionType)
            //{
            //    continue;
            //}

            bool match = true;
            for (int idxPrecon = 0; idxPrecon < wsPropertyNum; ++idxPrecon)
            {
                BitArray flag = precondition.worldPropertySetFlag;
                expandFlag = flag.Get(idxPrecon);
                if(expandFlag)
                {
                    //Debug.Log("search Action has " + (WorldPropKey)idxPrecon + " value: " + precondition.GetWorldStateProperty(idxPrecon).value);
                    selfFlag = curFlag.Get(idxPrecon);
                    //Debug.Log("self Action has? " + (WorldPropKey)idxPrecon + selfFlag);
                    if (!selfFlag)
                    {
                        //Debug.Log("self Action NOT has " + (WorldPropKey)idxPrecon);
                        match = false;
                        break;
                    }
                    else
                    {
                        Debug.Log("self Action has " + (WorldPropKey)idxPrecon + " value: " + curState.GetWorldStateProperty(idxPrecon).value);
                        if (!precondition.GetWorldStateProperty(idxPrecon).Equals(curState.GetWorldStateProperty(idxPrecon)))
                        {
                            match = false;
                            break;
                        }
                    }
                }
            }
            if(match)
            {
                WorldState newState = curState.MergeToNew(effects);
                PlanNode planNode = new PlanNode(newState, tmpAction);
                int g = node.gCost + tmpAction.cost;
                int h = newState.GetWorldStateDiffNum(goal);
                planNode.gCost = g;
                planNode.hCost = h;
                planNode.costTotal = g + h;
                ret.Add(planNode);
                //m_allNodes.Add(tmpAction.actionType);
            }
        }

        return ret;
    }

    public bool IsInFringe(ActionType actionType)
    {
        foreach(PlanNode node in m_lstNode)
        {
            if(node.action.actionType == actionType)
            {
                return true;
            }
        }
        return false;
    }

    public void BuildPreconditionsTable()
    {
        List < Action > actions = ActionManager.me.GetAllActions();
        int num = actions.Count;
        WorldState preconditions;
        BitArray flags;
        int wsPropertyNum = (int)WorldPropKey.World_Prop_Num;

        for(int idxAction = 0; idxAction < num; ++idxAction)
        {
            Action curAction = actions[idxAction];
            preconditions = curAction.precondictions;
            flags = preconditions.worldPropertySetFlag;

            for(int idxWProp = 0; idxWProp < wsPropertyNum; ++idxWProp)
            {
                if(flags.Get(idxWProp))
                {
                    List<Action> outPredictions = null;
                    if(m_dicPreconditions.TryGetValue(idxWProp, out outPredictions))
                    {
                        outPredictions.Add(curAction);
                    }
                    else
                    {
                        m_dicPreconditions[idxWProp] = new List<Action>();
                        m_dicPreconditions[idxWProp].Add(curAction);
                    }
                }
            }
        }
    }
}

public class PlanNode: FastPriorityQueueNode
{
    public int id;
    public string name;
    public int costTotal = 0;
    public int hCost = 0;
    public int gCost = 0;
    public WorldState curState;
    public Action action;

    public PlanNode(WorldState worldState, Action curAction)
    {
        curState = worldState;
        action = curAction;
    }
}
