using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace NoteExpress
{
    class ClassProcess
    {

        public String sDefaultPath = "\\NoteExpress\\";
        private String sUserDocPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private String sPreferenceFilename = "processbox.bin";
        private XmlDocument doc = new XmlDocument();
        private int iCountMax = 100000000;

        public ClassProcess(System.Diagnostics.Process[] process)
        {
            System.IO.Directory.CreateDirectory(sUserDocPath + sDefaultPath);

            // If file not exist we create it before loading it
            if (!File.Exists(getPreferenceFilename()))
            {
                // Save file
                iniFile();
            }

            this.loadFile();

            /*
            // Loop to make all as inactive
            XmlNodeList elemListId = doc.GetElementsByTagName("active");
            for (int i = 0; i < elemListId.Count; i++)
            {
                elemListId[i].InnerText = "0";
            }*/

            /*
            // Loop on each current process
            for (int i = 0; i < process.Length; i++)
            {
                // find a node - here the one with id='1234'
                XmlNode node = doc.SelectSingleNode("/processes/process[@id='" + process[i].Id + "']/date");

                // if found....
                if (node != null)
                {
                    // get its parent node
                    //XmlNode parent = node.ParentNode;
                    node.InnerText = "1";
                }

            }
            this.saveFile();*/

            /*
            // Clean the xml file to remove all process that is no longer there
            XmlNodeList elemListId2 = doc.GetElementsByTagName("active");
            List<XmlNode> nodesToDelete = new List<XmlNode>();
            for (int i = 0; i < elemListId2.Count; i++)
            {
                if (Int32.Parse(elemListId2[i].InnerXml) < 1) {
                    nodesToDelete.Add(elemListId2[i].ParentNode);
                }
            }*/

            /*
            for (int i = 0; i < nodesToDelete.Count; i++ )
            {
                // get its parent node
                XmlNode parent = nodesToDelete[i].ParentNode;

                // remove the child node
                parent.RemoveChild(nodesToDelete[i]);
            }
             * */

            // Save new state
            //this.saveFile();
        }

        public void resetProcessFile()
        {
            // reset all file
            iniFile();
        }

        private bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void deleteById(int procId) 
        {
            // Reload from file
            this.loadFile();

            // find a node - here the one with id='1234'
            XmlNode node = doc.SelectSingleNode("/processes/process[@id='" + procId + "']");

            if (node != null)
            {
                // get its parent node
                XmlNode parent = node.ParentNode;

                // remove the child node
                parent.RemoveChild(node);
            }
            // Save new state
            this.saveFile();

        }

        private void loadFile()
        {
            //This will lock the execution until the file is ready
            //TODO: Add some logic to make it async and cancelable
            int iCount = 0;
            while (!IsFileReady(getPreferenceFilename()) && iCount < iCountMax)
            {
                iCount++;
            }

            if (iCount >= iCountMax)
            {
                Console.WriteLine("Failed to load after count of " + iCount);
            }
            else
            {
                Console.WriteLine("Loading after count of " + iCount);

                using (var stream = File.Open(getPreferenceFilename(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = XmlTextReader.Create(stream))
                {
                    this.doc.Load(reader);
                    reader.Close();
                }
            }
 
        }

        private String getPreferenceFilename()
        {
            return sUserDocPath + sDefaultPath + sPreferenceFilename;
        }

        public void addToList(int processID, String sPath) 
        {
            // Reload from file
            this.loadFile();

            // Add a process element.
            XmlElement elementProcess = doc.CreateElement("process");
            elementProcess.SetAttribute("id", "" + processID);

            var dateCurrent = DateTime.Now;

            XmlElement element = doc.CreateElement("date");
            element.InnerText = dateCurrent.ToString("yyyy-MM-dd") + "";
            elementProcess.AppendChild(element);

            element = doc.CreateElement("id");
            element.InnerText = "" + processID;
            elementProcess.AppendChild(element);

            element = doc.CreateElement("path");
            element.InnerText = sPath;
            elementProcess.AppendChild(element);

            this.doc.DocumentElement.AppendChild(elementProcess);

        }
        
        private void iniFile() 
        {
            // Create the XmlDocument.
            doc.LoadXml("<processes></processes>");

            doc.PreserveWhitespace = true;
            doc.Save(getPreferenceFilename());
            this.loadFile();
        }

        public String getPathById(int procId)
        {
            // Reload from file
            this.loadFile();

            String sPath = "";
            //Loop on each id if is the good return sPath if not will return empty string
            XmlNodeList elemListId = doc.GetElementsByTagName("id");
            XmlNodeList elemListPath = doc.GetElementsByTagName("path");
            for (int i = 0; i < elemListId.Count; i++)
            {
                if (procId == Int32.Parse(elemListId[i].InnerXml))
                {
                    sPath = elemListPath[i].InnerXml;
                    return sPath; // Return here will break also the loop
                }

            }
            return sPath;
        }

        public void setPathById(int procId, String sPath)
        {

            // Reload from file
            this.loadFile();

            //Loop on each id if is the good one save it
            XmlNodeList elemListId = doc.GetElementsByTagName("id");
            XmlNodeList elemListPath = doc.GetElementsByTagName("path");
            for (int i = 0; i < elemListId.Count; i++)
            {
                if (procId == Int32.Parse(elemListId[i].InnerXml))
                {
                    elemListPath[i].InnerText = sPath;
                }
                
            }
            this.saveFile();
        }

        public void saveFile()
        {

            //This will lock the execution until the file is ready
            //TODO: Add some logic to make it async and cancelable
            int iCount = 0;
            while (!IsFileReady(getPreferenceFilename()) && iCount < iCountMax)
            {
                iCount++;
            }

            if (iCount >= iCountMax)
            {
                Console.WriteLine("Unable to save after count of " + iCount);
            }
            else
            {
                //Console.WriteLine("Saving after count of " + iCount);

                this.doc.PreserveWhitespace = true;
                this.doc.Save(getPreferenceFilename());
                this.loadFile(); // reload from file
            }
        }
    }
}

