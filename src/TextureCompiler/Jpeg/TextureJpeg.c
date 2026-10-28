/* PC JPEG decoder around the pinned, unmodified IJG 6b source.
   This software is based in part on the work of the Independent JPEG Group. */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <setjmp.h>
#include "jpeglib.h"
#include "jerror.h"
struct probe_error { struct jpeg_error_mgr base; jmp_buf jump; };
static void fail(j_common_ptr info) { longjmp(((struct probe_error*)info->err)->jump,1); }
static void quiet(j_common_ptr info) { (void)info; }
static void start(j_decompress_ptr info) { (void)info; }
static boolean empty(j_decompress_ptr info) { ERREXIT(info,JERR_INPUT_EMPTY); return FALSE; }
static void skip(j_decompress_ptr info,long bytes) {
 if(bytes>0){if((size_t)bytes>info->src->bytes_in_buffer)ERREXIT(info,JERR_INPUT_EOF);info->src->next_input_byte+=bytes;info->src->bytes_in_buffer-=bytes;}
}
__declspec(dllexport) int __cdecl TextureDecodeJpeg(const unsigned char* input,int count,unsigned char* output,int capacity,int* width,int* height,int* grayscale) {
 struct jpeg_decompress_struct info; struct probe_error error; struct jpeg_source_mgr source;
 volatile int created=0; JSAMPARRAY row; size_t needed; unsigned int x,y;
 if(!input||count<2||!width||!height||!grayscale)return 0;
 memset(&info,0,sizeof(info)); info.err=jpeg_std_error(&error.base);error.base.error_exit=fail;error.base.output_message=quiet;
 if(setjmp(error.jump)){if(created)jpeg_destroy_decompress(&info);return 0;}
 jpeg_create_decompress(&info);created=1;
 source.init_source=start;source.fill_input_buffer=empty;source.skip_input_data=skip;source.resync_to_restart=jpeg_resync_to_restart;source.term_source=start;
 source.bytes_in_buffer=count;source.next_input_byte=input;info.src=&source;
 if(jpeg_read_header(&info,TRUE)!=JPEG_HEADER_OK){jpeg_destroy_decompress(&info);return 0;}
 if(info.image_width==0||info.image_height==0||info.image_width>8192||info.image_height>8192||(size_t)info.image_width*info.image_height>=16777216){jpeg_destroy_decompress(&info);return 0;}
 if(info.num_components!=1&&info.num_components!=3){jpeg_destroy_decompress(&info);return 0;}
 *grayscale=(info.jpeg_color_space==JCS_GRAYSCALE);
 *width=(int)info.image_width;*height=(int)info.image_height;needed=(size_t)*width * *height * 4;
 if(!output){jpeg_destroy_decompress(&info);return 1;}
 if(capacity<0||(size_t)capacity!=needed){jpeg_destroy_decompress(&info);return 0;}
 info.out_color_space=JCS_RGB;info.dct_method=JDCT_IFAST;info.do_fancy_upsampling=TRUE;
 jpeg_start_decompress(&info);if(info.output_components!=3){jpeg_destroy_decompress(&info);return 0;}
 row=(*info.mem->alloc_sarray)((j_common_ptr)&info,JPOOL_IMAGE,info.output_width*3,1);
 while(info.output_scanline<info.output_height){y=info.output_scanline;jpeg_read_scanlines(&info,row,1);for(x=0;x<info.output_width;x++){size_t at=((size_t)y*info.output_width+x)*4;output[at]=row[0][x*3];output[at+1]=row[0][x*3+1];output[at+2]=row[0][x*3+2];output[at+3]=255;}}
 jpeg_finish_decompress(&info);jpeg_destroy_decompress(&info);return 1;
}
