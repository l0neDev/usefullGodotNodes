//test

using Godot;
using System.Collections.Generic;

using AIcode.classes;
using AIcode.funcs;

public class Button : Godot.Button
{
    CheckBox cheapest, debug;

    public override void _Ready()
    {
        cheapest = GetNode<CheckBox>("cheapest");
        debug = GetNode<CheckBox>("debug");
    }

    private void _on_Button_pressed()
    {
        var conditions = new List<Condition>();
        var condition = new Condition("alive", true);
        conditions.Add(condition);
        condition = new Condition("enemy alive", true);
        conditions.Add(condition);
        condition = new Condition("hurt");
        conditions.Add(condition);
        condition = new Condition("enemy hurt");
        conditions.Add(condition);
        condition = new Condition("have weapon");
        conditions.Add(condition);
        condition = new Condition("have bomb");
        conditions.Add(condition);
        condition = new Condition("have heal item");
        conditions.Add(condition);        
        condition = new Condition("see enemy");
        conditions.Add(condition);
        condition = new Condition("enemy see me");
        conditions.Add(condition);
        condition = new Condition("close to enemy");
        conditions.Add(condition);

        var actions = new List<ActorAction>();
        var action = new ActorAction("find enemy", 0, new string[] { "see enemy" }, null, new string[] { "see enemy", "enemy see me" }, new bool[] { true, true });
        actions.Add(action);
        action = new ActorAction("find weapon", 0, new string[] { "have heal item", "have weapon", "have bomb", "enemy see me" }, null, new string[] { "have weapon" }, new bool[] { true });
        actions.Add(action);
        action = new ActorAction("find bomb", 1, new string[] { "have heal item", "have bomb", "have weapon", "enemy see me" }, null, new string[] { "have bomb" }, new bool[] { true });
        actions.Add(action);
        action = new ActorAction("find heal item", 0, new string[] { "have bomb", "have weapon", "have heal item", "enemy see me" }, null, new string[] { "have heal item" }, new bool[] { true });
        actions.Add(action);
        action = new ActorAction("heal yourself", 0, new string[] { "have heal item", "hurt", "close to enemy" }, new bool[] { true, true }, new string[] { "have heal item", "hurt" }, null);
        actions.Add(action);
        action = new ActorAction("chase enemy", 0, new string[] { "see enemy", "close to enemy" }, new bool[] { true }, new string[] { "close to enemy" }, new bool[] { true });
        actions.Add(action);
        action = new ActorAction("attack enemy", 0, new string[] { "have weapon", "close to enemy" }, new bool[] { true, true }, new string[] { "enemy hurt", "hurt", "have weapon" }, new bool[] { true, true });
        actions.Add(action);
        action = new ActorAction("fall back", 0, new string[] { "close to enemy" }, new bool[] { true }, new string[] { "close to enemy" }, null);
        actions.Add(action);
        action = new ActorAction("run away", 0, new string[] { "enemy see me", "close to enemy" }, new bool[] { true }, new string[] { "see enemy", "enemy see me" }, null);
        actions.Add(action);
        action = new ActorAction("finish enemy", 0, new string[] { "have weapon", "close to enemy", "enemy hurt", "hurt" }, new bool[] { true, true, true }, new string[] { "enemy alive" }, null);
        actions.Add(action);
        action = new ActorAction("suicide with enemy", 1, new string[] { "have bomb", "close to enemy" }, new bool[] { true, true }, new string[] { "enemy alive", "alive" }, null);
        actions.Add(action);

        var goal = new Goal("defeat enemy", new string[] { "enemy alive" }, null);

        var actionChain = Planner.GetPlan(goal, actions, conditions, cheapest.Pressed, debug.Pressed);

        if (actionChain is null)
        {
            GD.Print("goal already reached or can't be reached, so no plan :(");
            return;
        }

        GD.Print("goal reached! (see Godot's console)");
        GD.PrintRaw("\nactions for solving goal so far (", actionChain.Count, "):\n");
        foreach(var act in actionChain)
        {
            GD.PrintRaw("[", act.ActionName, "] > ");
        }
        GD.PrintRaw("goal!\n");
    }
}
