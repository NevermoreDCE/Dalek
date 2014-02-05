using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

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

    class LocationCollection
    {
        #region Collection and Indexer
        private Location[,] locations;
        public Location this[int x, int y]
        {
            get
            {
                return locations[x, y];
            }
            set
            {
                locations[x, y] = value;
            }
        }
        #endregion

        #region Private Variables
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        #endregion

        #region Public Methods
        public static double GetDistance(Point a, Point b)
        {
            double sqrX = Math.Pow(a.X - b.X, 2);
            double sqrY = Math.Pow(a.Y - b.Y, 2);
            double rootZ = Math.Sqrt(sqrX + sqrY);
            return Math.Round(rootZ);
        }

        public Point GetNextPoint(Point sourceLoc, Point targetLoc)
        {
            return getNextPoint(sourceLoc, targetLoc);
        }
        #endregion

        #region Private Methods
        private Point getNextPoint(Point sourceLoc, Point targetLoc)
        {
            openList.Clear();
            closedList.Clear();
            GridFacing facing = findFacing(sourceLoc, targetLoc);
            foreach (Node node in getNeighbors(new Node(sourceLoc.X, sourceLoc.Y), (int)facing, targetLoc))
            {
                openList.Add(node);
            }

            List<Point> path = new List<Point>();
            Node previous = new Node();
            while (openList.Count > 0)
            {
                // current
                int indexS = openList.Min(f => f.F);
                Node node = openList.First(f => f.F == indexS && f != previous);
                // check for goal
                if (node.Loc == targetLoc)
                {
                    return recurseNextNode(node).Loc;
                }

                // remove from openset
                openList.Remove(node);

                // add to closedset
                closedList.Add(node);

                List<Node> sortedNeighbors = getNeighbors(node, (int)facing, targetLoc);

                foreach (Node neighbor in sortedNeighbors)
                {
                    // not in closed set or blocked
                    if (closedList.Contains(neighbor))
                        continue;
                    int tempG = node.G + 1;
                    // not already in openset or temp g less than existing g;
                    if (!openList.Any(f => f.Loc == neighbor.Loc && f.G == tempG) || tempG < neighbor.G)
                    {
                        neighbor.Parent = node;
                        neighbor.G = tempG;
                        neighbor.H = int.Parse(GetDistance(neighbor.Loc, targetLoc).ToString());
                        if (!openList.Contains(neighbor) && !closedList.Contains(neighbor))
                            openList.Add(neighbor);
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
                if (newNode.Loc.X >= 0 && newNode.Loc.X < locations.GetLength(0) && newNode.Loc.Y >= 0 && newNode.Loc.Y < locations.GetLength(1))
                {
                    if (!locations[newNode.Loc.X, newNode.Loc.Y].IsBlocked)
                    {
                        newNode.G = node.G + 1;
                        newNode.H = int.Parse(GetDistance(newNode.Loc, targetLoc).ToString());
                        if (!closedList.Contains(newNode))
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
            locations = new Location[x, y];
        }
        #endregion
    }
}
