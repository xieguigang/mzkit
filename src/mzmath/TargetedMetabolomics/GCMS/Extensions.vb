Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace GCMS

    Public Module Extensions

        Public Function ContentTable(msl As IEnumerable(Of MSLIon),
                                     contents As Dictionary(Of String, Double),
                                     [IS] As Dictionary(Of String, String)) As ContentTable

            Dim ionLevels As New Dictionary(Of String, SampleContentLevels)
            Dim ref As New Dictionary(Of String, Standards)
            Dim ISnames As Index(Of String) = [IS].Values.Distinct.Indexing

            For Each ion As MSLIon In msl.Where(Function(i) Not i.Name Like ISnames)
                ref(ion.Name) = New Standards With {
                    .Name = ion.Name,
                    .ID = ion.Name,
                    .Factor = 1,
                    .C = contents,
                    .[IS] = [IS].TryGetValue(ion.Name),
                    .ISTD = .IS
                }
                ionLevels(ion.Name) = New SampleContentLevels(contents, directMap:=True)
            Next

            Return New ContentTable(ionLevels, ref, Nothing)
        End Function
    End Module
End Namespace