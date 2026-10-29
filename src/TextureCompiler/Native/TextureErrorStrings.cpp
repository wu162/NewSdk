// PC status-code strings, reconstructed from the fixed native implementation.
#include "TextureNativeApi.h"
namespace {
const char Message_a201c[] = "ERROR NOT DEFINED";
const char Message_a2030[] = "IMAGE_EXCEEDS_INTERNAL_SIZE   ";
const char Message_a2050[] = "CUBE_MAP_NEEDS_SIX_FACES   ";
const char Message_a206c[] = "MEMORY_ALLOCATION_FAILED         ";
const char Message_a2090[] = "UNSUPPORTED_FORMAT         ";
const char Message_a20ac[] = "SURFACE_IS_VOLUME_MAP        ";
const char Message_a20cc[] = "SURFACE_IS_CUBE_MAP          ";
const char Message_a20ec[] = "NAN                       ";
const char Message_a2108[] = "CANT_CREATE_EFFECT        ";
const char Message_a2124[] = "FILE_FORMAT_NOT_SUPPORTED ";
const char Message_a2140[] = "FILE_NOT_ENOUGH_INDICES   ";
const char Message_a215c[] = "FILE_BAD_INDEX            ";
const char Message_a2178[] = "OUT_OF_MEMORY             ";
const char Message_a2194[] = "CANT_UNLOCK_TEXTURE       ";
const char Message_a21b0[] = "NEED_PIXEL_SHADER2        ";
const char Message_a21cc[] = "CANT_SET_MATRIX           ";
const char Message_a21e8[] = "INVALID_FILENAME          ";
const char Message_a2204[] = "CANT_FIND_FILE            ";
const char Message_a2220[] = "CANT_CLONE_MESH           ";
const char Message_a223c[] = "CANT_UNLOCK_INDEX_BUFFER  ";
const char Message_a2258[] = "CANT_COMPILE_EFFECT_FILE  ";
const char Message_a2274[] = "LINE_AND_PLANE_PARALLEL   ";
const char Message_a2290[] = "LINE_ON_PLANE             ";
const char Message_a22ac[] = "ZERO_TRIANGLE_AREA        ";
const char Message_a22c8[] = "ZERO_DIAGONAL             ";
const char Message_a22e4[] = "NO_CONVERGENCE            ";
const char Message_a2300[] = "CANT_CREATE_PRECONDITIONER  ";
const char Message_a2320[] = "CHART_AVE_DISTORTION_EXCEEDED  ";
const char Message_a2340[] = "CHART_MAX_DISTORTION_EXCEEDED  ";
const char Message_a2360[] = "CHART_NO_FACES           ";
const char Message_a237c[] = "CHART_OVERLAP            ";
const char Message_a2398[] = "CHART_NO_UNPINNED        ";
const char Message_a23b4[] = "CHART_ZERO_SIZE          ";
const char Message_a23d0[] = "CHART_NO_POINTS_FOUND    ";
const char Message_a23ec[] = "CHART_CANT_OPTIMIZE      ";
const char Message_a2408[] = "CHART_NO_DATA            ";
const char Message_a2424[] = "CHART_BAD_PERIMETER      ";
const char Message_a2440[] = "CHART_CANNOT_INVERT      ";
const char Message_a245c[] = "CHART_FOLDED             ";
const char Message_a2478[] = "NOT_RING_LOOP            ";
const char Message_a2494[] = "FAILED_UNLOCK            ";
const char Message_a24b0[] = "CANT_LOCK                ";
const char Message_a24cc[] = "BAD_ARG                  ";
const char Message_a24e8[] = "WRITE_FAILED             ";
const char Message_a2504[] = "READ_FAILED              ";
const char Message_a2520[] = "RGBA_IO_ERR              ";
const char Message_a253c[] = "HDR_IO_ERR               ";
const char Message_a2558[] = "CANT_DECOMPOSE_MATRIX    ";
const char Message_a2574[] = "NO_FILENAME              ";
const char Message_a2590[] = "PSD_IO_ERR               ";
const char Message_a25ac[] = "NEED_4_PLANES_FOR_RGBE   ";
const char Message_a25c8[] = "CANT_CREATE_VERTEX_DECL  ";
const char Message_a25e4[] = "CANT_CREATE_PIXEL_SHADER ";
const char Message_a2600[] = "CANT_CREATE_VERTEX_BUFFER  ";
const char Message_a261c[] = "UNKNOWN_FORMAT           ";
const char Message_a2638[] = "CANT_CREATE_TEXTURE      ";
const char Message_a2654[] = "BAD_FORMAT               ";
const char Message_a2670[] = "CANT_CREATE_CUBE_MAP     ";
const char Message_a268c[] = "CANT_CREATE_INDEX_BUFFER ";
const char Message_a26a8[] = "PNG_IO_ERR               ";
const char Message_a26c4[] = "TGA_RLE_ERR              ";
const char Message_a26e0[] = "TGA_IO_ERR               ";
const char Message_a26fc[] = "TIFF_IO_ERR              ";
const char Message_a2718[] = "GBM_IO_ERR               ";
const char Message_a2734[] = "BAD_FLAGS                ";
const char Message_a2750[] = "EMPTY_IMAGE              ";
const char Message_a276c[] = "CANT_INITIALIZE_CONVOLVER  ";
const char Message_a2788[] = "CANT_MAKE_VECTOR         ";
const char Message_a27a4[] = "CANT_INVERT_MATRIX       ";
const char Message_a27c0[] = "CANT_NORMALIZE           ";
const char Message_a27dc[] = "CANT_MAKE_MATRIX         ";
const char Message_a27f8[] = "IMAGE_NOT_POWER_2        ";
const char Message_a2814[] = "DEPTH_IS_NOT_3_OR_4      ";
const char Message_a2830[] = "INPUT_POINTER_ZERO       ";
const char Message_a284c[] = "OUTPUT_FILE_IS_READ_ONLY ";
const char Message_a2868[] = "IMAGE_NOT_MULT4          ";
const char Message_a2884[] = "CANT_OPEN_PROFILE        ";
const char Message_a28a0[] = "UNKNOWN_CONVERSION       ";
const char Message_a28bc[] = "IMAGES_NOT_SAME_SIZE     ";
const char Message_a28d8[] = "IMAGE_NOT_SQUARE         ";
const char Message_a28f4[] = "BAD_LIST_FILE_CONTENTS   ";
const char Message_a2910[] = "CANT_OPEN_LIST_FILE      ";
const char Message_a292c[] = "BAD_LIST_FILE            ";
const char Message_a2948[] = "CANT_DECOMPRESS_IMAGE    ";
const char Message_a2964[] = "BAD_OUTPUT_DIRECTORY     ";
const char Message_a2980[] = "BAD_OPTION               ";
const char Message_a299c[] = "Can't Open Output File    ";
const char Message_a29b8[] = "Can't Open Input File     ";
const char Message_a29d4[] = "FAIL                     ";
const char Message_a29f0[] = "OK                       ";
const char* const Messages[101] = {
    Message_a2030, // -100
    Message_a2050, // -99
    Message_a206c, // -98
    Message_a2090, // -97
    Message_a20ac, // -96
    Message_a20cc, // -95
    Message_a20ec, // -94
    Message_a2108, // -93
    Message_a2124, // -92
    Message_a2140, // -91
    Message_a215c, // -90
    Message_a201c, // -89
    Message_a201c, // -88
    Message_a201c, // -87
    Message_a26fc, // -86
    Message_a2178, // -85
    Message_a2194, // -84
    Message_a21b0, // -83
    Message_a21cc, // -82
    Message_a21e8, // -81
    Message_a2204, // -80
    Message_a2220, // -79
    Message_a223c, // -78
    Message_a201c, // -77
    Message_a2258, // -76
    Message_a2274, // -75
    Message_a2290, // -74
    Message_a22ac, // -73
    Message_a22c8, // -72
    Message_a22e4, // -71
    Message_a2300, // -70
    Message_a201c, // -69
    Message_a201c, // -68
    Message_a201c, // -67
    Message_a201c, // -66
    Message_a201c, // -65
    Message_a201c, // -64
    Message_a201c, // -63
    Message_a201c, // -62
    Message_a2320, // -61
    Message_a2340, // -60
    Message_a2360, // -59
    Message_a237c, // -58
    Message_a2398, // -57
    Message_a23b4, // -56
    Message_a23d0, // -55
    Message_a23ec, // -54
    Message_a2408, // -53
    Message_a2424, // -52
    Message_a2440, // -51
    Message_a245c, // -50
    Message_a2478, // -49
    Message_a2494, // -48
    Message_a24b0, // -47
    Message_a24cc, // -46
    Message_a24e8, // -45
    Message_a2504, // -44
    Message_a2520, // -43
    Message_a253c, // -42
    Message_a2558, // -41
    Message_a2574, // -40
    Message_a2590, // -39
    Message_a25ac, // -38
    Message_a25c8, // -37
    Message_a25e4, // -36
    Message_a2600, // -35
    Message_a261c, // -34
    Message_a2638, // -33
    Message_a2654, // -32
    Message_a2670, // -31
    Message_a268c, // -30
    Message_a26a8, // -29
    Message_a26c4, // -28
    Message_a26e0, // -27
    Message_a2718, // -26
    Message_a2734, // -25
    Message_a2750, // -24
    Message_a276c, // -23
    Message_a2788, // -22
    Message_a27a4, // -21
    Message_a27c0, // -20
    Message_a27dc, // -19
    Message_a27f8, // -18
    Message_a2814, // -17
    Message_a2830, // -16
    Message_a284c, // -15
    Message_a2868, // -14
    Message_a2884, // -13
    Message_a28a0, // -12
    Message_a28bc, // -11
    Message_a28d8, // -10
    Message_a28f4, // -9
    Message_a2910, // -8
    Message_a292c, // -7
    Message_a2948, // -6
    Message_a2964, // -5
    Message_a2980, // -4
    Message_a299c, // -3
    Message_a29b8, // -2
    Message_a29d4, // -1
    Message_a29f0, // 0
};
}
extern "C" const char* __cdecl TextureGetErrorString(std::int32_t code) {
    if (code < -100 || code > 0) return Message_a201c;
    return Messages[code + 100];
}
