Imports System.Collections.Generic

Namespace com.Demo2
    Public Class ChemicalFormula
        Private FormulaField As List(Of ChemicalElement)

        ''' <summary>
        ''' @return
        ''' </summary>
        Public Overridable Property formula As List(Of ChemicalElement)
            Get
                Return FormulaField
            End Get
            Set(ByVal value As List(Of ChemicalElement))
                FormulaField = value
            End Set
        End Property
    End Class
End Namespace
