
public enum ActionType
{
    //add new before this
    Invalid,
    Scout,
    Approach,
    Aim,
    Shoot,
    Load,
    Detonate_Bomb,
    Flee,
    Action_Num,
}

public class Action
{
    protected ActionType m_actionType;

    protected WorldState m_precondictions = new WorldState();
    protected WorldState m_effects = new WorldState();

    public int cost = 3;

    public Action(ActionType actiontype)
    {
        m_actionType = actiontype;
    }

    public WorldState precondictions { get { return m_precondictions; } }
    public WorldState effects { get { return m_effects; } }
    public ActionType actionType { get { return m_actionType; } }
}
