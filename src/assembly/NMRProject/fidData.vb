Imports System.Numerics
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.nmrML
Imports FFT = Microsoft.VisualBasic.Math.SignalProcessing.FourierTransform

''' <summary>
''' NMR方法检测到的与时间相关的原子震荡数据
''' </summary>
Public Class fidData

    Public Property time As Double()
    Public Property amplitude As Double()

    ''' <summary>
    ''' 基于傅里叶变换将时域数据转换为频域数据
    ''' </summary>
    ''' <returns></returns>
    Public Function FourierTransform() As FrequencyData
        Dim signal As Complex() = time.Select(Function(t, i) New Complex(t, amplitude(i))).ToArray
        Dim freq As Double()
        Dim ampl As Double()

        Call FFT.DFT(signal, FFT.Direction.Forward)

        freq = signal.Select(Function(c) c.Real).ToArray
        ampl = signal.Select(Function(c) c.Imaginary).ToArray

        Return New FrequencyData With {
            .frequency = freq,
            .amplitude = ampl
        }
    End Function

    Public Shared Function Create(data As acquisitionMultiD) As fidData
        Dim complex As fidComplex() = data.ParseMatrix
        Dim real As Double() = complex.Select(Function(c) c.real).ToArray
        Dim image As Double() = complex.Select(Function(c) c.imaging).ToArray

        Return New fidData With {
            .time = real,
            .amplitude = image
        }
    End Function

End Class

Public Class FrequencyData

    Public Property frequency As Double()
    Public Property amplitude As Double()

End Class
