# SPGraphViz - Home

Project was moved from Codeplex as last one will be shut down in Dec 2017.
## Project Description
SPGraphviz - create your own visualization graphs without programming in Sharepoint. Just define graph on DOT language in simple text file, upload it into document library and specify URL in SPGraphvizWebPart - it will make graphical representation of your graph

![](https://github.com/sadomovalex/spgraphviz/blob/master/docs/Home_graphviz.png)

## Usage
* Define graph using the [DOT language](http://www.graphviz.org/pdf/dotguide.pdf) in simple .txt file. E.g. the graph shown at the beginning of this page is defiend using the following file:
```
digraph example {
        size="6,6";
        node [color=lightblue2, style=filled](color=lightblue2,-style=filled);
        "Start page" -> "Departments";
        "Start page" -> "News";
        "Start page" -> "Personal sites";
        "Departments" -> "IT";
        "Departments" -> "HR";
        "Departments" -> "Sales";
        "Personal sites" -> "Alexey Sadomov";
        "Personal sites" -> "...";
}
```
As you see it is quite straightforward: you define digraph and relationships between nodes. DOT is very flexible language, but you don't need to know it deeply. For simple scenarios you can use this example as a reference. For more layout settings you can check examples from [Graphviz gallery](http://www.graphviz.org/Gallery.php)

* Upload created .txt file with graph definition into document library in your site collection

* Specify absolute URL of uploaded file in web part properties (Custom Properties > Dot file URL):
![](https://github.com/sadomovalex/spgraphviz/blob/master/docs/Home_properties.png)
(in our example I specified the following URL in "Dot file URL" property: http://example.com/Documents/test.txt)

* Click Apply. Graphical representation of the graph will be shown on the publishing page:
![](https://github.com/sadomovalex/spgraphviz/blob/master/docs/Home_webpart.png)

## Additional resources
* Author's blog: [http://sadomovalex.blogspot.com](http://sadomovalex.blogspot.com)
* For more details about SPGraphvizWebPart's features and installation instructions: [Documentation](https://github.com/sadomovalex/spgraphviz/blob/master/docs/Documentation.md)
* DOT language reference: [http://www.graphviz.org/pdf/dotguide.pdf](http://www.graphviz.org/pdf/dotguide.pdf)
* Graphviz gallery: [http://www.graphviz.org/Gallery.php](http://www.graphviz.org/Gallery.php)
