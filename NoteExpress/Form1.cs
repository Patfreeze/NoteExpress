using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace NotepadExpress
{
    public partial class Form1 : Form
    {

        public String sProgramName = "Note Express";
        public String sDefaultPath = "\\NoteExpress\\";
        private String sCurrentNamefile = "";
        private String sOpenFile = "";
        private Timer SearchTextBoxTimer;
        private Timer CloseAlreadyNoteTimer;
        private String sUserDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private int iFunnyFac = 0;
        private int iDelay = 750;
        
        public Form1(string[] args)
        {
            InitializeComponent();

            System.IO.Directory.CreateDirectory(sUserDocPath + sDefaultPath);

            var date1 = DateTime.Now;

            sCurrentNamefile = date1.ToString("yyyy-MM-dd_HHmmss") + "";
            this.Text = sCurrentNamefile + " - " + sProgramName;
            this.toolStripStatusLabel1.Text = "Note Express - Best express note never made :P ";

            if (args.Length != 0)
            {
                /*
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("args[{0}] == {1}", i, args[i]);
                }
                 */
                // args[0] = chemin vers fichier

                sOpenFile = args[0];
                this.Text = sOpenFile + " - " + sProgramName;
                addFileToText(args[0]);
            }

        }

        /* GET FILE NAME */
        public String getFileName() {
            return sCurrentNamefile + ".txt";
        }

        /* GET PATH FILE NAME */
        public String getPathFile() {
            if (sOpenFile != "")
            {
                return sOpenFile;
            }
            else {
                return sUserDocPath + sDefaultPath + getFileName();
            }
        }

        private void SaveFile() {
            //using (File.Create(getPathFile()));
            if (richTextBox1.TextLength > 0)
            {
                richTextBox1.SaveFile(getPathFile(), RichTextBoxStreamType.PlainText);
            }
            else if (sOpenFile == "")
            {
                File.Delete(getPathFile());
            }
        }



        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            //Console.Write(sCurrentNamefile+"\n");
            //this.Dispose();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Dispose();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.Show();
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine();
            // TODO: Create a New Instance of
            System.Diagnostics.Process noteExpress = new System.Diagnostics.Process();
            noteExpress.StartInfo.FileName = ProgramFilesx86()+sDefaultPath+"noteexpress.exe";
            //noteExpress.StartInfo.Arguments = " C:\\craftbukkit\\bukkit.yml";
            noteExpress.Start();

            //System.Diagnostics.Process.Start("noteexpress.exe");
            //Form1 formNew = new Form1(false);
            //formNew.Show();
        }

        private void CloseAlreadyNoteTimer_Tick(object sender, EventArgs e)
        {
            List<String> list = new List<String>();
            var myProcess = Process.GetProcessesByName("NoteExpress");
            var myProcess2 = Process.GetProcessesByName("NoteExpress");
            for (int i = 0; i < myProcess.Length; i++)
            {
                for (int z = 0; z < myProcess2.Length; z++)
                {
                    if (myProcess[i].Id != myProcess2[z].Id && myProcess[i].MainWindowTitle == myProcess2[z].MainWindowTitle)
                    {
                        if (!list.Contains(myProcess[z].MainWindowTitle))
                        {
                            list.Add(myProcess[z].MainWindowTitle);
                            Console.WriteLine("Closing: " + myProcess[z].MainWindowTitle);
                            myProcess[z].CloseMainWindow();
                        }
                    }
                }
            }

            CloseAlreadyNoteTimer.Stop();
            CloseAlreadyNoteTimer.Dispose();
            CloseAlreadyNoteTimer = null;
            if (File.Exists(getPathFile()))
            {
                this.Dispose(); // If this note was already saved close it
            }

        }

        private void SearchTextBoxTimer_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("The user finished typing.");

            //SearchTextBox_TextChanged();
            SaveFile();
            SearchTextBoxTimer.Stop();
            SearchTextBoxTimer.Dispose();
            SearchTextBoxTimer = null;
            iDelay = 750;

            var dateSave = DateTime.Now;
            toolStripStatusLabel1.Text = dateSave.ToString("yyyy-MM-dd HH:mm:ss") + " | Saving at " + getPathFile();

        }

        private String getNextFunnyFact() {

            String[] a_sFunnyFac = {
                "The power of a simple note, is to be keep it somewhere easy to find",
                "Do or do not, there is no try...",
                "Dont forget to save this note! To late I saved for you. XD",
                "Do you remember the first note you write?",
                "Remove the pain in the ice! Yes, ice! I dont want bad word here.",
                "Look around, your note is somewhere."
            };

            iFunnyFac = iFunnyFac + 1;
            //Console.WriteLine("Next note: " + iFunnyFac);

            if (iFunnyFac >= a_sFunnyFac.Length) {
                iFunnyFac = 0;
            }

            return a_sFunnyFac[iFunnyFac];
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (SearchTextBoxTimer != null)
            {
                //Console.WriteLine("The user is currently typing. " + SearchTextBoxTimer.Interval);
                if (SearchTextBoxTimer.Interval < iDelay)
                {
                    SearchTextBoxTimer.Interval += iDelay;
                    iDelay = iDelay + 600;
                    //Console.WriteLine("Delaying..." + SearchTextBoxTimer.Interval);
                }
            }
            else
            {
                //Console.WriteLine("The user just started typing.");
                SearchTextBoxTimer = new System.Windows.Forms.Timer();
                SearchTextBoxTimer.Tick += new EventHandler(SearchTextBoxTimer_Tick);
                SearchTextBoxTimer.Interval = 500;
                SearchTextBoxTimer.Start();

                toolStripStatusLabel1.Text = "Note Express - " + getNextFunnyFact();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

        }

        private void addFileToText(string file) {
            this.Text = this.sOpenFile + " - " + sProgramName;
            string[] a_readLines = File.ReadAllLines(file);
            this.richTextBox1.Lines = a_readLines;
        }

        private void addFileToText(string[] files)
        {
            //Console.WriteLine("File name: "+files[0]);
            this.sOpenFile = files[0];
            this.Text = this.sOpenFile + " - " + sProgramName;
            string[] a_readLines = File.ReadAllLines(files[0]);
            //Console.WriteLine("FirstLine: "+a_readLines[0]);
            this.richTextBox1.Lines = a_readLines;
            //this.richTextBox1.Select(this.richTextBox1.Text.Length - 1, 0);
            //this.richTextBox1.ScrollToCaret();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //Console.WriteLine(openFileDialog1.FileNames);
            // Add files
            addFileToText(openFileDialog1.FileNames);

            // Reset value to default
            openFileDialog1.FileName = "*.*";
        }

        private String getFolderPath() {
            return sUserDocPath + sDefaultPath;
        }

        private void openAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Open All in the NoteExpress folder
            DirectoryInfo d = new DirectoryInfo(this.getFolderPath());
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files



            foreach (FileInfo file in Files)
            {
                Console.WriteLine(file.Name);
                System.Diagnostics.Process noteExpress = new System.Diagnostics.Process();
                noteExpress.StartInfo.FileName = ProgramFilesx86() + sDefaultPath + "noteexpress.exe";
                noteExpress.StartInfo.Arguments = " "+this.getFolderPath()+file.Name;
                noteExpress.Start();
            }

            CloseAlreadyNoteTimer = new System.Windows.Forms.Timer();
            CloseAlreadyNoteTimer.Tick += new EventHandler(CloseAlreadyNoteTimer_Tick);
            CloseAlreadyNoteTimer.Interval = 500;
            CloseAlreadyNoteTimer.Start();



        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("Save as Completed");
                richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                var dateSave = DateTime.Now;
                toolStripStatusLabel1.Text = dateSave.ToString("yyyy-MM-dd HH:mm:ss") + " | Saving at " + saveFileDialog1.FileName;
            }
        }
    }
}
