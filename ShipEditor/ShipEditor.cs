using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using StarShips;
using StarShips.PartBase;

namespace ShipEditor
{
    public partial class ShipEditor : Form
    {
        List<Ship> ExistingShips = new List<Ship>();
        Ship ship = new Ship();
        BindingSource bsShipParts = new BindingSource();
        List<ShipPart> ExistingParts = new List<ShipPart>();
        string defaultShipPartsFileName = "ShipParts.xml";
        XDocument shipDoc = new XDocument(new XElement("ships"));
        string currentShipDocFileName;
        
        public ShipEditor()
        {
            InitializeComponent();
            drpPartList.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpPartList_ItemValueNeeded);
            tbxShipName.TextChanged += new EventHandler(tbxShipName_TextChanged);
            nudHitpoints.ValueChanged += new EventHandler(nudHitpoints_ValueChanged);
            lblPartName.DataBindings.Add("Text", ship.Equipment, "Name");
            bsShipParts.DataSource = ship.Equipment;
            LoadParts();
            ShowShipList();
            ShowShip();
        }

        private void LoadParts()
        {
            ExistingParts.Clear();
            XDocument doc = XDocument.Load(defaultShipPartsFileName);
            XElement weaponParts = doc.Element("shipParts").Element("weaponParts");
            foreach (XElement weaponPart in weaponParts.Elements())
                ExistingParts.Add(new WeaponPart(weaponPart));
            XElement defenseParts = doc.Element("shipParts").Element("defenseParts");
            foreach (XElement defensePart in defenseParts.Elements())
                ExistingParts.Add(new DefensePart(defensePart));
            XElement actionParts = doc.Element("shipParts").Element("actionParts");
            foreach (XElement actionPart in actionParts.Elements())
                ExistingParts.Add(new ActionPart(actionPart));
            cbxPartList.DataSource = ExistingParts;
            cbxPartList.DisplayMember = "Name";
        }

        void nudHitpoints_ValueChanged(object sender, EventArgs e)
        {
            ship.HP.Max = int.Parse(nudHitpoints.Value.ToString());
            ship.HP.Current = int.Parse(nudHitpoints.Value.ToString());
        }

        void tbxShipName_TextChanged(object sender, EventArgs e)
        {
            ship.Name = tbxShipName.Text;
        }

        void drpPartList_ItemValueNeeded(object sender, Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventArgs e)
        {
            if (e.ItemIndex < ship.Equipment.Count && e.ItemIndex>=0)
            {
                switch (e.Control.Name)
                {
                    case "lblPartName":
                        e.Value = bsShipParts[e.ItemIndex].ToString();
                        break;
                }
            }
        }

        void LoadShipList(XDocument shipDoc)
        {
            ExistingShips.Clear();
            foreach (var EShip in shipDoc.Element("ships").Elements())
                ExistingShips.Add(new Ship(EShip, ExistingParts));
        }

        private void ShowShipList()
        {
            BindingList<Ship> bsShips = new BindingList<Ship>(ExistingShips);
            cbxShipList.DataSource = bsShips;
            cbxShipList.DisplayMember = "Name";
            if (bsShips.Count < 1)
                cbxShipList.Text = string.Empty;
        }

        private void ShowShip()
        {
            tbxShipName.Text = ship.Name;
            nudHitpoints.Value = ship.HP.Max;
            bsShipParts.DataSource = ship.Equipment;
            drpPartList.DataSource = bsShipParts;
            drpPartList.BeginResetItemTemplate();
            drpPartList.EndResetItemTemplate();
        }

        #region Add/Remove Part
        private void btnDeletePart_Click(object sender, EventArgs e)
        {
            int index = drpPartList.CurrentItemIndex;
            bsShipParts.RemoveAt(index);
            drpPartList.BeginResetItemTemplate();
            drpPartList.EndResetItemTemplate();
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            bsShipParts.Add(cbxPartList.SelectedItem);
            ShowShip();
        }
        #endregion

        #region New/Save/Load From File

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdOpen.ShowDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfdSave.FileName = "Ships.xml";
            sfdSave.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentShipDocFileName == string.Empty)
            {
                sfdSave.FileName = "Ships.xml";
                sfdSave.ShowDialog();
            }
            shipDoc.Save(currentShipDocFileName);
        }

        private void sfdSave_FileOk(object sender, CancelEventArgs e)
        {
            foreach(Ship EShip in ExistingShips)
                EShip.GetObjectXML(shipDoc);
            shipDoc.Save(sfdSave.FileName);
        }

        private void ofdOpen_FileOk(object sender, CancelEventArgs e)
        {
            currentShipDocFileName = ofdOpen.FileName;
            shipDoc = XDocument.Load(ofdOpen.FileName);
            LoadShipList(shipDoc);
            ShowShipList();
            ship = new Ship();
            bsShipParts.DataSource = ship.Equipment;
            ShowShip();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExistingShips = new List<Ship>();
            ship = new Ship();
            ShowShip();
        }

        #endregion

        #region Clear/Save Current/Load Selected
        private void btnLoadShip_Click(object sender, EventArgs e)
        {
            ship = (Ship)cbxShipList.SelectedItem;
            ShowShip();
        }

        private void btnDeleteShip_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this Ship ??", "Confirm Delete!!", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                Ship ShipToDelete = (Ship)cbxShipList.SelectedItem;
                XElement ElementToDelete = shipDoc.Descendants("ship").First(f => f.Attribute("name").Value == ShipToDelete.Name);
                ElementToDelete.Remove();
                LoadShipList(shipDoc);
                ShowShipList();
            }
        }

        private void btnSaveShip_Click(object sender, EventArgs e)
        {
            ship.Name = tbxShipName.Text;
            ship.HP.Max = int.Parse(nudHitpoints.Value.ToString());
            ship.GetObjectXML(shipDoc);
            LoadShipList(shipDoc);
            ShowShipList();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ship = new Ship();
            ShowShip();
        }
        #endregion

        

        

        

        
    }
}
