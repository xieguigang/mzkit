Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite

''' <summary>
''' KEGG代谢物数据模型
''' </summary>
Public Class KEGGMetabolite : Inherits MetaboliteAnnotation

    ''' <summary>该代谢物参与的所有KEGG通路ID集合</summary>
    Public Property Pathways As New HashSet(Of String)

    ''' <summary>
    ''' 从分子式重新计算精确分子量 (当ExactMass不可靠时使用)
    ''' </summary>
    Public Function RecalculateMass() As Double
        If String.IsNullOrEmpty(Formula) Then
            Return ExactMass
        Else
            Dim mass = FormulaScanner.EvaluateExactMass(Formula)
            If mass > 0 Then
                ExactMass = mass
            End If
            Return ExactMass
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"{Id} ({CommonName})"
    End Function
End Class

''' <summary>
''' KEGG代谢通路数据模型
''' </summary>
Public Class KEGGPathway

    ''' <summary>KEGG Pathway ID, 例如 map00010 (糖酵解/糖异生)</summary>
    Public Property ID As String

    ''' <summary>通路名称</summary>
    Public Property Name As String

    Public Property Description As String

    ''' <summary>该通路包含的所有代谢物ID集合</summary>
    Public Property Metabolites As New HashSet(Of String)

    Public Overrides Function ToString() As String
        Return $"{ID} ({Name})"
    End Function
End Class