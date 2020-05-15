using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashShred
{
    public delegate void DriveSelected(string letter);
    public partial class SectorCleaner : Form
    {
        public event DriveSelected Selected;
        public SectorCleaner()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SectorCleaner_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            foreach (DriveInfo d in SectorCleanerAlgorithm.GetDrives())
            {
                comboBox1.Items.Add(d.Name);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        public void OnSelected(string le)
        {
            Selected?.Invoke(le);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OnSelected(comboBox1.SelectedItem.ToString());
            
        }
    }
}