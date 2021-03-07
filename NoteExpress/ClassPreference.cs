using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NoteExpress
{
    class ClassPreference
    {

        public String sDefaultPath = "\\NoteExpress\\";
        private String sUserDocPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private String sPreferenceFilename = "noteexpress.bin";

        private String sVersion = ""; // Version in the file
        private String sCurrentVersion = "1.0"; // Current version

        private Boolean bOpenAllAtStart = false;
        private String sNewNoteChoice = "Blank";
        
        public ClassPreference()
        {
            System.IO.Directory.CreateDirectory(sUserDocPath + sDefaultPath);

            // If file not exist we create it before loading it
            if (!File.Exists(getPreferenceFilename()))
            {
                // Save file
                saveFilePreference();
            }
            else {
                // File exist: Do we have the good version in the file
                loadFilePreference();
            }

        }

        public void setOpenAllAtStart(Boolean bOpenAll) {
            this.bOpenAllAtStart = bOpenAll;
        }

        public Boolean getOpenAllAtStart() {
            return this.bOpenAllAtStart;
        }

        public void setNewNoteChoice(String sNewnoteChoice) {

            // We accept only this value
            switch (sNewnoteChoice)
            {
                case "Add Date":
                case "Add Date-Time":
                case "Add Title Date-Time":
                case "Blank":
                    this.sNewNoteChoice = sNewnoteChoice;
                    break;

                default:
                    // We leave it like it was before if not good
                    break;
            }
        }

        public String getNewNoteChoice() {
            return this.sNewNoteChoice;
        }

        private String getPreferenceFilename() {
            return sUserDocPath+sDefaultPath+sPreferenceFilename;
        }

        public void loadFilePreference()
        {
            // Open the File Stream
            FileStream fs1 = new FileStream(getPreferenceFilename(), FileMode.Open);
            
            // Read the binary
            BinaryReader br = new BinaryReader(fs1);

            // Read version
            this.sVersion = br.ReadString();

            // Not the good version
            if (!this.sCurrentVersion.Contains(this.sVersion)) { 
                // To bad we overwrite
                fs1.Close();
                saveFilePreference();
                return;
            }

            // Get Binary info
            this.bOpenAllAtStart = br.ReadBoolean();

            // Get the String for new note
            this.sNewNoteChoice = br.ReadString();

            /*
            // Example
            byte _byte = br.ReadByte();
            char _char = br.ReadChar();
            int _int = br.ReadInt16();
            String _string = br.ReadString();
            double _dbl = br.ReadDouble();
            Console.WriteLine(_byte);
            Console.WriteLine(_char);
            Console.WriteLine(_int);
            Console.WriteLine(_dbl);
            */

            // Close the file
            fs1.Close();
            
        }

        public void saveFilePreference()
        {
            // Create or write if not exist
            FileStream fs = new FileStream(getPreferenceFilename(), FileMode.Create);

            //Write binary
            BinaryWriter bw = new BinaryWriter(fs);

            // Save version preference
            bw.Write(this.sCurrentVersion);  //int version

            // Save preference Open All
            bw.Write(this.bOpenAllAtStart);  //writing bool value

            // Save preference On new note
            bw.Write(this.sNewNoteChoice);

            /*
            // Example
            bw.Write(Convert.ToByte('a')); //writing byte
            bw.Write('a');                 //writing character
            
            bw.Write(123);                 //number
            bw.Write(123.12);              // double value
            */
            bw.Close();
            
        }
    }
}
