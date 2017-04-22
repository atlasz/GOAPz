using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMyGoap : MonoBehaviour {
	// Use this for initialization
	void Start ()
    {
        WorldState world = new WorldState();

        //ap.SetPrecondition("scout", "armedwithgun", true);
        //ap.SetPostcondition("scout", "enemyvisible", true);

        //ap.SetPrecondition("approach", "enemyvisible", true);
        //ap.SetPostcondition("approach", "nearenemy", true);

        //ap.SetPrecondition("aim", "enemyvisible", true);
        //ap.SetPrecondition("aim", "weaponloaded", true);
        //ap.SetPostcondition("aim", "enemylinedup", true);

        //ap.SetPrecondition("shoot", "enemylinedup", true);
        //ap.SetPostcondition("shoot", "enemyalive", false);

        //ap.SetPrecondition("load", "armedwithgun", true);
        //ap.SetPostcondition("load", "weaponloaded", true);

        //ap.SetPrecondition("detonatebomb", "armedwithbomb", true);
        //ap.SetPrecondition("detonatebomb", "nearenemy", true);
        //ap.SetPostcondition("detonatebomb", "alive", false);
        //ap.SetPostcondition("detonatebomb", "enemyalive", false);

        //ap.SetPrecondition("flee", "enemyvisible", true);
        //ap.SetPostcondition("flee", "nearenemy", false);

        Action scout = new Action(ActionType.Scout);
        scout.precondictions.SetWorldStateProperty(WorldPropKey.Armed_With_Gun, WorldPropType.Default, 1);
        scout.effects.SetWorldStateProperty(WorldPropKey.Enemy_Visible, WorldPropType.Default, 1);

        Action approach = new Action(ActionType.Approach);
        approach.precondictions.SetWorldStateProperty(WorldPropKey.Enemy_Visible, WorldPropType.Default, 1);
        approach.effects.SetWorldStateProperty(WorldPropKey.Near_Enemy, WorldPropType.Default, 1);

        Action aim = new Action(ActionType.Aim);
        aim.precondictions.SetWorldStateProperty(WorldPropKey.Enemy_Visible, WorldPropType.Default, 1);
        aim.precondictions.SetWorldStateProperty(WorldPropKey.Weapon_Loaded, WorldPropType.Default, 1);
        aim.effects.SetWorldStateProperty(WorldPropKey.Enemy_Linedup, WorldPropType.Default, 1);

        Action shoot = new Action(ActionType.Shoot);
        shoot.precondictions.SetWorldStateProperty(WorldPropKey.Enemy_Linedup, WorldPropType.Default, 1);
        shoot.effects.SetWorldStateProperty(WorldPropKey.Enemy_Alive, WorldPropType.Default, 0);

        Action load = new Action(ActionType.Load);
        load.precondictions.SetWorldStateProperty(WorldPropKey.Armed_With_Gun, WorldPropType.Default, 1);
        load.effects.SetWorldStateProperty(WorldPropKey.Weapon_Loaded, WorldPropType.Default, 1);

        Action detonatebomb = new Action(ActionType.Detonate_Bomb);
        detonatebomb.precondictions.SetWorldStateProperty(WorldPropKey.Armed_With_Bomb, WorldPropType.Default, 1);
        detonatebomb.precondictions.SetWorldStateProperty(WorldPropKey.Near_Enemy, WorldPropType.Default, 1);
        detonatebomb.effects.SetWorldStateProperty(WorldPropKey.Alive, WorldPropType.Default, 0);
        detonatebomb.effects.SetWorldStateProperty(WorldPropKey.Enemy_Alive, WorldPropType.Default, 0);

        Action flee = new Action(ActionType.Flee);
        flee.precondictions.SetWorldStateProperty(WorldPropKey.Enemy_Visible, WorldPropType.Default, 1);
        flee.effects.SetWorldStateProperty(WorldPropKey.Near_Enemy, WorldPropType.Default, 0);

        ActionManager.me.AddAction(scout);
        ActionManager.me.AddAction(approach);
        ActionManager.me.AddAction(aim);
        ActionManager.me.AddAction(shoot);
        ActionManager.me.AddAction(load);
        ActionManager.me.AddAction(detonatebomb);
        ActionManager.me.AddAction(flee);
        //startState.Set(ap.FindConditionNameIndex("enemyvisible"), false);
        //startState.Set(ap.FindConditionNameIndex("armedwithgun"), true);
        //startState.Set(ap.FindConditionNameIndex("weaponloaded"), false);
        //startState.Set(ap.FindConditionNameIndex("enemylinedup"), false);
        //startState.Set(ap.FindConditionNameIndex("enemyalive"), true);
        //startState.Set(ap.FindConditionNameIndex("armedwithbomb"), true);
        //startState.Set(ap.FindConditionNameIndex("nearenemy"), false);
        //startState.Set(ap.FindConditionNameIndex("alive"), true);

        world.SetWorldStateProperty(WorldPropKey.Enemy_Visible, WorldPropType.Default, 0);
        world.SetWorldStateProperty(WorldPropKey.Armed_With_Gun, WorldPropType.Default, 1);
        world.SetWorldStateProperty(WorldPropKey.Weapon_Loaded, WorldPropType.Default, 0);
        world.SetWorldStateProperty(WorldPropKey.Enemy_Linedup, WorldPropType.Default, 0);
        world.SetWorldStateProperty(WorldPropKey.Armed_With_Bomb, WorldPropType.Default, 1);
        world.SetWorldStateProperty(WorldPropKey.Near_Enemy, WorldPropType.Default, 0);
        world.SetWorldStateProperty(WorldPropKey.Alive, WorldPropType.Default, 1);

        //var goal = new WorldState(0, -1);
        //goal.Set(ap.FindConditionNameIndex("enemyalive"), false);
        //goal.Set(ap.FindConditionNameIndex("alive"), true);

        WorldState goal = new WorldState();
        goal.SetWorldStateProperty(WorldPropKey.Enemy_Alive, WorldPropType.Default, 0);
        goal.SetWorldStateProperty(WorldPropKey.Alive, WorldPropType.Default, 1);

        //m_planer.Plan(world, goal);
        AStarSharpNode[] plan = AStarPlaner.Plan(world, goal, new ListStorage());
        for(int i = 0; i < plan.Length; ++i)
        {
            Debug.Log("Plan " + i + " :" + plan[i].actionType);
        }
    }
	
}
