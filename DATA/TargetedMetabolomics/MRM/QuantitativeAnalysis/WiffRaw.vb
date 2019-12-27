Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.MRM.Data
Imports SMRUCC.MassSpectrum.Math.MRM.Models

Public Module WiffRaw

    ''' <summary>
    ''' 从原始数据之中扫描峰面积数据，返回来的数据集之中的<see cref="DataSet.ID"/>是HMDB代谢物编号
    ''' </summary>
    ''' <param name="mzMLRawFiles">``*.wiff``，转换之后的结果文件夹，其中标准曲线的数据都是默认使用``L数字``标记的。</param>
    ''' <param name="ions">包括离子对的定义数据以及浓度区间</param>
    ''' <param name="TPAFactors">
    ''' ``{<see cref="Standards.HMDB"/>, <see cref="Standards.Factor"/>}``，这个是为了计算亮氨酸和异亮氨酸这类无法被区分的物质的峰面积所需要的
    ''' </param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function Scan(mzMLRawFiles$(),
                         ions As IonPair(),
                         peakAreaMethod As PeakArea.Methods,
                         TPAFactors As Dictionary(Of String, Double),
                         Optional ByRef refName$() = Nothing,
                         Optional removesWiffName As Boolean = False) As DataSet()

        Dim ionTPAs As New Dictionary(Of String, Dictionary(Of String, Double))
        Dim refNames As New List(Of String)
        Dim level$
        Dim wiffName$ = mzMLRawFiles _
            .Select(Function(path) path.ParentDirName) _
            .GroupBy(Function(name) name) _
            .OrderByDescending(Function(name) name.Count) _
            .First _
            .Key

        Call $"The wiff raw file name is: {wiffName}".__DEBUG_ECHO

        For Each ion As IonPair In ions
            ionTPAs(ion.accession) = New Dictionary(Of String, Double)
        Next

        For Each file As String In mzMLRawFiles
            ' 得到当前的这个原始文件之中的峰面积数据
            Dim TPA() = file.ScanTPA(
                ionpairs:=ions,
                peakAreaMethod:=peakAreaMethod,
                TPAFactors:=TPAFactors
            )

            refNames += file.BaseName
            level$ = file.BaseName

            If removesWiffName Then
                level = level.Replace(wiffName, "").Trim("-"c, " "c)
            End If

            For Each ion In TPA
                ionTPAs(ion.name).Add(level, ion.area)
            Next
        Next

        refName = refNames

        Return ionTPAs _
            .Select(Function(ion)
                        Return New DataSet With {
                            .ID = ion.Key,
                            .Properties = ion.Value
                        }
                    End Function) _
            .ToArray
    End Function
End Module
