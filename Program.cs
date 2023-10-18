using System.Xml;
namespace XSnatch
{
    internal class Program
    {
        static int Main(string[] args)
        {
            // Define vars:
            string inputFile, targetName, parentName, parentAttributeKey, parentAttributeValue, outputFile;

            //if (args.Length == 0)
            //{
            inputFile = "sma_gentext.xml";
            targetName = "target";
            parentName = "*";
            parentAttributeKey = "id";
            parentAttributeValue = "42007";
            outputFile = "output.txt";
            //}
            //NYI: Get the arguments from the command line.
            try
            {
                //Open file, search and save match to var
                string result;
                using (StreamReader inputStream = new StreamReader(File.Open(inputFile, FileMode.Open)))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(inputStream);

                    //Find the element by using XPATH:
                    string xpath = $"//{parentName}[@{parentAttributeKey}='{parentAttributeValue}']//{targetName}";
                    result = xmlDoc.SelectSingleNode(xpath).InnerText;
                }
                //Write to file
                //TODO: Add prompting for overwrite, rename or quit
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    writer.Write(result);
                    Console.WriteLine($"Found element <{targetName}> Value: '{result}'");
                    Console.Write($"Saved to {outputFile}");

                }
                return 0;
            }
            catch (FileNotFoundException ex) // File doesn't exist
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }
        }
    }
}