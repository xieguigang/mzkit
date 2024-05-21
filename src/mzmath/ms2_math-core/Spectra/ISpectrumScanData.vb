Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Spectra

    ''' <summary>
    ''' an abstract spectrum source data model which is describ the source data model 
    ''' from the assembly rawdata file.
    ''' </summary>
    Public Interface ISpectrumScanData : Inherits IMs1Scan, IReadOnlyId

        ReadOnly Property ActivationMethod As ActivationMethods
        ReadOnly Property CollisionEnergy As Double
        ReadOnly Property Peaks As Double()
        ReadOnly Property PeaksIntensity As Double()

    End Interface
End Namespace