#Region "Microsoft.VisualBasic::60e722d52bef6bc37fdb20a6d08ebb48, mzkit\src\visualize\TissueMorphology\SpatialMapping\SpotMap.vb"

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

'   Total Lines: 79
'    Code Lines: 36
' Comment Lines: 34
'   Blank Lines: 9
'     File Size: 2.17 KB


' Class SpotMap
' 
'     Properties: barcode, flag, physicalXY, SMX, SMY
'                 spotXY, STX, STY, TissueMorphology, X
'                 Y
' 
'     Function: GetSMDataPoints
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph

''' <summary>
''' A spot of spatial transcriptomics mapping to a 
''' collection of spatial metabolism pixels
''' </summary>
Public Class SpotMap : Implements IPoint2D

    ''' <summary>
    ''' the spot barcode
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property barcode As String
    ''' <summary>
    ''' tissue flag
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property flag As Integer
    ''' <summary>
    ''' the physical xy
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property physicalXY As Integer()
    ''' <summary>
    ''' the original raw spot xy
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property spotXY As Integer()

    ''' <summary>
    ''' pixel x of the spatial transcriptomics spot.
    ''' (after spatial transform)
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property STX As Double
    ''' <summary>
    ''' pixel y of the spatial transcriptomics spot.
    ''' (after spatial transform)
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property STY As Double

    <XmlAttribute> Public Property SMX As Integer()
    <XmlAttribute> Public Property SMY As Integer()

    ''' <summary>
    ''' tissue tag data of the SMdata
    ''' </summary>
    ''' <returns></returns>
    <XmlText> Public Property TissueMorphology As String

    Private ReadOnly Property X As Integer Implements IPoint2D.X
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return STX
        End Get
    End Property

    Private ReadOnly Property Y As Integer Implements IPoint2D.Y
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return STY
        End Get
    End Property

    Public Iterator Function GetSMDataPoints() As IEnumerable(Of Point)
        For i As Integer = 0 To SMX.Length - 1
            Yield New Point(SMX(i), SMY(i))
        Next
    End Function

End Class
