# ImportCSV

This project allow you to quickly import very large CSV files


```
public class CodeLists
{
  public int A { get; set; }
  public float B { get; set; }
  public string C { get; set; }
}


IEnumerable<CodeLists> codelist = ImportCSV.Import<CodeLists>(File.OpenRead(@"C:\temp\veryBIGcsvFile.csv"), ';');
// do your stuff... :-)

```

