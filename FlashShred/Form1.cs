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
        private Algorithm algorithm = new Algorithm();
       
        public Form1()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
            algorithm.onPerformStepEvent += Algorithm_onPerformStepEvent;
        }

        private void Algorithm_onPerformStepEvent(string message)
        {
            progressBar1.Invoke(new Action(() => progressBar1.PerformStep()));
            label1.Invoke(new Action(() => label1.Text = message));
            Console.WriteLine("Depan " + message);
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
                if (listBox1.Items.Count > 0)
                {
                    algorithm.FileNames = listBox1.Items.OfType<string>().ToArray();
                    algorithm.CheckValidity();
                }
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

        private async void button2_Click(object sender, EventArgs e)
        {
            
            
            progressBar1.Step = 1;
            progressBar1.Maximum = (algorithm.FileNames.Length * 2) + 7;
          await algorithm.shred();
           
        }

    }
}
