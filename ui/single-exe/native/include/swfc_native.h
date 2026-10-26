/* SPDX-License-Identifier: GPL-3.0-only */
#ifndef SWFC_NATIVE_H
#define SWFC_NATIVE_H
#include <stdint.h>
#ifdef __cplusplus
extern "C" {
#endif

/* ABI v1: fixed-width values; no C++ objects/exceptions cross this boundary. */
uint32_t swfc_get_abi_version(void);
/* Bits: optimizer=1, triangulation=2, JPEG=4. Zero means none migrated. */
uint32_t swfc_get_features(void);

#ifdef __cplusplus
}
#endif
#endif
