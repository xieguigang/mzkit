import r_lambda
import os

from r_lambda.docker import docker_image

# pip3 install r_lambda

def make_chromatogram_exports(rawdir, sampleinfo, group_name = "QC", 
                              export_dir = "./", 
                              overlaps_size = [2900,1600],
                              overlaps_layout = "padding:5% 5% 10% 12%;", 
                              file_color = "blue",
                              image_id = "metaboanalyst:20260501",
                              run_debug = False):
    
    rawdir = os.path.abspath(rawdir)
    sampleinfo = os.path.abspath(sampleinfo)
    export_dir = os.path.abspath(export_dir)

    args = {
        "rawdir": rawdir, 
        "sampleinfo": sampleinfo, 
        "group_name": group_name, 
        "export_dir": export_dir, 
        "overlaps_size": overlaps_size,
        "overlaps_layout": overlaps_layout, 
        "file_color": file_color
    }