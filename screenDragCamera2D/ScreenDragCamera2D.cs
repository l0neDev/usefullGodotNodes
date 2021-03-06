//script for standart Godot's camera2D to implement screen drag function.
//note: its for tochscreen by default. To use it with mouse simply turn on "emulate touch from mouse" in project settings > input devices > pointing.

using Godot;

class ScreenDragCamera2D : Camera2D
{
    #region signals
    [Signal]
    delegate void screenDragStarted(Vector2 position); //emits on start of drag action, returns start position of it
    [Signal]
    delegate void screenDragEnded(Vector2 position); //emits on dragging end, returns end drag position
    [Signal]
    delegate void dragFuncEnabled(); //emits when dragging enabled
    [Signal]
    delegate void dragFuncDisabled(); //emits when dragging disabled
    #endregion

    #region editor variables
    private bool _dragFunctionEnabled = true;
    /// <summary>
    /// switch to enable/disable drag function
    /// </summary>
    [Export]
    public bool dragFunctionEnabled
    {
        get { return _dragFunctionEnabled; }
        set
        {
            _dragFunctionEnabled = value;
            if (_dragFunctionEnabled) EmitSignal(nameof(dragFuncEnabled));
            else EmitSignal(nameof(dragFuncDisabled));
        }
    }
    /// <summary>
    /// speed of camera follow drag (if set more than 1 - strongly recomend to set camera smoothing!)
    /// </summary>
    [Export]
    public float dragSpeed { get; set; } = 1;
    #endregion

    #region other variables
    /// <summary>
    /// current status of dragging (always false if [dragFunctionEnabled] = false)
    /// </summary>    
    public bool isDragging { get; set; } = false;
    /// <summary>
    /// direction of drag, to know where to move camera
    /// </summary>
    private Vector2 dragDirection;
    /// <summary>
    /// variable to save prevous drag direction (for check on changes)
    /// </summary>
    private Vector2 oldDirection;
    #endregion

    #region processing drag func
    public override void _Process(float delta) //process drag in here only if processing mode set to idle
    {
        if (ProcessMode == Camera2DProcessMode.Idle) //check for mode
        {
            if (dragFunctionEnabled && isDragging && dragDirection != oldDirection) //if dragfunc enabled, dragging now and direction changed
            {
                Position += -dragDirection * dragSpeed; //moving cam
                oldDirection = dragDirection; //saving current direction as prevous for next drag event
            }
            //note: if you not checking direction change - camera will move even if u stop dragging, but not released mouse button/finger from screen
            //cause godot will stop providing input events if you not moving mouse/finger! even if you still pressing!
        }
    }
    public override void _PhysicsProcess(float delta) //all same as _Process but if ProcessMode of camera2D set to physics
    {
        if (ProcessMode == Camera2DProcessMode.Physics)
        {
            if (dragFunctionEnabled && isDragging && dragDirection != oldDirection)
            {
                Position += -dragDirection * dragSpeed;
                oldDirection = dragDirection;
            }
        }
    }
    #endregion

    #region handle input
    public override void _UnhandledInput(InputEvent @event)
    {
        if (dragFunctionEnabled && @event is InputEventScreenDrag dragEvent) //catch screen drag input only if [dragFunctionEnabled] = true
        {
            if (!isDragging)
            {
                EmitSignal(nameof(screenDragStarted), dragEvent.Position);
                isDragging = true;
            }

            dragDirection = dragEvent.Relative;
        }

        if (isDragging && @event is InputEventScreenTouch touchEvent && !@event.IsPressed()) //stop drag only if dragging now
        {            
            EmitSignal(nameof(screenDragEnded), touchEvent.Position);
            isDragging = false;
            GetTree().SetInputAsHandled(); //mark input as handled to prevent event forwarding to other touchable/clickable objects under mouse/finger
        }
    }
    #endregion
}
