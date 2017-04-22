

using System.Collections.Generic;

public class ActionManager
{
    public List<Action> m_lstActions = new List<Action>();
    public static readonly ActionManager me = new ActionManager();

    public List<Action> GetAllActions()
    {
        return m_lstActions;
    }

    public void AddAction(Action action)
    {
        m_lstActions.Add(action);
    }
}
