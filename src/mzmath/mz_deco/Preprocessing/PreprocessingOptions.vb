Namespace LCMS.Preprocessing

    ''' <summary>
    ''' LC-MS数据预处理流程的配置参数类
    ''' </summary>
    ''' <remarks>
    ''' LC-MS表达矩阵数据预处理模块 - 枚举定义与配置类
    ''' 本模块定义了缺失值处理、归一化和批次矫正的所有方法枚举，
    ''' 以及预处理流程的配置参数类。
    ''' </remarks>
    Public Class PreprocessingOptions

        ' ---- 缺失值处理配置 ----

        ''' <summary>缺失值处理方法，默认为HalfMin</summary>
        Public Property MissingValueMethod As MissingValueMethod = MissingValueMethod.HalfMin

        ''' <summary>KNN插补的近邻数K，默认5</summary>
        Public Property KNN_K As Integer = 5

        ''' <summary>PCA插补的主成分数，默认5</summary>
        Public Property PCA_Components As Integer = 5

        ''' <summary>PCA插补NIPALS算法的最大迭代次数</summary>
        Public Property PCA_MaxIterations As Integer = 100

        ''' <summary>PCA插补NIPALS算法的收敛容差</summary>
        Public Property PCA_Tolerance As Double = 0.000001

        ' ---- 归一化配置 ----

        ''' <summary>归一化方法，默认为TotalIonCount</summary>
        Public Property NormalizationMethod As NormalizationMethod = NormalizationMethod.TotalIonCount

        ''' <summary>对数变换的偏移量，避免对0取对数</summary>
        Public Property LogOffset As Double = 1.0

        ''' <summary>PQN归一化的参考样本选择方式：median或mean</summary>
        Public Property PQN_ReferenceType As String = "median"

        ' ---- 批次矫正配置 ----

        ''' <summary>批次矫正方法，默认为None</summary>
        Public Property BatchCorrectionMethod As BatchCorrectionMethod = BatchCorrectionMethod.None

        ''' <summary>LOESS回归的平滑参数span，默认0.75</summary>
        Public Property LOESS_Span As Double = 0.75

        ''' <summary>LOESS回归的多项式阶数，默认2</summary>
        Public Property LOESS_Degree As Integer = 2

        ''' <summary>ComBat是否使用参数化经验贝叶斯，默认True</summary>
        Public Property ComBat_Parametric As Boolean = True

        ''' <summary>SVR的正则化参数C，默认1.0</summary>
        Public Property SVR_C As Double = 1.0

        ''' <summary>SVR的不敏感损失函数参数epsilon</summary>
        Public Property SVR_Epsilon As Double = 0.1

        ''' <summary>SVR的RBF核参数gamma</summary>
        Public Property SVR_Gamma As Double = 0.1

        ''' <summary>SVR最大迭代次数</summary>
        Public Property SVR_MaxIterations As Integer = 1000

        ''' <summary>SVR学习率</summary>
        Public Property SVR_LearningRate As Double = 0.01

        ' ---- 通用配置 ----

        ''' <summary>缺失值的标记值，0表示丰度为0的值被视为缺失</summary>
        Public Property MissingValueMarker As Double = 0.0

        ''' <summary>是否将NaN视为缺失值</summary>
        Public Property TreatNaNAsMissing As Boolean = True

        ''' <summary>QC样本的分组标签</summary>
        Public Property QCLabel As String = "QC"

        ''' <summary>缺失值过滤阈值，缺失率超过此值则删除该特征</summary>
        Public Property MaxMissingRate As Double = 0.8

        ''' <summary>是否在预处理前对特征进行缺失值过滤</summary>
        Public Property EnableMissingValueFilter As Boolean = True

    End Class

End Namespace
