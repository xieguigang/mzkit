Public Interface IReportRender

    ReadOnly Property annotation As AnnotationPack

    ''' <summary>
    ''' the color schema for the heatmap rendering
    ''' </summary>
    ''' <returns></returns>
    Property colorSet As String()
    ''' <summary>
    ''' ordinal of the sample files or the sample file display selection list
    ''' </summary>
    ''' <returns></returns>
    Property samplefiles As String()

    Function GetIon(xcms_id As String) As AlignmentHit

    ''' <summary>
    ''' implements of the html table report
    ''' </summary>
    ''' <param name="refSet"></param>
    ''' <param name="rt_cell"></param>
    ''' <returns>this function generates the html table code for view report</returns>
    Function Tabular(refSet As IEnumerable(Of String), Optional rt_cell As Boolean = False) As IEnumerable(Of String)

End Interface
