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
    
    os.makedirs(export_dir, exist_ok=True)

    rawdir = os.path.abspath(rawdir)
    sampleinfo = os.path.abspath(sampleinfo)
    export_dir = os.path.abspath(export_dir)
    args = {
        # directory contains mzXML/mzML files
        "rawdir": rawdir, 
        # ID, sample_name, sample_info
        "sampleinfo": sampleinfo, 
        "group_name": group_name, 
        # directory for export save chromatogram plot images
        "export_dir": export_dir, 
        "overlaps_size": overlaps_size,
        "overlaps_layout": overlaps_layout, 
        "file_color": file_color
    }

    return r_lambda.call_lambda(
        "mzkit::make_chromatogram_exports",
        argv=args,
        options={"cache.enable": True},
        workdir=export_dir,
        docker=docker_image(id=image_id, volumn= ["rawdir","sampleinfo","export_dir"], 
                            name=None, tty = False,
                            host_net = True),
        run_debug=run_debug
    )

if __name__ == "__main__":

    make_chromatogram_exports(rawdir = "./pos", sampleinfo = "./sampleinfo.csv", group_name = "QC", 
                              export_dir = "./chromatogram_exports", 
                              overlaps_size = [2900,1600],
                              overlaps_layout = "padding:5% 5% 10% 12%;", 
                              file_color = "blue",
                              image_id = "metaboanalyst:20260501",
                              run_debug = False)