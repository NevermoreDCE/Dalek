using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StarShips;
using StarShips.Parts;
using StarShips.Interfaces;
using StarShips.Actions;
using System.Xml.Linq;

namespace PartXMLTest
{
    public partial class Form1 : Form
    {
        Ship ship = new Ship();
        XDocument doc;
        string filename = "ShipParts.xml";

        public Form1()
        {
            InitializeComponent();
            InitShip();
        }

        private void InitShip()
        {
            ship = new Ship();
            ship.Name = "Test Ship";
            ship.HP.Max = 50;
            ship.HP.Current = 50;
            ship.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<ShipAction>()));
            ship.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<ShipAction>()));
            ship.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<ShipAction>()));
            ship.Equipment.Add(new WeaponPart("Laser Beam", 1, 5, 2, 0, new List<ShipAction>()));
            List<ShipAction> ShieldGenAction = new List<ShipAction>();
            ShieldGenAction.Add(new RepairThisPart(2));
            ship.Equipment.Add(new DefensePart("Shield Generator", 15, 1, "Down", "Penetrating", ShieldGenAction));
            ship.Equipment.Add(new DefensePart("Shield Generator", 15, 1, "Down", "Penetrating", ShieldGenAction));
            ship.Equipment.Add(new DefensePart("Armor Plate", 15, 3, "Destroyed", "Shattering", new List<ShipAction>()));
            List<ShipAction> DmgControlAction = new List<ShipAction>();
            DmgControlAction.Add(new RepairTargetShip(5));
            ship.Equipment.Add(new ActionPart("Damage Control", 1, "Regen: 5 HPs", DmgControlAction));
            lblShip.Text = ship.Name;
            ShowShipDetails(ship, tlpShip);
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

        private void btnGenerateXML_Click(object sender, EventArgs e)
        {
            generateXML();
        }

        void generateXML()
        {
            doc =
                new XDocument(
                    new XElement("shipParts",
                        new XElement("weaponParts"),
                        new XElement("defenseParts"),
                        new XElement("actionParts")));
            foreach (ShipPart part in ship.Equipment)
                part.GetObjectXML(doc);
            label2.Text = doc.ToString();
        }

        private void btnSaveXML_Click(object sender, EventArgs e)
        {
            if (doc == null)
                generateXML();
            doc.Save(filename);
        }

        private void btnLoadXML_Click(object sender, EventArgs e)
        {
            doc = XDocument.Load(filename);
            label2.Text = doc.ToString();
        }
    }
}
