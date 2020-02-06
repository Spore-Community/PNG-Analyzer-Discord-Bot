using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DiscordBot
{
    //// A Spore Asset (creation)
    public class Asset
    {
        private int version;
        private string typeId;
        private string groupId;
        private string instanceId;
        private string machineId;
        private long assetId;
        private long parentId;
        private string time;
        private string username;
        private long userId;
        private string name;
        private string desc;
        private string tags;

        private int partCount;

        public Asset(string filePath)
        {
            // Extract data to XML file
            if(!File.Exists(filePath+".xml")){
                Process.Start("python2", "\"C:\\Users\\KyleN\\OneDrive\\Documents\\Spore Files\\PNG Decoder\\spore_decoder_by_ymgve_and_rick.py\" \""+filePath+"\"");

                int timer = 0;
                while(!File.Exists(filePath+".xml") && timer<1000){
                    Thread.Sleep(1);
                    timer++;
                }
                Console.WriteLine($"Decoding took {timer}ms");
            }

            // Read data from the file
            //try
            {
                using (var reader = new StreamReader(filePath+".xml", Encoding.GetEncoding("iso-8859-1")))
                {
                    var spore = ReadAssetData(reader, 5);
                    if(!spore.Equals("spore")) Console.WriteLine("File is not a Spore asset!");

                    version = Convert.ToInt32(ReadAssetData(reader, 4));
                    if(version!=5 && version!=6) Console.WriteLine("Asset version is unknown!");

                    typeId = ReadAssetData(reader, 8);
                    groupId = ReadAssetData(reader, 8);
                    instanceId = ReadAssetData(reader, 8);
                    machineId = ReadAssetData(reader, 8);

                    assetId = Convert.ToInt64(ReadAssetData(reader, 16), 16);

                    if(version==6)
                    {
                        parentId = Convert.ToInt64(ReadAssetData(reader, 16), 16);
                    }

                    time = ReadAssetData(reader, 16);

                    var userLength = Convert.ToInt32(ReadAssetData(reader, 2), 16);
                    username = ReadAssetData(reader, userLength);
                    userId = Convert.ToInt64(ReadAssetData(reader, 16), 16);

                    var nameLength = Convert.ToInt32(ReadAssetData(reader, 2), 16);
                    name = ReadAssetData(reader, nameLength);
                    
                    var descLength = Convert.ToInt32(ReadAssetData(reader, 3), 16);
                    desc = ReadAssetData(reader, descLength);
                    
                    var tagsLength = Convert.ToInt32(ReadAssetData(reader, 2), 16);
                    tags = ReadAssetData(reader, tagsLength);

                    // Skip two zeroes
                    ReadAssetData(reader, 2);
                    // XML doc
                    var doc = reader.ReadToEnd();
                    var partCountIndex = doc.IndexOf("blocks count=")+14;
                    var endIndex = doc.IndexOf("\"", partCountIndex)-partCountIndex;
                    var partCountString = doc.Substring(partCountIndex, endIndex);
                    partCount = Convert.ToInt32(partCountString);

                    printInfo();
                }
            }
            //catch (Exception)
            {
                
            }
        }

        private static string ReadAssetData(StreamReader reader, int length)
        {
            var data = new char[length];
            reader.Read(data, 0, data.Length);
            return new string(data);
        }

        public long Id {get => assetId;}
        public string Name {get => name;}
        public string Author {get => username;}
        public long AuthorId {get => userId;}
        public string Created {get => time;}
        public string Description {get => desc;}
        public string Tags {get => tags;}
        //public string Type {get;}
        //public string Subtype {get;}
        //public double Rating {get;}
        public long Parent {get => parentId;}

        public string SporeWebUrl {get => "http://www.spore.com/sporepedia#qry=sast-"+Id;}

        public int PartCount {get => partCount;}

        public void printInfo()
        {
            Console.WriteLine($"{Name} (ID: {Id}) by {Author}");
        }

        public string getInfo()
        {
            var url = Id>0 ? SporeWebUrl : "*Unshared*";
            var original = Parent>0 ? "*Edited Creation*\n" : "";
            var desc = Description.Length>0 ? Description+"\n" : "";
            var tags = Tags.Length>0 ? " - Tags: "+Tags : "";
            
            return $"**{Name}** by {Author}\n{desc}*{PartCount} parts{tags}*\n{original}{url}";
        }

        public string getInfoAdvanced()
        {
            return $"Asset ID: {Id}\nAuthor ID: {AuthorId}\nCreated: {Created}\nParent Asset: {Parent}\nPart Count: {PartCount}\nPNG version: {version}\nType ID: {typeId}\nGroup ID: {groupId}\nInstance ID: {instanceId}\nMachine ID: {machineId}";
        }

    }
}