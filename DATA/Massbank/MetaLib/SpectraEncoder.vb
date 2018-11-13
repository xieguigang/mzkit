#Region "Microsoft.VisualBasic::dac57cc30295e067c25f1ad473ddd973, Massbank\MetaLib\SpectraEncoder.vb"

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

    ' Module SpectraEncoder
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Function: Decode, GetEncoder
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text

''' <summary>
''' Spectra matrix encoder helper for mysql/csv
''' </summary>
Public Module SpectraEncoder

    Public Delegate Function Encoder(Of T)(mzData As T()) As String

    Public Function GetEncoder(Of T)(getX As Func(Of T, Double), getY As Func(Of T, Double)) As Encoder(Of T)
        Return Function(matrix As T()) As String
                   Dim table$ = matrix _
                       .Select(Function(m)
                                   Return {getX(m), getY(m)}.JoinBy(ASCII.TAB)
                               End Function) _
                       .JoinBy(vbCrLf)
                   Dim bytes As Byte() = TextEncodings.UTF8WithoutBOM.GetBytes(table)
                   Dim base64$ = bytes.ToBase64String

                   Return base64
               End Function
    End Function

    Public Function Decode(base64 As String) As (x#, y#)()
        Dim bytes As Byte() = Convert.FromBase64String(base64)
        Dim table$ = TextEncodings.UTF8WithoutBOM.GetString(bytes)
        Dim fragments$() = table.LineTokens
        Dim matrix = fragments _
            .Select(Function(r)
                        With r.Split(ASCII.TAB)
                            Return (x:=Val(.ByRef(0)), y:=Val(.ByRef(1)))
                        End With
                    End Function) _
            .ToArray

        Return matrix
    End Function
End Module

