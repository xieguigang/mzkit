#Region "Microsoft.VisualBasic::cd496da35bd405db447c909a3adb4e93, src\assembly\assembly\ASCII\MGF\MetaData.vb"

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

    '     Class MetaData
    ' 
    '         Properties: activation, collisionEnergy, compound_class, formula, kegg
    '                     mass, MetaTable, mzcloud, name, polarity
    '                     precursor_type, scan
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ASCII.MGF

    Public Class MetaData

        Dim meta As Dictionary(Of String, String)

#Region "MetaData getter/setter"

        Public Property collisionEnergy As String
            Get
                Return meta.TryGetValue("collisionEnergy")
            End Get
            Set(value As String)
                meta("collisionEnergy") = value
            End Set
        End Property

        Public Property activation As String
            Get
                Return meta.TryGetValue("activation")
            End Get
            Set(value As String)
                meta("activation") = value
            End Set
        End Property

        Public Property precursor_type As String
            Get
                Return meta.TryGetValue("precursor_type")
            End Get
            Set(value As String)
                meta("precursor_type") = value
            End Set
        End Property

        Public Property scan As String
            Get
                Return meta.TryGetValue("scan")
            End Get
            Set(value As String)
                meta("scan") = value
            End Set
        End Property

        Public Property name As String
            Get
                Return meta.TryGetValue("name")
            End Get
            Set(value As String)
                meta("name") = value
            End Set
        End Property

        Public Property compound_class As String
            Get
                Return meta.TryGetValue("compound_class")
            End Get
            Set(value As String)
                meta("compound_class") = value
            End Set
        End Property

        Public Property formula As String
            Get
                Return meta.TryGetValue("formula")
            End Get
            Set(value As String)
                meta("formula") = value
            End Set
        End Property

        Public Property mass As Double
            Get
                Return meta.TryGetValue("mass")
            End Get
            Set(value As Double)
                meta("mass") = value
            End Set
        End Property

        Public Property kegg As String
            Get
                Return meta.TryGetValue("kegg")
            End Get
            Set(value As String)
                meta("kegg") = value
            End Set
        End Property

        Public Property mzcloud As String
            Get
                Return meta.TryGetValue("mzcloud")
            End Get
            Set(value As String)
                meta("mzcloud") = value
            End Set
        End Property

        Public Property polarity As String
            Get
                Return meta.TryGetValue("polarity")
            End Get
            Set(value As String)
                meta("polarity") = value
            End Set
        End Property

#End Region

        Public ReadOnly Property MetaTable As Dictionary(Of String, String)
            Get
                Return meta
            End Get
        End Property

        Default Public Property Item(key As String) As String
            Get
                Return meta.TryGetValue(key)
            End Get
            Set
                meta(key) = Value
            End Set
        End Property

        Sub New()
            Call Me.New(New Dictionary(Of String, String))
        End Sub

        Sub New(meta As Dictionary(Of String, String))
            Me.meta = meta
        End Sub

        Public Overrides Function ToString() As String
            Return meta.GetJson
        End Function

        Public Shared Narrowing Operator CType(meta As MetaData) As Dictionary(Of String, String)
            Return meta.meta
        End Operator

    End Class
End Namespace
