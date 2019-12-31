#Region "Microsoft.VisualBasic::d964571334d960cf98e598afc1e7deac, DATA\Massbank\Public\TMIC\HMDB\Tables\NameValue.vb"

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

    '     Structure NameValue
    ' 
    '         Properties: ID, match, metabolite, name, type
    ' 
    '         Function: (+2 Overloads) Equals, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace TMIC.HMDB

    Public Structure NameValue : Implements IEquatable(Of NameValue)

        Public Property name As String
        Public Property match As String
        Public Property type As String
        Public Property metabolite As String
        Public Property ID As String

        Public Overrides Function ToString() As String
            Return $"name={name}, match={match}, metabolite={metabolite}, type={type}"
        End Function

        Public Overloads Function Equals(other As NameValue) As Boolean Implements IEquatable(Of NameValue).Equals
            Return other.ToString = Me.ToString
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If obj Is Nothing Then
                Return False
            ElseIf Not obj.GetType Is GetType(NameValue) Then
                Return False
            End If

            Return Equals(other:=DirectCast(obj, NameValue))
        End Function
    End Structure
End Namespace
