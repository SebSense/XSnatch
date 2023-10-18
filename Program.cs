using System.Xml;
namespace XSnatch
{
    internal class Program
    {
        /* XSnatch arguments:
         * 
         * example (win):
         * XSnatch.exe inputFilePath targetElement parentElement* immediateParent*
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
            string[] parentElement, targetElement;
            //Sets the xpath divider chars between parentElement and targetElement to releative.
            string xpathDivider = "//";
            string xpath = "//", xpathParent, xpathTarget;
            char[] trimChars = { '[', ']', ' ' };
            string arg;
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
                        arg = args[2].Trim(trimChars);
                        parentElement = arg.Split('-');
                        if (parentElement.Length > 1)
                        {
                            xpath += parentElement[0] + "[";
                            for (int i = 1; i < parentElement.Length; i++)
                            {
                                string attr = "@" + parentElement[i];
                                string key, value;
                                if (attr.Contains('='))
                                {
                                    key = attr.Split("=")[0];
                                    value = attr.Split("=")[1];
                                    if (value.First() == '"') value = value.Remove(0, 1);
                                    if (value.First() != '\'') value = "'" + value;
                                    if (value.Last() == '"') value = value.Remove(value.Length - 1, 1);
                                    if (value.Last() != '\'') value = value + "'";
                                    attr = key + "=" + value;
                                }
                                if (i > 1) attr = " and " + attr;
                                xpath += attr;
                            }
                            xpath += "]";
                        }
                        else xpath += arg;
                        xpath += xpathDivider;
                        goto case 2;
                    case 2:
                        arg = args[1].Trim(trimChars);
                        targetElement = arg.Split('-');
                        if (targetElement.Length > 1)
                        {
                            xpath += targetElement[0] + "[";
                            Console.WriteLine(xpath);
                            for (int i = 1; i < targetElement.Length; i++)
                            {
                                string attr = "@" + targetElement[i];
                                string key, value;
                                if (attr.Contains('='))
                                {
                                    key = attr.Split("=")[0];
                                    value = attr.Split("=")[1];
                                    if (value.First() != '\'') value = "'" + value;
                                    if (value.Last() != '\'') value = value + "'";
                                    attr = key + "=" + value;
                                }
                                if (i > 1) attr = " and " + attr;
                                xpath += attr;
                            }
                            xpath += "]";
                        }
                        else xpath += arg;
                        inputFile = args[0];
                        break;
                    case 0:
                        //If loaded with no args, enter data for the default case:
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
                        do
                        {
                            Console.Write("Enter filename >");
                            outputFile = Console.ReadLine();
                        } while (outputFile == String.Empty);
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
                            do{
                                Console.Write("Enter filename >");
                                outputFile = Console.ReadLine();
                            } while (outputFile == String.Empty);
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
    }
}