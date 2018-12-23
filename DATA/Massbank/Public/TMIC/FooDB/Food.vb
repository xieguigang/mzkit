#Region "Microsoft.VisualBasic::7a23d0f7e7f7a95ba31bbd72a174a206, Massbank\Public\TMIC\FooDB\Food.vb"

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

    '     Class FoodSource
    ' 
    '         Properties: content, food_general_name, food_id, food_name, foodb_id
    '                     group, HMDB, name, range, reference
    '                     unit
    ' 
    '         Function: ToString
    '         Class parser
    ' 
    '             Function: ToString, TryParse
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace TMIC.FooDB

    Public Class FoodSource

        Public Property foodb_id As String
        Public Property HMDB As String
        Public Property name As String
        Public Property content As Double
        <Column("range", GetType(parser))>
        Public Property range As DoubleRange
        Public Property unit As String
        Public Property food_id As String
        Public Property group As String
        ''' <summary>
        ''' 学名
        ''' </summary>
        ''' <returns></returns>
        Public Property food_name As String
        ''' <summary>
        ''' 俗名
        ''' </summary>
        ''' <returns></returns>
        Public Property food_general_name As String
        Public Property reference As String

        Public Overrides Function ToString() As String
            Return $"{name} @ {food_general_name} ({food_name}) = {content} ({unit})"
        End Function

        Private Class parser : Implements IParser

            Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
                With DirectCast(obj, DoubleRange)
                    Return $"{ .Min}~{ .Max}"
                End With
            End Function

            Public Function TryParse(cell As String) As Object Implements IParser.TryParse
                Return CType(cell, DoubleRange)
            End Function
        End Class
    End Class
End Namespace
