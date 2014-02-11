using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StarShips;
using System.Xml.Linq;
using StarShips.Parts;

namespace HullMaker
{
    public partial class HullMaker : Form
    {
        #region Properties
        List<ShipHull> ExistingHulls = new List<ShipHull>();
        XDocument hullsDoc;
        ShipHull hull = new ShipHull();
        List<PartCount> PartCounts = new List<PartCount>();
        BindingSource bsPartLimits = new BindingSource();
        List<Type> PartTypes = new List<Type>();
        List<string> WeaponPartFiringTypes = new List<string>();
        ThemeSettings ts;
        #endregion

        public HullMaker()
        {
            InitializeComponent();
            drpPartLimits.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpPartLimits_ItemValueNeeded);
            cbxPartType.SelectedIndexChanged+=new EventHandler(cbxPartType_SelectedIndexChanged);
            bsPartLimits.DataSource = PartCounts;
            ts = new ThemeSettings(XDocument.Load("ThemeSettings.xml"));
            LoadHulls();
            LoadPartTypes();
            ShowPartTypes();
        }

        

        #region Load
        private void LoadHulls()
        {
            hullsDoc = XDocument.Load("ShipHulls.xml");
            ExistingHulls = ShipHull.GetShipHulls(hullsDoc);
            cbxExistingHulls.DataSource = ExistingHulls;
            cbxExistingHulls.DisplayMember = "Name";
        }

        private void LoadPartTypes()
        {
            var type = typeof(ShipPart);
            IEnumerable<Type> partListE = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.GetType() != type);
            PartTypes = new List<Type>(partListE.Where(t => t != typeof(ShipPart)));
            cbxPartType.DataSource = PartTypes;
            cbxPartType.DisplayMember = "Name";
        }

        private void LoadPartDetail(bool IsWeapon)
        {
            cbxPartDetail.Items.Clear();
            if (IsWeapon)
            {
                cbxPartDetail.Items.Add(" - None - ");
                cbxPartDetail.Enabled = true;
                foreach (string firingType in ts.FiringTypes)
                    cbxPartDetail.Items.Add(firingType);
            }
            else
                cbxPartDetail.Enabled = false;
        }

        #endregion

        #region Events
        void drpPartLimits_ItemValueNeeded(object sender, Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventArgs e)
        {
            if (e.ItemIndex < bsPartLimits.Count && e.ItemIndex >= 0)
            {
                switch (e.Control.Name)
                {
                    case "lblPartLimitTitle":
                        e.Value = bsPartLimits[e.ItemIndex].ToString();
                        break;
                }
            }
        }

        void cbxPartType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPartType.SelectedIndex >= 0)
            {
                bool isWeapon = ((Type)cbxPartType.SelectedItem).Name == "WeaponPart";
                LoadPartDetail(isWeapon);
            }
        }
        #endregion

        #region Buttons
        private void btnLoadHull_Click(object sender, EventArgs e)
        {
            hull = (ShipHull)cbxExistingHulls.SelectedItem;
            PartCounts = hull.AllowedParts;
            ShowHull();
        }
        private void btnAddPartLimit_Click(object sender, EventArgs e)
        {
            Type newPartType = (Type)cbxPartType.SelectedItem;
            string ActionMechanism = string.Empty;
            if (cbxPartDetail.SelectedIndex >= 0 && cbxPartDetail.SelectedText!=" - None - ")
                ActionMechanism = cbxPartDetail.SelectedItem.ToString();
            int CountOfParts = int.Parse(nudMaxPartCount.Value.ToString());
            PartCount newPartCount;
            if (PartCounts.Where(f => f.PartType == newPartType && f.ActionMechanism == ActionMechanism).Count() > 0)
            {
                newPartCount = PartCounts.First(f => f.PartType == newPartType && f.ActionMechanism == ActionMechanism);
                newPartCount.CountOfParts = CountOfParts;
            }
            else
            {
                newPartCount = new PartCount(newPartType, ActionMechanism, CountOfParts);
                bsPartLimits.Add(newPartCount);
            }
            ShowParts();
            ShowPartTypes();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            hull = new ShipHull();
            ShowHull();
            tbxHullName.Text = string.Empty;
        }
        private void btnSaveHull_Click(object sender, EventArgs e)
        {
            string Name = tbxHullName.Text;
            int MaxHP = int.Parse(nudHullPointsMax.Value.ToString());

            hull = new ShipHull(Name, MaxHP, PartCounts,tbxImage.Text);
            
            hull.GetObjectXML(hullsDoc);
            hullsDoc.Save("ShipHulls.xml");
            LoadHulls();

        }
        #endregion

        #region Show
        private void ShowHull()
        {
            tbxHullName.Text = hull.Name;
            nudHullPointsMax.Value = hull.HullPoints.Max;
            tbxImage.Text = hull.ImageURL;
            PartCounts = hull.AllowedParts;
            ShowParts();
        }

        private void ShowParts()
        {
            bsPartLimits.DataSource = PartCounts;
            drpPartLimits.DataSource = bsPartLimits;
            drpPartLimits.BeginResetItemTemplate();
            drpPartLimits.EndResetItemTemplate();
        }

        private void ShowPartTypes()
        {
            cbxPartType.SelectedIndex = -1;
            cbxPartDetail.Items.Clear();
            cbxPartDetail.SelectedText = string.Empty;
            cbxPartDetail.Enabled = false;
            nudMaxPartCount.Value = 0;
        }
        #endregion

    }
}
