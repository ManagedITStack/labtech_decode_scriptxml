using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.IO.Compression;

namespace labtech_decode_scriptxml
{
    class Program
    {
        /* Exit codes that may be returned by this program */
        enum ExitCode : int
        {
            Success = 0,
            ShowedHelp = 1,
            InputFileNotFound = 2,
            InputFileInvalid = 3,
            OutputFileExists = 4,
            UnknownError = 100
        }

        /* Print the string padded ExitCodes Enum to the console. */
        static void WriteExitCodes()
        {
            Array values = Enum.GetValues(typeof(ExitCode));

            foreach (ExitCode val in values)
            {
                Console.WriteLine(String.Format("    {1,5}: {0}", Enum.GetName(typeof(ExitCode), val), (int)val));
            }
        }

        static int Main(string[] args)
        {
            /* If no arguments, return Help */
            if (args.Length < 1 || args[0] == "--help")
            {

                WriteHelp();
                return (int)ExitCode.ShowedHelp;

                /* If arguments, continue */
            }
            else
            {

                /* Define local variables */
                string inputPath = "";
                string outputPath = "";
                bool argOverwrite = false;

                /* Loop through arguments on at a time */
                foreach (string arg in args)
                {

                    /* Split argument into a list with first element being the label
                     * and second element being the value */
                    List<string> listArg = new List<string>(arg.Split('='));
                    string argLabel = listArg[0];
                    listArg.RemoveAt(0);
                    string[] arrValue = listArg.ToArray();
                    string argValue = String.Join("=", arrValue);

                    /* Map arguments to designaed local variables */
                    if (argLabel == "--input")
                    {
                        inputPath = argValue;
                    }
                    else if (argLabel == "--output")
                    {
                        outputPath = argValue;
                    }
                    else if (argLabel == "--overwrite" && argValue == "true")
                    {
                        argOverwrite = true;
                    }
                } // We've finished processing arguments, continue

                /* If we have both input and output files, continue */
                if (inputPath.Length > 0 && outputPath.Length > 0)
                {
                    string inputPathFile = "";

                    /* Check if the input file exists in the provided location */
                    try
                    {
                        if (File.Exists(inputPath))
                        {
                            inputPathFile = inputPath;
                        }
                        /* Check if the input file exists in the current working directory */
                        else if (File.Exists(Path.Combine(Environment.CurrentDirectory, inputPath)))
                        {
                            inputPathFile = Path.Combine(Environment.CurrentDirectory, inputPath);
                        }
                        /* Input file does not exist */
                        else
                        {
                            Console.WriteLine(" ");
                            Console.WriteLine("Could not open input file. Check path or permissions.");
                            return (int)ExitCode.InputFileNotFound;
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine(" ");
                        Console.WriteLine("Could not open input file. Check path or permissions.");
                        return (int)ExitCode.InputFileNotFound;
                    } // End checking if input file exists

                    /* Check if the input file is a valid XML file that can be opened */
                    XmlDocument inputXMLdoc = new XmlDocument();

                    try
                    {
                        inputXMLdoc.Load(Path.Combine(Environment.CurrentDirectory, inputPath));
                    }
                    catch (XmlException e)
                    {
                        Console.WriteLine(" ");
                        Console.WriteLine("  Problem reading the XML input file.");
                        Console.WriteLine("  " + e.ToString());
                        return (int)ExitCode.InputFileInvalid;
                    } // End checking if input file 

                    /* Deal with overwriting the output file */
                    try
                    {
                        /* We cannot write to the output file */
                        if (argOverwrite == false && File.Exists(outputPath))
                        {
                            Console.WriteLine(" ");
                            Console.WriteLine("Cannot write to output file.");
                            Console.WriteLine("Use --overwrite=true to overwrite existing output files.");
                            return (int)ExitCode.OutputFileExists;
                        }
                    }
                    /* We cannot write to the output file */
                    catch
                    {
                        Console.WriteLine(" ");
                        Console.WriteLine("Cannot write to output file.");
                        Console.WriteLine("Use --overwrite=true to overwrite existing output files.");
                        return (int)ExitCode.OutputFileExists;
                    } // End dealing with out file

                    /* Read XML file to find encoded string */
                    XmlNodeList scriptData = inputXMLdoc.GetElementsByTagName("ScriptData");
                    XmlNodeList licenseData = inputXMLdoc.GetElementsByTagName("LicenseData");

                    /* Decode the gzipped compressed string */
                    string scriptDataString = GunzipString(scriptData[0].InnerText);
                    string licenseDataString = GunzipString(licenseData[0].InnerText);

                    /* Write new string to the XML node */
                    scriptData[0].InnerText = "";
                    licenseData[0].InnerText = "";

                    /* Generate a new fragment from the decoded XML */
                    XmlDocumentFragment scriptDecodedNode = inputXMLdoc.CreateDocumentFragment();
                    XmlDocumentFragment licenseDecodedNode = inputXMLdoc.CreateDocumentFragment();
                    scriptDecodedNode.InnerXml = scriptDataString;
                    licenseDecodedNode.InnerXml = licenseDataString;

                    /* Replace the original node with the decode node */
                    XmlNode scriptDataParent = scriptData[0].ParentNode;
                    XmlNode licenseDataParent = licenseData[0].ParentNode;
                    scriptDataParent.ReplaceChild(scriptDecodedNode, scriptData[0]);
                    licenseDataParent.ReplaceChild(licenseDecodedNode, licenseData[0]);

                    /* Find the File nodes */
                    XmlNodeList filesNodes = inputXMLdoc.SelectNodes("LabTech_Expansion/PackedScript/File");

                    /* Loop through the File nodes */
                    foreach (XmlNode fileNode in filesNodes)
                    {
                        /* Don't touch invalid or empty File nodes */
                        if (!string.IsNullOrEmpty(fileNode.Attributes["Bytes"].Value) && !string.IsNullOrEmpty(fileNode.Attributes["Name"].Value))
                        {
                            /* Convert the Base 64 string to bytes */
                            byte[] filebytes = Convert.FromBase64String(fileNode.Attributes["Bytes"].Value);

                            /* Create the directory path if it doesn't exist */
                            if (!Directory.Exists(Path.GetDirectoryName(fileNode.Attributes["Name"].Value.Replace(@"L:\", @"C:\"))))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(fileNode.Attributes["Name"].Value.Replace(@"L:\", @"C:\")));
                            }

                            /* If we're overwriting, then overwrite the existing files in the path */
                            if (argOverwrite && File.Exists(fileNode.Attributes["Name"].Value.Replace(@"L:\", @"C:\")))
                            {
                                File.Delete(fileNode.Attributes["Name"].Value.Replace(@"L:\", @"C:\"));
                            }

                            /* Create a filestream */
                            FileStream fs = new FileStream(fileNode.Attributes["Name"].Value.Replace(@"L:\",@"C:\"),
                                                           FileMode.CreateNew,
                                                           FileAccess.Write,
                                                           FileShare.None);

                            /* Write the bytes to disk and close the file */
                            fs.Write(filebytes, 0, filebytes.Length);
                            fs.Close();
                        }
                    }

                    /* Create a new XMLdoc for output by copying the modified inputXML */
                    XmlDocument outputXMLdoc = new XmlDocument();
                    outputXMLdoc = inputXMLdoc;
                    outputXMLdoc.Save(outputPath);

                    /* We can write to the output file */
                    Console.WriteLine(" ");
                    Console.WriteLine("Writing output to: " + outputPath);

                    /* Return Success */
                    Console.WriteLine("Well, everything looks pretty good. Enjoy.");
                    return (int)ExitCode.Success;

                    /* We don't have all the required arguments :( */
                }
                else
                {
                    WriteHelp();
                    return (int)ExitCode.ShowedHelp;
                }
            }

        }

        static void WriteHelp()
        {
            Console.WriteLine(" ");
            Console.WriteLine("  ABOUT:                                   |----------------------------|");
            Console.WriteLine("                                           | Labtech Packed Script XML  |");
            Console.WriteLine("    Use this to decode Labtech packed      |    Decoder - Version 0.1   |");
            Console.WriteLine("    XML script files to an XML file.       |----------------------------|");
            Console.WriteLine(" ");
            Console.WriteLine("  USAGE: [optional arguments]");
            Console.WriteLine(" ");
            Console.WriteLine("    labtech_decode_scriptxml.exe");
            Console.WriteLine("                     --input=input.xml");
            Console.WriteLine("                     --output=output.txt");
            Console.WriteLine("                    [--overwrite=false|true]");
            Console.WriteLine(" ");
            Console.WriteLine("  EXIT CODES: ");
            Console.WriteLine(" ");
            WriteExitCodes();
            Console.WriteLine(" ");
        }

        public static string GzipString(string argUncompressedString)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream((Stream)memoryStream, CompressionMode.Compress))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)gzipStream))
                    {
                        streamWriter.WriteLine(argUncompressedString);
                        streamWriter.Close();
                    }
                }
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public static string GunzipString(string argCompressedString)
        {
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(argCompressedString)))
            {
                using (GZipStream gzipStream = new GZipStream((Stream)memoryStream, CompressionMode.Decompress))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)gzipStream))
                        return streamReader.ReadToEnd();
                }
            }
        }
    }
}
