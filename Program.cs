using System.Xml;
namespace XSnatch
{
    internal class Program
    {
        /* XSnatch arguments:
         * 
         *          XSnatch.exe inputFilePath targetElement parentElement* immediateParent*
         *          
         * example: XSnatch.exe sma_gentext.xml target *-id=42007
         * 
         *  * = optional
         *
         *      Argument        Example                 Explanation
         *  inputFilePath   |   filename.xml    |    relative path to an .xml file
         *  targetElement   |   elem            |    the element whose value you want to extract.
         *  parentElement   |   elem-attr=val   |    an (optional) parent element. The program will only look for the target value match in this elements child-elements
         *  immediateParent |   a               |    if included, makes the program only search through the immediate child-elements of the parent. Leave it out for default - searching through all child-elements at any depth.
         *
         *  element formatting:
         *      elem-attr=val-attr=val
         *  where elem is the element you are looking for and attr is an optional attribute with value val
         *                      
         *                      Example         |       Match           
         *                  |   elem            |   matches any "elem" element
         *                  |   *               |   matches any element
         *                  |   *-type=34       |   matches any element that has an attribute called type with value 34
         *                  |   entry-id        |   matches an <entry> element that has an attribute called id with any value
         * 
         */
        static int Main(string[] args)
        {
            string xpath = "//"; //Stringbuilder for an XPATH address to be used for searching through the .xml file.
            //Sets the xpath divider chars between parentElement and targetElement to releative.
            string xpathDivider = "//";
            string inputFile, outputFile = "output.txt";

            try
            {
                switch (args.Length)
                {
                    case 4:
                        //Changes the xpath divider between parentElement and targetElement to be absolute, this will only point to immediate children of parentElement
                        if (args[3] == "a") xpathDivider = "/";
                        goto case 3;
                    case 3:
                        //Convert the arg to an xpath string and add it to stringbuilder
                        xpath += ArgToXpath(args[2]);
                        xpath += xpathDivider;
                        goto case 2;
                    case 2:
                        //Convert the arg to an xpath string and add it to stringbuilder
                        xpath += ArgToXpath(args[1]);
                        inputFile = args[0];
                        break;
                    case 0:
                        //If loaded with no args, enter hardcoded data for the default case:
                        inputFile = "sma_gentext.xml";
                        xpath = "//*[@id='42007']//target";
                        break;
                    default:
                        throw new ArgumentException("incorrect number of args. Syntax is: 'XSnatch inputFile targetElement parentElement(optional) absoluteParent(optional)'. See reame.md for more help");
                }
                //Check that the file extension is xml
                if (inputFile.Split('.').Last().ToLower() != "xml")
                    throw (new FormatException("input file must be an .xml file."));
                //Open file, search and save match to var
                string result;
                using (StreamReader inputStream = new StreamReader(File.Open(inputFile, FileMode.Open)))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(inputStream);

                    //DEBUG: Print the XPATH:
                    //Console.WriteLine($"Searching with XPATH: \"{xpath}\"");

                    //Find the element by using XPATH:
                    result = xmlDoc.SelectSingleNode(xpath).InnerText;
                }

                Console.WriteLine($"Found element! Value: '{result}'");
                //Prompt user to save
                while (true)
                {
                    Console.Write($"Save result to '{outputFile}'? (yes/rename/quit) >");
                    string command = Console.ReadLine().ToLower();
                    if (command == "q" || command == "quit" || command == "no" || command == "n")
                        return 0;
                    if (command == "r" || command == "rename")
                    {
                        outputFile = GetInput("Enter filename >");
                        break;
                    }
                    if (command == "y" || command == "yes")
                        break;
                }
                //Write to file
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    //Prompt user to overwrite, rename or abort:
                    while (File.Exists(outputFile))
                    {
                        Console.Write($"File {outputFile} already exists. Overwrite? (yes/rename/quit) >");
                        string command = Console.ReadLine().ToLower();
                        if (command == "r" || command == "rename")
                        {
                            outputFile = GetInput("Enter filename >");
                        }  
                        else if (command == "q" || command == "quit")
                        {
                            return 0;
                        }
                        else if (command == "y" || command == "yes")
                        {
                            break;
                        }
                    }
                    writer.Write(result);
                    Console.Write($"Saved to {outputFile}");
                }
                return 0;
            }
            catch (NullReferenceException) // No match
            {
                Console.WriteLine($"Search complete. No match found.");
                return 0;
            }
            catch (FileNotFoundException ex) // File doesn't exist
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }
            catch (FormatException ex) //User tried to input a non .xlm filename
            {
                Console.WriteLine("Error: " + ex.Message);
                return 2;
            }
            catch (ArgumentException ex) // User tried to input a non .xlm filename
            {
                Console.WriteLine("Error: " + ex.Message);
                return 3;
            }
            catch (Exception ex) // Unknown error
            {
                Console.WriteLine("Error: " + ex.Message);
                return 4;
            }
        }
        //string ArgToXpath(string, char) - Takes a command-line argument and reformats it into a part of an XPATH element query
        //splitChar is the char used in the input string to separate name and attributes in the element.
        private static string ArgToXpath(string arg, char splitChar = '-')
        {
            //split arg to array of [element name, attribute, attribute, attribute, ...]
            string[] element = arg.Trim().Split(splitChar);
            string result = "";
            if (element.Length > 1)
            //if the element had attributes:
            {
                //start building the result string with the element name and bracket to start adding attributes
                result += element[0] + "[";
                //the rest of the strings in the array are attributes. Add them to the string builder:
                for (int i = 1; i < element.Length; i++)
                {
                    string attr = "@" + element[i];
                    string key, value;
                    if (attr.Contains('='))
                    //If the string has defined an attribute with a value this makes sure the value is surrounded by quotes:
                    {
                        key = attr.Split("=")[0];
                        value = attr.Split("=")[1];
                        if (value.First() != '\'' && value.First() != '"') value = "'" + value;
                        if (value.Last() != '\'' && value.Last() != '"') value = value + "'";
                        attr = key + "=" + value;
                    }
                    if (i > 1) attr = " and " + attr;
                    result += attr;
                }
                result += "]";
            }
            else result += arg;
            return result;
        }

        //string GetInput(string) - Gets a string from the user
        static private string GetInput(string query)
        {
            string input = String.Empty;
            while(input == String.Empty)
            {
                Console.Write(query);
                input = Console.ReadLine();
            }
            return input;
        }
    }

}