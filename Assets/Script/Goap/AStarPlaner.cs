
using System;
using System.Collections;
using System.Collections.Generic;

public class AStarPlaner
{
    public static AStarSharpNode[] Plan(WorldState start, WorldState goal, IStorage storage)
    {
        AStarSharpNode currentNode = new AStarSharpNode();
        currentNode.wordState = start;
        currentNode.parentWordState = start;
        currentNode.costSoFar = 0; // g
        currentNode.heuristicCost = start.GetWorldStateDiffNum(goal); //h
        currentNode.costSoFarAndHeurisitcCost = currentNode.costSoFar + currentNode.heuristicCost; // f
        currentNode.actionType = ActionType.Invalid;
        currentNode.parent = null;
        currentNode.depth = 1;

        storage.AddToOpenList(currentNode);

        while (true)
        {
            if (!storage.HasOpened())
            { // Console.WriteLine( "Did not find a path." ); 
                return null;
            }

            currentNode = storage.RemoveCheapestOpenNode();
            // Console.WriteLine ("--------------------------------------\n");
            // Console.WriteLine("CurrentNode: " + currentNode);
            // Console.WriteLine("CurrentState: " + currentNode.ws);
            // Console.WriteLine(string.Format("Opened: {0}    Closed: {1}", storage._opened.Count, storage._closed.Count));

            storage.AddToClosedList(currentNode);
            // Console.WriteLine("CLOSING: " + currentNode);

            if (currentNode.wordState.IsSatisfiedOtherState(goal))
            {
                // Console.WriteLine ("Finished with plan");
                return ReconstructPlan(currentNode);
            }

            //				var actionnames = new string[ActionPlanner.MAXACTIONS ];
            //				var actioncosts = new int[ ActionPlanner.MAXACTIONS ];
            //				var to = new WorldState[ ActionPlanner.MAXACTIONS ];
            //				int numtransitions = ap.GetPossibleTransitions(currentNode.ws, to, actionnames, actioncosts, ActionPlanner.MAXACTIONS );

            var neighbours = GetExpand(currentNode);

            for (var i = 0; i < neighbours.Count; i++)
            {
                var cur = neighbours[i];

                // Console.WriteLine("Processing {0} -> {1}", currentNode.actionname, cur.actionname);
                // Console.WriteLine("State: " + cur.ws);

                var opened = storage.FindOpened(cur);
                var closed = storage.FindClosed(cur);
                int cost = currentNode.costSoFar + cur.costSoFar;

                // Console.WriteLine("Cost: {0}  Idx Opened: {1}  Idx Closed: {2}", cost, opened, closed);

                // if neighbor in OPEN and cost less than g(neighbor):
                if (opened != null && cost < opened.costSoFar)
                {
                    // Console.WriteLine("OPENED Neighbor: " + opened.Value.ws);
                    // Console.WriteLine("neighbor in OPEN and cost less than g(neighbor)");

                    // remove neighbor from OPEN, because new path is better
                    storage.RemoveOpened(opened);
                    opened = null;
                }

                // if neighbor in CLOSED and cost less than g(neighbor):
                if (closed != null && cost < closed.costSoFar)
                {
                    // Console.WriteLine("CLOSED Neighbor: " + closed.Value.ws);
                    // Console.WriteLine("neighbor in CLOSED and cost less than g(neighbor)");

                    // remove neighbor from CLOSED
                    storage.RemoveClosed(closed);
                }

                // if neighbor not in OPEN and neighbor not in CLOSED:
                if (opened == null && closed == null)
                {
                    AStarSharpNode nb = new AStarSharpNode();
                    nb.wordState = cur.wordState;
                    nb.costSoFar = cost;
                    nb.heuristicCost = cur.wordState.GetWorldStateDiffNum(goal);
                    nb.costSoFarAndHeurisitcCost = nb.costSoFar + nb.heuristicCost;
                    nb.actionType = cur.actionType;
                    nb.parentWordState = currentNode.wordState;
                    nb.parent = currentNode;
                    nb.depth = currentNode.depth + 1;
                    storage.AddToOpenList(nb);

                    // Console.WriteLine("NEW OPENED: " + nb.ToString());
                }
                // Console.WriteLine("\n--\n");
            }
        }
    }

    //!< Internal function to reconstruct the plan by tracing from last node to initial node.
    static AStarSharpNode[] ReconstructPlan(AStarSharpNode goalnode)
    {
        var plan = new AStarSharpNode[goalnode.depth - 1];

        AStarSharpNode curnode = goalnode;
        for (var i = 0; i < goalnode.depth - 1; i++)
        {
            plan[i] = curnode;
            curnode = curnode.parent;
        }
        return plan;
    }

    static List<AStarSharpNode> GetExpand(AStarSharpNode node)
    {
        List<AStarSharpNode> ret = new List<AStarSharpNode>();

        List<Action> actions = ActionManager.me.GetAllActions();
        int num = actions.Count;
        int wsPropertyNum = (int)WorldPropKey.World_Prop_Num;
        bool expandFlag = false;
        bool selfFlag = false;
        WorldState curState = node.wordState;
        BitArray curFlag = curState.worldPropertySetFlag;

        for (int idxAction = 0; idxAction < num; ++idxAction)
        {
            Action tmpAction = actions[idxAction];
            WorldState precondition = tmpAction.precondictions;
            WorldState effects = tmpAction.effects;

            bool match = true;
            for (int idxPrecon = 0; idxPrecon < wsPropertyNum; ++idxPrecon)
            {
                BitArray flag = precondition.worldPropertySetFlag;
                expandFlag = flag.Get(idxPrecon);
                if (expandFlag)
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
                        //Debug.Log("self Action has " + (WorldPropKey)idxPrecon + " value: " + curState.GetWorldStateProperty(idxPrecon).value);
                        if (!precondition.GetWorldStateProperty(idxPrecon).Equals(curState.GetWorldStateProperty(idxPrecon)))
                        {
                            match = false;
                            break;
                        }
                    }
                }
            }
            if (match)
            {
                WorldState newState = curState.MergeToNew(effects);
                AStarSharpNode currentNode = new AStarSharpNode();
                //int g = node.gCost + tmpAction.cost;
                //int h = newState.GetWorldStateDiffNum(goal);
                //planNode.gCost = g;
                //planNode.hCost = h;
                //planNode.costTotal = g + h;

                //node.actionname = this.actionNames[i];
                //node.costSoFar = this.act_costs[i];
                //node.ws = ApplyPostConditions(this, i, fr);
                //result.Add(node);

                currentNode.wordState = newState;
                currentNode.parentWordState = curState;
                currentNode.costSoFar = tmpAction.cost; // g
                //currentNode.heuristicCost = newState.GetWorldStateDiffNum(goal); //h
                //currentNode.costSoFarAndHeurisitcCost = currentNode.costSoFar + currentNode.heuristicCost; // f
                currentNode.actionType = tmpAction.actionType;
                //currentNode.parent = node;
                //currentNode.depth = node.depth + 1;
                ret.Add(currentNode);
                //m_allNodes.Add(tmpAction.actionType);
            }
        }

        return ret;
    }
}

public class AStarSharpNode : IEquatable<AStarSharpNode>
{
    public WorldState wordState;
    public int costSoFar;               //!< The cost so far.
    public int heuristicCost;               //!< The heuristic for remaining cost (don't overestimate!)
    public int costSoFarAndHeurisitcCost;               //!< g+h combined.
    public ActionType actionType;       //!< How did we get to this node?
    public WorldState parentWordState;   //!< Where did we come from?
    public AStarSharpNode parent;
    public int depth;


    public override string ToString()
    {
        return string.Format("[{0} | {1}]: {2}", costSoFar, heuristicCost, actionType);
    }

    #region IEquatable implementation

    public bool Equals(AStarSharpNode other)
    {
        return wordState.Equals(other);
    }

    #endregion
}

public interface IStorage
{
    AStarSharpNode FindOpened(AStarSharpNode node);
    AStarSharpNode FindClosed(AStarSharpNode node);
    bool HasOpened();
    void RemoveOpened(AStarSharpNode node);
    void RemoveClosed(AStarSharpNode node);
    bool IsOpen(AStarSharpNode node);
    bool IsClosed(AStarSharpNode node);
    void AddToOpenList(AStarSharpNode node);
    void AddToClosedList(AStarSharpNode node);
    AStarSharpNode RemoveCheapestOpenNode();
}

public class ListStorage : IStorage
{
    public List<AStarSharpNode> _opened;
    public List<AStarSharpNode> _closed;

    public ListStorage()
    {
        _opened = new List<AStarSharpNode>(16);
        _closed = new List<AStarSharpNode>(16);
    }

    public AStarSharpNode FindOpened(AStarSharpNode node)
    {
        for (var i = 0; i < _opened.Count; i++)
        {
            //				long care = node.ws.dontcare ^ -1L;
            //				if ((node.ws.values & care) == (_opened[i].ws.values & care)) {
            //					return _closed [i];
            //				}

            if (node.Equals(_opened[i]))
            {
                return _opened[i];
            }
        }
        return null;
    }

    public AStarSharpNode FindClosed(AStarSharpNode node)
    {
        for (var i = 0; i < _closed.Count; i++)
        {
            //				long care = node.ws.dontcare ^ -1L;
            //				if ((node.ws.values & care) == (_closed[i].ws.values & care)) {
            //					return _closed [i];
            //				}
            if (node.Equals(_closed[i]))
            {
                return _closed[i];
            }

        }
        return null;
    }

    public bool HasOpened()
    {
        return _opened.Count > 0;
    }

    public void RemoveOpened(AStarSharpNode node)
    {
        _opened.Remove(node);
    }

    public void RemoveClosed(AStarSharpNode node)
    {
        _closed.Remove(node);
    }

    public bool IsOpen(AStarSharpNode node)
    {
        return _opened.Contains(node);
    }

    public bool IsClosed(AStarSharpNode node)
    {
        return _closed.Contains(node);
    }

    public void AddToOpenList(AStarSharpNode node)
    {
        _opened.Add(node);
    }

    public void AddToClosedList(AStarSharpNode node)
    {
        _closed.Add(node);
    }

    public AStarSharpNode RemoveCheapestOpenNode()
    {
        int lowestVal = int.MaxValue;
        int lowestIdx = -1;
        for (int i = 0; i < _opened.Count; i++)
        {
            if (_opened[i].costSoFarAndHeurisitcCost < lowestVal)
            {
                lowestVal = _opened[i].costSoFarAndHeurisitcCost;
                lowestIdx = i;
            }
        }
        var val = _opened[lowestIdx];
        _opened.RemoveAt(lowestIdx);
        return val;


        //			var item = _opened.Min ();
        //			_opened.Remove (item);
        //			return item;
    }
}
