Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models

Public Class ClassyfireInfoTable : Implements ICompoundClass

    ''' <summary>
    ''' Compound id in given database.
    ''' </summary>
    ''' <returns></returns>
    Public Property CompoundID As String
    Public Property kingdom As String Implements ICompoundClass.kingdom
    Public Property super_class As String Implements ICompoundClass.super_class
    Public Property [class] As String Implements ICompoundClass.class
    Public Property sub_class As String Implements ICompoundClass.sub_class
    Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

    Public Shared Iterator Function PopulateMolecules(anno As IEnumerable(Of ClassyfireAnnotation)) As IEnumerable(Of ClassyfireInfoTable)
        For Each compound In anno.GroupBy(Function(a) a.CompoundID)

        Next
    End Function
End Class
