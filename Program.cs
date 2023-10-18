namespace XSnatch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* Define vars:
             * DECLARE inputFile, targetKey, parentKey, parentValue, outputFile AS STRINGS
             * IF Args: INITIALIZE vars FROM Args
             * ELSE:    INITIALIZE DEFAULT vars*/


            /* Verify filetype of inputFile as XML or throw exception
             * 
             * INITIALIZE inputType = inputFile.Split('.').Last()
             * INITIALIZE fileContents AS STRING
             * DEFINE targetValues AS LIST<STRING>
             * 
             * IF inputType = "XML":
             *      
             */     //Try to load the file as XML to fileContents or throw exception

            /*Extract the value(s)
     *      FOREACH parentKey:parentValue IN inputFile: 
     *      IF targetKey: ADD targetValue to targetValues
     */

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


        }
    }
}