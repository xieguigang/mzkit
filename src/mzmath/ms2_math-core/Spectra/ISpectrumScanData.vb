Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Spectra

    ''' <summary>
    ''' an abstract spectrum source data model which is describ the source data model 
    ''' from the assembly rawdata file.
    ''' </summary>
    Public Interface ISpectrumScanData : Inherits IMs1Scan, IReadOnlyId, ISpectrumVector

        ReadOnly Property ActivationMethod As ActivationMethods
        ReadOnly Property CollisionEnergy As Double
        ReadOnly Property Charge As Integer
        ReadOnly Property Polarity As IonModes

    End Interface
End Namespace