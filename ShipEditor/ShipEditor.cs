using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StarShips;
using StarShips.PartBase;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ShipEditor
{
    public partial class ShipEditor : Form
    {
        Ship ship = new Ship();
        BindingSource bs = new BindingSource();
        
        public ShipEditor()
        {
            InitializeComponent();
            drpPartList.ItemValueNeeded += new Microsoft.VisualBasic.PowerPacks.DataRepeaterItemValueEventHandler(drpPartList_ItemValueNeeded);
            tbxShipName.TextChanged += new EventHandler(tbxShipName_TextChanged);
            nudHitpoints.ValueChanged += new EventHandler(nudHitpoints_ValueChanged);
            lblPartName.DataBindings.Add("Text", ship.Equipment, "Name");
            bs.DataSource = ship.Equipment;
            ShowParts();
            ShowShip();
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
                        e.Value = bs[e.ItemIndex].ToString();
                        break;
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdOpen.ShowDialog();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfdSave.FileName = tbxShipName.Text + ".ship";
            sfdSave.ShowDialog();
        }

        private void ShowShip()
        {
            tbxShipName.Text = ship.Name;
            nudHitpoints.Value = ship.HP.Max;
            drpPartList.DataSource = bs;
            drpPartList.BeginResetItemTemplate();
            drpPartList.EndResetItemTemplate();
        }

        private void ShowParts()
        {
            var type = typeof(ShipPart);
            IEnumerable<Type> partListE = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.GetType()!=type);
            List<Type> partList = new List<Type>(partListE.Where(t=>t!=typeof(ShipPart)));
            cbxPartList.DataSource = partList;
            cbxPartList.DisplayMember = "Name";
            cbxPartList.SelectedIndex = 0;
            
        }


        private void btnDeletePart_Click(object sender, EventArgs e)
        {
            int index = drpPartList.CurrentItemIndex;
            bs.RemoveAt(index);
            drpPartList.BeginResetItemTemplate();
            drpPartList.EndResetItemTemplate();
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            Type newPartType = (Type)cbxPartList.SelectedItem;
            ShipPart newPart = (ShipPart)Activator.CreateInstance(newPartType);
            bs.Add(newPart);
            ShowShip();
        }

        private void sfdSave_FileOk(object sender, CancelEventArgs e)
        {
            ship.Name = tbxShipName.Text;
            ship.HP.Max = int.Parse(nudHitpoints.Value.ToString());
            ship.HP.Current = ship.HP.Max;
            string fileName = sfdSave.FileName;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, ship);
            stream.Close();
        }

        private void ofdOpen_FileOk(object sender, CancelEventArgs e)
        {
            string fileName = ofdOpen.FileName;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            ship = (Ship)formatter.Deserialize(stream);
            bs.DataSource = ship.Equipment;
            stream.Close();
            ShowShip();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ship = new Ship();
            ShowShip();
        }
    }
}
