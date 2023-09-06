Imports System.Collections.Generic


Public Class FacadeLipidParser
        Implements ILipidParser
        Private ReadOnly map As Dictionary(Of String, List(Of ILipidParser)) = New Dictionary(Of String, List(Of ILipidParser))()

        Public ReadOnly Property Target As String = String.Empty Implements ILipidParser.Target

        Public Function Parse(ByVal lipidStr As String) As ILipid Implements ILipidParser.Parse
            Dim key = lipidStr.Split()(0)
            Dim parsers As List(Of ILipidParser) = Nothing, lipid As ILipid = Nothing

            If map.TryGetValue(key, parsers) Then
                For Each parser In parsers

                    If CSharpImpl.__Assign(lipid, TryCast(parser.Parse(lipidStr), ILipid)) IsNot Nothing Then
                        Return lipid
                    End If
                Next
            End If
            Return Nothing
        End Function

        Public Sub Add(ByVal parser As ILipidParser)
            If Not map.ContainsKey(parser.Target) Then
                map.Add(parser.Target, New List(Of ILipidParser)())
            End If
            map(parser.Target).Add(parser)
        End Sub

        Public Sub Remove(ByVal parser As ILipidParser)
            If map.ContainsKey(parser.Target) Then
                map(parser.Target).Remove(parser)
            End If
        End Sub

        Public Shared ReadOnly Property [Default] As ILipidParser
            Get
                If defaultField Is Nothing Then
                    Dim parser = New FacadeLipidParser()
                    Call New List(Of ILipidParser) From {
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

                    }.ForEach(New Action(Of ILipidParser)(AddressOf parser.Add))
                    defaultField = parser
                End If
                Return defaultField
            End Get
        End Property
        Private Shared defaultField As ILipidParser

End Class

