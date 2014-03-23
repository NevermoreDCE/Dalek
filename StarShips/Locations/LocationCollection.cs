using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using StarShips.Orders;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization;
using StarShips.StarSystems;

namespace StarShips.Locations
{
    enum GridFacing
    {
        NorthEast = 0,
        East = 1,
        SouthEast = 2,
        South = 3,
        SouthWest = 4,
        West = 5,
        NorthWest = 6,
        North = 7

    }
    [Serializable]
    public class LocationCollection : ISerializable, IEnumerable<Location>
    {
        #region Collection and Indexer
        private Location[,] _locations;
        public Location this[int x, int y]
        {
            get
            {
                return _locations[x, y];
            }
            set
            {
                _locations[x, y] = value;
            }
        }
        public int GetLength(int dimension)
        {
            return _locations.GetLength(dimension);
        }
        #endregion

        #region Private Variables
        List<Node> _openList = new List<Node>();
        List<Node> _closedList = new List<Node>();
        #endregion

        #region Public Methods
        public static double GetTacticalDistance(Point a, Point b)
        {
            double sqrX = Math.Pow(a.X - b.X, 2);
            double sqrY = Math.Pow(a.Y - b.Y, 2);
            return Math.Sqrt(sqrX + sqrY);
        }
        public StarSystem GetNextSystem(StarSystem startingSystem, StarSystem targetSystem)
        {
            // Players might call for path to current system
            if (startingSystem == targetSystem)
                return startingSystem;

            // Queue needs to store path-thus-far and current system
            Queue<Tuple<List<StarSystem>, StarSystem>> StarSystemQueue = new Queue<Tuple<List<StarSystem>, StarSystem>>();

            // Need to track visited systems to prevent infinite loops
            List<StarSystem> visitedSystems = new List<StarSystem>();
            // Starting system is already visited
            visitedSystems.Add(startingSystem);

            // For connected systems that we haven't already visited
            foreach (StarSystem system in startingSystem.GetConnectedStarSystems().Where(f => !visitedSystems.Contains(f)))
            {
                List<StarSystem> pathList = new List<StarSystem>();
                pathList.Add(system);
                // Add to visited systems so it's not evaluated in the loop
                visitedSystems.Add(system);
                // Enqueue the path & system
                StarSystemQueue.Enqueue(new Tuple<List<StarSystem>, StarSystem>(pathList, system));
            }
            // Loop til there's an answer or all paths are exausted
            while (StarSystemQueue.Count > 0)
            {
                // Grab current from the queue
                Tuple<List<StarSystem>, StarSystem> currentSystem = StarSystemQueue.Dequeue();

                // If current is the target, return the first system from the path
                if (currentSystem.Item2 == targetSystem)
                    return currentSystem.Item1.First();

                // For connected systems that we haven't already visited
                foreach (StarSystem system in currentSystem.Item2.GetConnectedStarSystems().Where(f => !visitedSystems.Contains(f)))
                {
                    // rebuild path list to prevent changing other paths by reference
                    List<StarSystem> pathList = new List<StarSystem>();
                    foreach (var previous in currentSystem.Item1)
                        pathList.Add(previous);
                    pathList.Add(system); // add new system to path
                    visitedSystems.Add(system); // add new system to visited
                    // Enqueue the path & system
                    StarSystemQueue.Enqueue(new Tuple<List<StarSystem>, StarSystem>(pathList, system));
                }
            }
            // No valid answer at this point, return starting system and handle in outer code
            return startingSystem;
        }
        
        public Point GetTargetPointInRadius(Point origin, Point target, double radius, int maxX, int maxY)
        {
            
            if (GetTacticalDistance(origin, target) <= radius)
                return origin;

            return getTargetPointOnRadius(origin, target, radius, maxX, maxY);
        }

        public Point GetTargetPointOnRadius(Point origin, Point target, double radius, int maxX, int maxY)
        {
            return getTargetPointOnRadius(origin, target, radius, maxX, maxY);
        }
        
        public Point GetNextPoint(Point sourceLoc, Point targetLoc)
        {
            return getNextPoint(sourceLoc, targetLoc);
        }

        public Point MoveShipToPoint(Ship ship, Point targetLoc)
        {
            Point current;
            Point next = ship.TacticalPosition;
            current = ship.TacticalPosition;
            next = this.GetNextPoint(ship.TacticalPosition, targetLoc);
            this[current.X, current.Y].Ships.Remove(ship);
            this[next.X, next.Y].Ships.Add(ship);
            ship.TacticalPosition = next;
            ship.MP.Reduce(1);

            return next;
        }
        public Point WarpShipToSystem(Ship ship, StarSystem nextSystem)
        {
            StarSystem previousSystem = ship.StrategicSystem;
            Point next = nextSystem.GetWarpPointPosition(previousSystem);
            this[ship.StrategicPosition.X, ship.StrategicPosition.Y].Ships.Remove(ship);
            nextSystem.StrategicLocations[next.X, next.Y].Ships.Add(ship);
            ship.StrategicPosition = next;
            ship.StrategicSystem = nextSystem;
            // Warping does not cost MP

            return next;
        }
        #endregion

        #region Private Methods
        private Point getTargetPointOnRadius(Point origin, Point target, double radius, int maxX, int maxY)
        {
            // figure out dimension of target matrix: radius plus target square
            int dimension = int.Parse((radius * 2).ToString()) + 1;
            // figure out zero point of target matrix: target minus radius
            Point zero = new Point(int.Parse((target.X - radius).ToString()), int.Parse((target.Y - radius).ToString()));
            // create and populate target matrix
            Point[,] matrix = new Point[dimension, dimension];
            for (int x = 0; x < dimension; x++)
            {
                for (int y = 0; y < dimension; y++)
                {
                    matrix[x, y] = new Point(zero.X + x, zero.Y + y);
                }
            }
            // get all points in radius
            List<Point> inRadius = new List<Point>();
            foreach (Point p in matrix)
                if (Math.Round(GetTacticalDistance(target, p),0) == radius && (p.X > -1 && p.X <= maxX-1 && p.Y > -1 && p.Y <= maxY-1))
                    inRadius.Add(p);

            double shortestDistance = double.MaxValue;
            Point result = new Point(target.X, target.Y);
            foreach (Point p in inRadius)
            {
                double current = GetTacticalDistance(origin, p);
                if (current < shortestDistance && !_locations[p.X,p.Y].IsBlocked)
                {
                    shortestDistance = current;
                    result = p;
                }
            }

            return result;
        }

        private Point getNextPoint(Point sourceLoc, Point targetLoc)
        {
            // clear the lists
            _openList.Clear();
            _closedList.Clear();
            // find direction of targetLoc
            GridFacing facing = findFacing(sourceLoc, targetLoc);
            // get starting adjacent locations
            foreach (Node node in getNeighbors(new Node(sourceLoc.X, sourceLoc.Y), (int)facing, targetLoc))
            {
                _openList.Add(node);
            }

            Node previous = new Node();
            while (_openList.Count > 0)
            {
                // current lowest G+H
                int indexS = _openList.Min(f => f.F);
                // get next node with lowest G+H that is not the previous node
                Node node = _openList.First(f => f.F == indexS && f != previous);
                // check for goal
                if (node.Loc == targetLoc)
                {
                    return recurseNextNode(node).Loc;
                }
                // remove from openset
                _openList.Remove(node);
                // add to closedset
                _closedList.Add(node);
                // get neighbors of current location
                List<Node> sortedNeighbors = getNeighbors(node, (int)facing, targetLoc);
                foreach (Node neighbor in sortedNeighbors)
                {
                    // not in closed set
                    if (_closedList.Contains(neighbor))
                        continue;
                    // steps from source
                    int tempG = node.G + 1;
                    // not already in openset or temp g less than existing g;
                    if (!_openList.Any(f => f.Loc == neighbor.Loc && f.G <= tempG) )//|| tempG < neighbor.G)
                    {
                        neighbor.Parent = node;
                        neighbor.G = tempG;
                        neighbor.H = Convert.ToInt32(GetTacticalDistance(neighbor.Loc, targetLoc));
                        if (!_openList.Contains(neighbor) && !_closedList.Contains(neighbor))
                            _openList.Add(neighbor);
                    }
                }
                previous = node;
            }

            return new Point(0, 0);
        }

        private Node recurseNextNode(Node node)
        {
            Node result = node;
            if (node.Parent != null)
            {
                result = recurseNextNode(node.Parent);
            }
            return result;
        }

        private GridFacing findFacing(Point a, Point b)
        {
            if (a.X > b.X)
            {
                if (a.Y > b.Y)
                    return GridFacing.NorthWest;
                else if (a.Y < b.Y)
                    return GridFacing.SouthWest;
                else
                    return GridFacing.West;
            }
            else if (a.X < b.X)
            {
                if (a.Y > b.Y)
                    return GridFacing.NorthEast;
                else if (a.Y < b.Y)
                    return GridFacing.SouthEast;
                else
                    return GridFacing.East;
            }
            else
            {
                if (a.Y > b.Y)
                    return GridFacing.North;
                else
                    return GridFacing.South;
            }
        }

        private List<Node> getNeighbors(Node node, int offset, Point targetLoc)
        {
            List<Node> results = new List<Node>();
            for (int i = 0; i < 8; i++)
            {
                int val = i + offset;
                if (val > 7)
                    val = val - 8;
                Node newNode = findNeighborInDirection(node, (GridFacing)Enum.Parse(typeof(GridFacing), (val).ToString()));
                if (newNode.Loc.X >= 0 && newNode.Loc.X < _locations.GetLength(0) && newNode.Loc.Y >= 0 && newNode.Loc.Y < _locations.GetLength(1))
                {
                    if (!_locations[int.Parse(newNode.Loc.X.ToString()), int.Parse(newNode.Loc.Y.ToString())].IsBlocked)
                    {
                        newNode.G = node.G + 1;
                        newNode.H = Convert.ToInt32(GetTacticalDistance(newNode.Loc, targetLoc));
                        newNode.Depth = node.Depth + 1;
                        if (!_closedList.Contains(newNode))
                            results.Add(newNode);
                    }
                }
            }

            return results;
        }

        private Node findNeighborInDirection(Node source, GridFacing direction)
        {
            Node result = new Node();
            switch (direction)
            {
                case GridFacing.NorthEast:
                    result = new Node(source.Loc.X + 1, source.Loc.Y - 1);
                    break;
                case GridFacing.East:
                    result = new Node(source.Loc.X + 1, source.Loc.Y);
                    break;
                case GridFacing.SouthEast:
                    result = new Node(source.Loc.X + 1, source.Loc.Y + 1);
                    break;
                case GridFacing.North:
                    result = new Node(source.Loc.X, source.Loc.Y - 1);
                    break;
                case GridFacing.South:
                    result = new Node(source.Loc.X, source.Loc.Y + 1);
                    break;
                case GridFacing.NorthWest:
                    result = new Node(source.Loc.X - 1, source.Loc.Y - 1);
                    break;
                case GridFacing.West:
                    result = new Node(source.Loc.X - 1, source.Loc.Y);
                    break;
                case GridFacing.SouthWest:
                    result = new Node(source.Loc.X - 1, source.Loc.Y + 1);
                    break;
            }
            return result;
        }
        #endregion

        #region Constructors
        public LocationCollection(int x, int y)
        {
            _locations = new Location[x, y];
        }
        public LocationCollection(SerializationInfo info, StreamingContext context)
        {
            _locations = (Location[,])info.GetValue("Locations", typeof(Location[,]));
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Locations", this._locations);
        }
        #endregion

        #region IEnumerable Methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public LocColEnum GetEnumerator()
        {
            return new LocColEnum(_locations);
        }

        IEnumerator<Location> IEnumerable<Location>.GetEnumerator()
        {
            return (IEnumerator<Location>)GetEnumerator();
        }
        #endregion

        
    }

    public class LocColEnum : IEnumerator<Location>
    {
        public Location[,] _locations;

        int positionX = 0;
        int positionY = 0;

        public LocColEnum(Location[,] locations)
        {
            _locations = locations;
        }
        public LocColEnum()
        {

        }
        object IEnumerator.Current
        {
            get { return Current; }
        }
        public Location Current
        {
            get
            {
                try
                {
                    return (Location)_locations[positionX,positionY];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            if (positionY + 1 >= _locations.GetLength(1))
            {
                positionY = 0;
                positionX++;
            }
            else
                positionY++;
            return (positionX < _locations.GetLength(0));
        }

        public void Reset()
        {
            positionX = 0;
            positionY = 0;
        }

        public void Dispose()
        {
            _locations = null;
        }
    }
}
