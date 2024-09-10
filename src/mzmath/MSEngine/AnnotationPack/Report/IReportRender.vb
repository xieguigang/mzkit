Public Interface IReportRender

    ReadOnly Property annotation As AnnotationPack

    ''' <summary>
    ''' the color schema for the heatmap rendering
    ''' </summary>
    ''' <returns></returns>
    Property colorSet As String()

    Function GetIon(xcms_id As String) As AlignmentHit
    Function Tabular(refSet As IEnumerable(Of String), Optional rt_cell As Boolean = False) As IEnumerable(Of String)

End Interface
