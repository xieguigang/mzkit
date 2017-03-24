# spectrum
METLIN MS/MS spectrum data display

![](./spectrum/Plot.png)

```vbnet
' Test code
Call Data.Load("./SpectrumChart/Spectrum.json") _
    .Data(Scan0) _
    .Plot(title:="H<sub>2</sub>O<sub>5</sub>C<sub>3</sub>Ag</br><span style=""color:blue; font-size:20"">Test MS/MS spectra plot</span>") _
    .SaveAs("./spectrum/Plot.png")
```

## Molecular-Weight-Calculator
Imports from project: https://omics.pnl.gov/software/molecular-weight-calculator-net-dll-version