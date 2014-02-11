using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StarShips.Interfaces;
using StarShips;
using System.Xml.Linq;
using StarShips.Parts;

namespace PartMaker
{
    public partial class PartMaker : Form
    {
        XDocument doc;
        ThemeSettings themeSettings;
        List<ShipPart> ExistingParts = new List<ShipPart>();
        ShipPart Part;
        List<ShipAction> PartActions = new List<ShipAction>();
        BindingSource bs = new BindingSource();
        string filename = "ShipParts.xml";

        public PartMaker()
        {
            InitializeComponent();
            doc = XDocument.Load(filename);
            // ThemeSettings
            themeSettings = new ThemeSettings(XDocument.Load("ThemeSettings.xml"));

            // Action Grids
            drpWeapPartActionList.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpWeapPartActionList_ItemValueNeeded);
            drpDefPartActionList.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpDefPartActionList_ItemValueNeeded);
            drpActPartActionList.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpActPartActionList_ItemValueNeeded);
            lblWeapPartActionTitle.DataBindings.Add("Text", PartActions, "Name");
            lblDefPartActionTitle.DataBindings.Add("Text", PartActions, "Name");
            lblActPartActionTitle.DataBindings.Add("Text", PartActions, "Name");
            bs.DataSource = PartActions;

            // Existing Parts DDL
            LoadParts();
            
            // Available Action DDLs
            ShowActions();

            // Weapon Tab - Damage Types DDL
            ShowDamageTypes();

            // Weapon Table - Firing Types DDL
            ShowFiringTypes();
        }

        #region ItemValueNeeded
        void drpWeapPartActionList_ItemValueNeeded(object sender, Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventArgs e)
        {
            if (e.ItemIndex < bs.Count && e.ItemIndex >= 0)
            {
                switch (e.Control.Name)
                {
                    case "lblWeapPartActionTitle":
                        e.Value = bs[e.ItemIndex].ToString();
                        break;
                }
            }
        }

        void drpActPartActionList_ItemValueNeeded(object sender, Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventArgs e)
        {
            if (e.ItemIndex < bs.Count && e.ItemIndex >= 0)
            {
                switch (e.Control.Name)
                {
                    case "lblActPartActionTitle":
                        e.Value = bs[e.ItemIndex].ToString();
                        break;
                }
            }
        }

        void drpDefPartActionList_ItemValueNeeded(object sender, Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventArgs e)
        {
            if (e.ItemIndex < bs.Count && e.ItemIndex >= 0)
            {
                switch (e.Control.Name)
                {
                    case "lblDefPartActionTitle":
                        e.Value = bs[e.ItemIndex].ToString();
                        break;
                }
            }
        }
        #endregion

        private void LoadParts()
        {
            ExistingParts = ShipPart.GetShipPartList(doc, new Ship());
            cbxExistingParts.DataSource = ExistingParts;
            cbxExistingParts.DisplayMember = "Name";
        }

        void ShowActions()
        {
            var type = typeof(ShipAction);
            IEnumerable<Type> partListE = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.GetType() != type);
            List<Type> partList = new List<Type>(partListE.Where(t => t != typeof(ShipAction)));
            cbxWeapPartActions.DataSource = partList;
            cbxWeapPartActions.DisplayMember = "Name";
            cbxWeapPartActions.SelectedIndex = 0;
            cbxDefPartActions.DataSource = partList;
            cbxDefPartActions.DisplayMember = "Name";
            cbxDefPartActions.SelectedIndex = 0;
            cbxActPartActions.DataSource = partList;
            cbxActPartActions.DisplayMember = "Name";
            cbxActPartActions.SelectedIndex = 0;
        }

        private void AddAction(ComboBox PartActions, NumericUpDown ActionValue)
        {
            Type newActType = (Type)PartActions.SelectedItem;
            int[] ActionValues = new int[1];
            ActionValues[0] = int.Parse(ActionValue.Value.ToString());
            ShipAction newAct = (ShipAction)Activator.CreateInstance(newActType, ActionValues);
            bs.Add(newAct);
        }

        private void RemoveAction(Microsoft.VisualBasic.PowerPacks.DataRepeater ActionList)
        {
            int index = ActionList.CurrentItemIndex;
            bs.RemoveAt(index);
        }

        private void ShowPartActions(Microsoft.VisualBasic.PowerPacks.DataRepeater ActionList)
        {
            bs.DataSource = PartActions;
            ActionList.DataSource = bs;
            ActionList.BeginResetItemTemplate();
            ActionList.EndResetItemTemplate();
        }

        private void ShowDamageTypes()
        {
            cbxDamageTypes.DataSource = themeSettings.DamageTypes;
            cbxDamageTypes.SelectedIndex = -1;
        }

        private void ShowFiringTypes()
        {
            cbxFiringTypes.DataSource = themeSettings.FiringTypes;
            cbxFiringTypes.SelectedIndex = -1;
        }
        
        #region Add Action
        private void btnActPartActionAdd_Click(object sender, EventArgs e)
        {
            AddAction(cbxActPartActions, nudActPartActionValue);
            ShowPartActions(drpActPartActionList);
        }

        private void btnDefPartActionAdd_Click(object sender, EventArgs e)
        {
            AddAction(cbxDefPartActions, nudDefPartActionValue);
            ShowPartActions(drpDefPartActionList);
        }

        private void btnWeapPartActionAdd_Click(object sender, EventArgs e)
        {
            AddAction(cbxWeapPartActions, nudWeapPartActionValue);
            ShowPartActions(drpWeapPartActionList);
        }
        #endregion

        #region Remove Action
        private void btnWeapPartActionRemove_Click(object sender, EventArgs e)
        {
            RemoveAction(drpWeapPartActionList);
            ShowPartActions(drpWeapPartActionList);
        }

        private void btnDefPartActionRemove_Click(object sender, EventArgs e)
        {
            RemoveAction(drpDefPartActionList);
            ShowPartActions(drpDefPartActionList);
        }

        private void btnActPartActionRemove_Click(object sender, EventArgs e)
        {
            RemoveAction(drpActPartActionList);
            ShowPartActions(drpActPartActionList);
        }
        #endregion

        #region Clear Part
        private void btnActPartClear_Click(object sender, EventArgs e)
        {
            tbxActPartName.Text = string.Empty;
            nudActPartHP.Value = 1;
            nudActPartPointCost.Value = 0;
            bs.Clear();
            ShowPartActions(drpActPartActionList);

            tbxActPartDescr.Text = string.Empty;
        }

        private void btnDefPartClear_Click(object sender, EventArgs e)
        {
            tbxDefPartName.Text = string.Empty;
            nudDefPartHP.Value = 1;
            nudDefPartPointCost.Value = 0;
            bs.Clear();
            ShowPartActions(drpDefPartActionList);
            
            nudDefPartDR.Value = 0;
            tbxDefPartDownAdjective.Text = string.Empty;
            tbxDefPartPenetrateVerb.Text = string.Empty;
        }

        private void btnWeapPartClear_Click(object sender, EventArgs e)
        {
            tbxWeapPartName.Text = string.Empty;
            nudWeapPartHP.Value = 1;
            nudWeapPartPointCost.Value = 0;
            bs.Clear();
            ShowPartActions(drpWeapPartActionList);
            ShowDamageTypes();
            ShowFiringTypes();

            nudWeapPartWeaponDamage.Value = 0;
            nudWeapPartCritMultiplier.Value = 0;
            nudWeapPartReload.Value = 0;
        }
        #endregion

        #region Save Part
        private void btnWeapPartSavePart_Click(object sender, EventArgs e)
        {
            string Name = tbxWeapPartName.Text;
            int HP = int.Parse(nudWeapPartHP.Value.ToString());
            int Dmg = int.Parse(nudWeapPartWeaponDamage.Value.ToString());
            int Crit = int.Parse(nudWeapPartCritMultiplier.Value.ToString());
            int Reload = int.Parse(nudWeapPartReload.Value.ToString());
            string DamageType = cbxDamageTypes.SelectedItem.ToString();
            string FiringType = cbxFiringTypes.SelectedItem.ToString();
            Part = new WeaponPart(new Ship(), Name, HP, Dmg, DamageType, FiringType, Crit, Reload, PartActions);
            
            Part.GetObjectXML(doc);
            doc.Save(filename);
            Part = null;
            LoadParts();
        }

        private void btnDefPartSavePart_Click(object sender, EventArgs e)
        {
            string Name = tbxDefPartName.Text;
            int HP = int.Parse(nudDefPartHP.Value.ToString());
            int DR = int.Parse(nudDefPartDR.Value.ToString());
            string Down = tbxDefPartDownAdjective.Text;
            string Pen = tbxDefPartPenetrateVerb.Text;
            Part = new DefensePart(new Ship(),Name, HP, DR, Down, Pen, PartActions);

            Part.GetObjectXML(doc);
            doc.Save(filename);
            Part = null;
            LoadParts();
        }

        private void btnActPartSavePart_Click(object sender, EventArgs e)
        {
            string Name = tbxActPartName.Text;
            int HP = int.Parse(nudActPartHP.Value.ToString());
            string Descr = tbxActPartDescr.Text;
            Part = new ActionPart(new Ship(), Name, HP, Descr, PartActions);

            Part.GetObjectXML(doc);
            doc.Save(filename);
            Part = null;
            LoadParts();
        }
        #endregion

        #region Load Part
        private void btnLoadPart_Click(object sender, EventArgs e)
        {
            Part = (ShipPart)cbxExistingParts.SelectedItem;
            if (Part is WeaponPart)
                LoadWeaponPart();
            else if (Part is DefensePart)
                LoadDefensePart();
            else if (Part is ActionPart)
                LoadActionPart();

            Part = null;
        }

        private void LoadWeaponPart()
        {
            WeaponPart weap = (WeaponPart)Part;
            tbxWeapPartName.Text = weap.Name;
            nudWeapPartHP.Value = weap.HP.Max;
            if (themeSettings.FiringTypes.Where(f => f == weap.FiringType).Count() > 0)
                cbxFiringTypes.SelectedItem = weap.FiringType;
            else
                cbxFiringTypes.SelectedIndex = -1;
            if (themeSettings.DamageTypes.Where(f => f == weap.DamageType).Count() > 0)
                cbxDamageTypes.SelectedItem = weap.DamageType;
            else
                cbxDamageTypes.SelectedIndex = -1;
            PartActions = weap.Actions;
            ShowPartActions(drpWeapPartActionList); 
            
            nudWeapPartWeaponDamage.Value = weap.WeaponDamage;
            nudWeapPartCritMultiplier.Value = weap.CritMultiplier;
            nudWeapPartReload.Value = weap.ReloadTime;
            tcPartTypes.SelectedTab = tbpWeapon;
        }

        private void LoadDefensePart()
        {
            DefensePart def = (DefensePart)Part;
            tbxDefPartName.Text = def.Name;
            nudDefPartHP.Value = def.HP.Max;
            PartActions = def.Actions;
            ShowPartActions(drpDefPartActionList);

            nudDefPartDR.Value = def.DR;
            tbxDefPartDownAdjective.Text = def.DownAdjective;
            tbxDefPartPenetrateVerb.Text = def.PenetrateVerb;
            tcPartTypes.SelectedTab = tbpDefense;
        }

        private void LoadActionPart()
        {
            ActionPart act = (ActionPart)Part;
            tbxActPartName.Text = act.Name;
            nudActPartHP.Value = act.HP.Max;
            PartActions = act.Actions;
            ShowPartActions(drpActPartActionList);

            tbxActPartDescr.Text = act.Description;
            tcPartTypes.SelectedTab = tbpAction;
        }

        #endregion
    }
}
