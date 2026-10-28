/* Test-only encoder using the pinned public IJG6b API.
   This software is based in part on the work of the Independent JPEG Group.
   No original TextureCompiler code is used to construct these source files. */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "jpeglib.h"

int main(int argc, char** argv) {
    struct jpeg_compress_struct info;
    struct jpeg_error_mgr error;
    unsigned char row[64 * 4];
    FILE* file;
    int width, height, space, quality, progressive, pattern, components, x, c;
    if(argc != 8) return 2;
    width=atoi(argv[2]);height=atoi(argv[3]);space=atoi(argv[4]);quality=atoi(argv[5]);progressive=atoi(argv[6]);pattern=atoi(argv[7]);
    if(width<1||width>64||height<1||height>64||space<0||space>2||quality<1||quality>100||progressive<0||progressive>1||pattern<0||pattern>3)return 3;
    file=fopen(argv[1],"wb");if(!file)return 4;
    memset(&info,0,sizeof(info));info.err=jpeg_std_error(&error);
    jpeg_create_compress(&info);jpeg_stdio_dest(&info,file);
    info.image_width=width;info.image_height=height;
    components=space==0?3:4;info.input_components=components;info.in_color_space=space==0?JCS_RGB:JCS_CMYK;
    jpeg_set_defaults(&info);jpeg_set_colorspace(&info,space==0?JCS_RGB:space==1?JCS_CMYK:JCS_YCCK);
    jpeg_set_quality(&info,quality,TRUE);if(progressive)jpeg_simple_progression(&info);
    jpeg_start_compress(&info,TRUE);
    while(info.next_scanline<info.image_height){
        int y=info.next_scanline;
        for(x=0;x<width;x++)for(c=0;c<components;c++)
            row[x*components+c]=(unsigned char)(pattern==0?(x*1271+y*395+c*739+17)&255:pattern==1?0:pattern==2?255:((x+y+c)&1)*255);
        {JSAMPROW rows[1];rows[0]=row;jpeg_write_scanlines(&info,rows,1);}
    }
    jpeg_finish_compress(&info);jpeg_destroy_compress(&info);
    if(fclose(file))return 5;return 0;
}
