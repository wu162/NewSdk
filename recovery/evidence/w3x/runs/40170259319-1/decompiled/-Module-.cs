using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using _003CCppImplementationDetails_003E;
using _003CCrtImplementationDetails_003E;
using BinaryAssetBuilder.Core;
using Endian;
using Marshaler;
using RNA;
using Relo;
using SageBinaryData;
using SageMathOnly.Math;
using W3DMeshProcessor;
using eastl;
using std;

internal class _003CModule_003E
{
	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040GHFPNOJB_0040bad_003F5allocation_003F_0024AA_0040/* Not supported: data(62 61 64 20 61 6C 6C 6F 63 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040/* Not supported: data(45 41 53 54 4C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0FB_0040_0024_0024CBD _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040/* Not supported: data(43 3A 5C 70 72 6F 6A 65 63 74 73 5C 52 61 33 5C 50 72 6F 64 75 63 74 69 6F 6E 5C 63 6F 64 65 5C 65 78 74 65 72 6E 61 6C 5C 45 41 53 54 4C 5C 31 2E 30 36 2E 30 31 5C 69 6E 63 6C 75 64 65 5C 45 41 53 54 4C 2F 61 6C 6C 6F 63 61 74 6F 72 2E 68 00) */;

	internal static bool _003FA0x0e388f25_002EinitTable/* Not supported: data(00) */;

	internal unsafe static delegate*<void> _003FA0x0e388f25_002EinitTable_0024initializer_0024/* Not supported: data(0B 00 00 06) */;

	internal static _0024ArrayType_0024_0024_0024BY0BAA_0040M _003FA0x0e388f25_002Efiltertable/* Not supported: data(77 CC 2B 32 95 BF D6 33 BD 37 86 35 AC C5 27 37 17 B7 D1 38 6F 12 83 3A 0A D7 23 3C CD CC CC 3D 00 00 80 3F 00 00 20 41 00 00 C8 42 00 00 7A 44 00 40 1C 46 00 50 C3 47 00 24 74 49 80 96 18 4B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) */;

	internal unsafe static sbyte* std_002E_003FA0x0e388f25_002E_bad_alloc_Message/* Not supported: data(6C E1 00 10) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07JPHEIBJD_0040W3DMesh_003F_0024AA_0040/* Not supported: data(57 33 44 4D 65 73 68 00) */;

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

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040PCEPIOMH_0040BaseRenderAssetType_003F_0024AA_0040/* Not supported: data(42 61 73 65 52 65 6E 64 65 72 41 73 73 65 74 54 79 70 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040GILDMIHG_0040BoneInfluence_003F_0024AA_0040/* Not supported: data(42 6F 6E 65 49 6E 66 6C 75 65 6E 63 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08NMAPBDDP_0040Triangle_003F_0024AA_0040/* Not supported: data(54 72 69 61 6E 67 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09JBPNNFEK_0040BoxMinMax_003F_0024AA_0040/* Not supported: data(42 6F 78 4D 69 6E 4D 61 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06JBMJMGAL_0040Sphere_003F_0024AA_0040/* Not supported: data(53 70 68 65 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040ECHPOGAB_0040FXShaderConstant_003F_0024AA_0040/* Not supported: data(46 58 53 68 61 64 65 72 43 6F 6E 73 74 61 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BI_0040_0024_0024CBD _003F_003F_C_0040_0BI_0040NMCKDACM_0040FXShaderConstantTexture_003F_0024AA_0040/* Not supported: data(46 58 53 68 61 64 65 72 43 6F 6E 73 74 61 6E 74 54 65 78 74 75 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040LHFGLMDD_0040FXShaderConstantFloat_003F_0024AA_0040/* Not supported: data(46 58 53 68 61 64 65 72 43 6F 6E 73 74 61 6E 74 46 6C 6F 61 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040CGFKBPNN_0040FXShaderConstantInt_003F_0024AA_0040/* Not supported: data(46 58 53 68 61 64 65 72 43 6F 6E 73 74 61 6E 74 49 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BF_0040_0024_0024CBD _003F_003F_C_0040_0BF_0040OOBDMKDI_0040FXShaderConstantBool_003F_0024AA_0040/* Not supported: data(46 58 53 68 61 64 65 72 43 6F 6E 73 74 61 6E 74 42 6F 6F 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040EKKFAPIM_0040FXShaderMaterial_003F_0024AA_0040/* Not supported: data(46 58 53 68 61 64 65 72 4D 61 74 65 72 69 61 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07IFDFKAHK_0040AABTree_003F_0024AA_0040/* Not supported: data(41 41 42 54 72 65 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BK_0040_0024_0024CBD _003F_003F_C_0040_0BK_0040GLFGEIED_0040W3DMeshPipelineVertexData_003F_0024AA_0040/* Not supported: data(57 33 44 4D 65 73 68 50 69 70 65 6C 69 6E 65 56 65 72 74 65 78 44 61 74 61 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09EPGLIBAJ_0040SubObject_003F_0024AA_0040/* Not supported: data(53 75 62 4F 62 6A 65 63 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040HFCDOJOG_0040W3DContainer_003F_0024AA_0040/* Not supported: data(57 33 44 43 6F 6E 74 61 69 6E 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040MOBFHGCB_0040W3DHierarchy_003F_0024AA_0040/* Not supported: data(57 33 44 48 69 65 72 61 72 63 68 79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BF_0040_0024_0024CBD _003F_003F_C_0040_0BF_0040CMLEHIBO_0040AnimationChannelBase_003F_0024AA_0040/* Not supported: data(41 6E 69 6D 61 74 69 6F 6E 43 68 61 6E 6E 65 6C 42 61 73 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BH_0040_0024_0024CBD _003F_003F_C_0040_0BH_0040DFMKONBP_0040AnimationChannelScalar_003F_0024AA_0040/* Not supported: data(41 6E 69 6D 61 74 69 6F 6E 43 68 61 6E 6E 65 6C 53 63 61 6C 61 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BL_0040_0024_0024CBD _003F_003F_C_0040_0BL_0040DPPCACKP_0040AnimationChannelQuaternion_003F_0024AA_0040/* Not supported: data(41 6E 69 6D 61 74 69 6F 6E 43 68 61 6E 6E 65 6C 51 75 61 74 65 72 6E 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BM_0040_0024_0024CBD _003F_003F_C_0040_0BM_0040FGGHIKGH_0040AnimationChannelBaseRuntime_003F_0024AA_0040/* Not supported: data(41 6E 69 6D 61 74 69 6F 6E 43 68 61 6E 6E 65 6C 42 61 73 65 52 75 6E 74 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BK_0040_0024_0024CBD _003F_003F_C_0040_0BK_0040BMKPLGAI_0040AnimationChannelTimecoded_003F_0024AA_0040/* Not supported: data(41 6E 69 6D 61 74 69 6F 6E 43 68 61 6E 6E 65 6C 54 69 6D 65 63 6F 64 65 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040LAICBIKO_0040AnimationChannelDelta_003F_0024AA_0040/* Not supported: data(41 6E 69 6D 61 74 69 6F 6E 43 68 61 6E 6E 65 6C 44 65 6C 74 61 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040MNAEJFEK_0040W3DAnimationRuntime_003F_0024AA_0040/* Not supported: data(57 33 44 41 6E 69 6D 61 74 69 6F 6E 52 75 6E 74 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040HAGJEOJN_0040W3DAnimation_003F_0024AA_0040/* Not supported: data(57 33 44 41 6E 69 6D 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040OOFOOKKI_0040W3DCollisionBox_003F_0024AA_0040/* Not supported: data(57 33 44 43 6F 6C 6C 69 73 69 6F 6E 42 6F 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040NHJPPGGC_0040CameraAligned_003F_0024AA_0040/* Not supported: data(43 61 6D 65 72 61 41 6C 69 67 6E 65 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040KHKGDCGM_0040CameraOriented_003F_0024AA_0040/* Not supported: data(43 61 6D 65 72 61 4F 72 69 65 6E 74 65 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06EFAGIKOH_0040Normal_003F_0024AA_0040/* Not supported: data(4E 6F 72 6D 61 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04BGDBHLHN_0040Skin_003F_0024AA_0040/* Not supported: data(53 6B 69 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040LHPCLPIO_0040Orientation_003F_0024AA_0040/* Not supported: data(4F 72 69 65 6E 74 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040KLDABBOG_0040Visibility_003F_0024AA_0040/* Not supported: data(56 69 73 69 62 69 6C 69 74 79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040MPFNLKPH_0040XTranslation_003F_0024AA_0040/* Not supported: data(58 54 72 61 6E 73 6C 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040BCMLGDHC_0040YTranslation_003F_0024AA_0040/* Not supported: data(59 54 72 61 6E 73 6C 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040KPABAPLM_0040ZTranslation_003F_0024AA_0040/* Not supported: data(5A 54 72 61 6E 73 6C 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01GBGANLPD_00400_003F_0024AA_0040/* Not supported: data(30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01DGKLNCNG_0040R_003F_0024AA_0040/* Not supported: data(52 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01BBODEMC_0040G_003F_0024AA_0040/* Not supported: data(47 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01HMGJMAIH_0040B_003F_0024AA_0040/* Not supported: data(42 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01FHEEJDEE_0040A_003F_0024AA_0040/* Not supported: data(41 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04KOACHJEN_0040Type_003F_0024AA_0040/* Not supported: data(54 79 70 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY00_0024_0024CBD _003F_003F_C_0040_00CNPNBAHC_0040_003F_0024AA_0040/* Not supported: data(00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06ODJMJFLN_0040Weight_003F_0024AA_0040/* Not supported: data(57 65 69 67 68 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04CCGIJNFG_0040Bone_003F_0024AA_0040/* Not supported: data(42 6F 6E 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04FABLJDN_0040Name_003F_0024AA_0040/* Not supported: data(4E 61 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01MMEEDKFM_0040X_003F_0024AA_0040/* Not supported: data(58 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01NFFPALBN_0040Y_003F_0024AA_0040/* Not supported: data(59 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01POHCFINO_0040Z_003F_0024AA_0040/* Not supported: data(5A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01ELNMCGJD_0040W_003F_0024AA_0040/* Not supported: data(57 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05LPIJGKJ_0040Value_003F_0024AA_0040/* Not supported: data(56 61 6C 75 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01EJNLAFE_0040P_003F_0024AA_0040/* Not supported: data(50 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05HOHBLJAP_0040Front_003F_0024AA_0040/* Not supported: data(46 72 6F 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04FEOMFELI_0040Back_003F_0024AA_0040/* Not supported: data(42 61 63 6B 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05EHPGHEDK_0040Begin_003F_0024AA_0040/* Not supported: data(42 65 67 69 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05IJGIMMHE_0040Count_003F_0024AA_0040/* Not supported: data(43 6F 75 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03LLNBGIF_0040Min_003F_0024AA_0040/* Not supported: data(4D 69 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03BJDGPCOK_0040Max_003F_0024AA_0040/* Not supported: data(4D 61 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08IEBJFPPC_0040Children_003F_0024AA_0040/* Not supported: data(43 68 69 6C 64 72 65 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05HHBDIHGG_0040Polys_003F_0024AA_0040/* Not supported: data(50 6F 6C 79 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040GFCHFNAP_0040PolyIndices_003F_0024AA_0040/* Not supported: data(50 6F 6C 79 49 6E 64 69 63 65 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04OKAPPFIB_0040Node_003F_0024AA_0040/* Not supported: data(4E 6F 64 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06BCNEEODO_0040TypeId_003F_0024AA_0040/* Not supported: data(54 79 70 65 49 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040LBFPEED_0040ShaderName_003F_0024AA_0040/* Not supported: data(53 68 61 64 65 72 4E 61 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040DDEFCAPO_0040TechniqueName_003F_0024AA_0040/* Not supported: data(54 65 63 68 6E 69 71 75 65 4E 61 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040FPINIBCG_0040TechniqueIndex_003F_0024AA_0040/* Not supported: data(54 65 63 68 6E 69 71 75 65 49 6E 64 65 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09EKDBGGMD_0040Constants_003F_0024AA_0040/* Not supported: data(43 6F 6E 73 74 61 6E 74 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05JABNIMDI_0040Pivot_003F_0024AA_0040/* Not supported: data(50 69 76 6F 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09CLPNCNDO_0040NumFrames_003F_0024AA_0040/* Not supported: data(4E 75 6D 46 72 61 6D 65 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040OCNGDGDE_0040FirstFrame_003F_0024AA_0040/* Not supported: data(46 69 72 73 74 46 72 61 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06GJMFONGK_0040Radius_003F_0024AA_0040/* Not supported: data(52 61 64 69 75 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06NOADDNNE_0040Center_003F_0024AA_0040/* Not supported: data(43 65 6E 74 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01FCMHBHNC_0040V_003F_0024AA_0040/* Not supported: data(56 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03CCFGKGDJ_0040Nrm_003F_0024AA_0040/* Not supported: data(4E 72 6D 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04MPGECEBJ_0040Dist_003F_0024AA_0040/* Not supported: data(44 69 73 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05LAPONLG_0040false_003F_0024AA_0040/* Not supported: data(66 61 6C 73 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040GDEEFGDC_0040BinaryMove_003F_0024AA_0040/* Not supported: data(42 69 6E 61 72 79 4D 6F 76 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05MPDBFDHF_0040Frame_003F_0024AA_0040/* Not supported: data(46 72 61 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01NANMIPIL_0040N_003F_0024AA_0040/* Not supported: data(4E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01GAPBHFFA_0040T_003F_0024AA_0040/* Not supported: data(54 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01GFHCPBMG_0040C_003F_0024AA_0040/* Not supported: data(43 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01JPJNBJEM_0040I_003F_0024AA_0040/* Not supported: data(49 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08JMEOJNII_0040Vertices_003F_0024AA_0040/* Not supported: data(56 65 72 74 69 63 65 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07DKDJFLGE_0040Normals_003F_0024AA_0040/* Not supported: data(4E 6F 72 6D 61 6C 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08DJLAMBOA_0040Tangents_003F_0024AA_0040/* Not supported: data(54 61 6E 67 65 6E 74 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09CPGIGOEP_0040Binormals_003F_0024AA_0040/* Not supported: data(42 69 6E 6F 72 6D 61 6C 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040DJBJPDKO_0040VertexColors_003F_0024AA_0040/* Not supported: data(56 65 72 74 65 78 43 6F 6C 6F 72 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09NIBCOMFM_0040TexCoords_003F_0024AA_0040/* Not supported: data(54 65 78 43 6F 6F 72 64 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040LNBMENPE_0040BoneInfluences_003F_0024AA_0040/* Not supported: data(42 6F 6E 65 49 6E 66 6C 75 65 6E 63 65 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040LMLGBMDA_0040ShadeIndices_003F_0024AA_0040/* Not supported: data(53 68 61 64 65 49 6E 64 69 63 65 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09FDDEOHGF_0040Hierarchy_003F_0024AA_0040/* Not supported: data(48 69 65 72 61 72 63 68 79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09HFKGELHH_0040FrameRate_003F_0024AA_0040/* Not supported: data(46 72 61 6D 65 52 61 74 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08INPNJAIK_0040Channels_003F_0024AA_0040/* Not supported: data(43 68 61 6E 6E 65 6C 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040/* Not supported: data(74 72 75 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040FPKKANKC_0040AllowTimeCoded_003F_0024AA_0040/* Not supported: data(41 6C 6C 6F 77 54 69 6D 65 43 6F 64 65 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040PACLLEOL_0040AllowAdaptiveDelta_003F_0024AA_0040/* Not supported: data(41 6C 6C 6F 77 41 64 61 70 74 69 76 65 44 65 6C 74 61 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05CGKLNCBH_00400_003F4001_003F_0024AA_0040/* Not supported: data(30 2E 30 30 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040OJKBNDPA_0040MaxTranslationError_003F_0024AA_0040/* Not supported: data(4D 61 78 54 72 61 6E 73 6C 61 74 69 6F 6E 45 72 72 6F 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04DKCHIKEL_00400_003F401_003F_0024AA_0040/* Not supported: data(30 2E 30 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040OAPHHBAN_0040MaxRotationError_003F_0024AA_0040/* Not supported: data(4D 61 78 52 6F 74 61 74 69 6F 6E 45 72 72 6F 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040KDDHFEEI_0040MaxVisibilityError_003F_0024AA_0040/* Not supported: data(4D 61 78 56 69 73 69 62 69 6C 69 74 79 45 72 72 6F 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04IBBOIMJ_00400_003F403_003F_0024AA_0040/* Not supported: data(30 2E 30 33 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040DFLPCJGM_0040MaxAdaptiveDeltaError_003F_0024AA_0040/* Not supported: data(4D 61 78 41 64 61 70 74 69 76 65 44 65 6C 74 61 45 72 72 6F 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03HLLJOCDO_00401_003F40_003F_0024AA_0040/* Not supported: data(31 2E 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040NJLHHJDO_0040ForceReductionRate_003F_0024AA_0040/* Not supported: data(46 6F 72 63 65 52 65 64 75 63 74 69 6F 6E 52 61 74 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040GOBLKNOJ_0040CompressionSettings_003F_0024AA_0040/* Not supported: data(43 6F 6D 70 72 65 73 73 69 6F 6E 53 65 74 74 69 6E 67 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06FHBILLDG_0040Hidden_003F_0024AA_0040/* Not supported: data(48 69 64 64 65 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040MBLKBNFB_0040CastShadow_003F_0024AA_0040/* Not supported: data(43 61 73 74 53 68 61 64 6F 77 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040JJFKGPIN_0040GeometryType_003F_0024AA_0040/* Not supported: data(47 65 6F 6D 65 74 72 79 54 79 70 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09NLJGGGGI_0040SortLevel_003F_0024AA_0040/* Not supported: data(53 6F 72 74 4C 65 76 65 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040NJIMOLLO_0040BoundingBox_003F_0024AA_0040/* Not supported: data(42 6F 75 6E 64 69 6E 67 42 6F 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040IGIKKCLE_0040BoundingSphere_003F_0024AA_0040/* Not supported: data(42 6F 75 6E 64 69 6E 67 53 70 68 65 72 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09LCKIAIBL_0040Triangles_003F_0024AA_0040/* Not supported: data(54 72 69 61 6E 67 6C 65 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08IHCECLKE_0040FXShader_003F_0024AA_0040/* Not supported: data(46 58 53 68 61 64 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040 _003FA0x325460ff_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4AnimationChannelType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4AnimationChannelType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B/* Not supported: data(4C E8 00 10 03 00 00 00 58 E8 00 10 04 00 00 00 64 E8 00 10 00 00 00 00 74 E8 00 10 01 00 00 00 84 E8 00 10 02 00 00 00) */;

	internal static float DataInitializers_002E_003FA0x325460ff_002ESECONDS_PER_LOGICFRAME_REAL/*Field data (rva=0x259c4) could not be foundin any section!*/;

	internal static float DataInitializers_002E_003FA0x325460ff_002EMSEC_PER_SECOND/* Not supported: data(00 00 7A 44) */;

	internal static float DataInitializers_002E_003FA0x325460ff_002ERADS_PER_DEGREE/* Not supported: data(35 FA 8E 3C) */;

	internal static float DataInitializers_002E_003FA0x325460ff_002ELOGICFRAMES_PER_SECOND/* Not supported: data(00 00 A0 40) */;

	internal static float DataInitializers_002E_003FA0x325460ff_002ELOGICFRAMES_PER_SECONDS_REAL/* Not supported: data(00 00 A0 40) */;

	internal static float DataInitializers_002E_003FA0x325460ff_002ELOGICFRAMES_PER_MSEC_REAL/* Not supported: data(0A D7 A3 3B) */;

	internal static float DataInitializers_002E_003FA0x325460ff_002EPI/* Not supported: data(DB 0F 49 40) */;

	internal unsafe static delegate*<void> DataInitializers_002E_003FA0x325460ff_002ESECONDS_PER_LOGICFRAME_REAL_0024initializer_0024/* Not supported: data(3B 00 00 06) */;

	internal static uint _003FTypeHash_0040W3DMesh_0040SageBinaryData_0040_00402IB/* Not supported: data(9C 08 8A CD) */;

	internal static uint _003FTypeHash_0040AnimationChannelBase_0040SageBinaryData_0040_00402IB/* Not supported: data(AF AE 48 C2) */;

	internal static uint _003FTypeHash_0040FXShaderConstantFloat_0040SageBinaryData_0040_00402IB/* Not supported: data(09 15 C2 9C) */;

	internal static uint _003FTypeHash_0040AnimationChannelScalar_0040SageBinaryData_0040_00402IB/* Not supported: data(DE 6E 7F D5) */;

	internal static uint _003FTypeHash_0040Color4f_0040SageBinaryData_0040_00402IB/* Not supported: data(61 97 24 CD) */;

	internal unsafe static sbyte* _003FTypeName_0040W3DCollisionBox_0040SageBinaryData_0040_00402PBDB/* Not supported: data(24 E2 00 10) */;

	internal static uint _003FTypeHash_0040LogicRandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(55 6E 41 51) */;

	internal static uint _003FTypeHash_0040TimeRange_0040SageBinaryData_0040_00402IB/* Not supported: data(9F 6D 09 2F) */;

	internal static uint _003FTypeHash_0040FXShaderConstantBool_0040SageBinaryData_0040_00402IB/* Not supported: data(FB FF 15 8F) */;

	internal unsafe static sbyte* _003FTypeName_0040ClientRandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(40 E4 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AnimationChannelBaseRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(8C E2 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Color3f_0040SageBinaryData_0040_00402PBDB/* Not supported: data(A8 E4 00 10) */;

	internal static uint _003FTypeHash_0040MoneyTransaction_0040SageBinaryData_0040_00402IB/* Not supported: data(F5 0B 52 C2) */;

	internal static uint _003FTypeHash_0040SubObject_0040SageBinaryData_0040_00402IB/* Not supported: data(BD 59 32 D7) */;

	internal unsafe static sbyte* _003FTypeName_0040Triangle_0040SageBinaryData_0040_00402PBDB/* Not supported: data(DC E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Color4f_0040SageBinaryData_0040_00402PBDB/* Not supported: data(A0 E4 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Sphere_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C8 E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040W3DHierarchy_0040SageBinaryData_0040_00402PBDB/* Not supported: data(F4 E2 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AnimationChannelQuaternion_0040SageBinaryData_0040_00402PBDB/* Not supported: data(A8 E2 00 10) */;

	internal static uint _003FTypeHash_0040BoxMinMax_0040SageBinaryData_0040_00402IB/* Not supported: data(B0 C2 33 BE) */;

	internal unsafe static sbyte* _003FTypeName_0040AnimationChannelDelta_0040SageBinaryData_0040_00402PBDB/* Not supported: data(58 E2 00 10) */;

	internal static uint _003FTypeHash_0040Triangle_0040SageBinaryData_0040_00402IB/* Not supported: data(9E 12 B0 70) */;

	internal static uint _003FTypeHash_0040W3DAnimationRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(E1 5C 50 D6) */;

	internal static uint _003FTypeHash_0040W3DAnimation_0040SageBinaryData_0040_00402IB/* Not supported: data(E8 5F 45 55) */;

	internal unsafe static sbyte* _003FTypeName_0040BoxMinMax_0040SageBinaryData_0040_00402PBDB/* Not supported: data(D0 E3 00 10) */;

	internal static uint _003FTypeHash_0040RealRange_0040SageBinaryData_0040_00402IB/* Not supported: data(E0 E0 EA F4) */;

	internal unsafe static sbyte* _003FTypeName_0040AnimationChannelBase_0040SageBinaryData_0040_00402PBDB/* Not supported: data(DC E2 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040FXShaderMaterial_0040SageBinaryData_0040_00402PBDB/* Not supported: data(44 E3 00 10) */;

	internal static uint _003FTypeHash_0040FXShaderMaterial_0040SageBinaryData_0040_00402IB/* Not supported: data(53 BD 2B EE) */;

	internal static uint _003FTypeHash_0040RandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(5F B4 AC C7) */;

	internal unsafe static sbyte* _003FTypeName_0040MoneyTransaction_0040SageBinaryData_0040_00402PBDB/* Not supported: data(0C E4 00 10) */;

	internal static uint _003FTypeHash_0040BoneInfluence_0040SageBinaryData_0040_00402IB/* Not supported: data(A9 26 4E C8) */;

	internal static uint _003FTypeHash_0040FXShaderConstantTexture_0040SageBinaryData_0040_00402IB/* Not supported: data(13 CA A5 DA) */;

	internal static uint _003FTypeHash_0040BaseRenderAssetType_0040SageBinaryData_0040_00402IB/* Not supported: data(44 BF 7B E6) */;

	internal unsafe static sbyte* _003FTypeName_0040LogicRandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(58 E4 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040W3DMeshPipelineVertexData_0040SageBinaryData_0040_00402PBDB/* Not supported: data(20 E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040ReplaceTexture_0040SageBinaryData_0040_00402PBDB/* Not supported: data(30 E4 00 10) */;

	internal static uint _003FTypeHash_0040AnimationChannelTimecoded_0040SageBinaryData_0040_00402IB/* Not supported: data(8F 04 44 91) */;

	internal unsafe static sbyte* _003FTypeName_0040W3DAnimationRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(44 E2 00 10) */;

	internal static uint _003FTypeHash_0040AnimationChannelDelta_0040SageBinaryData_0040_00402IB/* Not supported: data(A9 50 3A 51) */;

	internal static uint _003FTypeHash_0040ReplaceTexture_0040SageBinaryData_0040_00402IB/* Not supported: data(00 49 32 8C) */;

	internal unsafe static sbyte* _003FTypeName_0040W3DAnimation_0040SageBinaryData_0040_00402PBDB/* Not supported: data(34 E2 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AnimationChannelTimecoded_0040SageBinaryData_0040_00402PBDB/* Not supported: data(70 E2 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040FXShaderConstantTexture_0040SageBinaryData_0040_00402PBDB/* Not supported: data(9C E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040BaseRenderAssetType_0040SageBinaryData_0040_00402PBDB/* Not supported: data(F8 E3 00 10) */;

	internal static uint _003FTypeHash_0040W3DMeshPipelineVertexData_0040SageBinaryData_0040_00402IB/* Not supported: data(F3 CF 50 69) */;

	internal static uint _003FTypeHash_0040FXShaderConstant_0040SageBinaryData_0040_00402IB/* Not supported: data(61 06 6F 3E) */;

	internal unsafe static sbyte* _003FTypeName_0040BoneInfluence_0040SageBinaryData_0040_00402PBDB/* Not supported: data(E8 E3 00 10) */;

	internal static uint _003FTypeHash_0040AnimationChannelBaseRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(EF 20 7B 87) */;

	internal unsafe static sbyte* _003FTypeName_0040FXShaderConstant_0040SageBinaryData_0040_00402PBDB/* Not supported: data(B4 E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040W3DContainer_0040SageBinaryData_0040_00402PBDB/* Not supported: data(04 E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040TimeRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(94 E4 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040FXShaderConstantFloat_0040SageBinaryData_0040_00402PBDB/* Not supported: data(84 E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040FXShaderConstantInt_0040SageBinaryData_0040_00402PBDB/* Not supported: data(70 E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040SoundOrEvaEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(20 E4 00 10) */;

	internal static uint _003FTypeHash_0040W3DCollisionBox_0040SageBinaryData_0040_00402IB/* Not supported: data(99 F6 26 6E) */;

	internal unsafe static sbyte* _003FTypeName_0040IntRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(7C E4 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040RandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(6C E4 00 10) */;

	internal static uint _003FTypeHash_0040AABTree_0040SageBinaryData_0040_00402IB/* Not supported: data(78 BB 3D 1C) */;

	internal unsafe static sbyte* _003FTypeName_0040AABTree_0040SageBinaryData_0040_00402PBDB/* Not supported: data(3C E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040SubObject_0040SageBinaryData_0040_00402PBDB/* Not supported: data(14 E3 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040FXShaderConstantBool_0040SageBinaryData_0040_00402PBDB/* Not supported: data(58 E3 00 10) */;

	internal static uint _003FTypeHash_0040ClientRandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(26 E0 35 42) */;

	internal static uint _003FTypeHash_0040W3DHierarchy_0040SageBinaryData_0040_00402IB/* Not supported: data(61 DE 94 ED) */;

	internal static uint _003FTypeHash_0040AnimationChannelQuaternion_0040SageBinaryData_0040_00402IB/* Not supported: data(82 02 29 51) */;

	internal static uint _003FTypeHash_0040W3DContainer_0040SageBinaryData_0040_00402IB/* Not supported: data(3F A3 9D 61) */;

	internal static uint _003FTypeHash_0040FXShaderConstantInt_0040SageBinaryData_0040_00402IB/* Not supported: data(C8 70 02 FD) */;

	internal static uint _003FTypeHash_0040SoundOrEvaEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(DE 4F AE 40) */;

	internal unsafe static sbyte* _003FTypeName_0040RealRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(88 E4 00 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AnimationChannelScalar_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C4 E2 00 10) */;

	internal static uint _003FTypeHash_0040Sphere_0040SageBinaryData_0040_00402IB/* Not supported: data(F8 1A 7B C6) */;

	internal unsafe static sbyte* _003FTypeName_0040W3DMesh_0040SageBinaryData_0040_00402PBDB/* Not supported: data(B0 E4 00 10) */;

	internal static uint _003FTypeHash_0040Color3f_0040SageBinaryData_0040_00402IB/* Not supported: data(16 5A D0 9B) */;

	internal static uint _003FTypeHash_0040IntRange_0040SageBinaryData_0040_00402IB/* Not supported: data(B5 BF 53 89) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040 _003FA0x325460ff_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4MeshGeometryType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4MeshGeometryType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B/* Not supported: data(F4 E7 00 10 02 00 00 00 04 E8 00 10 03 00 00 00 14 E8 00 10 00 00 00 00 1C E8 00 10 01 00 00 00) */;

	internal unsafe static sbyte* std_002E_003FA0x325460ff_002E_bad_alloc_Message/* Not supported: data(6C E1 00 10) */;

	public unsafe static int** __unep_0040_003F_003F_0024BookmarkCompare_0040U_003F_0024GenericBookmark_0040I_0040Tracker_0040Relo_0040_0040_0040Tracker_0040Relo_0040_0040_0024_0024FCA_NABU_003F_0024GenericBookmark_0040I_004001_00400_0040Z/* Not supported: data(90 42 00 10) */;

	internal unsafe static sbyte* std_002E_003FA0x8a5de553_002E_bad_alloc_Message/* Not supported: data(6C E1 00 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3IList_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 7C 27 02 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2INode_0040Marshaler_0040_00408/* Not supported: data(3C 27 02 10 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4Value_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 88 45 02 10 B4 27 02 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2Value_0040_00408/* Not supported: data(D0 27 02 10 F4 26 02 10 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05Q6AXXZ _003F_003F_7List_0040_00406B_0040/* Not supported: data(C0 92 00 10 70 92 00 10 E0 90 00 10 A0 88 00 10 20 9B 00 10 C4 F3 00 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_12 _003F_003F_R0_003FAVValue_0040_0040_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 3F 41 56 56 61 6C 75 65 40 40 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040Node_0040_00408/* Not supported: data(B8 45 02 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 00 28 02 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2List_0040_00408/* Not supported: data(68 28 02 10 84 27 02 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3INode_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 34 27 02 10) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7Value_0040_00406B_0040/* Not supported: data(C0 88 00 10 F0 88 00 10 10 94 00 10 C4 F3 00 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040IList_0040Marshaler_0040_00408/* Not supported: data(44 45 02 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 6C 27 02 10) */;

	internal static _0024ArrayType_0024_0024_0024BY06Q6AXXZ _003F_003F_7Node_0040_00406B_0040/* Not supported: data(70 88 00 10 40 97 00 10 F0 8E 00 10 B0 98 00 10 D0 99 00 10 60 95 00 10 C4 F3 00 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4IList_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 44 45 02 10 6C 27 02 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3IValue_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 EC 26 02 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_23 _003F_003F_R0_003FAVIValue_0040Marshaler_0040_0040_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 3F 41 56 49 56 61 6C 75 65 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3List_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 5C 28 02 10) */;

	internal static _0024ArrayType_0024_0024_0024BY06Q6AXXZ _003F_003F_7INode_0040Marshaler_0040_00406B_0040/* Not supported: data(7A BA 00 10 7A BA 00 10 7A BA 00 10 7A BA 00 10 7A BA 00 10 D0 87 00 10 C4 F3 00 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4IValue_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 C8 44 02 10 DC 26 02 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040Value_0040_00408/* Not supported: data(88 45 02 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 B4 27 02 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4Node_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 B8 45 02 10 00 28 02 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3Value_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 C4 27 02 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_11 _003F_003F_R0_003FAVNode_0040_0040_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 3F 41 56 4E 6F 64 65 40 40 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_22 _003F_003F_R0_003FAVINode_0040Marshaler_0040_0040_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 3F 41 56 49 4E 6F 64 65 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_22 _003F_003F_R0_003FAVIList_0040Marshaler_0040_0040_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 3F 41 56 49 4C 69 73 74 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2IList_0040Marshaler_0040_00408/* Not supported: data(84 27 02 10 00) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2IValue_0040Marshaler_0040_00408/* Not supported: data(F4 26 02 10 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040List_0040_00408/* Not supported: data(E8 45 02 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 4C 28 02 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040IValue_0040Marshaler_0040_00408/* Not supported: data(C8 44 02 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 DC 26 02 10) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7IValue_0040Marshaler_0040_00406B_0040/* Not supported: data(7A BA 00 10 7A BA 00 10 60 87 00 10 C4 F3 00 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2Node_0040_00408/* Not supported: data(1C 28 02 10 3C 27 02 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3Node_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 10 28 02 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4INode_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 08 45 02 10 24 27 02 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4List_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 E8 45 02 10 4C 28 02 10) */;

	internal unsafe static sbyte* std_002E_003FA0x385f832d_002E_bad_alloc_Message/* Not supported: data(6C E1 00 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040INode_0040Marshaler_0040_00408/* Not supported: data(08 45 02 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 24 27 02 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_11 _003F_003F_R0_003FAVList_0040_0040_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 3F 41 56 4C 69 73 74 40 40 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05Q6AXXZ _003F_003F_7IList_0040Marshaler_0040_00406B_0040/* Not supported: data(7A BA 00 10 7A BA 00 10 7A BA 00 10 7A BA 00 10 40 88 00 10 C4 F3 00 10) */;

	internal static _0024ArrayType_0024_0024_0024BY0EJ_0040_0024_0024CBD _003F_003F_C_0040_0EJ_0040GDEJDBKH_0040Assert_003F5failed_003F3_003F5srcIndex_003F5_003F_0024DM_003F5conver_0040/* Not supported: data(41 73 73 65 72 74 20 66 61 69 6C 65 64 3A 20 73 72 63 49 6E 64 65 78 20 3C 20 63 6F 6E 76 65 72 74 65 72 44 61 74 61 2E 6D 5F 49 6E 76 65 72 73 65 42 6F 6E 65 4D 61 70 70 69 6E 67 54 61 62 6C 65 2E 73 69 7A 65 28 29 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0FG_0040_0024_0024CBD _003F_003F_C_0040_0FG_0040LMNBEMCD_0040Assert_003F5failed_003F3_003F5_003F_0024CKbone_003F5_003F_0024DO_003F_0024DN_003F50_003F5_003F_0024CG_003F_0024CG_003F5_003F_0024CKbo_0040/* Not supported: data(41 73 73 65 72 74 20 66 61 69 6C 65 64 3A 20 2A 62 6F 6E 65 20 3E 3D 20 30 20 26 26 20 2A 62 6F 6E 65 20 3C 20 73 74 61 74 69 63 5F 63 61 73 74 3C 69 6E 74 3E 28 69 6E 76 65 72 73 65 42 6F 6E 65 4D 61 70 70 69 6E 67 54 61 62 6C 65 2E 73 69 7A 65 28 29 29 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BL_0040_0024_0024CBD _003F_003F_C_0040_0BL_0040DBJIEAG_0040Assert_003F5failed_003F3_003F5m2_003F5_003F9_003F5m_003F5_003F_0024DM_003F_0024DN_003F54_003F_0024AA_0040/* Not supported: data(41 73 73 65 72 74 20 66 61 69 6C 65 64 3A 20 6D 32 20 2D 20 6D 20 3C 3D 20 34 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040BKCHANDB_0040_003F_0024CFc_003F_0024CFX_003F3_003F_0024CF02X_003F3_003F_0024CFs_003F_0024AA_0040/* Not supported: data(25 63 25 58 3A 25 30 32 58 3A 25 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BL_0040_0024_0024CBD _003F_003F_C_0040_0BL_0040GICDAKDA_0040Assert_003F5failed_003F3_003F5m2_003F5_003F9_003F5m_003F5_003F_0024DN_003F_0024DN_003F52_003F_0024AA_0040/* Not supported: data(41 73 73 65 72 74 20 66 61 69 6C 65 64 3A 20 6D 32 20 2D 20 6D 20 3D 3D 20 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DF_0040_0024_0024CBD _003F_003F_C_0040_0DF_0040HPFNIGGK_0040Assert_003F5failed_003F3_003F5_003F_0024CBconverterData_003F4m__0040/* Not supported: data(41 73 73 65 72 74 20 66 61 69 6C 65 64 3A 20 21 63 6F 6E 76 65 72 74 65 72 44 61 74 61 2E 6D 5F 45 6E 64 69 61 6E 2E 49 73 42 69 67 45 6E 64 69 61 6E 28 29 00) */;

	internal static Vector4 _003FA0x431bf441_002E_003FdefaultColor_0040_003FL_0040_003F_003FDetermineVertexDataContents_0040W3DMeshProcessor_0040_0040YAXABUW3DMeshPipelineVertexData_0040SageBinaryData_0040_0040AAV_003F_0024vector_0040UDataMapping_0040W3DMeshProcessor_0040_0040Vallocator_0040eastl_0040_0040_0040eastl_0040_0040_0040Z_00404UVector4_0040Math_0040SageMathOnly_0040_0040A/* Not supported: data(00 00 80 3F 00 00 80 3F 00 00 80 3F 00 00 80 3F) */;

	internal static _0024_s__CatchableTypeArray_0024_extraBytes_8 _CTA2PAD/* Not supported: data(02 00 00 00 04 29 02 10 20 29 02 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_5 _003F_003F_R0PAD_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 50 41 44 00) */;

	internal static _s__ThrowInfo _TI2PAD/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 F8 28 02 10) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBUDataHandler_0040W3DMeshProcessor_0040_0040 W3DMeshProcessor_002E_003FA0x431bf441_002EDataHandlers_Xenon/* Not supported: data(01 00 00 00 1C 00 00 00 08 00 00 00 E0 9F 00 10 04 00 00 00 1A 00 00 00 04 00 00 00 50 A2 00 10 06 00 00 00 1A 00 00 00 04 00 00 00 50 A2 00 10 05 00 00 00 1A 00 00 00 04 00 00 00 50 A2 00 10 03 00 00 00 1B 00 00 00 04 00 00 00 40 9D 00 10 02 00 00 00 12 00 00 00 04 00 00 00 D0 A6 00 10 07 00 00 00 12 00 00 00 01 00 00 00 30 A0 00 10 08 00 00 00 11 00 00 00 01 00 00 00 E0 A0 00 10) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBUDataHandler_0040W3DMeshProcessor_0040_0040 W3DMeshProcessor_002E_003FA0x431bf441_002EDataHandlers_PS3/* Not supported: data(01 00 00 00 1C 00 00 00 08 00 00 00 E0 9F 00 10 04 00 00 00 1A 00 00 00 04 00 00 00 50 A2 00 10 06 00 00 00 1A 00 00 00 04 00 00 00 50 A2 00 10 05 00 00 00 1A 00 00 00 04 00 00 00 50 A2 00 10 03 00 00 00 1B 00 00 00 04 00 00 00 40 9D 00 10 02 00 00 00 12 00 00 00 04 00 00 00 D0 A6 00 10 07 00 00 00 12 00 00 00 01 00 00 00 30 A0 00 10 08 00 00 00 11 00 00 00 01 00 00 00 E0 A0 00 10) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBUDataHandler_0040W3DMeshProcessor_0040_0040 W3DMeshProcessor_002E_003FA0x431bf441_002EDataHandlers_Windows/* Not supported: data(01 00 00 00 09 00 00 00 0C 00 00 00 80 A0 00 10 04 00 00 00 09 00 00 00 0C 00 00 00 80 A0 00 10 06 00 00 00 09 00 00 00 0C 00 00 00 80 A0 00 10 05 00 00 00 09 00 00 00 0C 00 00 00 80 A0 00 10 03 00 00 00 05 00 00 00 08 00 00 00 D0 9B 00 10 02 00 00 00 12 00 00 00 04 00 00 00 D0 A6 00 10 07 00 00 00 12 00 00 00 01 00 00 00 30 A0 00 10 08 00 00 00 11 00 00 00 01 00 00 00 E0 A0 00 10) */;

	internal unsafe static sbyte* std_002E_003FA0x431bf441_002E_bad_alloc_Message/* Not supported: data(6C E1 00 10) */;

	internal static _s__CatchableType _CT_003F_003F_R0PAX_004084/* Not supported: data(01 00 00 00 44 46 02 10 00 00 00 00 FF FF FF FF 00 00 00 00 04 00 00 00 00 00 00 00) */;

	internal static _s__CatchableType _CT_003F_003F_R0PAD_004084/* Not supported: data(01 00 00 00 34 46 02 10 00 00 00 00 FF FF FF FF 00 00 00 00 04 00 00 00 00 00 00 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_5 _003F_003F_R0PAX_00408/* Not supported: data(C4 F3 00 10 00 00 00 00 2E 50 41 58 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04GKJNKENE_0040void_003F_0024AA_0040/* Not supported: data(76 6F 69 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04HAIMDOLD_00401f32_003F_0024AA_0040/* Not supported: data(31 66 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04BHELJJBO_00401s32_003F_0024AA_0040/* Not supported: data(31 73 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04HAKDIIHE_00401s16_003F_0024AA_0040/* Not supported: data(31 73 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03NHAENFNF_00401s8_003F_0024AA_0040/* Not supported: data(31 73 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04DHCMEEGD_00402f32_003F_0024AA_0040/* Not supported: data(32 66 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04FAOLODMO_00402s32_003F_0024AA_0040/* Not supported: data(32 73 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04DHADPCKE_00402s16_003F_0024AA_0040/* Not supported: data(32 73 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03MFLBHKDL_00402s8_003F_0024AA_0040/* Not supported: data(32 73 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04KEMGNND_00403f32_003F_0024AA_0040/* Not supported: data(33 66 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04GNILMKHO_00403s32_003F_0024AA_0040/* Not supported: data(33 73 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04KGDNLBE_00403s16_003F_0024AA_0040/* Not supported: data(33 73 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03HNANBNFO_00403s8_003F_0024AA_0040/* Not supported: data(33 73 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04LIGMLBMD_00404f32_003F_0024AA_0040/* Not supported: data(34 66 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04NPKLBGGO_00404s32_003F_0024AA_0040/* Not supported: data(34 73 33 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04LIEDAHAE_00404s16_003F_0024AA_0040/* Not supported: data(34 73 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03OANKCFOH_00404s8_003F_0024AA_0040/* Not supported: data(34 73 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03OEFHFJFF_00404u8_003F_0024AA_0040/* Not supported: data(34 75 38 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04OAJLLEAL_00404u8n_003F_0024AA_0040/* Not supported: data(34 75 38 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040KNBDCFNH_00404u8EndianSwapped_003F_0024AA_0040/* Not supported: data(34 75 38 45 6E 64 69 61 6E 53 77 61 70 70 65 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BC_0040_0024_0024CBD _003F_003F_C_0040_0BC_0040CKBNACNN_00404u8nEndianSwapped_003F_0024AA_0040/* Not supported: data(34 75 38 6E 45 6E 64 69 61 6E 53 77 61 70 70 65 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05CKPCCCCK_00402s16n_003F_0024AA_0040/* Not supported: data(32 73 31 36 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05PMKLMBDH_00404s16n_003F_0024AA_0040/* Not supported: data(34 73 31 36 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04HJFCCDEO_00403u10_003F_0024AA_0040/* Not supported: data(33 75 31 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05OFCDINDN_00403s10n_003F_0024AA_0040/* Not supported: data(33 73 31 30 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05OEOBOHAK_00403s11n_003F_0024AA_0040/* Not supported: data(33 73 31 31 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04FAMEFFAJ_00402f16_003F_0024AA_0040/* Not supported: data(32 66 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04NPIEKAKJ_00404f16_003F_0024AA_0040/* Not supported: data(34 66 31 36 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06EKNFNOKI_0040custom_003F_0024AA_0040/* Not supported: data(63 75 73 74 6F 6D 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BO_0040_0024_0024CBUElementInfoT_0040_0040 _003FA0xfa41eb23_002Es_ElementInfo/* Not supported: data(4C F2 00 10 00 00 00 00 00 00 00 00 44 F2 00 10 01 00 00 00 04 00 04 00 3C F2 00 10 01 00 00 00 04 00 04 00 34 F2 00 10 01 00 00 00 02 00 02 00 30 F2 00 10 01 00 00 00 01 00 01 00 28 F2 00 10 02 00 00 00 08 00 04 00 20 F2 00 10 02 00 00 00 08 00 04 00 18 F2 00 10 02 00 00 00 04 00 02 00 14 F2 00 10 02 00 00 00 02 00 01 00 0C F2 00 10 03 00 00 00 0C 00 04 00 04 F2 00 10 03 00 00 00 0C 00 04 00 FC F1 00 10 03 00 00 00 06 00 02 00 F8 F1 00 10 03 00 00 00 03 00 01 00 F0 F1 00 10 04 00 00 00 10 00 04 00 E8 F1 00 10 04 00 00 00 10 00 04 00 E0 F1 00 10 04 00 00 00 08 00 02 00 DC F1 00 10 04 00 00 00 04 00 01 00 D8 F1 00 10 04 00 00 00 04 00 01 00 D0 F1 00 10 04 00 00 00 04 00 01 00 D0 F1 00 10 04 00 00 00 04 00 01 00 BC F1 00 10 04 00 00 00 04 00 01 00 A8 F1 00 10 04 00 00 00 04 00 01 00 A0 F1 00 10 02 00 00 00 04 00 02 00 98 F1 00 10 04 00 00 00 08 00 02 00 90 F1 00 10 03 00 00 00 04 00 04 00 88 F1 00 10 03 00 00 00 04 00 04 00 80 F1 00 10 03 00 00 00 04 00 04 00 78 F1 00 10 02 00 00 00 04 00 02 00 70 F1 00 10 04 00 00 00 08 00 02 00 68 F1 00 10 04 00 00 00 00 00 00 00) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_mp_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static int _003FUninitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FUninitialized_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(EB 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xi_vt_a/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedPerAppDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedPerAppDomain_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(F0 00 00 06) */;

	[FixedAddressValueType]
	internal static bool _003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FIsDefaultDomain_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(EC 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_ma_a/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedNative_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedNative_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(EE 00 00 06) */;

	[FixedAddressValueType]
	internal static int _003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitialized_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(EA 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_ma_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedVtables_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedVtables_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(ED 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xi_vt_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedPerProcess_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedPerProcess_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(EF 00 00 06) */;

	internal static bool _003FInitializedPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x25dd7) could not be foundin any section!*/;

	internal static bool _003FEntered_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x25dd4) could not be foundin any section!*/;

	internal static bool _003FInitializedNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x25dd5) could not be foundin any section!*/;

	internal static int _003FCount_0040AllDomains_0040_003CCrtImplementationDetails_003E_0040_00402HA/*Field data (rva=0x25dd0) could not be foundin any section!*/;

	internal static _003CCrtImplementationDetails_003E.TriBool.State _003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A/* Not supported: data() */;

	internal static _003CCrtImplementationDetails_003E.TriBool.State _003FhasPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A/* Not supported: data() */;

	internal static bool _003FInitializedNativeFromCCTOR_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x25dd6) could not be foundin any section!*/;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_mp_a/* Not supported: data(00 00 00 00) */;

	public unsafe static int** __unep_0040_003FDoNothing_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z/* Not supported: data(05 C4 00 10) */;

	public unsafe static int** __unep_0040_003F_UninitializeDefaultDomain_0040LanguageSupport_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z/* Not supported: data(81 C5 00 10) */;

	[FixedAddressValueType]
	internal static uint __exit_list_size_app_domain;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x23130d9f_002E_003F_lock_0024initializer_0024_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0P6MXXZA/* Not supported: data(09 01 00 06) */;

	[FixedAddressValueType]
	internal unsafe static delegate*<void>* __onexitbegin_app_domain;

	internal static uint _003FA0x23130d9f_002E__exit_list_size/*Field data (rva=0x25ed4) could not be foundin any section!*/;

	[FixedAddressValueType]
	internal unsafe static delegate*<void>* __onexitend_app_domain;

	internal unsafe static delegate*<void>* _003FA0x23130d9f_002E__onexitbegin_m/*Field data (rva=0x25ecc) could not be foundin any section!*/;

	internal unsafe static delegate*<void>* _003FA0x23130d9f_002E__onexitend_m/*Field data (rva=0x25ed0) could not be foundin any section!*/;

	public static uint eastl_002EgEmptyString/*Field data (rva=0x259c8) could not be foundin any section!*/;

	public static _0024ArrayType_0024_0024_0024BY01Q6AXXZ _003F_003F_7type_info_0040_00406B_0040/* Not supported: data(30 BA 00 10 00 00 00 00) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AXXZ __xc_z/* Not supported: data(00) */;

	public static volatile uint __native_vcclrit_reason/* Not supported: data(FF FF FF FF) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AXXZ __xc_a/* Not supported: data(00) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AHXZ __xi_a/* Not supported: data(00) */;

	public static volatile global::__enative_startup_state __native_startup_state/* Not supported: data() */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AHXZ __xi_z/* Not supported: data(00) */;

	public unsafe static void* __native_startup_lock/*Field data (rva=0x25f0c) could not be foundin any section!*/;

	public static volatile uint __native_dllmain_reason/* Not supported: data(FF FF FF FF) */;

	internal unsafe static void delete(void* P_0, void* P_1)
	{
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

	internal unsafe static ushort Endian_002EEndianHelper_002EEndianToPlatform(EndianHelper* P_0, ushort value)
	{
		if (*(bool*)P_0)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num);
			*(sbyte*)(&num) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 1));
			System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = *(sbyte*)(&value);
			value = num;
		}
		return value;
	}

	internal unsafe static float Endian_002EEndianHelper_002EEndianToPlatform(EndianHelper* P_0, float value)
	{
		if (*(bool*)P_0)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(sbyte*)(&num) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref value, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)(&value);
			*(uint*)(&value) = num;
		}
		return value;
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

	internal unsafe static WWQuat* _002A(WWQuat* P_0, WWQuat* a, WWQuat* b)
	{
		*(float*)P_0 = (float)((double)((float*)a)[1] * (double)((float*)b)[2] - (double)((float*)a)[2] * (double)((float*)b)[1] + ((double)(*(float*)a) * (double)((float*)b)[3] + (double)(*(float*)b) * (double)((float*)a)[3]));
		((float*)P_0)[1] = (float)((double)((float*)b)[1] * (double)((float*)a)[3] + (double)((float*)a)[1] * (double)((float*)b)[3] - ((double)(*(float*)a) * (double)((float*)b)[2] - (double)((float*)a)[2] * (double)(*(float*)b)));
		((float*)P_0)[2] = (float)((double)((float*)b)[1] * (double)(*(float*)a) - (double)(*(float*)b) * (double)((float*)a)[1] + ((double)((float*)a)[2] * (double)((float*)b)[3] + (double)((float*)a)[3] * (double)((float*)b)[2]));
		((float*)P_0)[3] = (float)((double)((float*)a)[3] * (double)((float*)b)[3] - ((double)(*(float*)a) * (double)(*(float*)b) + (double)((float*)b)[1] * (double)((float*)a)[1] + (double)((float*)a)[2] * (double)((float*)b)[2]));
		return P_0;
	}

	internal unsafe static void Slerp(WWQuat* res, WWQuat* p, WWQuat* q, float alpha)
	{
		float num = (float)((double)(*(float*)p) * (double)(*(float*)q) + (double)((float*)p)[1] * (double)((float*)q)[1] + (double)((float*)p)[2] * (double)((float*)q)[2] + (double)((float*)p)[3] * (double)((float*)q)[3]);
		int num2;
		if (num < 0f)
		{
			num = 0f - num;
			num2 = 1;
		}
		else
		{
			num2 = 0;
		}
		float num3;
		if (1.0 - (double)num < 9.99999905104687E-09)
		{
			num3 = (float)(1.0 - (double)alpha);
		}
		else
		{
			float num4 = (float)Math.Acos(num);
			float num5 = (float)Math.Sin(num4);
			float num6 = (float)(1.0 / (double)num5);
			double num7 = num4;
			double num8 = (double)alpha * num7;
			num3 = (float)((double)(float)Math.Sin((float)(num7 - num8)) * (double)num6);
			alpha = (float)((double)(float)Math.Sin((float)num8) * (double)num6);
		}
		if (num2 != 0)
		{
			alpha = 0f - alpha;
		}
		*(float*)res = (float)((double)(*(float*)p) * (double)num3 + (double)(*(float*)q) * (double)alpha);
		((float*)res)[1] = (float)((double)num3 * (double)((float*)p)[1] + (double)alpha * (double)((float*)q)[1]);
		((float*)res)[2] = (float)((double)((float*)p)[2] * (double)num3 + (double)((float*)q)[2] * (double)alpha);
		((float*)res)[3] = (float)((double)((float*)p)[3] * (double)num3 + (double)((float*)q)[3] * (double)alpha);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool _003FA0x0e388f25_002E_initTable()
	{
		int num = 0;
		do
		{
			*(float*)((ref *(_003F*)(num * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY0BAA_0040M, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _003FA0x0e388f25_002Efiltertable, 64)))) = (float)(1.0 - (double)(float)Math.Sin((double)(float)((double)(float)num / 240.0) * 90.0 * 3.1415927410125732 / 180.0));
			num++;
		}
		while (num < 240);
		return true;
	}

	internal static void _003FA0x0e388f25_002E_003F_003F__E_003FA0x0e388f25_0040initTable_0040_0040YMXXZ()
	{
		_003FA0x0e388f25_002EinitTable = _003FA0x0e388f25_002E_initTable();
	}

	internal unsafe static int _003FA0x0e388f25_002EFindUselessPacket(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* anim, float delta)
	{
		int num = *(int*)anim;
		int num2 = (((int*)anim)[1] - num) / 24;
		switch (num2)
		{
		case 0:
		case 1:
			return -1;
		case 2:
			if (*(bool*)(num + 44))
			{
				return -1;
			}
			if (delta >= 0f)
			{
				delta = (float)((double)(float)(uint)(*(int*)(num + 24) - *(int*)num) * (double)delta);
				uint num17 = 0u;
				int num18 = num + 4;
				vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)num18;
				if (0u < (uint)(((int*)ptr2)[1] - *(int*)ptr2 >> 2))
				{
					vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)num18;
					int num19 = ((int*)ptr3)[1] - *(int*)ptr3 >> 2;
					int num20 = *(int*)(num + 28);
					do
					{
						double num21 = *(float*)num20;
						if (!((double)(float)Math.Abs((double)(float)(num21 - num21)) > (double)delta))
						{
							num17++;
							num20 += 4;
							continue;
						}
						return -1;
					}
					while (num17 < (uint)num19);
				}
			}
			return 1;
		default:
		{
			uint num3 = 0u;
			float num4 = float.MaxValue;
			uint num5 = 0u;
			uint num6 = (uint)(num2 - 2);
			if (0 < num6)
			{
				int num7 = num + 68;
				int num8 = num + 52;
				do
				{
					if (*(byte*)(num7 - 48) == 0 && *(byte*)num7 == 0)
					{
						uint num9 = *(uint*)(num7 - 68);
						float num10 = (float)((double)(float)((uint)(*(int*)(num7 - 44)) - num9) / (double)(float)((uint)(*(int*)(num7 - 20)) - num9));
						float num11 = 0f;
						uint num12 = 0u;
						if (0u < (uint)(*(int*)(num8 - 44) - *(int*)(num7 - 64) >> 2))
						{
							int num13 = *(int*)num8;
							int num14 = num13;
							int num15 = *(int*)(num7 - 64) - num13;
							vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr;
							do
							{
								float num16 = (float)Math.Abs((double)(float)(((double)(*(float*)num14) - (double)(*(float*)(num15 + num14))) * (double)num10 + (double)(*(float*)(num15 + num14)) - (double)(*(float*)num14)));
								if ((double)num16 > (double)num11)
								{
									num11 = num16;
								}
								num12++;
								num14 += 4;
								ptr = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(num7 - 64);
							}
							while (num12 < (uint)(((int*)ptr)[1] - *(int*)ptr >> 2));
						}
						if ((double)num11 < (double)delta)
						{
							return (int)(num5 + 1);
						}
						if ((double)num11 < (double)num4)
						{
							num3 = num5 + 1;
							num4 = num11;
						}
					}
					num5++;
					num8 += 24;
					num7 += 24;
				}
				while (num5 < num6);
			}
			if (delta < 0f && num3 != 0)
			{
				return (int)num3;
			}
			return -1;
		}
		}
	}

	internal unsafe static int _003FA0x0e388f25_002EFindUselessPacketQ(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* anim, float delta)
	{
		int num = *(int*)anim;
		int num2 = (((int*)anim)[1] - num) / 24;
		switch (num2)
		{
		case 0:
		case 1:
			return -1;
		case 2:
			if (*(bool*)(num + 44))
			{
				return -1;
			}
			if (delta >= 0f)
			{
				int num29 = *(int*)(num + 4);
				float num30 = *(float*)num29;
				float num31 = *(float*)(num29 + 4);
				float num32 = *(float*)(num29 + 8);
				float num33 = *(float*)(num29 + 12);
				int num34 = *(int*)(num + 28);
				float num35 = *(float*)num34;
				float num36 = *(float*)(num34 + 4);
				float num37 = *(float*)(num34 + 8);
				float num38 = *(float*)(num34 + 12);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat7);
				*(float*)(&wWQuat7) = num35;
				System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat7, 4)) = num36;
				System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat7, 8)) = num37;
				System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat7, 12)) = num38;
				float num39 = 0f - num30;
				float num40 = 0f - num31;
				float num41 = 0f - num32;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat8);
				*(float*)(&wWQuat8) = num39;
				System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat8, 4)) = num40;
				System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat8, 8)) = num41;
				System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat8, 12)) = num33;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat9);
				WWQuat* ptr2 = _002A(&wWQuat9, &wWQuat8, &wWQuat7);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat10);
				System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat10, 12)) = ((float*)ptr2)[3];
				float num42 = (float)Math.Abs((double)System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat10, 12)));
				if (num42 < 1f)
				{
					float num43 = num42;
					if ((double)(float)Math.Acos(num43) * 2.0 > (double)delta)
					{
						return -1;
					}
				}
			}
			return 1;
		default:
		{
			uint num3 = 0u;
			float num4 = float.MaxValue;
			uint num5 = 0u;
			if (0u < (uint)(num2 - 2))
			{
				int num6 = num + 4;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat2);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat3);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat4);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat5);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out WWQuat wWQuat6);
				do
				{
					if (*(byte*)(num6 + 64 - 48) == 0 && *(byte*)(num6 + 64) == 0)
					{
						int num7 = *(int*)num6;
						float num8 = *(float*)num7;
						float num9 = *(float*)(num7 + 4);
						float num10 = *(float*)(num7 + 8);
						float num11 = *(float*)(num7 + 12);
						*(float*)(&wWQuat) = num8;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat, 4)) = num9;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat, 8)) = num10;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat, 12)) = num11;
						int num12 = *(int*)(num6 + 24);
						float num13 = *(float*)num12;
						float num14 = *(float*)(num12 + 4);
						float num15 = *(float*)(num12 + 8);
						float num16 = *(float*)(num12 + 12);
						int num17 = *(int*)(num6 + 48);
						float num18 = *(float*)num17;
						float num19 = *(float*)(num17 + 4);
						float num20 = *(float*)(num17 + 8);
						float num21 = *(float*)(num17 + 12);
						*(float*)(&wWQuat2) = num18;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat2, 4)) = num19;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat2, 8)) = num20;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat2, 12)) = num21;
						uint num22 = *(uint*)(num6 - 4);
						Slerp(&wWQuat3, &wWQuat, &wWQuat2, (float)((double)(float)((uint)(*(int*)(num6 + 20)) - num22) / (double)(float)((uint)(*(int*)(num6 + 44)) - num22)));
						float num23 = 0f - num13;
						float num24 = 0f - num14;
						float num25 = 0f - num15;
						*(float*)(&wWQuat4) = num23;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat4, 4)) = num24;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat4, 8)) = num25;
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat4, 12)) = num16;
						WWQuat* ptr = _002A(&wWQuat5, &wWQuat4, &wWQuat3);
						System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat6, 12)) = ((float*)ptr)[3];
						float num26 = (float)Math.Abs((double)System.Runtime.CompilerServices.Unsafe.As<WWQuat, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref wWQuat6, 12)));
						if (num26 >= 1f)
						{
							return (int)(num5 + 1);
						}
						float num27 = num26;
						float num28 = (float)Math.Acos(num27) * 2f;
						if ((double)num28 <= (double)delta)
						{
							return (int)(num5 + 1);
						}
						if ((double)num28 < (double)num4)
						{
							num3 = num5 + 1;
							num4 = num28;
						}
					}
					num5++;
					num6 += 24;
				}
				while (num5 < (uint)(num2 - 2));
			}
			if (delta < 0f && num3 != 0)
			{
				return (int)num3;
			}
			return -1;
		}
		}
	}

	internal unsafe static CompressedTimecoded* CompressedTimecoded_002ECompress(AnimationChannelBase* srcCh, vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* src, W3DAnimation.CompressionSettings* settings)
	{
		CompressedTimecoded* ptr = (CompressedTimecoded*)@new(24u);
		CompressedTimecoded* ptr3;
		try
		{
			if (ptr != null)
			{
				vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr + 8);
				*(int*)ptr2 = 0;
				((int*)ptr2)[1] = 0;
				((int*)ptr2)[2] = 0;
				ptr3 = ptr;
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
		*(int*)ptr3 = ((int*)srcCh)[1];
		((short*)ptr3)[2] = ((short*)srcCh)[6];
		eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_003D((vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 8), src);
		uint num = (uint)((double)(float)(uint)((((int*)src)[1] - *(int*)src) / 24) * (double)((float*)settings)[4]);
		if (num < 1)
		{
			num = 1u;
		}
		int num2 = *(int*)ptr3;
		if (num2 == 3)
		{
			float num3 = ((float*)settings)[1];
			while (true)
			{
				vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 8);
				if ((uint)((((int*)ptr4)[1] - *(int*)ptr4) / 24) <= num && !(num3 >= 0f))
				{
					break;
				}
				int num4 = _003FA0x0e388f25_002EFindUselessPacketQ((vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 8), num3);
				if (num4 < 0)
				{
					if (num3 < 0f)
					{
						break;
					}
					num3 = -1f;
				}
				else
				{
					eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002Eerase((vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 8), (SrcFrame*)(num4 * 24 + ((int*)ptr3)[2]));
				}
			}
		}
		else
		{
			float num5 = ((num2 != 4) ? (*(float*)settings) : ((float*)settings)[2]);
			float num6 = num5;
			while (true)
			{
				vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 8);
				if ((uint)((((int*)ptr5)[1] - *(int*)ptr5) / 24) <= num && !(num6 >= 0f))
				{
					break;
				}
				int num7 = _003FA0x0e388f25_002EFindUselessPacket((vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 8), num6);
				if (num7 < 0)
				{
					if (num6 < 0f)
					{
						break;
					}
					num6 = -1f;
				}
				else
				{
					eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002Eerase((vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 8), (SrcFrame*)(num7 * 24 + ((int*)ptr3)[2]));
				}
			}
		}
		return ptr3;
	}

	internal unsafe static uint CompressedTimecoded_002EEstimateSize(CompressedTimecoded* P_0)
	{
		CompressedTimecoded* ptr = (CompressedTimecoded*)((byte*)P_0 + 8);
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)ptr;
		if (*(int*)ptr2 == ((int*)ptr2)[1])
		{
			return 0u;
		}
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)ptr;
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(*(int*)ptr + 4);
		return (uint)(((((int*)ptr4)[1] - *(int*)ptr4 >> 2) * 4 + 2) * ((((int*)ptr3)[1] - *(int*)ptr3) / 24));
	}

	internal unsafe static void CompressedTimecoded_002EWriteOut(CompressedTimecoded* P_0, Tracker* tracker, EndianHelper* helper, AnimationChannelTimecoded** dst)
	{
		Relo_002ETracker_002EPush(tracker, (void**)dst, 36u, 1u);
		int num = 1624821609;
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(sbyte*)(&num2) = 96;
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = -40;
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = -49;
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = 105;
			num = (int)num2;
		}
		*(int*)(int)(*(uint*)dst) = num;
		int num3 = *(int*)P_0;
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
			*(sbyte*)(&num4) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3)) = *(sbyte*)(&num3);
			num3 = (int)num4;
		}
		*(int*)(*(int*)dst + 4) = num3;
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		uint num5 = (uint)((((int*)ptr)[1] - *(int*)ptr) / 24);
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num6);
			*(sbyte*)(&num6) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 3)) = *(sbyte*)(&num5);
			num5 = num6;
		}
		*(uint*)(*(int*)dst + 8) = num5;
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		uint num7;
		if (*(int*)ptr2 == ((int*)ptr2)[1])
		{
			num7 = 1u;
		}
		else
		{
			vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(((int*)P_0)[2] + 4);
			num7 = (uint)(((int*)ptr3)[1] - *(int*)ptr3 >> 2);
		}
		uint num8 = num7;
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num9);
			*(sbyte*)(&num9) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num9, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num9, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num9, 3)) = *(sbyte*)(&num7);
			num8 = num9;
		}
		*(uint*)(*(int*)dst + 12) = num8;
		ushort num10 = ((ushort*)P_0)[2];
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num11);
			*(sbyte*)(&num11) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 1));
			System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 1)) = *(sbyte*)(&num10);
			num10 = num11;
		}
		*(ushort*)(*(int*)dst + 16) = num10;
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(((int*)P_0)[2] + 4);
		uint num12 = (uint)((((int*)ptr5)[1] - *(int*)ptr5 >> 2) * ((((int*)ptr4)[1] - *(int*)ptr4) / 24));
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num13);
			*(sbyte*)(&num13) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 3)) = *(sbyte*)(&num12);
			num12 = num13;
		}
		*(uint*)(*(int*)dst + 28) = num12;
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		uint num14 = (uint)((((int*)ptr6)[1] - *(int*)ptr6) / 24);
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num15);
			*(sbyte*)(&num15) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 3)) = *(sbyte*)(&num14);
			num14 = num15;
		}
		*(uint*)(*(int*)dst + 20) = num14;
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr7 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		if (*(int*)ptr7 != ((int*)ptr7)[1])
		{
			vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr8 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
			vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr9 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(((int*)P_0)[2] + 4);
			Relo_002ETracker_002EPush(tracker, (void**)(*(int*)dst + 32), 4u, (uint)((((int*)ptr9)[1] - *(int*)ptr9 >> 2) * ((((int*)ptr8)[1] - *(int*)ptr8) / 24)));
			stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr10 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)tracker + 4);
			((int*)ptr10)[1] += -4;
			int num16 = *(int*)dst;
			float* ptr11 = (float*)(int)(*(uint*)(num16 + 32));
			vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr12 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
			Relo_002ETracker_002EPush(tracker, (void**)(num16 + 24), 2u, (uint)((((int*)ptr12)[1] - *(int*)ptr12) / 24));
			stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr13 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)tracker + 4);
			((int*)ptr13)[1] += -4;
			ushort* ptr14 = (ushort*)(int)(*(uint*)(*(int*)dst + 24));
			uint num17 = 0u;
			vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr15 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
			if (0u < (uint)((((int*)ptr15)[1] - *(int*)ptr15) / 24))
			{
				int num18 = 0;
				do
				{
					SrcFrame* ptr16 = (SrcFrame*)(((int*)P_0)[2] + num18);
					ushort num19 = (ushort)(*(uint*)ptr16);
					if (((bool*)ptr16)[20])
					{
						num19 |= 0x8000;
					}
					*ptr14 = Endian_002EEndianHelper_002EEndianToPlatform(helper, num19);
					ptr14++;
					uint num20 = 0u;
					if (0 < eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Esize((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr16 + 4)))
					{
						do
						{
							float* ptr17 = (float*)(int)(num20 * 4 + (uint)((int*)ptr16)[1]);
							*ptr11 = Endian_002EEndianHelper_002EEndianToPlatform(helper, *ptr17);
							ptr11++;
							num20++;
						}
						while (num20 < eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Esize((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr16 + 4)));
					}
					num17++;
					num18 += 24;
					ptr15 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
				}
				while (num17 < (uint)((((int*)ptr15)[1] - *(int*)ptr15) / 24));
			}
		}
		((int*)((byte*)tracker + 4))[1] += -4;
	}

	internal unsafe static float CompressedAdaptiveDelta_002ECompressFrame(CompressedAdaptiveDelta* P_0, CompressedAdaptiveDelta.Frame* dst, int filter, float* val, float scale, float* curPrevVal, bool* excessError, float maxError)
	{
		*(sbyte*)dst = (sbyte)filter;
		int num = ((((int*)P_0)[7] == 4) ? 8 : 128);
		float num2 = *val;
		float num3 = 0f;
		float num4 = (float)((double)(*(float*)((ref *(_003F*)(filter * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY0BAA_0040M, _003F>(ref _003FA0x0e388f25_002Efiltertable)))) * (double)scale);
		if (num == 128)
		{
			num4 *= 0.0625f;
		}
		float num5 = (float)(1.0 / (double)num4);
		int num6 = 1;
		do
		{
			float num7 = (float)Math.Abs((double)(float)((double)(*(float*)(num6 * 4 + (byte*)val)) - (double)num2));
			int num8 = (int)(double)(float)Math.Floor((float)((double)num7 * (double)num5 + 0.5));
			if ((double)(*(float*)(num6 * 4 + (byte*)val)) < (double)num2)
			{
				num8 = -num8;
				int num9 = -num;
				if (num8 < num9)
				{
					num8 = num9;
				}
			}
			else if (num8 >= num)
			{
				num8 = num - 1;
			}
			double num10 = (double)(float)num8 * (double)num4 + (double)num2;
			float num11 = (float)Math.Abs((double)(float)(num10 - (double)(*(float*)(num6 * 4 + (byte*)val))));
			*(num6 + (sbyte*)dst) = (sbyte)num8;
			num2 = (float)num10;
			num3 = (float)((double)num11 + (double)num3);
			if (excessError != null && (double)num11 > (double)maxError)
			{
				*excessError = true;
			}
			num6++;
		}
		while (num6 <= 16);
		if (curPrevVal != null)
		{
			*curPrevVal = num2;
		}
		return num3;
	}

	internal unsafe static CompressedAdaptiveDelta* CompressedAdaptiveDelta_002ECompress(AnimationChannelBase* srcCh, vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* src, W3DAnimation.CompressionSettings* settings)
	{
		if ((uint)((((int*)src)[1] - *(int*)src) / 24) < 2u)
		{
			return null;
		}
		CompressedAdaptiveDelta* ptr = (CompressedAdaptiveDelta*)@new(52u);
		CompressedAdaptiveDelta* ptr2;
		try
		{
			ptr2 = ((ptr == null) ? null : CompressedAdaptiveDelta_002E_007Bctor_007D(ptr));
		}
		catch
		{
			//try-fault
			delete(ptr);
			throw;
		}
		*(int*)ptr2 = ((int*)srcCh)[1];
		((short*)ptr2)[2] = ((short*)srcCh)[6];
		float num = 0f;
		SrcFrame* ptr3 = (SrcFrame*)(*(int*)src + 24);
		int num2 = ((int*)src)[1];
		SrcFrame* ptr4 = (SrcFrame*)num2;
		if (ptr3 != ptr4)
		{
			ptr4 = (SrcFrame*)num2;
			do
			{
				uint num3 = 0u;
				if (0u < (uint)(((int*)((byte*)ptr3 - 20))[7] - ((int*)ptr3)[1] >> 2))
				{
					int num4 = *((int*)ptr3 - 5);
					int num5 = num4;
					int num6 = ((int*)ptr3)[1] - num4;
					vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr5;
					do
					{
						float num7 = (float)((double)(*(float*)(num6 + num5)) - (double)(*(float*)num5));
						float num8 = (float)Math.Abs((double)num7);
						if ((double)num8 > (double)num)
						{
							num = num8;
						}
						num3++;
						num5 += 4;
						ptr5 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 + 4);
					}
					while (num3 < (uint)(((int*)ptr5)[1] - *(int*)ptr5 >> 2));
				}
				ptr3 = (SrcFrame*)((byte*)ptr3 + 24);
			}
			while (ptr3 != ptr4);
		}
		((float*)ptr2)[2] = (float)((double)num / 7.0);
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(*(int*)src + 4);
		uint num9 = (uint)(((int*)ptr2)[12] = ((int*)ptr6)[1] - *(int*)ptr6 >> 2);
		uint num10 = 0u;
		if (0 < num9)
		{
			CompressedAdaptiveDelta* ptr7 = (CompressedAdaptiveDelta*)((byte*)ptr2 + 12);
			do
			{
				eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Epush_back((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)ptr7, (float*)(int)(num10 * 4 + (uint)(*(int*)(*(int*)src + 4))));
				num10++;
			}
			while (num10 < num9);
		}
		CompressedAdaptiveDelta* ptr8 = (CompressedAdaptiveDelta*)((byte*)ptr2 + 28);
		int num11 = 4;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY03M _0024ArrayType_0024_0024_0024BY03M);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0BB_0040M _0024ArrayType_0024_0024_0024BY0BB_0040M);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CompressedAdaptiveDelta.Frame frame);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CompressedAdaptiveDelta.Frame frame2);
		while (true)
		{
			*(int*)ptr8 = num11;
			eastl_002Evector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E_002Eclear((vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr2 + 32));
			if (0 < num9)
			{
				int num12 = ((int*)ptr2)[3];
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ref _0024ArrayType_0024_0024_0024BY03M, num12, num9 << 2);
			}
			bool flag = false;
			uint num13 = 1u;
			uint num14 = (uint)((((int*)src)[1] - *(int*)src) / 24 + 15) >> 4;
			if (num14 == 0)
			{
				break;
			}
			int num15 = 24;
			do
			{
				num14--;
				uint num16 = 0u;
				if (0 < num9)
				{
					W3DAnimation.CompressionSettings* ptr9 = (W3DAnimation.CompressionSettings*)((byte*)settings + 12);
					_0024ArrayType_0024_0024_0024BY03M* ptr10 = &_0024ArrayType_0024_0024_0024BY03M;
					do
					{
						*(float*)(&_0024ArrayType_0024_0024_0024BY0BB_0040M) = *(float*)ptr10;
						uint num17 = 0u;
						int num18 = *(int*)src;
						int num19 = (((int*)src)[1] - num18) / 24;
						int num20 = (int)(num16 * 4);
						int num21 = num18 + num15 + 4;
						do
						{
							if (num17 + num13 < (uint)num19)
							{
								*(float*)((ref *(_003F*)(num17 * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY0BB_0040M, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY0BB_0040M, 4)))) = *(float*)(num20 + *(int*)num21);
							}
							else
							{
								*(float*)((ref *(_003F*)(num17 * 4)) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY0BB_0040M, _003F>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _0024ArrayType_0024_0024_0024BY0BB_0040M, 4)))) = *(float*)((ref *(_003F*)(num17 * 4)) + (ref *(_003F*)(&_0024ArrayType_0024_0024_0024BY0BB_0040M)));
							}
							num17++;
							num21 += 24;
						}
						while (num17 < 16);
						int filter = -1;
						float num22 = float.MaxValue;
						int num23 = 0;
						do
						{
							float num24 = CompressedAdaptiveDelta_002ECompressFrame(ptr2, &frame, num23, (float*)(&_0024ArrayType_0024_0024_0024BY0BB_0040M), ((float*)ptr2)[2], null, null, 0f);
							if ((double)num24 < (double)num22)
							{
								filter = num23;
								num22 = num24;
							}
							num23++;
						}
						while (num23 < 256);
						CompressedAdaptiveDelta_002ECompressFrame(ptr2, &frame2, filter, (float*)(&_0024ArrayType_0024_0024_0024BY0BB_0040M), ((float*)ptr2)[2], (float*)ptr10, &flag, *(float*)ptr9);
						eastl_002Evector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E_002Epush_back((vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr2 + 32), &frame2);
						num16++;
						ptr10 = (_0024ArrayType_0024_0024_0024BY03M*)((byte*)ptr10 + 4);
					}
					while (num16 < num9);
				}
				num13 += 16;
				num15 += 384;
			}
			while (num14 != 0);
			if (!flag)
			{
				break;
			}
			num11 += 4;
			if (num11 < 12)
			{
				continue;
			}
			CompressedAdaptiveDelta* ptr11 = ptr2;
			try
			{
				vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* ptr12 = (vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr2 + 32);
				int num25 = *(int*)ptr12;
				if (num25 != 0)
				{
					delete_005B_005D((void*)num25);
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)ptr11 + 12);
				throw;
			}
			vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr13 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr2 + 12);
			int num26 = *(int*)ptr13;
			if (num26 != 0)
			{
				delete_005B_005D((void*)num26);
			}
			delete(ptr2);
			return null;
		}
		return ptr2;
	}

	internal unsafe static CompressedAdaptiveDelta* CompressedAdaptiveDelta_002E_007Bctor_007D(CompressedAdaptiveDelta* P_0)
	{
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 12);
		*(int*)ptr = 0;
		((int*)ptr)[1] = 0;
		((int*)ptr)[2] = 0;
		try
		{
			vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 32);
			*(int*)ptr2 = 0;
			((int*)ptr2)[1] = 0;
			((int*)ptr2)[2] = 0;
			return P_0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)P_0 + 12);
			throw;
		}
	}

	internal unsafe static uint CompressedAdaptiveDelta_002EEstimateSize(CompressedAdaptiveDelta* P_0)
	{
		CompressedAdaptiveDelta* ptr = (CompressedAdaptiveDelta*)((byte*)P_0 + 12);
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)ptr;
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)ptr;
		return (uint)((((int*)ptr3)[1] - *(int*)ptr3 >> 2) * 4 + ((((int*)ptr2)[1] - *(int*)ptr2 >> 2) * ((int*)P_0)[7] + 7 >>> 3) * ((((int*)((byte*)P_0 + 32))[1] - ((int*)P_0)[8]) / 17));
	}

	internal unsafe static void CompressedAdaptiveDelta_002EWriteOut(CompressedAdaptiveDelta* P_0, Tracker* tracker, EndianHelper* helper, AnimationChannelDelta** dst)
	{
		Relo_002ETracker_002EPush(tracker, (void**)dst, 44u, 1u);
		int num = 2012836613;
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(sbyte*)(&num2) = 119;
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = -7;
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = 115;
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = 5;
			num = (int)num2;
		}
		*(int*)(int)(*(uint*)dst) = num;
		int num3 = *(int*)P_0;
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
			*(sbyte*)(&num4) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3)) = *(sbyte*)(&num3);
			num3 = (int)num4;
		}
		*(int*)(*(int*)dst + 4) = num3;
		vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 32);
		uint num5 = (uint)((((int*)ptr)[1] - *(int*)ptr) / 17) / (uint)((int*)P_0)[12] * 16;
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num6);
			*(sbyte*)(&num6) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 3)) = *(sbyte*)(&num5);
			num5 = num6;
		}
		*(uint*)(*(int*)dst + 8) = num5;
		int num7 = ((int*)P_0)[12];
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num8);
			*(sbyte*)(&num8) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 3)) = *(sbyte*)(&num7);
			num7 = (int)num8;
		}
		*(int*)(*(int*)dst + 12) = num7;
		ushort num9 = ((ushort*)P_0)[2];
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num10);
			*(sbyte*)(&num10) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num9, 1));
			System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 1)) = *(sbyte*)(&num9);
			num9 = num10;
		}
		*(ushort*)(*(int*)dst + 16) = num9;
		float num11 = ((float*)P_0)[2];
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num12);
			*(sbyte*)(&num12) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 1)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 2)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num12, 3)) = *(sbyte*)(&num11);
			*(uint*)(&num11) = num12;
		}
		*(float*)(*(int*)dst + 20) = num11;
		int num13 = ((int*)P_0)[7];
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num14);
			*(sbyte*)(&num14) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 3)) = *(sbyte*)(&num13);
			num13 = (int)num14;
		}
		*(int*)(*(int*)dst + 32) = num13;
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 12);
		uint num15 = (uint)(((int*)ptr2)[1] - *(int*)ptr2 >> 2);
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num16);
			*(sbyte*)(&num16) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 3)) = *(sbyte*)(&num15);
			num15 = num16;
		}
		*(uint*)(*(int*)dst + 24) = num15;
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 12);
		Relo_002ETracker_002EPush(tracker, (void**)(*(int*)dst + 28), 4u, (uint)(((int*)ptr3)[1] - *(int*)ptr3 >> 2));
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr4 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)tracker + 4);
		((int*)ptr4)[1] += -4;
		uint num17 = 0u;
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr5 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 12);
		if (0u < (uint)(((int*)ptr5)[1] - *(int*)ptr5 >> 2))
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num19);
			do
			{
				float num18 = *(float*)(int)(num17 * 4 + (uint)((int*)P_0)[3]);
				if (*(bool*)helper)
				{
					*(sbyte*)(&num19) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num18, 3));
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num19, 1)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num18, 2));
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num19, 2)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num18, 1));
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num19, 3)) = *(sbyte*)(&num18);
					*(uint*)(&num18) = num19;
				}
				*(float*)(int)((uint)(*(int*)(*(int*)dst + 28)) + num17 * 4) = num18;
				num17++;
				ptr5 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 12);
			}
			while (num17 < (uint)(((int*)ptr5)[1] - *(int*)ptr5 >> 2));
		}
		int num20 = ((((int*)P_0)[7] == 4) ? 9 : 17);
		vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* ptr6 = (vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 32);
		uint num21 = (uint)((((int*)ptr6)[1] - *(int*)ptr6) / 17 * num20);
		uint num22 = num21;
		if (*(bool*)helper)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num23);
			*(sbyte*)(&num23) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num21, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num23, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num21, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num23, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num21, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num23, 3)) = *(sbyte*)(&num21);
			num22 = num23;
		}
		*(uint*)(*(int*)dst + 36) = num22;
		Relo_002ETracker_002EPush(tracker, (void**)(*(int*)dst + 40), 1u, num21);
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr7 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)tracker + 4);
		((int*)ptr7)[1] += -4;
		byte* ptr8 = (byte*)(int)(*(uint*)(*(int*)dst + 40));
		uint num24 = 0u;
		vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* ptr9 = (vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 32);
		if (0u < (uint)((((int*)ptr9)[1] - *(int*)ptr9) / 17))
		{
			int num25 = 0;
			do
			{
				CompressedAdaptiveDelta.Frame* ptr10 = (CompressedAdaptiveDelta.Frame*)(((int*)P_0)[8] + num25);
				*ptr8 = *(byte*)ptr10;
				ptr8++;
				if (((int*)P_0)[7] == 4)
				{
					CompressedAdaptiveDelta.Frame* ptr11 = (CompressedAdaptiveDelta.Frame*)((byte*)ptr10 + 2);
					uint num26 = 8u;
					do
					{
						*ptr8 = (byte)((*((sbyte*)ptr11 - 1) & 0xF) | (*(sbyte*)ptr11 << 4));
						ptr8++;
						ptr11 = (CompressedAdaptiveDelta.Frame*)((byte*)ptr11 + 2);
						num26--;
					}
					while (num26 != 0);
				}
				else
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr8, (byte*)ptr10 + 1, 16);
					ptr8 += 16;
				}
				num24++;
				num25 += 17;
				ptr9 = (vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 32);
			}
			while (num24 < (uint)((((int*)ptr9)[1] - *(int*)ptr9) / 17));
		}
		stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr12 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)tracker + 4);
		((int*)ptr12)[1] += -4;
	}

	internal unsafe static void eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static uint eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Esize(vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		return (uint)(((int*)P_0)[1] - *(int*)P_0 >> 2);
	}

	internal unsafe static void eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002Epush_back(vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0, float* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num < (uint)((int*)P_0)[2])
		{
			int num2 = (int)num;
			((int*)P_0)[1] = (int)(num + 4);
			if (num2 != 0)
			{
				*(float*)num2 = *value;
			}
		}
		else
		{
			eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(P_0, (float*)(int)num, value);
		}
	}

	internal unsafe static void eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		try
		{
			eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoDestroyValues(P_0, (SrcFrame*)(int)(*(uint*)P_0), (SrcFrame*)(int)((uint*)P_0)[1]);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<VectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002EVectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), P_0);
			throw;
		}
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_003D(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0, vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* x)
	{
		if (x != P_0)
		{
			int num = ((int*)x)[1];
			int num2 = *(int*)x;
			uint num3 = (uint)((num - num2) / 24);
			int num4 = *(int*)P_0;
			vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr2;
			if (num3 > (uint)((((int*)P_0)[2] - num4) / 24))
			{
				SrcFrame* ptr = eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoRealloc_003Cstruct_0020SrcFrame_0020_002A_003E(P_0, num3, (SrcFrame*)num2, (SrcFrame*)num);
				ptr2 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
				eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoDestroyValues(P_0, (SrcFrame*)(int)(*(uint*)P_0), (SrcFrame*)(int)(*(uint*)ptr2));
				SrcFrame* ptr3 = (SrcFrame*)(int)(*(uint*)P_0);
				if (ptr3 != null)
				{
					delete_005B_005D(ptr3);
				}
				*(int*)P_0 = (int)ptr;
				((int*)P_0)[2] = (int)((int)(num3 * 24) + (byte*)ptr);
			}
			else
			{
				ptr2 = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
				int num5 = (*(int*)ptr2 - num4) / 24;
				if (num3 > (uint)num5)
				{
					eastl_002Ecopy_chooser_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E((SrcFrame*)num2, (SrcFrame*)(num5 * 24 + num2), (SrcFrame*)num4);
					int num6 = *(int*)ptr2;
					eastl_002Euninitialized_copy_ptr_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E((SrcFrame*)((num6 - *(int*)P_0) / 24 * 24 + *(int*)x), (SrcFrame*)(int)((uint*)x)[1], (SrcFrame*)num6);
				}
				else
				{
					SrcFrame* result = (SrcFrame*)num4;
					SrcFrame* last = (SrcFrame*)num;
					SrcFrame* first = eastl_002Ecopy_chooser_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E((SrcFrame*)num2, last, result);
					eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoDestroyValues(P_0, first, (SrcFrame*)(int)(*(uint*)ptr2));
				}
			}
			*(int*)ptr2 = (int)(num3 * 24) + *(int*)P_0;
		}
		return P_0;
	}

	internal unsafe static SrcFrame* eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002Eerase(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0, SrcFrame* position)
	{
		SrcFrame* ptr = (SrcFrame*)((byte*)position + 24);
		uint num = ((uint*)P_0)[1];
		if ((nuint)ptr < (nuint)(int)num)
		{
			SrcFrame* last = (SrcFrame*)(int)num;
			eastl_002Ecopy_chooser_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(ptr, last, position);
		}
		((int*)P_0)[1] += -24;
		SrcFrame* ptr2 = (SrcFrame*)(int)((uint*)P_0)[1];
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr3 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr2 + 4);
		int num2 = *(int*)ptr3;
		if (num2 != 0)
		{
			delete_005B_005D((void*)num2);
		}
		return position;
	}

	internal unsafe static void eastl_002Evector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E_002Epush_back(vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* P_0, CompressedAdaptiveDelta.Frame* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num < (uint)((int*)P_0)[2])
		{
			int num2 = (int)num;
			((int*)P_0)[1] = (int)(num + 17);
			if (num2 != 0)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(num2, value, 17);
			}
		}
		else
		{
			eastl_002Evector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(P_0, (CompressedAdaptiveDelta.Frame*)(int)num, value);
		}
	}

	internal unsafe static void eastl_002Evector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E_002Eclear(vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = ((int*)P_0)[1];
		CompressedAdaptiveDelta.Frame* ptr = (CompressedAdaptiveDelta.Frame*)num;
		CompressedAdaptiveDelta.Frame* ptr2 = (CompressedAdaptiveDelta.Frame*)(int)(*(uint*)P_0);
		CompressedAdaptiveDelta.Frame* ptr3 = (CompressedAdaptiveDelta.Frame*)num;
		CompressedAdaptiveDelta.Frame* ptr4 = ptr2;
		CompressedAdaptiveDelta.Frame* ptr5 = ptr;
		if (ptr != ptr3)
		{
			do
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr4, ptr5, 17);
				ptr4 = (CompressedAdaptiveDelta.Frame*)((byte*)ptr4 + 17);
				ptr5 = (CompressedAdaptiveDelta.Frame*)((byte*)ptr5 + 17);
			}
			while (ptr5 != ptr3);
		}
		((int*)P_0)[1] = (int)(((int*)P_0)[1] + (nint)((byte*)ptr - (nuint)ptr2) / (nint)(-17) * 17);
	}

	internal unsafe static void SrcFrame_002E_007Bdtor_007D(SrcFrame* P_0)
	{
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
		int num = *(int*)ptr;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
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

	internal unsafe static void eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0, float* position, float* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num != (uint)((int*)P_0)[2])
		{
			float* ptr = value;
			if (value >= position && (nuint)value < (nuint)(int)num)
			{
				ptr = value + 1;
			}
			float* ptr2 = (float*)(int)num;
			if (ptr2 != null)
			{
				*ptr2 = *(float*)(int)(num - 4);
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
		float* ptr3;
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
		ptr3 = (float*)new_005B_005D(num4 << 2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		goto IL_0094;
		IL_0094:
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cfloat_0020_002A_002Cvoid_003E obj);
		*(int*)(&obj) = *(int*)P_0;
		float* ptr4 = (float*)((byte*)position - *(int*)(&obj));
		float* ptr5 = ((nint)ptr4 >> 2) + (float*)memmove(ptr3, (void*)(int)(*(uint*)(&obj)), (uint)ptr4);
		if (ptr5 != null)
		{
			*ptr5 = *value;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cfloat_0020_002A_002Cvoid_003E obj2);
		*(int*)(&obj2) = (int)(ptr5 + 1);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cfloat_0020_002A_002Cvoid_003E obj3);
		*(int*)(&obj3) = ((int*)P_0)[1];
		float* ptr6 = (float*)((nuint)(*(int*)(&obj3)) - (nuint)position);
		ptr5 = ((nint)ptr6 >> 2) + (float*)memmove((void*)(int)(*(uint*)(&obj2)), position, (uint)ptr6);
		float* ptr7 = (float*)(int)(*(uint*)P_0);
		if (ptr7 != null)
		{
			delete_005B_005D(ptr7);
		}
		*(int*)P_0 = (int)ptr3;
		((int*)P_0)[1] = (int)ptr5;
		((int*)P_0)[2] = (int)((int)(num4 * 4) + (byte*)ptr3);
	}

	internal unsafe static void eastl_002EVectorBase_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(VectorBase_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)(int)num);
		}
	}

	internal unsafe static void eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoDestroyValues(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0, SrcFrame* first, SrcFrame* last)
	{
		if (first >= last)
		{
			return;
		}
		do
		{
			vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)first + 4);
			int num = *(int*)ptr;
			if (num != 0)
			{
				delete_005B_005D((void*)num);
			}
			first = (SrcFrame*)((byte*)first + 24);
		}
		while (first < last);
	}

	internal unsafe static void eastl_002EVectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(VectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)(int)num);
		}
	}

	internal unsafe static SrcFrame* eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoRealloc_003Cstruct_0020SrcFrame_0020_002A_003E(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0, uint n, SrcFrame* first, SrcFrame* last)
	{
		SrcFrame* ptr = (SrcFrame*)((n == 0) ? null : new_005B_005D(n * 24, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209));
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E dest);
		*(int*)(&dest) = (int)ptr;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E last2);
		*(int*)(&last2) = (int)last;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E first2);
		*(int*)(&first2) = (int)first;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E obj);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed);
		eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_0020_003E(&obj, first2, last2, dest, _unnamed);
		return ptr;
	}

	internal unsafe static SrcFrame* eastl_002Euninitialized_copy_ptr_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(SrcFrame* first, SrcFrame* last, SrcFrame* result)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E dest);
		*(int*)(&dest) = (int)result;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E last2);
		*(int*)(&last2) = (int)last;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E first2);
		*(int*)(&first2) = (int)first;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E obj);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed);
		eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_0020_003E(&obj, first2, last2, dest, _unnamed);
		return (SrcFrame*)(int)(*(uint*)(&obj));
	}

	internal unsafe static void eastl_002Evector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(vector_003CCompressedAdaptiveDelta_003A_003AFrame_002Ceastl_003A_003Aallocator_003E* P_0, CompressedAdaptiveDelta.Frame* position, CompressedAdaptiveDelta.Frame* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num != (uint)((int*)P_0)[2])
		{
			CompressedAdaptiveDelta.Frame* ptr = value;
			if (value >= position && (nuint)value < (nuint)(int)num)
			{
				ptr = (CompressedAdaptiveDelta.Frame*)((byte*)value + 17);
			}
			CompressedAdaptiveDelta.Frame* ptr2 = (CompressedAdaptiveDelta.Frame*)(int)num;
			if (ptr2 != null)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr2, (int)(num - 17), 17);
			}
			num = ((uint*)P_0)[1];
			CompressedAdaptiveDelta.Frame* ptr3 = (CompressedAdaptiveDelta.Frame*)(int)num;
			CompressedAdaptiveDelta.Frame* ptr4 = (CompressedAdaptiveDelta.Frame*)(int)(num - 17);
			if (ptr4 != position)
			{
				do
				{
					ptr4 = (CompressedAdaptiveDelta.Frame*)((byte*)ptr4 - 17);
					ptr3 = (CompressedAdaptiveDelta.Frame*)((byte*)ptr3 - 17);
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr3, ptr4, 17);
				}
				while (ptr4 != position);
			}
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(position, ptr, 17);
			((int*)P_0)[1] += 17;
			return;
		}
		uint num2 = (uint)(((int)num - *(int*)P_0) / 17);
		uint num3;
		CompressedAdaptiveDelta.Frame* ptr5;
		if (num2 != 0)
		{
			num3 = num2 << 1;
			if (num3 == 0)
			{
				ptr5 = null;
				goto IL_00b5;
			}
		}
		else
		{
			num3 = 1u;
		}
		ptr5 = (CompressedAdaptiveDelta.Frame*)new_005B_005D(num3 * 17, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		goto IL_00b5;
		IL_00b5:
		CompressedAdaptiveDelta.Frame* ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_003E((CompressedAdaptiveDelta.Frame*)(int)(*(uint*)P_0), position, ptr5);
		if (ptr6 != null)
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr6, value, 17);
		}
		ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_003E(position, (CompressedAdaptiveDelta.Frame*)(int)((uint*)P_0)[1], (CompressedAdaptiveDelta.Frame*)((byte*)ptr6 + 17));
		CompressedAdaptiveDelta.Frame* ptr7 = (CompressedAdaptiveDelta.Frame*)(int)(*(uint*)P_0);
		if (ptr7 != null)
		{
			delete_005B_005D(ptr7);
		}
		*(int*)P_0 = (int)ptr5;
		((int*)P_0)[1] = (int)ptr6;
		((int*)P_0)[2] = (int)((int)(num3 * 17) + (byte*)ptr5);
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

	internal unsafe static generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E* eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_0020_003E(generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E* P_0, generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E first, generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E last, generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E dest, integral_constant_003Cbool_002C0_003E __unnamed003)
	{
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(P_0, ref dest, 4);
		while ((byte)((*(int*)(&first) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			void* ptr = (void*)(int)(*(uint*)P_0);
			SrcFrame* ptr2 = (SrcFrame*)ptr;
			try
			{
				if (ptr != null)
				{
					*(int*)ptr = *(int*)(int)(*(uint*)(&first));
					eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bctor_007D((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr + 4), (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(*(int*)(&first) + 4));
					try
					{
						((sbyte*)ptr)[20] = *(sbyte*)(*(int*)(&first) + 20);
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)ptr2 + 4);
						throw;
					}
				}
			}
			catch
			{
				//try-fault
				delete(ptr2, ptr);
				throw;
			}
			*(int*)(&first) += 24;
			*(int*)P_0 += 24;
		}
		return P_0;
	}

	internal unsafe static CompressedAdaptiveDelta.Frame* eastl_002Euninitialized_copy_ptr_003Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_003E(CompressedAdaptiveDelta.Frame* first, CompressedAdaptiveDelta.Frame* last, CompressedAdaptiveDelta.Frame* result)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E dest);
		*(int*)(&dest) = (int)result;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E last2);
		*(int*)(&last2) = (int)last;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E first2);
		*(int*)(&first2) = (int)first;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E obj);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed);
		eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E_0020_003E(&obj, first2, last2, dest, _unnamed);
		return (CompressedAdaptiveDelta.Frame*)(int)(*(uint*)(&obj));
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

	internal unsafe static SrcFrame* eastl_002Ecopy_chooser_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(SrcFrame* first, SrcFrame* last, SrcFrame* result)
	{
		SrcFrame* ptr = result;
		SrcFrame* ptr2 = first;
		if (first != last)
		{
			int num = (int)((byte*)first - (nuint)result);
			do
			{
				*(int*)ptr = *(int*)ptr2;
				eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_003D((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr + 4), (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr + num + 4));
				((sbyte*)ptr)[20] = ((sbyte*)ptr2)[20];
				ptr = (SrcFrame*)((byte*)ptr + 24);
				ptr2 = (SrcFrame*)((byte*)ptr2 + 24);
			}
			while (ptr2 != last);
		}
		return ptr;
	}

	internal unsafe static generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E* eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020CompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E_0020_003E(generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E* P_0, generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E first, generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E last, generic_iterator_003CCompressedAdaptiveDelta_003A_003AFrame_0020_002A_002Cvoid_003E dest, integral_constant_003Cbool_002C0_003E __unnamed003)
	{
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(P_0, ref dest, 4);
		while ((byte)((*(int*)(&first) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			int num = *(int*)P_0;
			void* ptr = (void*)num;
			CompressedAdaptiveDelta.Frame* ptr2 = (CompressedAdaptiveDelta.Frame*)ptr;
			try
			{
				if (ptr != null)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(ptr, *(int*)(&first), 17);
				}
			}
			catch
			{
				//try-fault
				delete(ptr2, ptr);
				throw;
			}
			*(int*)(&first) += 17;
			*(int*)P_0 = num + 17;
		}
		return P_0;
	}

	[SpecialName]
	internal unsafe static vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bctor_007D(vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0, vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* x)
	{
		uint num = (uint)(((int*)x)[1] - *(int*)x >> 2);
		float* ptr = (float*)((num == 0) ? null : new_005B_005D(num << 2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209));
		*(int*)P_0 = (int)ptr;
		vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
		*(int*)ptr2 = (int)ptr;
		((int*)P_0)[2] = (int)((int)(num * 4) + (byte*)ptr);
		try
		{
			float* ptr3 = (float*)(int)(*(uint*)P_0);
			int num2 = ((int*)x)[1];
			float* ptr4 = (float*)(int)(*(uint*)x);
			uint num3 = (uint)((nuint)num2 - (nuint)ptr4);
			*(int*)ptr2 = (int)(((int)num3 >> 2) * 4 + (byte*)memmove(ptr3, ptr4, num3));
			return P_0;
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<VectorBase_003Cfloat_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002EVectorBase_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), P_0);
			throw;
		}
	}

	internal unsafe static vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_003D(vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0, vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* x)
	{
		if (x != P_0)
		{
			int num = ((int*)x)[1];
			int num2 = *(int*)x;
			uint num3 = (uint)(num - num2 >> 2);
			int num4 = *(int*)P_0;
			vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr2;
			if (num3 > (uint)(((int*)P_0)[2] - num4 >> 2))
			{
				float* ptr = eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002EDoRealloc_003Cfloat_0020_002A_003E(P_0, num3, (float*)num2, (float*)num);
				ptr2 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
				float* ptr3 = (float*)(int)(*(uint*)P_0);
				if (ptr3 != null)
				{
					delete_005B_005D(ptr3);
				}
				*(int*)P_0 = (int)ptr;
				((int*)P_0)[2] = (int)((int)(num3 * 4) + (byte*)ptr);
			}
			else
			{
				ptr2 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
				int num5 = *(int*)ptr2 - num4 >> 2;
				if (num3 > (uint)num5)
				{
					float* ptr4 = (float*)num2;
					memmove((void*)num4, ptr4, (uint)((nint)(num5 << 2) - (nint)ptr4 + num2));
					System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cfloat_0020_002A_002Cvoid_003E obj);
					uint num6 = (uint)(*(int*)(&obj) = *(int*)ptr2);
					System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cfloat_0020_002A_002Cvoid_003E obj2);
					*(int*)(&obj2) = ((int*)x)[1];
					System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cfloat_0020_002A_002Cvoid_003E obj3);
					*(int*)(&obj3) = ((int)num6 - *(int*)P_0 >> 2) * 4 + *(int*)x;
					memmove((void*)(int)(*(uint*)(&obj)), (void*)(int)(*(uint*)(&obj3)), (uint)(*(int*)(&obj2) - *(int*)(&obj3)));
				}
				else
				{
					float* ptr5 = (float*)num;
					float* ptr6 = (float*)num2;
					memmove((void*)num4, ptr6, (uint)((byte*)ptr5 - (nuint)ptr6));
				}
			}
			*(int*)ptr2 = (int)(num3 * 4) + *(int*)P_0;
		}
		return P_0;
	}

	internal unsafe static float* eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002EDoRealloc_003Cfloat_0020_002A_003E(vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* P_0, uint n, float* first, float* last)
	{
		float* ptr = (float*)((n == 0) ? null : new_005B_005D(n << 2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209));
		memmove(ptr, first, (uint)((byte*)last - (nuint)first));
		return ptr;
	}

	internal unsafe static void Marshaler_002ENodeHolder_002E_007Bdtor_007D(NodeHolder* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)num)))((IntPtr)(int)num);
		}
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

	internal unsafe static void Relo_002ETracker_002EAddReference(Tracker* P_0, void* location, uint value)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out Tracker.GenericBookmark_003Cunsigned_0020int_003E obj);
		System.Runtime.CompilerServices.Unsafe.As<Tracker.GenericBookmark_003Cunsigned_0020int_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4)) = (int)((byte*)location - *(int*)((*(int*)(&obj) = *(int*)(((int*)P_0)[2] - 4)) * 12 + ((int*)P_0)[5]));
		System.Runtime.CompilerServices.Unsafe.As<Tracker.GenericBookmark_003Cunsigned_0020int_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 8)) = value;
		*(uint*)location = value;
		vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 52);
		uint num = ((uint*)ptr)[1];
		if (num < (uint)((int*)ptr)[2])
		{
			int num2 = (int)num;
			((int*)ptr)[1] = (int)(num + 12);
			if (num2 != 0)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(num2, ref obj, 12);
			}
		}
		else
		{
			eastl_002Evector_003CRelo_003A_003ATracker_003A_003AGenericBookmark_003Cunsigned_0020int_003E_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(ptr, (Tracker.GenericBookmark_003Cunsigned_0020int_003E*)(int)num, &obj);
		}
	}

	internal unsafe static void eastl_002Estack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E_002E_007Bdtor_007D(stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal static void _003FA0x325460ff_002E_003F_003F__E_003FA0x325460ff_0040SECONDS_PER_LOGICFRAME_REAL_0040DataInitializers_0040_0040YMXXZ()
	{
		DataInitializers_002E_003FA0x325460ff_002ESECONDS_PER_LOGICFRAME_REAL = (float)(1.0 / (double)DataInitializers_002E_003FA0x325460ff_002ELOGICFRAMES_PER_SECONDS_REAL);
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, RGBAColor* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01DGKLNCNG_0040R_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01BBODEMC_0040G_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((float*)@object)[1] = (float)atof(ptr4);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
			IValue* ptr5 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01HMGJMAIH_0040B_003F_0024AA_0040), null);
			if (ptr5 != null)
			{
				sbyte* ptr6 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr5)))((nint)ptr5);
				((float*)@object)[2] = (float)atof(ptr6);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr5 + 4)))((nint)ptr5);
			}
			IValue* ptr7 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01FHEEJDEE_0040A_003F_0024AA_0040), null);
			if (ptr7 != null)
			{
				sbyte* ptr8 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr7)))((nint)ptr7);
				((float*)@object)[3] = (float)atof(ptr8);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr7 + 4)))((nint)ptr7);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, BoneInfluence* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06ODJMJFLN_0040Weight_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04CCGIJNFG_0040Bone_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((short*)@object)[2] = (short)strtoul(ptr4, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, FXShaderConstant* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04FABLJDN_0040Name_003F_0024AA_0040), null);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), (String_003Cchar_003E*)((byte*)@object + 4), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, Vector3* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01MMEEDKFM_0040X_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01NFFPALBN_0040Y_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((float*)@object)[1] = (float)atof(ptr4);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
			IValue* ptr5 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01POHCFINO_0040Z_003F_0024AA_0040), null);
			if (ptr5 != null)
			{
				sbyte* ptr6 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr5)))((nint)ptr5);
				((float*)@object)[2] = (float)atof(ptr6);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr5 + 4)))((nint)ptr5);
			}
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(Vector3* @object, EndianHelper* state)
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
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(byte*)(&num2) = ((byte*)@object + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((byte*)@object + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((byte*)@object + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = ((sbyte*)@object)[4];
			((int*)@object)[1] = (int)num2;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(byte*)(&num3) = ((byte*)@object + 8)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((byte*)@object + 8)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((byte*)@object + 8)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = ((sbyte*)@object)[8];
			((int*)@object)[2] = (int)num3;
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, Vector2* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01MMEEDKFM_0040X_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01NFFPALBN_0040Y_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((float*)@object)[1] = (float)atof(ptr4);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, Quaternion* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01MMEEDKFM_0040X_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01NFFPALBN_0040Y_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((float*)@object)[1] = (float)atof(ptr4);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
			IValue* ptr5 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01POHCFINO_0040Z_003F_0024AA_0040), null);
			if (ptr5 != null)
			{
				sbyte* ptr6 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr5)))((nint)ptr5);
				((float*)@object)[2] = (float)atof(ptr6);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr5 + 4)))((nint)ptr5);
			}
			IValue* ptr7 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01ELNMCGJD_0040W_003F_0024AA_0040), null);
			if (ptr7 != null)
			{
				sbyte* ptr8 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr7)))((nint)ptr7);
				((float*)@object)[3] = (float)atof(ptr8);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr7 + 4)))((nint)ptr7);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, FXShaderConstantBool* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LPIJGKJ_0040Value_003F_0024AA_0040), null);
		try
		{
			INode* ptr = (INode*)(int)(*(uint*)num);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, IValue*>)(int)(*(uint*)(*(int*)ptr + 16)))((nint)ptr), (bool*)@object + 12, state);
			}
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
		Marshaler_002EMarshal(node, (FXShaderConstant*)@object, state);
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AABTree.Node.Children* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05HOHBLJAP_0040Front_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(uint*)@object = strtoul(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04FEOMFELI_0040Back_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((int*)@object)[1] = (int)strtoul(ptr4, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AABTree.Node.Polys* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05EHPGHEDK_0040Begin_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(uint*)@object = strtoul(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05IJGIMMHE_0040Count_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((int*)@object)[1] = (int)strtoul(ptr4, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AABTree.Node* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_03LLNBGIF_0040Min_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal((INode*)(int)(*(uint*)num), (Vector3*)@object, state);
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
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder2);
		int num2 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_03BJDGPCOK_0040Max_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal((INode*)(int)(*(uint*)num2), (Vector3*)((byte*)@object + 12), state);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder2);
			throw;
		}
		if (*(int*)(&nodeHolder2) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder2)))))((IntPtr)(*(int*)(&nodeHolder2)));
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder3);
		int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_08IEBJFPPC_0040Children_003F_0024AA_0040), null);
		try
		{
			INode* ptr = (INode*)(int)(*(uint*)num3);
			AABTree.Node.Children** ptr2 = (AABTree.Node.Children**)((byte*)@object + 24);
			if (ptr != null)
			{
				Relo_002ETracker_002EPush(state, (void**)ptr2, 8u, 1u);
				Marshaler_002EMarshal(ptr, (AABTree.Node.Children*)(int)(*(uint*)ptr2), state);
				((int*)((byte*)state + 4))[1] += -4;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder3);
			throw;
		}
		if (*(int*)(&nodeHolder3) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder3)))))((IntPtr)(*(int*)(&nodeHolder3)));
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder4);
		int num4 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder4, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05HHBDIHGG_0040Polys_003F_0024AA_0040), null);
		try
		{
			INode* ptr3 = (INode*)(int)(*(uint*)num4);
			AABTree.Node.Polys** ptr4 = (AABTree.Node.Polys**)((byte*)@object + 28);
			if (ptr3 != null)
			{
				Relo_002ETracker_002EPush(state, (void**)ptr4, 8u, 1u);
				Marshaler_002EMarshal(ptr3, (AABTree.Node.Polys*)(int)(*(uint*)ptr4), state);
				((int*)((byte*)state + 4))[1] += -4;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder4);
			throw;
		}
		if (*(int*)(&nodeHolder4) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder4)))))((IntPtr)(*(int*)(&nodeHolder4)));
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AABTree* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0M_0040GFCHFNAP_0040PolyIndices_003F_0024AA_0040), null);
		try
		{
			INode* ptr = (INode*)(int)(*(uint*)num);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cunsigned_0020int_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01EJNLAFE_0040P_003F_0024AA_0040)), (List_003Cunsigned_0020int_003E*)@object, state);
			}
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
		Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAABTree_003A_003ANode_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04OKAPPFIB_0040Node_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003AAABTree_003A_003ANode_003E*)((byte*)@object + 8), state);
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, FXShaderConstant** pointer, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06BCNEEODO_0040TypeId_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GBGANLPD_00400_003F_0024AA_0040));
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			if (ptr != null)
			{
				num = strtoul(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			switch (num)
			{
			case 2750958653u:
				Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantBool_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(node, pointer, state);
				break;
			case 2300057986u:
				Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantInt_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(node, pointer, state);
				break;
			case 2777716390u:
				Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantTexture_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(node, pointer, state);
				break;
			default:
				Marshaler_002EMarshalUnknownPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(node, pointer, state);
				break;
			case 3741900581u:
				Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantFloat_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(node, pointer, state);
				break;
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, FXShaderMaterial* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040LBFPEED_0040ShaderName_003F_0024AA_0040), null);
		if (ptr != null)
		{
			Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), (String_003Cchar_003E*)@object, state);
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
		}
		IValue* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0O_0040DDEFCAPO_0040TechniqueName_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_00CNPNBAHC_0040_003F_0024AA_0040));
		if (ptr2 != null)
		{
			Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr2)))((nint)ptr2), (String_003Cchar_003E*)((byte*)@object + 8), state);
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr2 + 4)))((nint)ptr2);
		}
		IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0P_0040FPINIBCG_0040TechniqueIndex_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GBGANLPD_00400_003F_0024AA_0040));
		if (ptr3 != null)
		{
			sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
			((sbyte*)@object)[24] = (sbyte)strtoul(ptr4, null, 0);
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09EKDBGGMD_0040Constants_003F_0024AA_0040), null);
		try
		{
			INode* ptr5 = (INode*)(int)(*(uint*)num);
			if (ptr5 != null)
			{
				Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_0020_002A_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr5 + 4)))((nint)ptr5, null), (List_003CSageBinaryData_003A_003AFXShaderConstant_0020_002A_003E*)((byte*)@object + 16), state);
			}
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
	}

	internal unsafe static void Marshaler_002EEndianCorrect(FXShaderConstant** pointer, EndianHelper* state)
	{
		uint num = *(uint*)pointer;
		if (num != 0)
		{
			switch (*(int*)(int)num)
			{
			case -1544008643:
				Marshaler_002EEndianCorrectPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantBool_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(pointer, state);
				break;
			case -1994909310:
				Marshaler_002EEndianCorrect((FXShaderConstantInt*)(int)num, state);
				break;
			case -1517250906:
				Marshaler_002EEndianCorrectPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantTexture_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(pointer, state);
				break;
			default:
				Marshaler_002EEndianCorrectUnknownPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(pointer, state);
				break;
			case -553066715:
				Marshaler_002EEndianCorrect((FXShaderConstantFloat*)(int)num, state);
				break;
			}
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(FXShaderMaterial* @object, EndianHelper* state)
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
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(byte*)(&num2) = ((byte*)@object + 8)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((byte*)@object + 8)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((byte*)@object + 8)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = ((sbyte*)@object)[8];
			((int*)@object)[2] = (int)num2;
		}
		Marshaler_002EEndianCorrect_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_0020_002A_003E((List_003CSageBinaryData_003A_003AFXShaderConstant_0020_002A_003E*)((byte*)@object + 16), state);
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AnimationChannelBase* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JABNIMDI_0040Pivot_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				((short*)@object)[6] = (short)strtoul(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AAnimationChannelType_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04KOACHJEN_0040Type_003F_0024AA_0040), null), (AnimationChannelType*)@object + 1, state);
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040OCNGDGDE_0040FirstFrame_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((int*)@object)[2] = (int)strtoul(ptr4, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, Sphere* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06GJMFONGK_0040Radius_003F_0024AA_0040), null);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
			int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06NOADDNNE_0040Center_003F_0024AA_0040), null);
			try
			{
				Marshaler_002EMarshal((INode*)(int)(*(uint*)num), (Vector3*)((byte*)@object + 4), state);
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
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, BoxMinMax* @object, Tracker* state)
	{
		if (node != null)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
			int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_03LLNBGIF_0040Min_003F_0024AA_0040), null);
			try
			{
				Marshaler_002EMarshal((INode*)(int)(*(uint*)num), (Vector3*)@object, state);
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
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder2);
			int num2 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_03BJDGPCOK_0040Max_003F_0024AA_0040), null);
			try
			{
				Marshaler_002EMarshal((INode*)(int)(*(uint*)num2), (Vector3*)((byte*)@object + 12), state);
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder2);
				throw;
			}
			if (*(int*)(&nodeHolder2) != 0)
			{
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder2)))))((IntPtr)(*(int*)(&nodeHolder2)));
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, Triangle* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		Marshaler_002EMarshal_003Cunsigned_0020int_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01FCMHBHNC_0040V_003F_0024AA_0040)), (List_003Cunsigned_0020int_003E*)@object, state);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_03CCFGKGDJ_0040Nrm_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal((INode*)(int)(*(uint*)num), (Vector3*)((byte*)@object + 8), state);
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
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder2);
		int num2 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04MPGECEBJ_0040Dist_003F_0024AA_0040), null);
		try
		{
			INode* ptr = (INode*)(int)(*(uint*)num2);
			if (ptr != null)
			{
				IValue* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, IValue*>)(int)(*(uint*)(*(int*)ptr + 16)))((nint)ptr);
				if (ptr2 != null)
				{
					sbyte* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr2)))((nint)ptr2);
					((float*)@object)[5] = (float)atof(ptr3);
					((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr2 + 4)))((nint)ptr2);
				}
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder2);
			throw;
		}
		if (*(int*)(&nodeHolder2) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder2)))))((IntPtr)(*(int*)(&nodeHolder2)));
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(FXShaderConstantFloat* @object, EndianHelper* state)
	{
		Marshaler_002EEndianCorrect_003Cfloat_003E((List_003Cfloat_003E*)((byte*)@object + 12), state);
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
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(byte*)(&num2) = ((byte*)@object + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((byte*)@object + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((byte*)@object + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = ((sbyte*)@object)[4];
			((int*)@object)[1] = (int)num2;
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, FXShaderConstantInt* @object, Tracker* state)
	{
		if (node != null)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
			int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LPIJGKJ_0040Value_003F_0024AA_0040), null);
			try
			{
				Marshaler_002EMarshal_003Cint_002Cclass_0020Relo_003A_003ATracker_003E((INode*)(int)(*(uint*)num), (int*)@object + 3, state);
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
			Marshaler_002EMarshal(node, (FXShaderConstant*)@object, state);
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(FXShaderConstantInt* @object, EndianHelper* state)
	{
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(byte*)(&num) = ((byte*)@object + 12)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((byte*)@object + 12)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((byte*)@object + 12)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = ((sbyte*)@object)[12];
			((int*)@object)[3] = (int)num;
			if (*(bool*)state)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
				*(sbyte*)(&num2) = ((sbyte*)@object)[3];
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((sbyte*)@object)[2];
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((sbyte*)@object)[1];
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)@object;
				*(uint*)@object = num2;
			}
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(byte*)(&num3) = ((byte*)@object + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((byte*)@object + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((byte*)@object + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = ((sbyte*)@object)[4];
			((int*)@object)[1] = (int)num3;
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AnimationChannelScalar.Frame* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, IValue*>)(int)(*(uint*)(*(int*)node + 16)))((nint)node);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040GDEEFGDC_0040BinaryMove_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LAPONLG_0040false_003F_0024AA_0040)), (bool*)@object + 4, state);
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, FXShaderConstantTexture* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LPIJGKJ_0040Value_003F_0024AA_0040), null);
		try
		{
			INode* ptr = (INode*)(int)(*(uint*)num);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AAssetReference_003Cstruct_0020SageBinaryData_003A_003ATexture_002Cclass_0020RNA_003A_003ATextureC_003E_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, IValue*>)(int)(*(uint*)(*(int*)ptr + 16)))((nint)ptr), (AssetReference_003CSageBinaryData_003A_003ATexture_002CRNA_003A_003ATextureC_003E*)((byte*)@object + 12), state);
			}
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
		Marshaler_002EMarshal(node, (FXShaderConstant*)@object, state);
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, W3DMeshPipelineVertexData* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003AVertices_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_08JMEOJNII_0040Vertices_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003AVertices_003E*)@object, state);
		Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ANormals_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_07DKDJFLGE_0040Normals_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ANormals_003E*)((byte*)@object + 8), state);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_08DJLAMBOA_0040Tangents_003F_0024AA_0040), null);
		try
		{
			INode* ptr = (INode*)(int)(*(uint*)num);
			W3DMeshPipelineVertexData.Tangents** ptr2 = (W3DMeshPipelineVertexData.Tangents**)((byte*)@object + 16);
			if (ptr != null)
			{
				Relo_002ETracker_002EPush(state, (void**)ptr2, 8u, 1u);
				W3DMeshPipelineVertexData.Tangents* target = (W3DMeshPipelineVertexData.Tangents*)(int)(*(uint*)ptr2);
				Marshaler_002EMarshal_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector3_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GAPBHFFA_0040T_003F_0024AA_0040)), (List_003CSageMathOnly_003A_003AMath_003A_003AVector3_003E*)target, state);
				((int*)((byte*)state + 4))[1] += -4;
			}
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
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder2);
		int num2 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09CPGIGOEP_0040Binormals_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ABinormals_003E((INode*)(int)(*(uint*)num2), (W3DMeshPipelineVertexData.Binormals**)((byte*)@object + 20), state);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder2);
			throw;
		}
		if (*(int*)(&nodeHolder2) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder2)))))((IntPtr)(*(int*)(&nodeHolder2)));
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder3);
		int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0N_0040DJBJPDKO_0040VertexColors_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003AVertexColors_003E((INode*)(int)(*(uint*)num3), (W3DMeshPipelineVertexData.VertexColors**)((byte*)@object + 24), state);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder3);
			throw;
		}
		if (*(int*)(&nodeHolder3) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder3)))))((IntPtr)(*(int*)(&nodeHolder3)));
		}
		Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ATexCoords_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09NIBCOMFM_0040TexCoords_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ATexCoords_003E*)((byte*)@object + 28), state);
		Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ABoneInfluences_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0P_0040LNBMENPE_0040BoneInfluences_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ABoneInfluences_003E*)((byte*)@object + 36), state);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder4);
		int num4 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder4, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0N_0040LMLGBMDA_0040ShadeIndices_003F_0024AA_0040), null);
		try
		{
			INode* ptr3 = (INode*)(int)(*(uint*)num4);
			W3DMeshPipelineVertexData.ShadeIndices** ptr4 = (W3DMeshPipelineVertexData.ShadeIndices**)((byte*)@object + 44);
			if (ptr3 != null)
			{
				Relo_002ETracker_002EPush(state, (void**)ptr4, 8u, 1u);
				W3DMeshPipelineVertexData.ShadeIndices* target2 = (W3DMeshPipelineVertexData.ShadeIndices*)(int)(*(uint*)ptr4);
				Marshaler_002EMarshal_003Cunsigned_0020int_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01JPJNBJEM_0040I_003F_0024AA_0040)), (List_003Cunsigned_0020int_003E*)target2, state);
				((int*)((byte*)state + 4))[1] += -4;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder4);
			throw;
		}
		if (*(int*)(&nodeHolder4) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder4)))))((IntPtr)(*(int*)(&nodeHolder4)));
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, W3DAnimation.CompressionSettings* @object, Tracker* state)
	{
		if (node != null)
		{
			Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0P_0040FPKKANKC_0040AllowTimeCoded_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040)), (bool*)@object + 20, state);
			Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BD_0040PACLLEOL_0040AllowAdaptiveDelta_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040)), (bool*)@object + 21, state);
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BE_0040OJKBNDPA_0040MaxTranslationError_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05CGKLNCBH_00400_003F4001_003F_0024AA_0040));
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*(float*)@object = (float)atof(ptr2);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BB_0040OAPHHBAN_0040MaxRotationError_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04DKCHIKEL_00400_003F401_003F_0024AA_0040));
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((float*)@object)[1] = (float)atof(ptr4);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
			IValue* ptr5 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BD_0040KDDHFEEI_0040MaxVisibilityError_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04DKCHIKEL_00400_003F401_003F_0024AA_0040));
			if (ptr5 != null)
			{
				sbyte* ptr6 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr5)))((nint)ptr5);
				((float*)@object)[2] = (float)atof(ptr6);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr5 + 4)))((nint)ptr5);
			}
			IValue* ptr7 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BG_0040DFLPCJGM_0040MaxAdaptiveDeltaError_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04IBBOIMJ_00400_003F403_003F_0024AA_0040));
			if (ptr7 != null)
			{
				sbyte* ptr8 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr7)))((nint)ptr7);
				((float*)@object)[3] = (float)atof(ptr8);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr7 + 4)))((nint)ptr7);
			}
			IValue* ptr9 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BD_0040NJLHHJDO_0040ForceReductionRate_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_03HLLJOCDO_00401_003F40_003F_0024AA_0040));
			if (ptr9 != null)
			{
				sbyte* ptr10 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr9)))((nint)ptr9);
				((float*)@object)[4] = (float)atof(ptr10);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr9 + 4)))((nint)ptr9);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AnimationChannelBase** pointer, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06BCNEEODO_0040TypeId_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GBGANLPD_00400_003F_0024AA_0040));
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			if (ptr != null)
			{
				num = strtoul(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			switch (num)
			{
			default:
				Marshaler_002EMarshalUnknownPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_003E(node, pointer, state);
				break;
			case 4131577120u:
				Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelQuaternion_002Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_003E(node, pointer, state);
				break;
			case 1355976437u:
				Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelScalar_002Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_003E(node, pointer, state);
				break;
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, W3DAnimation* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AAssetReference_003Cstruct_0020SageBinaryData_003A_003AW3DHierarchy_002Cclass_0020Sage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09FDDEOHGF_0040Hierarchy_003F_0024AA_0040), null), (AssetReference_003CSageBinaryData_003A_003AW3DHierarchy_002CSage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E*)((byte*)@object + 4), state);
		IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09CLPNCNDO_0040NumFrames_003F_0024AA_0040), null);
		if (ptr != null)
		{
			sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
			((int*)@object)[2] = (int)strtoul(ptr2, null, 0);
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
		}
		IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09HFKGELHH_0040FrameRate_003F_0024AA_0040), null);
		if (ptr3 != null)
		{
			sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
			((int*)@object)[3] = (int)strtoul(ptr4, null, 0);
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BE_0040GOBLKNOJ_0040CompressionSettings_003F_0024AA_0040), null);
		try
		{
			INode* ptr5 = (INode*)(int)(*(uint*)num);
			W3DAnimation.CompressionSettings** ptr6 = (W3DAnimation.CompressionSettings**)((byte*)@object + 16);
			if (ptr5 != null)
			{
				Relo_002ETracker_002EPush(state, (void**)ptr6, 24u, 1u);
				Marshaler_002EMarshal(ptr5, (W3DAnimation.CompressionSettings*)(int)(*(uint*)ptr6), state);
				((int*)((byte*)state + 4))[1] += -4;
			}
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
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder2);
		int num2 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_08INPNJAIK_0040Channels_003F_0024AA_0040), null);
		try
		{
			INode* ptr7 = (INode*)(int)(*(uint*)num2);
			if (ptr7 != null)
			{
				Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_0020_002A_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr7 + 4)))((nint)ptr7, null), (List_003CSageBinaryData_003A_003AAnimationChannelBase_0020_002A_003E*)((byte*)@object + 20), state);
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder2);
			throw;
		}
		if (*(int*)(&nodeHolder2) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder2)))))((IntPtr)(*(int*)(&nodeHolder2)));
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, W3DMesh* @object, Tracker* state)
	{
		if (node == null)
		{
			return;
		}
		Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06FHBILLDG_0040Hidden_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LAPONLG_0040false_003F_0024AA_0040)), (bool*)@object + 92, state);
		Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040MBLKBNFB_0040CastShadow_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LAPONLG_0040false_003F_0024AA_0040)), (bool*)@object + 93, state);
		Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AMeshGeometryType_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0N_0040JJFKGPIN_0040GeometryType_003F_0024AA_0040), null), (MeshGeometryType*)@object + 2, state);
		IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09NLJGGGGI_0040SortLevel_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GBGANLPD_00400_003F_0024AA_0040));
		if (ptr != null)
		{
			sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
			((sbyte*)@object)[94] = (sbyte)strtoul(ptr2, null, 0);
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
		int num = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0M_0040NJIMOLLO_0040BoundingBox_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal((INode*)(int)(*(uint*)num), (BoxMinMax*)((byte*)@object + 12), state);
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
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder2);
		int num2 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0P_0040IGIKKCLE_0040BoundingSphere_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal((INode*)(int)(*(uint*)num2), (Sphere*)((byte*)@object + 36), state);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder2);
			throw;
		}
		if (*(int*)(&nodeHolder2) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder2)))))((IntPtr)(*(int*)(&nodeHolder2)));
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder3);
		int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09LCKIAIBL_0040Triangles_003F_0024AA_0040), null);
		try
		{
			INode* ptr3 = (INode*)(int)(*(uint*)num3);
			if (ptr3 != null)
			{
				Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ATriangle_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GAPBHFFA_0040T_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003ATriangle_003E*)((byte*)@object + 52), state);
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder3);
			throw;
		}
		if (*(int*)(&nodeHolder3) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder3)))))((IntPtr)(*(int*)(&nodeHolder3)));
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder4);
		int num4 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder4, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_08IHCECLKE_0040FXShader_003F_0024AA_0040), null);
		try
		{
			Marshaler_002EMarshal((INode*)(int)(*(uint*)num4), (FXShaderMaterial*)((byte*)@object + 60), state);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder4);
			throw;
		}
		if (*(int*)(&nodeHolder4) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder4)))))((IntPtr)(*(int*)(&nodeHolder4)));
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder5);
		int num5 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, sbyte*, sbyte*, NodeHolder*>)(int)(*(uint*)(*(int*)node + 8)))((nint)node, &nodeHolder5, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_07IFDFKAHK_0040AABTree_003F_0024AA_0040), null);
		try
		{
			INode* ptr4 = (INode*)(int)(*(uint*)num5);
			AABTree** ptr5 = (AABTree**)((byte*)@object + 88);
			if (ptr4 != null)
			{
				Relo_002ETracker_002EPush(state, (void**)ptr5, 16u, 1u);
				Marshaler_002EMarshal(ptr4, (AABTree*)(int)(*(uint*)ptr5), state);
				((int*)((byte*)state + 4))[1] += -4;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<NodeHolder*, void>)(&Marshaler_002ENodeHolder_002E_007Bdtor_007D), &nodeHolder5);
			throw;
		}
		if (*(int*)(&nodeHolder5) != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)(*(uint*)(&nodeHolder5)))))((IntPtr)(*(int*)(&nodeHolder5)));
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(W3DMesh* @object, EndianHelper* state)
	{
		int num = ((int*)@object)[2];
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(sbyte*)(&num2) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)(&num);
			num = (int)num2;
		}
		((int*)@object)[2] = num;
		BoxMinMax* ptr = (BoxMinMax*)((byte*)@object + 12);
		Marshaler_002EEndianCorrect((Vector3*)ptr, state);
		Marshaler_002EEndianCorrect((Vector3*)((byte*)ptr + 12), state);
		Sphere* ptr2 = (Sphere*)((byte*)@object + 36);
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(sbyte*)(&num3) = ((sbyte*)ptr2)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)ptr2)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((sbyte*)ptr2)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)ptr2;
			*(uint*)ptr2 = num3;
		}
		Marshaler_002EEndianCorrect((Vector3*)((byte*)ptr2 + 4), state);
		Marshaler_002EEndianCorrect_003Cstruct_0020SageBinaryData_003A_003ATriangle_003E((List_003CSageBinaryData_003A_003ATriangle_003E*)((byte*)@object + 52), state);
		Marshaler_002EEndianCorrect((FXShaderMaterial*)((byte*)@object + 60), state);
		uint num4 = ((uint*)@object)[22];
		if (num4 != 0)
		{
			AABTree* ptr3 = (AABTree*)(int)num4;
			Marshaler_002EEndianCorrect_003Cunsigned_0020int_003E((List_003Cunsigned_0020int_003E*)ptr3, state);
			Marshaler_002EEndianCorrect_003Cstruct_0020SageBinaryData_003A_003AAABTree_003A_003ANode_003E((List_003CSageBinaryData_003A_003AAABTree_003A_003ANode_003E*)((byte*)ptr3 + 8), state);
		}
	}

	internal unsafe static void* CompressedTimecoded_002E__delDtor(CompressedTimecoded* P_0, uint P_1)
	{
		vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* ptr = (vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 8);
		try
		{
			eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoDestroyValues(ptr, (SrcFrame*)(int)(*(uint*)ptr), (SrcFrame*)(int)((uint*)ptr)[1]);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<VectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002EVectorBase_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), ptr);
			throw;
		}
		int num = *(int*)ptr;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AAssetReference_003Cstruct_0020SageBinaryData_003A_003ATexture_002Cclass_0020RNA_003A_003ATextureC_003E_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, AssetReference_003CSageBinaryData_003A_003ATexture_002CRNA_003A_003ATextureC_003E* @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* ptr = strchr(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value), 92);
			if (ptr != null)
			{
				uint value2 = (uint)atoi(ptr + 1);
				Relo_002ETracker_002EAddReference(state, @object, value2);
			}
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cunsigned_0020int_003E(IList* list, List_003Cunsigned_0020int_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 4u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal_003Cunsigned_0020int_002Cclass_0020Relo_003A_003ATracker_003E((INode*)(int)(*(uint*)num3), (uint*)(int)(num2 * 4 + (uint)((int*)target)[1]), tracker);
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAABTree_003A_003ANode_003E(IList* list, List_003CSageBinaryData_003A_003AAABTree_003A_003ANode_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 32u, num);
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
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num4), (AABTree.Node*)(num2 + ((int*)target)[1]), tracker);
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
					num2 += 32;
					num3--;
				}
				while (num3 != 0);
			}
			((int*)((byte*)tracker + 4))[1] += -4;
		}
		((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)list + 12)))((nint)list);
	}

	internal unsafe static void Marshaler_002EEndianCorrect_003Cunsigned_0020int_003E(List_003Cunsigned_0020int_003E* list, EndianHelper* endian)
	{
		int num = ((int*)list)[1];
		uint* ptr = (uint*)num;
		uint* ptr2 = (uint*)(*(int*)list * 4 + num);
		if (ptr != ptr2)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			do
			{
				if (*(bool*)endian)
				{
					*(sbyte*)(&num2) = ((sbyte*)ptr)[3];
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((sbyte*)ptr)[2];
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((sbyte*)ptr)[1];
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)ptr;
					*ptr = num2;
				}
				ptr++;
			}
			while (ptr != ptr2);
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(sbyte*)(&num3) = ((sbyte*)list)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)list)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((sbyte*)list)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)list;
			*(uint*)list = num3;
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect_003Cstruct_0020SageBinaryData_003A_003AAABTree_003A_003ANode_003E(List_003CSageBinaryData_003A_003AAABTree_003A_003ANode_003E* list, EndianHelper* endian)
	{
		int num = ((int*)list)[1];
		AABTree.Node* ptr = (AABTree.Node*)num;
		AABTree.Node* ptr2 = (AABTree.Node*)(*(int*)list * 32 + num);
		if (ptr != ptr2)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num6);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num7);
			do
			{
				Marshaler_002EEndianCorrect((Vector3*)ptr, endian);
				Marshaler_002EEndianCorrect((Vector3*)((byte*)ptr + 12), endian);
				uint num2 = ((uint*)ptr)[6];
				if (num2 != 0)
				{
					AABTree.Node.Children* ptr3 = (AABTree.Node.Children*)(int)num2;
					if (*(bool*)endian)
					{
						*(sbyte*)(&num3) = ((sbyte*)ptr3)[3];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)ptr3)[2];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((sbyte*)ptr3)[1];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)ptr3;
						*(uint*)ptr3 = num3;
					}
					if (*(bool*)endian)
					{
						*(byte*)(&num4) = ((byte*)ptr3 + 4)[3];
						System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = ((byte*)ptr3 + 4)[2];
						System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2)) = ((byte*)ptr3 + 4)[1];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3)) = ((sbyte*)ptr3)[4];
						((int*)ptr3)[1] = (int)num4;
					}
				}
				uint num5 = ((uint*)ptr)[7];
				if (num5 != 0)
				{
					AABTree.Node.Polys* ptr4 = (AABTree.Node.Polys*)(int)num5;
					if (*(bool*)endian)
					{
						*(sbyte*)(&num6) = ((sbyte*)ptr4)[3];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 1)) = ((sbyte*)ptr4)[2];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 2)) = ((sbyte*)ptr4)[1];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 3)) = *(sbyte*)ptr4;
						*(uint*)ptr4 = num6;
					}
					if (*(bool*)endian)
					{
						*(byte*)(&num7) = ((byte*)ptr4 + 4)[3];
						System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 1)) = ((byte*)ptr4 + 4)[2];
						System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 2)) = ((byte*)ptr4 + 4)[1];
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num7, 3)) = ((sbyte*)ptr4)[4];
						((int*)ptr4)[1] = (int)num7;
					}
				}
				ptr = (AABTree.Node*)((byte*)ptr + 32);
			}
			while (ptr != ptr2);
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num8);
			*(sbyte*)(&num8) = ((sbyte*)list)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 1)) = ((sbyte*)list)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 2)) = ((sbyte*)list)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num8, 3)) = *(sbyte*)list;
			*(uint*)list = num8;
		}
	}

	internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantFloat_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(INode* node, FXShaderConstant** pointer, Tracker* tracker)
	{
		FXShaderConstantFloat* ptr = (FXShaderConstantFloat*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 20u, 1u);
		if (node != null)
		{
			Marshaler_002EMarshal_003Cfloat_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LPIJGKJ_0040Value_003F_0024AA_0040)), (List_003Cfloat_003E*)((byte*)ptr + 12), tracker);
			Marshaler_002EMarshal(node, (FXShaderConstant*)ptr, tracker);
		}
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = -553066715;
	}

	internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantBool_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(INode* node, FXShaderConstant** pointer, Tracker* tracker)
	{
		FXShaderConstantBool* ptr = (FXShaderConstantBool*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 16u, 1u);
		Marshaler_002EMarshal(node, ptr, tracker);
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = -1544008643;
	}

	internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantTexture_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(INode* node, FXShaderConstant** pointer, Tracker* tracker)
	{
		FXShaderConstantTexture* ptr = (FXShaderConstantTexture*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 16u, 1u);
		Marshaler_002EMarshal(node, ptr, tracker);
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = -1517250906;
	}

	internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantInt_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(INode* node, FXShaderConstant** pointer, Tracker* tracker)
	{
		FXShaderConstantInt* ptr = (FXShaderConstantInt*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 16u, 1u);
		Marshaler_002EMarshal(node, ptr, tracker);
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = -1994909310;
	}

	internal unsafe static void Marshaler_002EMarshalUnknownPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(INode* node, FXShaderConstant** pointer, Tracker* tracker)
	{
		FXShaderConstant* ptr = (FXShaderConstant*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 12u, 1u);
		if (node != null)
		{
			IValue* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04FABLJDN_0040Name_003F_0024AA_0040), null);
			if (ptr2 != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr2)))((nint)ptr2), (String_003Cchar_003E*)((byte*)ptr + 4), tracker);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr2 + 4)))((nint)ptr2);
			}
		}
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = 0;
	}

	internal unsafe static void Marshaler_002EEndianCorrectPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantBool_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(FXShaderConstant** pointer, EndianHelper* endian)
	{
		FXShaderConstantBool* ptr = (FXShaderConstantBool*)(int)(*(uint*)pointer);
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(sbyte*)(&num) = ((sbyte*)ptr)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((sbyte*)ptr)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((sbyte*)ptr)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)ptr;
			*(uint*)ptr = num;
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(byte*)(&num2) = ((byte*)ptr + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((byte*)ptr + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((byte*)ptr + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = ((sbyte*)ptr)[4];
			((int*)ptr)[1] = (int)num2;
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrectPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstantTexture_002Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(FXShaderConstant** pointer, EndianHelper* endian)
	{
		FXShaderConstantTexture* ptr = (FXShaderConstantTexture*)(int)(*(uint*)pointer);
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(sbyte*)(&num) = ((sbyte*)ptr)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((sbyte*)ptr)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((sbyte*)ptr)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)ptr;
			*(uint*)ptr = num;
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(byte*)(&num2) = ((byte*)ptr + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((byte*)ptr + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((byte*)ptr + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = ((sbyte*)ptr)[4];
			((int*)ptr)[1] = (int)num2;
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrectUnknownPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_003E(FXShaderConstant** pointer, EndianHelper* endian)
	{
		FXShaderConstant* ptr = (FXShaderConstant*)(int)(*(uint*)pointer);
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(sbyte*)(&num) = ((sbyte*)ptr)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((sbyte*)ptr)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((sbyte*)ptr)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = *(sbyte*)ptr;
			*(uint*)ptr = num;
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(byte*)(&num2) = ((byte*)ptr + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((byte*)ptr + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((byte*)ptr + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = ((sbyte*)ptr)[4];
			((int*)ptr)[1] = (int)num2;
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_0020_002A_003E(List_003CSageBinaryData_003A_003AFXShaderConstant_0020_002A_003E* list, EndianHelper* endian)
	{
		int num = ((int*)list)[1];
		FXShaderConstant** ptr = (FXShaderConstant**)num;
		FXShaderConstant** ptr2 = (FXShaderConstant**)(*(int*)list * 4 + num);
		if (ptr != ptr2)
		{
			do
			{
				Marshaler_002EEndianCorrect(ptr, endian);
				ptr = (FXShaderConstant**)((byte*)ptr + 4);
			}
			while (ptr != ptr2);
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(sbyte*)(&num2) = ((sbyte*)list)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((sbyte*)list)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((sbyte*)list)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)list;
			*(uint*)list = num2;
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AAnimationChannelType_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, AnimationChannelType* @object, Tracker* state)
	{
		if (value != null)
		{
			EnumNameGetter_003Cint_003E.EnumPair* ptr = EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value), (EnumNameGetter_003Cint_003E.EnumPair*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x325460ff_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4AnimationChannelType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4AnimationChannelType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B), 5u);
			int num = ((ptr == null) ? 3 : ((int*)ptr)[1]);
			*@object = (AnimationChannelType)num;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cint_002Cclass_0020Relo_003A_003ATracker_003E(INode* node, int* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, IValue*>)(int)(*(uint*)(*(int*)node + 16)))((nint)node);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*@object = strtol(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
		}
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelScalar_003A_003AFrame_003E(IList* list, List_003CSageBinaryData_003A_003AAnimationChannelScalar_003A_003AFrame_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 8u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num3), (AnimationChannelScalar.Frame*)(int)(num2 * 8 + (uint)((int*)target)[1]), tracker);
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelQuaternion_003A_003AFrame_003E(IList* list, List_003CSageBinaryData_003A_003AAnimationChannelQuaternion_003A_003AFrame_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 20u, num);
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
						INode* ptr = (INode*)(int)(*(uint*)num4);
						AnimationChannelQuaternion.Frame* ptr2 = (AnimationChannelQuaternion.Frame*)(num2 + ((int*)target)[1]);
						if (ptr != null)
						{
							Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)ptr + 12)))((nint)ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040GDEEFGDC_0040BinaryMove_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05LAPONLG_0040false_003F_0024AA_0040)), (bool*)ptr2 + 16, tracker);
							Marshaler_002EMarshal(ptr, (Quaternion*)ptr2, tracker);
						}
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
					num2 += 20;
					num3--;
				}
				while (num3 != 0);
			}
			((int*)((byte*)tracker + 4))[1] += -4;
		}
		((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)list + 12)))((nint)list);
	}

	internal unsafe static void Marshaler_002EEndianCorrect_003Cfloat_003E(List_003Cfloat_003E* list, EndianHelper* endian)
	{
		int num = ((int*)list)[1];
		float* ptr = (float*)num;
		float* ptr2 = (float*)(*(int*)list * 4 + num);
		if (ptr != ptr2)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			do
			{
				if (*(bool*)endian)
				{
					*(sbyte*)(&num2) = ((sbyte*)ptr)[3];
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((sbyte*)ptr)[2];
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((sbyte*)ptr)[1];
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *(sbyte*)ptr;
					*(uint*)ptr = num2;
				}
				ptr++;
			}
			while (ptr != ptr2);
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(sbyte*)(&num3) = ((sbyte*)list)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)list)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((sbyte*)list)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)list;
			*(uint*)list = num3;
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector3_003E(IList* list, List_003CSageMathOnly_003A_003AMath_003A_003AVector3_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 12u, num);
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
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num4), (Vector3*)(num2 + ((int*)target)[1]), tracker);
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
					num2 += 12;
					num3--;
				}
				while (num3 != 0);
			}
			((int*)((byte*)tracker + 4))[1] += -4;
		}
		((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)list + 12)))((nint)list);
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020RGBAColor_003E(IList* list, List_003CRGBAColor_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 16u, num);
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
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num4), (RGBAColor*)(num2 + ((int*)target)[1]), tracker);
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
					num2 += 16;
					num3--;
				}
				while (num3 != 0);
			}
			((int*)((byte*)tracker + 4))[1] += -4;
		}
		((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)list + 12)))((nint)list);
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector2_003E(IList* list, List_003CSageMathOnly_003A_003AMath_003A_003AVector2_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 8u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num3), (Vector2*)(int)(num2 * 8 + (uint)((int*)target)[1]), tracker);
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ABoneInfluence_003E(IList* list, List_003CSageBinaryData_003A_003ABoneInfluence_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 8u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num3), (BoneInfluence*)(int)(num2 * 8 + (uint)((int*)target)[1]), tracker);
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ABinormals_003E(INode* node, W3DMeshPipelineVertexData.Binormals** pointer, Tracker* tracker)
	{
		if (node != null)
		{
			Relo_002ETracker_002EPush(tracker, (void**)pointer, 8u, 1u);
			W3DMeshPipelineVertexData.Binormals* target = (W3DMeshPipelineVertexData.Binormals*)(int)(*(uint*)pointer);
			Marshaler_002EMarshal_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector3_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01HMGJMAIH_0040B_003F_0024AA_0040)), (List_003CSageMathOnly_003A_003AMath_003A_003AVector3_003E*)target, tracker);
			((int*)((byte*)tracker + 4))[1] += -4;
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003AVertexColors_003E(INode* node, W3DMeshPipelineVertexData.VertexColors** pointer, Tracker* tracker)
	{
		if (node != null)
		{
			Relo_002ETracker_002EPush(tracker, (void**)pointer, 8u, 1u);
			W3DMeshPipelineVertexData.VertexColors* target = (W3DMeshPipelineVertexData.VertexColors*)(int)(*(uint*)pointer);
			Marshaler_002EMarshal_003Cstruct_0020RGBAColor_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GFHCPBMG_0040C_003F_0024AA_0040)), (List_003CRGBAColor_003E*)target, tracker);
			((int*)((byte*)tracker + 4))[1] += -4;
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ATexCoords_003E(IList* list, List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ATexCoords_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 8u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						INode* ptr = (INode*)(int)(*(uint*)num3);
						W3DMeshPipelineVertexData.TexCoords* target2 = (W3DMeshPipelineVertexData.TexCoords*)(int)(num2 * 8 + (uint)((int*)target)[1]);
						if (ptr != null)
						{
							Marshaler_002EMarshal_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector2_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01GAPBHFFA_0040T_003F_0024AA_0040)), (List_003CSageMathOnly_003A_003AMath_003A_003AVector2_003E*)target2, tracker);
						}
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AAssetReference_003Cstruct_0020SageBinaryData_003A_003AW3DHierarchy_002Cclass_0020Sage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, AssetReference_003CSageBinaryData_003A_003AW3DHierarchy_002CSage_003A_003ARender_003A_003AW3DHierarchyWrapper_003E* @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* ptr = strchr(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value), 92);
			if (ptr != null)
			{
				uint value2 = (uint)atoi(ptr + 1);
				Relo_002ETracker_002EAddReference(state, @object, value2);
			}
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelScalar_002Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_003E(INode* node, AnimationChannelBase** pointer, Tracker* tracker)
	{
		AnimationChannelScalar* ptr = (AnimationChannelScalar*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 24u, 1u);
		if (node != null)
		{
			Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelScalar_003A_003AFrame_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05MPDBFDHF_0040Frame_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003AAnimationChannelScalar_003A_003AFrame_003E*)((byte*)ptr + 16), tracker);
			Marshaler_002EMarshal(node, (AnimationChannelBase*)ptr, tracker);
		}
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = 1355976437;
	}

	internal unsafe static void Marshaler_002EMarshalPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelQuaternion_002Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_003E(INode* node, AnimationChannelBase** pointer, Tracker* tracker)
	{
		AnimationChannelQuaternion* ptr = (AnimationChannelQuaternion*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 24u, 1u);
		if (node != null)
		{
			Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelQuaternion_003A_003AFrame_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)node + 4)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05MPDBFDHF_0040Frame_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003AAnimationChannelQuaternion_003A_003AFrame_003E*)((byte*)ptr + 16), tracker);
			Marshaler_002EMarshal(node, (AnimationChannelBase*)ptr, tracker);
		}
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = -163390176;
	}

	internal unsafe static void Marshaler_002EMarshalUnknownPolymorphicType_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_003E(INode* node, AnimationChannelBase** pointer, Tracker* tracker)
	{
		AnimationChannelBase* ptr = (AnimationChannelBase*)Relo_002ETracker_002EPush(tracker, (void**)pointer, 16u, 1u);
		Marshaler_002EMarshal(node, ptr, tracker);
		((int*)((byte*)tracker + 4))[1] += -4;
		*(int*)ptr = 0;
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ATriangle_003E(IList* list, List_003CSageBinaryData_003A_003ATriangle_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 24u, num);
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
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num4), (Triangle*)(num2 + ((int*)target)[1]), tracker);
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
					num2 += 24;
					num3--;
				}
				while (num3 != 0);
			}
			((int*)((byte*)tracker + 4))[1] += -4;
		}
		((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)list + 12)))((nint)list);
	}

	internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AMeshGeometryType_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, MeshGeometryType* @object, Tracker* state)
	{
		if (value != null)
		{
			EnumNameGetter_003Cint_003E.EnumPair* ptr = EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value), (EnumNameGetter_003Cint_003E.EnumPair*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x325460ff_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4MeshGeometryType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4MeshGeometryType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B), 4u);
			int num = ((ptr == null) ? 2 : ((int*)ptr)[1]);
			*@object = (MeshGeometryType)num;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect_003Cstruct_0020SageBinaryData_003A_003ATriangle_003E(List_003CSageBinaryData_003A_003ATriangle_003E* list, EndianHelper* endian)
	{
		int num = ((int*)list)[1];
		Triangle* ptr = (Triangle*)num;
		Triangle* ptr2 = (Triangle*)(*(int*)list * 24 + num);
		if (ptr != ptr2)
		{
			Triangle* ptr3 = (Triangle*)((byte*)ptr + 22);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			do
			{
				Marshaler_002EEndianCorrect_003Cunsigned_0020int_003E((List_003Cunsigned_0020int_003E*)ptr, endian);
				Marshaler_002EEndianCorrect((Vector3*)((byte*)ptr3 - 14), endian);
				if (*(bool*)endian)
				{
					*(sbyte*)(&num2) = ((sbyte*)ptr3)[1];
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = *(sbyte*)ptr3;
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = *((sbyte*)ptr3 - 1);
					System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = *((sbyte*)ptr3 - 2);
					*(uint*)((byte*)ptr3 - 2) = num2;
				}
				ptr = (Triangle*)((byte*)ptr + 24);
				ptr3 = (Triangle*)((byte*)ptr3 + 24);
			}
			while (ptr != ptr2);
		}
		if (*(bool*)endian)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(sbyte*)(&num3) = ((sbyte*)list)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)list)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((sbyte*)list)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)list;
			*(uint*)list = num3;
		}
	}

	internal unsafe static void eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002Eresize(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0, uint n, SrcFrame* value)
	{
		int num = ((int*)P_0)[1];
		int num2 = *(int*)P_0;
		int num3 = (num - num2) / 24;
		if (n > (uint)num3)
		{
			eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoInsertValues(P_0, (SrcFrame*)num, n - (uint)num3, value);
			return;
		}
		SrcFrame* ptr = (SrcFrame*)num;
		SrcFrame* ptr2 = (SrcFrame*)(int)(n * 24 + (uint)num2);
		SrcFrame* last = (SrcFrame*)num;
		SrcFrame* first = eastl_002Ecopy_chooser_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(ptr, last, ptr2);
		eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoDestroyValues(P_0, first, (SrcFrame*)(int)((uint*)P_0)[1]);
		((int*)P_0)[1] = (int)(((int*)P_0)[1] + (nint)((byte*)ptr - (nuint)ptr2) / (nint)(-24) * 24);
	}

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DAnimation_003E(Tracker* P_0, W3DAnimation** rootPointer)
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DAnimationRuntime_003E(Tracker* P_0, W3DAnimationRuntime** rootPointer)
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
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 24u);
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DMesh_003E(Tracker* P_0, W3DMesh** rootPointer)
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
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 96u);
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003E(Tracker* P_0, W3DMeshPipelineVertexData** rootPointer)
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
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 48u);
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

	internal unsafe static void Marshaler_002EMarshal_003Cunsigned_0020int_002Cclass_0020Relo_003A_003ATracker_003E(INode* node, uint* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, IValue*>)(int)(*(uint*)(*(int*)node + 16)))((nint)node);
			if (ptr != null)
			{
				sbyte* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr);
				*@object = strtoul(ptr2, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AFXShaderConstant_0020_002A_003E(IList* list, List_003CSageBinaryData_003A_003AFXShaderConstant_0020_002A_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 4u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num3), (FXShaderConstant**)(int)(num2 * 4 + (uint)((int*)target)[1]), tracker);
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

	internal unsafe static void Marshaler_002EMarshal_003Cfloat_003E(IList* list, List_003Cfloat_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 4u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						INode* ptr = (INode*)(int)(*(uint*)num3);
						float* ptr2 = (float*)(int)(num2 * 4 + (uint)((int*)target)[1]);
						if (ptr != null)
						{
							IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, IValue*>)(int)(*(uint*)(*(int*)ptr + 16)))((nint)ptr);
							if (ptr3 != null)
							{
								sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
								*ptr2 = (float)atof(ptr4);
								((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
							}
						}
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003AVertices_003E(IList* list, List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003AVertices_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 8u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						INode* ptr = (INode*)(int)(*(uint*)num3);
						W3DMeshPipelineVertexData.Vertices* target2 = (W3DMeshPipelineVertexData.Vertices*)(int)(num2 * 8 + (uint)((int*)target)[1]);
						if (ptr != null)
						{
							Marshaler_002EMarshal_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector3_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01FCMHBHNC_0040V_003F_0024AA_0040)), (List_003CSageMathOnly_003A_003AMath_003A_003AVector3_003E*)target2, tracker);
						}
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ANormals_003E(IList* list, List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ANormals_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 8u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						INode* ptr = (INode*)(int)(*(uint*)num3);
						W3DMeshPipelineVertexData.Normals* target2 = (W3DMeshPipelineVertexData.Normals*)(int)(num2 * 8 + (uint)((int*)target)[1]);
						if (ptr != null)
						{
							Marshaler_002EMarshal_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector3_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01NANMIPIL_0040N_003F_0024AA_0040)), (List_003CSageMathOnly_003A_003AMath_003A_003AVector3_003E*)target2, tracker);
						}
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ABoneInfluences_003E(IList* list, List_003CSageBinaryData_003A_003AW3DMeshPipelineVertexData_003A_003ABoneInfluences_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 8u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						INode* ptr = (INode*)(int)(*(uint*)num3);
						W3DMeshPipelineVertexData.BoneInfluences* target2 = (W3DMeshPipelineVertexData.BoneInfluences*)(int)(num2 * 8 + (uint)((int*)target)[1]);
						if (ptr != null)
						{
							Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003ABoneInfluence_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, IList*>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_01JPJNBJEM_0040I_003F_0024AA_0040)), (List_003CSageBinaryData_003A_003ABoneInfluence_003E*)target2, tracker);
						}
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003AAnimationChannelBase_0020_002A_003E(IList* list, List_003CSageBinaryData_003A_003AAnimationChannelBase_0020_002A_003E* target, Tracker* tracker)
	{
		if (list == null)
		{
			return;
		}
		uint num = (uint)(*(int*)target = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, int>)(int)(*(uint*)(*(int*)list + 8)))((nint)list));
		if (num != 0)
		{
			Relo_002ETracker_002EPush(tracker, (void**)((byte*)target + 4), 4u, num);
			uint num2 = 0u;
			if (0 < num)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out NodeHolder nodeHolder);
				do
				{
					int num3 = (int)((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, NodeHolder*, NodeHolder*>)(int)(*(uint*)(*(int*)list + 4)))((nint)list, &nodeHolder);
					try
					{
						Marshaler_002EMarshal((INode*)(int)(*(uint*)num3), (AnimationChannelBase**)(int)(num2 * 4 + (uint)((int*)target)[1]), tracker);
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

	internal unsafe static void eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoInsertValues(vector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E* P_0, SrcFrame* position, uint n, SrcFrame* value)
	{
		int num = ((int*)P_0)[1];
		if (n <= (uint)((((int*)P_0)[2] - num) / 24))
		{
			if (n == 0)
			{
				return;
			}
			System.Runtime.CompilerServices.Unsafe.SkipInit(out SrcFrame srcFrame);
			*(int*)(&srcFrame) = *(int*)value;
			eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bctor_007D((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)), (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)value + 4));
			try
			{
				System.Runtime.CompilerServices.Unsafe.As<SrcFrame, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 20)) = ((sbyte*)value)[20];
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)));
				throw;
			}
			try
			{
				uint num2 = (uint)(((nint)((int*)P_0)[1] - (nint)position) / 24);
				num = ((int*)P_0)[1];
				SrcFrame* ptr = (SrcFrame*)num;
				if (n < num2)
				{
					int num3 = (int)(n * 24);
					int num4 = num - num3;
					int num5 = num;
					eastl_002Euninitialized_copy_ptr_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E((SrcFrame*)num4, (SrcFrame*)num5, (SrcFrame*)num5);
					((int*)P_0)[1] += num3;
					eastl_002Ecopy_backward_chooser_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(position, (SrcFrame*)num4, ptr);
					eastl_002Efill_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_003E(position, (SrcFrame*)(num3 + (byte*)position), &srcFrame);
				}
				else
				{
					System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E first);
					*(int*)(&first) = num;
					uint num6 = n - num2;
					System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed);
					eastl_002Euninitialized_fill_n_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_002Cunsigned_0020int_002Cstruct_0020SrcFrame_003E(first, num6, &srcFrame, _unnamed);
					((int*)P_0)[1] += (int)(num6 * 24);
					int num7 = ((int*)P_0)[1];
					eastl_002Euninitialized_copy_ptr_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(position, ptr, (SrcFrame*)num7);
					((int*)P_0)[1] += (int)(num2 * 24);
					eastl_002Efill_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_003E(position, ptr, &srcFrame);
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<SrcFrame*, void>)(&SrcFrame_002E_007Bdtor_007D), &srcFrame);
				throw;
			}
			vector_003Cfloat_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4));
			if (System.Runtime.CompilerServices.Unsafe.As<SrcFrame, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)) != 0)
			{
				delete_005B_005D((void*)(int)System.Runtime.CompilerServices.Unsafe.As<SrcFrame, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref srcFrame, 4)));
			}
		}
		else
		{
			uint num8 = (uint)((num - *(int*)P_0) / 24);
			uint num9 = ((num8 == 0) ? 1u : (num8 << 1));
			uint num10 = n + num8;
			uint num11 = ((num9 <= num10) ? num10 : num9);
			SrcFrame* ptr3 = (SrcFrame*)((num11 == 0) ? null : new_005B_005D(num11 * 24, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209));
			SrcFrame* ptr4 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E((SrcFrame*)(int)(*(uint*)P_0), position, ptr3);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E first2);
			*(int*)(&first2) = (int)ptr4;
			System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed2);
			eastl_002Euninitialized_fill_n_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_002Cunsigned_0020int_002Cstruct_0020SrcFrame_003E(first2, n, value, _unnamed2);
			ptr4 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(position, (SrcFrame*)(int)((uint*)P_0)[1], (SrcFrame*)((int)(n * 24) + (byte*)ptr4));
			eastl_002Evector_003CSrcFrame_002Ceastl_003A_003Aallocator_003E_002EDoDestroyValues(P_0, (SrcFrame*)(int)(*(uint*)P_0), (SrcFrame*)(int)((uint*)P_0)[1]);
			SrcFrame* ptr5 = (SrcFrame*)(int)(*(uint*)P_0);
			if (ptr5 != null)
			{
				delete_005B_005D(ptr5);
			}
			*(int*)P_0 = (int)ptr3;
			((int*)P_0)[1] = (int)ptr4;
			((int*)P_0)[2] = (int)((int)(num11 * 24) + (byte*)ptr3);
		}
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

	internal unsafe static void eastl_002Efill_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_003E(SrcFrame* first, SrcFrame* last, SrcFrame* value)
	{
		SrcFrame* ptr = first;
		if (first != last)
		{
			SrcFrame* x = (SrcFrame*)((byte*)value + 4);
			SrcFrame* ptr2 = (SrcFrame*)((byte*)value + 20);
			do
			{
				*(int*)ptr = *(int*)value;
				SrcFrame* ptr3 = (SrcFrame*)((byte*)ptr + 20);
				eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_003D((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr3 - 16), (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)x);
				*(sbyte*)ptr3 = *(sbyte*)ptr2;
				ptr = (SrcFrame*)((byte*)ptr + 24);
			}
			while (ptr != last);
		}
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

	internal unsafe static void eastl_002Euninitialized_fill_n_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020SrcFrame_0020_002A_002Cvoid_003E_002Cunsigned_0020int_002Cstruct_0020SrcFrame_003E(generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E first, uint n, SrcFrame* value, integral_constant_003Cbool_002C0_003E __unnamed003)
	{
		generic_iterator_003CSrcFrame_0020_002A_002Cvoid_003E obj = first;
		if (n == 0)
		{
			return;
		}
		do
		{
			void* ptr = (void*)(int)(*(uint*)(&obj));
			SrcFrame* ptr2 = (SrcFrame*)(int)(*(uint*)(&obj));
			try
			{
				if (*(int*)(&obj) != 0)
				{
					*(int*)(int)(*(uint*)(&obj)) = *(int*)value;
					eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bctor_007D((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)(*(int*)(&obj) + 4), (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)value + 4));
					try
					{
						*(sbyte*)(*(int*)(&obj) + 20) = ((sbyte*)value)[20];
					}
					catch
					{
						//try-fault
						___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), (byte*)ptr2 + 4);
						throw;
					}
				}
			}
			catch
			{
				//try-fault
				delete(ptr2, ptr);
				throw;
			}
			n--;
			*(int*)(&obj) += 24;
		}
		while (n != 0);
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

	internal unsafe static SrcFrame* eastl_002Ecopy_backward_chooser_003Cstruct_0020SrcFrame_0020_002A_002Cstruct_0020SrcFrame_0020_002A_003E(SrcFrame* first, SrcFrame* last, SrcFrame* result)
	{
		SrcFrame* ptr = result;
		SrcFrame* ptr2 = last;
		if (last != first)
		{
			int num = (int)((byte*)last - (nuint)result);
			do
			{
				ptr2 = (SrcFrame*)((byte*)ptr2 - 24);
				ptr = (SrcFrame*)((byte*)ptr - 24);
				*(int*)ptr = *(int*)ptr2;
				eastl_002Evector_003Cfloat_002Ceastl_003A_003Aallocator_003E_002E_003D((vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr + 4), (vector_003Cfloat_002Ceastl_003A_003Aallocator_003E*)((byte*)ptr + num + 4));
				((sbyte*)ptr)[20] = ((sbyte*)ptr2)[20];
			}
			while (ptr2 != first);
		}
		return ptr;
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

	internal unsafe static void W3DMeshProcessor_002ECopyTexCoord(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (numElements == 0)
		{
			return;
		}
		int num = ((int*)converterData)[5];
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num5);
		do
		{
			float num2 = *(float*)srcData;
			if (*(bool*)num)
			{
				*(sbyte*)(&num3) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)(&num2);
				*(uint*)(&num2) = num3;
			}
			*(float*)destData = num2;
			float num4 = (float)(1.0 - (double)((float*)srcData)[1]);
			if (*(bool*)num)
			{
				*(sbyte*)(&num5) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 1)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 2)) = System.Runtime.CompilerServices.Unsafe.As<float, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 3)) = *(sbyte*)(&num4);
				*(uint*)(&num4) = num5;
			}
			((float*)destData)[1] = num4;
			srcData = srcPitch + srcData;
			destData = destPitch + destData;
			numElements--;
		}
		while (numElements != 0);
	}

	internal unsafe static void W3DMeshProcessor_002ECompressTexCoord(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (numElements == 0)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num14);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num16);
		do
		{
			float num = *(float*)srcData;
			float num2 = (float)(1.0 - (double)((float*)srcData)[1]);
			float num3 = num;
			uint num4 = (uint)(*(int*)(&num3) & 0x7FFFFFFF);
			uint num5 = (uint)((*(int*)(&num3) >>> 16) & 0x8000);
			ushort num6;
			if (num4 > 1207955455)
			{
				num6 = (ushort)((ushort)num5 | 0x7FFF);
			}
			else
			{
				num4 = ((num4 >= 947912704) ? (num4 - 939524096) : (((num4 & 0x7FFFFF) | 0x800000) >> (int)(113 - (num4 >> 23))));
				uint num7 = num4;
				num6 = (ushort)((num7 + ((num7 >> 13) & 1) + 4095 >> 13) | num5);
			}
			float num8 = num2;
			uint num9 = (uint)(*(int*)(&num8) & 0x7FFFFFFF);
			uint num10 = (uint)((*(int*)(&num8) >>> 16) & 0x8000);
			ushort num11;
			if (num9 > 1207955455)
			{
				num11 = (ushort)((ushort)num10 | 0x7FFF);
			}
			else
			{
				num9 = ((num9 >= 947912704) ? (num9 - 939524096) : (((num9 & 0x7FFFFF) | 0x800000) >> (int)(113 - (num9 >> 23))));
				uint num12 = num9;
				num11 = (ushort)((num12 + ((num12 >> 13) & 1) + 4095 >> 13) | num10);
			}
			ushort num13 = num6;
			if (*(bool*)(int)((uint*)converterData)[5])
			{
				*(sbyte*)(&num14) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 1));
				System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 1)) = *(sbyte*)(&num6);
				num13 = num14;
			}
			*(ushort*)destData = num13;
			ushort num15 = num11;
			if (*(bool*)(int)((uint*)converterData)[5])
			{
				*(sbyte*)(&num16) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 1));
				System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 1)) = *(sbyte*)(&num11);
				num15 = num16;
			}
			((short*)destData)[1] = (short)num15;
			srcData = srcPitch + srcData;
			destData = destPitch + destData;
			numElements--;
		}
		while (numElements != 0);
	}

	internal unsafe static void W3DMeshProcessor_002EVector4_To_UByte4N(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (numElements == 0)
		{
			return;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out __vector4 _vector);
		System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 12)) = 0.5f;
		System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 8)) = 0.5f;
		System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 4)) = 0.5f;
		*(float*)(&_vector) = 0.5f;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out __vector4 _vector2);
		System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector2, 12)) = -0.5f;
		System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector2, 8)) = -0.5f;
		System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector2, 4)) = -0.5f;
		*(float*)(&_vector2) = -0.5f;
		ConverterData* ptr = (ConverterData*)((byte*)converterData + 20);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out __vector4 _vector3);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out __vector4 _vector4);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out __vector4 _vector5);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out __vector4 _vector6);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _XMUBYTEN4 xMUBYTEN);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num18);
		do
		{
			float num = *(float*)srcData;
			float num2 = ((float*)srcData)[1];
			float num3 = ((float*)srcData)[2];
			float num4 = ((float*)srcData)[3];
			float num5 = ((!(0.0 > (double)num)) ? num : 0f);
			float num6 = ((!(0.0 > (double)num2)) ? num2 : 0f);
			float num7 = ((!(0.0 > (double)num3)) ? num3 : 0f);
			float num8 = ((!(0.0 > (double)num4)) ? num4 : 0f);
			float num9 = ((!(1.0 < (double)num5)) ? num5 : 1f);
			float num10 = ((!(1.0 < (double)num6)) ? num6 : 1f);
			float num11 = ((!(1.0 < (double)num7)) ? num7 : 1f);
			float num12 = ((!(1.0 < (double)num8)) ? num8 : 1f);
			*(float*)(&_vector3) = (float)((double)num9 * 255.0);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 4)) = (float)((double)num10 * 255.0);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 8)) = (float)((double)num11 * 255.0);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 12)) = (float)((double)num12 * 255.0);
			uint num13 = ((*(float*)(&_vector3) < 0f) ? uint.MaxValue : 0u);
			uint num14 = ((System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 4)) < 0f) ? uint.MaxValue : 0u);
			uint num15 = ((System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 8)) < 0f) ? uint.MaxValue : 0u);
			uint num16 = ((System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 12)) < 0f) ? uint.MaxValue : 0u);
			*(uint*)(&_vector4) = (~num13 & (uint)(*(int*)(&_vector))) | ((uint)(*(int*)(&_vector2)) & num13);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector4, 4)) = (~num14 & (uint)System.Runtime.CompilerServices.Unsafe.As<__vector4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 4))) | ((uint)System.Runtime.CompilerServices.Unsafe.As<__vector4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector2, 4)) & num14);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector4, 8)) = (~num15 & (uint)System.Runtime.CompilerServices.Unsafe.As<__vector4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 8))) | ((uint)System.Runtime.CompilerServices.Unsafe.As<__vector4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector2, 8)) & num15);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector4, 12)) = (~num16 & (uint)System.Runtime.CompilerServices.Unsafe.As<__vector4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 12))) | ((uint)System.Runtime.CompilerServices.Unsafe.As<__vector4, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector2, 12)) & num16);
			*(float*)(&_vector5) = (float)((double)(*(float*)(&_vector4)) + (double)(*(float*)(&_vector3)));
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector5, 4)) = (float)((double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector4, 4)) + (double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 4)));
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector5, 8)) = (float)((double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector4, 8)) + (double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 8)));
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector5, 12)) = (float)((double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector4, 12)) + (double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector3, 12)));
			*(float*)(&_vector6) = (int)(double)(*(float*)(&_vector5));
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector6, 4)) = (int)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector5, 4));
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector6, 8)) = (int)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector5, 8));
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector6, 12)) = (int)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector5, 12));
			*(sbyte*)(&xMUBYTEN) = (sbyte)((uint)(double)(*(float*)(&_vector6)) & 0xFF);
			System.Runtime.CompilerServices.Unsafe.As<_XMUBYTEN4, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xMUBYTEN, 1)) = (sbyte)((uint)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector6, 4)) & 0xFF);
			System.Runtime.CompilerServices.Unsafe.As<_XMUBYTEN4, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xMUBYTEN, 2)) = (sbyte)((uint)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector6, 8)) & 0xFF);
			System.Runtime.CompilerServices.Unsafe.As<_XMUBYTEN4, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xMUBYTEN, 3)) = (sbyte)((uint)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector6, 12)) & 0xFF);
			uint num17 = *(uint*)(&xMUBYTEN);
			if (*(bool*)(int)(*(uint*)ptr))
			{
				*(sbyte*)(&num18) = System.Runtime.CompilerServices.Unsafe.As<_XMUBYTEN4, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xMUBYTEN, 3));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num18, 1)) = System.Runtime.CompilerServices.Unsafe.As<_XMUBYTEN4, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xMUBYTEN, 2));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num18, 2)) = System.Runtime.CompilerServices.Unsafe.As<_XMUBYTEN4, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref xMUBYTEN, 1));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num18, 3)) = *(sbyte*)(&xMUBYTEN);
				num17 = num18;
			}
			*destData = *(sbyte*)(&num17);
			destData[1] = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num17, 1));
			destData[2] = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num17, 2));
			destData[3] = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num17, 3));
			srcData = srcPitch + srcData;
			destData = destPitch + destData;
			numElements--;
		}
		while (numElements != 0);
	}

	internal unsafe static void W3DMeshProcessor_002EVector3_To_Half4(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (numElements == 0)
		{
			return;
		}
		float num = 0f;
		uint num2 = (uint)(*(int*)(&num) & 0x7FFFFFFF);
		uint num3 = (uint)((*(int*)(&num) >>> 16) & 0x8000);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num26);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num28);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num30);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num32);
		do
		{
			float num4 = *(float*)srcData;
			float num5 = ((float*)srcData)[1];
			float num6 = ((float*)srcData)[2];
			float num7 = num4;
			uint num8 = (uint)(*(int*)(&num7) & 0x7FFFFFFF);
			uint num9 = (uint)((*(int*)(&num7) >>> 16) & 0x8000);
			ushort num10;
			if (num8 > 1207955455)
			{
				num10 = (ushort)((ushort)num9 | 0x7FFF);
			}
			else
			{
				num8 = ((num8 >= 947912704) ? (num8 - 939524096) : (((num8 & 0x7FFFFF) | 0x800000) >> (int)(113 - (num8 >> 23))));
				uint num11 = num8;
				num10 = (ushort)((num11 + ((num11 >> 13) & 1) + 4095 >> 13) | num9);
			}
			float num12 = num5;
			uint num13 = (uint)(*(int*)(&num12) & 0x7FFFFFFF);
			uint num14 = (uint)((*(int*)(&num12) >>> 16) & 0x8000);
			ushort num15;
			if (num13 > 1207955455)
			{
				num15 = (ushort)((ushort)num14 | 0x7FFF);
			}
			else
			{
				num13 = ((num13 >= 947912704) ? (num13 - 939524096) : (((num13 & 0x7FFFFF) | 0x800000) >> (int)(113 - (num13 >> 23))));
				uint num16 = num13;
				num15 = (ushort)((num16 + ((num16 >> 13) & 1) + 4095 >> 13) | num14);
			}
			float num17 = num6;
			uint num18 = (uint)(*(int*)(&num17) & 0x7FFFFFFF);
			uint num19 = (uint)((*(int*)(&num17) >>> 16) & 0x8000);
			ushort num20;
			if (num18 > 1207955455)
			{
				num20 = (ushort)((ushort)num19 | 0x7FFF);
			}
			else
			{
				num18 = ((num18 >= 947912704) ? (num18 - 939524096) : (((num18 & 0x7FFFFF) | 0x800000) >> (int)(113 - (num18 >> 23))));
				uint num21 = num18;
				num20 = (ushort)((num21 + ((num21 >> 13) & 1) + 4095 >> 13) | num19);
			}
			uint num22 = num2;
			ushort num23;
			if (num22 > 1207955455)
			{
				num23 = (ushort)((ushort)num3 | 0x7FFF);
			}
			else
			{
				num22 = ((num22 >= 947912704) ? (num22 - 939524096) : (((num22 & 0x7FFFFF) | 0x800000) >> (int)(113 - (num22 >> 23))));
				uint num24 = num22;
				num23 = (ushort)((num24 + ((num24 >> 13) & 1) + 4095 >> 13) | num3);
			}
			ushort num25 = num10;
			if (*(bool*)(int)((uint*)converterData)[5])
			{
				*(sbyte*)(&num26) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 1));
				System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num26, 1)) = *(sbyte*)(&num10);
				num25 = num26;
			}
			*(ushort*)destData = num25;
			ushort num27 = num15;
			if (*(bool*)(int)((uint*)converterData)[5])
			{
				*(sbyte*)(&num28) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num15, 1));
				System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num28, 1)) = *(sbyte*)(&num15);
				num27 = num28;
			}
			((short*)destData)[1] = (short)num27;
			ushort num29 = num20;
			if (*(bool*)(int)((uint*)converterData)[5])
			{
				*(sbyte*)(&num30) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num20, 1));
				System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num30, 1)) = *(sbyte*)(&num20);
				num29 = num30;
			}
			((short*)destData)[2] = (short)num29;
			ushort num31 = num23;
			if (*(bool*)(int)((uint*)converterData)[5])
			{
				*(sbyte*)(&num32) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num23, 1));
				System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num32, 1)) = *(sbyte*)(&num23);
				num31 = num32;
			}
			((short*)destData)[3] = (short)num31;
			srcData = srcPitch + srcData;
			destData = destPitch + destData;
			numElements--;
		}
		while (numElements != 0);
	}

	internal unsafe static void W3DMeshProcessor_002EConvertBlendIndex(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (numElements == 0)
		{
			return;
		}
		ConverterData* ptr = (ConverterData*)((byte*)converterData + 4);
		while (true)
		{
			ushort num = *(ushort*)srcData;
			int num2 = *(int*)converterData;
			if ((uint)num >= (uint)(*(int*)ptr - num2 >> 1))
			{
				break;
			}
			*destData = (sbyte)(*(ushort*)(num * 2 + num2));
			srcData = srcPitch + srcData;
			destData = destPitch + destData;
			numElements--;
			if (numElements == 0)
			{
				return;
			}
		}
		sbyte* ptr2 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0EJ_0040GDEJDBKH_0040Assert_003F5failed_003F3_003F5srcIndex_003F5_003F_0024DM_003F5conver_0040);
		_CxxThrowException(&ptr2, (_s__ThrowInfo*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _TI2PAD));
	}

	internal unsafe static void W3DMeshProcessor_002EConvertBlendWeight(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (numElements != 0)
		{
			do
			{
				float num = *(float*)srcData;
				*destData = (sbyte)((uint)((double)num * 255.0 + 0.5) & 0xFF);
				srcData = srcPitch + srcData;
				destData = destPitch + destData;
				numElements--;
			}
			while (numElements != 0);
		}
	}

	internal unsafe static void W3DMeshProcessor_002ECompressNormalizedDirection(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (numElements == 0)
		{
			return;
		}
		ConverterData* ptr = (ConverterData*)((byte*)converterData + 20);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out __vector4 _vector);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num11);
		do
		{
			float num = *(float*)srcData;
			float num2 = ((float*)srcData)[1];
			float num3 = ((float*)srcData)[2];
			float num4 = ((!(-1.0 > (double)num)) ? num : (-1f));
			float num5 = ((!(-1.0 > (double)num2)) ? num2 : (-1f));
			float num6 = ((!(-1.0 > (double)num3)) ? num3 : (-1f));
			float num7 = ((!(1.0 < (double)num4)) ? num4 : 1f);
			float num8 = ((!(1.0 < (double)num5)) ? num5 : 1f);
			float num9 = ((!(1.0 < (double)num6)) ? num6 : 1f);
			*(float*)(&_vector) = (float)((double)num7 * 1023.0);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 4)) = (float)((double)num8 * 1023.0);
			System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 8)) = (float)((double)num9 * 511.0);
			uint num10 = ((((uint)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 4)) & 0x7FF) | ((uint)(double)System.Runtime.CompilerServices.Unsafe.As<__vector4, float>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _vector, 8)) << 11)) << 11) | ((uint)(double)(*(float*)(&_vector)) & 0x7FF);
			if (*(bool*)(int)(*(uint*)ptr))
			{
				*(sbyte*)(&num11) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 3));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 2));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num10, 1));
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num11, 3)) = *(sbyte*)(&num10);
				num10 = num11;
			}
			*(uint*)destData = num10;
			srcData = srcPitch + srcData;
			destData = destPitch + destData;
			numElements--;
		}
		while (numElements != 0);
	}

	internal unsafe static void W3DMeshProcessor_002EDetermineVertexDataContents(W3DMeshPipelineVertexData* pipelineData, vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* mappings)
	{
		eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002Eclear(mappings);
		int num = 0;
		if (0 < *(int*)pipelineData)
		{
			W3DMeshPipelineVertexData* ptr = (W3DMeshPipelineVertexData*)((byte*)pipelineData + 4);
			vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* ptr2 = (vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E*)((byte*)mappings + 8);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping);
			do
			{
				*(int*)(&dataMapping) = 0;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping, 4)) = num;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping, 8)) = *(int*)(num * 8 + *(int*)ptr + 4);
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping, 12)) = 12;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping, 16)) = 0;
				uint num2 = ((uint*)mappings)[1];
				if (num2 < (uint)(*(int*)ptr2))
				{
					int num3 = (int)num2;
					((int*)mappings)[1] = (int)(num2 + 20);
					if (num3 != 0)
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(num3, ref dataMapping, 20);
					}
				}
				else
				{
					eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num2, &dataMapping);
				}
				num++;
			}
			while (num < *(int*)pipelineData);
		}
		int num4 = 0;
		if (0 < ((int*)pipelineData)[2])
		{
			W3DMeshPipelineVertexData* ptr3 = (W3DMeshPipelineVertexData*)((byte*)pipelineData + 12);
			vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* ptr4 = (vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E*)((byte*)mappings + 8);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping2);
			do
			{
				*(int*)(&dataMapping2) = 1;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping2, 4)) = num4;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping2, 8)) = *(int*)(num4 * 8 + *(int*)ptr3 + 4);
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping2, 12)) = 12;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping2, 16)) = 0;
				uint num5 = ((uint*)mappings)[1];
				if (num5 < (uint)(*(int*)ptr4))
				{
					int num6 = (int)num5;
					((int*)mappings)[1] = (int)(num5 + 20);
					if (num6 != 0)
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(num6, ref dataMapping2, 20);
					}
				}
				else
				{
					eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num5, &dataMapping2);
				}
				num4++;
			}
			while (num4 < ((int*)pipelineData)[2]);
		}
		uint num7 = ((uint*)pipelineData)[6];
		vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* ptr5;
		vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* ptr6;
		if (num7 != 0)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping3);
			*(int*)(&dataMapping3) = 5;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping3, 4)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping3, 8)) = *(int*)(int)(num7 + 4);
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping3, 12)) = 16;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping3, 16)) = 0;
			ptr5 = (vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E*)((byte*)mappings + 8);
			ptr6 = (vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E*)((byte*)mappings + 4);
			uint num8 = *(uint*)ptr6;
			if (num8 < (uint)(*(int*)ptr5))
			{
				int num9 = (int)num8;
				*(uint*)ptr6 = num8 + 20;
				if (num9 != 0)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(num9, ref dataMapping3, 20);
				}
			}
			else
			{
				eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num8, &dataMapping3);
			}
		}
		else
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping4);
			*(int*)(&dataMapping4) = 5;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping4, 4)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping4, 8)) = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x431bf441_002E_003FdefaultColor_0040_003FL_0040_003F_003FDetermineVertexDataContents_0040W3DMeshProcessor_0040_0040YAXABUW3DMeshPipelineVertexData_0040SageBinaryData_0040_0040AAV_003F_0024vector_0040UDataMapping_0040W3DMeshProcessor_0040_0040Vallocator_0040eastl_0040_0040_0040eastl_0040_0040_0040Z_00404UVector4_0040Math_0040SageMathOnly_0040_0040A);
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping4, 12)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping4, 16)) = 0;
			ptr5 = (vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E*)((byte*)mappings + 8);
			ptr6 = (vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E*)((byte*)mappings + 4);
			uint num8 = *(uint*)ptr6;
			if (num8 < (uint)(*(int*)ptr5))
			{
				int num10 = (int)num8;
				*(uint*)ptr6 = num8 + 20;
				if (num10 != 0)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(num10, ref dataMapping4, 20);
				}
			}
			else
			{
				eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num8, &dataMapping4);
			}
		}
		uint num11 = ((uint*)pipelineData)[4];
		if (num11 != 0 && ((int*)pipelineData)[5] != 0)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping5);
			*(int*)(&dataMapping5) = 2;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping5, 4)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping5, 8)) = *(int*)(int)(num11 + 4);
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping5, 12)) = 12;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping5, 16)) = 0;
			uint num12 = *(uint*)ptr6;
			if (num12 < (uint)(*(int*)ptr5))
			{
				int num13 = (int)num12;
				*(uint*)ptr6 = num12 + 20;
				if (num13 != 0)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(num13, ref dataMapping5, 20);
				}
			}
			else
			{
				eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num12, &dataMapping5);
			}
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping6);
			*(int*)(&dataMapping6) = 3;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping6, 4)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping6, 8)) = *(int*)(((int*)pipelineData)[5] + 4);
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping6, 12)) = 12;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping6, 16)) = 0;
			uint num8 = *(uint*)ptr6;
			if (num8 < (uint)(*(int*)ptr5))
			{
				int num14 = (int)num8;
				*(uint*)ptr6 = num8 + 20;
				if (num14 != 0)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(num14, ref dataMapping6, 20);
				}
			}
			else
			{
				eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num8, &dataMapping6);
			}
		}
		uint num15 = 0u;
		if (0u < (uint)((int*)pipelineData)[7])
		{
			W3DMeshPipelineVertexData* ptr7 = (W3DMeshPipelineVertexData*)((byte*)pipelineData + 32);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping7);
			do
			{
				*(int*)(&dataMapping7) = 4;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping7, 4)) = num15;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping7, 8)) = *(int*)((int)(num15 * 8) + *(int*)ptr7 + 4);
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping7, 12)) = 8;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping7, 16)) = 0;
				eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002Epush_back(mappings, &dataMapping7);
				num15++;
			}
			while (num15 < (uint)((int*)pipelineData)[7]);
		}
		uint* ptr8 = (uint*)pipelineData + 9;
		int num16 = *((*ptr8 >= (uint)(*(int*)pipelineData)) ? ((int*)pipelineData) : ((int*)ptr8));
		int num17 = 0;
		if (0 < num16)
		{
			W3DMeshPipelineVertexData* ptr9 = (W3DMeshPipelineVertexData*)((byte*)pipelineData + 40);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping8);
			do
			{
				*(int*)(&dataMapping8) = 7;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping8, 4)) = num17;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping8, 8)) = *(int*)(num17 * 8 + *(int*)ptr9 + 4) + 4;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping8, 12)) = 8;
				System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping8, 16)) = 0;
				uint num8 = *(uint*)ptr6;
				if (num8 < (uint)(*(int*)ptr5))
				{
					int num18 = (int)num8;
					*(uint*)ptr6 = num8 + 20;
					if (num18 != 0)
					{
						// IL cpblk instruction
						System.Runtime.CompilerServices.Unsafe.CopyBlock(num18, ref dataMapping8, 20);
					}
				}
				else
				{
					eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num8, &dataMapping8);
				}
				num17++;
			}
			while (num17 < num16);
		}
		if (num16 <= 1)
		{
			return;
		}
		int num19 = 0;
		if (0 >= num16)
		{
			return;
		}
		W3DMeshPipelineVertexData* ptr10 = (W3DMeshPipelineVertexData*)((byte*)pipelineData + 40);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out DataMapping dataMapping9);
		do
		{
			*(int*)(&dataMapping9) = 6;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping9, 4)) = num19;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping9, 8)) = *(int*)(num19 * 8 + *(int*)ptr10 + 4);
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping9, 12)) = 8;
			System.Runtime.CompilerServices.Unsafe.As<DataMapping, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref dataMapping9, 16)) = 0;
			uint num8 = *(uint*)ptr6;
			if (num8 < (uint)(*(int*)ptr5))
			{
				int num20 = (int)num8;
				*(uint*)ptr6 = num8 + 20;
				if (num20 != 0)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(num20, ref dataMapping9, 20);
				}
			}
			else
			{
				eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(mappings, (DataMapping*)(int)num8, &dataMapping9);
			}
			num19++;
		}
		while (num19 < num16);
	}

	internal unsafe static int W3DMeshProcessor_002EDetermineVertexDataLayout(vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* mappings, DataHandler* dataHandlers, Tracker* tracker, EndianHelper* endian, TargetPlatform platform)
	{
		int num = 0;
		DataMapping* ptr = (DataMapping*)(int)(*(uint*)mappings);
		if (ptr != (DataMapping*)((int*)mappings)[1])
		{
			int num3;
			do
			{
				((int*)ptr)[4] = num;
				int num2 = *(int*)ptr;
				if ((num2 == 7 || num2 == 6) && *(bool*)endian && platform != TargetPlatform.PlayStation3)
				{
					((int*)ptr)[4] = num - (((int*)ptr)[1] << 1) + 3;
				}
				num = ((int*)((byte*)dataHandlers + num2 * 16))[2] + num;
				num3 = ((int*)mappings)[1];
				DataMapping* ptr2 = (DataMapping*)((byte*)ptr + 20);
				if (ptr2 == (DataMapping*)num3 || *(int*)ptr2 != num2)
				{
					num = (num + 3) & -4;
				}
				ptr = ptr2;
			}
			while (ptr != (DataMapping*)num3);
		}
		return num;
	}

	internal unsafe static void W3DMeshProcessor_002EComputeBoneMappingTable(W3DMeshPipelineVertexData* pipelineData, vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E* boneMappingTable, vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E* inverseBoneMappingTable)
	{
		int num = ((int*)boneMappingTable)[1];
		ushort* ptr = (ushort*)num;
		ushort* ptr2 = (ushort*)(int)(*(uint*)boneMappingTable);
		ushort* ptr3 = (ushort*)num;
		memmove(ptr2, ptr, (uint)((byte*)ptr3 - (nuint)ptr));
		((int*)boneMappingTable)[1] = (int)(((int*)boneMappingTable)[1] + ((nint)((byte*)ptr - (nuint)ptr2) >> 1) * -2);
		int num2 = ((int*)inverseBoneMappingTable)[1];
		ushort* ptr4 = (ushort*)num2;
		ushort* ptr5 = (ushort*)(int)(*(uint*)inverseBoneMappingTable);
		ushort* ptr6 = (ushort*)num2;
		memmove(ptr5, ptr4, (uint)((byte*)ptr6 - (nuint)ptr4));
		((int*)inverseBoneMappingTable)[1] = (int)(((int*)inverseBoneMappingTable)[1] + ((nint)((byte*)ptr4 - (nuint)ptr5) >> 1) * -2);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E obj);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlock(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4), 0, 16);
		System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4)) = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4));
		System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 8)) = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4));
		System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 12)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 16)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 20)) = 0;
		try
		{
			int num3 = 0;
			if (0 < ((int*)pipelineData)[9])
			{
				W3DMeshPipelineVertexData* ptr7 = (W3DMeshPipelineVertexData*)((byte*)pipelineData + 40);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C1_003E _unnamed);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out pair_003Ceastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cbool_003E obj2);
				do
				{
					List_003CSageBinaryData_003A_003ABoneInfluence_003E* ptr8 = (List_003CSageBinaryData_003A_003ABoneInfluence_003E*)(num3 * 8 + *(int*)ptr7);
					int num4 = 0;
					if (0 < *(int*)ptr8)
					{
						List_003CSageBinaryData_003A_003ABoneInfluence_003E* ptr9 = (List_003CSageBinaryData_003A_003ABoneInfluence_003E*)((byte*)ptr8 + 4);
						do
						{
							ushort* value = (ushort*)(num4 * 8 + *(int*)ptr9 + 4);
							// IL initblk instruction
							System.Runtime.CompilerServices.Unsafe.InitBlock(ref _unnamed, 0, 1);
							eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoInsertValue((rbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E*)(&obj), &obj2, value, _unnamed);
							num4++;
						}
						while (num4 < *(int*)ptr8);
					}
					num3++;
				}
				while (num3 < ((int*)pipelineData)[9]);
			}
			if (System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 20)) != 0)
			{
				num = ((int*)boneMappingTable)[1];
				ushort* ptr10 = (ushort*)num;
				ushort* ptr11 = (ushort*)(int)(*(uint*)boneMappingTable);
				ushort* ptr12 = (ushort*)num;
				memmove(ptr11, ptr10, (uint)((byte*)ptr12 - (nuint)ptr10));
				((int*)boneMappingTable)[1] = (int)(((int*)boneMappingTable)[1] + ((nint)((byte*)ptr10 - (nuint)ptr11) >> 1) * -2);
				System.Runtime.CompilerServices.Unsafe.SkipInit(out rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj3);
				*(int*)(&obj3) = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4));
				System.Runtime.CompilerServices.Unsafe.SkipInit(out rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj4);
				*(int*)(&obj4) = System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 8));
				ushort* position = (ushort*)(int)(*(uint*)boneMappingTable);
				rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E last = obj3;
				rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E first = obj4;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out forward_iterator_tag _unnamed2);
				eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002EDoInsertFromIterator_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_0020_003E(boneMappingTable, position, first, last, _unnamed2);
				ushort num5 = 0;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj5);
				*(int*)(&obj5) = (int)eastl_002ERBTreeDecrement((rbtree_node_base*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4)));
				eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002Eresize(inverseBoneMappingTable, (uint)(*(ushort*)(*(int*)(&obj5) + 16) + 1), &num5);
				int num6 = 0;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj6);
				*(int*)(&obj6) = System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 8));
				while ((byte)(((void*)(*(int*)(&obj6)) != System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4))) ? 1u : 0u) != 0)
				{
					int num7 = *(int*)inverseBoneMappingTable;
					ushort num8 = *(ushort*)(*(int*)(&obj6) + 16);
					if (num8 < ((int*)inverseBoneMappingTable)[1] - num7 >> 1)
					{
						*(short*)(num8 * 2 + num7) = (short)num6;
						*(int*)(&obj6) = (int)eastl_002ERBTreeIncrement((rbtree_node_base*)(int)(*(uint*)(&obj6)));
						num6++;
						continue;
					}
					sbyte* ptr13 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FG_0040LMNBEMCD_0040Assert_003F5failed_003F3_003F5_003F_0024CKbone_003F5_003F_0024DO_003F_0024DN_003F50_003F5_003F_0024CG_003F_0024CG_003F5_003F_0024CKbo_0040);
					_CxxThrowException(&ptr13, (_s__ThrowInfo*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _TI2PAD));
					break;
				}
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Eset_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &obj);
			throw;
		}
		rbtree_node_003Cunsigned_0020short_003E* ptr14 = (rbtree_node_003Cunsigned_0020short_003E*)(int)System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 12));
		if (System.Runtime.CompilerServices.Unsafe.As<set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 12)) != 0)
		{
			rbtree_node_003Cunsigned_0020short_003E* ptr15;
			do
			{
				eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoNukeSubtree((rbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E*)(&obj), (rbtree_node_003Cunsigned_0020short_003E*)(int)(*(uint*)ptr14));
				ptr15 = (rbtree_node_003Cunsigned_0020short_003E*)(int)((uint*)ptr14)[1];
				delete_005B_005D(ptr14);
				ptr14 = ptr15;
			}
			while (ptr15 != null);
		}
	}

	internal unsafe static void eastl_002Eset_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(set_003Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		rbtree_node_003Cunsigned_0020short_003E* ptr = (rbtree_node_003Cunsigned_0020short_003E*)(int)((uint*)P_0)[3];
		if (ptr != null)
		{
			rbtree_node_003Cunsigned_0020short_003E* ptr2;
			do
			{
				eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoNukeSubtree((rbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E*)P_0, (rbtree_node_003Cunsigned_0020short_003E*)(int)(*(uint*)ptr));
				ptr2 = (rbtree_node_003Cunsigned_0020short_003E*)(int)((uint*)ptr)[1];
				delete_005B_005D(ptr);
				ptr = ptr2;
			}
			while (ptr2 != null);
		}
	}

	internal unsafe static void W3DMeshProcessor_002EBuildVertexDeclaration(vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* mappings, String_003Cchar_003E* declaration, DataHandler* dataHandlers, Tracker* tracker, TargetPlatform platform)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0CAA_0040D _0024ArrayType_0024_0024_0024BY0CAA_0040D);
		sbyte* ptr = (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D);
		DataMapping* ptr2 = (DataMapping*)(int)(*(uint*)mappings);
		if (ptr2 != (DataMapping*)((int*)mappings)[1])
		{
			do
			{
				if (ptr2 != (DataMapping*)(*(int*)mappings))
				{
					*ptr = 32;
					ptr++;
				}
				int num = *(int*)ptr2;
				UsageType usageType = (UsageType)num;
				int num2 = ((int*)ptr2)[1];
				int num3 = ((int*)ptr2)[4];
				if (usageType == (UsageType)7 || usageType == (UsageType)6)
				{
					DataMapping* ptr3 = ptr2;
					uint num4 = ((uint*)mappings)[1];
					if (ptr2 != (DataMapping*)(int)num4)
					{
						while (*(int*)ptr3 == num)
						{
							ptr3 = (DataMapping*)((byte*)ptr3 + 20);
							if (ptr3 == (DataMapping*)(int)num4)
							{
								break;
							}
						}
					}
					if ((nint)((byte*)ptr3 - (nuint)ptr2) / (nint)20 > 4)
					{
						sbyte* ptr4 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BL_0040DBJIEAG_0040Assert_003F5failed_003F3_003F5m2_003F5_003F9_003F5m_003F5_003F_0024DM_003F_0024DN_003F54_003F_0024AA_0040);
						_CxxThrowException(&ptr4, (_s__ThrowInfo*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _TI2PAD));
						break;
					}
					if (platform == TargetPlatform.Xbox360)
					{
						int num5 = num3;
						num3 = num5 - num5 % 4;
					}
					ptr2 = (DataMapping*)((byte*)ptr3 - 20);
				}
				sbyte b = 63;
				DataHandler* ptr5 = (DataHandler*)((byte*)dataHandlers + (int)usageType * 16);
				RNA_002EElementNameToCharCode(*(ElementNameE*)ptr5, &b);
				ptr = sprintf(ptr, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0N_0040BKCHANDB_0040_003F_0024CFc_003F_0024CFX_003F3_003F_0024CF02X_003F3_003F_0024CFs_003F_0024AA_0040), __arglist((int)b, num2, num3, RNA_002EElementFormatGetName(((ElementFormatE*)ptr5)[1]))) + ptr;
				ptr2 = (DataMapping*)((byte*)ptr2 + 20);
			}
			while (ptr2 != (DataMapping*)((int*)mappings)[1]);
		}
		*ptr = 0;
		sbyte* ptr6 = (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D);
		sbyte* ptr7 = (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D);
		if (*(bool*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D))
		{
			do
			{
				ptr7++;
			}
			while (*ptr7 != 0);
		}
		uint num6 = (uint)(*(int*)declaration = (ref *(_003F*)ptr7) - (ref *(_003F*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D)));
		String_003Cchar_003E* ptr8 = (String_003Cchar_003E*)((byte*)declaration + 4);
		Relo_002ETracker_002EPush(tracker, (void**)ptr8, 1u, num6 + 1);
		sbyte* ptr9 = (sbyte*)(int)(*(uint*)ptr8);
		sbyte b2;
		do
		{
			*ptr9 = *ptr6;
			ptr6++;
			b2 = *ptr9;
			ptr9++;
		}
		while (b2 != 0);
		((int*)((byte*)tracker + 4))[1] += -4;
	}

	internal unsafe static void W3DMeshProcessor_002EBuildVertexData(W3DMeshPipelineVertexData* pipelineData, W3DMeshVertexData* outputData, Tracker* tracker, EndianHelper* endian, TargetPlatform platform)
	{
		DataHandler* ptr = null;
		switch (platform)
		{
		case TargetPlatform.Win32:
			ptr = (DataHandler*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref W3DMeshProcessor_002E_003FA0x431bf441_002EDataHandlers_Windows);
			break;
		case TargetPlatform.Xbox360:
			ptr = (DataHandler*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref W3DMeshProcessor_002E_003FA0x431bf441_002EDataHandlers_Xenon);
			break;
		case TargetPlatform.PlayStation3:
			ptr = (DataHandler*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref W3DMeshProcessor_002E_003FA0x431bf441_002EDataHandlers_PS3);
			break;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E);
		*(int*)(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E) = 0;
		System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 8)) = 0;
		try
		{
			W3DMeshProcessor_002EDetermineVertexDataContents(pipelineData, &vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E);
			int num = W3DMeshProcessor_002EDetermineVertexDataLayout(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, ptr, tracker, endian, platform);
			int num2 = *(int*)(int)((uint*)pipelineData)[1];
			System.Runtime.CompilerServices.Unsafe.SkipInit(out ConverterData converterData);
			*(int*)(&converterData) = 0;
			System.Runtime.CompilerServices.Unsafe.As<ConverterData, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref converterData, 4)) = 0;
			System.Runtime.CompilerServices.Unsafe.As<ConverterData, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref converterData, 8)) = 0;
			try
			{
				System.Runtime.CompilerServices.Unsafe.As<ConverterData, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref converterData, 16)) = (int)tracker;
				System.Runtime.CompilerServices.Unsafe.As<ConverterData, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref converterData, 20)) = (int)endian;
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &converterData);
				throw;
			}
			try
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E obj);
				*(int*)(&obj) = 0;
				System.Runtime.CompilerServices.Unsafe.As<vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4)) = 0;
				System.Runtime.CompilerServices.Unsafe.As<vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 8)) = 0;
				try
				{
					W3DMeshProcessor_002EComputeBoneMappingTable(pipelineData, &obj, (vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E*)(&converterData));
					int num3 = num2;
					if (*(bool*)endian)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
						*(sbyte*)(&num4) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3)) = *(sbyte*)(&num2);
						num3 = (int)num4;
					}
					*(int*)outputData = num3;
					int num5 = num;
					if (*(bool*)endian)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num6);
						*(sbyte*)(&num6) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num6, 3)) = *(sbyte*)(&num);
						num5 = (int)num6;
					}
					((int*)outputData)[1] = num5;
					uint num7 = (uint)(num2 * num);
					Relo_002ETracker_002EPush(tracker, (void**)((byte*)outputData + 8), 1u, num7);
					// IL initblk instruction
					System.Runtime.CompilerServices.Unsafe.InitBlock(((int*)outputData)[2], 0, num7);
					if (*(int*)(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E) != System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 4)))
					{
						int num8 = *(int*)(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E) + 12;
						do
						{
							((delegate* unmanaged[Cdecl, Cdecl]<int, sbyte*, int, sbyte*, int, ConverterData*, void>)(int)((uint*)((byte*)ptr + *(int*)(num8 - 12) * 16))[3])(num2, (sbyte*)(int)(*(uint*)(num8 - 4)), *(int*)num8, (sbyte*)(*(int*)(num8 + 4) + ((int*)outputData)[2]), num, &converterData);
							num8 += 20;
						}
						while (num8 - 12 != System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 4)));
					}
					DataMapping* ptr2 = (DataMapping*)(int)(*(uint*)(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E));
					if (*(int*)(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E) != System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 4)))
					{
						do
						{
							if (*(int*)ptr2 != 6)
							{
								ptr2 = (DataMapping*)((byte*)ptr2 + 20);
								continue;
							}
							DataMapping* ptr3 = ptr2;
							if (ptr2 != (DataMapping*)System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 4)))
							{
								int num9 = *(int*)ptr2;
								while (*(int*)ptr3 == num9)
								{
									ptr3 = (DataMapping*)((byte*)ptr3 + 20);
									if (ptr3 == (DataMapping*)System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 4)))
									{
										break;
									}
								}
							}
							if ((nint)((byte*)ptr3 - (nuint)ptr2) / (nint)20 != 2)
							{
								sbyte* ptr4 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BL_0040GICDAKDA_0040Assert_003F5failed_003F3_003F5m2_003F5_003F9_003F5m_003F5_003F_0024DN_003F_0024DN_003F52_003F_0024AA_0040);
								_CxxThrowException(&ptr4, (_s__ThrowInfo*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _TI2PAD));
							}
							int num10 = ((int*)outputData)[2];
							byte* ptr5 = (byte*)(((int*)ptr2)[9] + num10);
							byte* ptr6 = (byte*)(((int*)ptr2)[4] + num10);
							int num11 = num2;
							if (num2 != 0)
							{
								do
								{
									*ptr5 = (byte)(255 - *ptr6);
									ptr6 = num + ptr6;
									ptr5 = num + ptr5;
									num11--;
								}
								while (num11 != 0);
							}
							break;
						}
						while (ptr2 != (DataMapping*)System.Runtime.CompilerServices.Unsafe.As<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, 4)));
					}
					stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr7 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)tracker + 4);
					((int*)ptr7)[1] += -4;
					W3DMeshProcessor_002EBuildVertexDeclaration(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E, (String_003Cchar_003E*)((byte*)outputData + 12), ptr, tracker, platform);
					int num12 = System.Runtime.CompilerServices.Unsafe.As<vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref obj, 4)) - *(int*)(&obj) >> 1;
					uint num13 = (uint)num12;
					if (*(bool*)endian)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num14);
						*(sbyte*)(&num14) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 3));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 1)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 2));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 2)) = System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num13, 1));
						System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num14, 3)) = *(sbyte*)(&num13);
						num13 = num14;
					}
					((int*)outputData)[5] = (int)num13;
					uint objectCount = (uint)num12;
					W3DMeshVertexData* ptr8 = (W3DMeshVertexData*)((byte*)outputData + 24);
					Relo_002ETracker_002EPush(tracker, (void**)ptr8, 2u, objectCount);
					uint num15 = 0u;
					if (0u < (uint)num12)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out ushort num17);
						do
						{
							ushort num16 = *(ushort*)(int)(num15 * 2 + (uint)(*(int*)(&obj)));
							if (*(bool*)endian)
							{
								*(sbyte*)(&num17) = System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num16, 1));
								System.Runtime.CompilerServices.Unsafe.As<ushort, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num17, 1)) = *(sbyte*)(&num16);
								num16 = num17;
							}
							*(ushort*)(int)(num15 * 2 + (uint)(*(int*)ptr8)) = num16;
							num15++;
						}
						while (num15 < (uint)num12);
					}
					stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E* ptr9 = (stack_003Cunsigned_0020int_002Ceastl_003A_003Avector_003Cunsigned_0020int_002Ceastl_003A_003Aallocator_003E_0020_003E*)((byte*)tracker + 4);
					((int*)ptr9)[1] += -4;
				}
				catch
				{
					//try-fault
					___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &obj);
					throw;
				}
				if (*(int*)(&obj) != 0)
				{
					delete_005B_005D((void*)(int)(*(uint*)(&obj)));
				}
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<ConverterData*, void>)(&W3DMeshProcessor_002EConverterData_002E_007Bdtor_007D), &converterData);
				throw;
			}
			if (*(int*)(&converterData) != 0)
			{
				delete_005B_005D((void*)(int)(*(uint*)(&converterData)));
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		if (*(int*)(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			delete_005B_005D((void*)(int)(*(uint*)(&vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E)));
		}
	}

	internal unsafe static void W3DMeshProcessor_002EConverterData_002E_007Bdtor_007D(ConverterData* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static void eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static void eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002Eresize(vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E* P_0, uint n, ushort* value)
	{
		int num = ((int*)P_0)[1];
		int num2 = *(int*)P_0;
		int num3 = num - num2 >> 1;
		if (n > (uint)num3)
		{
			eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002EDoInsertValues(P_0, (ushort*)num, n - (uint)num3, value);
			return;
		}
		ushort* ptr = (ushort*)num;
		ushort* ptr2 = (ushort*)(int)(n * 2 + (uint)num2);
		ushort* ptr3 = (ushort*)num;
		memmove(ptr2, ptr, (uint)((byte*)ptr3 - (nuint)ptr));
		((int*)P_0)[1] = (int)(((int*)P_0)[1] + ((nint)((byte*)ptr - (nuint)ptr2) >> 1) * -2);
	}

	internal unsafe static void W3DMeshProcessor_002ECopyDirect_003Cstruct_0020SageMathOnly_003A_003AMath_003A_003AVector3_003E(int numElements, sbyte* srcData, int srcPitch, sbyte* destData, int destPitch, ConverterData* converterData)
	{
		if (*(bool*)(int)((uint*)converterData)[5])
		{
			sbyte* ptr = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0DF_0040HPFNIGGK_0040Assert_003F5failed_003F3_003F5_003F_0024CBconverterData_003F4m__0040);
			_CxxThrowException(&ptr, (_s__ThrowInfo*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _TI2PAD));
		}
		if (numElements != 0)
		{
			do
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(destData, srcData, 12);
				srcData = srcPitch + srcData;
				destData = destPitch + destData;
				numElements--;
			}
			while (numElements != 0);
		}
	}

	internal unsafe static void eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (num != 0)
		{
			delete_005B_005D((void*)num);
		}
	}

	internal unsafe static void eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002Epush_back(vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* P_0, DataMapping* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num < (uint)((int*)P_0)[2])
		{
			int num2 = (int)num;
			((int*)P_0)[1] = (int)(num + 20);
			if (num2 != 0)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(num2, value, 20);
			}
		}
		else
		{
			eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(P_0, (DataMapping*)(int)num, value);
		}
	}

	internal unsafe static void eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002Eclear(vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = ((int*)P_0)[1];
		DataMapping* ptr = (DataMapping*)num;
		DataMapping* ptr2 = (DataMapping*)(int)(*(uint*)P_0);
		DataMapping* ptr3 = (DataMapping*)num;
		DataMapping* ptr4 = ptr2;
		DataMapping* ptr5 = ptr;
		if (ptr != ptr3)
		{
			do
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr4, ptr5, 20);
				ptr4 = (DataMapping*)((byte*)ptr4 + 20);
				ptr5 = (DataMapping*)((byte*)ptr5 + 20);
			}
			while (ptr5 != ptr3);
		}
		((int*)P_0)[1] = (int)(((int*)P_0)[1] + (nint)((byte*)ptr - (nuint)ptr2) / (nint)(-20) * 20);
	}

	internal unsafe static void eastl_002Evector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E_002EDoInsertValue(vector_003CW3DMeshProcessor_003A_003ADataMapping_002Ceastl_003A_003Aallocator_003E* P_0, DataMapping* position, DataMapping* value)
	{
		uint num = ((uint*)P_0)[1];
		if (num != (uint)((int*)P_0)[2])
		{
			DataMapping* ptr = value;
			if (value >= position && (nuint)value < (nuint)(int)num)
			{
				ptr = (DataMapping*)((byte*)value + 20);
			}
			DataMapping* ptr2 = (DataMapping*)(int)num;
			if (ptr2 != null)
			{
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr2, (int)(num - 20), 20);
			}
			num = ((uint*)P_0)[1];
			DataMapping* ptr3 = (DataMapping*)(int)num;
			DataMapping* ptr4 = (DataMapping*)(int)(num - 20);
			if (ptr4 != position)
			{
				do
				{
					ptr4 = (DataMapping*)((byte*)ptr4 - 20);
					ptr3 = (DataMapping*)((byte*)ptr3 - 20);
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr3, ptr4, 20);
				}
				while (ptr4 != position);
			}
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(position, ptr, 20);
			((int*)P_0)[1] += 20;
			return;
		}
		uint num2 = (uint)(((int)num - *(int*)P_0) / 20);
		uint num3;
		DataMapping* ptr5;
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
		ptr5 = (DataMapping*)new_005B_005D(num3 * 20, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		goto IL_00ac;
		IL_00ac:
		DataMapping* ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_003E((DataMapping*)(int)(*(uint*)P_0), position, ptr5);
		if (ptr6 != null)
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr6, value, 20);
		}
		ptr6 = eastl_002Euninitialized_copy_ptr_003Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_003E(position, (DataMapping*)(int)((uint*)P_0)[1], (DataMapping*)((byte*)ptr6 + 20));
		DataMapping* ptr7 = (DataMapping*)(int)(*(uint*)P_0);
		if (ptr7 != null)
		{
			delete_005B_005D(ptr7);
		}
		*(int*)P_0 = (int)ptr5;
		((int*)P_0)[1] = (int)ptr6;
		((int*)P_0)[2] = (int)((int)(num3 * 20) + (byte*)ptr5);
	}

	internal unsafe static void eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoNukeSubtree(rbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E* P_0, rbtree_node_003Cunsigned_0020short_003E* pNode)
	{
		if (pNode != null)
		{
			rbtree_node_003Cunsigned_0020short_003E* ptr;
			do
			{
				eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoNukeSubtree(P_0, (rbtree_node_003Cunsigned_0020short_003E*)(int)(*(uint*)pNode));
				ptr = (rbtree_node_003Cunsigned_0020short_003E*)(int)((uint*)pNode)[1];
				delete_005B_005D(pNode);
				pNode = ptr;
			}
			while (ptr != null);
		}
	}

	internal unsafe static pair_003Ceastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cbool_003E* eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoInsertValue(rbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E* P_0, pair_003Ceastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cbool_003E* P_1, ushort* value, integral_constant_003Cbool_002C1_003E __unnamed001)
	{
		rbtree_node_003Cunsigned_0020short_003E* ptr = (rbtree_node_003Cunsigned_0020short_003E*)(int)((uint*)P_0)[3];
		rbtree_node_003Cunsigned_0020short_003E* ptr2 = (rbtree_node_003Cunsigned_0020short_003E*)((byte*)P_0 + 4);
		bool flag = true;
		if (ptr != null)
		{
			ushort num = *value;
			do
			{
				flag = (uint)num < (uint)((ushort*)ptr)[8];
				ptr2 = ptr;
				ptr = (rbtree_node_003Cunsigned_0020short_003E*)((!flag) ? ((int)(*(uint*)ptr)) : ((int)((uint*)ptr)[1]));
			}
			while (ptr != null);
		}
		rbtree_node_003Cunsigned_0020short_003E* pNodeParent = ptr2;
		if (flag)
		{
			if (ptr2 == (rbtree_node_003Cunsigned_0020short_003E*)((int*)P_0)[2])
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj);
				eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoInsertValueImpl(P_0, &obj, ptr2, value, false);
				// IL cpblk instruction
				System.Runtime.CompilerServices.Unsafe.CopyBlock(P_1, ref obj, 4);
				((sbyte*)P_1)[4] = 1;
				return P_1;
			}
			ptr2 = (rbtree_node_003Cunsigned_0020short_003E*)eastl_002ERBTreeDecrement((rbtree_node_base*)ptr2);
		}
		if ((uint)((ushort*)ptr2)[8] < (uint)(*value))
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj2);
			eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoInsertValueImpl(P_0, &obj2, pNodeParent, value, false);
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(P_1, ref obj2, 4);
			((sbyte*)P_1)[4] = 1;
			return P_1;
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj3);
		*(int*)(&obj3) = (int)ptr2;
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(P_1, ref obj3, 4);
		((sbyte*)P_1)[4] = 0;
		return P_1;
	}

	internal unsafe static void eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002EDoInsertValues(vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E* P_0, ushort* position, uint n, ushort* value)
	{
		int num = ((int*)P_0)[1];
		if (n <= (uint)(((int*)P_0)[2] - num >> 1))
		{
			if (n == 0)
			{
				return;
			}
			ushort num2 = *value;
			uint num3 = (uint)((nint)num - (nint)position >> 1);
			num = ((int*)P_0)[1];
			ushort* ptr = (ushort*)num;
			if (n < num3)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj);
				*(int*)(&obj) = num;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj2);
				*(int*)(&obj2) = num;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj3);
				int num4 = (*(int*)(&obj3) = num - (int)(n * 2));
				memmove((void*)(int)(*(uint*)(&obj)), (void*)(int)(*(uint*)(&obj3)), (uint)(*(int*)(&obj2) - *(int*)(&obj3)));
				((int*)P_0)[1] += (int)(n * 2);
				uint num5 = (uint)((nuint)num4 - (nuint)position);
				memmove((byte*)ptr - ((int)num5 >> 1 << 1), position, num5);
				ushort* ptr2 = position;
				ushort* ptr3 = (ushort*)((int)(n * 2) + (byte*)position);
				if (position != ptr3)
				{
					do
					{
						*ptr2 = num2;
						ptr2++;
					}
					while (ptr2 != ptr3);
				}
				return;
			}
			uint num6 = n - num3;
			eastl_002Euninitialized_fill_n_ptr_003Cunsigned_0020short_002Cunsigned_0020int_003E((ushort*)num, num6, &num2);
			((int*)P_0)[1] += (int)(num6 << 1);
			int num7 = ((int*)P_0)[1];
			System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj4);
			*(int*)(&obj4) = num7;
			memmove((void*)(int)(*(uint*)(&obj4)), position, (uint)((byte*)ptr - (nuint)position));
			((int*)P_0)[1] += (int)(num3 << 1);
			ushort* ptr4 = position;
			if (position != ptr)
			{
				do
				{
					*ptr4 = num2;
					ptr4++;
				}
				while (ptr4 != ptr);
			}
			return;
		}
		uint num8 = (uint)(num - *(int*)P_0 >> 1);
		uint num9 = ((num8 == 0) ? 1u : (num8 << 1));
		uint num10 = n + num8;
		uint num11 = ((num9 <= num10) ? num10 : num9);
		ushort* ptr5 = (ushort*)((num11 == 0) ? null : new_005B_005D(num11 << 1, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209));
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj5);
		*(int*)(&obj5) = *(int*)P_0;
		ushort* ptr6 = (ushort*)((byte*)position - *(int*)(&obj5));
		ushort* ptr7 = ((nint)ptr6 >> 1) + (ushort*)memmove(ptr5, (void*)(int)(*(uint*)(&obj5)), (uint)ptr6);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj6);
		*(int*)(&obj6) = (int)ptr7;
		uint num12 = n;
		generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj7 = obj6;
		ushort num13 = *value;
		if (n != 0)
		{
			do
			{
				num12--;
				*(ushort*)(int)(*(uint*)(&obj7)) = num13;
				*(int*)(&obj7) += 2;
			}
			while (num12 != 0);
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj8);
		*(int*)(&obj8) = (int)((int)(n * 2) + (byte*)ptr7);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj9);
		*(int*)(&obj9) = ((int*)P_0)[1];
		ushort* ptr8 = (ushort*)((nuint)(*(int*)(&obj9)) - (nuint)position);
		ptr7 = ((nint)ptr8 >> 1) + (ushort*)memmove((void*)(int)(*(uint*)(&obj8)), position, (uint)ptr8);
		ushort* ptr9 = (ushort*)(int)(*(uint*)P_0);
		if (ptr9 != null)
		{
			delete_005B_005D(ptr9);
		}
		*(int*)P_0 = (int)ptr5;
		((int*)P_0)[1] = (int)ptr7;
		((int*)P_0)[2] = (int)((int)(num11 * 2) + (byte*)ptr5);
	}

	internal unsafe static DataMapping* eastl_002Euninitialized_copy_ptr_003Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_003E(DataMapping* first, DataMapping* last, DataMapping* result)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E dest);
		*(int*)(&dest) = (int)result;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E last2);
		*(int*)(&last2) = (int)last;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E first2);
		*(int*)(&first2) = (int)first;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E obj);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out integral_constant_003Cbool_002C0_003E _unnamed);
		eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E_0020_003E(&obj, first2, last2, dest, _unnamed);
		return (DataMapping*)(int)(*(uint*)(&obj));
	}

	internal unsafe static rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E* eastl_002Erbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E_002EDoInsertValueImpl(rbtree_003Cunsigned_0020short_002Cunsigned_0020short_002Ceastl_003A_003Aless_003Cunsigned_0020short_003E_002Ceastl_003A_003Aallocator_002Ceastl_003A_003Ause_self_003Cunsigned_0020short_003E_002C0_002C1_003E* P_0, rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E* P_1, rbtree_node_003Cunsigned_0020short_003E* pNodeParent, ushort* value, [MarshalAs(UnmanagedType.U1)] bool bForceToLeft)
	{
		RBTreeSide rBTreeSide = ((!bForceToLeft && pNodeParent != (byte*)P_0 + 4 && !((uint)(*value) < (uint)((ushort*)pNodeParent)[8])) ? ((RBTreeSide)1) : ((RBTreeSide)0));
		rbtree_node_003Cunsigned_0020short_003E* ptr = (rbtree_node_003Cunsigned_0020short_003E*)new_005B_005D(20u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
		ushort* ptr2 = (ushort*)ptr + 8;
		if (ptr2 != null)
		{
			*ptr2 = *value;
		}
		eastl_002ERBTreeInsert((rbtree_node_base*)ptr, (rbtree_node_base*)pNodeParent, (rbtree_node_base*)((byte*)P_0 + 4), rBTreeSide);
		((int*)P_0)[5]++;
		*(int*)P_1 = (int)ptr;
		return P_1;
	}

	internal unsafe static void eastl_002Evector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E_002EDoInsertFromIterator_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_0020_003E(vector_003Cunsigned_0020short_002Ceastl_003A_003Aallocator_003E* P_0, ushort* position, rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E first, rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E last, forward_iterator_tag __unnamed003)
	{
		if ((byte)((*(int*)(&first) != *(int*)(&last)) ? 1 : 0) == 0)
		{
			return;
		}
		rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj = first;
		int num = 0;
		while ((byte)((*(int*)(&obj) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			*(int*)(&obj) = (int)eastl_002ERBTreeIncrement((rbtree_node_base*)(int)(*(uint*)(&obj)));
			num++;
		}
		int num2 = ((int*)P_0)[1];
		if ((uint)num <= (uint)(((int*)P_0)[2] - num2 >> 1))
		{
			uint num3 = (uint)((nint)num2 - (nint)position >> 1);
			int num4 = ((int*)P_0)[1];
			ushort* ptr = (ushort*)num4;
			if ((uint)num < num3)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj2);
				*(int*)(&obj2) = num4;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj3);
				*(int*)(&obj3) = num4;
				int num5 = num * 2;
				System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj4);
				int num6 = (*(int*)(&obj4) = num4 - num5);
				memmove((void*)(int)(*(uint*)(&obj2)), (void*)(int)(*(uint*)(&obj4)), (uint)(*(int*)(&obj3) - *(int*)(&obj4)));
				((int*)P_0)[1] += num5;
				uint num7 = (uint)((nuint)num6 - (nuint)position);
				memmove((byte*)ptr - ((int)num7 >> 1 << 1), position, num7);
				eastl_002Ecopy_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cunsigned_0020short_0020_002A_003E(first, last, position);
				return;
			}
			rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj5 = first;
			uint num8 = num3;
			if (num3 != 0)
			{
				do
				{
					num8--;
					*(int*)(&obj5) = (int)eastl_002ERBTreeIncrement((rbtree_node_base*)(int)(*(uint*)(&obj5)));
				}
				while (num8 != 0);
			}
			eastl_002Euninitialized_copy_ptr_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cunsigned_0020short_0020_002A_003E(obj5, last, (ushort*)(int)((uint*)P_0)[1]);
			((int*)P_0)[1] += num - (int)num3 << 1;
			int num9 = ((int*)P_0)[1];
			System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj6);
			*(int*)(&obj6) = num9;
			memmove((void*)(int)(*(uint*)(&obj6)), position, (uint)((byte*)ptr - (nuint)position));
			int num10 = (int)(num3 * 2);
			((int*)P_0)[1] += num10;
			eastl_002Ecopy_backward_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cunsigned_0020short_0020_002A_003E(first, obj5, (ushort*)(num10 + (byte*)position));
		}
		else
		{
			uint num11 = (uint)(num2 - *(int*)P_0 >> 1);
			uint num12 = ((num11 == 0) ? 1u : (num11 << 1));
			uint num13 = num11 + (uint)num;
			uint num14 = ((num12 <= num13) ? num13 : num12);
			ushort* ptr2 = (ushort*)((num14 == 0) ? null : new_005B_005D(num14 << 1, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209));
			System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj7);
			*(int*)(&obj7) = *(int*)P_0;
			ushort* ptr3 = (ushort*)((byte*)position - *(int*)(&obj7));
			ushort* result = ((nint)ptr3 >> 1) + (ushort*)memmove(ptr2, (void*)(int)(*(uint*)(&obj7)), (uint)ptr3);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj8);
			*(int*)(&obj8) = (int)eastl_002Euninitialized_copy_ptr_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cunsigned_0020short_0020_002A_003E(first, last, result);
			System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj9);
			*(int*)(&obj9) = ((int*)P_0)[1];
			ushort* ptr4 = (ushort*)((nuint)(*(int*)(&obj9)) - (nuint)position);
			ushort* ptr5 = ((nint)ptr4 >> 1) + (ushort*)memmove((void*)(int)(*(uint*)(&obj8)), position, (uint)ptr4);
			ushort* ptr6 = (ushort*)(int)(*(uint*)P_0);
			if (ptr6 != null)
			{
				delete_005B_005D(ptr6);
			}
			*(int*)P_0 = (int)ptr2;
			((int*)P_0)[1] = (int)ptr5;
			((int*)P_0)[2] = (int)((int)(num14 * 2) + (byte*)ptr2);
		}
	}

	internal unsafe static void eastl_002Euninitialized_fill_n_ptr_003Cunsigned_0020short_002Cunsigned_0020int_003E(ushort* first, uint n, ushort* value)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj);
		*(int*)(&obj) = (int)first;
		uint num = n;
		generic_iterator_003Cunsigned_0020short_0020_002A_002Cvoid_003E obj2 = obj;
		ushort num2 = *value;
		if (n != 0)
		{
			do
			{
				num--;
				*(ushort*)(int)(*(uint*)(&obj2)) = num2;
				*(int*)(&obj2) += 2;
			}
			while (num != 0);
		}
	}

	internal unsafe static generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E* eastl_002Euninitialized_copy_impl_003Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E_002Cclass_0020eastl_003A_003Ageneric_iterator_003Cstruct_0020W3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E_0020_003E(generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E* P_0, generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E first, generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E last, generic_iterator_003CW3DMeshProcessor_003A_003ADataMapping_0020_002A_002Cvoid_003E dest, integral_constant_003Cbool_002C0_003E __unnamed003)
	{
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock(P_0, ref dest, 4);
		while ((byte)((*(int*)(&first) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			int num = *(int*)P_0;
			void* ptr = (void*)num;
			DataMapping* ptr2 = (DataMapping*)ptr;
			try
			{
				if (ptr != null)
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr, *(int*)(&first), 20);
				}
			}
			catch
			{
				//try-fault
				delete(ptr2, ptr);
				throw;
			}
			*(int*)(&first) += 20;
			*(int*)P_0 = num + 20;
		}
		return P_0;
	}

	internal unsafe static ushort* eastl_002Ecopy_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cunsigned_0020short_0020_002A_003E(rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E first, rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E last, ushort* result)
	{
		ushort* ptr = result;
		rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj = first;
		while ((byte)((*(int*)(&obj) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			*ptr = *(ushort*)(*(int*)(&obj) + 16);
			ptr++;
			*(int*)(&obj) = (int)eastl_002ERBTreeIncrement((rbtree_node_base*)(int)(*(uint*)(&obj)));
		}
		return ptr;
	}

	internal unsafe static ushort* eastl_002Euninitialized_copy_ptr_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cunsigned_0020short_0020_002A_003E(rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E first, rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E last, ushort* result)
	{
		ushort* ptr = result;
		rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj = first;
		while ((byte)((*(int*)(&obj) != *(int*)(&last)) ? 1u : 0u) != 0)
		{
			*ptr = *(ushort*)(*(int*)(&obj) + 16);
			ptr++;
			*(int*)(&obj) = (int)eastl_002ERBTreeIncrement((rbtree_node_base*)(int)(*(uint*)(&obj)));
		}
		return ptr;
	}

	internal unsafe static ushort* eastl_002Ecopy_backward_003Cstruct_0020eastl_003A_003Arbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E_002Cunsigned_0020short_0020_002A_003E(rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E first, rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E last, ushort* result)
	{
		ushort* ptr = result;
		rbtree_iterator_003Cunsigned_0020short_002Cunsigned_0020short_0020const_0020_002A_002Cunsigned_0020short_0020const_0020_0026_003E obj = last;
		while ((byte)((*(int*)(&obj) != *(int*)(&first)) ? 1u : 0u) != 0)
		{
			ptr--;
			*(int*)(&obj) = (int)eastl_002ERBTreeDecrement((rbtree_node_base*)(int)(*(uint*)(&obj)));
			*ptr = *(ushort*)(*(int*)(&obj) + 16);
		}
		return ptr;
	}

	internal unsafe static sbyte* RNA_002EElementFormatGetName(ElementFormatE eFormat)
	{
		return (sbyte*)(int)(*(uint*)((ref *(_003F*)((int)eFormat * 12)) + (ref System.Runtime.CompilerServices.Unsafe.As<_0024ArrayType_0024_0024_0024BY0BO_0040_0024_0024CBUElementInfoT_0040_0040, _003F>(ref _003FA0xfa41eb23_002Es_ElementInfo))));
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool RNA_002EElementNameToCharCode(ElementNameE eElementName, sbyte* cName)
	{
		switch (eElementName)
		{
		case (ElementNameE)1:
			*cName = 112;
			goto IL_0058;
		case (ElementNameE)4:
			*cName = 110;
			goto IL_0058;
		case (ElementNameE)2:
			*cName = 99;
			goto IL_0058;
		case (ElementNameE)3:
			*cName = 116;
			goto IL_0058;
		case (ElementNameE)5:
			*cName = 98;
			goto IL_0058;
		case (ElementNameE)6:
			*cName = 103;
			goto IL_0058;
		case (ElementNameE)7:
			*cName = 119;
			goto IL_0058;
		case (ElementNameE)8:
			*cName = 105;
			goto IL_0058;
		default:
			{
				*cName = 0;
				return false;
			}
			IL_0058:
			return true;
		}
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

	[DebuggerStepThrough]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
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

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* @new(uint P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void delete(void* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void delete_005B_005D(void* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* calloc(uint P_0, uint P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* memmove(void* P_0, void* P_1, uint P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern double atof(sbyte* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern uint strtoul(sbyte* P_0, sbyte** P_1, int P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int strtol(sbyte* P_0, sbyte** P_1, int P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int atoi(sbyte* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern sbyte* strchr(sbyte* P_0, int P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* malloc(uint P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void free(void* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* new_005B_005D(uint P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern rbtree_node_base* eastl_002ERBTreeIncrement(rbtree_node_base* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern rbtree_node_base* eastl_002ERBTreeDecrement(rbtree_node_base* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void eastl_002ERBTreeInsert(rbtree_node_base* P_0, rbtree_node_base* P_1, rbtree_node_base* P_2, RBTreeSide P_3);

	[DllImport("", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void _CxxThrowException(void* P_0, _s__ThrowInfo* P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int sprintf(sbyte* P_0, sbyte* P_1, __arglist);

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
