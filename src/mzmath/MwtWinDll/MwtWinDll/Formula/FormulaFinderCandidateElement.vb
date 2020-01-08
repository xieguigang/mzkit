#Region "Microsoft.VisualBasic::ec5c779a9ebd084cc907c0d049dc1915, src\mzmath\MwtWinDll\MwtWinDll\Formula\FormulaFinderCandidateElement.vb"

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

    ' Class FormulaFinderCandidateElement
    ' 
    '     Properties: Charge, CountMaximum, CountMinimum, Mass, OriginalName
    '                 PercentCompMaximum, PercentCompMinimum, Symbol
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Friend Class FormulaFinderCandidateElement

    ReadOnly mOriginalName As String

    Public Property Mass As Double
    Public Property Charge As Double

    Public Property CountMinimum As Integer
    Public Property CountMaximum As Integer

    Public Property PercentCompMinimum As Double
    Public Property PercentCompMaximum As Double

    ' ReSharper disable once ConvertToVbAutoProperty
    Public ReadOnly Property OriginalName As String
        Get
            Return mOriginalName
        End Get
    End Property

    Public Property Symbol As String

    Public Sub New(elementOrAbbrevSymbol As String)
        mOriginalName = String.Copy(elementOrAbbrevSymbol)
        Symbol = String.Copy(elementOrAbbrevSymbol)
    End Sub

    Public Overrides Function ToString() As String
        If Symbol = mOriginalName Then
            Return Symbol & ": " & Mass.ToString("0.0000") & " Da, charge " & Charge.ToString()
        Else
            Return mOriginalName & "(" & Symbol & "): " & Mass.ToString("0.0000") & " Da, charge " & Charge.ToString()
        End If
    End Function
End Class
