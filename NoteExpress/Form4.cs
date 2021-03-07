using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NoteExpress
{
    public partial class FormPreference : Form
    {
        ClassPreference classPreference = null;
        String sLastNoteChoice = "";

        public FormPreference()
        {
            InitializeComponent();
            classPreference = new ClassPreference();
            classPreference.loadFilePreference();
            checkBox1.Checked = classPreference.getOpenAllAtStart();
            comboBox1.Text = classPreference.getNewNoteChoice();
            this.sLastNoteChoice = classPreference.getNewNoteChoice();
        }

        private void savePreference() {
            classPreference.setOpenAllAtStart(checkBox1.Checked);
            classPreference.setNewNoteChoice(comboBox1.Text);
            classPreference.saveFilePreference();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            savePreference();
        }

        private void label_openall_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = !checkBox1.Checked;
            savePreference();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            savePreference();
        }

    }

}
