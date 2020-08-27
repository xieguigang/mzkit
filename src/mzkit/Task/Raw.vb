#Region "Microsoft.VisualBasic::ef3d7fa15a94ee2bf09368c587a9633a, src\mzkit\Task\Raw.vb"

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

    ' Class Raw
    ' 
    '     Properties: cache, numOfScans, rtmax, rtmin, scans
    '                 source
    ' 
    ' Class ScanEntry
    ' 
    '     Properties: charge, id, intensity, mz, polarity
    '                 rt
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class Raw

    Public Property source As String
    Public Property cache As String

    Public Property rtmin As Double
    Public Property rtmax As Double

    Public ReadOnly Property numOfScans As Integer
        Get
            If scans.IsNullOrEmpty Then
                Return 0
            Else
                Return scans.Length
            End If
        End Get
    End Property

    Public Property scans As ScanEntry()

End Class

Public Class ScanEntry

    Public Property id As String
    Public Property mz As Double
    Public Property rt As Double
    Public Property charge As Double
    Public Property intensity As Double
    Public Property polarity As Integer

    Public Overrides Function ToString() As String
        Return id
    End Function
End Class
