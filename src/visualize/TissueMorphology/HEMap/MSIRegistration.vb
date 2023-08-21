Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Namespace HEMap

    Public Module MSIRegistration

        ''' <summary>
        ''' Apply the spatial mapping between the MSI/HEstain
        ''' </summary>
        ''' <param name="register"></param>
        ''' <param name="MSI"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SpatialTranslation(register As SpatialRegister, MSI As PointF()) As PointF()
            Dim newPolygon As New Polygon2D(MSI.Rotate(register.rotation))
            Dim y As Double() = newPolygon.ypoints
            Dim newDims As Size = newPolygon.GetSize
            Dim dx As New DoubleRange(newPolygon.xpoints)
            Dim dy As New DoubleRange(newPolygon.ypoints)
            Dim scale1 As New DoubleRange(0, register.MSIscale.Width)
            Dim scale2 As New DoubleRange(0, register.MSIscale.Height)

            ' translate the MSI spot to view spot
            MSI = newPolygon.xpoints _
                .Select(Function(xi, i)
                            Return New PointF With {
                                .X = dx.ScaleMapping(xi, scale1),
                                .Y = dy.ScaleMapping(y(i), scale2)
                            }
                        End Function) _
                .ToArray

            ' do offset
            newPolygon = New Polygon2D(MSI) + register.offset

            Return newPolygon.AsEnumerable.ToArray
        End Function
    End Module
End Namespace