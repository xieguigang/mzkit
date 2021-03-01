#Region "Microsoft.VisualBasic::74725751c6a3556174043f6e928ae77a, metaDNA\Models\MassIndexKey.vb"

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

    ' Structure MassIndexKey
    ' 
    '     Function: ComparesMass, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

''' <summary>
''' Indexed of target compound by m/z
''' </summary>
Public Structure MassIndexKey

    Dim mz As Double
    Dim precursorType As String

    ''' <summary>
    ''' debug view
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return $"{precursorType} {mz}"
    End Function

    Friend Shared Function ComparesMass(tolerance As Tolerance) As Comparison(Of MassIndexKey)
        Return Function(x As MassIndexKey, b As MassIndexKey) As Integer
                   If tolerance(x.mz, b.mz) Then
                       Return 0
                   ElseIf x.mz > b.mz Then
                       Return 1
                   Else
                       Return -1
                   End If
               End Function
    End Function

End Structure
