using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using _003CCppImplementationDetails_003E;
using _003CCrtImplementationDetails_003E;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.TextureCompiler;
using Endian;
using Marshaler;
using Memory;
using Relo;
using SageBinaryData;
using eastl;

internal class _003CModule_003E
{
	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040GHFPNOJB_0040bad_003F5allocation_003F_0024AA_0040/* Not supported: data(62 61 64 20 61 6C 6C 6F 63 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07GKACAEDJ_0040Texture_003F_0024AA_0040/* Not supported: data(54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BL_0040_0024_0024CBD _003F_003F_C_0040_0BL_0040OELANADE_0040TerrainTextureAtlasRuntime_003F_0024AA_0040/* Not supported: data(54 65 72 72 61 69 6E 54 65 78 74 75 72 65 41 74 6C 61 73 52 75 6E 74 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07PANJAJNG_0040Color3f_003F_0024AA_0040/* Not supported: data(43 6F 6C 6F 72 33 66 00) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07PFJGBPFD_0040Color4f_003F_0024AA_0040/* Not supported: data(43 6F 6C 6F 72 34 66 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09KFNFGFBK_0040TimeRange_003F_0024AA_0040/* Not supported: data(54 69 6D 65 52 61 6E 67 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09IMMILGNF_0040RealRange_003F_0024AA_0040/* Not supported: data(52 65 61 6C 52 61 6E 67 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08NNHOCNMK_0040IntRange_003F_0024AA_0040/* Not supported: data(49 6E 74 52 61 6E 67 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040CKKGANJ_0040RandomVariable_003F_0024AA_0040/* Not supported: data(52 61 6E 64 6F 6D 56 61 72 69 61 62 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040BAEOEBCO_0040LogicRandomVariable_003F_0024AA_0040/* Not supported: data(4C 6F 67 69 63 52 61 6E 64 6F 6D 56 61 72 69 61 62 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BF_0040_0024_0024CBD _003F_003F_C_0040_0BF_0040GINIFKAB_0040ClientRandomVariable_003F_0024AA_0040/* Not supported: data(43 6C 69 65 6E 74 52 61 6E 64 6F 6D 56 61 72 69 61 62 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040NMEAADDN_0040ReplaceTexture_003F_0024AA_0040/* Not supported: data(52 65 70 6C 61 63 65 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040BNPMOACI_0040SoundOrEvaEvent_003F_0024AA_0040/* Not supported: data(53 6F 75 6E 64 4F 72 45 76 61 45 76 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040BEFBMPPK_0040MoneyTransaction_003F_0024AA_0040/* Not supported: data(4D 6F 6E 65 79 54 72 61 6E 73 61 63 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040EOININIB_0040OnDemandTexture_003F_0024AA_0040/* Not supported: data(4F 6E 44 65 6D 61 6E 64 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040FOIOEKEB_0040TerrainTextureCell_003F_0024AA_0040/* Not supported: data(54 65 72 72 61 69 6E 54 65 78 74 75 72 65 43 65 6C 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040BHFKAFCC_0040TerrainTextureTile_003F_0024AA_0040/* Not supported: data(54 65 72 72 61 69 6E 54 65 78 74 75 72 65 54 69 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040BNAHFJBD_0040TerrainTextureAtlas_003F_0024AA_0040/* Not supported: data(54 65 72 72 61 69 6E 54 65 78 74 75 72 65 41 74 6C 61 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BK_0040_0024_0024CBD _003F_003F_C_0040_0BK_0040DONGFNEA_0040TerrainTextureTileRuntime_003F_0024AA_0040/* Not supported: data(54 65 72 72 61 69 6E 54 65 78 74 75 72 65 54 69 6C 65 52 75 6E 74 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040/* Not supported: data(45 41 53 54 4C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0FB_0040_0024_0024CBD _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040/* Not supported: data(43 3A 5C 70 72 6F 6A 65 63 74 73 5C 52 61 33 5C 50 72 6F 64 75 63 74 69 6F 6E 5C 63 6F 64 65 5C 65 78 74 65 72 6E 61 6C 5C 45 41 53 54 4C 5C 31 2E 30 36 2E 30 31 5C 69 6E 63 6C 75 64 65 5C 45 41 53 54 4C 2F 61 6C 6C 6F 63 61 74 6F 72 2E 68 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040HPJLJGCI_0040CubeTexture_003F_0024AA_0040/* Not supported: data(43 75 62 65 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040CCNAHAGP_0040StandardTexture_003F_0024AA_0040/* Not supported: data(53 74 61 6E 64 61 72 64 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040LIBBLJME_0040VolumeTexture_003F_0024AA_0040/* Not supported: data(56 6F 6C 75 6D 65 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01KDCPPGHE_0040r_003F_0024AA_0040/* Not supported: data(72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01FJMABOPO_0040x_003F_0024AA_0040/* Not supported: data(78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01EANLCPLP_0040y_003F_0024AA_0040/* Not supported: data(79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04KOACHJEN_0040Type_003F_0024AA_0040/* Not supported: data(54 79 70 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY00_0024_0024CBD _003F_003F_C_0040_00CNPNBAHC_0040_003F_0024AA_0040/* Not supported: data(00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06MJDEEGAJ_0040IndexX_003F_0024AA_0040/* Not supported: data(49 6E 64 65 78 58 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06NACPHHEI_0040IndexY_003F_0024AA_0040/* Not supported: data(49 6E 64 65 78 59 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09OCLHBPOP_0040TextureID_003F_0024AA_0040/* Not supported: data(54 65 78 74 75 72 65 49 44 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040DCHNIHNI_0040BaseTexture_003F_0024AA_0040/* Not supported: data(42 61 73 65 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040KKHJNGCH_0040NormalTexture_003F_0024AA_0040/* Not supported: data(4E 6F 72 6D 61 6C 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04KCPDDLKF_0040Cell_003F_0024AA_0040/* Not supported: data(43 65 6C 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04OLCHHEMG_0040Tile_003F_0024AA_0040/* Not supported: data(54 69 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04CGOKOKBC_00402048_003F_0024AA_0040/* Not supported: data(32 30 34 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09JPAAJLPC_0040AtlasSize_003F_0024AA_0040/* Not supported: data(41 74 6C 61 73 53 69 7A 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040/* Not supported: data(74 72 75 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040ELMBKNGC_0040AllowLossyCompression_003F_0024AA_0040/* Not supported: data(41 6C 6C 6F 77 4C 6F 73 73 79 43 6F 6D 70 72 65 73 73 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040MOIKHIPB_0040D3DFMT_DXT5_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 35 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040KKOGLNPF_0040D3DFMT_DXT1_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0EA_0040_0024_0024CBE g_XGTextureFormatBitsPerPixel/* Not supported: data(01 01 08 10 10 10 20 20 08 08 10 10 10 20 20 10 20 20 04 08 08 40 20 20 10 20 40 10 20 40 10 20 40 20 40 80 20 40 80 08 10 10 20 08 08 10 10 10 20 08 20 04 08 08 20 20 20 60 04 04 04 04 20 20) */;

	internal static float DataInitializers_002E_003FA0x749b4e39_002ESECONDS_PER_LOGICFRAME_REAL/*Field data (rva=0xbfb18) could not be foundin any section!*/;

	internal static float DataInitializers_002E_003FA0x749b4e39_002EMSEC_PER_SECOND/* Not supported: data(00 00 7A 44) */;

	internal static float DataInitializers_002E_003FA0x749b4e39_002ERADS_PER_DEGREE/* Not supported: data(35 FA 8E 3C) */;

	internal static float DataInitializers_002E_003FA0x749b4e39_002ELOGICFRAMES_PER_SECOND/* Not supported: data(00 00 A0 40) */;

	internal static float DataInitializers_002E_003FA0x749b4e39_002ELOGICFRAMES_PER_SECONDS_REAL/* Not supported: data(00 00 A0 40) */;

	internal static float DataInitializers_002E_003FA0x749b4e39_002ELOGICFRAMES_PER_MSEC_REAL/* Not supported: data(0A D7 A3 3B) */;

	internal static float DataInitializers_002E_003FA0x749b4e39_002EPI/* Not supported: data(DB 0F 49 40) */;

	internal unsafe static delegate*<void> DataInitializers_002E_003FA0x749b4e39_002ESECONDS_PER_LOGICFRAME_REAL_0024initializer_0024/* Not supported: data(11 00 00 06) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_5 _003F_003F_R0PAD_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 50 41 44 00) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040 _003FA0x749b4e39_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4TextureType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4TextureType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B/* Not supported: data(FC 77 09 10 02 00 00 00 08 78 09 10 00 00 00 00 18 78 09 10 01 00 00 00) */;

	internal unsafe static Direct3D* BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3D/*Field data (rva=0xbf6b4) could not be foundin any section!*/;

	internal unsafe static XenonBundler* BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Bundler/*Field data (rva=0xbf3c0) could not be foundin any section!*/;

	internal unsafe static SimpleBuffer* BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_Buffer/*Field data (rva=0xbf3c4) could not be foundin any section!*/;

	internal unsafe static D3DDevice* BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3DDevice/*Field data (rva=0xbf6b8) could not be foundin any section!*/;

	internal static uint _003FTypeHash_0040Color4f_0040SageBinaryData_0040_00402IB/* Not supported: data(61 97 24 CD) */;

	internal static uint _003FTypeHash_0040LogicRandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(55 6E 41 51) */;

	internal static uint _003FTypeHash_0040TerrainTextureTileRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(D0 8F F3 B1) */;

	internal static uint _003FTypeHash_0040TimeRange_0040SageBinaryData_0040_00402IB/* Not supported: data(9F 6D 09 2F) */;

	internal unsafe static sbyte* _003FTypeName_0040ClientRandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(B0 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Color3f_0040SageBinaryData_0040_00402PBDB/* Not supported: data(18 74 09 10) */;

	internal static uint _003FTypeHash_0040MoneyTransaction_0040SageBinaryData_0040_00402IB/* Not supported: data(F5 0B 52 C2) */;

	internal static uint _003FTypeHash_0040TerrainTextureCell_0040SageBinaryData_0040_00402IB/* Not supported: data(C2 6F 8F 5C) */;

	internal unsafe static sbyte* _003FTypeName_0040OnDemandTexture_0040SageBinaryData_0040_00402PBDB/* Not supported: data(6C 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040TerrainTextureTile_0040SageBinaryData_0040_00402PBDB/* Not supported: data(44 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Color4f_0040SageBinaryData_0040_00402PBDB/* Not supported: data(10 74 09 10) */;

	internal static uint _003FTypeHash_0040Texture_0040SageBinaryData_0040_00402IB/* Not supported: data(DA F8 5F 9F) */;

	internal static uint _003FTypeHash_0040TerrainTextureAtlasRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(BC 07 7D 46) */;

	internal static uint _003FTypeHash_0040RealRange_0040SageBinaryData_0040_00402IB/* Not supported: data(E0 E0 EA F4) */;

	internal unsafe static sbyte* _003FTypeName_0040TerrainTextureAtlasRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(20 74 09 10) */;

	internal static uint _003FTypeHash_0040TerrainTextureAtlas_0040SageBinaryData_0040_00402IB/* Not supported: data(EA 13 E9 18) */;

	internal unsafe static sbyte* _003FTypeName_0040TerrainTextureTileRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(14 73 09 10) */;

	internal static uint _003FTypeHash_0040RandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(5F B4 AC C7) */;

	internal unsafe static sbyte* _003FTypeName_0040MoneyTransaction_0040SageBinaryData_0040_00402PBDB/* Not supported: data(7C 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040LogicRandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C8 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040ReplaceTexture_0040SageBinaryData_0040_00402PBDB/* Not supported: data(A0 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Texture_0040SageBinaryData_0040_00402PBDB/* Not supported: data(3C 74 09 10) */;

	internal static uint _003FTypeHash_0040ReplaceTexture_0040SageBinaryData_0040_00402IB/* Not supported: data(00 49 32 8C) */;

	internal static uint _003FTypeHash_0040OnDemandTexture_0040SageBinaryData_0040_00402IB/* Not supported: data(DE 8E 48 F8) */;

	internal unsafe static sbyte* _003FTypeName_0040TimeRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(04 74 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040SoundOrEvaEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(90 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040TerrainTextureAtlas_0040SageBinaryData_0040_00402PBDB/* Not supported: data(30 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040IntRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(EC 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040RandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(DC 73 09 10) */;

	internal static uint _003FTypeHash_0040ClientRandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(26 E0 35 42) */;

	internal static uint _003FTypeHash_0040TerrainTextureTile_0040SageBinaryData_0040_00402IB/* Not supported: data(16 55 D1 F9) */;

	internal static uint _003FTypeHash_0040SoundOrEvaEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(DE 4F AE 40) */;

	internal unsafe static sbyte* _003FTypeName_0040RealRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(F8 73 09 10) */;

	internal unsafe static sbyte* _003FTypeName_0040TerrainTextureCell_0040SageBinaryData_0040_00402PBDB/* Not supported: data(58 73 09 10) */;

	internal static uint _003FTypeHash_0040Color3f_0040SageBinaryData_0040_00402IB/* Not supported: data(16 5A D0 9B) */;

	internal static uint _003FTypeHash_0040IntRange_0040SageBinaryData_0040_00402IB/* Not supported: data(B5 BF 53 89) */;

	internal unsafe static sbyte* std_002E_003FA0x749b4e39_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	public unsafe static int** __unep_0040_003FTextureSort_0040TexturePacker_0040TextureCompiler_0040BinaryAssetBuilder_0040_0040_0024_0024FCA_NPAUTexture_0040123_00400_0040Z/* Not supported: data(E0 19 00 10) */;

	public unsafe static int** __unep_0040_003FnvDXTWriteCallback_0040TextureCompiler_0040BinaryAssetBuilder_0040_0040_0024_0024FYA_003FAW4NV_ERROR_CODE_0040_0040PBXIPBUMIPMapData_0040_0040PAX_0040Z/* Not supported: data(50 1A 00 10) */;

	public unsafe static int** __unep_0040_003F_003F_0024BookmarkCompare_0040U_003F_0024GenericBookmark_0040I_0040Tracker_0040Relo_0040_0040_0040Tracker_0040Relo_0040_0040_0024_0024FCA_NABU_003F_0024GenericBookmark_0040I_004001_00400_0040Z/* Not supported: data(70 28 00 10) */;

	internal unsafe static sbyte* std_002E_003FA0xa5d82742_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3IList_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 14 51 0B 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2INode_0040Marshaler_0040_00408/* Not supported: data(D4 50 0B 10 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4Value_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 4C 81 0B 10 4C 51 0B 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2Value_0040_00408/* Not supported: data(68 51 0B 10 8C 50 0B 10 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05Q6AXXZ _003F_003F_7List_0040_00406B_0040/* Not supported: data(40 7F 00 10 F0 7E 00 10 60 7D 00 10 20 75 00 10 A0 87 00 10 04 20 0A 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_12 _003F_003F_R0_003FAVValue_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 56 61 6C 75 65 40 40 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040Node_0040_00408/* Not supported: data(7C 81 0B 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 98 51 0B 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2List_0040_00408/* Not supported: data(00 52 0B 10 1C 51 0B 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3INode_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 CC 50 0B 10) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7Value_0040_00406B_0040/* Not supported: data(40 75 00 10 70 75 00 10 90 80 00 10 04 20 0A 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040IList_0040Marshaler_0040_00408/* Not supported: data(08 81 0B 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 04 51 0B 10) */;

	internal static _0024ArrayType_0024_0024_0024BY06Q6AXXZ _003F_003F_7Node_0040_00406B_0040/* Not supported: data(F0 74 00 10 C0 83 00 10 70 7B 00 10 30 85 00 10 50 86 00 10 E0 81 00 10 04 20 0A 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4IList_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 08 81 0B 10 04 51 0B 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3IValue_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 84 50 0B 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_23 _003F_003F_R0_003FAVIValue_0040Marshaler_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 49 56 61 6C 75 65 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3List_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 F4 51 0B 10) */;

	internal static _0024ArrayType_0024_0024_0024BY06Q6AXXZ _003F_003F_7INode_0040Marshaler_0040_00406B_0040/* Not supported: data(A8 E7 04 10 A8 E7 04 10 A8 E7 04 10 A8 E7 04 10 A8 E7 04 10 50 74 00 10 04 20 0A 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4IValue_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 8C 80 0B 10 74 50 0B 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040Value_0040_00408/* Not supported: data(4C 81 0B 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 4C 51 0B 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4Node_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 7C 81 0B 10 98 51 0B 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3Value_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 5C 51 0B 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_11 _003F_003F_R0_003FAVNode_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 4E 6F 64 65 40 40 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_22 _003F_003F_R0_003FAVINode_0040Marshaler_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 49 4E 6F 64 65 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_22 _003F_003F_R0_003FAVIList_0040Marshaler_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 49 4C 69 73 74 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2IList_0040Marshaler_0040_00408/* Not supported: data(1C 51 0B 10 00) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2IValue_0040Marshaler_0040_00408/* Not supported: data(8C 50 0B 10 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040List_0040_00408/* Not supported: data(AC 81 0B 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 E4 51 0B 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040IValue_0040Marshaler_0040_00408/* Not supported: data(8C 80 0B 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 74 50 0B 10) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7IValue_0040Marshaler_0040_00406B_0040/* Not supported: data(A8 E7 04 10 A8 E7 04 10 E0 73 00 10 04 20 0A 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2Node_0040_00408/* Not supported: data(B4 51 0B 10 D4 50 0B 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3Node_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 A8 51 0B 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4INode_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 CC 80 0B 10 BC 50 0B 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4List_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 AC 81 0B 10 E4 51 0B 10) */;

	internal unsafe static sbyte* std_002E_003FA0x52c0261d_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040INode_0040Marshaler_0040_00408/* Not supported: data(CC 80 0B 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 BC 50 0B 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_11 _003F_003F_R0_003FAVList_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 4C 69 73 74 40 40 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05Q6AXXZ _003F_003F_7IList_0040Marshaler_0040_00406B_0040/* Not supported: data(A8 E7 04 10 A8 E7 04 10 A8 E7 04 10 A8 E7 04 10 C0 74 00 10 04 20 0A 10) */;

	internal static uint Memory_002ENumAllocs/*Field data (rva=0xbfb1c) could not be foundin any section!*/;

	internal unsafe static AllocInfo* Memory_002ETail/*Field data (rva=0xbfb20) could not be foundin any section!*/;

	internal static _0024ArrayType_0024_0024_0024BY01I Memory_002EkPostGuard/*Field data (rva=0xbfb24) could not be foundin any section!*/;

	internal static _0024ArrayType_0024_0024_0024BY01I Memory_002EkPreGuard/*Field data (rva=0xbfb2c) could not be foundin any section!*/;

	internal unsafe static AllocInfo* Memory_002EHead/*Field data (rva=0xbfb34) could not be foundin any section!*/;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040FCGOGLON_0040D3DFMT_UNKNOWN_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 55 4E 4B 4E 4F 57 4E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040IBMLOODG_0040D3DFMT_DXT2_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040JINANPHH_0040D3DFMT_DXT3_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 33 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040NHJBEJLA_0040D3DFMT_DXT4_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 34 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040NACFAIP_0040D3DFMT_DXN_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 4E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09JNGIHEFB_0040D3DFMT_A8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09JFLAOHAC_0040D3DFMT_L8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040HMHILKM_0040D3DFMT_R5G6B5_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 35 47 36 42 35 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040JDOGFGOM_0040D3DFMT_R6G5B5_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 36 47 35 42 35 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040OAOOLKLI_0040D3DFMT_L6V5U5_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 36 56 35 55 35 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040MDABPPEB_0040D3DFMT_X1R5G5B5_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 58 31 52 35 47 35 42 35 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040MDGCALGL_0040D3DFMT_A1R5G5B5_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 31 52 35 47 35 42 35 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040OBHJIAIO_0040D3DFMT_A4R4G4B4_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 34 52 34 47 34 42 34 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040OBBKHEKE_0040D3DFMT_X4R4G4B4_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 58 34 52 34 47 34 42 34 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040FLGIOGJP_0040D3DFMT_Q4W4V4U4_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 51 34 57 34 56 34 55 34 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040JKNHIMAH_0040D3DFMT_A8L8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 38 4C 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040DCPPHNN_0040D3DFMT_G8R8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 47 38 52 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040FLOAFPGK_0040D3DFMT_V8U8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 56 38 55 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040KNOCFDNO_0040D3DFMT_D16_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040GIFGHLDB_0040D3DFMT_L16_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040CIEFJHCB_0040D3DFMT_R16F_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 31 36 46 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040BDHOKPEN_0040D3DFMT_R16F_EXPAND_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 31 36 46 5F 45 58 50 41 4E 44 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040OBHKFBGD_0040D3DFMT_UYVY_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 55 59 56 59 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040MNNJBFOL_0040D3DFMT_LE_UYVY_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 45 5F 55 59 56 59 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040DEPNMMII_0040D3DFMT_G8R8_G8B8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 47 38 52 38 5F 47 38 42 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040PAKPEPA_0040D3DFMT_R8G8_B8G8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 38 47 38 5F 42 38 47 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040ODAJPGIL_0040D3DFMT_YUY2_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 59 55 59 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040MPKKLCAD_0040D3DFMT_LE_YUY2_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 45 5F 59 55 59 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040CHHHKFAO_0040D3DFMT_A8R8G8B8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 38 52 38 47 38 42 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040CHBEFBCE_0040D3DFMT_X8R8G8B8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 58 38 52 38 47 38 42 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040FMIPJKLA_0040D3DFMT_A8B8G8R8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 38 42 38 47 38 52 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040FMOMGOJK_0040D3DFMT_X8B8G8R8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 58 38 42 38 47 38 52 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040FEBMLNHA_0040D3DFMT_X8L8V8U8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 58 38 4C 38 56 38 55 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040JNGGMDBP_0040D3DFMT_Q8W8V8U8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 51 38 57 38 56 38 55 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040JDACEKMG_0040D3DFMT_A2R10G10B10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 32 52 31 30 47 31 30 42 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040CLDJNKL_0040D3DFMT_X2R10G10B10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 58 32 52 31 30 47 31 30 42 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040OELFJMLB_0040D3DFMT_A2B10G10R10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 32 42 31 30 47 31 30 52 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040HJEMHJNO_0040D3DFMT_A2W10V10U10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 32 57 31 30 56 31 30 55 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040HHMKEDKE_0040D3DFMT_A16L16_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 31 36 4C 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040KENMFGDC_0040D3DFMT_G16R16_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 47 31 36 52 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040PIKCPJPB_0040D3DFMT_V16U16_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 56 31 36 55 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040BGKMGBIG_0040D3DFMT_R10G11B11_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 31 30 47 31 31 42 31 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040MDBNFAFJ_0040D3DFMT_R11G11B10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 31 31 47 31 31 42 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040PMOCFCJO_0040D3DFMT_W10V11U11_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 57 31 30 56 31 31 55 31 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040CJFDGDEB_0040D3DFMT_W11V11U10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 57 31 31 56 31 31 55 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040GOFAGFFF_0040D3DFMT_G16R16F_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 47 31 36 52 31 36 46 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040EFHMPMHE_0040D3DFMT_G16R16F_EXPAND_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 47 31 36 52 31 36 46 5F 45 58 50 41 4E 44 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040PLOGKFL_0040D3DFMT_L32_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040IFEFPHHG_0040D3DFMT_R32F_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 52 33 32 46 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040GGADDKEH_0040D3DFMT_A16B16G16R16_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 31 36 42 31 36 47 31 36 52 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040EPPNHJPC_0040D3DFMT_Q16W16V16U16_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 51 31 36 57 31 36 56 31 36 55 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BF_0040_0024_0024CBD _003F_003F_C_0040_0BF_0040EOPNDPIK_0040D3DFMT_A16B16G16R16F_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 31 36 42 31 36 47 31 36 52 31 36 46 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BM_0040_0024_0024CBD _003F_003F_C_0040_0BM_0040JNGHFAFA_0040D3DFMT_A16B16G16R16F_EXPAND_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 31 36 42 31 36 47 31 36 52 31 36 46 5F 45 58 50 41 4E 44 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040KIGKFFAF_0040D3DFMT_A32L32_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 33 32 4C 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040HLHMEAJD_0040D3DFMT_G32R32_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 47 33 32 52 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040CHACOPFA_0040D3DFMT_V32U32_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 56 33 32 55 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040MPFOFGDN_0040D3DFMT_G32R32F_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 47 33 32 52 33 32 46 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040EPPNOHIH_0040D3DFMT_A32B32G32R32_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 33 32 42 33 32 47 33 32 52 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040GGADKEDC_0040D3DFMT_Q32W32V32U32_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 51 33 32 57 33 32 56 33 32 55 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BF_0040_0024_0024CBD _003F_003F_C_0040_0BF_0040NFLAADOH_0040D3DFMT_A32B32G32R32F_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 41 33 32 42 33 32 47 33 32 52 33 32 46 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040MPJIGJCF_0040D3DFMT_LE_X8R8G8B8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 45 5F 58 38 52 38 47 38 42 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040MPPLJNAP_0040D3DFMT_LE_A8R8G8B8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 45 5F 41 38 52 38 47 38 42 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040PAOAIEIE_0040D3DFMT_LE_X2R10G10B10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 45 5F 58 32 52 31 30 47 31 30 42 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040GBFBFDOJ_0040D3DFMT_LE_A2R10G10B10_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 45 5F 41 32 52 31 30 47 31 30 42 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040CHJLEKAE_0040D3DFMT_DXT3A_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 33 41 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BC_0040_0024_0024CBD _003F_003F_C_0040_0BC_0040OAFAMHCI_0040D3DFMT_DXT3A_1111_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 33 41 5F 31 31 31 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040CDBGDGLG_0040D3DFMT_DXT5A_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 58 54 35 41 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040FLAKCHDJ_0040D3DFMT_CTX1_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 43 54 58 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040HJBFNLHE_0040D3DFMT_D24S8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 32 34 53 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040HFEADEJF_0040D3DFMT_D24X8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 32 34 58 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040GDMLJPGJ_0040D3DFMT_D24FS8_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 32 34 46 53 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040MKAKECLE_0040D3DFMT_D32_003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 44 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040JIPMKGEB_0040D3DFMT_LIN__003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 4C 49 4E 5F 00) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07NCBDPBMK_0040D3DFMT__003F_0024AA_0040/* Not supported: data(44 33 44 46 4D 54 5F 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BJ_0040_0024_0024CBD _003F_003F_C_0040_0BJ_0040CBKBDGBF_0040Image_003F5resize_003F5Error_003F3_003F5_003F_0024CF08x_003F_0024AA_0040/* Not supported: data(49 6D 61 67 65 20 72 65 73 69 7A 65 20 45 72 72 6F 72 3A 20 25 30 38 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BK_0040_0024_0024CBD _003F_003F_C_0040_0BK_0040JNIDALON_0040XGCopySurface_003F5Error_003F3_003F5_003F_0024CF08x_003F_0024AA_0040/* Not supported: data(58 47 43 6F 70 79 53 75 72 66 61 63 65 20 45 72 72 6F 72 3A 20 25 30 38 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DL_0040_0024_0024CBD _003F_003F_C_0040_0DL_0040DFFIKCNG_0040Top_003F5mip_003F5map_003F5of_003F5texture_003F5is_003F5all_003F5bl_0040/* Not supported: data(54 6F 70 20 6D 69 70 20 6D 61 70 20 6F 66 20 74 65 78 74 75 72 65 20 69 73 20 61 6C 6C 20 62 6C 61 63 6B 2E 20 54 68 61 74 27 73 20 70 72 6F 62 61 62 6C 79 20 77 72 6F 6E 67 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_19 _003F_003F_R0_003FAVCBaseTexture_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 43 42 61 73 65 54 65 78 74 75 72 65 40 40 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040CBaseTexture_0040_00408/* Not supported: data(F8 81 0B 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 30 52 0B 10) */;

	internal static _s__ThrowInfo _TI2CPAD/* Not supported: data(01 00 00 00 00 00 00 00 00 00 00 00 58 57 0B 10) */;

	internal static _0024ArrayType_0024_0024_0024BY0EK_0040_0024_0024CBUFORMATSPEC_0040_0040 _003FA0x4b7d5feb_002Eg_TextureFormats/* Not supported: data(84 7D 09 10 FF FF FF FF C8 78 09 10 52 01 20 1A 78 7D 09 10 53 01 20 1A 6C 7D 09 10 53 01 20 1A 60 7D 09 10 54 01 20 1A D4 78 09 10 54 01 20 1A 54 7D 09 10 71 01 20 1A 48 7D 09 10 02 01 90 04 3C 7D 09 10 02 01 00 28 2C 7D 09 10 44 01 28 28 1C 7D 09 10 45 01 28 28 0C 7D 09 10 45 0B 20 2A FC 7C 09 10 43 01 28 28 EC 7C 09 10 43 01 28 18 DC 7C 09 10 4F 01 28 18 CC 7C 09 10 4F 01 28 28 BC 7C 09 10 4F AB 20 1A B0 7C 09 10 4A 01 00 08 A4 7C 09 10 4A 01 20 2D 98 7C 09 10 4A AB 20 2D 8C 7C 09 10 58 01 22 1A 80 7C 09 10 58 01 00 28 74 7C 09 10 5E AB A2 2D 60 7C 09 10 5B AB A2 2D 54 7C 09 10 4C 01 20 1A 44 7C 09 10 0C 01 20 1A 30 7C 09 10 4C 01 28 18 1C 7C 09 10 4B 01 28 18 10 7C 09 10 4B 01 20 1A 00 7C 09 10 0B 01 20 1A F0 7B 09 10 86 01 28 18 E0 7B 09 10 86 01 28 28 D0 7B 09 10 86 01 20 1A C0 7B 09 10 86 01 20 2A B0 7B 09 10 86 0B 20 2A A0 7B 09 10 86 AB 20 1A 8C 7B 09 10 B6 01 28 18 78 7B 09 10 B6 01 28 28 64 7B 09 10 B6 01 20 1A 50 7B 09 10 B6 2B 20 1A 40 7B 09 10 99 01 00 08 30 7B 09 10 99 01 20 2D 20 7B 09 10 99 AB 20 2D 0C 7B 09 10 B7 01 28 28 F8 7A 09 10 B8 01 28 28 E4 7A 09 10 B7 AB 20 2A D0 7A 09 10 B8 AB 20 2A C0 7A 09 10 9F AB 22 2D A8 7A 09 10 9C AB 22 2D 9C 7A 09 10 A1 01 00 28 90 7A 09 10 A4 AB A2 2D 7C 7A 09 10 5A 01 20 1A 68 7A 09 10 5A AB 20 1A 50 7A 09 10 60 AB 22 1A 34 7A 09 10 5D AB 22 1A 24 7A 09 10 A2 01 00 08 14 7A 09 10 A2 01 20 2D 04 7A 09 10 A2 AB 20 2D F4 79 09 10 A5 AB 22 2D E0 79 09 10 A3 01 20 1A CC 79 09 10 A3 AB 20 1A B4 79 09 10 A6 AB 22 1A A0 79 09 10 06 01 28 28 8C 79 09 10 06 01 28 18 74 79 09 10 36 01 28 28 5C 79 09 10 36 01 28 18 4C 79 09 10 7A 01 20 1A 38 79 09 10 7D 01 20 1A 28 79 09 10 7B 01 20 1A 1C 79 09 10 7C 01 20 1A 0C 79 09 10 96 01 20 2D FC 78 09 10 96 01 A0 2D EC 78 09 10 97 01 22 1A E0 78 09 10 A1 01 22 1A) */;

	internal static _0024_s__CatchableTypeArray_0024_extraBytes_8 _CTA2PAD/* Not supported: data(02 00 00 00 64 57 0B 10 80 57 0B 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2CBaseTexture_0040_00408/* Not supported: data(48 52 0B 10 00) */;

	internal static _s__ThrowInfo _TI2PAD/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 58 57 0B 10) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7CBaseTexture_0040_00406B_0040/* Not supported: data(00 89 00 10 60 93 00 10 A8 E7 04 10 04 20 0A 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3CBaseTexture_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 40 52 0B 10) */;

	internal unsafe static sbyte* std_002E_003FA0x4b7d5feb_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _s__CatchableType _CT_003F_003F_R0PAX_004084/* Not supported: data(01 00 00 00 14 82 0B 10 00 00 00 00 FF FF FF FF 00 00 00 00 04 00 00 00 00 00 00 00) */;

	internal static _s__CatchableType _CT_003F_003F_R0PAD_004084/* Not supported: data(01 00 00 00 64 80 0B 10 00 00 00 00 FF FF FF FF 00 00 00 00 04 00 00 00 00 00 00 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_5 _003F_003F_R0PAX_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 50 41 58 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4CBaseTexture_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 F8 81 0B 10 30 52 0B 10) */;

	internal static uint _003FA0x32e32d13_002Eg_ulFPU/*Field data (rva=0xbfb38) could not be foundin any section!*/;

	internal unsafe static sbyte* std_002E_003FA0x32e32d13_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01EEMJAFIK_0040_003F6_003F_0024AA_0040/* Not supported: data(0A 00) */;

	internal unsafe static sbyte* std_002E_003FA0x97183c60_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _0024ArrayType_0024_0024_0024BY0EB_0040_0024_0024CBD _003F_003F_C_0040_0EB_0040KKNIMHJO_0040Couldn_003F8t_003F5create_003F5the_003F5internal_003F5ima_0040/* Not supported: data(43 6F 75 6C 64 6E 27 74 20 63 72 65 61 74 65 20 74 68 65 20 69 6E 74 65 72 6E 61 6C 20 69 6D 61 67 65 20 66 72 6F 6D 20 74 68 65 20 69 6E 20 6D 65 6D 6F 72 79 20 52 47 42 41 49 6D 61 67 65 2E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BN_0040_0024_0024CBD _003F_003F_C_0040_0BN_0040EMNJFBLD_0040Invalid_003F5texture_003F5format_003F5_003F_0024CC_003F_0024CFs_003F_0024CC_003F6_003F_0024AA_0040/* Not supported: data(49 6E 76 61 6C 69 64 20 74 65 78 74 75 72 65 20 66 6F 72 6D 61 74 20 22 25 73 22 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7CTexture2D_0040_00406B_0040/* Not supported: data(00 89 00 10 60 93 00 10 50 B5 00 10 04 20 0A 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3CTexture2D_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 88 52 0B 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040CTexture2D_0040_00408/* Not supported: data(40 82 0B 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 78 52 0B 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4CTexture2D_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 40 82 0B 10 78 52 0B 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_17 _003F_003F_R0_003FAVCTexture2D_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 43 54 65 78 74 75 72 65 32 44 40 40 00) */;

	internal unsafe static sbyte* std_002E_003FA0xc6b976f1_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2CTexture2D_0040_00408/* Not supported: data(94 52 0B 10 48 52 0B 10 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0EG_0040_0024_0024CBD _003F_003F_C_0040_0EG_0040DHMLJHNK_0040Couldn_003F8t_003F5create_003F5the_003F5internal_003F5ima_0040/* Not supported: data(43 6F 75 6C 64 6E 27 74 20 63 72 65 61 74 65 20 74 68 65 20 69 6E 74 65 72 6E 61 6C 20 69 6D 61 67 65 20 66 72 6F 6D 20 74 68 65 20 69 6E 20 6D 65 6D 6F 72 79 20 76 6F 6C 75 6D 65 20 74 65 78 74 75 72 65 2E 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040CVolumeTexture_0040_00408/* Not supported: data(70 82 0B 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 C4 52 0B 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_21 _003F_003F_R0_003FAVCVolumeTexture_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 43 56 6F 6C 75 6D 65 54 65 78 74 75 72 65 40 40 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7CVolumeTexture_0040_00406B_0040/* Not supported: data(00 89 00 10 B0 BA 00 10 D0 B9 00 10 04 20 0A 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3CVolumeTexture_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 D4 52 0B 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2CVolumeTexture_0040_00408/* Not supported: data(E0 52 0B 10 48 52 0B 10 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4CVolumeTexture_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 70 82 0B 10 C4 52 0B 10) */;

	internal unsafe static sbyte* std_002E_003FA0xda7a3648_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _0024ArrayType_0024_0024_0024BY02Q6AXXZ _003F_003F_7bad_alloc_0040std_0040_00406B_0040/* Not supported: data(A0 C2 00 10 C2 E9 04 10 04 20 0A 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040exception_0040std_0040_00408/* Not supported: data(94 82 0B 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 18 53 0B 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040logic_error_0040std_0040_00408/* Not supported: data(BC 82 0B 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 44 53 0B 10) */;

	internal unsafe static XenonBundler* g_pBundler/*Field data (rva=0xbfb40) could not be foundin any section!*/;

	internal static _0024_TypeDescriptor_0024_extraBytes_20 _003F_003F_R0_003FAVexception_0040std_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 65 78 63 65 70 74 69 6F 6E 40 73 74 64 40 40 00) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2bad_alloc_0040std_0040_00408/* Not supported: data(AC 53 0B 10 FC 52 0B 10 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040bad_alloc_0040std_0040_00408/* Not supported: data(E8 82 0B 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 90 53 0B 10) */;

	internal static _0024ArrayType_0024_0024_0024BY02Q6AXXZ _003F_003F_7logic_error_0040std_0040_00406B_0040/* Not supported: data(B0 BD 00 10 A0 BD 00 10 04 20 0A 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3bad_alloc_0040std_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 A0 53 0B 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_20 _003F_003F_R0_003FAVbad_alloc_0040std_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 62 61 64 5F 61 6C 6C 6F 63 40 73 74 64 40 40 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4logic_error_0040std_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 BC 82 0B 10 44 53 0B 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2exception_0040std_0040_00408/* Not supported: data(FC 52 0B 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3exception_0040std_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 28 53 0B 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2logic_error_0040std_0040_00408/* Not supported: data(60 53 0B 10 FC 52 0B 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3logic_error_0040std_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 54 53 0B 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_22 _003F_003F_R0_003FAVlogic_error_0040std_0040_0040_00408/* Not supported: data(04 20 0A 10 00 00 00 00 2E 3F 41 56 6C 6F 67 69 63 5F 65 72 72 6F 72 40 73 74 64 40 40 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4bad_alloc_0040std_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 E8 82 0B 10 90 53 0B 10) */;

	internal unsafe static sbyte* std_002E_003FA0xc96aae59_002E_bad_alloc_Message/* Not supported: data(44 74 09 10) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_mp_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static int _003FUninitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FUninitialized_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(8D 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xi_vt_a/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedPerAppDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedPerAppDomain_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(92 00 00 06) */;

	[FixedAddressValueType]
	internal static bool _003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FIsDefaultDomain_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(8E 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_ma_a/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedNative_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedNative_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(90 00 00 06) */;

	[FixedAddressValueType]
	internal static int _003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitialized_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(8C 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_ma_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedVtables_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedVtables_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(8F 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xi_vt_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedPerProcess_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedPerProcess_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(91 00 00 06) */;

	internal static bool _003FInitializedPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0xbffcf) could not be foundin any section!*/;

	internal static bool _003FEntered_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0xbffcc) could not be foundin any section!*/;

	internal static bool _003FInitializedNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0xbffcd) could not be foundin any section!*/;

	internal static int _003FCount_0040AllDomains_0040_003CCrtImplementationDetails_003E_0040_00402HA/*Field data (rva=0xbffc8) could not be foundin any section!*/;

	internal static _003CCrtImplementationDetails_003E.TriBool.State _003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A/* Not supported: data() */;

	internal static _003CCrtImplementationDetails_003E.TriBool.State _003FhasPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A/* Not supported: data() */;

	internal static bool _003FInitializedNativeFromCCTOR_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0xbffce) could not be foundin any section!*/;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_mp_a/* Not supported: data(00 00 00 00) */;

	public unsafe static int** __unep_0040_003FDoNothing_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z/* Not supported: data(85 F4 04 10) */;

	public unsafe static int** __unep_0040_003F_UninitializeDefaultDomain_0040LanguageSupport_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z/* Not supported: data(01 F6 04 10) */;

	[FixedAddressValueType]
	internal static uint __exit_list_size_app_domain;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x23130d9f_002E_003F_lock_0024initializer_0024_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0P6MXXZA/* Not supported: data(AB 00 00 06) */;

	[FixedAddressValueType]
	internal unsafe static delegate*<void>* __onexitbegin_app_domain;

	internal static uint _003FA0x23130d9f_002E__exit_list_size/*Field data (rva=0xc00cc) could not be foundin any section!*/;

	[FixedAddressValueType]
	internal unsafe static delegate*<void>* __onexitend_app_domain;

	internal unsafe static delegate*<void>* _003FA0x23130d9f_002E__onexitbegin_m/*Field data (rva=0xc00c4) could not be foundin any section!*/;

	internal unsafe static delegate*<void>* _003FA0x23130d9f_002E__onexitend_m/*Field data (rva=0xc00c8) could not be foundin any section!*/;

	public static _0024ArrayType_0024_0024_0024BY01Q6AXXZ _003F_003F_7type_info_0040_00406B_0040/* Not supported: data(5E E7 04 10 00 00 00 00) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AXXZ __xc_z/* Not supported: data(00) */;

	public static volatile uint __native_vcclrit_reason/* Not supported: data(FF FF FF FF) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AXXZ __xc_a/* Not supported: data(00) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AHXZ __xi_a/* Not supported: data(00) */;

	public static volatile global::__enative_startup_state __native_startup_state/* Not supported: data() */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AHXZ __xi_z/* Not supported: data(00) */;

	public unsafe static void* __native_startup_lock/*Field data (rva=0xc8e7c) could not be foundin any section!*/;

	public static volatile uint __native_dllmain_reason/* Not supported: data(FF FF FF FF) */;

	internal unsafe static void Marshaler_002ENodeHolder_002E_007Bdtor_007D(NodeHolder* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)num)))((IntPtr)(int)num);
		}
	}

	internal unsafe static void delete(void* P_0, void* P_1)
	{
	}

	internal unsafe static void RGBAMipMappedVolumeMap_002Eresize(RGBAMipMappedVolumeMap* P_0, uint width, uint height, uint depth, uint nMIPMaps)
	{
		if (nMIPMaps == 0)
		{
			uint num = 0u;
			uint num2 = ((width >= height) ? width : height);
			if (depth > num2)
			{
				num2 = depth;
			}
			if (num2 != 0)
			{
				do
				{
					num++;
					num2 >>= 1;
				}
				while (num2 != 0);
			}
			nMIPMaps = num;
		}
		nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E_002Eresize((nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E*)P_0, nMIPMaps);
		uint num3 = width;
		uint num4 = height;
		uint num5 = depth;
		if (0 >= nMIPMaps)
		{
			return;
		}
		int num6 = 0;
		uint num7 = nMIPMaps;
		do
		{
			nvVector_003CnvImage_003Crgba_t_003E_0020_003E* ptr = (nvVector_003CnvImage_003Crgba_t_003E_0020_003E*)(num6 + *(int*)P_0);
			nvVector_003CnvImage_003Crgba_t_003E_0020_003E_002Eresize(ptr, num5);
			if (0 < num5)
			{
				uint new_size = num4 * num3;
				int num8 = 0;
				uint num9 = num5;
				do
				{
					nvImage_003Crgba_t_003E* ptr2 = (nvImage_003Crgba_t_003E*)(*(int*)ptr + num8);
					nvVector_003Crgba_t_003E_002Eresize((nvVector_003Crgba_t_003E*)((byte*)ptr2 + 8), new_size);
					*(uint*)ptr2 = num3;
					((int*)ptr2)[1] = (int)num4;
					num8 += 24;
					num9--;
				}
				while (num9 != 0);
			}
			uint num10 = num3 >> 1;
			num3 = ((num10 == 0) ? 1u : num10);
			uint num11 = num4 >> 1;
			num4 = ((num11 == 0) ? 1u : num11);
			uint num12 = num5 >> 1;
			num5 = ((num12 == 0) ? 1u : num12);
			num6 += 12;
			num7--;
		}
		while (num7 != 0);
	}

	internal unsafe static void RGBAMipMappedVolumeMap_002E_007Bdtor_007D(RGBAMipMappedVolumeMap* P_0)
	{
		nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E_002Eresize((nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E*)P_0, 0u);
		((int*)P_0)[2] = 0;
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			free((void*)(int)num);
			*(int*)P_0 = 0;
		}
	}

	internal unsafe static nvCompressionOptions* nvCompressionOptions_002E_007Bctor_007D(nvCompressionOptions* P_0)
	{
		((sbyte*)P_0)[24] = 0;
		((int*)((byte*)P_0 + 24))[1] = 0;
		((float*)((byte*)P_0 + 24))[2] = 1f;
		((int*)((byte*)P_0 + 24))[3] = 1041;
		((int*)((byte*)P_0 + 24))[4] = 1010;
		((int*)((byte*)P_0 + 24))[5] = 1033;
		((byte*)P_0 + 24)[24] = 0;
		((byte*)P_0 + 24)[25] = 0;
		((byte*)P_0 + 24)[26] = 0;
		((byte*)P_0 + 24)[27] = 0;
		((byte*)P_0 + 24)[28] = 0;
		((byte*)P_0 + 24)[29] = 0;
		nvCompressionOptions_002ESetDefaultOptions(P_0);
		return P_0;
	}

	internal unsafe static void nvCompressionOptions_002ESetDefaultOptions(nvCompressionOptions* P_0)
	{
		*(int*)P_0 = 71;
		((float*)P_0)[1] = 400f;
		((int*)P_0)[15] = 0;
		((int*)P_0)[16] = 4;
		((float*)P_0)[17] = 1f;
		((float*)P_0)[18] = 1f;
		((sbyte*)P_0)[76] = 0;
		((float*)P_0)[20] = 4096f;
		((float*)P_0)[21] = 4096f;
		((sbyte*)P_0)[88] = 0;
		((float*)P_0)[23] = 4096f;
		((float*)P_0)[24] = 4096f;
		((int*)P_0)[25] = 30;
		((int*)P_0)[26] = 0;
		((int*)P_0)[27] = 2;
		((sbyte*)P_0)[112] = 0;
		((sbyte*)P_0)[1633] = 0;
		((sbyte*)P_0)[120] = 0;
		((sbyte*)P_0)[121] = 0;
		((sbyte*)P_0)[122] = 0;
		((sbyte*)P_0)[123] = 0;
		((sbyte*)P_0)[124] = 0;
		((sbyte*)P_0)[125] = 0;
		((float*)P_0)[32] = 0f;
		((float*)P_0)[33] = 0f;
		((float*)P_0)[34] = 0f;
		((float*)P_0)[35] = 0f;
		((sbyte*)P_0)[144] = 0;
		((sbyte*)P_0)[145] = 0;
		((float*)P_0)[37] = 0f;
		((float*)P_0)[38] = 0f;
		((float*)P_0)[39] = 0f;
		((float*)P_0)[40] = 0f;
		((int*)P_0)[41] = 0;
		((float*)P_0)[42] = 0.15f;
		((float*)P_0)[29] = 0.5f;
		((sbyte*)P_0)[1635] = 0;
		((sbyte*)P_0)[1636] = 0;
		((sbyte*)P_0)[1632] = 0;
		((int*)P_0)[110] = 0;
		((sbyte*)P_0)[444] = 0;
		((int*)P_0)[112] = 1;
		((int*)P_0)[2] = 0;
		((sbyte*)P_0)[56] = 0;
		((float*)P_0)[3] = 0.3086f;
		((float*)P_0)[4] = 0.6094f;
		((float*)P_0)[5] = 0.082f;
		((sbyte*)P_0)[452] = 0;
		((float*)P_0)[114] = 2.2f;
		((float*)P_0)[115] = 1f;
		((float*)P_0)[117] = 10f;
		((sbyte*)P_0)[464] = 0;
		((int*)P_0)[118] = 0;
		((int*)P_0)[119] = 0;
		((sbyte*)P_0)[1631] = 0;
		((sbyte*)P_0)[1630] = 0;
		((int*)P_0)[410] = 0;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY144M _0024ArrayType_0024_0024_0024BY144M);
		*(float*)(&_0024ArrayType_0024_0024_0024BY144M) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 4)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 8)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 12)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 16)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 20)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 24)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 28)) = -2f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 32)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 36)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 40)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 44)) = -2f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 48)) = 11f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 52)) = -2f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 56)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 60)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 64)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 68)) = -2f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 72)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 76)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 80)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 84)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 88)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 92)) = 0f;
		System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY144M, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 96)) = 0f;
		int num = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 16));
		int num2 = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 12));
		int num3 = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 8));
		int num4 = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY144M, 4));
		_0024ArrayType_0024_0024_0024BY144M* ptr = &_0024ArrayType_0024_0024_0024BY144M;
		nvCompressionOptions* ptr2 = (nvCompressionOptions*)((byte*)P_0 + 260);
		nvCompressionOptions* ptr3 = (nvCompressionOptions*)((byte*)P_0 + 256);
		nvCompressionOptions* ptr4 = (nvCompressionOptions*)((byte*)P_0 + 252);
		nvCompressionOptions* ptr5 = (nvCompressionOptions*)((byte*)P_0 + 248);
		nvCompressionOptions* ptr6 = (nvCompressionOptions*)((byte*)P_0 + 244);
		uint num5 = 5u;
		do
		{
			*(float*)ptr6 = *(float*)ptr;
			*(float*)ptr5 = *(float*)num4;
			*(float*)ptr4 = *(float*)num3;
			*(float*)ptr3 = *(float*)num2;
			*(float*)ptr2 = *(float*)num;
			ptr = (_0024ArrayType_0024_0024_0024BY144M*)((byte*)ptr + 20);
			ptr6 = (nvCompressionOptions*)((byte*)ptr6 + 20);
			num4 += 20;
			ptr5 = (nvCompressionOptions*)((byte*)ptr5 + 20);
			num3 += 20;
			ptr4 = (nvCompressionOptions*)((byte*)ptr4 + 20);
			num2 += 20;
			ptr3 = (nvCompressionOptions*)((byte*)ptr3 + 20);
			num += 20;
			ptr2 = (nvCompressionOptions*)((byte*)ptr2 + 20);
			num5--;
		}
		while (num5 != 0);
		((float*)P_0)[86] = 3f;
		((float*)P_0)[87] = 0f;
		((float*)P_0)[88] = 5f;
		((float*)P_0)[89] = 0.5f;
		((float*)P_0)[90] = 0f;
		((float*)P_0)[91] = 1f;
		((float*)P_0)[92] = 1f;
		((int*)P_0)[93] = 0;
		nvCompressionOptions* ptr7 = (nvCompressionOptions*)((byte*)P_0 + 376);
		uint num6 = 16u;
		do
		{
			*(int*)ptr7 = 1;
			ptr7 = (nvCompressionOptions*)((byte*)ptr7 + 4);
			num6--;
		}
		while (num6 != 0);
		((sbyte*)P_0)[1638] = 0;
		((sbyte*)P_0)[1637] = 0;
		((sbyte*)P_0)[172] = 0;
		nvCompressionOptions* ptr8 = (nvCompressionOptions*)((byte*)P_0 + 485);
		uint num7 = 256u;
		do
		{
			*((sbyte*)ptr8 - 1) = 0;
			*(sbyte*)ptr8 = 0;
			((sbyte*)ptr8)[1] = 0;
			((sbyte*)ptr8)[2] = 0;
			ptr8 = (nvCompressionOptions*)((byte*)ptr8 + 4);
			num7--;
		}
		while (num7 != 0);
		((int*)P_0)[120] = 0;
		((sbyte*)P_0)[1508] = 0;
		((float*)P_0)[378] = 1f;
		((float*)P_0)[379] = 1f;
		((float*)P_0)[380] = 1f;
		((float*)P_0)[381] = 1f;
		((float*)P_0)[382] = 0f;
		((float*)P_0)[383] = 0f;
		((float*)P_0)[384] = 0f;
		((float*)P_0)[385] = 0f;
		((float*)P_0)[386] = 1f;
		((float*)P_0)[387] = 1f;
		((float*)P_0)[388] = 1f;
		((float*)P_0)[389] = 1f;
		((float*)P_0)[390] = 0f;
		((float*)P_0)[391] = 0f;
		((float*)P_0)[392] = 0f;
		((float*)P_0)[393] = 0f;
		((sbyte*)P_0)[1576] = 0;
		((float*)P_0)[395] = 0.3086f;
		((float*)P_0)[396] = 0.6094f;
		((float*)P_0)[397] = 0.082f;
		((float*)P_0)[398] = 0f;
		((float*)P_0)[399] = 0f;
		((float*)P_0)[400] = 0f;
		((float*)P_0)[401] = 0f;
		((float*)P_0)[402] = 0f;
		((float*)P_0)[403] = 1f;
		((float*)P_0)[404] = 1f;
		((float*)P_0)[405] = 1f;
		((float*)P_0)[406] = 1f;
		((sbyte*)P_0)[1629] = 0;
		((sbyte*)P_0)[1628] = 0;
		((sbyte*)P_0)[1634] = 0;
	}

	internal unsafe static int Endian_002EEndianHelper_002EEndianToPlatform(EndianHelper* P_0, int value)
	{
		if (*(bool*)P_0)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(sbyte*)(&num) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)(&value);
			value = (int)num;
		}
		return value;
	}

	internal unsafe static uint Endian_002EEndianHelper_002EEndianToPlatform(EndianHelper* P_0, uint value)
	{
		if (*(bool*)P_0)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(sbyte*)(&num) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)(&value);
			value = num;
		}
		return value;
	}

	internal unsafe static void Relo_002EChunk_002E_007Bdtor_007D(Relo.Chunk* P_0)
	{
		uint num = ((uint*)P_0)[2];
		if (num != 0)
		{
			free((void*)(int)num);
			((int*)P_0)[2] = 0;
		}
		uint num2 = ((uint*)P_0)[4];
		if (num2 != 0)
		{
			free((void*)(int)num2);
			((int*)P_0)[4] = 0;
		}
		uint num3 = *(uint*)P_0;
		if (num3 != 0)
		{
			free((void*)(int)num3);
			*(int*)P_0 = 0;
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool Relo_002EChunk_002EAllocate(Relo.Chunk* P_0, uint instanceBufferSize, uint relocationBufferSize, uint importsBufferSize)
	{
		((int*)P_0)[3] = (int)relocationBufferSize;
		((int*)P_0)[1] = (int)instanceBufferSize;
		((int*)P_0)[5] = (int)importsBufferSize;
		if (relocationBufferSize != 0)
		{
			((int*)P_0)[2] = (int)malloc(relocationBufferSize);
		}
		uint num = ((uint*)P_0)[5];
		if (num != 0)
		{
			((int*)P_0)[4] = (int)malloc(num);
		}
		*(int*)P_0 = (int)malloc(((uint*)P_0)[1]);
		return true;
	}

	internal unsafe static void* Relo_002ETracker_002EPush(Tracker* P_0, void** pointerLocation, uint objectSize, uint objectCount)
	{
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)P_0 + 4);
		if (((((int*)ptr)[1] - *(int*)ptr) & -4) == 0)
		{
			uint num = Relo_002ETracker_002EAllocate(P_0, objectCount, objectSize);
			stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr2 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)P_0 + 4);
			uint num2 = ((uint*)ptr2)[1];
			if (num2 < (uint)((int*)ptr2)[2])
			{
				int num3 = (int)num2;
				((int*)ptr2)[1] = (int)(num2 + 4);
				if (num3 != 0)
				{
					*(uint*)num3 = num;
				}
			}
			else
			{
				eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E*)ptr2, (uint*)(int)num2, &num);
			}
			return (void*)(*(int*)pointerLocation = *(int*)(int)(num * 12 + (uint)((int*)P_0)[5]));
		}
		uint num4 = *(uint*)(((int*)P_0)[2] - 4);
		uint num5 = Relo_002ETracker_002EAllocate(P_0, objectCount, objectSize);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr3 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)P_0 + 4);
		uint num6 = ((uint*)ptr3)[1];
		if (num6 < (uint)((int*)ptr3)[2])
		{
			int num7 = (int)num6;
			((int*)ptr3)[1] = (int)(num6 + 4);
			if (num7 != 0)
			{
				*(uint*)num7 = num5;
			}
		}
		else
		{
			eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E*)ptr3, (uint*)(int)num6, &num5);
		}
		if (pointerLocation != null)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E obj);
			*(uint*)(&obj) = num4;
			int num8 = ((int*)P_0)[5];
			System.Runtime.CompilerServices.Unsafe.As<Tracker.GenericBookmark_003Cunsigned_0020int_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4)) = (int)((byte*)pointerLocation - *(int*)(int)(num4 * 12 + (uint)num8));
			System.Runtime.CompilerServices.Unsafe.As<Tracker.GenericBookmark_003Cunsigned_0020int_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 8)) = num5;
			*(int*)pointerLocation = *(int*)(int)(num5 * 12 + (uint)num8);
			vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
			uint num9 = ((uint*)ptr4)[1];
			if (num9 < (uint)((int*)ptr4)[2])
			{
				int num10 = (int)num9;
				((int*)ptr4)[1] = (int)(num9 + 12);
				if (num10 != 0)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(num10, ref obj, 12);
				}
			}
			else
			{
				eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(ptr4, (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)num9, &obj);
			}
			return (void*)(int)(*(uint*)pointerLocation);
		}
		return null;
	}

	internal unsafe static void Relo_002ETracker_002E_007Bdtor_007D(Tracker* P_0)
	{
		try
		{
			Tracker* ptr;
			try
			{
				try
				{
					try
					{
						try
						{
							ptr = (Tracker*)((byte*)P_0 + 20);
							while (true)
							{
								vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr;
								if (*(int*)ptr2 == ((int*)ptr2)[1])
								{
									break;
								}
								free((void*)(int)(*(uint*)(((int*)P_0)[6] - 12)));
								vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr;
								((int*)ptr3)[1] += -12;
							}
							uint num = ((uint*)P_0)[21];
							if (num != 0)
							{
								delete_005B_005D((void*)(int)num);
							}
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 68);
							throw;
						}
						vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 68);
						int num2 = *(int*)ptr4;
						if (num2 != 0)
						{
							delete_005B_005D((void*)num2);
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 52);
						throw;
					}
					vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
					int num3 = *(int*)ptr5;
					if (num3 != 0)
					{
						delete_005B_005D((void*)num3);
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 36);
					throw;
				}
				vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
				int num4 = *(int*)ptr6;
				if (num4 != 0)
				{
					delete_005B_005D((void*)num4);
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 20);
				throw;
			}
			vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr;
			int num5 = *(int*)ptr7;
			if (num5 != 0)
			{
				delete_005B_005D((void*)num5);
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*, void>)(&eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
			throw;
		}
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr8 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)P_0 + 4);
		int num6 = *(int*)ptr8;
		if (num6 != 0)
		{
			delete_005B_005D((void*)num6);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool Relo_002ETracker_002EMakeRelocatable(Tracker* P_0, Relo.Chunk* chunk, EndianHelper* endian, uint absoluteInstanceOffset)
	{
		vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
		uint num;
		if ((((int*)ptr)[1] - *(int*)ptr) / 12 != 0)
		{
			vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
			num = (uint)((((int*)ptr2)[1] - *(int*)ptr2) / 12 * 4 + 4);
		}
		else
		{
			num = 0u;
		}
		vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
		uint num2;
		if ((((int*)ptr3)[1] - *(int*)ptr3) / 12 != 0)
		{
			vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
			num2 = (uint)((((int*)ptr4)[1] - *(int*)ptr4) / 12 * 4 + 4);
		}
		else
		{
			num2 = 0u;
		}
		if (!Relo_002EChunk_002EAllocate(chunk, *(uint*)P_0, num2, num))
		{
			return false;
		}
		sbyte* ptr5 = (sbyte*)(int)(*(uint*)chunk);
		sbyte* ptr6 = ptr5;
		uint num3 = ((uint*)P_0)[21];
		if (num3 != 0)
		{
			delete_005B_005D((void*)(int)num3);
		}
		vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 20);
		uint num4 = (uint)((((int*)ptr7)[1] - *(int*)ptr7) / 12);
		uint num5 = ((num4 > 1073741823) ? uint.MaxValue : (num4 << 2));
		((int*)P_0)[21] = (int)new_005B_005D(num5);
		Tracker.Block* ptr8 = (Tracker.Block*)(int)((uint*)P_0)[5];
		if (ptr8 != (Tracker.Block*)((int*)P_0)[6])
		{
			int num6 = 0;
			do
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr6, *(int*)ptr8, ((int*)ptr8)[1]);
				*(int*)(num6 + ((int*)P_0)[21]) = (int)(ptr6 - (nuint)ptr5);
				num6 += 4;
				ptr6 = ((int*)ptr8)[1] + ptr6;
				ptr8 = (Tracker.Block*)((byte*)ptr8 + 12);
			}
			while (ptr8 != (Tracker.Block*)((int*)P_0)[6]);
		}
		if (num2 != 0)
		{
			uint* ptr9 = (uint*)(int)((uint*)chunk)[2];
			eastl_002Equick_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E((Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)((uint*)P_0)[9], (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)((uint*)P_0)[10], (delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte>)__unep_0040_003F_003F_0024BookmarkCompare_0040U_003F_0024GenericBookmark_0040I_0040Tracker_0040Relo_0040_0040_0040Tracker_0040Relo_0040_0040_0024_0024FCA_NABU_003F_0024GenericBookmark_0040I_004001_00400_0040Z);
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr10 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)((uint*)P_0)[9];
			if (ptr10 != (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((int*)P_0)[10])
			{
				do
				{
					uint num7 = (uint)(*(int*)(*(int*)ptr10 * 4 + ((int*)P_0)[21]) + ((int*)ptr10)[1]);
					*ptr9 = Endian_002EEndianHelper_002EEndianToPlatform(endian, num7 + absoluteInstanceOffset);
					uint num8 = *(uint*)(((int*)ptr10)[2] * 4 + ((int*)P_0)[21]);
					*(uint*)((int)num7 + ptr5) = Endian_002EEndianHelper_002EEndianToPlatform(endian, num8 + absoluteInstanceOffset);
					ptr9++;
					ptr10 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr10 + 12);
				}
				while (ptr10 != (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((int*)P_0)[10]);
			}
			*ptr9 = uint.MaxValue;
		}
		if (num != 0)
		{
			uint* ptr11 = (uint*)(int)((uint*)chunk)[4];
			eastl_002Equick_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E((Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)((uint*)P_0)[13], (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)((uint*)P_0)[14], (delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte>)__unep_0040_003F_003F_0024BookmarkCompare_0040U_003F_0024GenericBookmark_0040I_0040Tracker_0040Relo_0040_0040_0040Tracker_0040Relo_0040_0040_0024_0024FCA_NABU_003F_0024GenericBookmark_0040I_004001_00400_0040Z);
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr12 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)((uint*)P_0)[13];
			if (ptr12 != (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((int*)P_0)[14])
			{
				do
				{
					uint num9 = (uint)(*(int*)(*(int*)ptr12 * 4 + ((int*)P_0)[21]) + ((int*)ptr12)[1]);
					*ptr11 = Endian_002EEndianHelper_002EEndianToPlatform(endian, num9 + absoluteInstanceOffset);
					int num10 = ((int*)ptr12)[2];
					*(int*)((int)num9 + ptr5) = Endian_002EEndianHelper_002EEndianToPlatform(endian, num10 + 1);
					ptr11++;
					ptr12 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr12 + 12);
				}
				while (ptr12 != (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((int*)P_0)[14]);
			}
			*ptr11 = uint.MaxValue;
		}
		return true;
	}

	internal unsafe static uint Relo_002ETracker_002EAllocate(Tracker* P_0, uint count, uint elementSize)
	{
		uint num = (count * elementSize + 3) & 0xFFFFFFFCu;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.Block block);
		*(int*)(&block) = (int)calloc(1u, num);
		System.Runtime.CompilerServices.Unsafe.As<Tracker.Block, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref block, 4)) = num;
		System.Runtime.CompilerServices.Unsafe.As<Tracker.Block, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref block, 8)) = elementSize;
		Tracker* ptr = (Tracker*)((byte*)P_0 + 20);
		vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr;
		uint num2 = ((uint*)ptr2)[1];
		if (num2 < (uint)((int*)ptr2)[2])
		{
			int num3 = (int)num2;
			((int*)ptr2)[1] = (int)(num2 + 12);
			if (num3 != 0)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(num3, ref block, 12);
			}
		}
		else
		{
			eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(ptr2, (Tracker.Block*)(int)num2, &block);
		}
		*(int*)P_0 += System.Runtime.CompilerServices.Unsafe.As<Tracker.Block, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref block, 4));
		vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr;
		return (uint)((((int*)ptr3)[1] - *(int*)ptr3) / 12 - 1);
	}

	internal unsafe static void Relo_002ETracker_002EPop(Tracker* P_0)
	{
		((int*)((byte*)P_0 + 4))[1] += -4;
	}

	internal unsafe static void eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D(stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal static void _003FA0x749b4e39_002E_003F_003F__E_003FA0x749b4e39_0040SECONDS_PER_LOGICFRAME_REAL_0040DataInitializers_0040_0040YMXXZ()
	{
		DataInitializers_002E_003FA0x749b4e39_002ESECONDS_PER_LOGICFRAME_REAL = (float)(1.0 / (double)DataInitializers_002E_003FA0x749b4e39_002ELOGICFRAMES_PER_SECONDS_REAL);
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, TerrainTextureCell* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06MJDEEGAJ_0040IndexX_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(sbyte*)@object = (sbyte)strtoul(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06NACPHHEI_0040IndexY_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((sbyte*)@object)[1] = (sbyte)strtoul(ptr4, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(TerrainTextureTileRuntime* @object, EndianHelper* state)
	{
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(sbyte*)(&num) = ((sbyte*)@object)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((sbyte*)@object)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((sbyte*)@object)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)@object;
			*(uint*)@object = num;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num2);
			*(byte*)(&num2) = ((byte*)@object + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((sbyte*)@object)[4];
			((short*)@object)[2] = (short)num2;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num3);
			*(byte*)(&num3) = ((byte*)@object + 6)[1];
			System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)@object)[6];
			((short*)@object)[3] = (short)num3;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num4);
			*(byte*)(&num4) = ((byte*)@object + 8)[1];
			System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = ((sbyte*)@object)[8];
			((short*)@object)[4] = (short)num4;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num5);
			*(byte*)(&num5) = ((byte*)@object + 10)[1];
			System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 1)) = ((sbyte*)@object)[10];
			((short*)@object)[5] = (short)num5;
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, TerrainTextureTile* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09OCLHBPOP_0040TextureID_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(uint*)@object = strtoul(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0M_0040DCHNIHNI_0040BaseTexture_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3), (String_003Cchar_003E*)((byte*)@object + 4), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
			IValue* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0O_0040KKHJNGCH_0040NormalTexture_003F_0024AA_0040), null);
			if (ptr4 != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr4)))((nint)ptr4), (String_003Cchar_003E*)((byte*)@object + 12), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr4 + 4)))((nint)ptr4);
			}
			Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureCell_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04KCPDDLKF_0040Cell_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003ATerrainTextureCell_003E*)((byte*)@object + 20), state);
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, TerrainTextureAtlas* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09JPAAJLPC_0040AtlasSize_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04CGOKOKBC_00402048_003F_0024AA_0040));
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				((short*)@object)[6] = (short)strtoul(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BG_0040ELMBKNGC_0040AllowLossyCompression_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040)), (bool*)@object + 14, state);
			Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureTile_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04OLCHHEMG_0040Tile_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003ATerrainTextureTile_003E*)((byte*)@object + 4), state);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool BinaryAssetBuilder_002ETextureCompiler_002EHasAlphaChannel(_D3DFORMAT format)
	{
		if (format <= (_D3DFORMAT)438304893)
		{
			if (format != (_D3DFORMAT)438304893)
			{
				if (format <= (_D3DFORMAT)405274806)
				{
					if (format == (_D3DFORMAT)405274806 || format == (_D3DFORMAT)405274691 || format == (_D3DFORMAT)405274703 || format == (_D3DFORMAT)405274758)
					{
						goto IL_0149;
					}
				}
				else
				{
					switch (format - 438304851)
					{
					case (_D3DFORMAT)0:
					case (_D3DFORMAT)1:
					case (_D3DFORMAT)7:
					case (_D3DFORMAT)39:
					case (_D3DFORMAT)40:
						goto IL_0149;
					}
				}
				goto IL_0147;
			}
		}
		else if (format <= (_D3DFORMAT)438315702)
		{
			if (format != (_D3DFORMAT)438315702 && format != (_D3DFORMAT)438304902 && format != (_D3DFORMAT)438304931 && format != (_D3DFORMAT)438304950)
			{
				goto IL_0147;
			}
		}
		else if (format != (_D3DFORMAT)438479453 && format != (_D3DFORMAT)438479456 && format != (_D3DFORMAT)438479526)
		{
			goto IL_0147;
		}
		goto IL_0149;
		IL_0147:
		return false;
		IL_0149:
		return true;
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002ELoadImageA(nvImage_003Crgba_t_003E* dstImage, bool* hasAlphaChannel, sbyte* pPath)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out D3DTexture* ptr);
		if (D3DXCreateTextureFromFileA(BinaryAssetBuilder_002ETextureCompiler_002E_003FA0x749b4e39_002Eg_pD3DDevice, pPath, &ptr) < 0)
		{
			object[] args = new object[0];
			throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "D3DX cannot load the texture from disk.", args);
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _XGTEXTURE_DESC xGTEXTURE_DESC);
		XGGetTextureDesc((D3DBaseTexture*)ptr, 0u, &xGTEXTURE_DESC);
		*hasAlphaChannel = BinaryAssetBuilder_002ETextureCompiler_002EHasAlphaChannel((_D3DFORMAT)(System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 16)) & -257));
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _D3DLOCKED_RECT d3DLOCKED_RECT);
		D3DTexture_002ELockRect(ptr, 0u, &d3DLOCKED_RECT, null, 0u);
		uint num = System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 8));
		uint num2 = System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 4));
		nvImage_003Crgba_t_003E* ptr2 = (nvImage_003Crgba_t_003E*)((byte*)dstImage + 8);
		nvVector_003Crgba_t_003E_002Eresize((nvVector_003Crgba_t_003E*)ptr2, (uint)(System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 4)) * System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 8))));
		*(uint*)dstImage = num2;
		((int*)dstImage)[1] = (int)num;
		if (XGCopySurface((void*)(int)(*(uint*)ptr2), System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 4)) << 2, System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 4)), System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 8)), (_D3DFORMAT)438304774, null, (void*)(int)System.Runtime.CompilerServices.Unsafe.As<_D3DLOCKED_RECT, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref d3DLOCKED_RECT, 4)), *(int*)(&d3DLOCKED_RECT), (_D3DFORMAT)(System.Runtime.CompilerServices.Unsafe.As<_XGTEXTURE_DESC, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xGTEXTURE_DESC, 16)) & -193), null, 18u, 0f) < 0)
		{
			object[] args2 = new object[0];
			throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "XGraphics cannot copy the texture from the loaded image to the processing image.", args2);
		}
		D3DTexture_002EUnlockRect(ptr, 0u);
		if (D3DResource_002ERelease((D3DResource*)ptr) != 0)
		{
			object[] args3 = new object[0];
			throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "D3D Cannot release the texture memory.", args3);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool BinaryAssetBuilder_002ETextureCompiler_002EHas1BitAlphaChannel(nvImage_003Crgba_t_003E* image)
	{
		uint num = (uint)(((int*)image)[1] * *(int*)image);
		uint num2 = 0u;
		if (0 < num)
		{
			int num3 = ((int*)image)[2] + 3;
			do
			{
				byte b = *(byte*)num3;
				if (b == 0 || b == byte.MaxValue)
				{
					num2++;
					num3 += 4;
					continue;
				}
				return false;
			}
			while (num2 < num);
		}
		return true;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool BinaryAssetBuilder_002ETextureCompiler_002EHasConstantAlphaChannel(nvImage_003Crgba_t_003E* image)
	{
		uint num = (uint)(((int*)image)[1] * *(int*)image);
		int num2 = ((int*)image)[2];
		rgba_t* ptr = (rgba_t*)num2;
		uint num3 = 0u;
		if (0 < num)
		{
			byte b = ((byte*)ptr)[3];
			int num4 = num2 + 3;
			do
			{
				if (*(byte*)num4 == b)
				{
					num3++;
					num4 += 4;
					continue;
				}
				return false;
			}
			while (num3 < num);
		}
		return true;
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002EZeroAlphaChannel(nvImage_003Crgba_t_003E* image)
	{
		uint num = 0u;
		if (0u >= (uint)(*(int*)image))
		{
			return;
		}
		uint num2 = ((uint*)image)[1];
		do
		{
			uint num3 = 0u;
			if (0 < num2)
			{
				nvImage_003Crgba_t_003E* ptr = (nvImage_003Crgba_t_003E*)((byte*)image + 8);
				do
				{
					*(sbyte*)((*(int*)image * (int)num3 + (int)num) * 4 + *(int*)ptr + 3) = 0;
					num3++;
					num2 = ((uint*)image)[1];
				}
				while (num3 < num2);
			}
			num++;
		}
		while (num < (uint)(*(int*)image));
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002EOneAlphaChannel(nvImage_003Crgba_t_003E* image)
	{
		uint num = 0u;
		if (0u >= (uint)(*(int*)image))
		{
			return;
		}
		uint num2 = ((uint*)image)[1];
		do
		{
			uint num3 = 0u;
			if (0 < num2)
			{
				nvImage_003Crgba_t_003E* ptr = (nvImage_003Crgba_t_003E*)((byte*)image + 8);
				do
				{
					*(sbyte*)((*(int*)image * (int)num3 + (int)num) * 4 + *(int*)ptr + 3) = -1;
					num3++;
					num2 = ((uint*)image)[1];
				}
				while (num3 < num2);
			}
			num++;
		}
		while (num < (uint)(*(int*)image));
	}

	internal unsafe static rgba_t* BinaryAssetBuilder_002ETextureCompiler_002EGetPixel(nvImage_003Crgba_t_003E* image, double u, double v)
	{
		if (u < 0.0)
		{
			double num = u;
			u = num - ceil(num) + 1.0;
		}
		else
		{
			double num2 = u;
			u = num2 - Math.Floor(num2);
		}
		if (v < 0.0)
		{
			double num3 = v;
			v = num3 - ceil(num3) + 1.0;
		}
		else
		{
			double num4 = v;
			v = num4 - Math.Floor(num4);
		}
		uint num5 = *(uint*)image;
		return (rgba_t*)(((int)((double)((uint*)image)[1] * v) * (int)num5 + (int)((double)num5 * u)) * 4 + ((int*)image)[2]);
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002ECreateVolumeRGBAImage(RGBAMipMappedVolumeMap* dstImage, nvImage_003Crgba_t_003E* srcImage)
	{
		uint num = *(uint*)srcImage;
		uint num2 = ((uint*)srcImage)[1];
		uint num3 = num2;
		if (num % num3 != 0)
		{
			object[] args = new object[0];
			throw new BinaryAssetBuilderException(null, ErrorCode.BadTextureFile, "The texture's width is not a direct multiple of the height.  Cannot process it as a 3D texture.", args);
		}
		uint num4 = num2;
		uint num5 = num / num4;
		RGBAMipMappedVolumeMap_002Eresize(dstImage, num4, num4, num5, 1u);
		nvVector_003CnvImage_003Crgba_t_003E_0020_003E* ptr = (nvVector_003CnvImage_003Crgba_t_003E_0020_003E*)(int)(*(uint*)dstImage);
		if (0 >= num5)
		{
			return;
		}
		int num6 = 0;
		uint num7 = 0u;
		uint num8 = num5;
		do
		{
			nvImage_003Crgba_t_003E* ptr2 = (nvImage_003Crgba_t_003E*)(num6 + *(int*)ptr);
			uint num9 = 0u;
			if (0 < num4)
			{
				nvImage_003Crgba_t_003E* ptr3 = (nvImage_003Crgba_t_003E*)((byte*)srcImage + 8);
				nvImage_003Crgba_t_003E* ptr4 = (nvImage_003Crgba_t_003E*)((byte*)ptr2 + 8);
				do
				{
					uint num10 = 0u;
					do
					{
						rgba_t* ptr5 = (rgba_t*)((*(int*)srcImage * (int)num10 + (int)num7 + (int)num9) * 4 + *(int*)ptr3);
						rgba_t* ptr6 = (rgba_t*)((*(int*)ptr2 * (int)num10 + (int)num9) * 4 + *(int*)ptr4);
						*(sbyte*)ptr6 = *(sbyte*)ptr5;
						((sbyte*)ptr6)[1] = ((sbyte*)ptr5)[1];
						((sbyte*)ptr6)[2] = ((sbyte*)ptr5)[2];
						((sbyte*)ptr6)[3] = ((sbyte*)ptr5)[3];
						num10++;
					}
					while (num10 < num4);
					num9++;
				}
				while (num9 < num4);
			}
			num7 = num4 + num7;
			num6 += 24;
			num8--;
		}
		while (num8 != 0);
	}

	internal unsafe static TexturePacker.Texture* BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ETexture_002E_007Bctor_007D(TexturePacker.Texture* P_0)
	{
		*(int*)P_0 = 0;
		((int*)P_0)[1] = 0;
		((int*)P_0)[2] = 0;
		((short*)P_0)[6] = 0;
		((short*)P_0)[7] = 0;
		((short*)P_0)[8] = 0;
		((short*)P_0)[9] = 0;
		return P_0;
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ETexture_002EDeleteSourceTextures(TexturePacker.Texture* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			nvImage_003Crgba_t_003E* ptr = (nvImage_003Crgba_t_003E*)(int)num;
			nvVector_003Crgba_t_003E* ptr2 = (nvVector_003Crgba_t_003E*)((byte*)ptr + 8);
			((int*)ptr2)[1] = 0;
			((int*)ptr2)[2] = 0;
			uint num2 = *(uint*)ptr2;
			if (num2 != 0)
			{
				free((void*)(int)num2);
				*(int*)ptr2 = 0;
			}
			delete(ptr);
			*(int*)P_0 = 0;
		}
		uint num3 = ((uint*)P_0)[1];
		if (num3 != 0)
		{
			nvImage_003Crgba_t_003E* ptr3 = (nvImage_003Crgba_t_003E*)(int)num3;
			nvVector_003Crgba_t_003E* ptr4 = (nvVector_003Crgba_t_003E*)((byte*)ptr3 + 8);
			((int*)ptr4)[1] = 0;
			((int*)ptr4)[2] = 0;
			uint num4 = *(uint*)ptr4;
			if (num4 != 0)
			{
				free((void*)(int)num4);
				*(int*)ptr4 = 0;
			}
			delete(ptr3);
			((int*)P_0)[1] = 0;
		}
	}

	internal unsafe static TexturePacker* BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002E_007Bctor_007D(TexturePacker* P_0, TerrainTextureAtlas* atlas)
	{
		*(int*)P_0 = 0;
		((int*)P_0)[1] = 0;
		TexturePacker* ptr = (TexturePacker*)((byte*)P_0 + 8);
		vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)ptr;
		*(int*)ptr2 = 0;
		((int*)ptr2)[1] = 0;
		((int*)ptr2)[2] = 0;
		try
		{
			((sbyte*)P_0)[24] = 0;
			TerrainTextureAtlas* ptr3 = (TerrainTextureAtlas*)((byte*)atlas + 4);
			eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002Ereserve((vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)ptr, *(uint*)ptr3);
			TerrainTextureTile* ptr4 = (TerrainTextureTile*)(int)((uint*)atlas)[2];
			List_003CSageBinaryData_003A_003ATerrainTextureTile_003E* ptr5 = (List_003CSageBinaryData_003A_003ATerrainTextureTile_003E*)ptr3;
			TerrainTextureTile* ptr6 = (TerrainTextureTile*)(*(int*)ptr5 * 28 + ((int*)ptr5)[1]);
			if (ptr4 != ptr6)
			{
				do
				{
					BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002EAddTexture(P_0, (sbyte*)(int)((uint*)ptr4)[2], (sbyte*)(int)((uint*)ptr4)[4], *(uint*)ptr4);
					ptr4 = (TerrainTextureTile*)((byte*)ptr4 + 28);
				}
				while (ptr4 != ptr6);
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
			throw;
		}
		return P_0;
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002E_007Bdtor_007D(TexturePacker* P_0)
	{
		TexturePacker* ptr;
		try
		{
			uint num = 0u;
			ptr = (TexturePacker*)((byte*)P_0 + 8);
			vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)ptr;
			if (0u < (uint)(((int*)ptr2)[1] - *(int*)ptr2 >> 2))
			{
				do
				{
					int num2 = (int)(num * 4) + *(int*)ptr;
					if (*(int*)num2 != 0)
					{
						TexturePacker.Texture* ptr3 = (TexturePacker.Texture*)(int)(*(uint*)num2);
						if (ptr3 != null)
						{
							BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ETexture_002EDeleteSourceTextures(ptr3);
							((int*)ptr3)[2] = 0;
							((short*)ptr3)[6] = 0;
							((short*)ptr3)[7] = 0;
							((short*)ptr3)[8] = 0;
							((short*)ptr3)[9] = 0;
							delete(ptr3);
						}
					}
					num++;
					ptr2 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)ptr;
				}
				while (num < (uint)(((int*)ptr2)[1] - *(int*)ptr2 >> 2));
			}
			vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)ptr;
			int num3 = ((int*)ptr4)[1];
			TexturePacker.Texture** ptr5 = (TexturePacker.Texture**)num3;
			TexturePacker.Texture** ptr6 = (TexturePacker.Texture**)(int)(*(uint*)ptr4);
			TexturePacker.Texture** ptr7 = (TexturePacker.Texture**)num3;
			memmove(ptr6, ptr5, (uint)((byte*)ptr7 - (nuint)ptr5));
			((int*)ptr4)[1] = (int)(((int*)ptr4)[1] + ((nint)((byte*)ptr5 - (nuint)ptr6) >> 2) * -4);
			uint num4 = *(uint*)P_0;
			if (num4 != 0)
			{
				nvImage_003Crgba_t_003E* ptr8 = (nvImage_003Crgba_t_003E*)(int)num4;
				nvVector_003Crgba_t_003E* ptr9 = (nvVector_003Crgba_t_003E*)((byte*)ptr8 + 8);
				((int*)ptr9)[1] = 0;
				((int*)ptr9)[2] = 0;
				uint num5 = *(uint*)ptr9;
				if (num5 != 0)
				{
					free((void*)(int)num5);
					*(int*)ptr9 = 0;
				}
				delete(ptr8);
				*(int*)P_0 = 0;
			}
			TexturePacker* ptr10 = (TexturePacker*)((byte*)P_0 + 4);
			uint num6 = *(uint*)ptr10;
			if (num6 != 0)
			{
				nvImage_003Crgba_t_003E* ptr11 = (nvImage_003Crgba_t_003E*)(int)num6;
				nvVector_003Crgba_t_003E* ptr12 = (nvVector_003Crgba_t_003E*)((byte*)ptr11 + 8);
				((int*)ptr12)[1] = 0;
				((int*)ptr12)[2] = 0;
				uint num7 = *(uint*)ptr12;
				if (num7 != 0)
				{
					free((void*)(int)num7);
					*(int*)ptr12 = 0;
				}
				delete(ptr11);
				*(int*)ptr10 = 0;
			}
			((sbyte*)P_0)[24] = 0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
			throw;
		}
		vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr13 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)ptr;
		int num8 = *(int*)ptr13;
		if (num8 != 0)
		{
			delete_005B_005D((void*)num8);
		}
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002EAddTexture(TexturePacker* P_0, sbyte* basePath, sbyte* normalPath, uint Id)
	{
		if (((bool*)P_0)[24])
		{
			return;
		}
		string text = new string(basePath);
		string text2 = new string(normalPath);
		if (!File.Exists(text))
		{
			return;
		}
		if (text.ToLower().Contains("tga") && text2.ToLower().Contains("tga"))
		{
			TexturePacker.Texture* ptr = (TexturePacker.Texture*)@new(20u);
			TexturePacker.Texture* ptr2;
			try
			{
				ptr2 = ((ptr == null) ? null : BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ETexture_002E_007Bctor_007D(ptr));
			}
			catch
			{
				//try-fault
				delete(ptr);
				throw;
			}
			TexturePacker.Texture* ptr3 = ptr2;
			((int*)ptr2)[2] = (int)Id;
			nvImage_003Crgba_t_003E* ptr4 = (nvImage_003Crgba_t_003E*)@new(24u);
			nvImage_003Crgba_t_003E* ptr5;
			try
			{
				ptr5 = ((ptr4 == null) ? null : nvImage_003Crgba_t_003E_002E_007Bctor_007D(ptr4));
			}
			catch
			{
				//try-fault
				delete(ptr4);
				throw;
			}
			*(int*)ptr2 = (int)ptr5;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out bool flag);
			BinaryAssetBuilder_002ETextureCompiler_002ELoadImageA(ptr5, &flag, basePath);
			if (!flag)
			{
				BinaryAssetBuilder_002ETextureCompiler_002EZeroAlphaChannel((nvImage_003Crgba_t_003E*)(int)(*(uint*)ptr2));
			}
			if (*(int*)ptr2 != 0)
			{
				int num = *(int*)ptr2;
				if (*(int*)num == 256 && *(int*)(num + 4) == 256)
				{
					if (File.Exists(text2))
					{
						nvImage_003Crgba_t_003E* ptr6 = (nvImage_003Crgba_t_003E*)@new(24u);
						nvImage_003Crgba_t_003E* ptr7;
						try
						{
							ptr7 = ((ptr6 == null) ? null : nvImage_003Crgba_t_003E_002E_007Bctor_007D(ptr6));
						}
						catch
						{
							//try-fault
							delete(ptr6);
							throw;
						}
						((int*)ptr2)[1] = (int)ptr7;
						System.Runtime.CompilerServices.Unsafe.SkipInit(out bool flag2);
						BinaryAssetBuilder_002ETextureCompiler_002ELoadImageA(ptr7, &flag2, normalPath);
						BinaryAssetBuilder_002ETextureCompiler_002EOneAlphaChannel((nvImage_003Crgba_t_003E*)(int)((uint*)ptr2)[1]);
						((sbyte*)P_0)[25] = 1;
					}
					eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002Epush_back((vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8), &ptr3);
					return;
				}
			}
			object[] args = new object[0];
			throw new BinaryAssetBuilderException(ErrorCode.BadTextureFile, "The terrain texture must be 256x256 in size!", args);
		}
		object[] args2 = new object[0];
		throw new BinaryAssetBuilderException(ErrorCode.BadTextureFile, "Terrain textures must be targas!", args2);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ETextureSort(TexturePacker.Texture* texA, TexturePacker.Texture* texB)
	{
		return (uint)((int*)texA)[2] < (uint)((int*)texB)[2];
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002EPackTextures(TexturePacker* P_0)
	{
		((sbyte*)P_0)[24] = 1;
		vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		uint num = (uint)(((int*)ptr)[1] - *(int*)ptr >> 2);
		ushort width;
		ushort height;
		switch (num)
		{
		default:
		{
			object[] args = new object[0];
			throw new BinaryAssetBuilderException(ErrorCode.BadTextureFile, $"Maximum number of textures exceeded ({num}). Limit is 49.", args);
		}
		case 0u:
		case 1u:
			width = 512;
			height = 512;
			break;
		case 2u:
		case 3u:
			width = 1024;
			height = 512;
			break;
		case 4u:
		case 5u:
		case 6u:
		case 7u:
			width = 2048;
			height = 512;
			break;
		case 8u:
		case 9u:
			width = 1024;
			height = 1024;
			break;
		case 10u:
		case 11u:
		case 12u:
		case 13u:
		case 14u:
		case 15u:
		case 16u:
		case 17u:
		case 18u:
		case 19u:
		case 20u:
		case 21u:
			width = 2048;
			height = 1024;
			break;
		case 22u:
		case 23u:
		case 24u:
		case 25u:
		case 26u:
		case 27u:
		case 28u:
		case 29u:
		case 30u:
		case 31u:
		case 32u:
		case 33u:
		case 34u:
		case 35u:
		case 36u:
		case 37u:
		case 38u:
		case 39u:
		case 40u:
		case 41u:
		case 42u:
		case 43u:
		case 44u:
		case 45u:
		case 46u:
		case 47u:
		case 48u:
		case 49u:
			width = 2048;
			height = 2048;
			break;
		}
		BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002EMergeTextures(P_0, width, height);
		eastl_002Equick_sort_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E((TexturePacker.Texture**)(int)((uint*)P_0)[2], (TexturePacker.Texture**)(int)((uint*)P_0)[3], (delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte>)__unep_0040_003FTextureSort_0040TexturePacker_0040TextureCompiler_0040BinaryAssetBuilder_0040_0040_0024_0024FCA_NPAUTexture_0040123_00400_0040Z);
		uint num2 = 0u;
		vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		if (0u < (uint)(((int*)ptr2)[1] - *(int*)ptr2 >> 2))
		{
			ptr2 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
			vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr2 + 4);
			do
			{
				BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ETexture_002EDeleteSourceTextures((TexturePacker.Texture*)(int)(*(uint*)(int)(num2 * 4 + (uint)((int*)P_0)[2])));
				num2++;
			}
			while (num2 < (uint)(*(int*)ptr3 - *(int*)ptr2 >> 2));
		}
		return true;
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ECopyTexture(TexturePacker* P_0, nvImage_003Crgba_t_003E* dstImage, nvImage_003Crgba_t_003E* srcImage, ushort offsetX, uint offsetY)
	{
		int num = offsetX - 16;
		int num2 = (int)(offsetY - 16);
		int num3 = *(int*)srcImage;
		int num4 = ((int*)srcImage)[1];
		int num5 = num3 + 32;
		int num6 = num4 + 32;
		int num7 = 0;
		if (0 >= num5)
		{
			return;
		}
		do
		{
			int num8 = 0;
			if (0 < num6)
			{
				nvImage_003Crgba_t_003E* ptr = (nvImage_003Crgba_t_003E*)((byte*)dstImage + 8);
				double num9 = num4;
				double u = (double)(num7 - 16) / (double)num3;
				do
				{
					rgba_t* ptr2 = (rgba_t*)(((num8 + num2) * *(int*)dstImage + num7 + num) * 4 + *(int*)ptr);
					rgba_t* ptr3 = BinaryAssetBuilder_002ETextureCompiler_002EGetPixel(srcImage, u, (double)(num8 - 16) / num9);
					*(sbyte*)ptr2 = *(sbyte*)ptr3;
					((sbyte*)ptr2)[1] = ((sbyte*)ptr3)[1];
					((sbyte*)ptr2)[2] = ((sbyte*)ptr3)[2];
					((sbyte*)ptr2)[3] = ((sbyte*)ptr3)[3];
					num8++;
				}
				while (num8 < num6);
			}
			num7++;
		}
		while (num7 < num5);
	}

	internal unsafe static void BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002EMergeTextures(TexturePacker* P_0, ushort width, ushort height)
	{
		nvImage_003Crgba_t_003E* ptr = (nvImage_003Crgba_t_003E*)@new(24u);
		nvImage_003Crgba_t_003E* ptr4;
		try
		{
			if (ptr != null)
			{
				nvImage_003Crgba_t_003E* ptr2 = (nvImage_003Crgba_t_003E*)((byte*)ptr + 8);
				nvVector_003Crgba_t_003E* ptr3 = (nvVector_003Crgba_t_003E*)ptr2;
				*(int*)ptr3 = 0;
				((int*)ptr3)[1] = 0;
				((int*)ptr3)[2] = 0;
				try
				{
					*(int*)ptr = 0;
					((int*)ptr)[1] = 0;
					((sbyte*)ptr)[20] = 0;
					((int*)ptr2)[1] = 0;
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<nvVector_003Crgba_t_003E*, void>)(&nvVector_003Crgba_t_003E_002E_007Bdtor_007D), (byte*)ptr + 8);
					throw;
				}
				ptr4 = ptr;
			}
			else
			{
				ptr4 = null;
			}
		}
		catch
		{
			//try-fault
			delete(ptr);
			throw;
		}
		*(int*)P_0 = (int)ptr4;
		nvVector_003Crgba_t_003E_002Eresize((nvVector_003Crgba_t_003E*)((byte*)ptr4 + 8), (uint)(height * width));
		*(int*)ptr4 = width;
		((int*)ptr4)[1] = height;
		nvImage_003Crgba_t_003E_002ESetToZero((nvImage_003Crgba_t_003E*)(int)(*(uint*)P_0));
		if (((bool*)P_0)[25])
		{
			nvImage_003Crgba_t_003E* ptr5 = (nvImage_003Crgba_t_003E*)@new(24u);
			nvImage_003Crgba_t_003E* ptr6;
			try
			{
				ptr6 = ((ptr5 == null) ? null : nvImage_003Crgba_t_003E_002E_007Bctor_007D(ptr5));
			}
			catch
			{
				//try-fault
				delete(ptr5);
				throw;
			}
			((int*)P_0)[1] = (int)ptr6;
			nvVector_003Crgba_t_003E_002Eresize((nvVector_003Crgba_t_003E*)((byte*)ptr6 + 8), (uint)(width * height));
			*(int*)ptr6 = width;
			((int*)ptr6)[1] = height;
			nvImage_003Crgba_t_003E_002ESetToZero((nvImage_003Crgba_t_003E*)(int)((uint*)P_0)[1]);
		}
		ushort num = 16;
		ushort num2 = 16;
		ushort num3 = 0;
		vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		if (0u >= (uint)(((int*)ptr7)[1] - *(int*)ptr7 >> 2))
		{
			return;
		}
		do
		{
			TexturePacker.Texture* ptr8 = (TexturePacker.Texture*)(int)(*(uint*)(num3 * 4 + ((int*)P_0)[2]));
			((short*)ptr8)[6] = (short)num;
			((short*)ptr8)[7] = (short)num2;
			int num4 = *(int*)ptr8;
			((short*)ptr8)[8] = (short)(num + *(int*)num4);
			((short*)ptr8)[9] = (short)(*(int*)(num4 + 4) + num2);
			BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ECopyTexture(P_0, (nvImage_003Crgba_t_003E*)(int)(*(uint*)P_0), (nvImage_003Crgba_t_003E*)num4, num, num2);
			if (((bool*)P_0)[25])
			{
				uint num5 = ((uint*)ptr8)[1];
				if (num5 != 0)
				{
					BinaryAssetBuilder_002ETextureCompiler_002ETexturePacker_002ECopyTexture(P_0, (nvImage_003Crgba_t_003E*)(int)((uint*)P_0)[1], (nvImage_003Crgba_t_003E*)(int)num5, num, num2);
				}
			}
			num += 288;
			int num6 = *(int*)P_0;
			if (num + 288 > (ushort)(*(int*)num6))
			{
				num = 16;
				num2 += 288;
				if (num2 + 288 > (ushort)(*(int*)(num6 + 4)))
				{
					break;
				}
			}
			num3++;
			ptr7 = (vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		}
		while ((uint)num3 < (uint)(((int*)ptr7)[1] - *(int*)ptr7 >> 2));
	}

	internal unsafe static NV_ERROR_CODE BinaryAssetBuilder_002ETextureCompiler_002EnvDXTWriteCallback(void* buffer, uint count, MIPMapData* mipMapData, void* userData)
	{
		BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002EAppend((SimpleBuffer*)userData, buffer, count);
		return (NV_ERROR_CODE)0;
	}

	[DebuggerStepThrough]
	internal unsafe static void gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bdtor_007D(gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}

	[SpecialName]
	internal unsafe static gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bctor_007D(gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* P_0, gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* r)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)r));
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
		return P_0;
	}

	[DebuggerStepThrough]
	internal unsafe static void gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D(gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}

	internal unsafe static void nvVector_003CnvImage_003Crgba_t_003E_0020_003E_002Eresize(nvVector_003CnvImage_003Crgba_t_003E_0020_003E* P_0, uint new_size)
	{
		nvVector_003CnvImage_003Crgba_t_003E_0020_003E* ptr = (nvVector_003CnvImage_003Crgba_t_003E_0020_003E*)((byte*)P_0 + 4);
		uint num = *(uint*)ptr;
		*(uint*)ptr = new_size;
		if (new_size < num)
		{
			int num2 = (int)(new_size * 24);
			uint num3 = num - new_size;
			do
			{
				nvImage_003Crgba_t_003E* ptr2 = (nvImage_003Crgba_t_003E*)(num2 + *(int*)P_0);
				nvVector_003Crgba_t_003E* ptr3 = (nvVector_003Crgba_t_003E*)((byte*)ptr2 + 8);
				((int*)ptr3)[1] = 0;
				((int*)ptr3)[2] = 0;
				uint num4 = *(uint*)ptr3;
				if (num4 != 0)
				{
					free((void*)(int)num4);
					*(int*)ptr3 = 0;
				}
				num2 += 24;
				num3--;
			}
			while (num3 != 0);
		}
		uint num5 = *(uint*)ptr;
		if (num5 != 0)
		{
			nvVector_003CnvImage_003Crgba_t_003E_0020_003E* ptr4 = (nvVector_003CnvImage_003Crgba_t_003E_0020_003E*)((byte*)P_0 + 8);
			uint num6 = *(uint*)ptr4;
			if (num5 > num6 || num5 <= num6 >> 1)
			{
				uint num7 = (*(uint*)ptr4 = ((num6 != 0) ? ((num5 >> 2) + num5) : num5));
				if (num7 == 0)
				{
					int num8 = *(int*)P_0;
					if (num8 != 0)
					{
						free((void*)num8);
						*(int*)P_0 = 0;
					}
				}
				else
				{
					int num9 = *(int*)P_0;
					if (num9 != 0)
					{
						*(int*)P_0 = (int)realloc((void*)num9, num7 * 24);
					}
					else
					{
						*(int*)P_0 = (int)malloc(num7 * 24);
					}
				}
			}
		}
		uint num10 = num;
		if (num >= new_size)
		{
			return;
		}
		do
		{
			nvImage_003Crgba_t_003E* ptr5 = (nvImage_003Crgba_t_003E*)(int)(num10 * 24 + (uint)(*(int*)P_0));
			try
			{
				if (ptr5 != null)
				{
					nvImage_003Crgba_t_003E* ptr6 = (nvImage_003Crgba_t_003E*)((byte*)ptr5 + 8);
					nvVector_003Crgba_t_003E* ptr7 = (nvVector_003Crgba_t_003E*)ptr6;
					*(int*)ptr7 = 0;
					((int*)ptr7)[1] = 0;
					((int*)ptr7)[2] = 0;
					try
					{
						*(int*)ptr5 = 0;
						((int*)ptr5)[1] = 0;
						((sbyte*)ptr5)[20] = 0;
						((int*)ptr6)[1] = 0;
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<nvVector_003Crgba_t_003E*, void>)(&nvVector_003Crgba_t_003E_002E_007Bdtor_007D), (byte*)ptr5 + 8);
						throw;
					}
				}
			}
			catch
			{
				//try-fault
				delete(ptr5, (void*)(int)((uint)(*(int*)P_0) + num10 * 24));
				throw;
			}
			num10++;
		}
		while (num10 < new_size);
	}

	internal unsafe static nvImage_003Crgba_t_003E* nvImage_003Crgba_t_003E_002E_007Bctor_007D(nvImage_003Crgba_t_003E* P_0)
	{
		nvImage_003Crgba_t_003E* ptr = (nvImage_003Crgba_t_003E*)((byte*)P_0 + 8);
		nvVector_003Crgba_t_003E* ptr2 = (nvVector_003Crgba_t_003E*)ptr;
		*(int*)ptr2 = 0;
		((int*)ptr2)[1] = 0;
		((int*)ptr2)[2] = 0;
		try
		{
			*(int*)P_0 = 0;
			((int*)P_0)[1] = 0;
			((sbyte*)P_0)[20] = 0;
			((int*)ptr)[1] = 0;
			return P_0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<nvVector_003Crgba_t_003E*, void>)(&nvVector_003Crgba_t_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
			throw;
		}
	}

	internal unsafe static void nvImage_003Crgba_t_003E_002E_007Bdtor_007D(nvImage_003Crgba_t_003E* P_0)
	{
		nvVector_003Crgba_t_003E* ptr = (nvVector_003Crgba_t_003E*)((byte*)P_0 + 8);
		((int*)ptr)[1] = 0;
		((int*)ptr)[2] = 0;
		uint num = *(uint*)ptr;
		if (num != 0)
		{
			free((void*)(int)num);
			*(int*)ptr = 0;
		}
	}

	internal unsafe static void nvImage_003Crgba_t_003E_002ESetToZero(nvImage_003Crgba_t_003E* P_0)
	{
		uint num = 0u;
		if (0u < (uint)(*(int*)P_0 * ((int*)P_0)[1]))
		{
			nvImage_003Crgba_t_003E* ptr = (nvImage_003Crgba_t_003E*)((byte*)P_0 + 8);
			do
			{
				rgba_t* ptr2 = (rgba_t*)(int)(num * 4 + (uint)(*(int*)ptr));
				((sbyte*)ptr2)[3] = 0;
				((sbyte*)ptr2)[2] = 0;
				((sbyte*)ptr2)[1] = 0;
				*(sbyte*)ptr2 = 0;
				num++;
			}
			while (num < (uint)(*(int*)P_0 * ((int*)P_0)[1]));
		}
	}

	internal unsafe static void nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E_002Eresize(nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E* P_0, uint new_size)
	{
		nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E* ptr = (nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E*)((byte*)P_0 + 4);
		uint num = *(uint*)ptr;
		*(uint*)ptr = new_size;
		if (new_size < num)
		{
			int num2 = (int)(new_size * 12);
			uint num3 = num - new_size;
			do
			{
				nvVector_003CnvImage_003Crgba_t_003E_0020_003E* ptr2 = (nvVector_003CnvImage_003Crgba_t_003E_0020_003E*)(num2 + *(int*)P_0);
				nvVector_003CnvImage_003Crgba_t_003E_0020_003E_002Eresize(ptr2, 0u);
				((int*)ptr2)[2] = 0;
				uint num4 = *(uint*)ptr2;
				if (num4 != 0)
				{
					free((void*)(int)num4);
					*(int*)ptr2 = 0;
				}
				num2 += 12;
				num3--;
			}
			while (num3 != 0);
		}
		uint num5 = *(uint*)ptr;
		if (num5 != 0)
		{
			nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E* ptr3 = (nvVector_003CnvVector_003CnvImage_003Crgba_t_003E_0020_003E_0020_003E*)((byte*)P_0 + 8);
			uint num6 = *(uint*)ptr3;
			if (num5 > num6 || num5 <= num6 >> 1)
			{
				uint num7 = (*(uint*)ptr3 = ((num6 != 0) ? ((num5 >> 2) + num5) : num5));
				if (num7 == 0)
				{
					int num8 = *(int*)P_0;
					if (num8 != 0)
					{
						free((void*)num8);
						*(int*)P_0 = 0;
					}
				}
				else
				{
					int num9 = *(int*)P_0;
					if (num9 != 0)
					{
						*(int*)P_0 = (int)realloc((void*)num9, num7 * 12);
					}
					else
					{
						*(int*)P_0 = (int)malloc(num7 * 12);
					}
				}
			}
		}
		uint num10 = num;
		if (num >= new_size)
		{
			return;
		}
		do
		{
			nvVector_003CnvImage_003Crgba_t_003E_0020_003E* ptr4 = (nvVector_003CnvImage_003Crgba_t_003E_0020_003E*)(int)(num10 * 12 + (uint)(*(int*)P_0));
			try
			{
				if (ptr4 != null)
				{
					*(int*)ptr4 = 0;
					((int*)ptr4)[1] = 0;
					((int*)ptr4)[2] = 0;
				}
			}
			catch
			{
				//try-fault
				delete(ptr4, (void*)(int)((uint)(*(int*)P_0) + num10 * 12));
				throw;
			}
			num10++;
		}
		while (num10 < new_size);
	}

	[SpecialName]
	internal unsafe static gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bctor_007D(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* P_0, gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* r)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)r));
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
		return P_0;
	}

	[DebuggerStepThrough]
	internal unsafe static void gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_007Bdtor_007D(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}

	internal unsafe static InstanceDeclaration gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E_002E_002EP_0024AAVInstanceDeclaration_0040Core_0040BinaryAssetBuilder_0040_0040(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AInstanceDeclaration_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		return (InstanceDeclaration)((GCHandle)intPtr).Target;
	}

	[DebuggerStepThrough]
	internal unsafe static void gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E_002E_007Bdtor_007D(gcroot_003CBinaryAssetBuilder_003A_003ACore_003A_003AAssetBuffer_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}

	internal unsafe static void nvVector_003Crgba_t_003E_002E_007Bdtor_007D(nvVector_003Crgba_t_003E* P_0)
	{
		((int*)P_0)[1] = 0;
		((int*)P_0)[2] = 0;
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			free((void*)(int)num);
			*(int*)P_0 = 0;
		}
	}

	internal unsafe static void nvVector_003Crgba_t_003E_002Eresize(nvVector_003Crgba_t_003E* P_0, uint new_size)
	{
		nvVector_003Crgba_t_003E* ptr = (nvVector_003Crgba_t_003E*)((byte*)P_0 + 4);
		uint num = *(uint*)ptr;
		*(uint*)ptr = new_size;
		if (new_size != 0)
		{
			nvVector_003Crgba_t_003E* ptr2 = (nvVector_003Crgba_t_003E*)((byte*)P_0 + 8);
			uint num2 = *(uint*)ptr2;
			if (new_size > num2 || new_size <= num2 >> 1)
			{
				uint num3 = (*(uint*)ptr2 = ((num2 != 0) ? ((new_size >> 2) + new_size) : new_size));
				if (num3 == 0)
				{
					uint num4 = *(uint*)P_0;
					if (num4 != 0)
					{
						free((void*)(int)num4);
						*(int*)P_0 = 0;
					}
				}
				else
				{
					uint num5 = *(uint*)P_0;
					if (num5 != 0)
					{
						*(int*)P_0 = (int)realloc((void*)(int)num5, num3 << 2);
					}
					else
					{
						*(int*)P_0 = (int)malloc(num3 << 2);
					}
				}
			}
		}
		uint num6 = num;
		if (num < new_size)
		{
			do
			{
				rgba_t* ptr3 = (rgba_t*)(int)(num6 * 4 + (uint)(*(int*)P_0));
				num6++;
			}
			while (num6 < new_size);
		}
	}

	internal unsafe static void eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static void eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static void eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool Relo_002ETracker_002EBookmarkCompare_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* left, Tracker.GenericBookmark_003Cunsigned_0020int_003E* right)
	{
		uint num = *(uint*)left;
		uint num2 = *(uint*)right;
		if (num < num2)
		{
			return true;
		}
		if (num > num2)
		{
			return false;
		}
		return (uint)((int*)left)[2] < (uint)((int*)right)[2];
	}

	internal unsafe static EnumNameGetter_003Cint_003E.EnumPair* EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(sbyte* at, EnumNameGetter_003Cint_003E.EnumPair* app, uint an)
	{
		uint num = 0u;
		uint num2 = an;
		if (0 < an)
		{
			sbyte b = *at;
			do
			{
				uint num3 = num2 + num >> 1;
				int num4 = *(int*)((int)(num3 * 8) + (byte*)app);
				int num5 = num4;
				int num6 = 0;
				sbyte b2 = b;
				sbyte b3 = *(sbyte*)num5;
				if (b2 < b3)
				{
					goto IL_004c;
				}
				sbyte* ptr = at - num4;
				while (true)
				{
					if (b2 <= b3)
					{
						if (b2 == 0)
						{
							break;
						}
						num5++;
						b2 = ptr[num5];
						b3 = *(sbyte*)num5;
						if (b2 >= b3)
						{
							continue;
						}
						goto IL_004c;
					}
					num6 = 1;
					break;
				}
				goto IL_0054;
				IL_004c:
				num6 = -1;
				goto IL_0054;
				IL_0054:
				int num7 = num6;
				if (num7 != 0)
				{
					if (num7 > 0)
					{
						num = num3 + 1;
					}
					else
					{
						num2 = num3;
					}
					continue;
				}
				return (EnumNameGetter_003Cint_003E.EnumPair*)((int)(num3 * 8) + (byte*)app);
			}
			while (num < num2);
		}
		return null;
	}

	internal unsafe static void Marshaler_002EMarshal_003Cchar_003E(sbyte* text, String_003Cchar_003E* @string, Tracker* tracker)
	{
		sbyte* ptr = text;
		if (*text != 0)
		{
			do
			{
				ptr++;
			}
			while (*ptr != 0);
		}
		uint num = (uint)(*(int*)@string = (int)(ptr - (nuint)text));
		String_003Cchar_003E* ptr2 = (String_003Cchar_003E*)((byte*)@string + 4);
		Relo_002ETracker_002EPush(tracker, (void**)ptr2, 1u, num + 1);
		sbyte* ptr3 = (sbyte*)(int)(*(uint*)ptr2);
		sbyte b;
		do
		{
			*ptr3 = *text;
			text++;
			b = *ptr3;
			ptr3++;
		}
		while (b != 0);
		((int*)((byte*)tracker + 4))[1] += -4;
	}

	internal unsafe static sbyte* Relo_002ETracker_002EPush_003Cchar_003E(Tracker* P_0, sbyte** pointerLocation, uint objectCount)
	{
		return (sbyte*)Relo_002ETracker_002EPush(P_0, (void**)pointerLocation, 1u, objectCount);
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureCell_003E(IList* list, List_003CSageBinaryData_003A_003ATerrainTextureCell_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 2u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num3), (TerrainTextureCell*)(int)(num2 * 2 + (uint)((int*)target)[1]), tracker);
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
						throw;
					}
					if (*(int*)(&nodeHolder) != 0)
					{
						((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder)))))((IntPtr)(*(int*)(&nodeHolder)));
					}
					num2++;
				}
				while (num2 < num);
			}
			((int*)((byte*)tracker + 4))[1] += -4;
		}
		((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)list + 12)))((nint)list);
	}

	internal unsafe static void Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, bool* @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte b = *((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			int num = ((b == 49 || b == 116 || b == 84) ? 1 : 0);
			*@object = (byte)num != 0;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureTile_003E(IList* list, List_003CSageBinaryData_003A_003ATerrainTextureTile_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 28u, num);
			if (0 < num)
			{
				int num2 = 0;
				uint num3 = num;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num4 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num4), (TerrainTextureTile*)(num2 + ((int*)target)[1]), tracker);
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder);
						throw;
					}
					if (*(int*)(&nodeHolder) != 0)
					{
						((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder)))))((IntPtr)(*(int*)(&nodeHolder)));
					}
					num2 += 28;
					num3--;
				}
				while (num3 != 0);
			}
			((int*)((byte*)tracker + 4))[1] += -4;
		}
		((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)list + 12)))((nint)list);
	}

	internal unsafe static void eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static void eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002Ereserve(vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* P_0, uint n)
	{
		int num = *(int*)P_0;
		if (n > (uint)(((int*)P_0)[2] - num >> 2))
		{
			TexturePacker.Texture** ptr = (TexturePacker.Texture**)(int)((uint*)P_0)[1];
			TexturePacker.Texture** ptr2 = (TexturePacker.Texture**)num;
			TexturePacker.Texture** ptr3 = (TexturePacker.Texture**)((n == 0) ? null : new_005B_005D(n << 2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209));
			memmove(ptr3, ptr2, (uint)((byte*)ptr - (nuint)ptr2));
			TexturePacker.Texture** ptr4 = (TexturePacker.Texture**)(int)(*(uint*)P_0);
			if (ptr4 != null)
			{
				delete_005B_005D(ptr4);
			}
			int num2 = ((int*)P_0)[1] - *(int*)P_0 >> 2;
			*(int*)P_0 = (int)ptr3;
			((int*)P_0)[1] = (int)(num2 * 4 + (byte*)ptr3);
			((int*)P_0)[2] = (int)((int)(n * 4) + (byte*)ptr3);
		}
	}

	internal unsafe static void eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002Epush_back(vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* P_0, TexturePacker.Texture** value)
	{
		uint num = ((uint*)P_0)[1];
		if (num < (uint)((int*)P_0)[2])
		{
			int num2 = (int)num;
			((int*)P_0)[1] = (int)(num + 4);
			if (num2 != 0)
			{
				*(int*)num2 = *(int*)value;
			}
		}
		else
		{
			eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(P_0, (TexturePacker.Texture**)(int)num, value);
		}
	}

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003ATexture_003E(Tracker* P_0, Texture** rootPointer)
	{
		*(int*)P_0 = 0;
		Tracker* ptr = (Tracker*)((byte*)P_0 + 4);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr2 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
		*(int*)ptr2 = 0;
		((int*)ptr2)[1] = 0;
		((int*)ptr2)[2] = 0;
		try
		{
			Tracker* ptr3 = (Tracker*)((byte*)P_0 + 20);
			vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr3;
			*(int*)ptr4 = 0;
			((int*)ptr4)[1] = 0;
			((int*)ptr4)[2] = 0;
			try
			{
				vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
				*(int*)ptr5 = 0;
				((int*)ptr5)[1] = 0;
				((int*)ptr5)[2] = 0;
				try
				{
					vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
					*(int*)ptr6 = 0;
					((int*)ptr6)[1] = 0;
					((int*)ptr6)[2] = 0;
					try
					{
						vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 68);
						*(int*)ptr7 = 0;
						((int*)ptr7)[1] = 0;
						((int*)ptr7)[2] = 0;
						try
						{
							((int*)P_0)[21] = 0;
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 12u);
							stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr8 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
							uint num2 = ((uint*)ptr8)[1];
							if (num2 < (uint)((int*)ptr8)[2])
							{
								int num3 = (int)num2;
								((int*)ptr8)[1] = (int)(num2 + 4);
								if (num3 != 0)
								{
									*(uint*)num3 = num;
								}
							}
							else
							{
								eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E*)ptr8, (uint*)(int)num2, &num);
							}
							*(int*)rootPointer = *(int*)(int)(num * 12 + (uint)(*(int*)ptr3));
							return P_0;
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 68);
							throw;
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 52);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 36);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 20);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*, void>)(&eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
			throw;
		}
	}

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AOnDemandTexture_003E(Tracker* P_0, OnDemandTexture** rootPointer)
	{
		*(int*)P_0 = 0;
		Tracker* ptr = (Tracker*)((byte*)P_0 + 4);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr2 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
		*(int*)ptr2 = 0;
		((int*)ptr2)[1] = 0;
		((int*)ptr2)[2] = 0;
		try
		{
			Tracker* ptr3 = (Tracker*)((byte*)P_0 + 20);
			vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr3;
			*(int*)ptr4 = 0;
			((int*)ptr4)[1] = 0;
			((int*)ptr4)[2] = 0;
			try
			{
				vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
				*(int*)ptr5 = 0;
				((int*)ptr5)[1] = 0;
				((int*)ptr5)[2] = 0;
				try
				{
					vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
					*(int*)ptr6 = 0;
					((int*)ptr6)[1] = 0;
					((int*)ptr6)[2] = 0;
					try
					{
						vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 68);
						*(int*)ptr7 = 0;
						((int*)ptr7)[1] = 0;
						((int*)ptr7)[2] = 0;
						try
						{
							((int*)P_0)[21] = 0;
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 8u);
							stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr8 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
							uint num2 = ((uint*)ptr8)[1];
							if (num2 < (uint)((int*)ptr8)[2])
							{
								int num3 = (int)num2;
								((int*)ptr8)[1] = (int)(num2 + 4);
								if (num3 != 0)
								{
									*(uint*)num3 = num;
								}
							}
							else
							{
								eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E*)ptr8, (uint*)(int)num2, &num);
							}
							*(int*)rootPointer = *(int*)(int)(num * 12 + (uint)(*(int*)ptr3));
							return P_0;
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 68);
							throw;
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 52);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 36);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 20);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*, void>)(&eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
			throw;
		}
	}

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020TextureFileData_003E(Tracker* P_0, TextureFileData** rootPointer)
	{
		*(int*)P_0 = 0;
		Tracker* ptr = (Tracker*)((byte*)P_0 + 4);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr2 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
		*(int*)ptr2 = 0;
		((int*)ptr2)[1] = 0;
		((int*)ptr2)[2] = 0;
		try
		{
			Tracker* ptr3 = (Tracker*)((byte*)P_0 + 20);
			vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr3;
			*(int*)ptr4 = 0;
			((int*)ptr4)[1] = 0;
			((int*)ptr4)[2] = 0;
			try
			{
				vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
				*(int*)ptr5 = 0;
				((int*)ptr5)[1] = 0;
				((int*)ptr5)[2] = 0;
				try
				{
					vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
					*(int*)ptr6 = 0;
					((int*)ptr6)[1] = 0;
					((int*)ptr6)[2] = 0;
					try
					{
						vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 68);
						*(int*)ptr7 = 0;
						((int*)ptr7)[1] = 0;
						((int*)ptr7)[2] = 0;
						try
						{
							((int*)P_0)[21] = 0;
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 8u);
							stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr8 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
							uint num2 = ((uint*)ptr8)[1];
							if (num2 < (uint)((int*)ptr8)[2])
							{
								int num3 = (int)num2;
								((int*)ptr8)[1] = (int)(num2 + 4);
								if (num3 != 0)
								{
									*(uint*)num3 = num;
								}
							}
							else
							{
								eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E*)ptr8, (uint*)(int)num2, &num);
							}
							*(int*)rootPointer = *(int*)(int)(num * 12 + (uint)(*(int*)ptr3));
							return P_0;
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 68);
							throw;
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 52);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 36);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 20);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*, void>)(&eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
			throw;
		}
	}

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureAtlas_003E(Tracker* P_0, TerrainTextureAtlas** rootPointer)
	{
		*(int*)P_0 = 0;
		Tracker* ptr = (Tracker*)((byte*)P_0 + 4);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr2 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
		*(int*)ptr2 = 0;
		((int*)ptr2)[1] = 0;
		((int*)ptr2)[2] = 0;
		try
		{
			Tracker* ptr3 = (Tracker*)((byte*)P_0 + 20);
			vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr3;
			*(int*)ptr4 = 0;
			((int*)ptr4)[1] = 0;
			((int*)ptr4)[2] = 0;
			try
			{
				vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
				*(int*)ptr5 = 0;
				((int*)ptr5)[1] = 0;
				((int*)ptr5)[2] = 0;
				try
				{
					vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
					*(int*)ptr6 = 0;
					((int*)ptr6)[1] = 0;
					((int*)ptr6)[2] = 0;
					try
					{
						vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 68);
						*(int*)ptr7 = 0;
						((int*)ptr7)[1] = 0;
						((int*)ptr7)[2] = 0;
						try
						{
							((int*)P_0)[21] = 0;
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 16u);
							stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr8 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
							uint num2 = ((uint*)ptr8)[1];
							if (num2 < (uint)((int*)ptr8)[2])
							{
								int num3 = (int)num2;
								((int*)ptr8)[1] = (int)(num2 + 4);
								if (num3 != 0)
								{
									*(uint*)num3 = num;
								}
							}
							else
							{
								eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E*)ptr8, (uint*)(int)num2, &num);
							}
							*(int*)rootPointer = *(int*)(int)(num * 12 + (uint)(*(int*)ptr3));
							return P_0;
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 68);
							throw;
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 52);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 36);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 20);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*, void>)(&eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
			throw;
		}
	}

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003ATerrainTextureAtlasRuntime_003E(Tracker* P_0, TerrainTextureAtlasRuntime** rootPointer)
	{
		*(int*)P_0 = 0;
		Tracker* ptr = (Tracker*)((byte*)P_0 + 4);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr2 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
		*(int*)ptr2 = 0;
		((int*)ptr2)[1] = 0;
		((int*)ptr2)[2] = 0;
		try
		{
			Tracker* ptr3 = (Tracker*)((byte*)P_0 + 20);
			vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*)ptr3;
			*(int*)ptr4 = 0;
			((int*)ptr4)[1] = 0;
			((int*)ptr4)[2] = 0;
			try
			{
				vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 36);
				*(int*)ptr5 = 0;
				((int*)ptr5)[1] = 0;
				((int*)ptr5)[2] = 0;
				try
				{
					vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
					*(int*)ptr6 = 0;
					((int*)ptr6)[1] = 0;
					((int*)ptr6)[2] = 0;
					try
					{
						vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 68);
						*(int*)ptr7 = 0;
						((int*)ptr7)[1] = 0;
						((int*)ptr7)[2] = 0;
						try
						{
							((int*)P_0)[21] = 0;
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 28u);
							stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr8 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)ptr;
							uint num2 = ((uint*)ptr8)[1];
							if (num2 < (uint)((int*)ptr8)[2])
							{
								int num3 = (int)num2;
								((int*)ptr8)[1] = (int)(num2 + 4);
								if (num3 != 0)
								{
									*(uint*)num3 = num;
								}
							}
							else
							{
								eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue((vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E*)ptr8, (uint*)(int)num2, &num);
							}
							*(int*)rootPointer = *(int*)(int)(num * 12 + (uint)(*(int*)ptr3));
							return P_0;
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cvoid_0020_002A_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 68);
							throw;
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 52);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 36);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 20);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*, void>)(&eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
			throw;
		}
	}

	internal unsafe static void eastl_002Evector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(vector_003CRelo_003A_003ATracker_003A_003ABlock_002Ceastl_003A_003Aallocator_003E* P_0, Tracker.Block* position, Tracker.Block* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num != (uint)((int*)P_0)[2])
		{
			Tracker.Block* ptr = value;
			if (value >= position && (nuint)value < (nuint)(int)num)
			{
				ptr = (Tracker.Block*)((byte*)value + 12);
			}
			Tracker.Block* ptr2 = (Tracker.Block*)(int)num;
			if (ptr2 != null)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr2, (int)(num - 12), 12);
			}
			num = ((uint*)P_0)[1];
			Tracker.Block* ptr3 = (Tracker.Block*)(int)num;
			Tracker.Block* ptr4 = (Tracker.Block*)(int)(num - 12);
			if (ptr4 != position)
			{
				do
				{
					ptr4 = (Tracker.Block*)((byte*)ptr4 - 12);
					ptr3 = (Tracker.Block*)((byte*)ptr3 - 12);
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr3, ptr4, 12);
				}
				while (ptr4 != position);
			}
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(position, ptr, 12);
			((int*)P_0)[1] += 12;
			return;
		}
		uint num2 = (uint)(((int)num - *(int*)P_0) / 12);
		uint num3;
		Tracker.Block* ptr5;
		if (num2 != 0)
		{
			num3 = num2 << 1;
			if (num3 == 0)
			{
				ptr5 = null;
				goto IL_00ac;
			}
		}
		else
		{
			num3 = 1u;
		}
		ptr5 = (Tracker.Block*)new_005B_005D(num3 * 12, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		goto IL_00ac;
		IL_00ac:
		Tracker.Block* ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_003E((Tracker.Block*)(int)(*(uint*)P_0), position, ptr5);
		if (ptr6 != null)
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr6, value, 12);
		}
		ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_003E(position, (Tracker.Block*)(int)((uint*)P_0)[1], (Tracker.Block*)((byte*)ptr6 + 12));
		Tracker.Block* ptr7 = (Tracker.Block*)(int)(*(uint*)P_0);
		if (ptr7 != null)
		{
			delete_005B_005D(ptr7);
		}
		*(int*)P_0 = (int)ptr5;
		((int*)P_0)[1] = (int)ptr6;
		((int*)P_0)[2] = (int)((int)(num3 * 12) + (byte*)ptr5);
	}

	internal unsafe static void eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* P_0, Tracker.GenericBookmark_003Cunsigned_0020int_003E* position, Tracker.GenericBookmark_003Cunsigned_0020int_003E* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num != (uint)((int*)P_0)[2])
		{
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = value;
			if (value >= position && (nuint)value < (nuint)(int)num)
			{
				ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)value + 12);
			}
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr2 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)num;
			if (ptr2 != null)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr2, (int)(num - 12), 12);
			}
			num = ((uint*)P_0)[1];
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr3 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)num;
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr4 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)(num - 12);
			if (ptr4 != position)
			{
				do
				{
					ptr4 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr4 - 12);
					ptr3 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr3 - 12);
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr3, ptr4, 12);
				}
				while (ptr4 != position);
			}
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(position, ptr, 12);
			((int*)P_0)[1] += 12;
			return;
		}
		uint num2 = (uint)(((int)num - *(int*)P_0) / 12);
		uint num3;
		Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr5;
		if (num2 != 0)
		{
			num3 = num2 << 1;
			if (num3 == 0)
			{
				ptr5 = null;
				goto IL_00ac;
			}
		}
		else
		{
			num3 = 1u;
		}
		ptr5 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)new_005B_005D(num3 * 12, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		goto IL_00ac;
		IL_00ac:
		Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_003E((Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)(*(uint*)P_0), position, ptr5);
		if (ptr6 != null)
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr6, value, 12);
		}
		ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_003E(position, (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)((uint*)P_0)[1], (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr6 + 12));
		Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr7 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)(*(uint*)P_0);
		if (ptr7 != null)
		{
			delete_005B_005D(ptr7);
		}
		*(int*)P_0 = (int)ptr5;
		((int*)P_0)[1] = (int)ptr6;
		((int*)P_0)[2] = (int)((int)(num3 * 12) + (byte*)ptr5);
	}

	internal unsafe static void eastl_002Equick_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		if (first == last)
		{
			return;
		}
		int num = (int)((nint)((byte*)last - (nuint)first) / (nint)12);
		int num2 = num;
		int num3 = 0;
		if (num2 != 0)
		{
			do
			{
				num2 >>= 1;
				num3++;
			}
			while (num2 != 0);
		}
		eastl_002Equick_sort_impl_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, last, num3 * 2 - 2, compare);
		if (num > 28)
		{
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)first + 336);
			eastl_002Einsertion_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, ptr, compare);
			eastl_002Einsertion_sort_simple_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(ptr, last, compare);
		}
		else
		{
			eastl_002Einsertion_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, last, compare);
		}
	}

	internal unsafe static void eastl_002Evector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(vector_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Ceastl_003A_003Aallocator_003E* P_0, TexturePacker.Texture** position, TexturePacker.Texture** value)
	{
		uint num = ((uint*)P_0)[1];
		if (num != (uint)((int*)P_0)[2])
		{
			TexturePacker.Texture** ptr = value;
			if (value >= position && (nuint)value < (nuint)(int)num)
			{
				ptr = (TexturePacker.Texture**)((byte*)value + 4);
			}
			TexturePacker.Texture** ptr2 = (TexturePacker.Texture**)(int)num;
			if (ptr2 != null)
			{
				*(int*)ptr2 = *(int*)(int)(num - 4);
			}
			num = ((uint*)P_0)[1];
			uint num2 = (uint)((nuint)(int)(num - 4) - (nuint)position);
			memmove((void*)(int)(num - (uint)((int)num2 >> 2 << 2)), position, num2);
			*(int*)position = *(int*)ptr;
			((int*)P_0)[1] += 4;
			return;
		}
		uint num3 = (uint)((int)num - *(int*)P_0 >> 2);
		uint num4;
		TexturePacker.Texture** ptr3;
		if (num3 != 0)
		{
			num4 = num3 << 1;
			if (num4 == 0)
			{
				ptr3 = null;
				goto IL_0094;
			}
		}
		else
		{
			num4 = 1u;
		}
		ptr3 = (TexturePacker.Texture**)new_005B_005D(num4 << 2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		goto IL_0094;
		IL_0094:
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cvoid_003E obj);
		*(int*)(&obj) = *(int*)P_0;
		TexturePacker.Texture** ptr4 = (TexturePacker.Texture**)((byte*)position - *(int*)(&obj));
		TexturePacker.Texture** ptr5 = (TexturePacker.Texture**)(((nint)ptr4 >> 2) * 4 + (byte*)memmove(ptr3, (void*)(int)(*(uint*)(&obj)), (uint)ptr4));
		if (ptr5 != null)
		{
			*(int*)ptr5 = *(int*)value;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cvoid_003E obj2);
		*(int*)(&obj2) = (int)((byte*)ptr5 + 4);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CBinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cvoid_003E obj3);
		*(int*)(&obj3) = ((int*)P_0)[1];
		TexturePacker.Texture** ptr6 = (TexturePacker.Texture**)((nuint)(*(int*)(&obj3)) - (nuint)position);
		ptr5 = (TexturePacker.Texture**)(((nint)ptr6 >> 2) * 4 + (byte*)memmove((void*)(int)(*(uint*)(&obj2)), position, (uint)ptr6));
		TexturePacker.Texture** ptr7 = (TexturePacker.Texture**)(int)(*(uint*)P_0);
		if (ptr7 != null)
		{
			delete_005B_005D(ptr7);
		}
		*(int*)P_0 = (int)ptr3;
		((int*)P_0)[1] = (int)ptr5;
		((int*)P_0)[2] = (int)((int)(num4 * 4) + (byte*)ptr3);
	}

	internal unsafe static void eastl_002Equick_sort_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** last, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		if (first == last)
		{
			return;
		}
		int num = (int)((nint)((byte*)last - (nuint)first) >> 2);
		int num2 = num;
		int num3 = 0;
		if (num2 != 0)
		{
			do
			{
				num2 >>= 1;
				num3++;
			}
			while (num2 != 0);
		}
		eastl_002Equick_sort_impl_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, last, num3 * 2 - 2, compare);
		if (num > 28)
		{
			TexturePacker.Texture** ptr = (TexturePacker.Texture**)((byte*)first + 112);
			eastl_002Einsertion_sort_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, ptr, compare);
			eastl_002Einsertion_sort_simple_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(ptr, last, compare);
		}
		else
		{
			eastl_002Einsertion_sort_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, last, compare);
		}
	}

	internal unsafe static Tracker.Block* eastl_002Euninitialized_copy_ptr_003Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_003E(Tracker.Block* first, Tracker.Block* last, Tracker.Block* result)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E dest);
		*(int*)(&dest) = (int)result;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E last2);
		*(int*)(&last2) = (int)last;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E first2);
		*(int*)(&first2) = (int)first;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E obj);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed);
		eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E_0020_003E(&obj, first2, last2, dest, _unnamed);
		return (Tracker.Block*)(int)(*(uint*)(&obj));
	}

	internal unsafe static Tracker.GenericBookmark_003Cunsigned_0020int_003E* eastl_002Euninitialized_copy_ptr_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, Tracker.GenericBookmark_003Cunsigned_0020int_003E* result)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E dest);
		*(int*)(&dest) = (int)result;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E last2);
		*(int*)(&last2) = (int)last;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E first2);
		*(int*)(&first2) = (int)first;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E obj);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed);
		eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E_0020_003E(&obj, first2, last2, dest, _unnamed);
		return (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)(*(uint*)(&obj));
	}

	internal unsafe static void eastl_002Evector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(vector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E* P_0, uint* position, uint* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num != (uint)((int*)P_0)[2])
		{
			uint* ptr = value;
			if (value >= position && (nuint)value < (nuint)(int)num)
			{
				ptr = value + 1;
			}
			uint* ptr2 = (uint*)(int)num;
			if (ptr2 != null)
			{
				*ptr2 = *(uint*)(int)(num - 4);
			}
			num = ((uint*)P_0)[1];
			uint num2 = (uint)((nuint)(int)(num - 4) - (nuint)position);
			memmove((void*)(int)(num - (uint)((int)num2 >> 2 << 2)), position, num2);
			*position = *ptr;
			((int*)P_0)[1] += 4;
			return;
		}
		uint num3 = (uint)((int)num - *(int*)P_0 >> 2);
		uint num4;
		uint* ptr3;
		if (num3 != 0)
		{
			num4 = num3 << 1;
			if (num4 == 0)
			{
				ptr3 = null;
				goto IL_0094;
			}
		}
		else
		{
			num4 = 1u;
		}
		ptr3 = (uint*)new_005B_005D(num4 << 2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		goto IL_0094;
		IL_0094:
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020int_0020_002A_002Cvoid_003E obj);
		*(int*)(&obj) = *(int*)P_0;
		uint* ptr4 = (uint*)((byte*)position - *(int*)(&obj));
		uint* ptr5 = ((nint)ptr4 >> 2) + (uint*)memmove(ptr3, (void*)(int)(*(uint*)(&obj)), (uint)ptr4);
		if (ptr5 != null)
		{
			*ptr5 = *value;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020int_0020_002A_002Cvoid_003E obj2);
		*(int*)(&obj2) = (int)(ptr5 + 1);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020int_0020_002A_002Cvoid_003E obj3);
		*(int*)(&obj3) = ((int*)P_0)[1];
		uint* ptr6 = (uint*)((nuint)(*(int*)(&obj3)) - (nuint)position);
		ptr5 = ((nint)ptr6 >> 2) + (uint*)memmove((void*)(int)(*(uint*)(&obj2)), position, (uint)ptr6);
		uint* ptr7 = (uint*)(int)(*(uint*)P_0);
		if (ptr7 != null)
		{
			delete_005B_005D(ptr7);
		}
		*(int*)P_0 = (int)ptr3;
		((int*)P_0)[1] = (int)ptr5;
		((int*)P_0)[2] = (int)((int)(num4 * 4) + (byte*)ptr3);
	}

	internal unsafe static void eastl_002Equick_sort_impl_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, int kRecursionCount, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		if ((nint)((byte*)last - (nuint)first) / (nint)12 > 28)
		{
			while (kRecursionCount > 0)
			{
				Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = eastl_002Eget_partition_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, last, eastl_002Emedian_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((nint)((byte*)last - (nuint)first) / (nint)12 / 2 * 12 + (byte*)first), (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)last - 12), compare), compare);
				kRecursionCount--;
				eastl_002Equick_sort_impl_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(ptr, last, kRecursionCount, compare);
				last = ptr;
				if ((nint)((byte*)ptr - (nuint)first) / (nint)12 <= 28)
				{
					break;
				}
			}
		}
		if (kRecursionCount == 0)
		{
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* intPtr = last;
			eastl_002Epartial_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, intPtr, intPtr, compare);
		}
	}

	internal unsafe static void eastl_002Einsertion_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		if (first == last)
		{
			return;
		}
		Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)first + 12);
		if (ptr == last)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E obj);
		do
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref obj, ptr, 12);
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr2 = ptr;
			if (ptr != first)
			{
				while (compare(&obj, (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr2 - 12)) != 0)
				{
					Tracker.GenericBookmark_003Cunsigned_0020int_003E* num = ptr2;
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(num, (byte*)num - 12, 12);
					ptr2 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr2 - 12);
					if (ptr2 == first)
					{
						break;
					}
				}
			}
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr2, ref obj, 12);
			ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr + 12);
		}
		while (ptr != last);
	}

	internal unsafe static void eastl_002Einsertion_sort_simple_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = first;
		if (first == last)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E obj);
		do
		{
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr2 = ptr;
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref obj, ptr, 12);
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr3 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr - 12);
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr4 = ptr3;
			if (compare(&obj, ptr3) != 0)
			{
				do
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr2, ptr4, 12);
					ptr2 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr2 - 12);
					ptr4 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr4 - 12);
				}
				while (compare(&obj, ptr4) != 0);
			}
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr2, ref obj, 12);
			ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr + 12);
		}
		while (ptr != last);
	}

	internal unsafe static void eastl_002Equick_sort_impl_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** last, int kRecursionCount, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		if ((nint)((nuint)((byte*)last - (nuint)first) & unchecked((nuint)(-4))) > (nint)112)
		{
			while (kRecursionCount > 0)
			{
				TexturePacker.Texture** ptr = eastl_002Eget_partition_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, last, eastl_002Emedian_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, (TexturePacker.Texture**)(((nint)((byte*)last - (nuint)first) >> 2) / 2 * 4 + (byte*)first), (TexturePacker.Texture**)((byte*)last - 4), compare), compare);
				kRecursionCount--;
				eastl_002Equick_sort_impl_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(ptr, last, kRecursionCount, compare);
				last = ptr;
				if ((nint)((nuint)((byte*)ptr - (nuint)first) & unchecked((nuint)(-4))) <= (nint)112)
				{
					break;
				}
			}
		}
		if (kRecursionCount == 0)
		{
			TexturePacker.Texture** intPtr = last;
			eastl_002Epartial_sort_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, intPtr, intPtr, compare);
		}
	}

	internal unsafe static void eastl_002Einsertion_sort_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** last, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		if (first == last)
		{
			return;
		}
		TexturePacker.Texture** ptr = (TexturePacker.Texture**)((byte*)first + 4);
		if (ptr == last)
		{
			return;
		}
		do
		{
			TexturePacker.Texture* ptr2 = (TexturePacker.Texture*)(int)(*(uint*)ptr);
			TexturePacker.Texture** ptr3 = ptr;
			if (ptr != first)
			{
				while (compare(ptr2, (TexturePacker.Texture*)(int)(*((uint*)ptr3 - 1))) != 0)
				{
					TexturePacker.Texture** num = ptr3;
					*(int*)num = *((int*)num - 1);
					ptr3 = (TexturePacker.Texture**)((byte*)ptr3 - 4);
					if (ptr3 == first)
					{
						break;
					}
				}
			}
			*(int*)ptr3 = (int)ptr2;
			ptr = (TexturePacker.Texture**)((byte*)ptr + 4);
		}
		while (ptr != last);
	}

	internal unsafe static void eastl_002Einsertion_sort_simple_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** last, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		TexturePacker.Texture** ptr = first;
		if (first == last)
		{
			return;
		}
		do
		{
			TexturePacker.Texture** ptr2 = ptr;
			TexturePacker.Texture* ptr3 = (TexturePacker.Texture*)(int)(*(uint*)ptr);
			TexturePacker.Texture** ptr4 = (TexturePacker.Texture**)((byte*)ptr - 4);
			if (compare(ptr3, (TexturePacker.Texture*)(int)(*((uint*)ptr - 1))) != 0)
			{
				do
				{
					*(int*)ptr2 = *(int*)ptr4;
					ptr2 = (TexturePacker.Texture**)((byte*)ptr2 - 4);
					ptr4 = (TexturePacker.Texture**)((byte*)ptr4 - 4);
				}
				while (compare(ptr3, (TexturePacker.Texture*)(int)(*(uint*)ptr4)) != 0);
			}
			*(int*)ptr2 = (int)ptr3;
			ptr = (TexturePacker.Texture**)((byte*)ptr + 4);
		}
		while (ptr != last);
	}

	internal unsafe static generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E* eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E_0020_003E(generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E* P_0, generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E first, generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E last, generic_iterator_003CRelo_003A_003ATracker_003A_003ABlock_0020_002A_002Cvoid_003E dest, integral_constant_003Cbool_002C0_003E __unnamed003)
	{
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(P_0, ref dest, 4);
		while ((byte)((*(int*)(&first) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			int num = *(int*)P_0;
			void* ptr = (void*)num;
			Tracker.Block* ptr2 = (Tracker.Block*)ptr;
			try
			{
				if (ptr != null)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr, *(int*)(&first), 12);
				}
			}
			catch
			{
				//try-fault
				delete(ptr2, ptr);
				throw;
			}
			*(int*)(&first) += 12;
			*(int*)P_0 = num + 12;
		}
		return P_0;
	}

	internal unsafe static generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E* eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E_0020_003E(generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E* P_0, generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E first, generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E last, generic_iterator_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cvoid_003E dest, integral_constant_003Cbool_002C0_003E __unnamed003)
	{
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(P_0, ref dest, 4);
		while ((byte)((*(int*)(&first) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			int num = *(int*)P_0;
			void* ptr = (void*)num;
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr2 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)ptr;
			try
			{
				if (ptr != null)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr, *(int*)(&first), 12);
				}
			}
			catch
			{
				//try-fault
				delete(ptr2, ptr);
				throw;
			}
			*(int*)(&first) += 12;
			*(int*)P_0 = num + 12;
		}
		return P_0;
	}

	internal unsafe static Tracker.GenericBookmark_003Cunsigned_0020int_003E* eastl_002Emedian_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* a, Tracker.GenericBookmark_003Cunsigned_0020int_003E* b, Tracker.GenericBookmark_003Cunsigned_0020int_003E* c, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		if (compare(a, b) != 0)
		{
			if (compare(b, c) != 0)
			{
				return b;
			}
			if (compare(a, c) != 0)
			{
				return c;
			}
			return a;
		}
		if (compare(a, c) != 0)
		{
			return a;
		}
		if (compare(b, c) != 0)
		{
			return c;
		}
		return b;
	}

	internal unsafe static Tracker.GenericBookmark_003Cunsigned_0020int_003E* eastl_002Eget_partition_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, Tracker.GenericBookmark_003Cunsigned_0020int_003E* pivotValue, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E obj);
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(ref obj, pivotValue, 12);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E obj2);
		while (true)
		{
			if (compare(first, &obj) != 0)
			{
				first = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)first + 12);
				continue;
			}
			last = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)last - 12);
			if (compare(&obj, last) != 0)
			{
				do
				{
					last = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)last - 12);
				}
				while (compare(&obj, last) != 0);
			}
			if (first >= last)
			{
				break;
			}
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ref obj2, first, 12);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(first, last, 12);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(last, ref obj2, 12);
			first = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)first + 12);
		}
		return first;
	}

	internal unsafe static void eastl_002Epartial_sort_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* middle, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		eastl_002Emake_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, middle, compare);
		Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = middle;
		if (middle < last)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E value);
			do
			{
				if (compare(ptr, first) != 0)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ref value, ptr, 12);
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr, first, 12);
					eastl_002Eadjust_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, 0, (int)((nint)((byte*)middle - (nuint)first) / (nint)12), 0, value, compare);
				}
				ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr + 12);
			}
			while (ptr < last);
		}
		eastl_002Esort_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, middle, compare);
	}

	internal unsafe static TexturePacker.Texture** eastl_002Emedian_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** a, TexturePacker.Texture** b, TexturePacker.Texture** c, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		if (compare((TexturePacker.Texture*)(int)(*(uint*)a), (TexturePacker.Texture*)(int)(*(uint*)b)) != 0)
		{
			if (compare((TexturePacker.Texture*)(int)(*(uint*)b), (TexturePacker.Texture*)(int)(*(uint*)c)) != 0)
			{
				return b;
			}
			if (compare((TexturePacker.Texture*)(int)(*(uint*)a), (TexturePacker.Texture*)(int)(*(uint*)c)) != 0)
			{
				return c;
			}
			return a;
		}
		if (compare((TexturePacker.Texture*)(int)(*(uint*)a), (TexturePacker.Texture*)(int)(*(uint*)c)) != 0)
		{
			return a;
		}
		if (compare((TexturePacker.Texture*)(int)(*(uint*)b), (TexturePacker.Texture*)(int)(*(uint*)c)) != 0)
		{
			return c;
		}
		return b;
	}

	internal unsafe static TexturePacker.Texture** eastl_002Eget_partition_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** last, TexturePacker.Texture** pivotValue, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		TexturePacker.Texture* ptr = (TexturePacker.Texture*)(int)(*(uint*)pivotValue);
		while (true)
		{
			if (compare((TexturePacker.Texture*)(int)(*(uint*)first), ptr) != 0)
			{
				first = (TexturePacker.Texture**)((byte*)first + 4);
				continue;
			}
			last = (TexturePacker.Texture**)((byte*)last - 4);
			if (compare(ptr, (TexturePacker.Texture*)(int)(*(uint*)last)) != 0)
			{
				do
				{
					last = (TexturePacker.Texture**)((byte*)last - 4);
				}
				while (compare(ptr, (TexturePacker.Texture*)(int)(*(uint*)last)) != 0);
			}
			if (first >= last)
			{
				break;
			}
			TexturePacker.Texture* ptr2 = (TexturePacker.Texture*)(int)(*(uint*)first);
			*(int*)first = *(int*)last;
			*(int*)last = (int)ptr2;
			first = (TexturePacker.Texture**)((byte*)first + 4);
		}
		return first;
	}

	internal unsafe static void eastl_002Epartial_sort_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** middle, TexturePacker.Texture** last, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		eastl_002Emake_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, middle, compare);
		TexturePacker.Texture** ptr = middle;
		if (middle < last)
		{
			do
			{
				if (compare((TexturePacker.Texture*)(int)(*(uint*)ptr), (TexturePacker.Texture*)(int)(*(uint*)first)) != 0)
				{
					TexturePacker.Texture* value = (TexturePacker.Texture*)(int)(*(uint*)ptr);
					*(int*)ptr = *(int*)first;
					eastl_002Eadjust_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, 0, (int)((nint)((byte*)middle - (nuint)first) >> 2), 0, value, compare);
				}
				ptr = (TexturePacker.Texture**)((byte*)ptr + 4);
			}
			while (ptr < last);
		}
		eastl_002Esort_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, middle, compare);
	}

	internal unsafe static void eastl_002Emake_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		int num = (int)((nint)((byte*)last - (nuint)first) / (nint)12);
		if (num >= 2)
		{
			int num2 = (num - 2 >> 1) + 1;
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(num2 * 12 + (byte*)first);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E value);
			do
			{
				num2--;
				ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr - 12);
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref value, ptr, 12);
				eastl_002Eadjust_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, num2, num, num2, value, compare);
			}
			while (num2 != 0);
		}
	}

	internal unsafe static void eastl_002Eadjust_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, int topPosition, int heapSize, int position, Tracker.GenericBookmark_003Cunsigned_0020int_003E value, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		int num = position * 2 + 2;
		if (num < heapSize)
		{
			do
			{
				byte* num2 = (byte*)first + num * 12;
				if (compare((Tracker.GenericBookmark_003Cunsigned_0020int_003E*)num2, (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(num2 - 12)) != 0)
				{
					num--;
				}
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(position * 12 + (byte*)first, num * 12 + (byte*)first, 12);
				position = num;
				num = num * 2 + 2;
			}
			while (num < heapSize);
		}
		if (num == heapSize)
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(position * 12 + (byte*)first, (byte*)first + num * 12 - 12, 12);
			position = num - 1;
		}
		eastl_002Epromote_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, topPosition, position, value, compare);
	}

	internal unsafe static void eastl_002Esort_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, Tracker.GenericBookmark_003Cunsigned_0020int_003E* last, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		int num = (int)((nint)((byte*)last - (nuint)first) / (nint)12);
		if (num > 1)
		{
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)last - 12);
			Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr2 = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((nuint)12u - (nuint)first);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E value);
			int num2;
			do
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref value, ptr, 12);
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr, first, 12);
				eastl_002Eadjust_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(first, 0, num - 1, 0, value, compare);
				ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)ptr - 12);
				num2 = (int)((nint)((byte*)ptr2 + (nuint)ptr) / (nint)12);
				num = num2;
			}
			while (num2 > 1);
		}
	}

	internal unsafe static void eastl_002Emake_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** last, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		int num = (int)((nint)((byte*)last - (nuint)first) >> 2);
		if (num >= 2)
		{
			int num2 = (num - 2 >> 1) + 1;
			do
			{
				num2--;
				int topPosition = num2;
				int num3 = num2;
				eastl_002Eadjust_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, topPosition, num, num3, (TexturePacker.Texture*)(int)(*(uint*)(num3 * 4 + (byte*)first)), compare);
			}
			while (num2 != 0);
		}
	}

	internal unsafe static void eastl_002Eadjust_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, int topPosition, int heapSize, int position, TexturePacker.Texture* value, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		int num = position * 2 + 2;
		if (num < heapSize)
		{
			do
			{
				if (compare((TexturePacker.Texture*)(int)(*(uint*)(num * 4 + (byte*)first)), (TexturePacker.Texture*)(int)(*((uint*)(num * 4 + (byte*)first) - 1))) != 0)
				{
					num--;
				}
				*(int*)(position * 4 + (byte*)first) = *(int*)(num * 4 + (byte*)first);
				position = num;
				num = num * 2 + 2;
			}
			while (num < heapSize);
		}
		if (num == heapSize)
		{
			*(int*)(position * 4 + (byte*)first) = *((int*)(num * 4 + (byte*)first) - 1);
			position = num - 1;
		}
		eastl_002Epromote_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, topPosition, position, value, compare);
	}

	internal unsafe static void eastl_002Esort_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, TexturePacker.Texture** last, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		int num = (int)((nint)((byte*)last - (nuint)first) >> 2);
		if (num > 1)
		{
			TexturePacker.Texture** ptr = (TexturePacker.Texture**)((byte*)last - 4);
			TexturePacker.Texture** ptr2 = (TexturePacker.Texture**)((nuint)4u - (nuint)first);
			int num2;
			do
			{
				TexturePacker.Texture* value = (TexturePacker.Texture*)(int)(*(uint*)ptr);
				*(int*)ptr = *(int*)first;
				eastl_002Eadjust_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(first, 0, num - 1, 0, value, compare);
				ptr = (TexturePacker.Texture**)((byte*)ptr - 4);
				num2 = (int)((nint)((byte*)ptr2 + (nuint)ptr) >> 2);
				num = num2;
			}
			while (num2 > 1);
		}
	}

	internal unsafe static void eastl_002Epromote_heap_003Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020_002A_002Cint_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_002Cstruct_0020Relo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_0020const_0020_0026_0029_003E(Tracker.GenericBookmark_003Cunsigned_0020int_003E* first, int topPosition, int position, Tracker.GenericBookmark_003Cunsigned_0020int_003E value, delegate* unmanaged[Cdecl, Cdecl]<Tracker.GenericBookmark_003Cunsigned_0020int_003E*, Tracker.GenericBookmark_003Cunsigned_0020int_003E*, byte> compare)
	{
		int num = position - 1 >> 1;
		if (position > topPosition)
		{
			do
			{
				Tracker.GenericBookmark_003Cunsigned_0020int_003E* ptr = (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)((byte*)first + num * 12);
				if (compare(ptr, &value) == 0)
				{
					break;
				}
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(position * 12 + (byte*)first, ptr, 12);
				position = num;
				num = num - 1 >> 1;
			}
			while (position > topPosition);
		}
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(position * 12 + (byte*)first, ref value, 12);
	}

	internal unsafe static void eastl_002Epromote_heap_003Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0020_002A_002Cint_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cbool_0020_0028__cdecl_002A_0029_0028struct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_002Cstruct_0020BinaryAssetBuilder_003A_003ATextureCompiler_003A_003ATexturePacker_003A_003ATexture_0020_002A_0029_003E(TexturePacker.Texture** first, int topPosition, int position, TexturePacker.Texture* value, delegate* unmanaged[Cdecl, Cdecl]<TexturePacker.Texture*, TexturePacker.Texture*, byte> compare)
	{
		int num = position - 1 >> 1;
		if (position > topPosition)
		{
			while (compare((TexturePacker.Texture*)(int)(*(uint*)(num * 4 + (byte*)first)), value) != 0)
			{
				*(int*)(position * 4 + (byte*)first) = *(int*)(num * 4 + (byte*)first);
				position = num;
				num = num - 1 >> 1;
				if (position <= topPosition)
				{
					break;
				}
			}
		}
		*(int*)(position * 4 + (byte*)first) = (int)value;
	}

	internal unsafe static void* new_005B_005D(uint size, sbyte* pName, int flags, uint debugFlags, sbyte* file, int line)
	{
		return new_005B_005D(size);
	}

	internal unsafe static void Marshaler_002EIValue_002E_007Bdtor_007D(IValue* P_0)
	{
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IValue_0040Marshaler_0040_00406B_0040));
	}

	internal unsafe static void* Marshaler_002EIValue_002E__vecDelDtor(IValue* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			IValue* ptr = (IValue*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 4u, *(int*)ptr, (delegate*<void*, void>)(delegate*<IValue*, void>)(&Marshaler_002EIValue_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete(ptr);
			}
			return ptr;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IValue_0040Marshaler_0040_00406B_0040));
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static void Marshaler_002EINode_002E_007Bdtor_007D(INode* P_0)
	{
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7INode_0040Marshaler_0040_00406B_0040));
	}

	internal unsafe static void* Marshaler_002EINode_002E__vecDelDtor(INode* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			INode* ptr = (INode*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 4u, *(int*)ptr, (delegate*<void*, void>)(delegate*<INode*, void>)(&Marshaler_002EINode_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete(ptr);
			}
			return ptr;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7INode_0040Marshaler_0040_00406B_0040));
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static void Marshaler_002EIList_002E_007Bdtor_007D(IList* P_0)
	{
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IList_0040Marshaler_0040_00406B_0040));
	}

	internal unsafe static void* Marshaler_002EIList_002E__vecDelDtor(IList* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			IList* ptr = (IList*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 4u, *(int*)ptr, (delegate*<void*, void>)(delegate*<IList*, void>)(&Marshaler_002EIList_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete(ptr);
			}
			return ptr;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IList_0040Marshaler_0040_00406B_0040));
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static Node* Node_002E_007Bctor_007D(Node* P_0, gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* navigator, gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* namespaceManager)
	{
		try
		{
			try
			{
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7INode_0040Marshaler_0040_00406B_0040));
				try
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7Node_0040_00406B_0040));
					byte* num = (byte*)P_0 + 4;
					IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)navigator));
					*(int*)num = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
					try
					{
						((int*)P_0)[2] = (int)((IntPtr)GCHandle.Alloc(null)).ToPointer();
						try
						{
							byte* num2 = (byte*)P_0 + 12;
							IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)namespaceManager));
							*(int*)num2 = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr2).Target)).ToPointer();
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<global::gcroot_003CSystem_003A_003AString_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
							throw;
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<INode*, void>)(&Marshaler_002EINode_002E_007Bdtor_007D), P_0);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bdtor_007D), navigator);
				throw;
			}
			IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)navigator));
			((GCHandle)intPtr3).Free();
			*(int*)navigator = 0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), namespaceManager);
			throw;
		}
		IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)namespaceManager));
		((GCHandle)intPtr4).Free();
		*(int*)namespaceManager = 0;
		return P_0;
	}

	internal unsafe static void* Node_002E__vecDelDtor(Node* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			Node* ptr = (Node*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 16u, *(int*)ptr, (delegate*<void*, void>)(delegate*<Node*, void>)(&Node_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete_005B_005D(ptr);
			}
			return ptr;
		}
		Node_002E_007Bdtor_007D(P_0);
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static void Node_002E_007Bdtor_007D(Node* P_0)
	{
		try
		{
			try
			{
				try
				{
					gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* ptr = (gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*)((byte*)P_0 + 12);
					IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)ptr));
					((GCHandle)intPtr).Free();
					*(int*)ptr = 0;
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<global::gcroot_003CSystem_003A_003AString_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
					throw;
				}
				global::gcroot_003CSystem_003A_003AString_0020_005E_003E* ptr2 = (global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)((byte*)P_0 + 8);
				IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)ptr2));
				((GCHandle)intPtr2).Free();
				*(int*)ptr2 = 0;
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
				throw;
			}
			gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr3 = (gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E*)((byte*)P_0 + 4);
			IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)ptr3));
			((GCHandle)intPtr3).Free();
			*(int*)ptr3 = 0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<INode*, void>)(&Marshaler_002EINode_002E_007Bdtor_007D), P_0);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7INode_0040Marshaler_0040_00406B_0040));
	}

	internal unsafe static Node* Node_002E_007Bctor_007D(Node* P_0, global::gcroot_003CSystem_003A_003AString_0020_005E_003E* defaultValue)
	{
		try
		{
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7INode_0040Marshaler_0040_00406B_0040));
			try
			{
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7Node_0040_00406B_0040));
				((int*)P_0)[1] = (int)((IntPtr)GCHandle.Alloc(null)).ToPointer();
				try
				{
					byte* num = (byte*)P_0 + 8;
					IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)defaultValue));
					*(int*)num = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
					try
					{
						((int*)P_0)[3] = (int)((IntPtr)GCHandle.Alloc(null)).ToPointer();
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<global::gcroot_003CSystem_003A_003AString_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<INode*, void>)(&Marshaler_002EINode_002E_007Bdtor_007D), P_0);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<global::gcroot_003CSystem_003A_003AString_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D), defaultValue);
			throw;
		}
		IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)defaultValue));
		((GCHandle)intPtr2).Free();
		*(int*)defaultValue = 0;
		return P_0;
	}

	internal unsafe static IList* Node_002EGetChildNodes(Node* P_0, sbyte* name)
	{
		IntPtr intPtr = new IntPtr((void*)(int)((uint*)P_0)[1]);
		if (((GCHandle)intPtr).Target != null)
		{
			if (name != null)
			{
				List* ptr = (List*)@new(16u);
				try
				{
					if (ptr != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
						gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
						Node* ptr2 = (Node*)((byte*)P_0 + 12);
						gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* namespaceManager = gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bctor_007D(&obj, (gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*)ptr2);
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E obj2);
						try
						{
							gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E* ptr3 = &obj2;
							IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)ptr2));
							XmlNamespaceManager target = (XmlNamespaceManager)((GCHandle)intPtr2).Target;
							IntPtr intPtr3 = new IntPtr((void*)(int)((uint*)P_0)[1]);
							*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc((XPathNodeIterator)((XPathNavigator)((GCHandle)intPtr3).Target).Evaluate($"ea:{new string(name)}", (IXmlNamespaceResolver)target))).ToPointer();
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
							throw;
						}
						return (IList*)List_002E_007Bctor_007D(ptr, &obj2, namespaceManager);
					}
					return null;
				}
				catch
				{
					//try-fault
					delete(ptr);
					throw;
				}
			}
			List* ptr4 = (List*)@new(16u);
			try
			{
				if (ptr4 != null)
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj3);
					gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis2 = &obj3;
					gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* namespaceManager2 = gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bctor_007D(&obj3, (gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*)((byte*)P_0 + 12));
					System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E obj4);
					try
					{
						gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E* ptr5 = &obj4;
						IntPtr intPtr4 = new IntPtr((void*)(int)((uint*)P_0)[1]);
						*(int*)(&obj4) = (int)((IntPtr)GCHandle.Alloc(((XPathNavigator)((GCHandle)intPtr4).Target).SelectChildren(XPathNodeType.Element))).ToPointer();
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis2);
						throw;
					}
					return (IList*)List_002E_007Bctor_007D(ptr4, &obj4, namespaceManager2);
				}
				return null;
			}
			catch
			{
				//try-fault
				delete(ptr4);
				throw;
			}
		}
		return null;
	}

	internal unsafe static NodeHolder* Node_002EGetChildNode(Node* P_0, NodeHolder* P_1, sbyte* name, sbyte* defaultValue)
	{
		uint num = 0u;
		Node* ptr = null;
		IntPtr intPtr = new IntPtr((void*)(int)((uint*)P_0)[1]);
		if (((GCHandle)intPtr).Target != null)
		{
			IntPtr intPtr2 = new IntPtr((void*)(int)((uint*)P_0)[1]);
			XPathNavigator target = (XPathNavigator)((GCHandle)intPtr2).Target;
			IntPtr intPtr3 = new IntPtr((void*)(int)((uint*)P_0)[1]);
			if (((XPathNavigator)((GCHandle)intPtr3).Target).MoveToChild(new string(name), target.NamespaceURI))
			{
				Node* ptr2 = (Node*)@new(16u);
				try
				{
					Node* ptr4;
					try
					{
						if (ptr2 != null)
						{
							System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
							gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
							gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* namespaceManager = gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bctor_007D(&obj, (gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*)((byte*)P_0 + 12));
							System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
							try
							{
								gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr3 = &obj2;
								IntPtr intPtr4 = new IntPtr((void*)(int)((uint*)P_0)[1]);
								*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(((XPathNavigator)((GCHandle)intPtr4).Target).CreateNavigator())).ToPointer();
							}
							catch
							{
								//try-fault
								___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
								throw;
							}
							ptr4 = Node_002E_007Bctor_007D(ptr2, &obj2, namespaceManager);
						}
						else
						{
							ptr4 = null;
						}
					}
					catch
					{
						//try-fault
						delete(ptr2);
						throw;
					}
					ptr = ptr4;
					IntPtr intPtr5 = new IntPtr((void*)(int)((uint*)P_0)[1]);
					((XPathNavigator)((GCHandle)intPtr5).Target).MoveToParent();
					if (ptr4 != null)
					{
						goto IL_019e;
					}
				}
				catch
				{
					//try-fault
					if ((num & 1) != 0)
					{
						num &= 0xFFFFFFFEu;
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), P_1);
					}
					throw;
				}
			}
		}
		try
		{
			if (defaultValue != null)
			{
				Node* ptr5 = (Node*)@new(16u);
				Node* ptr7;
				try
				{
					if (ptr5 != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out global::gcroot_003CSystem_003A_003AString_0020_005E_003E obj3);
						global::gcroot_003CSystem_003A_003AString_0020_005E_003E* ptr6 = &obj3;
						*(int*)(&obj3) = (int)((IntPtr)GCHandle.Alloc(new string(defaultValue))).ToPointer();
						ptr7 = Node_002E_007Bctor_007D(ptr5, &obj3);
					}
					else
					{
						ptr7 = null;
					}
				}
				catch
				{
					//try-fault
					delete(ptr5);
					throw;
				}
				ptr = ptr7;
			}
		}
		catch
		{
			//try-fault
			if ((num & 1) != 0)
			{
				num &= 0xFFFFFFFEu;
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), P_1);
			}
			throw;
		}
		goto IL_019e;
		IL_019e:
		try
		{
			*(int*)P_1 = (int)ptr;
			num = 1u;
			return P_1;
		}
		catch
		{
			//try-fault
			if ((num & 1) != 0)
			{
				num &= 0xFFFFFFFEu;
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), P_1);
			}
			throw;
		}
	}

	internal unsafe static IValue* Node_002EGetAttributeValue(Node* P_0, sbyte* name, sbyte* defaultValue)
	{
		Value* result = null;
		IntPtr intPtr = new IntPtr((void*)(int)((uint*)P_0)[1]);
		if (((GCHandle)intPtr).Target != null)
		{
			IntPtr intPtr2 = new IntPtr((void*)(int)((uint*)P_0)[1]);
			if (((XPathNavigator)((GCHandle)intPtr2).Target).MoveToAttribute(new string(name), string.Empty))
			{
				Value* ptr = (Value*)@new(8u);
				Value* ptr3;
				try
				{
					if (ptr != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out global::gcroot_003CSystem_003A_003AString_0020_005E_003E obj);
						global::gcroot_003CSystem_003A_003AString_0020_005E_003E* ptr2 = &obj;
						IntPtr intPtr3 = new IntPtr((void*)(int)((uint*)P_0)[1]);
						*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(((XPathItem)((GCHandle)intPtr3).Target).Value)).ToPointer();
						ptr3 = Value_002E_007Bctor_007D(ptr, &obj);
					}
					else
					{
						ptr3 = null;
					}
				}
				catch
				{
					//try-fault
					delete(ptr);
					throw;
				}
				result = ptr3;
				IntPtr intPtr4 = new IntPtr((void*)(int)((uint*)P_0)[1]);
				((XPathNavigator)((GCHandle)intPtr4).Target).MoveToParent();
				if (ptr3 != null)
				{
					goto IL_011a;
				}
			}
		}
		if (defaultValue != null)
		{
			Value* ptr4 = (Value*)@new(8u);
			Value* ptr6;
			try
			{
				if (ptr4 != null)
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out global::gcroot_003CSystem_003A_003AString_0020_005E_003E obj2);
					global::gcroot_003CSystem_003A_003AString_0020_005E_003E* ptr5 = &obj2;
					*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(new string(defaultValue))).ToPointer();
					ptr6 = Value_002E_007Bctor_007D(ptr4, &obj2);
				}
				else
				{
					ptr6 = null;
				}
			}
			catch
			{
				//try-fault
				delete(ptr4);
				throw;
			}
			result = ptr6;
		}
		goto IL_011a;
		IL_011a:
		return (IValue*)result;
	}

	internal unsafe static IValue* Node_002EGetValue(Node* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)((uint*)P_0)[1]);
		if (((GCHandle)intPtr).Target != null)
		{
			Value* ptr = (Value*)@new(8u);
			try
			{
				if (ptr != null)
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out global::gcroot_003CSystem_003A_003AString_0020_005E_003E obj);
					global::gcroot_003CSystem_003A_003AString_0020_005E_003E* ptr2 = &obj;
					IntPtr intPtr2 = new IntPtr((void*)(int)((uint*)P_0)[1]);
					*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(((XPathItem)((GCHandle)intPtr2).Target).Value)).ToPointer();
					return (IValue*)Value_002E_007Bctor_007D(ptr, &obj);
				}
				return null;
			}
			catch
			{
				//try-fault
				delete(ptr);
				throw;
			}
		}
		Node* ptr3 = (Node*)((byte*)P_0 + 8);
		IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)ptr3));
		if (((GCHandle)intPtr3).Target != null)
		{
			Value* ptr4 = (Value*)@new(8u);
			try
			{
				if (ptr4 != null)
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out global::gcroot_003CSystem_003A_003AString_0020_005E_003E obj2);
					global::gcroot_003CSystem_003A_003AString_0020_005E_003E* ptr5 = &obj2;
					global::gcroot_003CSystem_003A_003AString_0020_005E_003E* text = gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bctor_007D(&obj2, (global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)ptr3);
					return (IValue*)Value_002E_007Bctor_007D(ptr4, text);
				}
				return null;
			}
			catch
			{
				//try-fault
				delete(ptr4);
				throw;
			}
		}
		return null;
	}

	internal unsafe static void Node_002ERelease(Node* P_0)
	{
		if (P_0 != null)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, uint, void*>)(int)(*(uint*)(*(int*)P_0 + 20)))((nint)P_0, 1u);
		}
	}

	internal unsafe static List* List_002E_007Bctor_007D(List* P_0, gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E* iterator, gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* namespaceManager)
	{
		try
		{
			try
			{
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IList_0040Marshaler_0040_00406B_0040));
				try
				{
					System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7List_0040_00406B_0040));
					byte* num = (byte*)P_0 + 4;
					IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)iterator));
					*(int*)num = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
					try
					{
						IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)iterator));
						XPathNodeIterator value = ((XPathNodeIterator)((GCHandle)intPtr2).Target).Clone();
						gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E* ptr = (gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*)((byte*)P_0 + 8);
						*(int*)ptr = (int)((IntPtr)GCHandle.Alloc(value)).ToPointer();
						try
						{
							byte* num2 = (byte*)P_0 + 12;
							IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)namespaceManager));
							*(int*)num2 = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr3).Target)).ToPointer();
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
							throw;
						}
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
						throw;
					}
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<IList*, void>)(&Marshaler_002EIList_002E_007Bdtor_007D), P_0);
					throw;
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E_002E_007Bdtor_007D), iterator);
				throw;
			}
			IntPtr intPtr4 = new IntPtr((void*)(int)(*(uint*)iterator));
			((GCHandle)intPtr4).Free();
			*(int*)iterator = 0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), namespaceManager);
			throw;
		}
		IntPtr intPtr5 = new IntPtr((void*)(int)(*(uint*)namespaceManager));
		((GCHandle)intPtr5).Free();
		*(int*)namespaceManager = 0;
		return P_0;
	}

	internal unsafe static void* List_002E__vecDelDtor(List* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			List* ptr = (List*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 16u, *(int*)ptr, (delegate*<void*, void>)(delegate*<List*, void>)(&List_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete_005B_005D(ptr);
			}
			return ptr;
		}
		List_002E_007Bdtor_007D(P_0);
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static void List_002E_007Bdtor_007D(List* P_0)
	{
		try
		{
			try
			{
				try
				{
					gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* ptr = (gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*)((byte*)P_0 + 12);
					IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)ptr));
					((GCHandle)intPtr).Free();
					*(int*)ptr = 0;
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 8);
					throw;
				}
				gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E* ptr2 = (gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*)((byte*)P_0 + 8);
				IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)ptr2));
				((GCHandle)intPtr2).Free();
				*(int*)ptr2 = 0;
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E_002E_007Bdtor_007D), (byte*)P_0 + 4);
				throw;
			}
			gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E* ptr3 = (gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E*)((byte*)P_0 + 4);
			IntPtr intPtr3 = new IntPtr((void*)(int)(*(uint*)ptr3));
			((GCHandle)intPtr3).Free();
			*(int*)ptr3 = 0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<IList*, void>)(&Marshaler_002EIList_002E_007Bdtor_007D), P_0);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IList_0040Marshaler_0040_00406B_0040));
	}

	internal unsafe static int List_002EGetCount(List* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)((uint*)P_0)[1]);
		return ((XPathNodeIterator)((GCHandle)intPtr).Target).Count;
	}

	internal unsafe static NodeHolder* List_002EGetNextNode(List* P_0, NodeHolder* P_1)
	{
		uint num = 0u;
		IntPtr intPtr = new IntPtr((void*)(int)((uint*)P_0)[1]);
		if (((XPathNodeIterator)((GCHandle)intPtr).Target).MoveNext())
		{
			Node* ptr = (Node*)@new(16u);
			try
			{
				Node* ptr3;
				try
				{
					if (ptr != null)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E obj);
						gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* pThis = &obj;
						IntPtr intPtr2 = new IntPtr((void*)(int)((uint*)P_0)[3]);
						*(int*)(&obj) = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr2).Target)).ToPointer();
						System.Runtime.CompilerServices.Unsafe.SkipInit(out gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E obj2);
						try
						{
							gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* ptr2 = &obj2;
							IntPtr intPtr3 = new IntPtr((void*)(int)((uint*)P_0)[1]);
							*(int*)(&obj2) = (int)((IntPtr)GCHandle.Alloc(((XPathNodeIterator)((GCHandle)intPtr3).Target).Current.CreateNavigator())).ToPointer();
						}
						catch
						{
							//try-fault
							___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bdtor_007D), pThis);
							throw;
						}
						ptr3 = Node_002E_007Bctor_007D(ptr, &obj2, &obj);
					}
					else
					{
						ptr3 = null;
					}
				}
				catch
				{
					//try-fault
					delete(ptr);
					throw;
				}
				*(int*)P_1 = (int)ptr3;
				num = 1u;
				return P_1;
			}
			catch
			{
				//try-fault
				if ((num & 1) != 0)
				{
					num &= 0xFFFFFFFEu;
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), P_1);
				}
				throw;
			}
		}
		*(int*)P_1 = 0;
		try
		{
			num = 1u;
			return P_1;
		}
		catch
		{
			//try-fault
			if ((num & 1) != 0)
			{
				num &= 0xFFFFFFFEu;
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), P_1);
			}
			throw;
		}
	}

	internal unsafe static void List_002EReset(List* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)((uint*)P_0)[2]);
		XPathNodeIterator target = ((XPathNodeIterator)((GCHandle)intPtr).Target).Clone();
		IntPtr intPtr2 = new IntPtr((void*)(int)((uint*)P_0)[1]);
		GCHandle gCHandle = (GCHandle)intPtr2;
		gCHandle.Target = target;
	}

	internal unsafe static void List_002ERelease(List* P_0)
	{
		if (P_0 != null)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, uint, void*>)(int)(*(uint*)(*(int*)P_0 + 16)))((nint)P_0, 1u);
		}
	}

	internal unsafe static Value* Value_002E_007Bctor_007D(Value* P_0, global::gcroot_003CSystem_003A_003AString_0020_005E_003E* text)
	{
		try
		{
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IValue_0040Marshaler_0040_00406B_0040));
			try
			{
				System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7Value_0040_00406B_0040));
				IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)text));
				((int*)P_0)[1] = (int)Marshal.StringToHGlobalAnsi((string)((GCHandle)intPtr).Target).ToPointer();
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<IValue*, void>)(&Marshaler_002EIValue_002E_007Bdtor_007D), P_0);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<global::gcroot_003CSystem_003A_003AString_0020_005E_003E*, void>)(&gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D), text);
			throw;
		}
		IntPtr intPtr2 = new IntPtr((void*)(int)(*(uint*)text));
		((GCHandle)intPtr2).Free();
		*(int*)text = 0;
		return P_0;
	}

	internal unsafe static void* Value_002E__vecDelDtor(Value* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			Value* ptr = (Value*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 8u, *(int*)ptr, (delegate*<void*, void>)(delegate*<Value*, void>)(&Value_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete_005B_005D(ptr);
			}
			return ptr;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7Value_0040_00406B_0040));
		try
		{
			Marshal.FreeHGlobal((IntPtr)(void*)(int)((uint*)P_0)[1]);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<IValue*, void>)(&Marshaler_002EIValue_002E_007Bdtor_007D), P_0);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IValue_0040Marshaler_0040_00406B_0040));
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static sbyte* Value_002EGetText(Value* P_0)
	{
		return (sbyte*)(int)((uint*)P_0)[1];
	}

	internal unsafe static void Value_002ERelease(Value* P_0)
	{
		if (P_0 != null)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, uint, void*>)(int)(*(uint*)(*(int*)P_0 + 8)))((nint)P_0, 1u);
		}
	}

	internal unsafe static void Value_002E_007Bdtor_007D(Value* P_0)
	{
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7Value_0040_00406B_0040));
		try
		{
			Marshal.FreeHGlobal((IntPtr)(void*)(int)((uint*)P_0)[1]);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<IValue*, void>)(&Marshaler_002EIValue_002E_007Bdtor_007D), P_0);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7IValue_0040Marshaler_0040_00406B_0040));
	}

	[SpecialName]
	internal unsafe static global::gcroot_003CSystem_003A_003AString_0020_005E_003E* gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bctor_007D(global::gcroot_003CSystem_003A_003AString_0020_005E_003E* P_0, global::gcroot_003CSystem_003A_003AString_0020_005E_003E* r)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)r));
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(((GCHandle)intPtr).Target)).ToPointer();
		return P_0;
	}

	[DebuggerStepThrough]
	internal unsafe static void gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D(global::gcroot_003CSystem_003A_003AString_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}

	internal unsafe static string gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_002EP_0024AAVString_0040System_0040_0040(global::gcroot_003CSystem_003A_003AString_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		return (string)((GCHandle)intPtr).Target;
	}

	[DebuggerStepThrough]
	internal unsafe static void gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E_002E_007Bdtor_007D(gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNodeIterator_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool _003CCrtImplementationDetails_003E_002ENativeDll_002EIsSafeForManagedCode()
	{
		if (!(__native_dllmain_reason != uint.MaxValue))
		{
			return true;
		}
		if (__native_vcclrit_reason != uint.MaxValue)
		{
			return true;
		}
		int num = ((__native_dllmain_reason != 1 && __native_dllmain_reason != 0) ? 1 : 0);
		return (byte)num != 0;
	}

	internal unsafe static int _003CCrtImplementationDetails_003E_002EDefaultDomain_002EDoNothing(void* cookie)
	{
		GC.KeepAlive(int.MaxValue);
		return 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool _003CCrtImplementationDetails_003E_002EDefaultDomain_002EHasPerProcess()
	{
		if (_003FhasPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A == (_003CCrtImplementationDetails_003E.TriBool.State)2)
		{
			void** ptr = (void**)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xc_mp_a);
			if (System.Runtime.CompilerServices.Unsafe.IsAddressLessThan(ref _003FA0x176e81b8_002E__xc_mp_a, ref _003FA0x176e81b8_002E__xc_mp_z))
			{
				do
				{
					if (*(int*)ptr == 0)
					{
						ptr = (void**)((byte*)ptr + 4);
						continue;
					}
					_003FhasPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A = (_003CCrtImplementationDetails_003E.TriBool.State)(-1);
					return true;
				}
				while (ptr < System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xc_mp_z));
			}
			_003FhasPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A = (_003CCrtImplementationDetails_003E.TriBool.State)0;
			return false;
		}
		return _003FhasPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A == (_003CCrtImplementationDetails_003E.TriBool.State)(-1);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool _003CCrtImplementationDetails_003E_002EDefaultDomain_002EHasNative()
	{
		if (_003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A == (_003CCrtImplementationDetails_003E.TriBool.State)2)
		{
			void** ptr = (void**)System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xi_a);
			if (System.Runtime.CompilerServices.Unsafe.IsAddressLessThan(ref __xi_a, ref __xi_z))
			{
				do
				{
					if (*(int*)ptr == 0)
					{
						ptr = (void**)((byte*)ptr + 4);
						continue;
					}
					_003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A = (_003CCrtImplementationDetails_003E.TriBool.State)(-1);
					return true;
				}
				while (ptr < System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xi_z));
			}
			void** ptr2 = (void**)System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xc_a);
			if (System.Runtime.CompilerServices.Unsafe.IsAddressLessThan(ref __xc_a, ref __xc_z))
			{
				do
				{
					if (*(int*)ptr2 == 0)
					{
						ptr2 = (void**)((byte*)ptr2 + 4);
						continue;
					}
					_003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A = (_003CCrtImplementationDetails_003E.TriBool.State)(-1);
					return true;
				}
				while (ptr2 < System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xc_z));
			}
			_003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A = (_003CCrtImplementationDetails_003E.TriBool.State)0;
			return false;
		}
		return _003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A == (_003CCrtImplementationDetails_003E.TriBool.State)(-1);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool _003CCrtImplementationDetails_003E_002EDefaultDomain_002ENeedsInitialization()
	{
		int num = (((_003CCrtImplementationDetails_003E_002EDefaultDomain_002EHasPerProcess() && !_003FInitializedPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA) || (_003CCrtImplementationDetails_003E_002EDefaultDomain_002EHasNative() && !_003FInitializedNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA && __native_startup_state == (global::__enative_startup_state)0)) ? 1 : 0);
		return (byte)num != 0;
	}

	internal unsafe static void _003CCrtImplementationDetails_003E_002EDefaultDomain_002EInitialize()
	{
		_003CCrtImplementationDetails_003E_002EDoCallBackInDefaultDomain((delegate* unmanaged[Stdcall, Stdcall]<void*, int>)__unep_0040_003FDoNothing_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z, null);
	}

	internal static void _003FA0x176e81b8_002E_003F_003F__E_003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA_0040_0040YMXXZ()
	{
		_003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA = 0;
	}

	internal static void _003FA0x176e81b8_002E_003F_003F__E_003FUninitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA_0040_0040YMXXZ()
	{
		_003FUninitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA = 0;
	}

	internal static void _003FA0x176e81b8_002E_003F_003F__E_003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA_0040_0040YMXXZ()
	{
		_003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA = false;
	}

	internal static void _003FA0x176e81b8_002E_003F_003F__E_003FInitializedVtables_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A_0040_0040YMXXZ()
	{
		_003FInitializedVtables_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)0;
	}

	internal static void _003FA0x176e81b8_002E_003F_003F__E_003FInitializedNative_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A_0040_0040YMXXZ()
	{
		_003FInitializedNative_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)0;
	}

	internal static void _003FA0x176e81b8_002E_003F_003F__E_003FInitializedPerProcess_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A_0040_0040YMXXZ()
	{
		_003FInitializedPerProcess_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)0;
	}

	internal static void _003FA0x176e81b8_002E_003F_003F__E_003FInitializedPerAppDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A_0040_0040YMXXZ()
	{
		_003FInitializedPerAppDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)0;
	}

	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeVtables(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0, "The C++ module failed to load during vtable initialization.\n");
		_003FInitializedVtables_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)1;
		_initterm_m((delegate*<void*>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xi_vt_a), (delegate*<void*>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xi_vt_z));
		_003FInitializedVtables_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)2;
	}

	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeDefaultAppDomain(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0, "The C++ module failed to load while attempting to initialize the default appdomain.\n");
		_003CCrtImplementationDetails_003E_002EDefaultDomain_002EInitialize();
	}

	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeNative(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0, "The C++ module failed to load during native initialization.\n");
		__security_init_cookie();
		_003FInitializedNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA = true;
		if (!_003CCrtImplementationDetails_003E_002ENativeDll_002EIsSafeForManagedCode())
		{
			_amsg_exit(33);
		}
		if (__native_startup_state == (global::__enative_startup_state)1)
		{
			_amsg_exit(33);
		}
		else if (__native_startup_state == (global::__enative_startup_state)0)
		{
			_003FInitializedNative_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)1;
			__native_startup_state = (global::__enative_startup_state)1;
			if (_initterm_e((delegate* unmanaged[Cdecl, Cdecl]<int>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xi_a), (delegate* unmanaged[Cdecl, Cdecl]<int>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xi_z)) != 0)
			{
				_003CCrtImplementationDetails_003E_002EThrowModuleLoadException(gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_002EP_0024AAVString_0040System_0040_0040((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0));
			}
			_initterm((delegate* unmanaged[Cdecl, Cdecl]<void>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xc_a), (delegate* unmanaged[Cdecl, Cdecl]<void>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref __xc_z));
			__native_startup_state = (global::__enative_startup_state)2;
			_003FInitializedNativeFromCCTOR_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA = true;
			_003FInitializedNative_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)2;
		}
	}

	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializePerProcess(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0, "The C++ module failed to load during process initialization.\n");
		_003FInitializedPerProcess_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)1;
		_initatexit_m();
		_initterm_m((delegate*<void*>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xc_mp_a), (delegate*<void*>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xc_mp_z));
		_003FInitializedPerProcess_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)2;
		_003FInitializedPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA = true;
	}

	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializePerAppDomain(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0, "The C++ module failed to load during appdomain initialization.\n");
		_003FInitializedPerAppDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)1;
		_initatexit_app_domain();
		_initterm_m((delegate*<void*>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xc_ma_a), (delegate*<void*>*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x176e81b8_002E__xc_ma_z));
		_003FInitializedPerAppDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A = (_003CCrtImplementationDetails_003E.Progress.State)2;
	}

	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeUninitializer(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0, "The C++ module failed to load during registration for the unload events.\n");
		_003CCrtImplementationDetails_003E_002ERegisterModuleUninitializer([ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] [PrePrepareMethod] (object source, EventArgs arguments) =>
		{
			if (_003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA != 0 && Interlocked.Exchange(ref _003FUninitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA, 1) == 0)
			{
				bool num = Interlocked.Decrement(ref _003FCount_0040AllDomains_0040_003CCrtImplementationDetails_003E_0040_00402HA) == 0;
				_app_exit_callback();
				if (num)
				{
					_003CCrtImplementationDetails_003E_002ELanguageSupport_002EUninitializeDefaultDomain();
				}
			}
		});
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002E_Initialize(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		_003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA = AppDomain.CurrentDomain.IsDefaultAppDomain();
		if (_003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA)
		{
			_003FEntered_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA = true;
		}
		_003CCrtImplementationDetails_003E_002EDoDllLanguageSupportValidation();
		void* ptr = _getFiberPtrId();
		int num = 0;
		int num2 = 0;
		RuntimeHelpers.PrepareConstrainedRegions();
		try
		{
			while (num2 == 0)
			{
				try
				{
				}
				finally
				{
					IntPtr comparand = (IntPtr)0;
					IntPtr value = (IntPtr)ptr;
					void* ptr2 = (void*)Interlocked.CompareExchange(ref System.Runtime.CompilerServices.Unsafe.As<void*, IntPtr>(ref __native_startup_lock), value, comparand);
					if (ptr2 == null)
					{
						num2 = 1;
					}
					else if (ptr2 == ptr)
					{
						num = 1;
						num2 = 1;
					}
				}
				if (num2 == 0)
				{
					Sleep(1000u);
				}
			}
			if (!_003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA && _003CCrtImplementationDetails_003E_002EDefaultDomain_002ENeedsInitialization())
			{
				_003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeDefaultAppDomain(P_0);
			}
		}
		finally
		{
			if (num == 0)
			{
				IntPtr value2 = (IntPtr)0;
				Interlocked.Exchange(ref System.Runtime.CompilerServices.Unsafe.As<void*, IntPtr>(ref __native_startup_lock), value2);
			}
		}
		_003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeVtables(P_0);
		if (_003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA)
		{
			_003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeNative(P_0);
			_003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializePerProcess(P_0);
		}
		_003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializePerAppDomain(P_0);
		_003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA = 1;
		_003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitializeUninitializer(P_0);
	}

	internal static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EUninitializeAppDomain()
	{
		_app_exit_callback();
	}

	internal unsafe static int _003CCrtImplementationDetails_003E_002ELanguageSupport_002E_UninitializeDefaultDomain(void* cookie)
	{
		_exit_callback();
		_003FInitializedPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA = false;
		if (_003FInitializedNativeFromCCTOR_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA)
		{
			_cexit();
			__native_startup_state = (global::__enative_startup_state)0;
			_003FInitializedNativeFromCCTOR_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA = false;
		}
		_003FInitializedNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA = false;
		return 0;
	}

	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EUninitializeDefaultDomain()
	{
		if (_003FEntered_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA)
		{
			if (AppDomain.CurrentDomain.IsDefaultAppDomain())
			{
				_003CCrtImplementationDetails_003E_002ELanguageSupport_002E_UninitializeDefaultDomain(null);
			}
			else
			{
				_003CCrtImplementationDetails_003E_002EDoCallBackInDefaultDomain((delegate* unmanaged[Stdcall, Stdcall]<void*, int>)__unep_0040_003F_UninitializeDefaultDomain_0040LanguageSupport_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z, null);
			}
		}
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[PrePrepareMethod]
	internal static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EDomainUnload(object source, EventArgs arguments)
	{
		if (_003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA != 0 && Interlocked.Exchange(ref _003FUninitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA, 1) == 0)
		{
			bool num = Interlocked.Decrement(ref _003FCount_0040AllDomains_0040_003CCrtImplementationDetails_003E_0040_00402HA) == 0;
			_app_exit_callback();
			if (num)
			{
				_003CCrtImplementationDetails_003E_002ELanguageSupport_002EUninitializeDefaultDomain();
			}
		}
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002ECleanup(_003CCrtImplementationDetails_003E.LanguageSupport* P_0, Exception innerException)
	{
		try
		{
			bool flag = Interlocked.Decrement(ref _003FCount_0040AllDomains_0040_003CCrtImplementationDetails_003E_0040_00402HA) == 0;
			_003CCrtImplementationDetails_003E_002ELanguageSupport_002EUninitializeAppDomain();
			if (flag)
			{
				_003CCrtImplementationDetails_003E_002ELanguageSupport_002EUninitializeDefaultDomain();
			}
		}
		catch (Exception ex)
		{
			_003CCrtImplementationDetails_003E_002EThrowNestedModuleLoadException(innerException, ex);
		}
		catch
		{
			_003CCrtImplementationDetails_003E_002EThrowNestedModuleLoadException(innerException, null);
		}
	}

	[DebuggerStepThrough]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitialize(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		bool flag = false;
		RuntimeHelpers.PrepareConstrainedRegions();
		try
		{
			gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0, "The C++ module failed to load.\n");
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				Interlocked.Increment(ref _003FCount_0040AllDomains_0040_003CCrtImplementationDetails_003E_0040_00402HA);
				flag = true;
			}
			_003CCrtImplementationDetails_003E_002ELanguageSupport_002E_Initialize(P_0);
		}
		catch (Exception ex)
		{
			if (flag)
			{
				_003CCrtImplementationDetails_003E_002ELanguageSupport_002ECleanup(P_0, ex);
			}
			_003CCrtImplementationDetails_003E_002EThrowModuleLoadException(gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_002EP_0024AAVString_0040System_0040_0040((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0), ex);
		}
		catch
		{
			if (flag)
			{
				_003CCrtImplementationDetails_003E_002ELanguageSupport_002ECleanup(P_0, null);
			}
			_003CCrtImplementationDetails_003E_002EThrowModuleLoadException(gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_002EP_0024AAVString_0040System_0040_0040((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0), null);
		}
	}

	[DebuggerStepThrough]
	static unsafe _003CModule_003E()
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _003CCrtImplementationDetails_003E.LanguageSupport languageSupport);
		_003CCrtImplementationDetails_003E_002ELanguageSupport_002E_007Bctor_007D(&languageSupport);
		try
		{
			_003CCrtImplementationDetails_003E_002ELanguageSupport_002EInitialize(&languageSupport);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<_003CCrtImplementationDetails_003E.LanguageSupport*, void>)(&_003CCrtImplementationDetails_003E_002ELanguageSupport_002E_007Bdtor_007D), &languageSupport);
			throw;
		}
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)(&languageSupport));
	}

	internal unsafe static _003CCrtImplementationDetails_003E.LanguageSupport* _003CCrtImplementationDetails_003E_002ELanguageSupport_002E_007Bctor_007D(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bctor_007D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0);
		return P_0;
	}

	internal unsafe static void _003CCrtImplementationDetails_003E_002ELanguageSupport_002E_007Bdtor_007D(_003CCrtImplementationDetails_003E.LanguageSupport* P_0)
	{
		gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bdtor_007D((global::gcroot_003CSystem_003A_003AString_0020_005E_003E*)P_0);
	}

	[DebuggerStepThrough]
	internal unsafe static global::gcroot_003CSystem_003A_003AString_0020_005E_003E* gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_007Bctor_007D(global::gcroot_003CSystem_003A_003AString_0020_005E_003E* P_0)
	{
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(null)).ToPointer();
		return P_0;
	}

	[DebuggerStepThrough]
	internal unsafe static global::gcroot_003CSystem_003A_003AString_0020_005E_003E* gcroot_003CSystem_003A_003AString_0020_005E_003E_002E_003D(global::gcroot_003CSystem_003A_003AString_0020_005E_003E* P_0, string t)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		GCHandle gCHandle = (GCHandle)intPtr;
		gCHandle.Target = t;
		return P_0;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	internal unsafe static void ___CxxCallUnwindDtor(delegate*<void*, void> pDtor, void* pThis)
	{
		try
		{
			pDtor(pThis);
		}
		catch when (__FrameUnwindFilter((global::_EXCEPTION_POINTERS*)Marshal.GetExceptionPointers()) != 0)
		{
		}
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	internal unsafe static void __ehvec_dtor(void* ptr, uint size, int count, delegate*<void*, void> pDtor)
	{
		int num = 0;
		ptr = (int)size * count + (byte*)ptr;
		try
		{
			while (true)
			{
				count--;
				if (count < 0)
				{
					break;
				}
				ptr = (byte*)ptr - (int)size;
				pDtor(ptr);
			}
			num = 1;
		}
		finally
		{
			if (num == 0)
			{
				__ArrayUnwind(ptr, size, count, pDtor);
			}
		}
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	internal unsafe static int _003FA0x21b573b5_002EArrayUnwindFilter(global::_EXCEPTION_POINTERS* pExPtrs)
	{
		if (*(int*)(int)(*(uint*)pExPtrs) != -529697949)
		{
			return 0;
		}
		terminate();
		return 0;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	internal unsafe static void __ArrayUnwind(void* ptr, uint size, int count, delegate*<void*, void> pDtor)
	{
		try
		{
			while (true)
			{
				count--;
				if (count >= 0)
				{
					ptr = (byte*)ptr - (int)size;
					pDtor(ptr);
					continue;
				}
				break;
			}
		}
		catch when (_003FA0x21b573b5_002EArrayUnwindFilter((global::_EXCEPTION_POINTERS*)Marshal.GetExceptionPointers()) != 0)
		{
		}
	}

	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002EAtExitLock_002EInitialize()
	{
		object value = new object();
		System.Runtime.CompilerServices.Unsafe.As<_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E, int>(ref _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A) = 0;
		_003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002ESet((_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A), value);
	}

	[DebuggerStepThrough]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool _003CCrtImplementationDetails_003E_002EAtExitLock_002EIsInitialized()
	{
		return _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002EGet((_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A)) != null;
	}

	internal unsafe static void _003FA0x23130d9f_002E_003F_003F__E_003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A_0040_0040YMXXZ()
	{
		_atexit_m_appdomain((delegate*<void>)(&_003FA0x23130d9f_002E_003F_003F__F_003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A_0040_0040YMXXZ));
	}

	internal unsafe static void _003FA0x23130d9f_002E_003F_003F__F_003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A_0040_0040YMXXZ()
	{
		_003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002E_007Bdtor_007D((_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A));
	}

	[DebuggerStepThrough]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool _003FA0x23130d9f_002E__global_lock()
	{
		bool result = false;
		if (_003CCrtImplementationDetails_003E_002EAtExitLock_002EIsInitialized())
		{
			Monitor.Enter(_003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002EGet((_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A)));
			result = true;
		}
		return result;
	}

	[DebuggerStepThrough]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool _003FA0x23130d9f_002E__global_unlock()
	{
		bool result = false;
		if (_003CCrtImplementationDetails_003E_002EAtExitLock_002EIsInitialized())
		{
			Monitor.Exit(_003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002EGet((_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A)));
			result = true;
		}
		return result;
	}

	[DebuggerStepThrough]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool _003FA0x23130d9f_002E__alloc_global_lock()
	{
		if (!_003CCrtImplementationDetails_003E_002EAtExitLock_002EIsInitialized())
		{
			_003CCrtImplementationDetails_003E_002EAtExitLock_002EInitialize();
		}
		return _003CCrtImplementationDetails_003E_002EAtExitLock_002EIsInitialized();
	}

	internal unsafe static int _atexit_helper(delegate*<void> func, uint* __pexit_list_size, delegate*<void>** __ponexitend, delegate*<void>** __ponexitbegin)
	{
		delegate*<void> delegate_002A = null;
		if (func == (delegate*<void>)null)
		{
			return -1;
		}
		if (_003FA0x23130d9f_002E__global_lock())
		{
			try
			{
				if (*__pexit_list_size - 1 < (uint)(*(int*)__ponexitend - *(int*)__ponexitbegin >>> 2))
				{
					try
					{
						uint num = *__pexit_list_size * 4;
						uint num2 = ((num >= 2048) ? 2048u : num);
						IntPtr cb = new IntPtr((int)(num + num2));
						IntPtr pv = new IntPtr((void*)(int)(*(uint*)__ponexitbegin));
						IntPtr intPtr = Marshal.ReAllocHGlobal(pv, cb);
						*(int*)__ponexitend = (int)(*(int*)__ponexitend + ((byte*)intPtr.ToPointer() - *(int*)__ponexitbegin));
						*(int*)__ponexitbegin = (int)intPtr.ToPointer();
						uint num3 = *__pexit_list_size;
						uint num4 = ((512 >= num3) ? num3 : 512u);
						*__pexit_list_size = num3 + num4;
					}
					catch (OutOfMemoryException)
					{
						IntPtr cb2 = new IntPtr((int)(*__pexit_list_size * 4 + 8));
						IntPtr pv2 = new IntPtr((void*)(int)(*(uint*)__ponexitbegin));
						IntPtr intPtr2 = Marshal.ReAllocHGlobal(pv2, cb2);
						*(int*)__ponexitend = (int)(*(int*)__ponexitend + ((byte*)intPtr2.ToPointer() - *(int*)__ponexitbegin));
						*(int*)__ponexitbegin = (int)intPtr2.ToPointer();
						*__pexit_list_size += 4u;
					}
				}
				*(int*)(int)(*(uint*)__ponexitend) = (int)func;
				*(int*)__ponexitend += 4;
				delegate_002A = func;
			}
			catch (OutOfMemoryException)
			{
			}
			finally
			{
				_003FA0x23130d9f_002E__global_unlock();
			}
			if (delegate_002A != (delegate*<void>)null)
			{
				return 0;
			}
		}
		return -1;
	}

	internal unsafe static void _exit_callback()
	{
		delegate*<void>* ptr = (delegate*<void>*)_decode_pointer(_003FA0x23130d9f_002E__onexitbegin_m);
		delegate*<void>* ptr2 = (delegate*<void>*)_decode_pointer(_003FA0x23130d9f_002E__onexitend_m);
		if (ptr == (delegate*<void>*)(-1) || ptr == null || ptr2 == null)
		{
			return;
		}
		ptr2 = (delegate*<void>*)((byte*)ptr2 - 4);
		if (ptr2 >= ptr)
		{
			do
			{
				uint num = *(uint*)ptr2;
				if ((void*)(int)num != _encoded_null())
				{
					((delegate*<void>)_decode_pointer((void*)(int)num))();
				}
				ptr2 = (delegate*<void>*)((byte*)ptr2 - 4);
			}
			while (ptr2 >= ptr);
		}
		IntPtr hglobal = new IntPtr(ptr);
		Marshal.FreeHGlobal(hglobal);
	}

	[DebuggerStepThrough]
	internal unsafe static int _initatexit_m()
	{
		int result = 0;
		if (_003FA0x23130d9f_002E__alloc_global_lock())
		{
			_003FA0x23130d9f_002E__onexitbegin_m = (delegate*<void>*)_encode_pointer(Marshal.AllocHGlobal(128).ToPointer());
			_003FA0x23130d9f_002E__onexitend_m = _003FA0x23130d9f_002E__onexitbegin_m;
			_003FA0x23130d9f_002E__exit_list_size = 32u;
			result = 1;
		}
		return result;
	}

	[DebuggerStepThrough]
	internal unsafe static int _initatexit_app_domain()
	{
		if (_003FA0x23130d9f_002E__alloc_global_lock())
		{
			__onexitbegin_app_domain = (delegate*<void>*)_encode_pointer(Marshal.AllocHGlobal(128).ToPointer());
			__onexitend_app_domain = __onexitbegin_app_domain;
			__exit_list_size_app_domain = 32u;
		}
		return 1;
	}

	internal unsafe static void _app_exit_callback()
	{
		delegate*<void>* ptr = (delegate*<void>*)_decode_pointer(__onexitbegin_app_domain);
		delegate*<void>* ptr2 = (delegate*<void>*)_decode_pointer(__onexitend_app_domain);
		if (ptr == (delegate*<void>*)(-1) || ptr == null || ptr2 == null)
		{
			return;
		}
		try
		{
			while (true)
			{
				ptr2 = (delegate*<void>*)((byte*)ptr2 - 4);
				if (ptr2 >= ptr)
				{
					if ((void*)(*(int*)ptr2) != _encoded_null())
					{
						((delegate*<void>)_decode_pointer((void*)(int)(*(uint*)ptr2)))();
					}
					continue;
				}
				break;
			}
		}
		finally
		{
			IntPtr hglobal = new IntPtr(ptr);
			Marshal.FreeHGlobal(hglobal);
		}
	}

	[DebuggerStepThrough]
	internal unsafe static int _atexit_m_appdomain(delegate*<void> func)
	{
		delegate*<void>* ptr = (delegate*<void>*)_decode_pointer(__onexitbegin_app_domain);
		delegate*<void>* ptr2 = (delegate*<void>*)_decode_pointer(__onexitend_app_domain);
		int result = _atexit_helper((delegate*<void>)_encode_pointer(func), (uint*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref __exit_list_size_app_domain), &ptr2, &ptr);
		__onexitbegin_app_domain = (delegate*<void>*)_encode_pointer(ptr);
		__onexitend_app_domain = (delegate*<void>*)_encode_pointer(ptr2);
		return result;
	}

	[DebuggerStepThrough]
	internal unsafe static object _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002EGet(_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E* P_0)
	{
		ValueType valueType = _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002E_handle(P_0);
		if (valueType != null)
		{
			return ((GCHandle)valueType).Target;
		}
		return null;
	}

	internal unsafe static void _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002E_007Bdtor_007D(_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E* P_0)
	{
		ValueType valueType = _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002E_handle(P_0);
		if (valueType != null)
		{
			((GCHandle)valueType).Free();
			*(int*)P_0 = 0;
		}
	}

	[DebuggerStepThrough]
	internal unsafe static ValueType _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002E_handle(_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			IntPtr value = new IntPtr((void*)(int)num);
			return GCHandle.FromIntPtr(value);
		}
		return null;
	}

	[DebuggerStepThrough]
	internal unsafe static void _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002ESet(_003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E* P_0, object value)
	{
		ValueType valueType = _003CCrtImplementationDetails_003E_002EHandle_003CSystem_003A_003AObject_0020_005E_003E_002E_handle(P_0);
		if (valueType == null)
		{
			valueType = GCHandle.Alloc(value);
			*(int*)P_0 = (int)GCHandle.ToIntPtr((GCHandle)valueType).ToPointer();
		}
		else
		{
			((GCHandle)valueType).Target = value;
		}
	}

	[DebuggerStepThrough]
	internal unsafe static int _initterm_e(delegate* unmanaged[Cdecl, Cdecl]<int>* pfbegin, delegate* unmanaged[Cdecl, Cdecl]<int>* pfend)
	{
		int num = 0;
		if (pfbegin < pfend)
		{
			while (num == 0)
			{
				uint num2 = *(uint*)pfbegin;
				if (num2 != 0)
				{
					num = ((delegate* unmanaged[Cdecl, Cdecl]<int>)(int)num2)();
				}
				pfbegin = (delegate* unmanaged[Cdecl, Cdecl]<int>*)((byte*)pfbegin + 4);
				if (pfbegin >= pfend)
				{
					break;
				}
			}
		}
		return num;
	}

	[DebuggerStepThrough]
	internal unsafe static void _initterm(delegate* unmanaged[Cdecl, Cdecl]<void>* pfbegin, delegate* unmanaged[Cdecl, Cdecl]<void>* pfend)
	{
		if (pfbegin >= pfend)
		{
			return;
		}
		do
		{
			uint num = *(uint*)pfbegin;
			if (num != 0)
			{
				((delegate* unmanaged[Cdecl, Cdecl]<void>)(int)num)();
			}
			pfbegin = (delegate* unmanaged[Cdecl, Cdecl]<void>*)((byte*)pfbegin + 4);
		}
		while (pfbegin < pfend);
	}

	[DebuggerStepThrough]
	internal static ModuleHandle _003CCrtImplementationDetails_003E_002EThisModule_002EHandle()
	{
		return typeof(_003CCrtImplementationDetails_003E.ThisModule).Module.ModuleHandle;
	}

	[DebuggerStepThrough]
	internal unsafe static void _initterm_m(delegate*<void*>* pfbegin, delegate*<void*>* pfend)
	{
		if (pfbegin >= pfend)
		{
			return;
		}
		do
		{
			uint num = *(uint*)pfbegin;
			if (num != 0)
			{
				_003CCrtImplementationDetails_003E_002EThisModule_002EResolveMethod_003Cvoid_0020const_0020_002A_0020__clrcall_0028void_0029_003E((delegate*<void*>)(int)num)();
			}
			pfbegin = (delegate*<void*>*)((byte*)pfbegin + 4);
		}
		while (pfbegin < pfend);
	}

	[DebuggerStepThrough]
	internal unsafe static delegate*<void*> _003CCrtImplementationDetails_003E_002EThisModule_002EResolveMethod_003Cvoid_0020const_0020_002A_0020__clrcall_0028void_0029_003E(delegate*<void*> methodToken)
	{
		return (delegate*<void*>)_003CCrtImplementationDetails_003E_002EThisModule_002EHandle().ResolveMethodHandle((int)methodToken).GetFunctionPointer().ToPointer();
	}

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern uint ReverseEndianness(uint P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern Direct3D* _003FA0x749b4e39_002EDirect3DCreate9(uint P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern uint Direct3D_002ERelease();

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int Direct3D_002ECreateDevice(uint P_0, _D3DDEVTYPE P_1, void* P_2, uint P_3, _D3DPRESENT_PARAMETERS_* P_4, D3DDevice** P_5);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern uint D3DDevice_002ERelease(D3DDevice* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern uint D3DResource_002ERelease(D3DResource* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int D3DTexture_002ELockRect(D3DTexture* P_0, uint P_1, _D3DLOCKED_RECT* P_2, tagRECT* P_3, uint P_4);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int D3DTexture_002EUnlockRect(D3DTexture* P_0, uint P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern SimpleBuffer* BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002E_007Bctor_007D(SimpleBuffer* P_0, uint P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002EReset(SimpleBuffer* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002EAppend(SimpleBuffer* P_0, void* P_1, uint P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* XenonBundler_002E__delDtor(XenonBundler* P_0, uint P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* BinaryAssetBuilder_002ETextureCompiler_002ESimpleBuffer_002E__delDtor(SimpleBuffer* P_0, uint P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern sbyte* getErrorString(NV_ERROR_CODE P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* @new(uint P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern int __CxxQueryExceptionSize();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int __CxxDetectRethrow(void* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int XGCopySurface(void* P_0, int P_1, uint P_2, uint P_3, _D3DFORMAT P_4, tagPOINT* P_5, void* P_6, int P_7, _D3DFORMAT P_8, tagRECT* P_9, uint P_10, float P_11);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int fclose(_iobuf* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int fseek(_iobuf* P_0, int P_1, int P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern uint strtoul(sbyte* P_0, sbyte** P_1, int P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* realloc(void* P_0, uint P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void XGGetTextureDesc(D3DBaseTexture* P_0, uint P_1, _XGTEXTURE_DESC* P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void __CxxUnregisterExceptionObject(void* P_0, int P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void delete(void* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int __CxxExceptionFilter(void* P_0, void* P_1, int P_2, void* P_3);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern uint fread(void* P_0, uint P_1, uint P_2, _iobuf* P_3);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void delete_005B_005D(void* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern NV_ERROR_CODE nvDDS_002EnvDXTcompress(RGBAMipMappedVolumeMap* P_0, nvCompressionOptions* P_1, delegate* unmanaged[Cdecl, Cdecl]<void*, uint, MIPMapData*, void*, NV_ERROR_CODE> P_2, tagRECT* P_3);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern NV_ERROR_CODE nvDDS_002EnvDXTcompress(nvImage_003Crgba_t_003E* P_0, nvCompressionOptions* P_1, delegate* unmanaged[Cdecl, Cdecl]<void*, uint, MIPMapData*, void*, NV_ERROR_CODE> P_2, tagRECT* P_3);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int __CxxRegisterExceptionObject(void* P_0, void* P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* malloc(uint P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* calloc(uint P_0, uint P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void free(void* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void XenonBundler_002EProcess2DTexture(XenonBundler* P_0, nvImage_003Crgba_t_003E* P_1, sbyte* P_2, [MarshalAs(UnmanagedType.U1)] bool P_3, uint P_4);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void XenonBundler_002EProcess3DTexture(XenonBundler* P_0, nvImage_003Crgba_t_003E* P_1, sbyte* P_2, uint P_3);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void XenonBundler_002EProcessDDS(XenonBundler* P_0, sbyte* P_1, D3DDevice* P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern XenonBundler* XenonBundler_002E_007Bctor_007D(XenonBundler* P_0, uint P_1, uint P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern double ceil(double P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* new_005B_005D(uint P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* memmove(void* P_0, void* P_1, uint P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int D3DXCreateTextureFromFileA(D3DDevice* P_0, sbyte* P_1, D3DTexture** P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* _getFiberPtrId();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void _amsg_exit(int P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void __security_init_cookie();

	[DllImport("", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void Sleep(uint P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void _003CCrtImplementationDetails_003E_002EThrowModuleLoadException(string P_0, Exception P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void _003CCrtImplementationDetails_003E_002EThrowModuleLoadException(string P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void _003CCrtImplementationDetails_003E_002EDoDllLanguageSupportValidation();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void _003CCrtImplementationDetails_003E_002EThrowNestedModuleLoadException(Exception P_0, Exception P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void _003CCrtImplementationDetails_003E_002ERegisterModuleUninitializer(EventHandler P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void _003CCrtImplementationDetails_003E_002EDoCallBackInDefaultDomain(delegate* unmanaged[Stdcall, Stdcall]<void*, int> P_0, void* P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void _cexit();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int __FrameUnwindFilter(global::_EXCEPTION_POINTERS* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern void terminate();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* _encode_pointer(void* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* _decode_pointer(void* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* _encoded_null();
}
