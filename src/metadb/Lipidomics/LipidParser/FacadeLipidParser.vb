#Region "Microsoft.VisualBasic::59207ceb92f54062e1c536883c13289e, metadb\Lipidomics\LipidParser\FacadeLipidParser.vb"

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

    '   Total Lines: 102
    '    Code Lines: 93
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 4.17 KB


    ' Class FacadeLipidParser
    ' 
    '     Properties: [Default], Target
    ' 
    '     Function: Parse
    ' 
    '     Sub: Add, Remove
    ' 
    ' /********************************************************************************/

#End Region

Public Class FacadeLipidParser
    Implements ILipidParser
    Private ReadOnly map As New Dictionary(Of String, List(Of ILipidParser))()

    Public ReadOnly Property Target As String = String.Empty Implements ILipidParser.Target

    Public Function Parse(lipidStr As String) As ILipid Implements ILipidParser.Parse
        Dim key = lipidStr.Split()(0)
        Dim parsers As List(Of ILipidParser) = Nothing, lipid As ILipid = Nothing

        If map.TryGetValue(key, parsers) Then
            For Each parser In parsers

                lipid = TryCast(parser.Parse(lipidStr), ILipid)

                If lipid IsNot Nothing Then
                    Return lipid
                End If
            Next
        End If
        Return Nothing
    End Function

    Public Sub Add(parser As ILipidParser)
        If Not map.ContainsKey(parser.Target) Then
            map.Add(parser.Target, New List(Of ILipidParser)())
        End If
        map(parser.Target).Add(parser)
    End Sub

    Public Sub Remove(parser As ILipidParser)
        If map.ContainsKey(parser.Target) Then
            map(parser.Target).Remove(parser)
        End If
    End Sub

    Public Shared ReadOnly Property [Default] As ILipidParser
        Get
            If defaultField Is Nothing Then
                Dim parser = New FacadeLipidParser()
                Call New ILipidParser() {
                        New BMPLipidParser(),
                        New CLLipidParser(),
                        New DGLipidParser(),
                        New HBMPLipidParser(),
                        New LPCLipidParser(),
                        New LPELipidParser(),
                        New LPGLipidParser(),
                        New LPILipidParser(),
                        New LPSLipidParser(),
                        New MGLipidParser(),
                        New PALipidParser(),
                        New PCLipidParser(),
                        New PELipidParser(),
                        New PGLipidParser(),
                        New PILipidParser(),
                        New PSLipidParser(),
                        New TGLipidParser(),
                        New DGLipidParser(),
                        New EtherPCLipidParser(),
                        New EtherPELipidParser(),
                        New EtherLPCLipidParser(),
                        New EtherLPELipidParser(),
                        New SMLipidParser(),
                        New CeramideLipidParser(),
                        New HexCerLipidParser(),
                        New Hex2CerLipidParser(),
                        New DGTALipidParser(),
                        New DGTSLipidParser(),
                        New LDGTALipidParser(),
                        New LDGTSLipidParser(),
                        New GM3LipidParser(),
                        New SHexCerLipidParser(),
                        New CARLipidParser(),
                        New CLLipidParser(),
                        New DMEDFAHFALipidParser(),
                        New DMEDFALipidParser(),
                        New CELipidParser(),
                        New PCd5LipidParser(),
                        New PEd5LipidParser(),
                        New PId5LipidParser(),
                        New PGd5LipidParser(),
                        New PSd5LipidParser(),
                        New LPCd5LipidParser(),
                        New LPEd5LipidParser(),
                        New LPId5LipidParser(),
                        New LPGd5LipidParser(),
                        New LPSd5LipidParser(),
                        New CeramideNsD7LipidParser(),
                        New SMd9LipidParser(),
                        New DGd5LipidParser(),
                        New TGd5LipidParser(),
                        New CEd7LipidParser()
                    }.ForEach(Sub(par, nil) parser.Add(par))
                defaultField = parser
            End If
            Return defaultField
        End Get
    End Property
    Private Shared defaultField As ILipidParser

End Class
