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
using AudioPhysicalRendererInternal;
using BinaryAssetBuilder.AudioCompiler;
using EA.Allocator;
using Endian;
using Marshaler;
using Relo;
using SageBinaryData;
using eastl;
using rw.audio.core;
using std;

internal class _003CModule_003E
{
	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040GHFPNOJB_0040bad_003F5allocation_003F_0024AA_0040/* Not supported: data(62 61 64 20 61 6C 6C 6F 63 61 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040CKDJFHMK_0040AudioFileRuntime_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 46 69 6C 65 52 75 6E 74 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BE_0040_0024_0024CBD _003F_003F_C_0040_0BE_0040MGAKKCNG_0040PathMusicMapRuntime_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 4D 61 70 52 75 6E 74 69 6D 65 00) */;

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

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040IFPEINBO_0040ObjectFilter_003F_0024AA_0040/* Not supported: data(4F 62 6A 65 63 74 46 69 6C 74 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09KMMONPLD_0040AudioFile_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 46 69 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BM_0040_0024_0024CBD _003F_003F_C_0040_0BM_0040CCAOJLPP_0040AudioVolumeSliderMultiplier_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 56 6F 6C 75 6D 65 53 6C 69 64 65 72 4D 75 6C 74 69 70 6C 69 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BF_0040_0024_0024CBD _003F_003F_C_0040_0BF_0040LGGFMELN_0040AudioEventLimitGroup_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 45 76 65 6E 74 4C 69 6D 69 74 47 72 6F 75 70 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040MNILEEHF_0040BaseAudioEventInfo_003F_0024AA_0040/* Not supported: data(42 61 73 65 41 75 64 69 6F 45 76 65 6E 74 49 6E 66 6F 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040IDFGKKFA_0040BaseSingleSound_003F_0024AA_0040/* Not supported: data(42 61 73 65 53 69 6E 67 6C 65 53 6F 75 6E 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BH_0040_0024_0024CBD _003F_003F_C_0040_0BH_0040DFKFDHCB_0040AudioFileRefWithWeight_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 46 69 6C 65 52 65 66 57 69 74 68 57 65 69 67 68 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040KJHGCMIE_0040MultisoundSubsoundRef_003F_0024AA_0040/* Not supported: data(4D 75 6C 74 69 73 6F 75 6E 64 53 75 62 73 6F 75 6E 64 52 65 66 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040MOOECIN_0040AudioEvent_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 45 76 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040HOBPOIOC_0040MusicTrack_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 54 72 61 63 6B 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040HDJLBDHJ_0040DialogEvent_003F_0024AA_0040/* Not supported: data(44 69 61 6C 6F 67 45 76 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040ELIKMGED_0040AmbientStream_003F_0024AA_0040/* Not supported: data(41 6D 62 69 65 6E 74 53 74 72 65 61 6D 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040JMEKDHBP_0040Multisound_003F_0024AA_0040/* Not supported: data(4D 75 6C 74 69 73 6F 75 6E 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BC_0040_0024_0024CBD _003F_003F_C_0040_0BC_0040DCGJAJJK_0040ObjectFilterAsset_003F_0024AA_0040/* Not supported: data(4F 62 6A 65 63 74 46 69 6C 74 65 72 41 73 73 65 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BP_0040_0024_0024CBD _003F_003F_C_0040_0BP_0040EJFFBCFN_0040MusicScriptConditionNuggetBase_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 42 61 73 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CI_0040_0024_0024CBD _003F_003F_C_0040_0CI_0040GCFOLCIC_0040MusicScriptConditionExpensiveNug_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 45 78 70 65 6E 73 69 76 65 4E 75 67 67 65 74 42 61 73 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DB_0040_0024_0024CBD _003F_003F_C_0040_0DB_0040HNBBAJKN_0040MusicScriptConditionNugget_Local_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 4C 6F 63 61 6C 50 6C 61 79 65 72 49 73 4F 62 73 65 72 76 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CM_0040_0024_0024CBD _003F_003F_C_0040_0CM_0040NFBBIPON_0040MusicScriptConditionNugget_Units_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 55 6E 69 74 73 46 61 72 46 72 6F 6D 42 61 73 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DA_0040_0024_0024CBD _003F_003F_C_0040_0DA_0040PGLNJJHJ_0040MusicScriptConditionNugget_TimeF_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 54 69 6D 65 46 72 6F 6D 53 74 61 72 74 4F 66 4C 65 76 65 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CM_0040_0024_0024CBD _003F_003F_C_0040_0CM_0040MMDCIGFH_0040MusicScriptConditionNugget_Track_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 54 72 61 63 6B 50 6C 61 79 65 64 43 6F 75 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DE_0040_0024_0024CBD _003F_003F_C_0040_0DE_0040GFJIGHLN_0040MusicScriptConditionNugget_Speci_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 53 70 65 63 69 66 69 63 54 72 61 63 6B 54 79 70 65 50 6C 61 79 69 6E 67 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CL_0040_0024_0024CBD _003F_003F_C_0040_0CL_0040OLMGFMAD_0040MusicScriptConditionNugget_AnyTr_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 41 6E 79 54 72 61 63 6B 50 6C 61 79 69 6E 67 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CO_0040_0024_0024CBD _003F_003F_C_0040_0CO_0040OFNFBHPC_0040MusicScriptConditionNugget_Objec_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 4F 62 6A 65 63 74 73 4F 66 54 79 70 65 45 78 69 73 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DC_0040_0024_0024CBD _003F_003F_C_0040_0DC_0040DAPOIAOL_0040MusicScriptConditionNugget_EvaEv_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 45 76 61 45 76 65 6E 74 50 6C 61 79 65 64 52 65 63 65 6E 74 6C 79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CP_0040_0024_0024CBD _003F_003F_C_0040_0CP_0040IACHHJBG_0040MusicScriptConditionNugget_Objec_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 4F 62 6A 65 63 74 73 4E 65 61 72 45 76 61 45 76 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CL_0040_0024_0024CBD _003F_003F_C_0040_0CL_0040IMIJDMAL_0040MusicScriptConditionNugget_Score_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 53 63 6F 72 65 64 4B 69 6C 6C 43 6F 75 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BP_0040_0024_0024CBD _003F_003F_C_0040_0BP_0040GPJBIHAC_0040MusicScriptConditionNugget_Not_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 4E 6F 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BO_0040_0024_0024CBD _003F_003F_C_0040_0BO_0040IIDGFANI_0040MusicScriptConditionNugget_Or_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 4F 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BP_0040_0024_0024CBD _003F_003F_C_0040_0BP_0040HMPCOPDC_0040MusicScriptConditionNugget_And_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 41 6E 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DE_0040_0024_0024CBD _003F_003F_C_0040_0DE_0040CAEMCIOP_0040MusicScriptConditionNugget_PathM_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 50 61 74 68 4D 75 73 69 63 53 74 61 74 65 4C 61 73 74 41 63 74 69 76 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DJ_0040_0024_0024CBD _003F_003F_C_0040_0DJ_0040KKBEMLPK_0040MusicScriptConditionNugget_PathM_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 50 61 74 68 4D 75 73 69 63 53 74 61 74 65 43 75 72 72 65 6E 74 6C 79 41 63 74 69 76 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CL_0040_0024_0024CBD _003F_003F_C_0040_0CL_0040GAHDNJFH_0040MusicScriptConditionNugget_Engag_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 45 6E 67 61 67 65 6D 65 6E 74 43 6F 75 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DA_0040_0024_0024CBD _003F_003F_C_0040_0DA_0040EDMPCJON_0040MusicScriptConditionNugget_Local_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 4C 6F 63 61 6C 50 6C 61 79 65 72 4B 69 6C 6C 52 61 74 69 6F 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CM_0040_0024_0024CBD _003F_003F_C_0040_0CM_0040CGOOAFMH_0040MusicScriptConditionNugget_HitBy_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 43 6F 6E 64 69 74 69 6F 6E 4E 75 67 67 65 74 5F 48 69 74 42 79 53 75 70 65 72 57 65 61 70 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BM_0040_0024_0024CBD _003F_003F_C_0040_0BM_0040PCIFAKEG_0040MusicScriptTimeoutSpecifier_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 54 69 6D 65 6F 75 74 53 70 65 63 69 66 69 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040DFJDABIN_0040MusicScriptTrack_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 53 63 72 69 70 74 54 72 61 63 6B 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040GADDPDEL_0040MusicPalette_003F_0024AA_0040/* Not supported: data(4D 75 73 69 63 50 61 6C 65 74 74 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040COONHBIC_0040PathMusicMap_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 4D 61 70 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040JBDNMGA_0040PathMusicTrack_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 54 72 61 63 6B 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040HOPJKBAE_0040PathMusicEvent_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 45 76 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040GBJPFPNB_0040PathMusicEventRuntime_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 45 76 65 6E 74 52 75 6E 74 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BC_0040_0024_0024CBD _003F_003F_C_0040_0BC_0040KIIDHKMH_0040PathMusicEventSet_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 45 76 65 6E 74 53 65 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BK_0040_0024_0024CBD _003F_003F_C_0040_0BK_0040COPELJDH_0040PathMusicGameDynamicState_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 47 61 6D 65 44 79 6E 61 6D 69 63 53 74 61 74 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BN_0040_0024_0024CBD _003F_003F_C_0040_0BN_0040HCAEMFLK_0040PathMusicGameDynamicStateSet_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 47 61 6D 65 44 79 6E 61 6D 69 63 53 74 61 74 65 53 65 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BP_0040_0024_0024CBD _003F_003F_C_0040_0BP_0040NKJDDMFF_0040PathMusicGameDynamicTransition_003F_0024AA_0040/* Not supported: data(50 61 74 68 4D 75 73 69 63 47 61 6D 65 44 79 6E 61 6D 69 63 54 72 61 6E 73 69 74 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BI_0040_0024_0024CBD _003F_003F_C_0040_0BI_0040DLAMEGDH_0040AudioFileMP3Passthrough_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 46 69 6C 65 4D 50 33 50 61 73 73 74 68 72 6F 75 67 68 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BP_0040_0024_0024CBD _003F_003F_C_0040_0BP_0040NPELLMAG_0040AudioFileMP3PassthroughRuntime_003F_0024AA_0040/* Not supported: data(41 75 64 69 6F 46 69 6C 65 4D 50 33 50 61 73 73 74 68 72 6F 75 67 68 52 75 6E 74 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040FIFNHEOG_0040MP3MusicTrack_003F_0024AA_0040/* Not supported: data(4D 50 33 4D 75 73 69 63 54 72 61 63 6B 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040HENAJFPM_0040MP3DialogEvent_003F_0024AA_0040/* Not supported: data(4D 50 33 44 69 61 6C 6F 67 45 76 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040JKFILBD_0040MP3AmbientStream_003F_0024AA_0040/* Not supported: data(4D 50 33 41 6D 62 69 65 6E 74 53 74 72 65 61 6D 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040/* Not supported: data(74 72 75 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040/* Not supported: data(45 41 53 54 4C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0FB_0040_0024_0024CBD _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040/* Not supported: data(43 3A 5C 70 72 6F 6A 65 63 74 73 5C 52 61 33 5C 50 72 6F 64 75 63 74 69 6F 6E 5C 63 6F 64 65 5C 65 78 74 65 72 6E 61 6C 5C 45 41 53 54 4C 5C 31 2E 30 36 2E 30 31 5C 69 6E 63 6C 75 64 65 5C 45 41 53 54 4C 2F 61 6C 6C 6F 63 61 74 6F 72 2E 68 00) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBD _003F_003F_C_0040_02JDPG_0040rb_003F_0024AA_0040/* Not supported: data(72 62 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04OKGDLNCL_0040NONE_003F_0024AA_0040/* Not supported: data(4E 4F 4E 45 00) */;

	internal static _0024ArrayType_0024_0024_0024BY08_0024_0024CBD _003F_003F_C_0040_08CNPBDDPF_0040EALAYER3_003F_0024AA_0040/* Not supported: data(45 41 4C 41 59 45 52 33 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03EBCCGHHJ_0040XMA_003F_0024AA_0040/* Not supported: data(58 4D 41 00) */;

	internal static _0024ArrayType_0024_0024_0024BY03_0024_0024CBD _003F_003F_C_0040_03DAMMOOMO_0040XAS_003F_0024AA_0040/* Not supported: data(58 41 53 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04KLKHHPIG_0040MAIN_003F_0024AA_0040/* Not supported: data(4D 41 49 4E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09LEBBGDPH_0040OVERLAY_0_003F_0024AA_0040/* Not supported: data(4F 56 45 52 4C 41 59 5F 30 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06HKBMKICC_0040Amount_003F_0024AA_0040/* Not supported: data(41 6D 6F 75 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04KOACHJEN_0040Type_003F_0024AA_0040/* Not supported: data(54 79 70 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY00_0024_0024CBD _003F_003F_C_0040_00CNPNBAHC_0040_003F_0024AA_0040/* Not supported: data(00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040EHGLMLAN_0040SubtitleStringName_003F_0024AA_0040/* Not supported: data(53 75 62 74 69 74 6C 65 53 74 72 69 6E 67 4E 61 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040EGJLDMJL_0040IsStreamed_003F_0024AA_0040/* Not supported: data(49 73 53 74 72 65 61 6D 65 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04PBAHLACE_0040File_003F_0024AA_0040/* Not supported: data(46 69 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040POONCDNP_0040PCSampleRate_003F_0024AA_0040/* Not supported: data(50 43 53 61 6D 70 6C 65 52 61 74 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040KDIHJJEJ_0040PCCompression_003F_0024AA_0040/* Not supported: data(50 43 43 6F 6D 70 72 65 73 73 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBD _003F_003F_C_0040_02IDKLBHEO_004075_003F_0024AA_0040/* Not supported: data(37 35 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09BBODNONO_0040PCQuality_003F_0024AA_0040/* Not supported: data(50 43 51 75 61 6C 69 74 79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040PMIILAPL_0040IsStreamedOnPC_003F_0024AA_0040/* Not supported: data(49 73 53 74 72 65 61 6D 65 64 4F 6E 50 43 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040DGAALKOO_0040XenonSampleRate_003F_0024AA_0040/* Not supported: data(58 65 6E 6F 6E 53 61 6D 70 6C 65 52 61 74 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BB_0040_0024_0024CBD _003F_003F_C_0040_0BB_0040PCJBHEOK_0040XenonCompression_003F_0024AA_0040/* Not supported: data(58 65 6E 6F 6E 43 6F 6D 70 72 65 73 73 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0N_0040_0024_0024CBD _003F_003F_C_0040_0N_0040PGIBAOFB_0040XenonQuality_003F_0024AA_0040/* Not supported: data(58 65 6E 6F 6E 51 75 61 6C 69 74 79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BC_0040_0024_0024CBD _003F_003F_C_0040_0BC_0040HPOJCLKK_0040IsStreamedOnXenon_003F_0024AA_0040/* Not supported: data(49 73 53 74 72 65 61 6D 65 64 4F 6E 58 65 6E 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0O_0040_0024_0024CBD _003F_003F_C_0040_0O_0040JOKJDKDN_0040PS3SampleRate_003F_0024AA_0040/* Not supported: data(50 53 33 53 61 6D 70 6C 65 52 61 74 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0P_0040_0024_0024CBD _003F_003F_C_0040_0P_0040ONODFOAE_0040PS3Compression_003F_0024AA_0040/* Not supported: data(50 53 33 43 6F 6D 70 72 65 73 73 69 6F 6E 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040PKMEKCLG_0040PS3Quality_003F_0024AA_0040/* Not supported: data(50 53 33 51 75 61 6C 69 74 79 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BA_0040_0024_0024CBD _003F_003F_C_0040_0BA_0040MCNOGFGE_0040IsStreamedOnPS3_003F_0024AA_0040/* Not supported: data(49 73 53 74 72 65 61 6D 65 64 4F 6E 50 53 33 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09CGCMFBKE_0040GUIPreset_003F_0024AA_0040/* Not supported: data(47 55 49 50 72 65 73 65 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBD _003F_003F_C_0040_02EGCJHIOB_0040id_003F_0024AA_0040/* Not supported: data(69 64 00) */;

	internal static _0024ArrayType_0024_0024_0024BY09_0024_0024CBD _003F_003F_C_0040_09FBMAGD_0040Alignment_003F_0024AA_0040/* Not supported: data(41 6C 69 67 6E 6D 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06IBDJIBGI_0040Filter_003F_0024AA_0040/* Not supported: data(46 69 6C 74 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04OKOFBOPB_0040Time_003F_0024AA_0040/* Not supported: data(54 69 6D 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05_0024_0024CBD _003F_003F_C_0040_05KOHFLOFG_0040Level_003F_0024AA_0040/* Not supported: data(4C 65 76 65 6C 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040PAIKECLF_0040PathfinderTrackHeader_003F_0024AA_0040/* Not supported: data(50 61 74 68 66 69 6E 64 65 72 54 72 61 63 6B 48 65 61 64 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY06_0024_0024CBD _003F_003F_C_0040_06BNNOMDLO_0040Handle_003F_0024AA_0040/* Not supported: data(48 61 6E 64 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BD_0040_0024_0024CBD _003F_003F_C_0040_0BD_0040KELLHDOG_0040XferAlternateEvent_003F_0024AA_0040/* Not supported: data(58 66 65 72 41 6C 74 65 72 6E 61 74 65 45 76 65 6E 74 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0L_0040_0024_0024CBD _003F_003F_C_0040_0L_0040IFDBOOL_0040IsXferable_003F_0024AA_0040/* Not supported: data(49 73 58 66 65 72 61 62 6C 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BG_0040_0024_0024CBD _003F_003F_C_0040_0BG_0040IKNBPEAO_0040PathfinderEventHeader_003F_0024AA_0040/* Not supported: data(50 61 74 68 66 69 6E 64 65 72 45 76 65 6E 74 48 65 61 64 65 72 00) */;

	internal static _0024ArrayType_0024_0024_0024BY07_0024_0024CBD _003F_003F_C_0040_07MAHLPAJA_0040_003F_0024CDdefine_003F_0024AA_0040/* Not supported: data(23 64 65 66 69 6E 65 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBD _003F_003F_C_0040_01CLKCMJKC_0040_003F5_003F_0024AA_0040/* Not supported: data(20 00) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBD _003F_003F_C_0040_02MDDDDAID_00400x_003F_0024AA_0040/* Not supported: data(30 78 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040MNCGACDM_0040PATH_TRACK__003F_0024AA_0040/* Not supported: data(50 41 54 48 5F 54 52 41 43 4B 5F 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0M_0040_0024_0024CBD _003F_003F_C_0040_0M_0040IHIOENAA_0040PATH_EVENT__003F_0024AA_0040/* Not supported: data(50 41 54 48 5F 45 56 45 4E 54 5F 00) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040 _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4PCAudioCompressionSetting_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4PCAudioCompressionSetting_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B/* Not supported: data(7C C0 0A 10 02 00 00 00 5C C0 0A 10 00 00 00 00 A4 C0 0A 10 01 00 00 00) */;

	internal static float rw_002Eaudio_002Ecore_002E_003FA0x65b208a5_002ECPU_CLOCK_MULTIPLIER/* Not supported: data(00 00 80 3F) */;

	internal static float rw_002Eaudio_002Ecore_002E_003FA0x65b208a5_002EDECIBEL_MIN/* Not supported: data(00 00 7A C4) */;

	internal static float rw_002Eaudio_002Ecore_002E_003FA0x65b208a5_002ECENTS_MIN/* Not supported: data(00 50 C3 C7) */;

	internal static float rw_002Eaudio_002Ecore_002E_003FA0x65b208a5_002EPITCHLINEARTOCENTSCONST/* Not supported: data(05 25 79 45) */;

	internal static float rw_002Eaudio_002Ecore_002E_003FA0x65b208a5_002EPI/* Not supported: data(DB 0F 49 40) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040 _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4PathMusicTrackType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4PathMusicTrackType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B/* Not supported: data(C8 C0 0A 10 00 00 00 00 D0 C0 0A 10 01 00 00 00) */;

	internal static uint _003F_003F_B_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_004051/* Not supported: data(00 00 00 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04Q6AXXZ _003F_003F_7ICoreAllocator_0040Allocator_0040EA_0040_00406B_0040/* Not supported: data(60 10 00 10 7A 96 00 10 7A 96 00 10 7A 96 00 10 C4 C6 0A 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040CoreAllocatorWrapper_0040AudioCompiler_0040BinaryAssetBuilder_0040_00408/* Not supported: data(68 91 0F 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 20 7E 0F 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_60 _003F_003F_R0_003FAVCoreAllocatorWrapper_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 43 6F 72 65 41 6C 6C 6F 63 61 74 6F 72 57 72 61 70 70 65 72 40 41 75 64 69 6F 43 6F 6D 70 69 6C 65 72 40 42 69 6E 61 72 79 41 73 73 65 74 42 75 69 6C 64 65 72 40 40 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4ICoreAllocator_0040Allocator_0040EA_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 20 91 0F 10 D8 7D 0F 10) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040 _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4XenonAudioCompressionSetting_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4XenonAudioCompressionSetting_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B/* Not supported: data(7C C0 0A 10 02 00 00 00 5C C0 0A 10 00 00 00 00 88 C0 0A 10 01 00 00 00) */;

	internal static _0024ArrayType_0024_0024_0024BY01_0024_0024CBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040 _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4PS3AudioCompressionSetting_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4PS3AudioCompressionSetting_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B/* Not supported: data(7C C0 0A 10 01 00 00 00 A4 C0 0A 10 00 00 00 00) */;

	internal static float DataInitializers_002E_003FA0x65b208a5_002ESECONDS_PER_LOGICFRAME_REAL/*Field data (rva=0x123088) could not be foundin any section!*/;

	internal static float DataInitializers_002E_003FA0x65b208a5_002EMSEC_PER_SECOND/* Not supported: data(00 00 7A 44) */;

	internal static float DataInitializers_002E_003FA0x65b208a5_002ERADS_PER_DEGREE/* Not supported: data(35 FA 8E 3C) */;

	internal static float DataInitializers_002E_003FA0x65b208a5_002ELOGICFRAMES_PER_SECOND/* Not supported: data(00 00 A0 40) */;

	internal static float DataInitializers_002E_003FA0x65b208a5_002ELOGICFRAMES_PER_SECONDS_REAL/* Not supported: data(00 00 A0 40) */;

	internal static float DataInitializers_002E_003FA0x65b208a5_002ELOGICFRAMES_PER_MSEC_REAL/* Not supported: data(0A D7 A3 3B) */;

	internal static float DataInitializers_002E_003FA0x65b208a5_002EPI/* Not supported: data(DB 0F 49 40) */;

	internal unsafe static delegate*<void> DataInitializers_002E_003FA0x65b208a5_002ESECONDS_PER_LOGICFRAME_REAL_0024initializer_0024/* Not supported: data(11 00 00 06) */;

	internal static basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E _003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) */;

	internal static basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E _003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04Q6AXXZ _003F_003F_7CoreAllocatorWrapper_0040AudioCompiler_0040BinaryAssetBuilder_0040_00406B_0040/* Not supported: data(30 1D 00 10 30 1C 00 10 00 1C 00 10 50 1C 00 10 C4 C6 0A 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3ICoreAllocator_0040Allocator_0040EA_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 E8 7D 0F 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2CoreAllocatorWrapper_0040AudioCompiler_0040BinaryAssetBuilder_0040_00408/* Not supported: data(3C 7E 0F 10 F0 7D 0F 10 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_34 _003F_003F_R0_003FAVICoreAllocator_0040Allocator_0040EA_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 49 43 6F 72 65 41 6C 6C 6F 63 61 74 6F 72 40 41 6C 6C 6F 63 61 74 6F 72 40 45 41 40 40 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040ICoreAllocator_0040Allocator_0040EA_0040_00408/* Not supported: data(20 91 0F 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 D8 7D 0F 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2ICoreAllocator_0040Allocator_0040EA_0040_00408/* Not supported: data(F0 7D 0F 10 00) */;

	internal static bool _003Fs_gaveMP3Warning_0040_003FCH_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA/* Not supported: data(00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3CoreAllocatorWrapper_0040AudioCompiler_0040BinaryAssetBuilder_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 30 7E 0F 10) */;

	internal static bool _003Fs_gaveMP3Warning_0040_003FBM_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA/* Not supported: data(00) */;

	internal static basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E _003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4CoreAllocatorWrapper_0040AudioCompiler_0040BinaryAssetBuilder_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 68 91 0F 10 20 7E 0F 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioEventLimitGroup_0040SageBinaryData_0040_00402PBDB/* Not supported: data(EC B8 0A 10) */;

	internal static uint _003FTypeHash_0040AudioFileRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(82 92 57 AF) */;

	internal static uint _003FTypeHash_0040Color4f_0040SageBinaryData_0040_00402IB/* Not supported: data(61 97 24 CD) */;

	internal static uint _003FTypeHash_0040LogicRandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(55 6E 41 51) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicTrack_0040SageBinaryData_0040_00402PBDB/* Not supported: data(80 B8 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_UnitsFarFromBase_0040SageBinaryData_0040_00402IB/* Not supported: data(A4 9D 2B D0) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_EvaEventPlayedRecently_0040SageBinaryData_0040_00402IB/* Not supported: data(26 4D CA 6F) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_PathMusicStateCurrentlyActive_0040SageBinaryData_0040_00402IB/* Not supported: data(34 C1 D7 C9) */;

	internal unsafe static sbyte* _003FTypeName_0040ObjectFilter_0040SageBinaryData_0040_00402PBDB/* Not supported: data(2C B9 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioVolumeSliderMultiplier_0040SageBinaryData_0040_00402PBDB/* Not supported: data(04 B9 0A 10) */;

	internal static uint _003FTypeHash_0040TimeRange_0040SageBinaryData_0040_00402IB/* Not supported: data(9F 6D 09 2F) */;

	internal unsafe static sbyte* _003FTypeName_0040ClientRandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(70 B9 0A 10) */;

	internal static uint _003FTypeHash_0040PathMusicTrack_0040SageBinaryData_0040_00402IB/* Not supported: data(33 CF 12 9F) */;

	internal static uint _003FTypeHash_0040DialogEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(F8 8B 1E D2) */;

	internal static uint _003FTypeHash_0040MultisoundSubsoundRef_0040SageBinaryData_0040_00402IB/* Not supported: data(57 45 4D 6B) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicMapRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(E0 B9 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_LocalPlayerKillRatio_0040SageBinaryData_0040_00402IB/* Not supported: data(ED 6F 67 C5) */;

	internal unsafe static sbyte* _003FTypeName_0040DialogEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(74 B8 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Color3f_0040SageBinaryData_0040_00402PBDB/* Not supported: data(D8 B9 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_SpecificTrackTypePlaying_0040SageBinaryData_0040_00402IB/* Not supported: data(56 BF 97 E5) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_AnyTrackPlaying_0040SageBinaryData_0040_00402PBDB/* Not supported: data(E0 B6 0A 10) */;

	internal static uint _003FTypeHash_0040MoneyTransaction_0040SageBinaryData_0040_00402IB/* Not supported: data(F5 0B 52 C2) */;

	internal static uint _003FTypeHash_0040AudioFileMP3Passthrough_0040SageBinaryData_0040_00402IB/* Not supported: data(8A 90 7B 92) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioFileMP3PassthroughRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(98 B3 0A 10) */;

	internal static uint _003FTypeHash_0040PathMusicGameDynamicState_0040SageBinaryData_0040_00402IB/* Not supported: data(A7 4D E2 E2) */;

	internal static uint _003FTypeHash_0040AudioEventLimitGroup_0040SageBinaryData_0040_00402IB/* Not supported: data(EA 4D 31 95) */;

	internal static uint _003FTypeHash_0040PathMusicEventSet_0040SageBinaryData_0040_00402IB/* Not supported: data(24 44 09 62) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_HitBySuperWeapon_0040SageBinaryData_0040_00402IB/* Not supported: data(46 C2 3C 24) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioFile_0040SageBinaryData_0040_00402PBDB/* Not supported: data(20 B9 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040Color4f_0040SageBinaryData_0040_00402PBDB/* Not supported: data(D0 B9 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_TimeFromStartOfLevel_0040SageBinaryData_0040_00402IB/* Not supported: data(6F 81 0B 45) */;

	internal static uint _003FTypeHash_0040MusicTrack_0040SageBinaryData_0040_00402IB/* Not supported: data(62 06 C5 07) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_EvaEventPlayedRecently_0040SageBinaryData_0040_00402PBDB/* Not supported: data(7C B6 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_PathMusicStateCurrentlyActive_0040SageBinaryData_0040_00402PBDB/* Not supported: data(50 B5 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_LocalPlayerIsObserver_0040SageBinaryData_0040_00402IB/* Not supported: data(E8 52 ED 6B) */;

	internal static uint _003FTypeHash_0040PathMusicMap_0040SageBinaryData_0040_00402IB/* Not supported: data(CB 4C 1B BA) */;

	internal static uint _003FTypeHash_0040MusicPalette_0040SageBinaryData_0040_00402IB/* Not supported: data(60 C1 6F D9) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_EngagementCount_0040SageBinaryData_0040_00402PBDB/* Not supported: data(24 B5 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_ScoredKillCount_0040SageBinaryData_0040_00402PBDB/* Not supported: data(20 B6 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_LocalPlayerIsObserver_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C8 B7 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_PathMusicStateLastActive_0040SageBinaryData_0040_00402PBDB/* Not supported: data(8C B5 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_HitBySuperWeapon_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C8 B4 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicEventRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(40 B4 0A 10) */;

	internal static uint _003FTypeHash_0040ObjectFilterAsset_0040SageBinaryData_0040_00402IB/* Not supported: data(5E 19 52 43) */;

	internal unsafe static sbyte* _003FTypeName_0040MP3DialogEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(78 B3 0A 10) */;

	internal static uint _003FTypeHash_0040RealRange_0040SageBinaryData_0040_00402IB/* Not supported: data(E0 E0 EA F4) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicTrack_0040SageBinaryData_0040_00402PBDB/* Not supported: data(68 B4 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_Not_0040SageBinaryData_0040_00402IB/* Not supported: data(8E 5A 05 33) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_SpecificTrackTypePlaying_0040SageBinaryData_0040_00402PBDB/* Not supported: data(0C B7 0A 10) */;

	internal static uint _003FTypeHash_0040PathMusicGameDynamicStateSet_0040SageBinaryData_0040_00402IB/* Not supported: data(37 9D 1D 51) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_ObjectsOfTypeExist_0040SageBinaryData_0040_00402IB/* Not supported: data(C1 84 64 A9) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_PathMusicStateLastActive_0040SageBinaryData_0040_00402IB/* Not supported: data(6F B6 0A 8C) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicPalette_0040SageBinaryData_0040_00402PBDB/* Not supported: data(88 B4 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicMap_0040SageBinaryData_0040_00402PBDB/* Not supported: data(78 B4 0A 10) */;

	internal static uint _003FTypeHash_0040RandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(5F B4 AC C7) */;

	internal static uint _003FTypeHash_0040PathMusicEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(EF CE D0 76) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_LocalPlayerKillRatio_0040SageBinaryData_0040_00402PBDB/* Not supported: data(F4 B4 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioFileMP3Passthrough_0040SageBinaryData_0040_00402PBDB/* Not supported: data(B8 B3 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MoneyTransaction_0040SageBinaryData_0040_00402PBDB/* Not supported: data(3C B9 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_ObjectsNearEvaEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(49 CD B9 74) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_Not_0040SageBinaryData_0040_00402PBDB/* Not supported: data(00 B6 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MP3AmbientStream_0040SageBinaryData_0040_00402PBDB/* Not supported: data(64 B3 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_Or_0040SageBinaryData_0040_00402IB/* Not supported: data(1D E9 E2 F2) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(8C B8 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040LogicRandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(88 B9 0A 10) */;

	internal static uint _003FTypeHash_0040MP3AmbientStream_0040SageBinaryData_0040_00402IB/* Not supported: data(CA 3D 53 1F) */;

	internal static uint _003FTypeHash_0040Multisound_0040SageBinaryData_0040_00402IB/* Not supported: data(89 5A 9C F7) */;

	internal unsafe static sbyte* _003FTypeName_0040ReplaceTexture_0040SageBinaryData_0040_00402PBDB/* Not supported: data(60 B9 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040BaseAudioEventInfo_0040SageBinaryData_0040_00402PBDB/* Not supported: data(D8 B8 0A 10) */;

	internal static uint _003FTypeHash_0040ReplaceTexture_0040SageBinaryData_0040_00402IB/* Not supported: data(00 49 32 8C) */;

	internal static uint _003FTypeHash_0040AudioEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(45 2E 0C 56) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptTrack_0040SageBinaryData_0040_00402PBDB/* Not supported: data(98 B4 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNuggetBase_0040SageBinaryData_0040_00402IB/* Not supported: data(FB A4 B7 EC) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_Or_0040SageBinaryData_0040_00402PBDB/* Not supported: data(E0 B5 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicGameDynamicTransition_0040SageBinaryData_0040_00402PBDB/* Not supported: data(D0 B3 0A 10) */;

	internal static uint _003FTypeHash_0040AudioFileMP3PassthroughRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(9D 07 8E 88) */;

	internal static uint _003FTypeHash_0040MP3MusicTrack_0040SageBinaryData_0040_00402IB/* Not supported: data(C5 AB 9F 7F) */;

	internal unsafe static sbyte* _003FTypeName_0040Multisound_0040SageBinaryData_0040_00402PBDB/* Not supported: data(58 B8 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicGameDynamicState_0040SageBinaryData_0040_00402PBDB/* Not supported: data(10 B4 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionExpensiveNuggetBase_0040SageBinaryData_0040_00402IB/* Not supported: data(CC 7C CD 86) */;

	internal static uint _003FTypeHash_0040AmbientStream_0040SageBinaryData_0040_00402IB/* Not supported: data(FB 24 16 6D) */;

	internal unsafe static sbyte* _003FTypeName_0040AmbientStream_0040SageBinaryData_0040_00402PBDB/* Not supported: data(64 B8 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040TimeRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C4 B9 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_TimeFromStartOfLevel_0040SageBinaryData_0040_00402PBDB/* Not supported: data(6C B7 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptTimeoutSpecifier_0040SageBinaryData_0040_00402IB/* Not supported: data(51 31 D8 95) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_And_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C0 B5 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040SoundOrEvaEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(50 B9 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040IntRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(AC B9 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040RandomVariable_0040SageBinaryData_0040_00402PBDB/* Not supported: data(9C B9 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicEventSet_0040SageBinaryData_0040_00402PBDB/* Not supported: data(2C B4 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MultisoundSubsoundRef_0040SageBinaryData_0040_00402PBDB/* Not supported: data(98 B8 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_ScoredKillCount_0040SageBinaryData_0040_00402IB/* Not supported: data(14 8D BE C2) */;

	internal static uint _003FTypeHash_0040ObjectFilter_0040SageBinaryData_0040_00402IB/* Not supported: data(EE 59 55 C0) */;

	internal static uint _003FTypeHash_0040BaseAudioEventInfo_0040SageBinaryData_0040_00402IB/* Not supported: data(02 DE A4 5F) */;

	internal static uint _003FTypeHash_0040ClientRandomVariable_0040SageBinaryData_0040_00402IB/* Not supported: data(26 E0 35 42) */;

	internal static uint _003FTypeHash_0040AudioVolumeSliderMultiplier_0040SageBinaryData_0040_00402IB/* Not supported: data(30 D8 99 57) */;

	internal static uint _003FTypeHash_0040AudioFileRefWithWeight_0040SageBinaryData_0040_00402IB/* Not supported: data(90 B8 25 43) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioFileRefWithWeight_0040SageBinaryData_0040_00402PBDB/* Not supported: data(B0 B8 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_ObjectsNearEvaEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(4C B6 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040BaseSingleSound_0040SageBinaryData_0040_00402PBDB/* Not supported: data(C8 B8 0A 10) */;

	internal static uint _003FTypeHash_0040PathMusicMapRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(9C 69 51 11) */;

	internal static uint _003FTypeHash_0040AudioFile_0040SageBinaryData_0040_00402IB/* Not supported: data(47 1E C8 53) */;

	internal static uint _003FTypeHash_0040PathMusicGameDynamicTransition_0040SageBinaryData_0040_00402IB/* Not supported: data(8E 72 F0 82) */;

	internal static uint _003FTypeHash_0040MP3DialogEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(F8 DB 50 78) */;

	internal static uint _003FTypeHash_0040SoundOrEvaEvent_0040SageBinaryData_0040_00402IB/* Not supported: data(DE 4F AE 40) */;

	internal unsafe static sbyte* _003FTypeName_0040RealRange_0040SageBinaryData_0040_00402PBDB/* Not supported: data(B8 B9 0A 10) */;

	internal static uint _003FTypeHash_0040PathMusicEventRuntime_0040SageBinaryData_0040_00402IB/* Not supported: data(D4 D9 49 29) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_And_0040SageBinaryData_0040_00402IB/* Not supported: data(3F D3 BA 1F) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptTimeoutSpecifier_0040SageBinaryData_0040_00402PBDB/* Not supported: data(AC B4 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicEvent_0040SageBinaryData_0040_00402PBDB/* Not supported: data(58 B4 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptTrack_0040SageBinaryData_0040_00402IB/* Not supported: data(64 2F DE BB) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_UnitsFarFromBase_0040SageBinaryData_0040_00402PBDB/* Not supported: data(9C B7 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_TrackPlayedCount_0040SageBinaryData_0040_00402PBDB/* Not supported: data(40 B7 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040PathMusicGameDynamicStateSet_0040SageBinaryData_0040_00402PBDB/* Not supported: data(F0 B3 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNugget_ObjectsOfTypeExist_0040SageBinaryData_0040_00402PBDB/* Not supported: data(B0 B6 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MP3MusicTrack_0040SageBinaryData_0040_00402PBDB/* Not supported: data(88 B3 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionNuggetBase_0040SageBinaryData_0040_00402PBDB/* Not supported: data(24 B8 0A 10) */;

	internal unsafe static sbyte* _003FTypeName_0040AudioFileRuntime_0040SageBinaryData_0040_00402PBDB/* Not supported: data(F4 B9 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_AnyTrackPlaying_0040SageBinaryData_0040_00402IB/* Not supported: data(2B 85 38 68) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_EngagementCount_0040SageBinaryData_0040_00402IB/* Not supported: data(12 C9 CB B5) */;

	internal unsafe static sbyte* _003FTypeName_0040MusicScriptConditionExpensiveNuggetBase_0040SageBinaryData_0040_00402PBDB/* Not supported: data(FC B7 0A 10) */;

	internal static uint _003FTypeHash_0040MusicScriptConditionNugget_TrackPlayedCount_0040SageBinaryData_0040_00402IB/* Not supported: data(90 CD 2A EF) */;

	internal unsafe static sbyte* _003FTypeName_0040ObjectFilterAsset_0040SageBinaryData_0040_00402PBDB/* Not supported: data(44 B8 0A 10) */;

	internal static uint _003FTypeHash_0040Color3f_0040SageBinaryData_0040_00402IB/* Not supported: data(16 5A D0 9B) */;

	internal static uint _003FTypeHash_0040IntRange_0040SageBinaryData_0040_00402IB/* Not supported: data(B5 BF 53 89) */;

	internal static uint _003FTypeHash_0040BaseSingleSound_0040SageBinaryData_0040_00402IB/* Not supported: data(69 1F D6 AF) */;

	internal static bool _003Fs_gaveMP3Warning_0040_003FM_0040_003F_003FProcessAudioFileInstance_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAMP_0024AAVAssetBuffer_0040Core_00404_0040P_0024AAVInstanceDeclaration_004064_0040_0040Z_00404_NA/* Not supported: data(00) */;

	internal unsafe static sbyte* std_002E_003FA0x65b208a5_002E_bad_alloc_Message/* Not supported: data(08 BA 0A 10) */;

	internal static uint EA_002EThread_002E_003FA0x65b208a5_002EkTimeoutNone/* Not supported: data(FF FF FF FF) */;

	public unsafe static int** __unep_0040_003FCharToLower_0040Utils_0040Marshaler_0040_0040_0024_0024FYADD_0040Z/* Not supported: data(10 12 00 10) */;

	public unsafe static int** __unep_0040_003F_003F_0024BookmarkCompare_0040U_003F_0024GenericBookmark_0040I_0040Tracker_0040Relo_0040_0040_0040Tracker_0040Relo_0040_0040_0024_0024FCA_NABU_003F_0024GenericBookmark_0040I_004001_00400_0040Z/* Not supported: data(F0 1F 00 10) */;

	internal unsafe static sbyte* std_002E_003FA0x0aaeb331_002E_bad_alloc_Message/* Not supported: data(08 BA 0A 10) */;

	internal static _0024ArrayType_0024_0024_0024BY02_0024_0024CBD _003F_003F_C_0040_02DKCKIIND_0040_003F_0024CFs_003F_0024AA_0040/* Not supported: data(25 73 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CB_0040_0024_0024CBD _003F_003F_C_0040_0CB_0040GANLGHBP_0040gwriteflush_003F5_003F9_003F5OUT_003F5OF_003F5DISK_003F5SPACE_003F6_0040/* Not supported: data(67 77 72 69 74 65 66 6C 75 73 68 20 2D 20 4F 55 54 20 4F 46 20 44 49 53 4B 20 53 50 41 43 45 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DE_0040_0024_0024CBD _003F_003F_C_0040_0DE_0040KIDLMJEG_0040galloc_003F5_003F9_003F5FAILED_003F4_003F5OUT_003F5OF_003F5MEMORY_003F4_003F5_0040/* Not supported: data(67 61 6C 6C 6F 63 20 2D 20 46 41 49 4C 45 44 2E 20 4F 55 54 20 4F 46 20 4D 45 4D 4F 52 59 2E 20 53 49 5A 45 20 52 45 51 55 45 53 54 45 44 20 25 6C 64 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04IJPGHCDI_0040GMEM_003F_0024AA_0040/* Not supported: data(47 4D 45 4D 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04JBNFAOHP_0040GEND_003F_0024AA_0040/* Not supported: data(47 45 4E 44 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CG_0040_0024_0024CBD _003F_003F_C_0040_0CG_0040NCGJECCC_0040gfree_003F5_003F9_003F5ATTEMPT_003F5TO_003F5FREE_003F5NULL_003F5POI_0040/* Not supported: data(67 66 72 65 65 20 2D 20 41 54 54 45 4D 50 54 20 54 4F 20 46 52 45 45 20 4E 55 4C 4C 20 50 4F 49 4E 54 45 52 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04IPEHMKME_0040GFRE_003F_0024AA_0040/* Not supported: data(47 46 52 45 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CG_0040_0024_0024CBD _003F_003F_C_0040_0CG_0040MJDOKNOD_0040gfree_003F5_003F9_003F5ATTEMPT_003F5TO_003F5FREE_003F5MEMORY_003F5T_0040/* Not supported: data(67 66 72 65 65 20 2D 20 41 54 54 45 4D 50 54 20 54 4F 20 46 52 45 45 20 4D 45 4D 4F 52 59 20 54 57 49 43 45 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DN_0040_0024_0024CBD _003F_003F_C_0040_0DN_0040BAJDGDMF_0040gfree_003F5_003F9_003F5SENTINAL_003F5CORRUPTED_003F_0024DL_003F5PROB_0040/* Not supported: data(67 66 72 65 65 20 2D 20 53 45 4E 54 49 4E 41 4C 20 43 4F 52 52 55 50 54 45 44 3B 20 50 52 4F 42 41 42 4C 45 20 4D 45 4D 4F 52 59 20 44 41 54 41 20 43 4F 52 52 55 50 54 49 4F 4E 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04ODOKNKHJ_0040con_003F3_003F_0024AA_0040/* Not supported: data(63 6F 6E 3A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DI_0040_0024_0024CBD _003F_003F_C_0040_0DI_0040CIMFANBI_0040gopen_003F5_003F9_003F5UNABLE_003F5TO_003F5ALLOCATE_003F5MEMOR_0040/* Not supported: data(67 6F 70 65 6E 20 2D 20 55 4E 41 42 4C 45 20 54 4F 20 41 4C 4C 4F 43 41 54 45 20 4D 45 4D 4F 52 59 20 46 4F 52 20 46 49 4C 45 20 48 41 4E 44 4C 45 20 28 25 73 29 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CC_0040_0024_0024CBD _003F_003F_C_0040_0CC_0040MBJJOILM_0040gopen_003F5_003F9_003F5SEEK_003F5ON_003F5OPEN_003F5FAILED_003F5_003F_0024CI_003F_0024CFs_003F_0024CJ_0040/* Not supported: data(67 6F 70 65 6E 20 2D 20 53 45 45 4B 20 4F 4E 20 4F 50 45 4E 20 46 41 49 4C 45 44 20 28 25 73 29 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DJ_0040_0024_0024CBD _003F_003F_C_0040_0DJ_0040HMENCKMP_0040gwopen_003F5_003F9_003F5ATTEMPT_003F5TO_003F5OPEN_003F5READONL_0040/* Not supported: data(67 77 6F 70 65 6E 20 2D 20 41 54 54 45 4D 50 54 20 54 4F 20 4F 50 45 4E 20 52 45 41 44 4F 4E 4C 59 20 46 49 4C 45 20 46 4F 52 20 57 52 49 54 49 4E 47 20 28 25 73 29 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DF_0040_0024_0024CBD _003F_003F_C_0040_0DF_0040ILMFDHPD_0040gwopen_003F5_003F9_003F5ATTEMPT_003F5TO_003F5OPEN_003F5DIRECTO_0040/* Not supported: data(67 77 6F 70 65 6E 20 2D 20 41 54 54 45 4D 50 54 20 54 4F 20 4F 50 45 4E 20 44 49 52 45 43 54 4F 52 59 20 46 4F 52 20 57 52 49 54 49 4E 47 20 28 25 73 29 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04CFCDPBIN_0040_003F4tm1_003F_0024AA_0040/* Not supported: data(2E 74 6D 31 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CN_0040_0024_0024CBD _003F_003F_C_0040_0CN_0040LDDEILAH_0040gwopen_003F5_003F9_003F5UNABLE_003F5TO_003F5OPEN_003F5FILE_003F5FOR_0040/* Not supported: data(67 77 6F 70 65 6E 20 2D 20 55 4E 41 42 4C 45 20 54 4F 20 4F 50 45 4E 20 46 49 4C 45 20 46 4F 52 20 57 52 49 54 45 20 28 25 73 29 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0DJ_0040_0024_0024CBD _003F_003F_C_0040_0DJ_0040FABBLEAH_0040gwopen_003F5_003F9_003F5UNABLE_003F5TO_003F5ALLOCATE_003F5MEMO_0040/* Not supported: data(67 77 6F 70 65 6E 20 2D 20 55 4E 41 42 4C 45 20 54 4F 20 41 4C 4C 4F 43 41 54 45 20 4D 45 4D 4F 52 59 20 46 4F 52 20 46 49 4C 45 20 48 41 4E 44 4C 45 20 28 25 73 29 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04OAOKCEO_0040_003F4tm2_003F_0024AA_0040/* Not supported: data(2E 74 6D 32 00) */;

	internal static _0024ArrayType_0024_0024_0024BY04_0024_0024CBD _003F_003F_C_0040_04BJBPLFHE_0040_003F4bak_003F_0024AA_0040/* Not supported: data(2E 62 61 6B 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CK_0040_0024_0024CBD _003F_003F_C_0040_0CK_0040BMDNBCPF_0040gread_003F5_003F9_003F5ATTEMPT_003F5TO_003F5READ_003F5PAST_003F5END_0040/* Not supported: data(67 72 65 61 64 20 2D 20 41 54 54 45 4D 50 54 20 54 4F 20 52 45 41 44 20 50 41 53 54 20 45 4E 44 20 4F 46 20 46 49 4C 45 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0CP_0040_0024_0024CBD _003F_003F_C_0040_0CP_0040HBDJGBPA_0040gwrite_003F5_003F9_003F5ATTEMPT_003F5TO_003F5WRITE_003F5TO_003F5A_003F5R_0040/* Not supported: data(67 77 72 69 74 65 20 2D 20 41 54 54 45 4D 50 54 20 54 4F 20 57 52 49 54 45 20 54 4F 20 41 20 52 45 41 44 20 4F 4E 4C 59 20 46 49 4C 45 0A 00) */;

	internal static _0024ArrayType_0024_0024_0024BY0BM_0040_0024_0024CBD _003F_003F_C_0040_0BM_0040OIPMCHLF_0040gwrite_003F5_003F9_003F5OUT_003F5OF_003F5DISK_003F5SPACE_003F6_003F_0024AA_0040/* Not supported: data(67 77 72 69 74 65 20 2D 20 4F 55 54 20 4F 46 20 44 49 53 4B 20 53 50 41 43 45 0A 00) */;

	internal static int galloccount/*Field data (rva=0x123350) could not be foundin any section!*/;

	internal static int gopenmethod/* Not supported: data(01 00 00 00) */;

	internal unsafe static delegate* unmanaged[Cdecl, Cdecl]<sbyte*, int> gprintf/* Not supported: data(20 69 00 10) */;

	internal unsafe static sbyte* std_002E_003FA0xc5a70bfa_002E_bad_alloc_Message/* Not supported: data(08 BA 0A 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3IList_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 0C 7F 0F 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2INode_0040Marshaler_0040_00408/* Not supported: data(CC 7E 0F 10 00) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4Value_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 A4 92 0F 10 44 7F 0F 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2Value_0040_00408/* Not supported: data(60 7F 0F 10 84 7E 0F 10 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05Q6AXXZ _003F_003F_7List_0040_00406B_0040/* Not supported: data(D0 7B 00 10 80 7B 00 10 F0 79 00 10 40 71 00 10 30 84 00 10 C4 C6 0A 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_12 _003F_003F_R0_003FAVValue_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 56 61 6C 75 65 40 40 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040Node_0040_00408/* Not supported: data(D4 92 0F 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 90 7F 0F 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2List_0040_00408/* Not supported: data(F8 7F 0F 10 14 7F 0F 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3INode_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 C4 7E 0F 10) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7Value_0040_00406B_0040/* Not supported: data(60 71 00 10 90 71 00 10 20 7D 00 10 C4 C6 0A 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040IList_0040Marshaler_0040_00408/* Not supported: data(60 92 0F 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 FC 7E 0F 10) */;

	internal static _0024ArrayType_0024_0024_0024BY06Q6AXXZ _003F_003F_7Node_0040_00406B_0040/* Not supported: data(10 71 00 10 50 80 00 10 C0 77 00 10 C0 81 00 10 E0 82 00 10 70 7E 00 10 C4 C6 0A 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4IList_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 60 92 0F 10 FC 7E 0F 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3IValue_0040Marshaler_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 01 00 00 00 7C 7E 0F 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_23 _003F_003F_R0_003FAVIValue_0040Marshaler_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 49 56 61 6C 75 65 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3List_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 EC 7F 0F 10) */;

	internal static _0024ArrayType_0024_0024_0024BY06Q6AXXZ _003F_003F_7INode_0040Marshaler_0040_00406B_0040/* Not supported: data(7A 96 00 10 7A 96 00 10 7A 96 00 10 7A 96 00 10 7A 96 00 10 70 70 00 10 C4 C6 0A 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4IValue_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 E4 91 0F 10 6C 7E 0F 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040Value_0040_00408/* Not supported: data(A4 92 0F 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 44 7F 0F 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4Node_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 D4 92 0F 10 90 7F 0F 10) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3Value_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 54 7F 0F 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_11 _003F_003F_R0_003FAVNode_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 4E 6F 64 65 40 40 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_22 _003F_003F_R0_003FAVINode_0040Marshaler_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 49 4E 6F 64 65 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_22 _003F_003F_R0_003FAVIList_0040Marshaler_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 49 4C 69 73 74 40 4D 61 72 73 68 61 6C 65 72 40 40 00) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2IList_0040Marshaler_0040_00408/* Not supported: data(14 7F 0F 10 00) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_4 _003F_003F_R2IValue_0040Marshaler_0040_00408/* Not supported: data(84 7E 0F 10 00) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040List_0040_00408/* Not supported: data(04 93 0F 10 01 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 DC 7F 0F 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040IValue_0040Marshaler_0040_00408/* Not supported: data(E4 91 0F 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 6C 7E 0F 10) */;

	internal static _0024ArrayType_0024_0024_0024BY03Q6AXXZ _003F_003F_7IValue_0040Marshaler_0040_00406B_0040/* Not supported: data(7A 96 00 10 7A 96 00 10 00 70 00 10 C4 C6 0A 10) */;

	internal static _0024_s__RTTIBaseClassArray_0024_extraBytes_8 _003F_003F_R2Node_0040_00408/* Not supported: data(AC 7F 0F 10 CC 7E 0F 10 00) */;

	internal static _s__RTTIClassHierarchyDescriptor _003F_003F_R3Node_0040_00408/* Not supported: data(00 00 00 00 00 00 00 00 02 00 00 00 A0 7F 0F 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4INode_0040Marshaler_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 24 92 0F 10 B4 7E 0F 10) */;

	internal static _s__RTTICompleteObjectLocator _003F_003F_R4List_0040_00406B_0040/* Not supported: data(00 00 00 00 00 00 00 00 00 00 00 00 04 93 0F 10 DC 7F 0F 10) */;

	internal unsafe static sbyte* std_002E_003FA0x9b63700f_002E_bad_alloc_Message/* Not supported: data(08 BA 0A 10) */;

	internal static _s__RTTIBaseClassDescriptor2 _003F_003F_R1A_0040_003F0A_0040EA_0040INode_0040Marshaler_0040_00408/* Not supported: data(24 92 0F 10 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 40 00 00 00 B4 7E 0F 10) */;

	internal static _0024_TypeDescriptor_0024_extraBytes_11 _003F_003F_R0_003FAVList_0040_0040_00408/* Not supported: data(C4 C6 0A 10 00 00 00 00 2E 3F 41 56 4C 69 73 74 40 40 00) */;

	internal static _0024ArrayType_0024_0024_0024BY05Q6AXXZ _003F_003F_7IList_0040Marshaler_0040_00406B_0040/* Not supported: data(7A 96 00 10 7A 96 00 10 7A 96 00 10 7A 96 00 10 E0 70 00 10 C4 C6 0A 10) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_mp_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static int _003FUninitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FUninitialized_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(8F 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xi_vt_a/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedPerAppDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedPerAppDomain_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(94 00 00 06) */;

	[FixedAddressValueType]
	internal static bool _003FIsDefaultDomain_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2_NA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FIsDefaultDomain_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(90 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_ma_a/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedNative_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedNative_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(92 00 00 06) */;

	[FixedAddressValueType]
	internal static int _003FInitialized_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2HA;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitialized_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(8E 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_ma_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedVtables_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedVtables_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(91 00 00 06) */;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xi_vt_z/* Not supported: data(00 00 00 00) */;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Progress.State _003FInitializedPerProcess_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2W4State_0040Progress_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x176e81b8_002E_003FInitializedPerProcess_0024initializer_0024_0040CurrentDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q2P6MXXZA/* Not supported: data(93 00 00 06) */;

	internal static bool _003FInitializedPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x126de7) could not be foundin any section!*/;

	internal static bool _003FEntered_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x126de4) could not be foundin any section!*/;

	internal static bool _003FInitializedNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x126de5) could not be foundin any section!*/;

	internal static int _003FCount_0040AllDomains_0040_003CCrtImplementationDetails_003E_0040_00402HA/*Field data (rva=0x126de0) could not be foundin any section!*/;

	internal static _003CCrtImplementationDetails_003E.TriBool.State _003FhasNative_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A/* Not supported: data() */;

	internal static _003CCrtImplementationDetails_003E.TriBool.State _003FhasPerProcess_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00400W4State_0040TriBool_00402_0040A/* Not supported: data() */;

	internal static bool _003FInitializedNativeFromCCTOR_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_00402_NA/*Field data (rva=0x126de6) could not be foundin any section!*/;

	internal static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY00Q6MPBXXZ _003FA0x176e81b8_002E__xc_mp_a/* Not supported: data(00 00 00 00) */;

	public unsafe static int** __unep_0040_003FDoNothing_0040DefaultDomain_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z/* Not supported: data(35 A0 00 10) */;

	public unsafe static int** __unep_0040_003F_UninitializeDefaultDomain_0040LanguageSupport_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024FCGJPAX_0040Z/* Not supported: data(B1 A1 00 10) */;

	[FixedAddressValueType]
	internal static uint __exit_list_size_app_domain;

	[FixedAddressValueType]
	internal static _003CCrtImplementationDetails_003E.Handle_003CSystem_003A_003AObject_0020_005E_003E _003F_lock_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0V_003F_0024Handle_0040P_0024AAVObject_0040System_0040_0040_00402_0040A;

	internal unsafe static delegate*<void> _003FA0x23130d9f_002E_003F_lock_0024initializer_0024_0040AtExitLock_0040_003CCrtImplementationDetails_003E_0040_0040_0024_0024Q0P6MXXZA/* Not supported: data(AD 00 00 06) */;

	[FixedAddressValueType]
	internal unsafe static delegate*<void>* __onexitbegin_app_domain;

	internal static uint _003FA0x23130d9f_002E__exit_list_size/*Field data (rva=0x126ee4) could not be foundin any section!*/;

	[FixedAddressValueType]
	internal unsafe static delegate*<void>* __onexitend_app_domain;

	internal unsafe static delegate*<void>* _003FA0x23130d9f_002E__onexitbegin_m/*Field data (rva=0x126edc) could not be foundin any section!*/;

	internal unsafe static delegate*<void>* _003FA0x23130d9f_002E__onexitend_m/*Field data (rva=0x126ee0) could not be foundin any section!*/;

	public static uint eastl_002EgEmptyString/*Field data (rva=0x1269d8) could not be foundin any section!*/;

	public static _0024ArrayType_0024_0024_0024BY01Q6AXXZ _003F_003F_7type_info_0040_00406B_0040/* Not supported: data(30 96 00 10 00 00 00 00) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AXXZ __xc_z/* Not supported: data(00) */;

	public static volatile uint __native_vcclrit_reason/* Not supported: data(FF FF FF FF) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AXXZ __xc_a/* Not supported: data(00) */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AHXZ __xi_a/* Not supported: data(00) */;

	public static volatile global::__enative_startup_state __native_startup_state/* Not supported: data() */;

	public static _003CCppImplementationDetails_003E._0024ArrayType_0024_0024_0024BY0A_0040P6AHXZ __xi_z/* Not supported: data(00) */;

	public unsafe static void* __native_startup_lock/*Field data (rva=0x12f024) could not be foundin any section!*/;

	public static volatile uint __native_dllmain_reason/* Not supported: data(FF FF FF FF) */;

	internal unsafe static void EA_002EAllocator_002EICoreAllocator_002E_007Bdtor_007D(ICoreAllocator* P_0)
	{
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7ICoreAllocator_0040Allocator_0040EA_0040_00406B_0040));
	}

	internal unsafe static void* EA_002EAllocator_002EICoreAllocator_002E__vecDelDtor(ICoreAllocator* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			ICoreAllocator* ptr = (ICoreAllocator*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 4u, *(int*)ptr, (delegate*<void*, void>)(delegate*<ICoreAllocator*, void>)(&EA_002EAllocator_002EICoreAllocator_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete(ptr);
			}
			return ptr;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7ICoreAllocator_0040Allocator_0040EA_0040_00406B_0040));
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static void delete(void* P_0, void* P_1)
	{
	}

	internal unsafe static void Marshaler_002ENodeHolder_002E_007Bdtor_007D(NodeHolder* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(int)(*(uint*)(int)num)))((IntPtr)(int)num);
		}
	}

	internal unsafe static uint FastHashInternal_002EFastHashFn(uint hash, sbyte* data, uint len)
	{
		if (len != 0 && data != null)
		{
			uint num = len & 3;
			len >>= 2;
			if (len != 0)
			{
				do
				{
					hash = *(ushort*)data + hash;
					hash ^= (((ushort*)data)[1] ^ (hash << 5)) << 11;
					data += 4;
					hash = (hash >> 11) + hash;
					len--;
				}
				while (len != 0);
			}
			switch (num)
			{
			case 3u:
				hash = *(ushort*)data + hash;
				hash ^= ((uint)(data[2] << 2) ^ hash) << 16;
				hash = (hash >> 11) + hash;
				break;
			case 2u:
			{
				hash = *(ushort*)data + hash;
				uint num3 = hash;
				hash = num3 ^ (num3 << 11);
				hash = (hash >> 17) + hash;
				break;
			}
			case 1u:
			{
				hash = (uint)(*data) + hash;
				uint num2 = hash;
				hash = num2 ^ (num2 << 10);
				hash = (hash >> 1) + hash;
				break;
			}
			}
			uint num4 = hash;
			hash = num4 ^ (num4 << 3);
			hash = (hash >> 5) + hash;
			uint num5 = hash;
			hash = num5 ^ (num5 << 2);
			hash = (hash >> 15) + hash;
			return (hash << 10) ^ hash;
		}
		return 0u;
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

	internal unsafe static void Relo_002EChunk_002E_007Bdtor_007D(Chunk* P_0)
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
	internal unsafe static bool Relo_002EChunk_002EAllocate(Chunk* P_0, uint instanceBufferSize, uint relocationBufferSize, uint importsBufferSize)
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
	internal unsafe static bool Relo_002ETracker_002EMakeRelocatable(Tracker* P_0, Chunk* chunk, EndianHelper* endian, uint absoluteInstanceOffset)
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

	internal static sbyte Marshaler_002EUtils_002ECharToLower(sbyte src)
	{
		return (sbyte)tolower(src);
	}

	internal static void _003FA0x65b208a5_002E_003F_003F__E_003FA0x65b208a5_0040SECONDS_PER_LOGICFRAME_REAL_0040DataInitializers_0040_0040YMXXZ()
	{
		DataInitializers_002E_003FA0x65b208a5_002ESECONDS_PER_LOGICFRAME_REAL = (float)(1.0 / (double)DataInitializers_002E_003FA0x65b208a5_002ELOGICFRAMES_PER_SECONDS_REAL);
	}

	internal unsafe static void Marshaler_002EEndianCorrect(AudioFileRuntime* @object, EndianHelper* state)
	{
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(byte*)(&num) = ((byte*)@object + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((byte*)@object + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((byte*)@object + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = ((sbyte*)@object)[4];
			((int*)@object)[1] = (int)num;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num2);
			*(byte*)(&num2) = ((byte*)@object + 12)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1)) = ((byte*)@object + 12)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2)) = ((byte*)@object + 12)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3)) = ((sbyte*)@object)[12];
			((int*)@object)[3] = (int)num2;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(byte*)(&num3) = ((byte*)@object + 16)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((byte*)@object + 16)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((byte*)@object + 16)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = ((sbyte*)@object)[16];
			((int*)@object)[4] = (int)num3;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
			*(byte*)(&num4) = ((byte*)@object + 24)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = ((byte*)@object + 24)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2)) = ((byte*)@object + 24)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3)) = ((sbyte*)@object)[24];
			((int*)@object)[6] = (int)num4;
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AudioFileMP3Passthrough* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04PBAHLACE_0040File_003F_0024AA_0040), null);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), (String_003Cchar_003E*)((byte*)@object + 4), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AString_003Cchar_003E_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BD_0040EHGLMLAN_0040SubtitleStringName_003F_0024AA_0040), null), (String_003Cchar_003E**)((byte*)@object + 12), state);
			Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040EGJLDMJL_0040IsStreamed_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040)), (bool*)@object + 16, state);
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, AudioFile* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04PBAHLACE_0040File_003F_0024AA_0040), null);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), (String_003Cchar_003E*)((byte*)@object + 4), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			Marshaler_002EMarshal_003Cint_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0N_0040POONCDNP_0040PCSampleRate_003F_0024AA_0040), null), (int**)((byte*)@object + 12), state);
			Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003APCAudioCompressionSetting_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0O_0040KDIHJJEJ_0040PCCompression_003F_0024AA_0040), null), (PCAudioCompressionSetting**)((byte*)@object + 16), state);
			IValue* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09BBODNONO_0040PCQuality_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_02IDKLBHEO_004075_003F_0024AA_0040));
			if (ptr2 != null)
			{
				sbyte* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr2)))((nint)ptr2);
				((int*)@object)[5] = strtol(ptr3, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr2 + 4)))((nint)ptr2);
			}
			Marshaler_002EMarshal_003Cbool_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0P_0040PMIILAPL_0040IsStreamedOnPC_003F_0024AA_0040), null), (bool**)((byte*)@object + 24), state);
			Marshaler_002EMarshal_003Cint_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BA_0040DGAALKOO_0040XenonSampleRate_003F_0024AA_0040), null), (int**)((byte*)@object + 28), state);
			Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AXenonAudioCompressionSetting_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BB_0040PCJBHEOK_0040XenonCompression_003F_0024AA_0040), null), (XenonAudioCompressionSetting**)((byte*)@object + 32), state);
			IValue* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0N_0040PGIBAOFB_0040XenonQuality_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_02IDKLBHEO_004075_003F_0024AA_0040));
			if (ptr4 != null)
			{
				sbyte* ptr5 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr4)))((nint)ptr4);
				((int*)@object)[9] = strtol(ptr5, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr4 + 4)))((nint)ptr4);
			}
			Marshaler_002EMarshal_003Cbool_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BC_0040HPOJCLKK_0040IsStreamedOnXenon_003F_0024AA_0040), null), (bool**)((byte*)@object + 40), state);
			Marshaler_002EMarshal_003Cint_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0O_0040JOKJDKDN_0040PS3SampleRate_003F_0024AA_0040), null), (int**)((byte*)@object + 44), state);
			Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003APS3AudioCompressionSetting_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0P_0040ONODFOAE_0040PS3Compression_003F_0024AA_0040), null), (PS3AudioCompressionSetting**)((byte*)@object + 48), state);
			IValue* ptr6 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040PKMEKCLG_0040PS3Quality_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_02IDKLBHEO_004075_003F_0024AA_0040));
			if (ptr6 != null)
			{
				sbyte* ptr7 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr6)))((nint)ptr6);
				((int*)@object)[13] = strtol(ptr7, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr6 + 4)))((nint)ptr6);
			}
			Marshaler_002EMarshal_003Cbool_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BA_0040MCNOGFGE_0040IsStreamedOnPS3_003F_0024AA_0040), null), (bool**)((byte*)@object + 56), state);
			Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AString_003Cchar_003E_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BD_0040EHGLMLAN_0040SubtitleStringName_003F_0024AA_0040), null), (String_003Cchar_003E**)((byte*)@object + 60), state);
			Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AString_003Cchar_003E_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_09CGCMFBKE_0040GUIPreset_003F_0024AA_0040), null), (String_003Cchar_003E**)((byte*)@object + 64), state);
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, PathMusicTrack* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04PBAHLACE_0040File_003F_0024AA_0040), null);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), (String_003Cchar_003E*)((byte*)@object + 4), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003APathMusicTrackType_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04KOACHJEN_0040Type_003F_0024AA_0040), null), (PathMusicTrackType*)@object + 3, state);
			IValue* ptr2 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BG_0040PAIKECLF_0040PathfinderTrackHeader_003F_0024AA_0040), null);
			if (ptr2 != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr2)))((nint)ptr2), (String_003Cchar_003E*)((byte*)@object + 16), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr2 + 4)))((nint)ptr2);
			}
			IValue* ptr3 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_06BNNOMDLO_0040Handle_003F_0024AA_0040), null);
			if (ptr3 != null)
			{
				sbyte* ptr4 = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr3)))((nint)ptr3);
				((int*)@object)[6] = strtol(ptr4, null, 0);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr3 + 4)))((nint)ptr3);
			}
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(PathMusicTrack* @object, EndianHelper* state)
	{
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(byte*)(&num) = ((byte*)@object + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((byte*)@object + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((byte*)@object + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = ((sbyte*)@object)[4];
			((int*)@object)[1] = (int)num;
		}
		int num2 = ((int*)@object)[3];
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
			*(sbyte*)(&num3) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 3));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 2));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = System.Runtime.CompilerServices.Unsafe.As<int, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num2, 1));
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)(&num2);
			num2 = (int)num3;
		}
		((int*)@object)[3] = num2;
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num4);
			*(byte*)(&num4) = ((byte*)@object + 16)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 1)) = ((byte*)@object + 16)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 2)) = ((byte*)@object + 16)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num4, 3)) = ((sbyte*)@object)[16];
			((int*)@object)[4] = (int)num4;
		}
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num5);
			*(byte*)(&num5) = ((byte*)@object + 24)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 1)) = ((byte*)@object + 24)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 2)) = ((byte*)@object + 24)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num5, 3)) = ((sbyte*)@object)[24];
			((int*)@object)[6] = (int)num5;
		}
	}

	internal unsafe static void Marshaler_002EEndianCorrect(PathMusicEventRuntime* @object, EndianHelper* state)
	{
		if (*(bool*)state)
		{
			System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
			*(byte*)(&num) = ((byte*)@object + 4)[3];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 1)) = ((byte*)@object + 4)[2];
			System.Runtime.CompilerServices.Unsafe.As<uint, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 2)) = ((byte*)@object + 4)[1];
			System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num, 3)) = ((sbyte*)@object)[4];
			((int*)@object)[1] = (int)num;
		}
		uint num2 = ((uint*)@object)[2];
		if (num2 != 0)
		{
			TypedAssetId_003CSageBinaryData_003A_003APathMusicEvent_003E* ptr = (TypedAssetId_003CSageBinaryData_003A_003APathMusicEvent_003E*)(int)num2;
			if (*(bool*)state)
			{
				System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num3);
				*(sbyte*)(&num3) = ((sbyte*)ptr)[3];
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 1)) = ((sbyte*)ptr)[2];
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 2)) = ((sbyte*)ptr)[1];
				System.Runtime.CompilerServices.Unsafe.As<uint, sbyte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref num3, 3)) = *(sbyte*)ptr;
				*(uint*)ptr = num3;
			}
		}
	}

	internal unsafe static void Marshaler_002EMarshal(INode* node, PathMusicEvent* @object, Tracker* state)
	{
		if (node != null)
		{
			IValue* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BG_0040IKNBPEAO_0040PathfinderEventHeader_003F_0024AA_0040), null);
			if (ptr != null)
			{
				Marshaler_002EMarshal_003Cchar_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)ptr)))((nint)ptr), (String_003Cchar_003E*)((byte*)@object + 4), state);
				((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)ptr + 4)))((nint)ptr);
			}
			Marshaler_002EMarshal_003Cstruct_0020TypedAssetId_003Cstruct_0020SageBinaryData_003A_003APathMusicEvent_003E_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0BD_0040KELLHDOG_0040XferAlternateEvent_003F_0024AA_0040), null), (TypedAssetId_003CSageBinaryData_003A_003APathMusicEvent_003E**)((byte*)@object + 12), state);
			Marshaler_002EMarshal_003Cbool_002Cclass_0020Relo_003A_003ATracker_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*, sbyte*, IValue*>)(int)(*(uint*)(*(int*)node + 12)))((nint)node, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0L_0040IFDBOOL_0040IsXferable_003F_0024AA_0040), (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04LOAJBDKD_0040true_003F_0024AA_0040)), (bool*)@object + 16, state);
		}
	}

	internal unsafe static CoreAllocatorWrapper* BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002E_007Bctor_007D(CoreAllocatorWrapper* P_0)
	{
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7ICoreAllocator_0040Allocator_0040EA_0040_00406B_0040));
		try
		{
			System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7CoreAllocatorWrapper_0040AudioCompiler_0040BinaryAssetBuilder_0040_00406B_0040));
			CoreAllocatorWrapper* ptr = (CoreAllocatorWrapper*)((byte*)P_0 + 8);
			EA_002EAllocator_002EGeneralAllocator_002E_007Bctor_007D((GeneralAllocator*)ptr, null, 0u, true, false, null, null);
			try
			{
				EA_002EAllocator_002EGeneralAllocator_002EInit((GeneralAllocator*)ptr, null, 0u, true, false, null, null);
				return P_0;
			}
			catch
			{
				//try-fault
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<GeneralAllocator*, void>)(&EA_002EAllocator_002EGeneralAllocator_002E_007Bdtor_007D), (byte*)P_0 + 8);
				throw;
			}
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<ICoreAllocator*, void>)(&EA_002EAllocator_002EICoreAllocator_002E_007Bdtor_007D), P_0);
			throw;
		}
	}

	internal unsafe static void* BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002EAlloc(CoreAllocatorWrapper* P_0, uint size, sbyte* name, uint flags)
	{
		return EA_002EAllocator_002EGeneralAllocator_002EMalloc((GeneralAllocator*)((byte*)P_0 + 8), size, 0);
	}

	internal unsafe static void* BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002EAlloc(CoreAllocatorWrapper* P_0, uint size, sbyte* name, uint flags, uint align, uint alignOffset)
	{
		return EA_002EAllocator_002EGeneralAllocator_002EMallocAligned((GeneralAllocator*)((byte*)P_0 + 8), size, align, alignOffset, 0);
	}

	internal unsafe static void BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002EFree(CoreAllocatorWrapper* P_0, void* block, uint size)
	{
		EA_002EAllocator_002EGeneralAllocator_002EFree((GeneralAllocator*)((byte*)P_0 + 8), block);
	}

	internal unsafe static void* BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002E__vecDelDtor(CoreAllocatorWrapper* P_0, uint P_1)
	{
		if ((P_1 & 2) != 0)
		{
			CoreAllocatorWrapper* ptr = (CoreAllocatorWrapper*)((byte*)P_0 - 4);
			__ehvec_dtor(P_0, 1320u, *(int*)ptr, (delegate*<void*, void>)(delegate*<CoreAllocatorWrapper*, void>)(&BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002E_007Bdtor_007D));
			if ((P_1 & 1) != 0)
			{
				delete_005B_005D(ptr);
			}
			return ptr;
		}
		try
		{
			EA_002EAllocator_002EGeneralAllocator_002E_007Bdtor_007D((GeneralAllocator*)((byte*)P_0 + 8));
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<ICoreAllocator*, void>)(&EA_002EAllocator_002EICoreAllocator_002E_007Bdtor_007D), P_0);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7ICoreAllocator_0040Allocator_0040EA_0040_00406B_0040));
		if ((P_1 & 1) != 0)
		{
			delete(P_0);
		}
		return P_0;
	}

	internal unsafe static void BinaryAssetBuilder_002EAudioCompiler_002ECoreAllocatorWrapper_002E_007Bdtor_007D(CoreAllocatorWrapper* P_0)
	{
		try
		{
			EA_002EAllocator_002EGeneralAllocator_002E_007Bdtor_007D((GeneralAllocator*)((byte*)P_0 + 8));
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<ICoreAllocator*, void>)(&EA_002EAllocator_002EICoreAllocator_002E_007Bdtor_007D), P_0);
			throw;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(P_0, (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_7ICoreAllocator_0040Allocator_0040EA_0040_00406B_0040));
	}

	internal unsafe static AutoGSTREAMCloser* BinaryAssetBuilder_002EAudioCompiler_002EAutoGSTREAMCloser_002E_007Bctor_007D(AutoGSTREAMCloser* P_0, STDSTREAM* streamToClose)
	{
		*(int*)P_0 = (int)streamToClose;
		return P_0;
	}

	internal unsafe static void BinaryAssetBuilder_002EAudioCompiler_002EAutoGSTREAMCloser_002E_007Bdtor_007D(AutoGSTREAMCloser* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			gclose((STDSTREAM*)(int)num);
			*(int*)P_0 = 0;
		}
	}

	internal unsafe static AutoSINSTANCECloser* BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bctor_007D(AutoSINSTANCECloser* P_0, SINSTANCE* instanceToClose)
	{
		*(int*)P_0 = (int)instanceToClose;
		return P_0;
	}

	internal unsafe static void BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCECloser_002E_007Bdtor_007D(AutoSINSTANCECloser* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			SIMEX_close((SINSTANCE*)(int)num);
			*(int*)P_0 = 0;
		}
	}

	internal unsafe static AutoSINSTANCEWCloser* BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCEWCloser_002E_007Bctor_007D(AutoSINSTANCEWCloser* P_0, SINSTANCE* instanceToClose)
	{
		*(int*)P_0 = (int)instanceToClose;
		return P_0;
	}

	internal unsafe static void BinaryAssetBuilder_002EAudioCompiler_002EAutoSINSTANCEWCloser_002E_007Bdtor_007D(AutoSINSTANCEWCloser* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			SIMEX_wclose((SINSTANCE*)(int)num);
			*(int*)P_0 = 0;
		}
	}

	internal unsafe static AutoSINFOFreer* BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bctor_007D(AutoSINFOFreer* P_0, SINFO* infoToFree)
	{
		*(int*)P_0 = (int)infoToFree;
		return P_0;
	}

	internal unsafe static void BinaryAssetBuilder_002EAudioCompiler_002EAutoSINFOFreer_002E_007Bdtor_007D(AutoSINFOFreer* P_0)
	{
		uint num = *(uint*)P_0;
		if (num != 0)
		{
			SIMEX_freesinfo((SINFO*)(int)num);
			*(int*)P_0 = 0;
		}
	}

	internal unsafe static void _003F_003F__FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_0040YMXXZ()
	{
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 8)) - System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref _003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) > 1 && System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref _003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) != 0)
		{
			delete_005B_005D((void*)(int)System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, uint>(ref _003FhexNumberPrefixString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B));
		}
	}

	internal unsafe static void _003F_003F__FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_0040YMXXZ()
	{
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 8)) - System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref _003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) > 1 && System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref _003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) != 0)
		{
			delete_005B_005D((void*)(int)System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, uint>(ref _003FspaceString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B));
		}
	}

	internal unsafe static void _003F_003F__FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_0040YMXXZ()
	{
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref _003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B, 8)) - System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref _003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) > 1 && System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref _003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B) != 0)
		{
			delete_005B_005D((void*)(int)System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, uint>(ref _003FdefineString_0040_003F1_003F_003FParsePathMusicHeaderLine_0040Plugin_0040AudioCompiler_0040BinaryAssetBuilder_0040_0040Q_0024AAM_NPBD00PAH_0040Z_00404V_003F_0024basic_string_0040DVallocator_0040eastl_0040_0040_0040eastl_0040_0040B));
		}
	}

	internal unsafe static gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bctor_007D(gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* P_0, XPathNavigator t)
	{
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(t)).ToPointer();
		return P_0;
	}

	[DebuggerStepThrough]
	internal unsafe static void gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E_002E_007Bdtor_007D(gcroot_003CSystem_003A_003AXml_003A_003AXPath_003A_003AXPathNavigator_0020_005E_003E* P_0)
	{
		IntPtr intPtr = new IntPtr((void*)(int)(*(uint*)P_0));
		((GCHandle)intPtr).Free();
		*(int*)P_0 = 0;
	}

	internal unsafe static gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E_002E_007Bctor_007D(gcroot_003CSystem_003A_003AXml_003A_003AXmlNamespaceManager_0020_005E_003E* P_0, XmlNamespaceManager t)
	{
		*(int*)P_0 = (int)((IntPtr)GCHandle.Alloc(t)).ToPointer();
		return P_0;
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

	internal unsafe static void eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D(basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* P_0)
	{
		int num = *(int*)P_0;
		if (((int*)P_0)[2] - num > 1)
		{
			sbyte* ptr = (sbyte*)num;
			if (ptr != null)
			{
				delete_005B_005D(ptr);
			}
		}
	}

	internal unsafe static uint eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002Efind(basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* P_0, basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* x, uint position)
	{
		uint num = *(uint*)x;
		uint num2 = (uint)((int*)x)[1] - num;
		sbyte* ptr = (sbyte*)(int)num;
		uint num3 = ((uint*)P_0)[1];
		uint num4 = *(uint*)P_0;
		sbyte* ptr3;
		if (num2 + position <= num3 - num4)
		{
			sbyte* ptr2 = eastl_002Esearch_003Cchar_0020_002A_002Cchar_0020const_0020_002A_003E((sbyte*)(int)(num4 + position), (sbyte*)(int)num3, ptr, ptr + (int)num2);
			if (ptr2 != (sbyte*)(int)num3 || num2 == 0)
			{
				ptr3 = ptr2 - (int)num4;
				goto IL_0041;
			}
		}
		ptr3 = (sbyte*)(-1);
		goto IL_0041;
		IL_0041:
		return (uint)ptr3;
	}

	internal unsafe static basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002Esubstr(basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* P_0, basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* P_1, uint position, uint n)
	{
		uint num = 0u;
		uint num2 = *(uint*)P_0;
		uint num3 = (uint)(((int*)P_0)[1] - (int)num2) - position;
		sbyte* pEnd = (sbyte*)(int)((num3 >= n ? ref n : ref num3) + num2 + position);
		sbyte* pBegin = (sbyte*)(int)(num2 + position);
		*(int*)P_1 = 0;
		((int*)P_1)[1] = 0;
		((int*)P_1)[2] = 0;
		eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize(P_1, pBegin, pEnd);
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
				___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), P_1);
			}
			throw;
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
	internal unsafe static bool eastl_002Eoperator_003D_003D_003Cchar_002Cclass_0020eastl_003A_003Aallocator_003E(basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* a, basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* b)
	{
		uint num = *(uint*)a;
		uint num2 = (uint)((int*)a)[1] - num;
		uint num3 = *(uint*)b;
		if (num2 == (uint)(((int*)b)[1] - (int)num3))
		{
			uint num4 = num2;
			int num5 = (int)num3;
			int num6 = (int)num;
			if (num4 == 0)
			{
				goto IL_005a;
			}
			sbyte b2 = *(sbyte*)num6;
			sbyte b3 = *(sbyte*)num5;
			if (b2 >= b3)
			{
				int num7 = (int)(num - num3);
				while (b2 <= b3)
				{
					if (num4 != 1)
					{
						num4--;
						num5++;
						b2 = *(sbyte*)(num7 + num5);
						b3 = *(sbyte*)num5;
						if (b2 < b3)
						{
							break;
						}
						continue;
					}
					goto IL_005a;
				}
			}
		}
		int num8 = 0;
		goto IL_0062;
		IL_005a:
		num8 = 1;
		goto IL_0062;
		IL_0062:
		return (byte)num8 != 0;
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

	internal unsafe static sbyte* Relo_002ETracker_002EPush_003Cchar_003E(Tracker* P_0, sbyte** pointerLocation, uint objectCount)
	{
		return (sbyte*)Relo_002ETracker_002EPush(P_0, (void**)pointerLocation, 1u, objectCount);
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020Relo_003A_003AString_003Cchar_003E_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, String_003Cchar_003E** @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* text = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			Relo_002ETracker_002EPush(state, (void**)@object, 8u, 1u);
			Marshaler_002EMarshal_003Cchar_003E(text, (String_003Cchar_003E*)(int)(*(uint*)@object), state);
			((int*)((byte*)state + 4))[1] += -4;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cint_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, int** @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* ptr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			Relo_002ETracker_002EPush(state, (void**)@object, 4u, 1u);
			*(int*)(int)(*(uint*)@object) = strtol(ptr, null, 0);
			((int*)((byte*)state + 4))[1] += -4;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003APCAudioCompressionSetting_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, PCAudioCompressionSetting** @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* at = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			Relo_002ETracker_002EPush(state, (void**)@object, 4u, 1u);
			PCAudioCompressionSetting* ptr = (PCAudioCompressionSetting*)(int)(*(uint*)@object);
			EnumNameGetter_003Cint_003E.EnumPair* ptr2 = EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(at, (EnumNameGetter_003Cint_003E.EnumPair*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4PCAudioCompressionSetting_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4PCAudioCompressionSetting_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B), 3u);
			int num = ((ptr2 == null) ? 2 : ((int*)ptr2)[1]);
			*ptr = (PCAudioCompressionSetting)num;
			((int*)((byte*)state + 4))[1] += -4;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cbool_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, bool** @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* intPtr = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			Relo_002ETracker_002EPush(state, (void**)@object, 1u, 1u);
			bool* ptr = (bool*)(int)(*(uint*)@object);
			sbyte b = *intPtr;
			int num = ((b == 49 || b == 116 || b == 84) ? 1 : 0);
			*ptr = (byte)num != 0;
			((int*)((byte*)state + 4))[1] += -4;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003AXenonAudioCompressionSetting_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, XenonAudioCompressionSetting** @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* at = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			Relo_002ETracker_002EPush(state, (void**)@object, 4u, 1u);
			XenonAudioCompressionSetting* ptr = (XenonAudioCompressionSetting*)(int)(*(uint*)@object);
			EnumNameGetter_003Cint_003E.EnumPair* ptr2 = EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(at, (EnumNameGetter_003Cint_003E.EnumPair*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4XenonAudioCompressionSetting_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4XenonAudioCompressionSetting_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B), 3u);
			int num = ((ptr2 == null) ? 2 : ((int*)ptr2)[1]);
			*ptr = (XenonAudioCompressionSetting)num;
			((int*)((byte*)state + 4))[1] += -4;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003APS3AudioCompressionSetting_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, PS3AudioCompressionSetting** @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* at = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			Relo_002ETracker_002EPush(state, (void**)@object, 4u, 1u);
			PS3AudioCompressionSetting* ptr = (PS3AudioCompressionSetting*)(int)(*(uint*)@object);
			EnumNameGetter_003Cint_003E.EnumPair* ptr2 = EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(at, (EnumNameGetter_003Cint_003E.EnumPair*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4PS3AudioCompressionSetting_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4PS3AudioCompressionSetting_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B), 2u);
			int num = ((ptr2 == null) ? 1 : ((int*)ptr2)[1]);
			*ptr = (PS3AudioCompressionSetting)num;
			((int*)((byte*)state + 4))[1] += -4;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cenum_0020SageBinaryData_003A_003APathMusicTrackType_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, PathMusicTrackType* @object, Tracker* state)
	{
		if (value != null)
		{
			EnumNameGetter_003Cint_003E.EnumPair* ptr = EnumLookup_002EFindNVPair_003Cstruct_0020EnumNameGetter_003Cint_003E_003A_003AEnumPair_003E(((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value), (EnumNameGetter_003Cint_003E.EnumPair*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x65b208a5_002E_003FenumMap_0040_003F1_003F_003FGetValue_0040_003F_0024EnumNameGetter_0040W4PathMusicTrackType_0040SageBinaryData_0040_0040_0040_0040SA_003FBW4PathMusicTrackType_0040SageBinaryData_0040_0040PBDPA_N_0040Z_00404QBUEnumPair_0040_003F_0024EnumNameGetter_0040H_0040_0040B), 2u);
			int num = ((ptr != null) ? ((int*)ptr)[1] : 0);
			*@object = (PathMusicTrackType)num;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020TypedAssetId_003Cstruct_0020SageBinaryData_003A_003APathMusicEvent_003E_0020_002A_002Cclass_0020Relo_003A_003ATracker_003E(IValue* value, TypedAssetId_003CSageBinaryData_003A_003APathMusicEvent_003E** @object, Tracker* state)
	{
		if (value != null)
		{
			sbyte* text = ((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, sbyte*>)(int)(*(uint*)(int)(*(uint*)value)))((nint)value);
			Relo_002ETracker_002EPush(state, (void**)@object, 4u, 1u);
			Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003APathMusicEvent_003E(text, (TypedAssetId_003CSageBinaryData_003A_003APathMusicEvent_003E*)(int)(*(uint*)@object), state);
			((int*)((byte*)state + 4))[1] += -4;
			((delegate* unmanaged[Thiscall, Thiscall]<IntPtr, void>)(int)(*(uint*)(*(int*)value + 4)))((nint)value);
		}
	}

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicMap_003E(Tracker* P_0, PathMusicMap** rootPointer)
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicMapRuntime_003E(Tracker* P_0, PathMusicMapRuntime** rootPointer)
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicTrack_003E(Tracker* P_0, PathMusicTrack** rootPointer)
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicEvent_003E(Tracker* P_0, PathMusicEvent** rootPointer)
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
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 20u);
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003APathMusicEventRuntime_003E(Tracker* P_0, PathMusicEventRuntime** rootPointer)
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFileMP3Passthrough_003E(Tracker* P_0, AudioFileMP3Passthrough** rootPointer)
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
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 20u);
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFileMP3PassthroughRuntime_003E(Tracker* P_0, AudioFileMP3PassthroughRuntime** rootPointer)
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFile_003E(Tracker* P_0, AudioFile** rootPointer)
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
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 68u);
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

	internal unsafe static Tracker* Relo_002ETracker_002E_007Bctor_007D_003Cstruct_0020SageBinaryData_003A_003AAudioFileRuntime_003E(Tracker* P_0, AudioFileRuntime** rootPointer)
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
							uint num = Relo_002ETracker_002EAllocate(P_0, 1u, 32u);
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

	internal unsafe static void eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize(basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* P_0, sbyte* pBegin, sbyte* pEnd)
	{
		uint num = (uint)(pEnd - (nuint)pBegin);
		uint num2 = num + 1;
		basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E* ptr2;
		if (num2 > 1)
		{
			void* ptr = new_005B_005D(num2, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_05JKPKJCFD_0040EASTL_003F_0024AA_0040), 0, 0u, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0FB_0040ODGLLPF_0040C_003F3_003F2projects_003F2Ra3_003F2Production_003F2code_003F2_0040), 209);
			*(int*)P_0 = (int)ptr;
			ptr2 = (basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
			*(int*)ptr2 = (int)ptr;
			((int*)P_0)[2] = (int)((byte*)ptr + (int)num2);
		}
		else
		{
			*(int*)P_0 = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref eastl_002EgEmptyString);
			ptr2 = (basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*)((byte*)P_0 + 4);
			*(int*)ptr2 = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref eastl_002EgEmptyString);
			((int*)P_0)[2] = (int)System.Runtime.CompilerServices.Unsafe.AsPointer(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref eastl_002EgEmptyString, 1));
		}
		sbyte* ptr3 = (sbyte*)(int)(*(uint*)P_0);
		memmove(ptr3, pBegin, num);
		sbyte* ptr4 = pEnd + (nuint)(ptr3 - (nuint)pBegin);
		*(int*)ptr2 = (int)ptr4;
		*ptr4 = 0;
	}

	internal unsafe static sbyte* eastl_002Esearch_003Cchar_0020_002A_002Cchar_0020const_0020_002A_003E(sbyte* first1, sbyte* last1, sbyte* first2, sbyte* last2)
	{
		if (first2 != last2)
		{
			sbyte* ptr = first2 + 1;
			if (ptr == last2)
			{
				sbyte* ptr2 = first1;
				if (first1 != last1)
				{
					sbyte b = *first2;
					while (*ptr2 != b)
					{
						ptr2++;
						if (ptr2 == last1)
						{
							break;
						}
					}
				}
				return ptr2;
			}
			if (first1 != last1)
			{
				do
				{
					sbyte b2 = *first2;
					sbyte* ptr3;
					sbyte* ptr4;
					sbyte* ptr5;
					while (true)
					{
						if (*first1 != b2)
						{
							first1++;
							if (first1 != last1)
							{
								continue;
							}
						}
						else if (first1 != last1)
						{
							ptr3 = ptr;
							ptr4 = first1 + 1;
							ptr5 = ptr4;
							if (ptr5 != last1)
							{
								break;
							}
						}
						return last1;
					}
					if (*ptr5 == *ptr)
					{
						do
						{
							ptr3++;
							if (ptr3 != last2)
							{
								ptr5++;
								if (ptr5 == last1)
								{
									return last1;
								}
								continue;
							}
							return first1;
						}
						while (*ptr5 == *ptr3);
					}
					first1 = ptr4;
				}
				while (first1 != last1);
			}
		}
		return first1;
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

	internal unsafe static void Marshaler_002EMarshal_003Cstruct_0020SageBinaryData_003A_003APathMusicEvent_003E(sbyte* text, TypedAssetId_003CSageBinaryData_003A_003APathMusicEvent_003E* reference, Tracker* tracker)
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
		*(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) = 0;
		System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) = 0;
		sbyte* ptr = text;
		if (*text != 0)
		{
			do
			{
				ptr++;
			}
			while (*ptr != 0);
		}
		uint num = (uint)(ptr - (nuint)text);
		eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002ERangeInitialize(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, text, (int)num + text);
		try
		{
			delegate* unmanaged[Cdecl, Cdecl]<sbyte, sbyte> _unep_0040_003FCharToLower_0040Utils_0040Marshaler_0040_0040_0024_0024FYADD_0040Z = (delegate* unmanaged[Cdecl, Cdecl]<sbyte, sbyte>)__unep_0040_003FCharToLower_0040Utils_0040Marshaler_0040_0040_0024_0024FYADD_0040Z;
			sbyte* ptr2 = (sbyte*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E));
			if (*(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 4)))
			{
				do
				{
					sbyte* intPtr = ptr2;
					*intPtr = _unep_0040_003FCharToLower_0040Utils_0040Marshaler_0040_0040_0024_0024FYADD_0040Z(*intPtr);
					ptr2++;
				}
				while (ptr2 != (sbyte*)System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 4)));
			}
			uint num2 = (uint)(System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 4)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E));
			*(uint*)reference = FastHashInternal_002EFastHashFn(num2, (sbyte*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)), num2);
		}
		catch
		{
			//try-fault
			___CxxCallUnwindDtor((delegate*<void*, void>)(delegate*<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E*, void>)(&eastl_002Ebasic_string_003Cchar_002Ceastl_003A_003Aallocator_003E_002E_007Bdtor_007D), &basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E);
			throw;
		}
		if (System.Runtime.CompilerServices.Unsafe.As<basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E, 8)) - *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) > 1 && *(int*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E) != 0)
		{
			delete_005B_005D((void*)(int)(*(uint*)(&basic_string_003Cchar_002Ceastl_003A_003Aallocator_003E)));
		}
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

	internal unsafe static void* new_005B_005D(uint size, sbyte* pName, int flags, uint debugFlags, sbyte* file, int line)
	{
		return new_005B_005D(size);
	}

	internal unsafe static int _003FA0xc5a70bfa_002Egwriteflush(STDSTREAM* h)
	{
		int num = 0;
		if (((int*)h)[7] != 0 && ((int*)h)[6] != 0)
		{
			fseek((_iobuf*)(int)(*(uint*)h), ((int*)h)[3], 0);
			STDSTREAM* ptr = (STDSTREAM*)((nuint)((int*)h)[5] - (nuint)h);
			if ((nint)((byte*)ptr - 1060) > unchecked((nint)null))
			{
				num = (int)fwrite((byte*)h + 1060, 1u, (uint)((byte*)ptr - 1060), (_iobuf*)(int)(*(uint*)h));
			}
			if (num < (nint)((int*)h)[5] - (nint)h - 1060)
			{
				gprintf((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0CB_0040GANLGHBP_0040gwriteflush_003F5_003F9_003F5OUT_003F5OF_003F5DISK_003F5SPACE_003F6_0040));
			}
			((int*)h)[7] = 0;
		}
		STDSTREAM* ptr2 = (STDSTREAM*)((byte*)h + 1060);
		((int*)h)[5] = (int)ptr2;
		((int*)h)[4] = (int)ptr2;
		fflush((_iobuf*)(int)(*(uint*)h));
		return num;
	}

	internal unsafe static void _003FA0xc5a70bfa_002EGIMEX_tempname(sbyte* d, sbyte* s, sbyte* ext)
	{
		sbyte* ptr = s;
		int num = (int)(d - (nuint)s);
		sbyte b;
		do
		{
			b = (*(num + ptr) = *ptr);
			ptr++;
		}
		while (b != 0);
		sbyte* ptr2 = d;
		sbyte b2 = *d;
		if (b2 != 0)
		{
			do
			{
				ptr2++;
			}
			while (*ptr2 != 0);
		}
		uint num2 = (uint)(ptr2 - (nuint)d);
		sbyte* ptr3 = d + (int)num2 - 1;
		if (ptr3 <= d)
		{
			goto IL_006a;
		}
		while (true)
		{
			sbyte b3 = *ptr3;
			if (b3 == 46)
			{
				break;
			}
			if (b3 != 58 && b3 != 47 && b3 != 92)
			{
				ptr3--;
				if (ptr3 > d)
				{
					continue;
				}
			}
			goto IL_006a;
		}
		goto IL_0084;
		IL_0084:
		*ptr3 = 0;
		sbyte* ptr4 = ext;
		sbyte* ptr5 = d;
		if (*d != 0)
		{
			do
			{
				ptr5++;
			}
			while (*ptr5 != 0);
		}
		if ((*ptr5 = *ext) != 0)
		{
			do
			{
				ptr5++;
				ptr4++;
			}
			while ((*ptr5 = *ptr4) != 0);
		}
		return;
		IL_006a:
		if (*ptr3 != 46)
		{
			sbyte* ptr6 = d;
			if (b2 != 0)
			{
				do
				{
					ptr6++;
				}
				while (*ptr6 != 0);
			}
			ptr3 = ptr6 - (nuint)d + (nuint)d;
		}
		goto IL_0084;
	}

	internal unsafe static int gfree(void* memptr)
	{
		int result = 0;
		if ((nuint)((byte*)memptr + 2047) <= (nuint)4094u)
		{
			gprintf((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0CG_0040NCGJECCC_0040gfree_003F5_003F9_003F5ATTEMPT_003F5TO_003F5FREE_003F5NULL_003F5POI_0040));
			goto IL_0153;
		}
		int num = *((int*)memptr - 1);
		uint num2 = 4u;
		sbyte* ptr = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04IJPGHCDI_0040GMEM_003F_0024AA_0040);
		void* ptr2 = (byte*)memptr - 8;
		sbyte b = *(sbyte*)ptr2;
		sbyte b2 = 71;
		if (b >= 71)
		{
			while (b <= b2)
			{
				if (num2 != 1)
				{
					num2--;
					ptr2 = (byte*)ptr2 + 1;
					ptr++;
					b = *(sbyte*)ptr2;
					b2 = *ptr;
					if (b < b2)
					{
						break;
					}
					continue;
				}
				goto IL_007f;
			}
		}
		goto IL_00cd;
		IL_00cd:
		uint num3 = 4u;
		sbyte* ptr3 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04IPEHMKME_0040GFRE_003F_0024AA_0040);
		void* ptr4 = (byte*)memptr - 8;
		sbyte b3 = *(sbyte*)ptr4;
		sbyte b4 = 71;
		if (b3 >= 71)
		{
			while (b3 <= b4)
			{
				if (num3 != 1)
				{
					num3--;
					ptr4 = (byte*)ptr4 + 1;
					ptr3++;
					b3 = *(sbyte*)ptr4;
					b4 = *ptr3;
					if (b3 < b4)
					{
						break;
					}
					continue;
				}
				goto IL_0110;
			}
		}
		gprintf((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0DN_0040BAJDGDMF_0040gfree_003F5_003F9_003F5SENTINAL_003F5CORRUPTED_003F_0024DL_003F5PROB_0040));
		goto IL_0153;
		IL_00c8:
		result = 1;
		// IL cpblk instruction
		System.Runtime.CompilerServices.Unsafe.CopyBlock((byte*)memptr - 8, ref _003F_003F_C_0040_04IPEHMKME_0040GFRE_003F_0024AA_0040, 4);
		free((byte*)memptr - 8);
		galloccount--;
		goto IL_0153;
		IL_0110:
		gprintf((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0CG_0040MJDOKNOD_0040gfree_003F5_003F9_003F5ATTEMPT_003F5TO_003F5FREE_003F5MEMORY_003F5T_0040));
		goto IL_0153;
		IL_007f:
		if (num > 0)
		{
			uint num4 = 4u;
			sbyte* ptr5 = (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04JBNFAOHP_0040GEND_003F_0024AA_0040);
			void* ptr6 = num + (byte*)memptr;
			sbyte b5 = *(sbyte*)ptr6;
			sbyte b6 = 71;
			if (b5 >= 71)
			{
				while (b5 <= b6)
				{
					if (num4 != 1)
					{
						num4--;
						ptr6 = (byte*)ptr6 + 1;
						ptr5++;
						b5 = *(sbyte*)ptr6;
						b6 = *ptr5;
						if (b5 < b6)
						{
							break;
						}
						continue;
					}
					goto IL_00c8;
				}
			}
		}
		goto IL_00cd;
		IL_0153:
		return result;
	}

	internal unsafe static STDSTREAM* gopen(sbyte* filename)
	{
		_iobuf* ptr;
		int num;
		if (strncmp(filename, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04ODOKNKHJ_0040con_003F3_003F_0024AA_0040), 4u) == 0)
		{
			ptr = __iob_func();
			num = 0;
		}
		else
		{
			ptr = fopen(filename, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_02JDPG_0040rb_003F_0024AA_0040));
			num = 1;
		}
		STDSTREAM* ptr2 = null;
		if (ptr != null)
		{
			if (fseek(ptr, 0, 2) == 0)
			{
				int num2 = ftell(ptr);
				fseek(ptr, 0, 0);
				sbyte* ptr3 = (sbyte*)malloc(9264u);
				if (ptr3 != null && ptr3 != (sbyte*)(-1))
				{
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr3, ref _003F_003F_C_0040_04IJPGHCDI_0040GMEM_003F_0024AA_0040, 4);
					galloccount++;
					((int*)ptr3)[1] = 9252;
					// IL cpblk instruction
					System.Runtime.CompilerServices.Unsafe.CopyBlock(ptr3 + 9260, ref _003F_003F_C_0040_04JBNFAOHP_0040GEND_003F_0024AA_0040, 4);
					ptr2 = (STDSTREAM*)(ptr3 + 8);
					if (ptr2 != null)
					{
						*(int*)ptr2 = (int)ptr;
						((int*)ptr2)[1] = 0;
						((int*)ptr2)[2] = num2;
						STDSTREAM* ptr4 = (STDSTREAM*)((byte*)ptr2 + 1060);
						((int*)ptr2)[4] = (int)ptr4;
						((int*)ptr2)[5] = (int)ptr4;
						((int*)ptr2)[6] = 0;
						((int*)ptr2)[7] = 0;
						((int*)ptr2)[8] = num;
						((int*)ptr2)[3] = 0;
						sbyte* ptr5 = filename;
						int num3 = (int)((byte*)ptr2 + 36 - (nuint)filename);
						sbyte b;
						do
						{
							b = (*(num3 + ptr5) = *ptr5);
							ptr5++;
						}
						while (b != 0);
						goto IL_0130;
					}
				}
				else
				{
					((delegate* unmanaged[Cdecl, Cdecl]<sbyte*, int, int>)gprintf)((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0DE_0040KIDLMJEG_0040galloc_003F5_003F9_003F5FAILED_003F4_003F5OUT_003F5OF_003F5MEMORY_003F4_003F5_0040), 9252);
					ptr2 = null;
				}
				((delegate* unmanaged[Cdecl, Cdecl]<sbyte*, sbyte*, int>)gprintf)((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0DI_0040CIMFANBI_0040gopen_003F5_003F9_003F5UNABLE_003F5TO_003F5ALLOCATE_003F5MEMOR_0040), filename);
			}
			else
			{
				((delegate* unmanaged[Cdecl, Cdecl]<sbyte*, sbyte*, int>)gprintf)((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0CC_0040MBJJOILM_0040gopen_003F5_003F9_003F5SEEK_003F5ON_003F5OPEN_003F5FAILED_003F5_003F_0024CI_003F_0024CFs_003F_0024CJ_0040), filename);
			}
		}
		goto IL_0130;
		IL_0130:
		return ptr2;
	}

	internal unsafe static int gclose(STDSTREAM* g)
	{
		int num = 1;
		if (g != null)
		{
			_003FA0xc5a70bfa_002Egwriteflush(g);
			int num2 = ((int*)g)[8];
			if (num2 != 0)
			{
				if (fclose((_iobuf*)(int)(*(uint*)g)) == 0)
				{
					num = 1;
					if (num2 == 2 || num2 == 3)
					{
						System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0CAA_0040D _0024ArrayType_0024_0024_0024BY0CAA_0040D);
						_003FA0xc5a70bfa_002EGIMEX_tempname((sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), (sbyte*)g + 36, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04OAOKCEO_0040_003F4tm2_003F_0024AA_0040));
						remove((sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D));
						rename((sbyte*)g + 36, (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D));
						if (rename((sbyte*)g + 548, (sbyte*)g + 36) == 0)
						{
							num = 1;
							if (num2 == 2)
							{
								System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0CAA_0040D _0024ArrayType_0024_0024_0024BY0CAA_0040D2);
								_003FA0xc5a70bfa_002EGIMEX_tempname((sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D2), (sbyte*)g + 36, (sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_04BJBPLFHE_0040_003F4bak_003F_0024AA_0040));
								remove((sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D2));
								rename((sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D), (sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D2));
							}
							else
							{
								remove((sbyte*)(&_0024ArrayType_0024_0024_0024BY0CAA_0040D));
							}
						}
						else
						{
							num = 0;
						}
					}
				}
				else
				{
					num = 0;
				}
			}
			num &= gfree(g);
		}
		return num;
	}

	internal unsafe static int gread(STDSTREAM* g, void* buf, int size)
	{
		int num = 0;
		int num2 = 0;
		if (g == null)
		{
			return 0;
		}
		if (size < 0)
		{
			size = 0;
		}
		int num3 = ((int*)g)[4];
		int num4 = ((int*)g)[5] - num3;
		int num5 = (num4 >= size ? ref size : ref num4);
		if (num5 > 0)
		{
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(buf, num3, num5);
			((int*)g)[4] += num5;
			((int*)g)[1] += num5;
			buf = num5 + (byte*)buf;
			size -= num5;
			num = num5;
		}
		int num6 = ((int*)g)[1];
		int num7 = ((int*)g)[2] - num6;
		size = (num7 >= size ? ref size : ref num7);
		if (size < 0)
		{
			gprintf((sbyte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003F_003F_C_0040_0CK_0040BMDNBCPF_0040gread_003F5_003F9_003F5ATTEMPT_003F5TO_003F5READ_003F5PAST_003F5END_0040));
			return num;
		}
		if (((int*)g)[5] - ((int*)g)[4] == 0 && size != 0)
		{
			int num8 = (num6 + size) / 8192 * 8192;
			_003FA0xc5a70bfa_002Egwriteflush(g);
			num6 = ((int*)g)[1];
			if (num6 < num8)
			{
				num2 = (int)fread(buf, 1u, (uint)(num8 - num6), (_iobuf*)(int)(*(uint*)g));
				buf = num2 + (byte*)buf;
				size -= num2;
			}
			((int*)g)[3] = num8;
			int num9 = ((int*)g)[2] - num8;
			int num10 = 8192;
			num5 = (num9 >= 8192 ? ref num10 : ref num9);
			fread((byte*)g + 1060, 1u, (uint)num5, (_iobuf*)(int)(*(uint*)g));
			// IL cpblk instruction
			System.Runtime.CompilerServices.Unsafe.CopyBlock(buf, ((int*)g)[4], size);
			num2 = size + num2;
			((int*)g)[4] += size;
			((int*)g)[5] += num5;
			((int*)g)[1] += num2;
		}
		return num2 + num;
	}

	internal unsafe static long glen(STDSTREAM* g)
	{
		return ((int*)g)[2];
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
		_003CCrtImplementationDetails_003E_002ERegisterModuleUninitializer([PrePrepareMethod] [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] (object source, EventArgs arguments) =>
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

	[PrePrepareMethod]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
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

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[DebuggerStepThrough]
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

	internal unsafe static int _atexit_m(delegate*<void> func)
	{
		delegate*<void>* ptr = (delegate*<void>*)_decode_pointer(_003FA0x23130d9f_002E__onexitbegin_m);
		delegate*<void>* ptr2 = (delegate*<void>*)_decode_pointer(_003FA0x23130d9f_002E__onexitend_m);
		int result = _atexit_helper((delegate*<void>)_encode_pointer(func), (uint*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref _003FA0x23130d9f_002E__exit_list_size), &ptr2, &ptr);
		_003FA0x23130d9f_002E__onexitbegin_m = (delegate*<void>*)_encode_pointer(ptr);
		_003FA0x23130d9f_002E__onexitend_m = (delegate*<void>*)_encode_pointer(ptr2);
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

	[DllImport("", CallingConvention = CallingConvention.ThisCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* std_002Eios_base_002E_002EPAX(ios_base* P_0);

	[DllImport("", CallingConvention = CallingConvention.ThisCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static extern bool std_002Eios_base_002Egood(ios_base* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern SNDAutoLock* AudioPhysicalRendererInternal_002ESNDAutoLock_002E_007Bctor_007D(SNDAutoLock* P_0, rw.audio.core.System* P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void AudioPhysicalRendererInternal_002ESNDAutoLock_002E_007Bdtor_007D(SNDAutoLock* P_0);

	[DllImport("", CallingConvention = CallingConvention.ThisCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E__vbaseDtor(basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E* P_0);

	[DllImport("", CallingConvention = CallingConvention.ThisCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern basic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E* std_002Ebasic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Egetline(basic_istream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E* P_0, sbyte* P_1, int P_2);

	[DllImport("", CallingConvention = CallingConvention.ThisCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E* std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002E_007Bctor_007D(basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E* P_0, sbyte* P_1, int P_2, int P_3, int P_4);

	[DllImport("", CallingConvention = CallingConvention.ThisCall, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void std_002Ebasic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E_002Eclose(basic_fstream_003Cchar_002Cstd_003A_003Achar_traits_003Cchar_003E_0020_003E* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void rw_002Eaudio_002Ecore_002EDecoderRegistry_002ERegisterAllDecoders(DecoderRegistry* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern rw.audio.core.System* rw_002Eaudio_002Ecore_002ESystem_002ECreateInstance(ICoreAllocator* P_0, int P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void rw_002Eaudio_002Ecore_002ESystem_002ERelease(rw.audio.core.System* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern DecoderRegistry* rw_002Eaudio_002Ecore_002ESystem_002EGetDecoderRegistry(rw.audio.core.System* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern EncoderRegistry* rw_002Eaudio_002Ecore_002ESystem_002EGetEncoderRegistry(rw.audio.core.System* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void rw_002Eaudio_002Ecore_002EEncoderRegistry_002ERegisterAllEncoders(EncoderRegistry* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* @new(uint P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_read(SINSTANCE* P_0, SINFO* P_1, int P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int fclose(_iobuf* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int fseek(_iobuf* P_0, int P_1, int P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_wclose(SINSTANCE* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_info(SINSTANCE* P_0, SINFO** P_1, int P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int ftell(_iobuf* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern sbyte* SIMEX_getsamplerepname(int P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_freesinfo(SINFO* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_close(SINSTANCE* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern SIMEXFILTERABOUT* SIMEX_filterabout(int P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void delete(void* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern uint fread(void* P_0, uint P_1, uint P_2, _iobuf* P_3);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void delete_005B_005D(void* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern _iobuf* fopen(sbyte* P_0, sbyte* P_1);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern int tolower(int P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int strtol(sbyte* P_0, sbyte** P_1, int P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_filterssound(SSOUND* P_0, int P_1, SIMEXFILTERPARAM* P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void rewind(_iobuf* P_0);

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
	public unsafe static extern sbyte* SIMEX_getlasterr();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public static extern double ceil(double P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_open(sbyte* P_0, long P_1, int P_2, SINSTANCE** P_3);

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
	public unsafe static extern int SIMEX_write(SINSTANCE* P_0, SINFO* P_1, int P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_create(sbyte* P_0, int P_1, SINSTANCE** P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern GeneralAllocator* EA_002EAllocator_002EGeneralAllocator_002E_007Bctor_007D(GeneralAllocator* P_0, void* P_1, uint P_2, [MarshalAs(UnmanagedType.U1)] bool P_3, [MarshalAs(UnmanagedType.U1)] bool P_4, delegate* unmanaged[Cdecl, Cdecl]<GeneralAllocator*, void*, uint, void*, uint> P_5, void* P_6);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void EA_002EAllocator_002EGeneralAllocator_002E_007Bdtor_007D(GeneralAllocator* P_0);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs(UnmanagedType.U1)]
	public unsafe static extern bool EA_002EAllocator_002EGeneralAllocator_002EInit(GeneralAllocator* P_0, void* P_1, uint P_2, [MarshalAs(UnmanagedType.U1)] bool P_3, [MarshalAs(UnmanagedType.U1)] bool P_4, delegate* unmanaged[Cdecl, Cdecl]<GeneralAllocator*, void*, uint, void*, uint> P_5, void* P_6);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* EA_002EAllocator_002EGeneralAllocator_002EMalloc(GeneralAllocator* P_0, uint P_1, int P_2);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void EA_002EAllocator_002EGeneralAllocator_002EFree(GeneralAllocator* P_0, void* P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern void* EA_002EAllocator_002EGeneralAllocator_002EMallocAligned(GeneralAllocator* P_0, uint P_1, uint P_2, uint P_3, int P_4);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int SIMEX_id(sbyte* P_0, long P_1);

	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int stdprintf(sbyte* P_0, __arglist);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern uint fwrite(void* P_0, uint P_1, uint P_2, _iobuf* P_3);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int remove(sbyte* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int fflush(_iobuf* P_0);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern _iobuf* __iob_func();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int strncmp(sbyte* P_0, sbyte* P_1, uint P_2);

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int rename(sbyte* P_0, sbyte* P_1);

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
	public static extern void terminate();

	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
	[SuppressUnmanagedCodeSecurity]
	public unsafe static extern int __FrameUnwindFilter(global::_EXCEPTION_POINTERS* P_0);

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
