
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

<HideModuleName>
Public Module Ms1FeatureExtensions

    ''' <summary>
    ''' 从xcms程序包所生成的格式为``MaTb``的uid中产生一个选择的区间
    ''' </summary>
    ''' <param name="mzrt"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 利用得到的编号列表从另外的一个物质表中选取对应的feature
    ''' </remarks>
    <Extension>
    Public Function peakGroup(mzrt As String, Optional dt% = 5) As NamedValue(Of Index(Of String))
        Dim mz = mzrt.Match("M\d+")
        Dim rt = mzrt.Match("T\d+")

        If rt.StringEmpty Then
            ' 当前的mz只有一个物质的peak
            Return New NamedValue(Of Index(Of String)) With {
                .Name = mzrt,
                .Value = {mzrt}
            }
        Else
            rt = rt.Match("\d+")
        End If

        Dim rtInt As Integer = rt.ParseInteger
        Dim uidGroup = Iterator Function() As IEnumerable(Of String)
                           For t As Integer = rtInt - dt To rtInt + dt
                               Yield $"{mz}T{t}"
                           Next
                       End Function

        Return New NamedValue(Of Index(Of String)) With {
            .Name = mzrt,
            .Value = uidGroup().ToArray
        }
    End Function
End Module
