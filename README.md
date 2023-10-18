# XSnatch - XML Element Extractor

## Purpose
This program, **XSnatch**, allows the user to extract the value of a specific element from an XML file based on a set of provided command-line arguments. It constructs an XPath query from the arguments to locate and fetch the desired XML elements. Once located, the program can save the value of the extracted element to a specified output file.

## Usage
```
XSnatch.exe [inputFilePath] [targetElement] [parentElement*] [immediateParent*]
```
```
XSnatch.exe
```
- When used without arguments it defaults to a given case: Finding the value of the element `target` when the attribute `id` is `42007` and saving the result as `output.txt`

## Arguments
- **inputFilePath**: Relative path to an XML file. e.g., `filename.xml`.
- **targetElement**: Specifies the XML element whose value needs to be extracted.
- **parentElement** (Optional): Limits the search for the targetElement to only those elements that are children of the specified parentElement.
- **immediateParent** (Optional): If set, narrows down the search to immediate children of the specified parentElement.

## Element Formatting
The format for elements with attributes is: `element-attribute=value`. For instance:
- `elem`: Matches any element named `elem`.
- `*`: Matches any XML element.
- `*-type=34`: Matches any element with an attribute named `type` having the value `34`.
- `entry-id`: Matches an `<entry>` element with an `id` attribute of any value.

## Example Usage
```
XSnatch.exe sma_gentext.xml target *-id=42007
```

## Error Handling
- Checks for valid XML file input.
- Captures exceptions for file not found, invalid format, argument errors, and other general exceptions.

## Note
The provided program is internal and, therefore, is meant to be used only within its assembly. For further information or specific usage cases, refer to the inline comments and documentation.

**Author**: Sebastian Senic  
**Date**: 2023-10-18
