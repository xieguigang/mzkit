#Region "Microsoft.VisualBasic::71ddb62d865bfaeedeb10435bcd7d467, mzkit\src\mzmath\mz_deco\xcms2.vb"

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

'   Total Lines: 88
'    Code Lines: 73
' Comment Lines: 3
'   Blank Lines: 12
'     File Size: 2.85 KB


' Class xcms2
' 
'     Properties: mz, mzmax, mzmin, npeaks, rt
'                 rtmax, rtmin
' 
'     Function: Load, totalPeakSum
' 
' Class PeakSet
' 
'     Properties: peaks, sampleNames
' 
'     Function: Norm, Subset
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' the peak table format table file model of xcms version 2
''' </summary>
Public Class xcms2 : Inherits DynamicPropertyBase(Of Double)

    Public Property ID As String
    Public Property mz As Double
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property rt As Double
    Public Property rtmin As Double
    Public Property rtmax As Double

    ''' <summary>
    ''' this feature has n sample data(value should be a positive number)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property npeaks As Integer
        Get
            Return Properties _
                .Where(Function(s) s.Value > 0) _
                .Count
        End Get
    End Property

    'Public Shared Function Load(file As String) As xcms2()
    '    Return DataSet _
    '        .LoadDataSet(Of xcms2)(file, uidMap:=NameOf(ID)) _
    '        .ToArray
    'End Function

    Public Overrides Function ToString() As String
        Return $"{ID}  {mz.ToString("F4")}@{rt.ToString("F4")}  {npeaks}peaks: {Properties.Keys.GetJson}"
    End Function

    Friend Function totalPeakSum() As xcms2
        Dim totalSum As Double = Properties.Values.Sum

        Return New xcms2 With {
            .ID = ID,
            .mz = mz,
            .mzmax = mzmax,
            .mzmin = mzmin,
            .rt = rt,
            .rtmax = rtmax,
            .rtmin = rtmin,
            .Properties = Properties.Keys _
                .ToDictionary(Function(name) name,
                              Function(name)
                                  Return Me(name) / totalSum * 10 ^ 8
                              End Function)
        }
    End Function
End Class

