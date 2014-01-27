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

namespace PartMaker
{
    public partial class PartMaker : Form
    {
        List<ShipPart> ExistingParts = new List<ShipPart>();
        ShipPart Part;
        List<IShipPartAction> PartActions = new List<IShipPartAction>();
        BindingSource bs;

        public PartMaker()
        {
            InitializeComponent();
            drpActionList.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpActionList_ItemValueNeeded);
            lblActionTitle.DataBindings.Add("Text", bs, "Name");
            bs.DataSource = PartActions;
            LoadParts();
            ShowActions();
        }

        private void LoadParts()
        {
            throw new NotImplementedException();
        }

        void drpActionList_ItemValueNeeded(object sender, Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventArgs e)
        {
            if (e.ItemIndex < bs.Count && e.ItemIndex >= 0)
            {
                switch (e.Control.Name)
                {
                    case "lblActionTitle":
                        e.Value = bs[e.ItemIndex].ToString();
                        break;
                }
            }
        }

        void ShowActions()
        {
            var type = typeof(IShipPartAction);
            IEnumerable<Type> partListE = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.GetType() != type);
            List<Type> partList = new List<Type>(partListE.Where(t => t != typeof(IShipPartAction)));
            cbxActions.DataSource = partList;
            cbxActions.DisplayMember = "Name";
            cbxActions.SelectedIndex = 0;
        }
    }
}
