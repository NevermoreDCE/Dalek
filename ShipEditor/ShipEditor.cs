using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using StarShips;
using StarShips.Parts;

namespace ShipEditor
{
    public partial class ShipEditor : Form
    {
        List<Ship> ExistingShips = new List<Ship>();
        List<ShipHull> ExistingHulls = new List<ShipHull>();
        Ship ship = new Ship();
        BindingSource bsShipParts = new BindingSource();
        List<ShipPart> ExistingParts = new List<ShipPart>();
        //string defaultShipPartsFileName = "ShipParts.xml";
        XDocument shipDoc = new XDocument(new XElement("ships"));
        string currentShipDocFileName = "Empires\\Default\\Ships.xml";
        
        public ShipEditor()
        {
            InitializeComponent();
            drpPartList.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpPartList_ItemValueNeeded);
            cbxShipHullTypes.SelectedIndexChanged += new EventHandler(cbxShipHullTypes_SelectedIndexChanged);
            tbxShipName.TextChanged += new EventHandler(tbxShipName_TextChanged);
            lblPartName.DataBindings.Add("Text", ship.Equipment, "Name");
            bsShipParts.DataSource = ship.Equipment;
            LoadHulls();
            LoadParts();
            shipDoc = XDocument.Load(currentShipDocFileName);
            LoadShipList(shipDoc);
            ShowShipList();
            ShowShip();
        }

        

        private void LoadHulls()
        {
            XDocument doc = XDocument.Load("ShipHulls.xml");
            ExistingHulls = ShipHull.GetShipHulls(doc);
            ExistingHulls.Sort(ShipHull.HullComparer);
            cbxShipHullTypes.DataSource = ExistingHulls;
            cbxShipHullTypes.DisplayMember = "Name";
            cbxShipHullTypes.SelectedIndex = -1;
        }

        private void LoadParts()
        {
            XDocument doc = XDocument.Load("ShipParts.xml");
            ExistingParts = ShipPart.GetShipPartList(doc, new Ship());
            cbxPartList.DataSource = ExistingParts;
            cbxPartList.DisplayMember = "Name";
            cbxPartList.SelectedIndex = -1;
        }

        void tbxShipName_TextChanged(object sender, EventArgs e)
        {
            ship.ClassName = tbxShipName.Text;
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

        void cbxShipHullTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxShipHullTypes.SelectedIndex >= 0)
            {
                ShipHull hull = (ShipHull)cbxShipHullTypes.SelectedItem;
                ship.HullType = hull;
            }
        }

        void LoadShipList(XDocument shipDoc)
        {
            ExistingShips.Clear();
            if(shipDoc.Element("ships").Elements().Count()>0)
            foreach (var EShip in shipDoc.Element("ships").Elements())
                ExistingShips.Add(new Ship(EShip, ExistingParts, ExistingHulls, new StarShips.Players.Player("None","No Empire","Default",false,0)));
        }

        private void ShowShipList()
        {
            BindingList<Ship> bsShips = new BindingList<Ship>(ExistingShips);
            cbxShipList.DataSource = bsShips;
            cbxShipList.DisplayMember = "ClassName";
            if (bsShips.Count < 1)
                cbxShipList.Text = string.Empty;
        }

        private void ShowShip()
        {
            tbxShipName.Text = ship.ClassName;
            if (ship.HullType != null)
            {
                int i = -1;
                foreach (var item in cbxShipHullTypes.Items)
                    if (((ShipHull)item).Name == ship.HullType.Name)
                        i = cbxShipHullTypes.Items.IndexOf(item);
                cbxShipHullTypes.SelectedIndex = i;
            }
            bsShipParts.DataSource = ship.Equipment;
            drpPartList.DataSource = bsShipParts;
            drpPartList.BeginResetItemTemplate();
            drpPartList.EndResetItemTemplate();
            if (ship.HullType.Mass > 0)
            {
                lblShipMass.Text = string.Format("Mass : {0}", ship.TotalMass.ToString());
                lblShipMP.Text = string.Format("MP: {0}",ship.GetMaxMP().ToString());
            }
        }

        #region Add/Remove Part
        private void btnDeletePart_Click(object sender, EventArgs e)
        {
            int index = drpPartList.CurrentItemIndex;
            bsShipParts.RemoveAt(index);
            drpPartList.BeginResetItemTemplate();
            drpPartList.EndResetItemTemplate();
            if (ship.HullType.Mass > 0)
            {
                lblShipMass.Text = string.Format("Mass : {0}", ship.TotalMass.ToString());
                lblShipMP.Text = string.Format("MP: {0}", ship.GetMaxMP().ToString());
            }
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            bool allowedPart = true;
            string message = string.Empty;
            ShipHull selectedHull = (ShipHull)cbxShipHullTypes.SelectedItem;
            ShipPart selectedPart = (ShipPart)cbxPartList.SelectedItem;
            
            object si = cbxPartList.SelectedItem;
            if (si is WeaponPart)
            {
                if (selectedHull.AllowedParts.Any(f => f.PartType == typeof(WeaponPart)))
                {
                    // is weapon, check for firing type match
                    if (selectedHull.AllowedParts.Any(f => f.PartType == typeof(WeaponPart) && f.ActionMechanism == ((WeaponPart)si).FiringType))
                    {
                        PartCount pc = selectedHull.AllowedParts.First(f => f.PartType == typeof(WeaponPart) && f.ActionMechanism == ((WeaponPart)si).FiringType);
                        if (ship.Equipment.Count(f => f is WeaponPart && ((WeaponPart)f).FiringType == pc.ActionMechanism) >= pc.CountOfParts)
                        {
                            allowedPart = false;
                            message = string.Format("Cannot add Weapon with Firing Type {0}, you already have the maximum {1} of those.", pc.ActionMechanism, pc.CountOfParts);
                        }

                    }
                    else // is weapon, but no firing type match, check for generic weapon limit
                    {
                        PartCount pc = selectedHull.AllowedParts.First(f => f.PartType == typeof(WeaponPart) && f.ActionMechanism == string.Empty);
                        if (ship.Equipment.Count(f => f is WeaponPart) >= pc.CountOfParts)
                        {
                            allowedPart = false;
                            message = string.Format("Cannot add Weapon, you already have the maximum {0} of those.", pc.CountOfParts);
                        }
                    }
                }

            }
            else if (si is DefensePart)
            {
                if (selectedHull.AllowedParts.Any(f => f.PartType == typeof(DefensePart)))
                {
                    PartCount pc = selectedHull.AllowedParts.First(f => f.PartType == typeof(DefensePart));
                    if (ship.Equipment.Count(f => f is DefensePart) >= pc.CountOfParts)
                    {
                        allowedPart = false;
                        message = string.Format("Cannot add Defense, you already have the maximum {0} of those.", pc.CountOfParts);
                    }
                }
            }
            else if (si is ActionPart)
            {
                if (selectedHull.AllowedParts.Any(f => f.PartType == typeof(ActionPart)))
                {
                    PartCount pc = selectedHull.AllowedParts.First(f => f.PartType == typeof(ActionPart));
                    if (ship.Equipment.Count(f => f is ActionPart) >= pc.CountOfParts)
                    {
                        allowedPart = false;
                        message = string.Format("Cannot add Action, you already have the maximum {0} of those.", pc.CountOfParts);
                    }
                }
            }
            else if (si is EnginePart)
            {
                if (selectedHull.AllowedParts.Any(f => f.PartType == typeof(EnginePart)))
                {
                    PartCount pc = selectedHull.AllowedParts.First(f => f.PartType == typeof(EnginePart));
                    if (ship.Equipment.Count(f => f is EnginePart) >= pc.CountOfParts)
                    {
                        allowedPart = false;
                        message = string.Format("Cannot add Engine, you already have the maximum {0} of those.", pc.CountOfParts);
                    }
                }
            }

            if(allowedPart)
                bsShipParts.Add(cbxPartList.SelectedItem);
            else
                MessageBox.Show(message);

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
                XElement ElementToDelete = shipDoc.Descendants("ship").First(f => f.Attribute("name").Value == ShipToDelete.ClassName);
                ElementToDelete.Remove();
                LoadShipList(shipDoc);
                ShowShipList();
            }
        }

        private void btnSaveShip_Click(object sender, EventArgs e)
        {
            ship.ClassName = tbxShipName.Text;
            ship.HullType = ((ShipHull)cbxShipHullTypes.SelectedItem).Clone();
            ship.MP.Max = ship.GetMaxMP();
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
