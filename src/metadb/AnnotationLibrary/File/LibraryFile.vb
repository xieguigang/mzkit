Imports System.IO.Compression
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public MustInherit Class LibraryFile

    Friend Const IndexPath As String = ".metadata/index"
    Friend Const annotationPath As String = ".metadata/annotations"

    Protected file As ZipArchive

    Private Class AnnotationSchema : Inherits SchemaProvider(Of ms2)

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Dim fields As New Dictionary(Of String, NilImplication) From {
                {NameOf(ms2.mz), NilImplication.MemberDefault},
                {NameOf(ms2.intensity), NilImplication.MemberDefault},
                {NameOf(ms2.Annotation), NilImplication.MemberDefault}
            }

            Yield (GetType(ms2), fields)
        End Function
    End Class

    Shared Sub New()
        Call MsgPackSerializer.DefaultContext.RegisterSerializer(New AnnotationSchema)
    End Sub

    Friend Shared Iterator Function LoadIndex(zip As ZipArchive) As IEnumerable(Of MassIndex)
        Dim list = From file As ZipArchiveEntry
                   In zip.Entries
                   Where file.FullName.StartsWith(IndexPath)
        Dim mass As MassIndex

        For Each i As ZipArchiveEntry In list
            mass = MsgPackSerializer.Deserialize(Of MassIndex)(i.Open)
            mass.referenceIds = mass.referenceIds.Distinct.ToArray

            Yield mass
        Next
    End Function

    ''' <summary>
    ''' create peak fragment annotation list
    ''' </summary>
    ''' <param name="spectrums"></param>
    ''' <returns></returns>
    Public Shared Function AnnotationSet(spectrums As IEnumerable(Of Spectrum)) As ms2()
        Dim Allfragments As New List(Of ms2)

        For Each spectrum As Spectrum In spectrums
            Call Allfragments.AddRange(spectrum.getMatrix)
        Next

        Dim annoGroups = Allfragments _
            .Where(Function(i) Not i.Annotation.StringEmpty) _
            .GroupBy(Function(i) i.Annotation) _
            .ToArray
        Dim nsize As Integer = Aggregate t In annoGroups Into Sum(t.Count)

        Return annoGroups _
            .Select(Function(i)
                        Return New ms2 With {
                            .mz = i.First.mz,
                            .Annotation = i.Key,
                            .intensity = i.Count / nsize
                        }
                    End Function) _
            .ToArray
    End Function

End Class
