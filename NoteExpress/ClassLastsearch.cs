using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using NoteExpress;

namespace NoteExpress
{
    class ClassLastsearch
    {
        public String sDefaultPath = "\\NoteExpress\\";
        private String sUserDocPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private String sFilename = "lastsearch.bin";
        private XmlDocument doc = new XmlDocument();

        public ClassLastsearch()
        {
            System.IO.Directory.CreateDirectory(sUserDocPath + sDefaultPath);

            // If file not exist we create it before loading it
            if (!File.Exists(getFilename()))
            {
                // Save file
                iniFile();
            }

            this.loadFile();
        }

        public List<ComboboxItem> getLastSearch()
        {

            List<ComboboxItem> a_sLastSearch = new List<ComboboxItem>();

            int iCount = 0;
            foreach (XmlNode node in this.doc.GetElementsByTagName("search"))
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = node.InnerText;
                item.Value = iCount;
                a_sLastSearch.Add(item);
                iCount++;
            }

            a_sLastSearch.Reverse();

            return a_sLastSearch;
        }
        
        private void loadFile()
        {
            //Console.WriteLine("Loading after count of " + iCount);
            using (var stream = File.Open(getFilename(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = XmlTextReader.Create(stream))
            {
                this.doc.Load(reader);
                reader.Close();
            }
         }

        private String getFilename()
        {
            return sUserDocPath + sDefaultPath + sFilename;
        }

        public void addToList(String sSearch) 
        {
            // Reload from file
            this.loadFile();

            // Count number of element if more than 10 delete last one before adding it
            if (this.doc.GetElementsByTagName("search").Count > 19)
            {
                // Replace the first one
                this.doc.GetElementsByTagName("searchs")[0].RemoveChild(this.doc.GetElementsByTagName("searchs")[0].FirstChild);
                
            }

            // Add a process element.
            XmlElement elementSearch = doc.CreateElement("search");
            elementSearch.InnerText = sSearch;
            this.doc.DocumentElement.AppendChild(elementSearch);
         

        }
        

        private void iniFile() 
        {
            // Create the XmlDocument.
            doc.LoadXml("<searchs></searchs>");

            doc.PreserveWhitespace = true;
            doc.Save(getFilename());
            this.loadFile();
        }

        public void saveFile()
        {
            //Console.WriteLine("Saving after count of " + iCount);
            this.doc.PreserveWhitespace = true;
            this.doc.Save(getFilename());
            this.loadFile(); // reload from file
        }
    }
}
