#Region "Microsoft.VisualBasic::4b289c7769d28c4804caadd18504206d, TargetedMetabolomics\RawFile.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Class RawFile
    ' 
    '     Properties: allSamples, blanks, hasBlankControls, numberOfStandardReference, patternOfRefer
    '                 samples, standards
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetLinearGroups, getLinearsGroup, GetRawFileList, hasPatternOf, (+2 Overloads) ScanDir
    '               ToString, WrapperForStandards
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

''' <summary>
''' a general file list data model of cdf files for 
''' GC-MS or mzML raw files from wiff converts output
''' </summary>
Public Class RawFile

    Public Property samples As String()
    Public Property standards As String()
    ''' <summary>
    ''' Blank controls of the <see cref="standards"/> reference
    ''' </summary>
    ''' <returns></returns>
    Public Property blanks As String()

    Public ReadOnly Property allSamples As String()
        Get
            Return standards.AsList + samples
        End Get
    End Property

    Public ReadOnly Property hasBlankControls As Boolean
        Get
            Return Not blanks.IsNullOrEmpty
        End Get
    End Property

    Public ReadOnly Property numberOfStandardReference As Integer
        Get
            Return getLinearsGroup(standards, patternOfRefer).Count
        End Get
    End Property

    Public Property patternOfRefer As String

    Private Sub New()
        samples = {}
        standards = {}
        blanks = {}
    End Sub

    ''' <summary>
    ''' 样本和标曲是在一起的
    ''' </summary>
    ''' <param name="directory$"></param>
    ''' <param name="patternOfRefer$"></param>
    ''' <param name="patternOfBlanks$"></param>
    Public Shared Function ScanDir(directory$,
                                   Optional patternOfRefer$ = ".+[-]CAL[-]?\d+",
                                   Optional patternOfBlanks$ = "KB[-]?\d+",
                                   Optional suffix$ = "*.mzML") As RawFile

        Dim mzML As String() = directory _
            .ListFiles(suffix) _
            .ToArray
        Dim standards As String() = mzML _
            .Where(Function(path)
                       Return hasPatternOf(path, patternOfRefer)
                   End Function) _
            .ToArray
        Dim blanks As String() = mzML _
            .Where(Function(path)
                       Return hasPatternOf(path, patternOfBlanks)
                   End Function) _
            .ToArray
        Dim samples As String() = mzML _
            .Where(Function(path)
                       Return Not hasPatternOf(path, patternOfRefer) AndAlso
                              Not hasPatternOf(path, patternOfBlanks)
                   End Function) _
            .ToArray

        Return New RawFile With {
            .patternOfRefer = patternOfRefer,
            .blanks = blanks,
            .standards = standards,
            .samples = samples
        }
    End Function

    ''' <summary>
    ''' 样本与标曲是分开的
    ''' </summary>
    ''' <param name="sampleDir$"></param>
    ''' <param name="referenceDir$"></param>
    ''' <param name="patternOfRefer$"></param>
    ''' <param name="patternOfBlanks$"></param>
    Public Shared Function ScanDir(sampleDir$, referenceDir$,
                                   Optional patternOfRefer$ = ".+[-]CAL[-]?\d+",
                                   Optional patternOfBlanks$ = "KB[-]?\d+",
                                   Optional suffix$ = "*.mzML") As RawFile

        Dim mzML As String() = referenceDir.ListFiles(suffix).ToArray
        Dim samples As String() = sampleDir.ListFiles(suffix).ToArray
        Dim standards As String() = mzML _
           .Where(Function(path)
                      Return hasPatternOf(path, patternOfRefer)
                  End Function) _
           .ToArray
        Dim blanks As String() = mzML _
            .Where(Function(path)
                       Return hasPatternOf(path, patternOfBlanks)
                   End Function) _
            .ToArray

        Return New RawFile With {
            .samples = samples,
            .standards = standards,
            .blanks = blanks,
            .patternOfRefer = patternOfRefer
        }
    End Function

    Public Function GetLinearGroups() As Dictionary(Of String, String())
        Return getLinearsGroup(standards, patternOfRefer) _
            .ToDictionary _
            .FlatTable
    End Function

    ''' <summary>
    ''' Get raw file list
    ''' </summary>
    ''' <returns></returns>
    Public Function GetRawFileList() As Dictionary(Of String, String())
        Return New Dictionary(Of String, String()) From {
            {NameOf(standards), standards},
            {NameOf(blanks), blanks},
            {NameOf(samples), samples}
        }
    End Function

    Private Shared Function hasPatternOf(path$, pattern As String) As Boolean
        Return Not path _
            .BaseName _
            .Match(pattern, RegexICSng) _
            .StringEmpty
    End Function

    Private Shared Iterator Function getLinearsGroup(standards As IEnumerable(Of String), patternOfRefer$) As IEnumerable(Of NamedValue(Of String()))
        Dim groups = standards _
            .GroupBy(Function(fileName)
                         Return fileName.BaseName.StringReplace(patternOfRefer, "")
                     End Function) _
            .ToArray

        For Each group As IGrouping(Of String, String) In groups
            Yield New NamedValue(Of String()) With {
                .Name = group.Key,
                .Value = group.ToArray
            }
        Next
    End Function

    Public Shared Function WrapperForStandards(standards As String(), patternOfRefer$) As RawFile
        Return New RawFile With {
            .patternOfRefer = patternOfRefer,
            .standards = standards
        }
    End Function

    Public Overrides Function ToString() As String
        If blanks.IsNullOrEmpty Then
            Return $"{samples.Length} samples with {standards.Length} reference point."
        Else
            Return $"{samples.Length} samples with {standards.Length} reference point and {blanks.Length} blanks."
        End If
    End Function
End Class
