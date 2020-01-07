# Rainmeter-XmlParser
Xml Parser Plugin for Rainmeter


### Install
Download latest build https://github.com/e2e8/Rainmeter-XmlParser/releases

Drop XmlParser.dll into Rainmeter Plugins Folder

### Usage

The plugin requires a Source and a Query. The string result returned is just the result of the XPath query (serialized if it is a list of objects). 

In most cases the desired result will be to get the the inner text of some element. XPath provides functions for this:

* _text()_ gets the text from a node
* _string()_ converts XPath result node to a string
* _normalize-space()_ trims leading and trailing whitespace

**Source:** valid xml string

**Query:** XPath query. https://en.wikipedia.org/wiki/XPath.

**Join:** (Optional) A string to use when joining Xml elements if the query results in multiple elements. Default ","

Example:
```
[Message]
Measure=Plugin
Plugin=XmlParser.dll
Source=<message><warning>Hello World</warning></message>
Query=normalize-space(string(/message/warning/text()))
```
