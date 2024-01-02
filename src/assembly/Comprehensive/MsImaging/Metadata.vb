#Region "Microsoft.VisualBasic::2bf81725ab993122f6246207d755dde5, mzkit\src\assembly\Comprehensive\MsImaging\Metadata.vb"

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

'   Total Lines: 63
'    Code Lines: 46
' Comment Lines: 5
'   Blank Lines: 12
'     File Size: 2.10 KB


'     Class Metadata
' 
'         Properties: [class], mass_range, physical_height, physical_width, resolution
'                     scan_x, scan_y
' 
'         Function: GetDimension, GetMetadata, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace MsImaging

    ''' <summary>
    ''' the mzpack metadata format for ms-imaging raw data
    ''' </summary>
    Public Class Metadata

        ''' <summary>
        ''' the max scan number in x axis
        ''' </summary>
        ''' <returns></returns>
        Public Property scan_x As Integer
        ''' <summary>
        ''' the max rows in y axis
        ''' </summary>
        ''' <returns></returns>
        Public Property scan_y As Integer
        ''' <summary>
        ''' only works for mzkit application <see cref="FileApplicationClass.MSImaging3D"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property scan_z As Integer
        ''' <summary>
        ''' the spatial resolution, usually be the average resolution on x and y axis
        ''' </summary>
        ''' <returns></returns>
        Public Property resolution As Double
        ''' <summary>
        ''' the ion m/z value range of the scan result.
        ''' </summary>
        ''' <returns></returns>
        Public Property mass_range As DoubleRange

        ''' <summary>
        ''' the string name of <see cref="FileApplicationClass"/>, 
        ''' not the description id value
        ''' </summary>
        ''' <returns></returns>
        Public Property [class] As String

        ''' <summary>
        ''' the calculated physical width of the sample slider data
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property physical_width As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return scan_x * resolution
            End Get
        End Property

        ''' <summary>
        ''' the calculated physical height of the sample slider data
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property physical_height As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return scan_y * resolution
            End Get
        End Property

        ''' <summary>
        ''' construct a blank ms-imaging metadata list
        ''' </summary>
        ''' <remarks>
        ''' for write metadata to mzpack file, use the <see cref="GetMetadata()"/>
        ''' function for generates the data dictionary object for write
        ''' </remarks>
        Sub New()
        End Sub

        Sub New(dimension As Size, Optional z As Integer = 0)
            scan_x = dimension.Width
            scan_y = dimension.Height
            scan_z = z
        End Sub

        ''' <summary>
        ''' Parse the list data which is read from the mzpack file
        ''' </summary>
        ''' <param name="list"></param>
        ''' <remarks>
        ''' for write metadata to mzpack file, use the <see cref="GetMetadata()"/> 
        ''' function for generates the data dictionary object for write
        ''' </remarks>
        Sub New(list As IDictionary(Of String, String))
            scan_x = Val(list.TryGetValue("width"))
            scan_y = Val(list.TryGetValue("height"))
            scan_z = Val(list.TryGetValue("z"))
            resolution = Val(list.TryGetValue("resolution"))
            mass_range = New DoubleRange(
                min:=Val(list.TryGetValue("mzmin")),
                max:=Val(list.TryGetValue("mzmax"))
            )
        End Sub

        ''' <summary>
        ''' get 2d canvas size
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetDimension() As Size
            Return New Size(scan_x, scan_y)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{mass_range.Min.ToString("F4")} - {mass_range.Max.ToString("F4")}] {scan_x}x{scan_y}@{resolution}um"
        End Function

        ''' <summary>
        ''' Contruct a dictionary object from current ms-imaging 
        ''' metadata object for write to mzpack file:
        ''' 
        ''' 1. width
        ''' 2. height
        ''' 3. resolution
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMetadata() As Dictionary(Of String, String)
            Dim datalist As New Dictionary(Of String, String) From {
                {"width", scan_x},
                {"height", scan_y},
                {"z", scan_z},
                {"resolution", resolution}
            }

            If Not mass_range Is Nothing Then
                datalist!mzmin = mass_range.Min
                datalist!mzmax = mass_range.Max
            End If

            Return datalist
        End Function

    End Class
End Namespace
