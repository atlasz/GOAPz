using System;
using System.Collections;
using System.Collections.Generic;

public enum WorldPropKey
{
    //add new before this
    Enemy_Visible,
    Armed_With_Gun,
    Weapon_Loaded,
    Enemy_Linedup,
    Armed_With_Bomb,
    Near_Enemy,
    Alive,
    Enemy_Alive,
    World_Prop_Num,
}

public enum WorldPropType
{
    Default,
}

public class WorldProperty : IEquatable<WorldProperty>
{
    public WorldPropKey key;
    public WorldPropType proType;
    public int value;

    public bool Equals(WorldProperty other)
    {
        if (other == null)
        {
            return false;
        }
        if(key != other.key)
        {
            return false;
        }
        if(value != other.value)
        {
            return false;
        }
        return proType == other.proType;
    }

    public WorldProperty Clone()
    {
        WorldProperty prop = new WorldProperty();
        prop.key = key;
        prop.proType = proType;
        prop.value = value;
        return prop;
    }
}

public class WorldState : IEquatable<WorldState>
{
    protected WorldProperty[] m_worldProps;
    protected BitArray m_propMask;
    private int m_worldPropertyNum = 0;

    public WorldState()
    {
        m_worldPropertyNum = (int)WorldPropKey.World_Prop_Num;
        m_worldProps = new WorldProperty[(int)WorldPropKey.World_Prop_Num];
        m_propMask = new BitArray((int)WorldPropKey.World_Prop_Num);
    }

    public WorldState(WorldProperty[] props, BitArray propMask)
    {
        m_worldProps = props;
        m_propMask = propMask;
        m_worldPropertyNum = (int)WorldPropKey.World_Prop_Num;
    }

    public WorldState Clone()
    { 
        int num = (int)WorldPropKey.World_Prop_Num;
        WorldProperty[] propArr = new WorldProperty[num];
        for(int i = 0; i < num; ++i)
        {
            WorldProperty tmp = m_worldProps[i];
            if (tmp != null)
            {
                propArr[i] = tmp.Clone();
            }
        }
        //Array.Copy(m_worldProps, propArr, num);
        return new WorldState(propArr, (BitArray)m_propMask.Clone());
    }

    public bool Equals(WorldState other)
    {
        if (other == null)
        {
            return false;
        }
        return GetWorldStateDiffNum(other) == 0;
    }

    public bool IsSatisfiedOtherState(WorldState otherState)
    {
        if (otherState == null)
        {
            return false;
        }

        bool bIsSelfSet = false;
        bool bIsOtherSet = false;

        //bool isSatisfied = true;
        for (int idxProperty = 0; idxProperty < m_worldPropertyNum; ++idxProperty)
        {
            bIsSelfSet = m_propMask.Get(idxProperty);
            bIsOtherSet = otherState.HasWorldProperty(idxProperty);
            if (bIsOtherSet)
            {
                if(bIsSelfSet)
                {
                    if (GetWorldStateProperty(idxProperty).value != otherState.GetWorldStateProperty(idxProperty).value ||
                        GetWorldStateProperty(idxProperty).proType != otherState.GetWorldStateProperty(idxProperty).proType)
                    {
                        return false;
                    }
                }
                else
                {
                    return false; 
                }
            }
        }
        return true;
    }

    public void SetWorldStateProperty(WorldPropKey key, WorldPropType proType, int value)
    {
        int idx = (int)key;
        if(m_worldProps[idx] == null)
        {
            m_worldProps[idx] = new WorldProperty();
        }
        m_worldProps[idx].key = key;
        m_worldProps[idx].proType = proType;
        m_worldProps[idx].value = value;
        m_propMask.Set(idx, true);
    }

    public void SetWorldStateProperty(WorldProperty prop)
    {
        SetWorldStateProperty(prop.key, prop.proType, prop.value);
    }

    public void ClearWorldStateProperty(WorldPropKey key)
    {
        m_propMask.Set((int)key, false);
    }

    public bool HasWorldProperty(WorldPropKey key)
    {
        return m_propMask.Get((int)key);
    }

    public bool HasWorldProperty(int idxMask)
    {
        return m_propMask.Get(idxMask);
    }

    public WorldProperty GetWorldStateProperty(WorldPropKey key)
    {
        int idx = (int)key;
        return GetWorldStateProperty(idx);
    }

    public WorldProperty GetWorldStateProperty(int idxKey)
    {
        if (idxKey >= m_worldPropertyNum)
        {
            return null;
        }

        if (m_propMask.Get(idxKey))
        {
            return m_worldProps[idxKey];
        }

        return null;
    }

    public int GetWorldStateDiffNum(WorldState otherState)
    {
        int diff = 0;
        bool bIsSelfSet = false;
        bool bIsOtherSet = false;

        for(int idxProperty = 0; idxProperty < m_worldPropertyNum; ++idxProperty)
        {
            bIsSelfSet = m_propMask.Get(idxProperty);
            bIsOtherSet = otherState.HasWorldProperty(idxProperty);
            if(bIsSelfSet && bIsOtherSet)
            {
                if(GetWorldStateProperty(idxProperty).value != otherState.GetWorldStateProperty(idxProperty).value ||
                    GetWorldStateProperty(idxProperty).proType != otherState.GetWorldStateProperty(idxProperty).proType)
                {
                    ++diff;
                }
            }
            else if(bIsSelfSet || bIsOtherSet)
            {
                ++diff;
            }
        }

        return diff;
    }

    public WorldState MergeToNew(WorldState to)
    {
        WorldState ret = this.Clone();
        int wsPropertyNum = (int)WorldPropKey.World_Prop_Num;
        bool bOtherSet = false;
        for (int idxWProp = 0; idxWProp < wsPropertyNum; ++idxWProp)
        {
            bOtherSet = to.HasWorldProperty(idxWProp);
            if (bOtherSet)
            {
                WorldProperty toProp = to.GetWorldStateProperty(idxWProp);
                ret.SetWorldStateProperty(toProp);
            }
        }
        return ret;
    }

    public BitArray worldPropertySetFlag { get { return m_propMask;} }
}
