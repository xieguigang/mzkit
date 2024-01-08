# tissue

tools for HE-stain image analysis
 
 Pathology is practiced by visual inspection of histochemically stained tissue slides. 
 While the hematoxylin and eosin (H&E) stain is most commonly used, special stains can
 provide additional contrast to different tissue components.
 
 Histological analysis of stained human tissue samples is the gold standard for evaluation 
 of many diseases, as the fundamental basis of any pathologic evaluation is the examination
 of histologically stained tissue affixed on a glass slide using either a microscope or 
 a digitized version of the histologic image following the image capture by a whole slide
 image (WSI) scanner. The histological staining step is a critical part of the pathology
 workflow and is required to provide contrast and color to tissue by facilitating a chromatic 
 distinction among different tissue constituents. The most common stain (otherwise referred 
 to as the routine stain) is the hematoxylin and eosin (H&E), which is applied to nearly 
 all clinical cases, covering ~80% of all the human tissue staining performed globally1. 
 The H&E stain is relatively easy to perform and is widely used across the industry. 
 In addition to H&E, there are a variety of other histological stains with different
 properties which are used by pathologists to better highlight different tissue 
 constituents.

+ [scan_tissue](tissue/scan_tissue.1) analysis the HE-stain image by blocks
+ [mark_nucleus](tissue/mark_nucleus.1) extract the possible nucleus pixel points from the image
+ [heatmap_layer](tissue/heatmap_layer.1) generates heatmap value
+ [RSD](tissue/RSD.1) evaluate the spatial RSD of a specific channel
