#Region "Microsoft.VisualBasic::076c3b270f78b15554c88d09c13a92a7, E:/mzkit/src/metadb/Lipidomics//LipidClassDictionary.vb"

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

    '   Total Lines: 76
    '    Code Lines: 61
    ' Comment Lines: 1
    '   Blank Lines: 14
    '     File Size: 2.76 KB


    ' Class LipidClassProperty
    ' 
    '     Properties: [Class], AcylChain, AlkylChain, DisplayName, SphingoChain
    '                 TotalChain
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' Class LipidClassDictionary
    ' 
    '     Properties: [Default], LbmItems
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ParseDictinary
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO

Public Class LipidClassProperty
        Public Sub New([class] As LbmClass, displayName As String, acylChain As Integer, alkylChain As Integer, sphingoChain As Integer)
            Me.Class = [class]
            Me.DisplayName = displayName
            Me.AcylChain = acylChain
            Me.AlkylChain = alkylChain
            Me.SphingoChain = sphingoChain
        End Sub

        Public ReadOnly Property [Class] As LbmClass
        Public ReadOnly Property DisplayName As String

        Public ReadOnly Property TotalChain As Integer
            Get
                Return AcylChain + AlkylChain + SphingoChain
            End Get
        End Property
        Public ReadOnly Property AcylChain As Integer
        Public ReadOnly Property AlkylChain As Integer
        Public ReadOnly Property SphingoChain As Integer
        ' public int ExtraAcylChain { get; }
    End Class

Public Class LipidClassDictionary

    Public ReadOnly Property LbmItems As IReadOnlyDictionary(Of LbmClass, LipidClassProperty)
        Get
            Return m_lbmItems
        End Get
    End Property

    ReadOnly m_lbmItems As Dictionary(Of LbmClass, LipidClassProperty)

    Private Sub New()
        m_lbmItems = New Dictionary(Of LbmClass, LipidClassProperty)()
    End Sub

    Public Shared ReadOnly Property [Default] As LipidClassDictionary
        Get
            Static defaultField As LipidClassDictionary

            If defaultField Is Nothing Then
                defaultField = ParseDictinary()
            End If

            Return defaultField
        End Get
    End Property

    Private Shared Function ParseDictinary() As LipidClassDictionary
        Dim result = New LipidClassDictionary()
        Dim acyl As Integer = Nothing, alkyl As Integer = Nothing, sphingo As Integer = Nothing

        Using stream = My.Resources.ResourceManager.GetStream("LipidClassProperties")
            Using reader = New StreamReader(stream)
                Call reader.ReadLine() ' skip header

                While reader.Peek() >= 0
                    Dim cols = reader.ReadLine().Split(","c)
                    Dim item = New LipidClassProperty(
                        System.Enum.Parse(GetType(LbmClass), cols(0)),
                        cols(1),
                        If(Integer.TryParse(cols(2), acyl), acyl, 0),
                        If(Integer.TryParse(cols(3), alkyl), alkyl, 0),
                        If(Integer.TryParse(cols(4), sphingo), sphingo, 0)
                    )

                    result.m_lbmItems(item.Class) = item
                End While
            End Using
        End Using
        Return result
    End Function
End Class
