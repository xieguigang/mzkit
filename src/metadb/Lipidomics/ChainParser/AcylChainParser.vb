#Region "Microsoft.VisualBasic::f10a6f17ff54c71259a30026aa9beaf3, E:/mzkit/src/metadb/Lipidomics//ChainParser/AcylChainParser.vb"

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

    '   Total Lines: 56
    '    Code Lines: 49
    ' Comment Lines: 1
    '   Blank Lines: 6
    '     File Size: 2.57 KB


    ' Class AcylChainParser
    ' 
    '     Function: Parse, ParseDoubleBond, ParseDoubleBondInfo, ParseOxidized
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions

Public Class AcylChainParser
    Private Shared ReadOnly CarbonPattern As String = "(?<carbon>\d+)"
    Private Shared ReadOnly DoubleBondPattern As String = "(?<db>\d+)(\(((?<dbpos>\d+[EZ]?),?)+\))?"
    Private Shared ReadOnly OxidizedPattern As String = "[;\(]((?<ox>O(?<oxnum>\d+)?)|((?<oxpos>\d+)OH,?)+\)?)"
    'private static readonly string OxidizedPattern = @"(\(?((?<oxpos>\d+)OH,?)+\)?|(?<ox>(?<oxnum>\d+)?O))";

    Public Shared ReadOnly Pattern As String = $"{CarbonPattern}:{DoubleBondPattern}({OxidizedPattern})?"
    Private Shared ReadOnly patternField As Regex = New Regex(Pattern, RegexOptions.Compiled)

    Public Function Parse(chainStr As String) As IChain
        Dim match = patternField.Match(chainStr)
        If match.Success Then
            Dim groups = match.Groups
            Dim numCarbon = Integer.Parse(groups("carbon").Value)
            Dim db = ParseDoubleBond(groups)
            Dim ox = ParseOxidized(groups)
            Return New AcylChain(numCarbon, db, ox)
        End If
        Return Nothing
    End Function

    Private Function ParseDoubleBond(groups As GroupCollection) As DoubleBond
        Dim dbnum = Integer.Parse(groups("db").Value)
        If groups("dbpos").Success Then
            Return New DoubleBond(dbnum, groups("dbpos").Captures.Cast(Of Capture)().[Select](Function(c) ParseDoubleBondInfo(c.Value)).ToArray())
        End If
        Return New DoubleBond(dbnum)
    End Function

    Private Function ParseDoubleBondInfo(bond As String) As DoubleBondInfo
        Select Case bond(bond.Length - 1)
            Case "E"c
                Return DoubleBondInfo.E(Integer.Parse(bond.TrimEnd("E"c)))
            Case "Z"c
                Return DoubleBondInfo.Z(Integer.Parse(bond.TrimEnd("Z"c)))
            Case Else
                Return DoubleBondInfo.Create(Integer.Parse(bond))
        End Select
    End Function

    Private Function ParseOxidized(groups As GroupCollection) As Oxidized
        If groups("oxpos").Success Then
            Dim oxpos = groups("oxpos").Captures.Cast(Of Capture)().[Select](Function(c) Integer.Parse(c.Value)).ToArray()
            Return New Oxidized(oxpos.Length, oxpos)
        End If
        If groups("ox").Success Then
            If groups("oxnum").Success Then
                Return New Oxidized(Integer.Parse(groups("oxnum").Value))
            End If
            Return New Oxidized(1)
        End If
        Return New Oxidized(0)
    End Function
End Class
