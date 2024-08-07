﻿# SingleCells

Single cells metabolomics data processor
 
 Single-cell analysis is a technique that measures only the target cell itself and can 
 extract information that would be buried in bulk-cell analysis with high-resolution.
> Single-cell metabolomics is a powerful tool that can reveal cellular heterogeneity and 
>  can elucidate the mechanisms of biological phenomena in detail. It is a promising 
>  approach in studying plants, especially when cellular heterogeneity has an impact on different 
>  biological processes. In addition, metabolomics, which can be regarded as a detailed 
>  phenotypic analysis, is expected to answer previously unrequited questions which will 
>  lead to expansion of crop production, increased understanding of resistance to diseases,
>  and in other applications as well.

+ [mz_matrix](SingleCells/mz_matrix.1) cast the matrix object as the dataframe
+ [as.expression](SingleCells/as.expression.1) Cast the ion feature matrix as the GCModeller expression matrix object
+ [apply.scale](SingleCells/apply.scale.1) scale matrix for each spot/cell sample
+ [cell_matrix](SingleCells/cell_matrix.1) export single cell expression matrix from the raw data scans
+ [SCM_ionStat](SingleCells/SCM_ionStat.1) do statistics of the single cell metabolomics ions features
+ [write.matrix](SingleCells/write.matrix.1) write the single cell ion feature data matrix
+ [open.matrix](SingleCells/open.matrix.1) open a single cell data matrix reader
+ [read.mz_matrix](SingleCells/read.mz_matrix.1) load the data matrix into memory at once
+ [df.mz_matrix](SingleCells/df.mz_matrix.1) cast matrix object to the R liked dataframe object
+ [cell_embedding](SingleCells/cell_embedding.1) create a session for create spot cell embedding
+ [cell_clusters](SingleCells/cell_clusters.1) export the cell clustering result
+ [embedding_sample](SingleCells/embedding_sample.1) push a sample data into the embedding session
+ [spot_vector](SingleCells/spot_vector.1) get the cell spot embedding result
+ [spatial_labels](SingleCells/spatial_labels.1) get the labels based on the spatial information of each spot
+ [singleCell_labels](SingleCells/singleCell_labels.1) 
+ [total_peaksum](SingleCells/total_peaksum.1) do matrix data normalization via total peak sum
+ [TrIQ_clip](SingleCells/TrIQ_clip.1) processing of the intensity value clip via TrIQ algorithm for the feature matrix
