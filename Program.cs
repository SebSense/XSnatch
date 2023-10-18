using System.Xml;
namespace XSnatch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Define vars:
            string inputFile, targetName, parentAttribute, parentValue, outputFile;

            //if (args.Length == 0)
            //{
            inputFile = "sma_gentext.xml";
            targetName = "target";
            parentAttribute = "id";
            parentValue = "42007";
            outputFile = "output.txt";
            //}


            /* Verify filetype of inputFile as XML or throw exception
             * 
             * INITIALIZE inputType = inputFile.Split('.').Last()
             * INITIALIZE fileContents AS STRING
             * DEFINE targetValues AS LIST<STRING>
             * 
             * IF inputType = "XML":
             *      
             */     //Try to load the file as XML to fileContents or throw exception
            StreamReader inputStream = new StreamReader(File.Open(inputFile, FileMode.Open));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(inputStream);

            //Find the element by using XPATH:
            XmlNode targetNode = xmlDoc.SelectSingleNode($"//*[@{parentAttribute}='{parentValue}']//{targetName}");
            string result = targetNode.InnerText;
            Console.WriteLine(result);

            //Placeholders for additional filetype support:
            /* ELSE IF inputType = "JSON": throw exception
             * ELSE IF inputType = ... */

            //ELSE: throw exception (invalid file) 

            /* IF targetValues IS EMPTY: PRINT NOT-FOUND-MESSAGE AND RETURN
             * ELSE: outputString = targetValues.Join(',')
             * 
             * IF outputFile EXISTS: Ask to overwrite?
             *      IF YES: overwrite
             *      ELSE:   return
             * ELSE: write outputFile
             * return


            */
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.Write(result);
            }

        }
    }
}