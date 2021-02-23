Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.mzML

    Public Class binaryDataArrayList : Inherits List

        <XmlElement(NameOf(binaryDataArray))>
        Public Property list As binaryDataArray()

        Default Public ReadOnly Property Item(i As Integer) As binaryDataArray
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return _list(i)
            End Get
        End Property

    End Class
End Namespace