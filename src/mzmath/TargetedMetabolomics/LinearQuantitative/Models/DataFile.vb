#Region "Microsoft.VisualBasic::901a4ee8945c05750877b0e8a0a8a5a4, mzmath\TargetedMetabolomics\LinearQuantitative\Models\DataFile.vb"

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


    ' Code Statistics:

    '   Total Lines: 78
    '    Code Lines: 55 (70.51%)
    ' Comment Lines: 9 (11.54%)
    '    - Xml Docs: 77.78%
    ' 
    '   Blank Lines: 14 (17.95%)
    '     File Size: 2.84 KB


    '     Class DataFile
    ' 
    '         Properties: filename, FileType, ionPeaks
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CreateQuantifyData, GetPeakData, MeasureFileType, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace LinearQuantitative

    ''' <summary>
    ''' model for a single rawdata file
    ''' </summary>
    Public Class DataFile : Implements INamedValue

        <XmlAttribute> Public Property filename As String Implements INamedValue.Key

        ''' <summary>
        ''' the multiple ion peak area data
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("Compound")> Public Property ionPeaks As IonPeakTableRow()

        Public ReadOnly Property FileType As SampleFiles
            Get
                Return MeasureFileType()
            End Get
        End Property

        Sub New(filename As String, compounds As IEnumerable(Of IonPeakTableRow))
            _filename = filename
            _ionPeaks = compounds.ToArray
        End Sub

        Sub New()
        End Sub

        Public Function MeasureFileType() As SampleFiles
            If filename.IsPattern(".*kb[-\s]*\d*") Then
                Return SampleFiles.KB
            ElseIf filename.IsPattern(".*QC[-\s]*\d*") Then
                Return SampleFiles.QC
            ElseIf filename.IsPattern(".*((Cal)|(L))[-\s]*\d+") Then
                Return SampleFiles.Standard
            Else
                Return SampleFiles.Sample
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPeakData() As Dictionary(Of String, Double)
            Return ionPeaks.ToDictionary(Function(a) a.ID, Function(a) a.TPA)
        End Function

        Public Function CreateQuantifyData(linears As IEnumerable(Of StandardCurve)) As Dictionary(Of String, Double)
            Dim ions = ionPeaks.ToDictionary(Function(a) a.ID)
            Dim contents As New Dictionary(Of String, Double)

            If linears Is Nothing Then
                Call "Missing linear model for make sample data quantification.".Warning
            End If

            For Each line As StandardCurve In linears.SafeQuery
                If ions.ContainsKey(line.name) Then
                    If line.IS Is Nothing OrElse line.IS.CheckIsEmpty Then
                        ' use peak area
                        Call contents.Add(line.name, line.linear.GetY(ions(line.name).TPA))
                    ElseIf ions(line.name).TPA_IS <= 0 Then
                        Call contents.Add(line.name, Double.NaN)
                    Else
                        ' use the A/IS ratio
                        Call contents.Add(line.name, line.linear.GetY(ions(line.name).TPA / ions(line.name).TPA_IS))
                    End If
                Else
                    Call contents.Add(line.name, 0)
                End If
            Next

            Return contents
        End Function

        Public Overrides Function ToString() As String
            Return $"[{FileType}] {ionPeaks.Count} compounds@{filename}"
        End Function

    End Class
End Namespace
