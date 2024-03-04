Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' a generic compound model for metaDNA modelling
''' </summary>
Public Interface GenericCompound
    Inherits IReadOnlyId
    Inherits IExactMassProvider
    Inherits ICompoundNameProvider
    Inherits IFormulaProvider

End Interface
