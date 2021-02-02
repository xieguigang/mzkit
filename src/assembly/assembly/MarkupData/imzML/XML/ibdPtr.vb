#Region "Microsoft.VisualBasic::ff88c58385476cb00fe4429b93956687, assembly\MarkupData\imzML\XML\ibdPtr.vb"

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

    '     Class ibdPtr
    ' 
    '         Properties: arrayLength, encodedLength, offset, UnderlyingType
    ' 
    '         Function: ParsePtr, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    ''' <summary>
    ''' ibd pointer data
    ''' </summary>
    Public Class ibdPtr

        Public Property offset As Long
        Public Property arrayLength As Integer
        Public Property encodedLength As Integer

        Public ReadOnly Property UnderlyingType As Type
            Get
                Dim sizeof As Integer = encodedLength / arrayLength

                If sizeof = 4 Then
                    Return GetType(Single)
                Else
                    Return GetType(Double)
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[&{offset.ToHexString}] new {UnderlyingType.Name.ToLower}({arrayLength} - 1){{}}"
        End Function

        Friend Shared Function ParsePtr(bin As binaryDataArray) As ibdPtr
            Dim arrayLength As Integer = Integer.Parse(bin.cvParams.KeyItem("external array length").value)
            Dim offset As Long = Long.Parse(bin.cvParams.KeyItem("external offset").value)
            Dim encodedLength As Integer = Integer.Parse(bin.cvParams.KeyItem("external encoded length").value)

            Return New ibdPtr With {
                .offset = offset,
                .arrayLength = arrayLength,
                .encodedLength = encodedLength
            }
        End Function
    End Class
End Namespace
