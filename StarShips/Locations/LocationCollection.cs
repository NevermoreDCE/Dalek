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
    public class LocationCollection : ISerializable
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
        public static double GetDistance(Point a, Point b)
        {
            double sqrX = Math.Pow(a.X - b.X, 2);
            double sqrY = Math.Pow(a.Y - b.Y, 2);
            return Math.Sqrt(sqrX + sqrY);
        }

        public Point GetTargetPointInRadius(Point origin, Point target, double radius, int maxX, int maxY)
        {
            
            if (GetDistance(origin, target) <= radius)
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
            Point next = ship.Position;
            current = ship.Position;
            next = this.GetNextPoint(ship.Position, targetLoc);
            this[current.X, current.Y].Ships.Remove(ship);
            this[next.X, next.Y].Ships.Add(ship);
            ship.Position = next;
            ship.MP.Reduce(1);

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
                if (Math.Round(GetDistance(target, p),0) == radius && (p.X > -1 && p.X <= maxX-1 && p.Y > -1 && p.Y <= maxY-1))
                    inRadius.Add(p);

            double shortestDistance = double.MaxValue;
            Point result = new Point(target.X, target.Y);
            foreach (Point p in inRadius)
            {
                double current = GetDistance(origin, p);
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
                        neighbor.H = Convert.ToInt32(GetDistance(neighbor.Loc, targetLoc));
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
                        newNode.H = Convert.ToInt32(GetDistance(newNode.Loc, targetLoc));
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
    }
}
