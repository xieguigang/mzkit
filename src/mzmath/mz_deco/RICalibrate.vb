Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports deco_math = BioNovoGene.Analytical.MassSpectrometry.Math.Extensions

Public Module RICalibrate

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="peakdata">should be a collection of the peak data from a single sample file.</param>
    ''' <param name="refer_points"></param>
    ''' <param name="C_atoms">
    ''' the number of carbon atoms for kovats retention index
    ''' </param>
    ''' <param name="map_RI_id">
    ''' metabolite name map to xcms_id
    ''' </param>
    ''' <param name="rawfile"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ConvertRI(peakdata As PeakFeature(),
                              refer_points As List(Of PeakFeature),
                              C_atoms As Dictionary(Of String, Integer),
                              map_RI_id As Dictionary(Of String, String),
                              Optional rawfile As String = Nothing) As PeakFeature()

        ' order raw data by rt
        peakdata = peakdata.OrderBy(Function(i) i.rt).ToArray
        refer_points = refer_points.OrderBy(Function(i) i.rt).AsList
        ' add a fake point
        refer_points.Add(New PeakFeature With {
            .RI = refer_points.Last.RI + 100,
            .rt = peakdata.Last.rt,
            .xcms_id = peakdata.Last.xcms_id
        })

        Dim a As (rt As Double, ri As Double)
        Dim b As (rt As Double, ri As Double)
        Dim offset As Integer = 0
        Dim id_a, id_b As String

        If Not C_atoms Is Nothing Then
            If Not map_RI_id.IsNullOrEmpty Then
                ' some reference data peak maybe missing from the lcms experiment data.
                C_atoms = C_atoms _
                    .Where(Function(t) map_RI_id.ContainsKey(t.Key)) _
                    .ToDictionary(Function(t) map_RI_id(t.Key),
                                  Function(t)
                                      Return t.Value
                                  End Function)
            End If

            If Not C_atoms.ContainsKey(peakdata(0).xcms_id) Then
                Call C_atoms.Add(peakdata(0).xcms_id, 0)
            End If
            If Not C_atoms.ContainsKey(peakdata.Last.xcms_id) Then
                Call C_atoms.Add(peakdata.Last.xcms_id, C_atoms.Values.Max + 1)
            End If
        End If

        If peakdata(0).RI > 0 Then
            a = (peakdata(0).rt, peakdata(0).RI)
            id_a = peakdata(0).xcms_id
            offset = 1
            b = (refer_points(1).rt, refer_points(1).RI)
            id_b = refer_points(1).xcms_id
        Else
            a = (peakdata(0).rt, 0)
            b = (refer_points(0).rt, refer_points(0).RI)
            id_a = peakdata(0).xcms_id
            id_b = refer_points(0).xcms_id
        End If

        For i As Integer = offset To peakdata.Length - 1
            peakdata(i).rawfile = If(rawfile, peakdata(i).rawfile)

            If peakdata(i).RI = 0 Then
                If C_atoms Is Nothing Then
                    peakdata(i).RI = deco_math.RetentionIndex(peakdata(i), a, b)
                Else
                    peakdata(i).RI = deco_math.KovatsRI(C_atoms(id_a), C_atoms(id_b), peakdata(i).rt, a.rt, b.rt)
                End If
            Else
                a = b
                id_a = id_b
                offset += 1
                b = (refer_points(offset).rt, refer_points(offset).RI)
                id_b = refer_points(offset).xcms_id
            End If
        Next

        ' and then evaluate the ri for each peak points
        Return peakdata
    End Function
End Module
