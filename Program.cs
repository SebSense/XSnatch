/*
 * XSnatch - XML Element Extractor
 * 
 * Purpose:
 * This program, XSnatch, allows the user to extract the value of a specific element from an XML file based on 
 * a set of provided command-line arguments. It constructs an XPath query from the arguments to 
 * locate and fetch the desired XML elements. Once located, the program can save the value of the 
 * extracted element to a specified output file.
 * 
 * Usage:
 * XSnatch.exe [inputFilePath] [targetElement] [parentElement*] [immediateParent*]
 * XSnatch.exe 
 * - When used without arguments it defaults to a given case: Finding the value of the element `target` when the attribute `id` is `42007` and saving the result as `output.txt`
 * 
 * Arguments:
 * - inputFilePath: Relative path to an XML file. e.g., "filename.xml"
 * - targetElement: Specifies the XML element whose value needs to be extracted.
 * - parentElement (Optional): Limits the search for the targetElement to only those elements 
 *                            that are children of the specified parentElement.
 * - immediateParent (Optional): Either left out or "a". If set, narrows down the search to immediate children of the 
 *                              specified parentElement.
 * 
 * Element Formatting:
 * The format for elements with attributes is: "element attribute=value". For instance:
 * - 'elem': Matches any element named "elem".
 * - '*': Matches any XML element.
 * - '"* type=34"': Matches any element with an attribute named "type" having the value "34".
 * - '"entry id"': Matches an <entry> element with an "id" attribute of any value.
 * 
 * Example Usage:
 * XSnatch.exe sma_gentext.xml target "* id=42007"
 * 
 * Error Handling:
 * - Checks for valid XML file input.
 * - Captures exceptions for file not found, invalid format, argument errors, and other general 
 *   exceptions.
 * 
 * Note:
 * The provided program is internal and, therefore, is meant to be used only within its assembly.
 * For further information or specific usage cases, refer to the inline comments and documentation.
 * 
 * Author: Sebastian Senic
 * Date: 2023-10-18
 */
using System.Xml;
namespace XSnatch
{
    internal class Program
    {
        static int Main(string[] args)
        {
            string xpath = "//"; //Stringbuilder for an XPATH address to be used for searching through the .xml file.
            string xpathDivider = "//"; //Sets the xpath divider chars between parentElement and targetElement to releative.
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
                StreamWriter writer = new StreamWriter(outputFile);
                writer.Write(result);
                writer.Close();
                Console.WriteLine($"Saved result '{result}' to '{outputFile}'");
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
        private static string ArgToXpath(string arg)
        {
            //split arg to array of [element name, attribute, attribute, attribute, ...]
            string[] element = arg.Trim().Split();
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