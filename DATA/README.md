Compound database I/O provider and Mass Spectrum database I/O provider

## Visualize MRM Chromatogram Plots

```vbnet
Dim xlsx$ = "./ion-pairs.xlsx"
Dim ionPairs = Extensions.LoadIonPairs(xlsx, "ion pairs")

For Each file As String In ls - l - r - "*.mzML" <= directory
    Call ionPairs.Plot(file).AsGDIImage.SaveAs($"./{file.BaseName}.png")
Next
```

![](./Data20180111-L7-40(4).png)
![](./Data20180111-L7-40(5).png)
![](./Data20180111-L7-40(6).png)
![](./Data20180111-WASH.png)

## MRM QuantitativeAnalysis

![](HMDB0000925.png)