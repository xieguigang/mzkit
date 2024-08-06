require(mzkit);

imports "annotation" from "mzkit";

let pack = "Z:\项目以外内容\2023\血清血浆物质列表\lipidomics_test\blood\HCS_new_20240807-2\MZKit.hdms";

    pack <- open.annotation_workspace(pack, io = "Read");
    pack <- annotation::get_annotations(pack);
    pack <- annotationKit::to_report(pack);

    print("files for save into mzvault database:");
    print(names(pack));