movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\J09_GL~1\\J09_GL~1.eaf' &compressed // flash 7, total frames: 663, frame rate: 30 fps, 1365x768 px

  &frame 0
    &constants 'UBriefGlobal_Crosshair1Flash', 'playSound', 'UBriefGlobal_Crosshair1Move', 'UBriefGlobal_Photo1Flash', 'UBriefGlobal_Photo1Place', 'UBriefGlobal_Photo1SlideIn', 'UBriefGlobal_Start', 'UBriefGlobal_SwipeHorizontal', 'UBriefGlobal_SwipeVertical', 'UBriefGlobal_ZoomIn', 'UBriefGlobal_ZoomOut', '_parent', 'screen', 'onBriefingLoaded'  
    &function2 playSound_CrossHairFlash () (r:1='_global')
      &pushsdb 0							//'UBriefGlobal_Crosshair1Flash'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_CrossHairFlash

    &function2 playSound_CrossHairMove () (r:1='_global')
      &pushsdb 2							//'UBriefGlobal_Crosshair1Move'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_CrossHairMove

    &function2 playSound_PhotoFlash () (r:1='_global')
      &pushsdb 3							//'UBriefGlobal_Photo1Flash'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_PhotoFlash

    &function2 playSound_PhotoPlace () (r:1='_global')
      &pushsdb 4							//'UBriefGlobal_Photo1Place'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_PhotoPlace

    &function2 playSound_PhotoSlideIn () (r:1='_global')
      &pushsdb 5							//'UBriefGlobal_Photo1SlideIn'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_PhotoSlideIn

    &function2 playSound_Start () (r:1='_global')
      &pushsdb 6							//'UBriefGlobal_Start'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_Start

    &function2 playSound_SwipeHorizontal () (r:1='_global')
      &pushsdb 7							//'UBriefGlobal_SwipeHorizontal'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_SwipeHorizontal

    &function2 playSound_SwipeVertical () (r:1='_global')
      &pushsdb 8							//'UBriefGlobal_SwipeVertical'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_SwipeVertical

    &function2 playSound_ZoomIn () (r:1='_global')
      &pushsdb 9							//'UBriefGlobal_ZoomIn'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_ZoomIn

    &function2 playSound_ZoomOut () (r:1='_global')
      &pushsdb 10							//'UBriefGlobal_ZoomOut'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_ZoomOut

    &pushzero
    &pushsdbgv 11							//'_parent'
    &pushsdbgm 12							//'screen'
    &dcallmp 13							// onBriefingLoaded()
  &end // of frame 0
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'global_map2' &as 1
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'grid' &as 2
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'blacktored' &as 3
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'red_fade.png' &as 4
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'TV_lines.png' &as 5
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'black frame' &as 8
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'RULERS' &as 11
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'ticker_stripe.png' &as 12
  &end // of importAssets

  &frame 1
    &pushzero
    &pushs 'playSound_start'
    &callfp
    &pushzero
    &pushs 'playSound_SwipeVertical'
    &callfp
  &end // of frame 1

  &frame 12
    &pushzero
    &pushs 'playSound_SwipeHorizontal'
    &callfp
  &end // of frame 12
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'scanline' &as 15
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'line' &as 16
  &end // of importAssets

  &frame 35
    &pushzero
    &pushs 'playSound_CrossHairMove'
    &callfp
  &end // of frame 35
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'brackets square' &as 17
  &end // of importAssets

  &frame 180
    &pushzero
    &pushs 'playSound_CrossHairFlash'
    &callfp
  &end // of frame 180
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'flashing square' &as 18
  &end // of importAssets

  &frame 189
    &pushzero
    &pushs 'playSound_ZoomIn'
    &callfp
  &end // of frame 189

  &frame 197
    &pushzero
    &pushs 'playSound_CrossHairMove'
    &callfp
  &end // of frame 197

  &frame 208
    &pushzero
    &pushs 'playSound_CrossHairFlash'
    &callfp
  &end // of frame 208

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &frame 219
    &pushzero
    &pushs 'playSound_ZoomOut'
    &callfp
    &pushzero
    &pushs 'playSound_PhotoSlideIn'
    &callfp
  &end // of frame 219

  &frame 228
    &pushzero
    &pushs 'playSound_photoFlash'
    &callfp
  &end // of frame 228
  
  &importAssets &from '..\\common\\shell\\missionBriefings\\fe_missionBriefingAssets.swf'
    'white bracket' &as 22
  &end // of importAssets

  &frame 348
    &pushzero
    &pushs 'playSound_PhotoSlideIn'
    &callfp
  &end // of frame 348

  &defineMovieClip 25 // total frames: 1
  &end // of defineMovieClip 25

  &frame 359
    &pushzero
    &pushs 'playSound_PhotoFlash'
    &callfp
  &end // of frame 359

  &frame 479
    &pushzero
    &pushs 'playSound_PhotoSlideIn'
    &callfp
  &end // of frame 479

  &defineMovieClip 28 // total frames: 1
  &end // of defineMovieClip 28

  &frame 489
    &pushzero
    &pushs 'playSound_PhotoFlash'
    &callfp
  &end // of frame 489

  &frame 609
    &pushzero
    &pushs 'playSound_PhotoSlideIn'
    &callfp
  &end // of frame 609

  &frame 619
    &pushzero
    &pushs 'playSound_PhotoPlace'
    &callfp
    &pushzero
    &pushs 'playSound_CrossHairMove'
    &callfp
  &end // of frame 619

  &frame 642
    &pushzero
    &pushs 'playSound_CrossHairFlash'
    &callfp
  &end // of frame 642

  &frame 662
    &stop
    &pushzero
    &pushsgv 'instanceName'
    &pushs 'loadMovie'
    &callmp
  &end // of frame 662
&end
