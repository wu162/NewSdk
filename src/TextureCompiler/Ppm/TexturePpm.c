/* This software is based in part on the work of the Independent JPEG Group.
 * Bounded wrapper for the adapted, pinned IJG6b row reader; see PROVENANCE.json.
 * Independent candidate only. No original compiler imports or decoder calls. */
#include <setjmp.h>
#include "rdppm.c"

struct ppm_error { struct jpeg_error_mgr base; jmp_buf jump; };
static void fail(j_common_ptr info) { longjmp(((struct ppm_error *)info->err)->jump, 1); }
static void quiet(j_common_ptr info) { (void)info; }

__declspec(dllexport) int __cdecl TextureDecodePpm(const unsigned char *input, int count,
    unsigned char *output, int capacity, int *width, int *height)
{
  struct jpeg_compress_struct info;
  struct ppm_error error;
  struct ppm_memory memory;
  cjpeg_source_ptr source;
  volatile int created = 0;
  unsigned int x, y;
  size_t needed;
  if (!input || count < 2 || !width || !height) return 0;
  memset(&info, 0, sizeof(info));
  info.err = jpeg_std_error(&error.base);
  error.base.error_exit = fail; error.base.output_message = quiet;
  if (setjmp(error.jump)) { if (created) jpeg_destroy_compress(&info); return 0; }
  jpeg_create_compress(&info); created = 1;
  memory.data = input; memory.size = (size_t)count; memory.position = 0;
  source = jinit_read_ppm(&info); source->input_file = (FILE *)&memory;
  (*source->start_input)(&info, source);
  *width = (int)info.image_width; *height = (int)info.image_height;
  needed = (size_t)*width * *height * 4;
  if (!output) { jpeg_destroy_compress(&info); return 1; }
  if (capacity < 0 || (size_t)capacity != needed) { jpeg_destroy_compress(&info); return 0; }
  for (y = 0; y < info.image_height; ++y) {
    if ((*source->get_pixel_rows)(&info, source) != 1) ERREXIT(&info, JERR_PPM_NOT);
    for (x = 0; x < info.image_width; ++x) {
      size_t at = ((size_t)y * info.image_width + x) * 4;
      ppm_source_ptr ppm = (ppm_source_ptr)source;
      unsigned int red, green, blue, packed;
      if (source->get_pixel_rows == get_raw_row) {
        red = source->buffer[0][x * 3];
        green = source->buffer[0][x * 3 + 1];
        blue = source->buffer[0][x * 3 + 2];
      } else {
        red = ppm->wide_rgb[x * 3];
        green = ppm->wide_rgb[x * 3 + 1];
        blue = ppm->wide_rgb[x * 3 + 2];
      }
      /* Native PC parser combines unmasked scaled integers before conversion
       * to RGBA. Invalid but bounded samples above maxval can affect adjacent
       * channels; a byte rescale table loses that information. */
      packed = 0xff000000U | (red << 16) | (green << 8) | blue;
      output[at] = (unsigned char)(packed >> 16);
      output[at + 1] = (unsigned char)(packed >> 8);
      output[at + 2] = (unsigned char)packed;
      output[at + 3] = (unsigned char)(packed >> 24);
    }
  }
  jpeg_destroy_compress(&info); return 1;
}
