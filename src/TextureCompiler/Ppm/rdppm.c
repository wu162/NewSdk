/*
 * rdppm.c
 *
 * Copyright (C) 1991-1997, Thomas G. Lane.
 * This file is part of the Independent JPEG Group's software.
 * For conditions of distribution and use, see the accompanying README file.
 *
 * Modified for the PC TextureCompiler compatibility candidate, 2026-10-29.
 * Bounded memory input, P3/P6 only, original token/comment/separator rules,
 * truncating sample rescale, dimensions and sample bounds. The upstream file
 * remains unmodified in recovery/reference/ijg6b/rdppm.c.
 *
 * This file contains routines to read input images in PPM/PGM format.
 * The extended 2-byte-per-sample raw PPM/PGM formats are supported.
 * The PBMPLUS library is NOT required to compile this software
 * (but it is highly useful as a set of PPM image manipulation programs).
 *
 * These routines may need modification for non-Unix environments or
 * specialized applications.  As they stand, they assume input from
 * an ordinary stdio stream.  They further assume that reading begins
 * at the start of the file; start_input may need work if the
 * user interface has already read some data (e.g., to determine that
 * the file is indeed PPM format).
 */

#include "cdjpeg.h"		/* Common decls for cjpeg/djpeg applications */

#ifdef PPM_SUPPORTED

/* FILE* in the application interface is an opaque bounded memory cursor here.
 * No stdio, original DLL, recovered decoder or gold is used by this reader. */
struct ppm_memory { const unsigned char *data; size_t size, position; };
static int ppm_getc(FILE *file)
{
  struct ppm_memory *m = (struct ppm_memory *) file;
  return m->position < m->size ? m->data[m->position++] : EOF;
}
static void ppm_ungetc(int ch, FILE *file)
{
  struct ppm_memory *m = (struct ppm_memory *) file;
  if (ch != EOF && m->position) --m->position;
}
static size_t ppm_read(FILE *file, void *buffer, size_t count)
{
  struct ppm_memory *m = (struct ppm_memory *) file;
  if (count > m->size - m->position) return 0;
  memcpy(buffer, m->data + m->position, count);
  m->position += count;
  return count;
}
static int ppm_space(int ch)
{
  return ch == ' ' || (ch >= '\t' && ch <= '\r');
}


/* Portions of this code are based on the PBMPLUS library, which is:
**
** Copyright (C) 1988 by Jef Poskanzer.
**
** Permission to use, copy, modify, and distribute this software and its
** documentation for any purpose and without fee is hereby granted, provided
** that the above copyright notice appear in all copies and that both that
** copyright notice and this permission notice appear in supporting
** documentation.  This software is provided "as is" without express or
** implied warranty.
*/


/* Macros to deal with unsigned chars as efficiently as compiler allows */

#ifdef HAVE_UNSIGNED_CHAR
typedef unsigned char U_CHAR;
#define UCH(x)	((int) (x))
#else /* !HAVE_UNSIGNED_CHAR */
#ifdef CHAR_IS_UNSIGNED
typedef char U_CHAR;
#define UCH(x)	((int) (x))
#else
typedef char U_CHAR;
#define UCH(x)	((int) (x) & 0xFF)
#endif
#endif /* HAVE_UNSIGNED_CHAR */


#define	ReadOK(file,buffer,len)	(ppm_read(file,buffer,len) == ((size_t) (len)))


/*
 * On most systems, reading individual bytes with getc() is drastically less
 * efficient than buffering a row at a time with fread().  On PCs, we must
 * allocate the buffer in near data space, because we are assuming small-data
 * memory model, wherein fread() can't reach far memory.  If you need to
 * process very wide images on a PC, you might have to compile in large-memory
 * model, or else replace fread() with a getc() loop --- which will be much
 * slower.
 */


/* Private version of data source object */

typedef struct {
  struct cjpeg_source_struct pub; /* public fields */

  U_CHAR *iobuffer;		/* non-FAR pointer to I/O buffer */
  JSAMPROW pixrow;		/* FAR pointer to same */
  size_t buffer_width;		/* width of I/O buffer */
  JSAMPLE *rescale;		/* => maxval-remapping array, or NULL */
  unsigned int maxval;
  unsigned int *wide_rgb; /* retain native PC overflow before packed-channel OR */
} ppm_source_struct;

typedef ppm_source_struct * ppm_source_ptr;


LOCAL(int)
pbm_getc (FILE * infile)
/* Read next char, skipping over any comments */
/* A comment/newline sequence is returned as a newline */
{
  register int ch;

  ch = ppm_getc(infile);
  if (ch == '#') {
    do {
      ch = ppm_getc(infile);
    } while (ch != '\n' && ch != EOF);
  }
  return ch;
}


LOCAL(unsigned int)
read_pbm_integer (j_compress_ptr cinfo, FILE * infile)
/* Read an unsigned decimal integer from the PPM file */
/* Swallows one trailing character after the integer */
/* Note that on a 16-bit-int machine, only values up to 64k can be read. */
/* This should not be a problem in practice. */
{
  register int ch;
  register unsigned int val;

  /* Skip any leading whitespace */
  do {
    ch = pbm_getc(infile);
    if (ch == EOF)
      ERREXIT(cinfo, JERR_INPUT_EOF);
  } while (ppm_space(ch));

  if (ch < '0' || ch > '9')
    ERREXIT(cinfo, JERR_PPM_NONNUMERIC);

  val = ch - '0';
  while ((ch = ppm_getc(infile)) >= '0' && ch <= '9') {
    if (val > (65535U - (unsigned int)(ch - '0')) / 10U)
      ERREXIT(cinfo, JERR_PPM_NONNUMERIC);
    val *= 10;
    val += ch - '0';
  }
  if (ch != EOF && !ppm_space(ch)) ERREXIT(cinfo, JERR_PPM_NONNUMERIC);
  ppm_ungetc(ch, infile); /* binary start_input consumes the exact separator */
  return val;
}


static unsigned int read_sample(j_compress_ptr cinfo, ppm_source_ptr source, FILE *infile)
{
  unsigned int value = read_pbm_integer(cinfo, infile);
  if (value > source->maxval) ERREXIT(cinfo, JERR_PPM_NOT);
  return value;
}
static unsigned int byte_sample(j_compress_ptr cinfo, ppm_source_ptr source, unsigned int value)
{
  if (value > source->maxval) ERREXIT(cinfo, JERR_PPM_NOT);
  return value;
}

/*
 * Read one row of pixels.
 *
 * We provide several different versions depending on input file format.
 * In all cases, input is scaled to the size of JSAMPLE.
 *
 * A really fast path is provided for reading byte/sample raw files with
 * maxval = MAXJSAMPLE, which is the normal case for 8-bit data.
 */


METHODDEF(JDIMENSION)
get_text_gray_row (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
/* This version is for reading text-format PGM files with any maxval */
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;
  FILE * infile = source->pub.input_file;
  register JSAMPROW ptr;
  register JSAMPLE *rescale = source->rescale;
  JDIMENSION col;

  ptr = source->pub.buffer[0];
  for (col = cinfo->image_width; col > 0; col--) {
    *ptr++ = rescale[read_sample(cinfo, source, infile)];
  }
  return 1;
}


METHODDEF(JDIMENSION)
get_text_rgb_row (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
/* IJG row traversal with PC truncation and wide intermediate components. */
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;
  FILE * infile = source->pub.input_file;
  JSAMPROW ptr = source->pub.buffer[0];
  unsigned int *wide = source->wide_rgb;
  JDIMENSION col;
  int channel;
  for (col = cinfo->image_width; col > 0; col--) {
    for (channel = 0; channel < 3; channel++) {
      unsigned int value = read_pbm_integer(cinfo, infile) * 255U / source->maxval;
      *wide++ = value;
      *ptr++ = (JSAMPLE)value;
    }
  }
  return 1;
}

METHODDEF(JDIMENSION)
get_scaled_gray_row (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
/* This version is for reading raw-byte-format PGM files with any maxval */
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;
  register JSAMPROW ptr;
  register U_CHAR * bufferptr;
  register JSAMPLE *rescale = source->rescale;
  JDIMENSION col;

  if (! ReadOK(source->pub.input_file, source->iobuffer, source->buffer_width))
    ERREXIT(cinfo, JERR_INPUT_EOF);
  ptr = source->pub.buffer[0];
  bufferptr = source->iobuffer;
  for (col = cinfo->image_width; col > 0; col--) {
    *ptr++ = rescale[byte_sample(cinfo, source, UCH(*bufferptr++))];
  }
  return 1;
}


METHODDEF(JDIMENSION)
get_scaled_rgb_row (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
/* Keep the upstream bounded row input; don't narrow before PC channel packing. */
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;
  JSAMPROW ptr = source->pub.buffer[0];
  U_CHAR *bufferptr = source->iobuffer;
  unsigned int *wide = source->wide_rgb;
  JDIMENSION col;
  int channel;
  if (!ReadOK(source->pub.input_file, source->iobuffer, source->buffer_width))
    ERREXIT(cinfo, JERR_INPUT_EOF);
  for (col = cinfo->image_width; col > 0; col--) {
    for (channel = 0; channel < 3; channel++) {
      unsigned int value = UCH(*bufferptr++) * 255U / source->maxval;
      *wide++ = value;
      *ptr++ = (JSAMPLE)value;
    }
  }
  return 1;
}

METHODDEF(JDIMENSION)
get_raw_row (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
/* This version is for reading raw-byte-format files with maxval = MAXJSAMPLE.
 * In this case we just read right into the JSAMPLE buffer!
 * Note that same code works for PPM and PGM files.
 */
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;

  if (! ReadOK(source->pub.input_file, source->iobuffer, source->buffer_width))
    ERREXIT(cinfo, JERR_INPUT_EOF);
  return 1;
}


METHODDEF(JDIMENSION)
get_word_gray_row (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
/* This version is for reading raw-word-format PGM files with any maxval */
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;
  register JSAMPROW ptr;
  register U_CHAR * bufferptr;
  register JSAMPLE *rescale = source->rescale;
  JDIMENSION col;

  if (! ReadOK(source->pub.input_file, source->iobuffer, source->buffer_width))
    ERREXIT(cinfo, JERR_INPUT_EOF);
  ptr = source->pub.buffer[0];
  bufferptr = source->iobuffer;
  for (col = cinfo->image_width; col > 0; col--) {
    register int temp;
    temp  = UCH(*bufferptr++);
    temp |= UCH(*bufferptr++) << 8;
    *ptr++ = rescale[temp];
  }
  return 1;
}


METHODDEF(JDIMENSION)
get_word_rgb_row (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
/* This version is for reading raw-word-format PPM files with any maxval */
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;
  register JSAMPROW ptr;
  register U_CHAR * bufferptr;
  register JSAMPLE *rescale = source->rescale;
  JDIMENSION col;

  if (! ReadOK(source->pub.input_file, source->iobuffer, source->buffer_width))
    ERREXIT(cinfo, JERR_INPUT_EOF);
  ptr = source->pub.buffer[0];
  bufferptr = source->iobuffer;
  for (col = cinfo->image_width; col > 0; col--) {
    register int temp;
    temp  = UCH(*bufferptr++);
    temp |= UCH(*bufferptr++) << 8;
    *ptr++ = rescale[temp];
    temp  = UCH(*bufferptr++);
    temp |= UCH(*bufferptr++) << 8;
    *ptr++ = rescale[temp];
    temp  = UCH(*bufferptr++);
    temp |= UCH(*bufferptr++) << 8;
    *ptr++ = rescale[temp];
  }
  return 1;
}


/*
 * Read the file header; return image size and component count.
 */

METHODDEF(void)
start_input_ppm (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
{
  ppm_source_ptr source = (ppm_source_ptr) sinfo;
  int c;
  unsigned int w, h, maxval;
  boolean need_iobuffer, use_raw_buffer, need_rescale;

  if (ppm_getc(source->pub.input_file) != 'P')
    ERREXIT(cinfo, JERR_PPM_NOT);

  c = ppm_getc(source->pub.input_file); /* subformat discriminator character */

  /* detect unsupported variants (ie, PBM) before trying to read header */
  switch (c) {
  case '3':			/* it's a text-format PPM file */
  case '6':			/* it's a raw-format PPM file */
    break;
  default:
    ERREXIT(cinfo, JERR_PPM_NOT);
    break;
  }

  /* fetch the remaining header info */
  w = read_pbm_integer(cinfo, source->pub.input_file);
  h = read_pbm_integer(cinfo, source->pub.input_file);
  maxval = read_pbm_integer(cinfo, source->pub.input_file);

  if (w <= 0 || h <= 0 || maxval <= 0 || w > 8192 || h > 8192 || (unsigned long)w * h >= 16777216UL) /* error check */
    ERREXIT(cinfo, JERR_PPM_NOT);

  source->wide_rgb = (unsigned int *)
    (*cinfo->mem->alloc_small)((j_common_ptr)cinfo, JPOOL_IMAGE,
                              (size_t)w * 3 * sizeof(unsigned int));
  source->maxval = maxval;
  cinfo->data_precision = BITS_IN_JSAMPLE; /* we always rescale data to this */
  cinfo->image_width = (JDIMENSION) w;
  cinfo->image_height = (JDIMENSION) h;

  if (c == '6') {
    if (maxval > 255) ERREXIT(cinfo, JERR_PPM_NOT);
    /* Native PC parser skips one byte; CR skips one additional byte. */
    if (ppm_getc(source->pub.input_file) == '\r') ppm_getc(source->pub.input_file);
  }

  /* initialize flags to most common settings */
  need_iobuffer = TRUE;		/* do we need an I/O buffer? */
  use_raw_buffer = FALSE;	/* do we map input buffer onto I/O buffer? */
  need_rescale = TRUE;		/* do we need a rescale array? */

  switch (c) {
  case '2':			/* it's a text-format PGM file */
    cinfo->input_components = 1;
    cinfo->in_color_space = JCS_GRAYSCALE;
    TRACEMS2(cinfo, 1, JTRC_PGM_TEXT, w, h);
    source->pub.get_pixel_rows = get_text_gray_row;
    need_iobuffer = FALSE;
    break;

  case '3':			/* it's a text-format PPM file */
    cinfo->input_components = 3;
    cinfo->in_color_space = JCS_RGB;
    TRACEMS2(cinfo, 1, JTRC_PPM_TEXT, w, h);
    source->pub.get_pixel_rows = get_text_rgb_row;
    need_iobuffer = FALSE;
    break;

  case '5':			/* it's a raw-format PGM file */
    cinfo->input_components = 1;
    cinfo->in_color_space = JCS_GRAYSCALE;
    TRACEMS2(cinfo, 1, JTRC_PGM, w, h);
    if (maxval > 255) {
      source->pub.get_pixel_rows = get_word_gray_row;
    } else if (maxval == MAXJSAMPLE && SIZEOF(JSAMPLE) == SIZEOF(U_CHAR)) {
      source->pub.get_pixel_rows = get_raw_row;
      use_raw_buffer = TRUE;
      need_rescale = FALSE;
    } else {
      source->pub.get_pixel_rows = get_scaled_gray_row;
    }
    break;

  case '6':			/* it's a raw-format PPM file */
    cinfo->input_components = 3;
    cinfo->in_color_space = JCS_RGB;
    TRACEMS2(cinfo, 1, JTRC_PPM, w, h);
    if (maxval > 255) {
      source->pub.get_pixel_rows = get_word_rgb_row;
    } else if (maxval == MAXJSAMPLE && SIZEOF(JSAMPLE) == SIZEOF(U_CHAR)) {
      source->pub.get_pixel_rows = get_raw_row;
      use_raw_buffer = TRUE;
      need_rescale = FALSE;
    } else {
      source->pub.get_pixel_rows = get_scaled_rgb_row;
    }
    break;
  }

  /* Allocate space for I/O buffer: 1 or 3 bytes or words/pixel. */
  if (need_iobuffer) {
    source->buffer_width = (size_t) w * cinfo->input_components *
      ((maxval<=255) ? SIZEOF(U_CHAR) : (2*SIZEOF(U_CHAR)));
    source->iobuffer = (U_CHAR *)
      (*cinfo->mem->alloc_small) ((j_common_ptr) cinfo, JPOOL_IMAGE,
				  source->buffer_width);
  }

  /* Create compressor input buffer. */
  if (use_raw_buffer) {
    /* For unscaled raw-input case, we can just map it onto the I/O buffer. */
    /* Synthesize a JSAMPARRAY pointer structure */
    /* Cast here implies near->far pointer conversion on PCs */
    source->pixrow = (JSAMPROW) source->iobuffer;
    source->pub.buffer = & source->pixrow;
    source->pub.buffer_height = 1;
  } else {
    /* Need to translate anyway, so make a separate sample buffer. */
    source->pub.buffer = (*cinfo->mem->alloc_sarray)
      ((j_common_ptr) cinfo, JPOOL_IMAGE,
       (JDIMENSION) w * cinfo->input_components, (JDIMENSION) 1);
    source->pub.buffer_height = 1;
  }

  /* Compute the rescaling array if required. */
  if (need_rescale) {
    INT32 val, half_maxval;

    /* On 16-bit-int machines we have to be careful of maxval = 65535 */
    source->rescale = (JSAMPLE *)
      (*cinfo->mem->alloc_small) ((j_common_ptr) cinfo, JPOOL_IMAGE,
				  (size_t) (((long) maxval + 1L) * SIZEOF(JSAMPLE)));
    half_maxval = maxval / 2;
    for (val = 0; val <= (INT32) maxval; val++) {
      /* The multiplication here must be done in 32 bits to avoid overflow */
      source->rescale[val] = (JSAMPLE) ((val*MAXJSAMPLE)/maxval);
    }
  }
}


/*
 * Finish up at the end of the file.
 */

METHODDEF(void)
finish_input_ppm (j_compress_ptr cinfo, cjpeg_source_ptr sinfo)
{
  /* no work */
}


/*
 * The module selection routine for PPM format input.
 */

GLOBAL(cjpeg_source_ptr)
jinit_read_ppm (j_compress_ptr cinfo)
{
  ppm_source_ptr source;

  /* Create module interface object */
  source = (ppm_source_ptr)
      (*cinfo->mem->alloc_small) ((j_common_ptr) cinfo, JPOOL_IMAGE,
				  SIZEOF(ppm_source_struct));
  /* Fill in method ptrs, except get_pixel_rows which start_input sets */
  source->pub.start_input = start_input_ppm;
  source->pub.finish_input = finish_input_ppm;

  return (cjpeg_source_ptr) source;
}

#endif /* PPM_SUPPORTED */
