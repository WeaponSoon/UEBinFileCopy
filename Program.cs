using System;
using System.Xml;
using System.IO;

namespace UEFileCheckout
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine("请指定源目录和目标目录");
                return;
            }

            string SourceDir = args[0];
            if(!Directory.Exists(SourceDir))
            {
                Console.WriteLine("源目录无效");
                return;
            }
            string DestDir = args[1];
            var DestDirInfo = Directory.CreateDirectory(DestDir);
            if(!Directory.Exists(DestDir))
            {
                Console.WriteLine("目标目录无效或创建失败");
                return;
            }

            string DependencyListPath = Path.Combine(SourceDir, ".ue4dependencies");
            if(!File.Exists(DependencyListPath))
            {
                Console.WriteLine("目标目录中无.ue4dependencies文件");
                return;
            }

            File.Copy(DependencyListPath, Path.Combine(DestDir, ".ue4dependencies"), true);

            FileStream DependencyFile = new FileStream(DependencyListPath, FileMode.Open);
            XmlDocument DependencyXml = new XmlDocument();
            DependencyXml.Load(DependencyFile);
            XmlNode ManifectNode = DependencyXml.SelectSingleNode("WorkingManifest");
            if(ManifectNode != null)
            {
                XmlNode FilesNode = ManifectNode.SelectSingleNode("Files");
                for(int FileId = 0; FileId < FilesNode.ChildNodes.Count; ++FileId)
                {
                    XmlNode FileNode = FilesNode.ChildNodes[FileId];
                    string FileRelativePath = FileNode.Attributes.GetNamedItem("Name").Value;
                    string FileSourcePath = Path.Combine(SourceDir, FileRelativePath);
                    string FileDestPath = Path.Combine(DestDir, FileRelativePath);

                    if(File.Exists(FileSourcePath))
                    {
                        Console.WriteLine("Copy File " + FileId + "/" + FilesNode.ChildNodes.Count + " : " + FileSourcePath + " --> " + FileDestPath);
                        string DestDirectDir = Path.GetDirectoryName(FileDestPath);
                        if(!Directory.Exists(DestDirectDir))
                        {
                            Directory.CreateDirectory(DestDirectDir);
                        }
                        File.Copy(FileSourcePath, FileDestPath, true);
                    }
                    else
                    {
                        Console.WriteLine("Copy File " + FileId + "/" + FilesNode.ChildNodes.Count + " : " + FileSourcePath + " does not exsiet");
                    }

                }
            }
            Console.WriteLine("Done!");
            
        }
    }
}
