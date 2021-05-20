using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using NoteExpress;

namespace NoteExpress
{
    public partial class FormSearch : Form
    {
        private System.Windows.Forms.Timer SearchTextBoxTimer;
        private int iDelay = 1000; // Delay after writting text
        private string sFolderPath = "";
        private string sFolderApp = "";
        private String sDefaultPath = "\\NoteExpress\\";
        private ClassLastsearch classLastsearch = new ClassLastsearch();
        private string defaultText = "| Type any word in the box then press the Search button.";
        private string errorMessage = "Sorry but we can't search for an empty string...\nBut if you thing is a good choice the result is NULL! XD";

        public FormSearch(string sFolderPath, string sFolderApp)
        {
            InitializeComponent();
            this.comboBoxSearch.Focus();
            this.sFolderPath = sFolderPath;
            this.sFolderApp = sFolderApp;

            // Load last search
            this.comboBoxSearch.Items.Clear();

            ComboboxItem itemList = new ComboboxItem();
            itemList.Text = this.defaultText;
            itemList.Value = 0;
            this.listBoxSearch.Items.Add(itemList);

            int iCount = 0;
            foreach (ComboboxItem item in this.classLastsearch.getLastSearch())
            {
                this.comboBoxSearch.Items.Add(item);
                iCount++;
            }
        }

        private void richTextBoxSearch_MouseUp(object sender, MouseEventArgs e)
        {
            //the number of the selected line
            int i = (e.Location.Y) / 20;
            //Console.WriteLine("coor:"+e.Location.Y+" line:"+i);

            // get the value of the number line.
            if (i < this.listBoxSearch.Items.Count)
            {
                // TODO: On click Open of focus the files
                Console.WriteLine(this.listBoxSearch.Items[i].ToString().Split('|')[0]);
            }
        }

        private void comboBoxSearch_TextUpdate(object sender, EventArgs e)
        {
            this.progressBarSearch.Value = 0;
            searchInFiles(false);

            //Console.WriteLine(this.comboBoxSearch.Text);
            if (this.comboBoxSearch.Text == "")
            {
                this.listBoxSearch.Items.Clear();
                ComboboxItem itemList = new ComboboxItem();
                itemList.Text = this.defaultText;
                itemList.Value = 0;
                this.listBoxSearch.Items.Add(itemList);
            }
        }



        private void buttonSearch_Click(object sender, EventArgs e)
        {
            //guard if empty
            if (this.comboBoxSearch.Text == "")
            {
                MessageBox.Show(this.errorMessage);
                return;
            }
            searchInFiles(true);
           
        }

        private void comboBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13) {
                //guard if empty
                if (this.comboBoxSearch.Text == "")
                {
                    MessageBox.Show(this.errorMessage);
                    return;
                }
                searchInFiles(true);
            }
        }

        private void searchInFiles(Boolean bForceSearch) {
            // Guard if string is empty
            if (this.comboBoxSearch.Text == "")
            {
                return;
            }

            if (!bForceSearch && this.comboBoxSearch.Text.Length < 3)
            {
                return;
            }
            // Start chrono
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();


            // Clear the last search
            this.listBoxSearch.Items.Clear();

            // Add info
            ComboboxItem itemEnd = new ComboboxItem();

            itemEnd.Text = "|************************  Double click on line below to open the note  ************************";
            itemEnd.Value = 0;
            this.listBoxSearch.Items.Add(itemEnd);

            itemEnd.Text = ".";
            itemEnd.Value = 0;
            this.listBoxSearch.Items.Add(itemEnd);


            // Use the AhoCorasick algo for searching
            AhoCorasick.Trie trie = new AhoCorasick.Trie();

            string[] words = this.comboBoxSearch.Text.ToLower().Split(' ');

            // add words
            foreach (string word in words)
            {
                trie.Add(word);
            }

            // build search tree
            trie.Build();

            // Loop only on  our folder
            //Console.WriteLine(this.sFolderPath);
            DirectoryInfo d = new DirectoryInfo(this.sFolderPath);
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files

            int iCountFile = Files.Length;
            int iProgress = 100 / iCountFile;
            this.progressBarSearch.Value = 0;
            Boolean bFoundOne = false;
            foreach (FileInfo file in Files)
            {
                // Read a text file line by line.
                string[] lines = File.ReadAllLines(this.sFolderPath + file.Name, Encoding.GetEncoding(Encoding.Default.WebName));

                int iCountLine = 1;
                foreach (string line in lines)
                {
                    // find words
                    foreach (string word in trie.Find(line.ToLower()))
                    {
                        ComboboxItem itemList = new ComboboxItem();
                        itemList.Text = file.Name + " | Find : '" + word + "' at line " + iCountLine + " '" + line;
                        itemList.Value = iCountLine;
                        itemList.Title = "Double-Click to open";
                        this.listBoxSearch.Items.Add(itemList);
                        bFoundOne = true;
                    }
                    iCountLine++;
                }

                this.progressBarSearch.Value += iProgress;
            }
            this.progressBarSearch.Value = 100;
            if (!bFoundOne) {
                ComboboxItem itemNothing = new ComboboxItem();
                itemNothing.Text = "| Sorry but nothing found in all files at '" + this.sFolderPath + "'  :(";
                itemNothing.Value = 0;
                this.listBoxSearch.Items.Add(itemNothing);  
            }

            if (SearchTextBoxTimer != null)
            {
                if (SearchTextBoxTimer.Interval < iDelay)
                {
                    SearchTextBoxTimer.Interval += iDelay;
                    iDelay = iDelay + 1000;
                }
            }
            else
            {
                SearchTextBoxTimer = new System.Windows.Forms.Timer();
                SearchTextBoxTimer.Tick += new EventHandler(SearchTextBoxTimer_Tick);
                SearchTextBoxTimer.Interval = 1000;
                SearchTextBoxTimer.Start();
            }
            watch.Stop();

            this.toolStripStatusLabelSearch.Text = "Search execution in "+watch.ElapsedMilliseconds+" ms";
        }


        private void SearchTextBoxTimer_Tick(object sender, EventArgs e)
        {
            this.progressBarSearch.Value = 0;
            SearchTextBoxTimer.Stop();
            SearchTextBoxTimer.Dispose();
            SearchTextBoxTimer = null;

            ComboboxItem item = new ComboboxItem();
            item.Text = this.comboBoxSearch.Text;
            item.Value = this.comboBoxSearch.Items.Count;

            this.comboBoxSearch.Items.Add(item);

            this.classLastsearch.addToList(this.comboBoxSearch.Text);
            this.classLastsearch.saveFile();
        }

        private void comboBoxSearch_TextChanged(object sender, EventArgs e)
        {
            searchInFiles(true);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            this.comboBoxSearch.Text = "";
            this.listBoxSearch.Items.Clear();

            ComboboxItem itemList = new ComboboxItem();
            itemList.Text = this.defaultText;
            itemList.Value = 0;
            this.listBoxSearch.Items.Add(itemList);
        }

        private void listBoxSearch_SelectedIndexChanged(object sender, MouseEventArgs e)
        {
            // If not open it
            string sFile = listBoxSearch.Text.Split('|')[0];

            if(sFile.Trim() != "") {
                System.Diagnostics.Process noteExpress = new System.Diagnostics.Process();
                noteExpress.StartInfo.FileName = this.sFolderApp + this.sDefaultPath + "noteexpress.exe";
                noteExpress.StartInfo.Arguments = " " + this.sFolderPath + sFile;
                noteExpress.Start();
            }
        }
    }
}
