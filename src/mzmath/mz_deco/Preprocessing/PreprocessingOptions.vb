Namespace LCMS.Preprocessing

    ''' <summary>
    ''' LC-MS数据预处理流程的配置参数类
    ''' </summary>
    ''' <remarks>
    ''' LC-MS expression matrix data preprocessing module - Enumerations and configuration classes.
    ''' This module defines method enumerations for missing value imputation, normalization, and batch correction,
    ''' as well as the configuration parameter class for the preprocessing workflow.
    ''' 
    ''' LC-MS表达矩阵数据预处理模块 - 枚举定义与配置类
    ''' 本模块定义了缺失值处理、归一化和批次矫正的所有方法枚举，
    ''' 以及预处理流程的配置参数类。
    ''' </remarks>
    Public Class PreprocessingOptions

        ' ---- Missing Value Imputation Configuration ----

        ''' <summary>The missing value imputation method. Defaults to HalfMin.</summary>
        ''' <remarks>Determines the algorithm used to fill in missing values in the expression matrix. 
        ''' Common methods include HalfMin (replacing with half of the minimum positive value of the feature), 
        ''' KNN, PCA, etc. The choice of method impacts downstream statistical sensitivity and variance.</remarks>
        Public Property MissingValueMethod As MissingValueMethod = MissingValueMethod.HalfMin

        ''' <summary>The number of nearest neighbors (K) for KNN imputation. Defaults to 5.</summary>
        ''' <remarks>In K-Nearest Neighbors (KNN) imputation, missing values are estimated by averaging the 
        ''' values of the K most similar samples (based on Euclidean distance or other metrics). A smaller K 
        ''' may introduce noise, while a larger K may oversmooth local variations.</remarks>
        Public Property KNN_K As Integer = 5

        ''' <summary>The number of principal components for PCA imputation. Defaults to 5.</summary>
        ''' <remarks>Probabilistic PCA or NIPALS-based PCA imputation reconstructs missing values using the 
        ''' specified number of principal components. This determines the dimensionality of the subspace 
        ''' used to capture the variance in the data for accurate estimation.</remarks>
        Public Property PCA_Components As Integer = 5

        ''' <summary>The maximum number of iterations for the NIPALS algorithm in PCA imputation.</summary>
        ''' <remarks>The Nonlinear Iterative Partial Least Squares (NIPALS) algorithm iteratively computes 
        ''' principal components. This parameter sets an upper bound on iterations to prevent infinite loops 
        ''' if the algorithm fails to converge.</remarks>
        Public Property PCA_MaxIterations As Integer = 100

        ''' <summary>The convergence tolerance for the NIPALS algorithm in PCA imputation.</summary>
        ''' <remarks>Defines the threshold for the change in estimated values between iterations. The algorithm 
        ''' stops when the change falls below this tolerance, indicating that the principal components have 
        ''' successfully converged.</remarks>
        Public Property PCA_Tolerance As Double = 0.000001

        ' ---- Normalization Configuration ----

        ''' <summary>The normalization method. Defaults to TotalIonCount.</summary>
        ''' <remarks>Specifies the algorithm used to adjust for systematic variations in total signal intensity 
        ''' across samples, making them comparable. Options typically include TotalIonCount (TIC), PQN, 
        ''' or internal standard normalization.</remarks>
        Public Property NormalizationMethod As NormalizationMethod = NormalizationMethod.TotalIonCount

        ''' <summary>The offset for log transformation to avoid taking the logarithm of zero.</summary>
        ''' <remarks>Applied as log(x + offset). Since metabolomics data often contains zero or near-zero 
        ''' values, adding an offset prevents undefined mathematical results and stabilizes the variance 
        ''' before downstream statistical analysis.</remarks>
        Public Property LogOffset As Double = 1.0

        ''' <summary>The reference sample selection method for PQN normalization: "median" or "mean".</summary>
        ''' <remarks>Probabilistic Quotient Normalization (PQN) requires a reference spectrum to calculate 
        ''' normalization quotients. This parameter determines whether the median or mean spectrum across 
        ''' all samples (or QC samples) is used as that reference.</remarks>
        Public Property PQN_ReferenceType As String = "median"

        ' ---- Batch Correction Configuration ----

        ''' <summary>The batch correction method. Defaults to None.</summary>
        ''' <remarks>Specifies the algorithm used to remove non-biological systematic variation (batch effects) 
        ''' between different analytical runs, ensuring that observed differences are biological rather than technical.</remarks>
        Public Property BatchCorrectionMethod As BatchCorrectionMethod = BatchCorrectionMethod.None

        ''' <summary>The smoothing parameter (span) for LOESS regression. Defaults to 0.75.</summary>
        ''' <remarks>Local Polynomial Regression (LOESS/LOWESS) span controls the proportion of data points 
        ''' used for each local fit. A larger span produces smoother curves (higher bias, lower variance), 
        ''' while a smaller span captures more local signal drift (lower bias, higher variance).</remarks>
        Public Property LOESS_Span As Double = 0.75

        ''' <summary>The polynomial degree for LOESS regression. Defaults to 2.</summary>
        ''' <remarks>Defines the degree of the polynomial fitted locally by LOESS. Typically set to 1 (linear) 
        ''' or 2 (quadratic). A degree of 2 is common as it captures curvilinear trends in instrumental 
        ''' signal drift across injection order.</remarks>
        Public Property LOESS_Degree As Integer = 2

        ''' <summary>Whether ComBat uses parametric empirical Bayes. Defaults to True.</summary>
        ''' <remarks>In the ComBat algorithm, parametric empirical Bayes assumes batch effect parameters follow 
        ''' a normal distribution, providing more robust shrinkage estimates, especially for small sample sizes. 
        ''' Non-parametric (False) uses a non-parametric prior, which can be better if the normality assumption 
        ''' is severely violated.</remarks>
        Public Property ComBat_Parametric As Boolean = True

        ''' <summary>The regularization parameter C for SVR. Defaults to 1.0.</summary>
        ''' <remarks>In Support Vector Regression (SVR), C determines the trade-off between model flatness and 
        ''' tolerance for deviations larger than epsilon. A larger C penalizes errors more heavily, leading to 
        ''' a more complex model that fits the training data closely.</remarks>
        Public Property SVR_C As Double = 1.0

        ''' <summary>The epsilon parameter for the SVR insensitive loss function.</summary>
        ''' <remarks>Defines the epsilon-tube within which no penalty is associated with predicted errors. 
        ''' Errors within this margin are ignored, which helps in creating a sparse model and smoothing 
        ''' out minor noise when modeling signal drift.</remarks>
        Public Property SVR_Epsilon As Double = 0.1

        ''' <summary>The RBF kernel parameter gamma for SVR.</summary>
        ''' <remarks>Controls the influence of a single training example in the Radial Basis Function (RBF) kernel. 
        ''' A larger gamma means a narrower Gaussian function, leading to a more complex, tighter fit to the 
        ''' training data (potential overfitting), while a smaller gamma yields a broader, smoother fit.</remarks>
        Public Property SVR_Gamma As Double = 0.1

        ''' <summary>The maximum number of iterations for SVR.</summary>
        ''' <remarks>Caps the number of iterations for the SVR optimization solver (e.g., Sequential Minimal Optimization). 
        ''' Prevents the algorithm from running indefinitely on non-converging datasets.</remarks>
        Public Property SVR_MaxIterations As Integer = 1000

        ''' <summary>The learning rate for SVR.</summary>
        ''' <remarks>Determines the step size at each iteration while moving toward a minimum of the loss function 
        ''' in gradient-based SVR optimization. A learning rate that is too large may overshoot the minimum, 
        ''' while one that is too small may lead to extremely slow convergence.</remarks>
        Public Property SVR_LearningRate As Double = 0.01

        ' ---- General Configuration ----

        ''' <summary>The marker value for missing values; 0 means values with an abundance of 0 are treated as missing.</summary>
        ''' <remarks>In LC-MS data, zero values often represent measurements below the limit of detection rather 
        ''' than a true biological absence. This parameter defines which numerical value in the raw matrix should 
        ''' be interpreted as a missing value for imputation algorithms.</remarks>
        Public Property MissingValueMarker As Double = 0.0

        ''' <summary>Whether to treat NaN as a missing value.</summary>
        ''' <remarks>If True, Double.NaN values in the expression matrix will be identified as missing data 
        ''' and subjected to the chosen missing value imputation method, alongside the MissingValueMarker.</remarks>
        Public Property TreatNaNAsMissing As Boolean = True

        ''' <summary>The group label for QC (Quality Control) samples.</summary>
        ''' <remarks>Identifies which samples in the metadata are QC samples (e.g., pooled samples). QC samples 
        ''' are crucial for normalization and batch correction algorithms (like LOESS or SVR) as they represent 
        ''' purely technical variation rather than biological variation.</remarks>
        Public Property QCLabel As String = "QC"

        ''' <summary>The missing value filter threshold; features with a missing rate exceeding this value will be removed.</summary>
        ''' <remarks>Features (metabolites) with extremely high missing rates provide little reliable information 
        ''' and can severely distort imputation. This sets the threshold (0.0 to 1.0) for removing such features 
        ''' before downstream analysis to ensure statistical robustness.</remarks>
        Public Property MaxMissingRate As Double = 0.8

        ''' <summary>Whether to filter features based on missing values before preprocessing.</summary>
        ''' <remarks>Determines if the MaxMissingRate filter is applied prior to the missing value imputation step. 
        ''' Filtering first is generally recommended to avoid imputing largely empty features based on sparse data, 
        ''' which can introduce artificial noise into the dataset.</remarks>
        Public Property EnableMissingValueFilter As Boolean = True

    End Class

End Namespace
