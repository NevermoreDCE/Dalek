using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StarShips.Randomizer;

namespace PathfindingTest
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
    public partial class Form1 : Form
    {
        Location[,] locations = new Location[10, 10];
        Ship Hunter;
        Point hunterOrigin;
        Ship Prey;
        Point preyOrigin;

        public Form1()
        {
            InitializeComponent();
            initShips();
            initLocations();
            displayLocations();
        }

        private void initShips()
        {
            Hunter = new Ship("Hunter");
            Prey = new Ship("Prey");
        }

        private void initLocations()
        {
            Location newLoc;
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    using (RNG rng = new RNG())
                    {
                        newLoc = new Location();
                        if (rng.d100() > 85)
                            newLoc.IsBlocked = true;
                        newLoc.Name = string.Format("Loc {0},{1}{2}",
                            x.ToString(),
                            y.ToString(),
                            (newLoc.IsBlocked ? Environment.NewLine + " (Blocked)" : string.Empty));
                        locations[x, y] = newLoc;
                    }
                }
            }
            using (RNG rng = new RNG())
            {
                bool validHunter = false;
                int randX=0;
                int randY = 0;
                while (!validHunter)
                {
                    randX = rng.d(9);
                    randY = rng.d(9);
                    if (!locations[randX, randY].IsBlocked)
                        validHunter = true;
                }
                locations[randX, randY].Ships.Add(Hunter);
                hunterOrigin = new Point(randX, randY);
                Hunter.Loc = new Point(randX, randY);

                bool validLoc = false;
                int newX=0;
                int newY=0;
                while (!validLoc)
                {
                    newX = rng.d(9);
                    newY = rng.d(9);

                    if ((Math.Abs(randX - newX) >= 5 || Math.Abs(randY - newY) >= 5) && !locations[newX,newY].IsBlocked)
                        validLoc = true;
                }
                locations[newX, newY].Ships.Add(Prey);
                preyOrigin = new Point(newX, newY);
                Prey.Loc = new Point(newX, newY);
            }

        }

        private void displayLocations()
        {
            grpField.Controls.Clear();
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    locations[x, y].Search = string.Empty;
                    SetupLabel(x, y);
                    grpField.Controls.Add(locations[x, y].label);
                }
            }
        }

        private void SetupLabel(int x, int y)
        {
            SetupLabel(x, y, false, false);
        }
        private void SetupLabel(int x, int y, bool refresh)
        {
            SetupLabel(x, y, refresh,false);
        }
        private void SetupLabel(int x, int y, bool refresh, bool pathPart)
        {
            locations[x, y].label.Text = locations[x, y].Name+(!string.IsNullOrEmpty(locations[x,y].Search)?Environment.NewLine+locations[x,y].Search:string.Empty);
            locations[x, y].label.Width = 64;
            locations[x, y].label.Height = 64;
            foreach (Ship ship in locations[x, y].Ships)
                locations[x, y].label.Text += Environment.NewLine + ship.Name;
            locations[x, y].label.Location = new Point(x * 64 + 6, y * 64 + 16);
            if (pathPart)
                locations[x, y].label.ForeColor = Color.Green;
            else if (locations[x, y].Ships.Contains(Hunter))
                locations[x, y].label.ForeColor = Color.Red;
            else if (locations[x, y].Ships.Contains(Prey))
                locations[x, y].label.ForeColor = Color.Blue;
            else locations[x, y].label.ForeColor = Color.Black;
            if (locations[x, y].IsBlocked)
                locations[x, y].label.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Bold);
            
            if(refresh)
                locations[x,y].label.Refresh();
        }

        private int GetDistance(Point a, Point b)
        {
            double sqrX = Math.Pow(a.X - b.X, 2);
            double sqrY = Math.Pow(a.Y - b.Y, 2);
            double rootZ = Math.Sqrt(sqrX + sqrY);
            double roundZ = Math.Round(rootZ);
            return int.Parse(roundZ.ToString());
        }

        List<Node> OpenList = new List<Node>();
        List<Node> ClosedList = new List<Node>();

        private void FindPath()
        {
            while (Hunter.Loc != Prey.Loc)
            {
                FindPathOneStep();
            }
        }

        private void FindPathOneStep()
        {
            Point current = Hunter.Loc;
            Point next = GetNextPoint(Hunter.Loc, Prey.Loc);

            locations[current.X, current.Y].Ships.Remove(Hunter);
            locations[next.X, next.Y].Ships.Add(Hunter);
            Hunter.Loc = next;

            lblPathTaken.Text += Environment.NewLine + string.Format("{0}, {1}", next.X, next.Y);
            
            SetupLabel(current.X, current.Y, true, true);
            SetupLabel(next.X, next.Y, true, true);

            lblPathTaken.Refresh();
        }

        private Point GetNextPoint(Point hunterLoc, Point preyLoc)
        {
            OpenList.Clear();
            ClosedList.Clear();
            GridFacing facing = findFacing(hunterLoc,preyLoc);
            foreach (Node node in Neighbors(new Node(hunterLoc.X, hunterLoc.Y), (int)facing))
            {
                OpenList.Add(node);
            }
            
            List<Point> path = new List<Point>();
            Node previous = new Node();
            while (OpenList.Count > 0)
            {
                // current
                int indexS = OpenList.Min(f => f.F);
                Node node = OpenList.First(f => f.F == indexS && f!=previous);
                // check for goal
                if (node.Loc == preyLoc)
                {
                    return RecurseNextNode(node).Loc;
                }
                
                // remove from openset
                OpenList.Remove(node);
                
                // add to closedset
                ClosedList.Add(node);

                List<Node> sortedNeighbors = Neighbors(node,(int)facing);

                foreach (Node neighbor in sortedNeighbors)
                {
                    // not in closed set or blocked
                    if (ClosedList.Contains(neighbor))
                        continue;
                    int tempG = node.G + 1;
                    // not already in openset or temp g less than existing g;
                    if (!OpenList.Any(f => f.Loc == neighbor.Loc && f.G == tempG) || tempG < neighbor.G)
                    {
                        neighbor.Parent = node;
                        neighbor.G = tempG;
                        neighbor.H = GetDistance(neighbor.Loc, preyLoc);
                        locations[neighbor.Loc.X, neighbor.Loc.Y].Search = neighbor.ToString();
                        SetupLabel(neighbor.Loc.X, neighbor.Loc.Y, true);
                        if (!OpenList.Contains(neighbor) && !ClosedList.Contains(neighbor))
                            OpenList.Add(neighbor);
                    }
                }
                previous = node;
            }
            
            return new Point(0,0);
        }

        private Node RecurseNextNode(Node node)
        {
            Node result = node;
            if (node.Parent != null)
            {
                result = RecurseNextNode(node.Parent);
            }
            return result;
        }

        GridFacing findFacing(Point a, Point b)
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

        private List<Node> Neighbors(Node node, int offset)
        {
            List<Node> results = new List<Node>();
            for (int i = 0; i < 8; i++)
            {
                int val = i + offset;
                if (val > 7)
                    val = val - 8;
                Node newNode = FindNeighborInDirection(node, (GridFacing)Enum.Parse(typeof(GridFacing), (val).ToString()));
                if (newNode.Loc.X >= 0 && newNode.Loc.X < locations.GetLength(0) && newNode.Loc.Y >= 0 && newNode.Loc.Y < locations.GetLength(1))
                {
                    if (!locations[newNode.Loc.X, newNode.Loc.Y].IsBlocked)
                    {
                        newNode.G = node.G + 1;
                        newNode.H = GetDistance(newNode.Loc, Prey.Loc);
                        if (!ClosedList.Contains(newNode))
                            results.Add(newNode);
                    }
                }
            }
            
            return results;
        }

        private Node FindNeighborInDirection(Node source, GridFacing direction)
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

        #region Buttons
        private void btnAgain_Click(object sender, EventArgs e)
        {
            FindPathOneStep();
        }

        private void btnNewMap_Click(object sender, EventArgs e)
        {
            initLocations();
            displayLocations();
            lblPathTaken.Text = "Path Taken:";
        }

        private void btnResetShips_Click(object sender, EventArgs e)
        {
            locations[Hunter.Loc.X, Hunter.Loc.Y].Ships.Remove(Hunter);
            locations[hunterOrigin.X, hunterOrigin.Y].Ships.Add(Hunter);
            Hunter.Loc = hunterOrigin;
            displayLocations();
            lblPathTaken.Text = "Path Taken:";
        }

        private void btnAllSteps_Click(object sender, EventArgs e)
        {
            lblPathTaken.Text = "Path Taken:";
            FindPath();
        }
        #endregion
    }

    class Location
    {
        public string Name;
        public string Search;
        public bool IsBlocked = false;
        public List<Ship> Ships = new List<Ship>();
        public Label label = new Label();
    }
    class Node: IComparable<Node>
    {
        public Point Loc;
        public int F { get { return G + H; } }
        public int G;
        public int H;
        public Node Parent;
        public Node()
        {
        }
        public Node(int x, int y)
        {
            this.Loc = new Point(x, y);
        }

        public int CompareTo(Node other)
        {
            if (this.Loc.X > other.Loc.X || this.Loc.Y > other.Loc.Y) return 1;
            if (this.Loc.X == other.Loc.X && this.Loc.Y==other.Loc.Y) return 0;
            return -1;
        }
        public override string ToString()
        {
            return string.Format("{0},{1}({2},{3},{4})", Loc.X, Loc.Y, this.H, this.G, this.F);
        }
    }
    class Ship
    {
        public string Name;
        public Point Loc;
        public Ship(string name)
        {
            this.Name = name;
        }
    }
}
