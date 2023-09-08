Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' This is the class variable to store the internal query of molecular formula.
''' The queries are stored in .EFD file of the same folder of main program (should be).
''' The EFD file will be retrieved by ExistFormulaDbParcer.cs.
''' </summary>

Public Class ExistFormulaQuery
    Public Sub New()
        PubchemCidList = New List(Of Integer)()
        Formula = New Formula()
    End Sub

    Public Sub New(formula As Formula, pubchemCidList As List(Of Integer), formulaRecords As Integer, dbRecords As Integer, dbNames As String)
        Me.Formula = formula
        Me.PubchemCidList = pubchemCidList
        Me.FormulaRecords = formulaRecords
        ResourceNumber = dbRecords
        ResourceNames = dbNames
    End Sub


    Public Property Formula As Formula


    Public Property PubchemCidList As List(Of Integer)

    Public Property FormulaRecords As Integer

    Public Property ResourceNumber As Integer

    Public Property ResourceNames As String
End Class

