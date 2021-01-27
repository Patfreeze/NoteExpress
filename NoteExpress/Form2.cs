using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NotepadExpress
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            var rtf = string.Format(@"{{\rtf1\ansi \line \b {0} {1}\b0 \line {2} \line {3} \line  \line \b All auto-saved file are in \b0 \line {4} \line }}",
                          "Note Express",
                          Application.ProductVersion,
                          "Made by Patrick Frenette",
                          "https://github.com/Patfreeze/NoteExpress",
                          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\","\\\\") + "\\\\NoteExpress");

            this.richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            //this.richTextBox1.SelectionStart = this.richTextBox1.TextLength;
            this.richTextBox1.SelectedRtf = rtf;
        }
    }
}
