using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StarShips;
using StarShips.Randomizer;
using StarShips.Interfaces;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using StarShips.Actions;
using StarShips.PartBase;

namespace Dalek
{
    public partial class ShipSim : Form
    {
        Ship Ship1;
        Ship Ship2;
        List<string> results = new List<string>();
        int Round = 1;
        bool victory = false;

        public ShipSim()
        {
            InitializeComponent();
        }

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
                btnFight.Enabled = true;
                btnToTheDeath.Enabled = true;
            }
        }

        private void btnFight_Click(object sender, EventArgs e)
        {
            if(Ship1!=null&&Ship2!=null)
                FightRound();
        }

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
                MessageBox.Show(string.Format("Stalemate in {0} rounds!",Round.ToString()));
                victory = true;
                btnFight.Enabled = false;
                btnToTheDeath.Enabled = false; 
            }
            else if (Ship1.HP.Current <= 0)
            {
                MessageBox.Show(string.Format("Ship 2 Wins in {0} rounds!",Round.ToString()));
                victory = true;
                btnFight.Enabled = false;
                btnToTheDeath.Enabled = false; 
            }
            else if (Ship2.HP.Current <= 0)
            {
                MessageBox.Show(string.Format("Ship 1 Wins in {0} rounds!", Round.ToString()));
                victory = true;
                btnFight.Enabled = false;
                btnToTheDeath.Enabled = false; 
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

        private void btnSelectShip1_Click(object sender, EventArgs e)
        {
            ofdShip1.ShowDialog();
        }

        private void btnSelectShip2_Click(object sender, EventArgs e)
        {
            ofdShip2.ShowDialog();
        }

        private void ofdShip1_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = ofdShip1.FileName;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Ship1 = (Ship)formatter.Deserialize(stream);
            stream.Close();
            gbxShip1.Text = Ship1.Name;
            ShowShipDetails(Ship1, tlpShip1);
            if (Ship1 != null && Ship2 != null)
            {
                btnFight.Enabled = true;
                btnToTheDeath.Enabled = true;
                btnResetShips.Enabled = true;
            }
        }

        private void ofdShip2_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = ofdShip2.FileName;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Ship2 = (Ship)formatter.Deserialize(stream);
            stream.Close();
            gbxShip2.Text = Ship2.Name;
            ShowShipDetails(Ship2, tlpShip2);
            if (Ship1 != null && Ship2 != null)
            {
                btnFight.Enabled = true;
                btnToTheDeath.Enabled = true;
                btnResetShips.Enabled = true;
            }
        }

        private void btnToTheDeath_Click(object sender, EventArgs e)
        {
            if (Ship1 != null && Ship2 != null)
                while (!victory)
                    FightRound();
        }

        private void btnTestShip2_Click(object sender, EventArgs e)
        {
            Ship1 = new Ship();
            Ship1.Name = "Test Ship";
            Ship1.HP.Max = 50;
            Ship1.HP.Current = 50;
            Ship1.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<IShipPartAction>()));
            Ship1.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<IShipPartAction>()));
            Ship1.Equipment.Add(new WeaponPart("Plasma Cannon", 1, 12, 3, 2, new List<IShipPartAction>()));
            Ship1.Equipment.Add(new WeaponPart("Torpedo", 1, 20, 2, 4, new List<IShipPartAction>()));
            List<IShipPartAction> ShieldGenAction = new List<IShipPartAction>();
            ShieldGenAction.Add(new RepairThisPart(2));
            Ship1.Equipment.Add(new DefensePart("Shield Generator", 15, 1, "Down", "Penetrating", ShieldGenAction));
            Ship1.Equipment.Add(new DefensePart("Shield Generator", 15, 1, "Down", "Penetrating", ShieldGenAction));
            Ship1.Equipment.Add(new DefensePart("Armor Plate", 15, 3, "Destroyed", "Shattering", new List<IShipPartAction>()));
            List<IShipPartAction> DmgControlAction = new List<IShipPartAction>();
            DmgControlAction.Add(new RepairTargetShip(5));
            //Ship1.Equipment.Add(new ActionPart("Damage Control", 1, "Regen: 5 HPs", DmgControlAction));
            ShowShipDetails(Ship1, tlpShip1);

            Ship2 = new Ship();
            Ship2.Name = "Test Ship";
            Ship2.HP.Max = 50;
            Ship2.HP.Current = 50;
            Ship2.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<IShipPartAction>()));
            Ship2.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<IShipPartAction>()));
            Ship2.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<IShipPartAction>()));
            Ship2.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<IShipPartAction>()));
            ShieldGenAction = new List<IShipPartAction>();
            ShieldGenAction.Add(new RepairThisPart(2));
            Ship2.Equipment.Add(new DefensePart("Shield Generator", 15,1, "Down", "Penetrating", ShieldGenAction));
            Ship2.Equipment.Add(new DefensePart("Shield Generator", 15,1, "Down", "Penetrating", ShieldGenAction));
            Ship2.Equipment.Add(new DefensePart("Armor Plate", 15, 3, "Destroyed", "Shattering", new List<IShipPartAction>()));
            DmgControlAction = new List<IShipPartAction>();
            DmgControlAction.Add(new RepairTargetShip(5));
            Ship2.Equipment.Add(new ActionPart("Damage Control", 1, "Regen: 5 HPs", DmgControlAction));
            //Ship2.Equipment.Add(new DamageControl());
            ShowShipDetails(Ship2, tlpShip2);

            if (Ship1 != null && Ship2 != null)
            {
                btnFight.Enabled = true;
                btnToTheDeath.Enabled = true;
                btnResetShips.Enabled = true;
            }
        }

        
    }
}
