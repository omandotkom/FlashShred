using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashShred
{
    
    public partial class Form1 : Form
    {
        private OpenFileDialog openFileDialog1 = new OpenFileDialog();
        public Form1()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Read the files
                listBox1.Items.Clear();
                foreach (String filePath in openFileDialog1.FileNames) {
                    listBox1.Items.Add(filePath);
                }
                Algorithm algorithm = new Algorithm(openFileDialog1.FileNames);
                
            }
        }
        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "My Image Browser";
        }

    }
}
