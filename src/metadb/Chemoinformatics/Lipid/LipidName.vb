Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq

Public Class LipidName

    Public Property className As String
    Public Property chains As Chain()

    Public ReadOnly Property hasStructureInfo As Boolean
        Get
            Return chains.All(Function(c) c.hasStructureInfo)
        End Get
    End Property

    Public Overrides Function ToString() As String
        If chains.Length = 1 AndAlso Not chains(Scan0).hasStructureInfo Then
            Return ToOverviewName()
        Else
            Return ToSystematicName()
        End If
    End Function

    Public Function ToSystematicName() As String
        Return $"{className}({(From chain As Chain
                               In chains
                               Let str = chain.ToString
                               Select str).JoinBy("_")})"
    End Function

    ''' <summary>
    ''' $"{className}({totalCarbons}:{totalDBes})"
    ''' </summary>
    ''' <returns></returns>
    Public Function ToOverviewName() As String
        Dim totalCarbons As Integer = Aggregate c As Chain In chains Into Sum(c.carbons)
        Dim totalDBes As Integer = Aggregate c As Chain In chains Into Sum(c.doubleBonds)

        Return $"{className}({totalCarbons}:{totalDBes})"
    End Function

    ''' <summary>
    ''' parse lipid name components
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Shared Function ParseLipidName(name As String) As LipidName
        Static namePattern1 As New Regex("[a-zA-Z0-9]+\(.+\)")
        Static namePattern2 As New Regex("[a-zA-Z0-9]+\s+.+")

        Dim className As String
        Dim components As String

        If name.IsPattern(namePattern1) Then
            Dim tokens = name.GetTagValue("(", trim:=True)

            className = tokens.Name
            components = tokens.Value
            components = components.Substring(0, components.Length - 1)
        Else
            Dim tokens = name.GetTagValue(" ", trim:=True)

            className = tokens.Name
            components = tokens.Value.Trim
        End If

        Return New LipidName With {
            .className = className,
            .chains = ChainParser(components).ToArray
        }
    End Function

    Private Shared Iterator Function ChainParser(components As String) As IEnumerable(Of Chain)
        Dim parts As String() = components.StringSplit("[/_]")

        For Each components In parts
            Yield Chain.ParseName(components)
        Next
    End Function
End Class
