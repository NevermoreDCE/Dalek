using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using StarShips;
using StarShips.PartBase;
using StarShips.Randomizer;

namespace Dalek
{
    public partial class ShipSim : Form
    {
        List<Ship> ExistingShips = new List<Ship>();
        List<ShipPart> ExistingParts = new List<ShipPart>();
        List<ShipHull> ExistingHulls = new List<ShipHull>();
        Ship Ship1;
        Ship Ship2;
        List<string> results = new List<string>();
        int Round = 1;
        bool victory = false;

        public ShipSim()
        {
            InitializeComponent();
            LoadShips();
            cbxShipList1.SelectedIndexChanged += new EventHandler(cbxShipList1_SelectedIndexChanged);
            cbxShipList2.SelectedIndexChanged += new EventHandler(cbxShipList2_SelectedIndexChanged);
        }

        
        #region Load Parts/Ships
        private void LoadParts()
        {
            XDocument doc = XDocument.Load("ShipParts.xml");
            ExistingParts = ShipPart.GetShipPartList(doc);
        }
        private void LoadHulls()
        {
            XDocument doc = XDocument.Load("ShipHulls.xml");
            ExistingHulls = ShipHull.GetShipHulls(doc);
        }

        void LoadShipList(XDocument shipDoc)
        {
            ExistingShips.Clear();
            foreach (var EShip in shipDoc.Element("ships").Elements())
                ExistingShips.Add(new Ship(EShip, ExistingParts,ExistingHulls));
        }

        private void LoadShips()
        {
            LoadParts();
            LoadHulls();
            XDocument shipsDoc = XDocument.Load("Ships.xml");
            LoadShipList(shipsDoc);
            BindingList<Ship> ships1Source = new BindingList<Ship>(ExistingShips);
            BindingList<Ship> ships2Source = new BindingList<Ship>(ExistingShips);
            cbxShipList1.DataSource = ships1Source;
            cbxShipList2.DataSource = ships2Source;
        }
        #endregion

        #region Buttons/Events
        private void btnResetShips_Click(object sender, EventArgs e)
        {
            if (Ship1 != null && Ship2 != null)
            {
                results = new List<string>();
                Ship1.HP.Current = Ship1.HP.Max;
                foreach (ShipPart part in Ship1.Equipment)
                    part.Repair(int.MaxValue);
                Ship2.HP.Current = Ship2.HP.Max;
                foreach (ShipPart part in Ship2.Equipment)
                    part.Repair(int.MaxValue);

                Round = 1;
                GridBind(new List<string>());

                victory = false;
                btnResetShips.Enabled = false;
                btnFight.Enabled = true;
                btnToTheDeath.Enabled = true;
            }
        }

        private void btnFight_Click(object sender, EventArgs e)
        {
            if (Ship1 != null && Ship2 != null)
                FightRound();
        }

        private void btnToTheDeath_Click(object sender, EventArgs e)
        {
            if (Ship1 != null && Ship2 != null)
            {
                while (!victory)
                    FightRound();
                btnResetShips.Enabled = true;
                btnFight.Enabled = false;
                btnToTheDeath.Enabled = false;
            }
        }

        void cbxShipList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ship1 = (Ship)cbxShipList1.SelectedItem;
            gbxShip1.Text = Ship1.Name;
            ShowShipDetails(Ship1, tlpShip1);
            if (Ship1 != null && Ship2 != null)
            {
                victory = false;
                btnFight.Enabled = true;
                btnToTheDeath.Enabled = true;
                btnResetShips.Enabled = true;
            }
        }

        void cbxShipList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ship2 = (Ship)cbxShipList2.SelectedItem;
            gbxShip2.Text = Ship2.Name;
            ShowShipDetails(Ship2, tlpShip2);
            if (Ship1 != null && Ship2 != null)
            {
                victory = false;
                btnFight.Enabled = true;
                btnToTheDeath.Enabled = true;
                btnResetShips.Enabled = true;
            }
        }

        #endregion

        #region Utility
        private void FightRound()
        {
            List<string> roundResults = new List<string>();
            roundResults.Add(string.Format("-=-=-=-=-=-=-=-=Round {0}: COMBAT=-=-=-=-=-=-=-=-", Round));
            using (RNG rand = new RNG())
            {
                if (rand.d100() > 50)
                {
                    roundResults.Add(string.Format("## Firing {0} ##", Ship1.Name));
                    roundResults.AddRange(Ship1.FireWeapons(Ship2));
                    roundResults.Add(string.Format("## Firing {0} ##", Ship2.Name));
                    roundResults.AddRange(Ship2.FireWeapons(Ship1));
                    roundResults.Add(string.Format("%% Recovering {0} %%", Ship1.Name));
                    roundResults.AddRange(Ship1.EndOfTurn());
                    roundResults.Add(string.Format("%% Recovering {0} %%", Ship2.Name));
                    roundResults.AddRange(Ship2.EndOfTurn());
                }
                else
                {
                    roundResults.Add(string.Format("## Firing {0} ##", Ship2.Name));
                    roundResults.AddRange(Ship2.FireWeapons(Ship1));
                    roundResults.Add(string.Format("## Firing {0} ##", Ship1.Name));
                    roundResults.AddRange(Ship1.FireWeapons(Ship2));
                    roundResults.Add(string.Format("%% Recovering {0} %%", Ship2.Name));
                    roundResults.AddRange(Ship2.EndOfTurn());
                    roundResults.Add(string.Format("%% Recovering {0} %%", Ship1.Name));
                    roundResults.AddRange(Ship1.EndOfTurn());
                }
            }

            roundResults.AddRange(results);
            results = roundResults;
            GridBind(results);

            if (Ship1.HP.Current <= 0 && Ship2.HP.Current <= 0)
            {
                MessageBox.Show(string.Format("Stalemate in {0} rounds!", Round.ToString()));
                victory = true;
                btnFight.Enabled = false;
                btnToTheDeath.Enabled = false;
                btnResetShips.Enabled = true;
            }
            else if (Ship1.HP.Current <= 0)
            {
                MessageBox.Show(string.Format("{0} (Ship 2) Wins in {1} rounds!",Ship2.Name, Round.ToString()));
                victory = true;
                btnFight.Enabled = false;
                btnToTheDeath.Enabled = false;
                btnResetShips.Enabled = true;
            }
            else if (Ship2.HP.Current <= 0)
            {
                MessageBox.Show(string.Format("{0} (Ship 1) Wins in {1} rounds!",Ship1.Name, Round.ToString()));
                victory = true;
                btnFight.Enabled = false;
                btnToTheDeath.Enabled = false;
                btnResetShips.Enabled = true;
            }

            Round++;
        }

        private void GridBind(List<string> results)
        {
            var ds = new BindingList<string>(results);
            lbxResults.DataSource = ds;
            lbxResults.DisplayMember = "Value";

            ShowShipDetails(Ship1, tlpShip1);
            ShowShipDetails(Ship2, tlpShip2);

        }

        private void ShowShipDetails(Ship ship, TableLayoutPanel tlp)
        {
            int rowcount;
            rowcount = 0;
            tlp.Controls.Clear();
            addLabel(string.Format("Current HP: {0}", ship.HP.ToString()), Color.Black, rowcount, tlp);
            rowcount++;
            foreach (ShipPart part in ship.Equipment)
            {
                addLabel(part.ToString(), (part.IsDestroyed ? Color.Red : Color.Black), rowcount, tlp);
                rowcount++;
            }
            addLabel(string.Empty, Color.Black, rowcount, tlp);
        }

        private void addLabel(string text, Color color, int row, TableLayoutPanel tlp)
        {
            Label label = new Label();
            label.Dock = DockStyle.Fill;
            label.Text = text;
            label.ForeColor = color;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            tlp.Controls.Add(label, 0, row);
        }
        #endregion

    }
}
