//script for sorting Ysort node's childs on every frame (see readme)
//note: it works only with childrens inherited from (or are) Node2D class, so other childrens of this node not going to be sorted

using Godot;
using System.Linq;
using System.Collections.Generic;

class YSort : Godot.YSort
{
    #region signals to emit
    [Signal]
    delegate void childrensSortEnabled(); //emits when [sortChildsEnabled] changed on true;
    [Signal]
    delegate void childrensSortDisabled(); //emits when [sortShildsEnabled] changed on false;
    [Signal]
    delegate void sortingOrderChanged(); //emits when [orderByClosest] changed;
    #endregion

    #region editor variables
    private bool _sortChildsEnabled = true;
    /// <summary>
    /// sort func will be called if true, and not if false
    /// </summary>
    [Export]
    public bool sortChildsEnabled
    {
        get { return _sortChildsEnabled; }
        set
        {
            _sortChildsEnabled = value;
            if (_sortChildsEnabled) EmitSignal(nameof(childrensSortEnabled));
            else EmitSignal(nameof(childrensSortDisabled));
        }
    }

    private bool _orderByClosest = true;
    /// <summary>
    /// what child will be firts in tree? closest if true, and farest if false
    /// </summary>
    [Export]
    public bool orderByClosest
    {
        get { return _orderByClosest; }
        set
        {
            _orderByClosest = value;
            EmitSignal(nameof(sortingOrderChanged));
        }
    }
    #endregion
    /// <summary>
    /// Node2D inherited (or are) childrens of this Ysort node
    /// </summary>
    IEnumerable<Node2D> YsortChilds;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        YsortChilds = GetChildren().OfType<Node2D>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (SortEnabled && sortChildsEnabled) SortNodesByY();
    }

    /// <summary>
    /// sorting func, result are - closest/farest (closest by default) node will be the first child of this Ysort node
    /// </summary>
    /// <param name="closest">sort by closest or farest?</param>
    private void SortNodesByY()
    {
        int Node2DChildsCount = GetChildren().OfType<Node2D>().Count();

        if (Node2DChildsCount < 2) //if there is nothing to sort (this Ysort node have no Node2D childs or have only one) just return(exit) from func
        {
            return;
        }

        if (Node2DChildsCount != YsortChilds.Count()) //if child count changed (some child(s) added or removed from this Ysort node) -> retaking nodes
        {
            YsortChilds = GetChildren().OfType<Node2D>();
        }

        //always doing sort func if we have some childs
        IEnumerable<Node2D> sortedChilds;

        if (orderByClosest)
        {
            sortedChilds = YsortChilds.OrderByDescending(child => child.Position.y);
        }
        else
        {
            sortedChilds = YsortChilds.OrderBy(child => child.Position.y);
        }

        if (sortedChilds.SequenceEqual(YsortChilds)) //if same order after sort func (we have same nodes and same order of them on screen) -> just return
        {
            return;
        }

        //if not same, saving new order and move childs on this Ysort node
        YsortChilds = sortedChilds;

        int moveToIndex = 0; //first element of YsortChilds will be moved on zero index position of childs of this Ysort node

        foreach(var child in YsortChilds)
        {
            MoveChild(child, moveToIndex);
            moveToIndex++; //and for next iterration of foreach we just increase index
        }
    }
}
