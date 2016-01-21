/// <summary>
///  Node for the A* algorithm.
/// </summary>
public class AStarNode
{
    // the position it represents
    public MapPosition position;
    // the parent node of this position (the step in the path before this one)
    public MapPosition parent = null;
    // the A* numbers
    public float f;
    public float g;

    public int terrainCost;

    /// <summary>
    /// Initializes a new instance of the <see cref="AStarNode"/> class.
    /// </summary>
    /// <param name="_position">The position in the map this node represents</param>
    /// <param name="_f">The sum of g and the estimate to get to the goal node from this node</param>
    /// <param name="_g">The cost to get to this node from the start of the path</param>
    public AStarNode(MapPosition _position, float _f = 1.0f, float _g = 0.0f)
    {
        position = _position;
        f = _f;
        g = _g;
    }
}