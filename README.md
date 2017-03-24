# spectrum
METLIN MS/MS spectrum data display

![](./spectrum/Plot.png)

```vbnet
' Test code
Call Data.Load("./SpectrumChart/Spectrum.json") _
	.Data(Scan0) _
    .Plot _
    .SaveAs("./spectrum/Plot.png")
```

## Molecular-Weight-Calculator
Imports from project: https://omics.pnl.gov/software/molecular-weight-calculator-net-dll-version