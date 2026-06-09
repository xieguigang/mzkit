Namespace Chromatogram.PeakFinding

    ''' <summary>
    ''' 峰检测算法类型枚举
    ''' </summary>
    Public Enum PeakDetectionMethod
        ''' <summary>
        ''' 局部极大值检测法：基于滑动窗口寻找局部强度最大值，
        ''' 配合信噪比阈值过滤，是最基础且广泛使用的峰检测方法。
        ''' 适用于信噪比较高、峰形较为规则的色谱数据。
        ''' </summary>
        LocalMaximum

        ''' <summary>
        ''' CentWave算法（受XCMS启发）：基于连续小波变换(CWT)的峰检测方法，
        ''' 通过墨西哥帽小波在不同尺度上对信号进行卷积，识别具有高斯形状的峰。
        ''' 对重叠峰和宽峰有较好的检测能力，是代谢组学中最常用的算法之一。
        ''' </summary>
        CentWave

        ''' <summary>
        ''' MatchedFilter算法（受XCMS启发）：使用高斯核函数对色谱信号进行卷积滤波，
        ''' 然后在滤波后的信号上检测峰。该方法通过匹配滤波器最大化信噪比，
        ''' 对噪声有较强的抑制能力，适用于低信噪比的数据。
        ''' </summary>
        MatchedFilter

        ''' <summary>
        ''' 一阶导数法：通过计算色谱信号的一阶导数，在导数零交叉点处检测峰，
        ''' 同时利用导数符号变化确定峰的起止边界。该方法对峰边界的确定较为精确，
        ''' 适合于峰形不对称或需要精确边界定义的场景。
        ''' </summary>
        FirstDerivative
    End Enum

    ''' <summary>
    ''' 峰面积计算算法类型枚举
    ''' </summary>
    Public Enum PeakAreaMethod
        ''' <summary>
        ''' 梯形法则积分：将峰区域下的面积用一系列梯形近似，
        ''' 计算简单高效，是色谱峰面积计算的最基本方法。
        ''' 对于等间距采样数据，等价于矩形求和。
        ''' </summary>
        Trapezoidal

        ''' <summary>
        ''' 辛普森法则积分：使用二次多项式对相邻三个数据点进行插值，
        ''' 然后对插值多项式积分，精度高于梯形法则。
        ''' 要求峰区域内至少有3个数据点。
        ''' </summary>
        Simpson

        ''' <summary>
        ''' 高斯曲线拟合：使用高斯函数对峰形进行最小二乘拟合，
        ''' 然后对拟合的高斯曲线进行解析积分计算峰面积。
        ''' 适用于峰形近似高斯分布的理想色谱峰，
        ''' 拟合参数（中心、宽度、高度）可提供额外的峰特征信息。
        ''' </summary>
        GaussianFitting

        ''' <summary>
        ''' 基线校正积分法：先估计峰区域的基线水平，
        ''' 然后从原始信号中扣除基线后使用梯形法则计算峰面积。
        ''' 该方法能有效消除基线漂移对峰面积计算的影响，
        ''' 是实际代谢组学数据处理中最常用的峰面积计算策略。
        ''' </summary>
        BaselineCorrected
    End Enum

    ''' <summary>
    ''' 基线估计算法类型枚举
    ''' </summary>
    Public Enum BaselineMethod
        ''' <summary>
        ''' 线性基线：连接峰起止点的直线作为基线
        ''' </summary>
        Linear

        ''' <summary>
        ''' 最小值基线：使用峰区域内的最小强度值作为基线
        ''' </summary>
        Minimum

        ''' <summary>
        ''' 百分位基线：使用峰区域内指定百分位数的强度值作为基线
        ''' </summary>
        Percentile

        ''' <summary>
        ''' 局部最小值基线：使用峰起止点附近局部最小值的平均作为基线
        ''' </summary>
        LocalMinimum
    End Enum

End Namespace