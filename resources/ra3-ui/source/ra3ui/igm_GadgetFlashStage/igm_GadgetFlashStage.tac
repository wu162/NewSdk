movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IGM_GA~1\\IGM_GA~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 31

    &frame 30
      &pushzero
      &pushs 'onEndFlashPulse'
      &callfp
    &end // of frame 30
  &end // of defineMovieClip 20
  
  &exportAssets
    20 &as 'GadgetFlash'
  &end // of exportAssets

  &frame 0
    &pushs 'obj'
    &pushthisgv
    &pushone
    &pushs 'igm_gadgetFlashStage'
    &new
    &setVariable
    &stop
  &end // of frame 0

  &defineMovieClip 21 // total frames: 0
  &end // of defineMovieClip 21
  
  &exportAssets
    21 &as '__Packages.igm_gadgetFlashStage'
  &end // of exportAssets
  
  &initMovieClip 21
    &constants '_global', 'igm_gadgetFlashStage', 'igm_gadgetFlashStage::igm_gadgetFlashStage( ', ' )', 'm_clip', 'onUnloading', 'bind0', 'igm_gadgetFlash', 'GadgetFlash', 'Object', 'registerClass', 'CreateHUDGadgetFlash', 'CreateGadgetFlash', 'bind1DynamicParams', 'prototype', 'igm_gadgetFlashStage::onUnloading()', 'igm_gadgetFlashStage::CreateGadgetFlash( ', 'm_nextFlashClipId', 'm_flashClipCount', 'igm_gadgetFlashStage::CreateGadgetFlash() flashId=', ', m_flashClipCount=', 'm_owner', 'm_targetClip', 'attachMovie', 'OnDestroyingGadgetFlash', 'igm_gadgetFlashStage::OnDestroyingGadgetFlash( ', 'igm_gadgetFlashStage::OnDestroyingGadgetFlash() m_flashClipCount=', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'igm_gadgetFlashStage'
    &not
    &not
    &jnz label2    
    &pushglobalgv
    &pushsdb 1							//'igm_gadgetFlashStage'
    &function2  (r:3='clip') (r:1='this', r:2='_global')
      &pushsdb 2							//'igm_gadgetFlashStage::igm_gadgetFlashStage( '
      &pushregister 3
      &toString
      &add
      &pushsdb 3							//' )'
      &add
      &trace
      &pushregister 1
      &pushsdb 4							//'m_clip'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdbgm 4							//'m_clip'
      &pushsdb 5							//'onUnloading'
      &pushregister 1
      &pushsdbgm 5							//'onUnloading'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 7							//'igm_gadgetFlash'
      &pushsdb 8							//'GadgetFlash'
      &pushbyte 2
      &pushsdbgv 9							//'Object'
      &dcallmp 10							// registerClass()
      &pushregister 2
      &pushsdb 11							//'CreateHUDGadgetFlash'
      &pushregister 1
      &pushsdbgm 12							//'CreateGadgetFlash'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 13							//'bind1DynamicParams'
      &callMethod
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 14							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 5							//'onUnloading'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 15							//'igm_gadgetFlashStage::onUnloading()'
      &trace
      &pushregister 2
      &pushsdb 11							//'CreateHUDGadgetFlash'
      &delete
      &pop
      &pushnull
      &pushsdb 8							//'GadgetFlash'
      &pushbyte 2
      &pushsdbgv 9							//'Object'
      &dcallmp 10							// registerClass()
      &pushregister 2
      &pushsdb 7							//'igm_gadgetFlash'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 4							//'m_clip'
      &pushsdb 5							//'onUnloading'
      &delete
      &pop
      &pushregister 2
      &pushsdb 1							//'igm_gadgetFlashStage'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 12							//'CreateGadgetFlash'
    &function2  (r:4='targetClip') (r:1='this')
      &pushsdb 16							//'igm_gadgetFlashStage::CreateGadgetFlash( '
      &pushregister 4
      &toString
      &add
      &pushsdb 3							//' )'
      &add
      &trace
      &pushregister 1
      &pushsdbgm 17							//'m_nextFlashClipId'
      &pushregister 1
      &pushsdb 17							//'m_nextFlashClipId'
      &pushregister 1
      &pushsdbgm 17							//'m_nextFlashClipId'
      &increment
      &setMember
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdb 18							//'m_flashClipCount'
      &pushregister 1
      &pushsdbgm 18							//'m_flashClipCount'
      &increment
      &setMember
      &pushsdb 19							//'igm_gadgetFlashStage::CreateGadgetFlash() flashId='
      &pushregister 2
      &toString
      &add
      &pushsdb 20							//', m_flashClipCount='
      &add
      &pushregister 1
      &pushsdbgm 18							//'m_flashClipCount'
      &toString
      &add
      &trace
      &pushsdb 21							//'m_owner'
      &pushregister 1
      &pushsdb 22							//'m_targetClip'
      &pushregister 4
      &pushbyte 2
      &initObject
      &pushregister 2
      &pushregister 2
      &toString
      &pushsdb 8							//'GadgetFlash'
      &pushbyte 4
      &pushregister 1
      &pushsdbgm 4							//'m_clip'
      &pushsdb 23							//'attachMovie'
      &callMethod
      &setRegister r:3
      &pop
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 24							//'OnDestroyingGadgetFlash'
    &function2  (r:2='flashClip') (r:1='this')
      &pushsdb 25							//'igm_gadgetFlashStage::OnDestroyingGadgetFlash( '
      &pushregister 2
      &toString
      &add
      &pushsdb 3							//' )'
      &add
      &trace
      &pushregister 1
      &pushsdb 18							//'m_flashClipCount'
      &pushregister 1
      &pushsdbgm 18							//'m_flashClipCount'
      &decrement
      &setRegister r:0
      &setMember
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label1      
      &pushregister 1
      &pushsdb 17							//'m_nextFlashClipId'
      &pushzero
      &setMember
     label1:
      &pushsdb 26							//'igm_gadgetFlashStage::OnDestroyingGadgetFlash() m_flashClipCount='
      &pushregister 1
      &pushsdbgm 18							//'m_flashClipCount'
      &toString
      &add
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 17							//'m_nextFlashClipId'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 18							//'m_flashClipCount'
    &pushzero
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'igm_gadgetFlashStage'
    &pushsdbgm 14							//'prototype'
    &pushbyte 3
    &pushsdb 27							//'ASSetPropFlags'
    &callFunction
   label2:
    &pop
  &end // of initMovieClip 21

  &defineMovieClip 22 // total frames: 0
  &end // of defineMovieClip 22
  
  &exportAssets
    22 &as '__Packages.igm_gadgetFlash'
  &end // of exportAssets
  
  &initMovieClip 22
    &constants '_global', 'igm_gadgetFlash', 'igm_gadgetFlash::igm_gadgetFlash() m_targetClip=', 'm_targetClip', ', m_targetClip._visible=', '_visible', 'TrackTarget', 'MovieClip', 'prototype', 'Close', 'm_done', 'SelfDestruct', 'm_owner', 'OnDestroyingGadgetFlash', 'removeMovieClip', 'getBounds', '_root', 'xMin', 'yMin', 'xMax', 'yMax', '_xscale', '_yscale', '_x', '_y', '_parent', 'm_lastXMin', 'm_lastXMax', 'm_lastYMin', 'm_lastYMax', 'defaultBorderOuterSize', 'defaultBorderInnerSize', 'Math', 'min', 'boxMC', 'tl', '_width', '_height', 'top', 'tr', 'right', 'br', 'bottom', 'bl', 'left', 'onEnterFrame', 'onEndFlashPulse', 'gotoAndPlay', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'igm_gadgetFlash'
    &not
    &not
    &jnz label16    
    &pushglobalgv
    &pushsdb 1							//'igm_gadgetFlash'
    &function2  () (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'igm_gadgetFlash::igm_gadgetFlash() m_targetClip='
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &toString
      &add
      &pushsdb 4							//', m_targetClip._visible='
      &add
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &pushsdbgm 5							//'_visible'
      &toString
      &add
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 6							// TrackTarget()
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'igm_gadgetFlash'
    &pushsdbgv 7							//'MovieClip'
    &extends
    &pushregister 0
    &pushsdbgm 8							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 9							//'Close'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 10							//'m_done'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 11							//'SelfDestruct'
    &function2  () (r:1='this')
      &pushregister 1
      &pushone
      &pushregister 1
      &pushsdbgm 12							//'m_owner'
      &dcallmp 13							// OnDestroyingGadgetFlash()
      &pushzero
      &pushregister 1
      &dcallmp 14							// removeMovieClip()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 6							//'TrackTarget'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &pushundef
      &equals
      &not
      &jnz label1      
      &pushundef
      &return
     label1:
      &pushzero
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &pushsdb 15							//'getBounds'
      &callMethod
      &setRegister r:17
      &pop
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &pushsdbgm 16							//'_root'
      &setRegister r:14
      &pop
      &pushregister 17
      &pushsdbgm 17							//'xMin'
      &setRegister r:8
      &pop
      &pushregister 17
      &pushsdbgm 18							//'yMin'
      &setRegister r:9
      &pop
      &pushregister 17
      &pushsdbgm 19							//'xMax'
      &setRegister r:10
      &pop
      &pushregister 17
      &pushsdbgm 20							//'yMax'
      &setRegister r:11
      &pop
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &setRegister r:2
      &pop
     label2:
      &pushfalse
      &jnz label7      
      &pushregister 2
      &pushregister 14
      &equals
      &not
      &jnz label3      
      &pushregister 1
      &pushsdb 5							//'_visible'
      &pushtrue
      &setMember
      &jmp label7      
     label3:
      &pushregister 2
      &pushsdbgm 5							//'_visible'
      &not
      &not
      &jnz label4      
      &pushregister 1
      &pushsdb 5							//'_visible'
      &pushfalse
      &setMember
      &pushundef
      &return
     label4:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &equals
      &not
      &not
      &jnz label6      
      &pushregister 2
      &pushsdbgm 21							//'_xscale'
      &setRegister r:4
      &pop
      &pushregister 2
      &pushsdbgm 22							//'_yscale'
      &setRegister r:3
      &pop
      &pushregister 4
      &pushbyte 100
      &equals
      &not
      &not
      &jnz label5      
      &pushregister 4
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:4
      &pop
      &pushregister 8
      &pushregister 4
      &multiply
      &setRegister r:8
      &pop
      &pushregister 10
      &pushregister 4
      &multiply
      &setRegister r:10
      &pop
     label5:
      &pushregister 3
      &pushbyte 100
      &equals
      &not
      &not
      &jnz label6      
      &pushregister 3
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:3
      &pop
      &pushregister 9
      &pushregister 3
      &multiply
      &setRegister r:9
      &pop
      &pushregister 11
      &pushregister 3
      &multiply
      &setRegister r:11
      &pop
     label6:
      &pushregister 2
      &pushsdbgm 23							//'_x'
      &setRegister r:6
      &pop
      &pushregister 2
      &pushsdbgm 24							//'_y'
      &setRegister r:5
      &pop
      &pushregister 8
      &pushregister 6
      &add
      &setRegister r:8
      &pop
      &pushregister 10
      &pushregister 6
      &add
      &setRegister r:10
      &pop
      &pushregister 9
      &pushregister 5
      &add
      &setRegister r:9
      &pop
      &pushregister 11
      &pushregister 5
      &add
      &setRegister r:11
      &pop
      &pushregister 2
      &pushsdbgm 25							//'_parent'
      &setRegister r:2
      &pop
      &jmp label2      
     label7:
      &pushregister 8
      &pushregister 1
      &pushsdbgm 26							//'m_lastXMin'
      &equals
      &dup
      &not
      &jnz label8      
      &pop
      &pushregister 10
      &pushregister 1
      &pushsdbgm 27							//'m_lastXMax'
      &equals
     label8:
      &dup
      &not
      &jnz label9      
      &pop
      &pushregister 9
      &pushregister 1
      &pushsdbgm 28							//'m_lastYMin'
      &equals
     label9:
      &dup
      &not
      &jnz label10      
      &pop
      &pushregister 11
      &pushregister 1
      &pushsdbgm 29							//'m_lastYMax'
      &equals
     label10:
      &not
      &jnz label11      
      &pushundef
      &return
     label11:
      &pushsdbgv 1							//'igm_gadgetFlash'
      &pushsdbgm 30							//'defaultBorderOuterSize'
      &setRegister r:13
      &pop
      &pushsdbgv 1							//'igm_gadgetFlash'
      &pushsdbgm 31							//'defaultBorderInnerSize'
      &setRegister r:7
      &pop
      &pushregister 10
      &pushregister 8
      &subtract
      &setRegister r:16
      &pop
      &pushregister 11
      &pushregister 9
      &subtract
      &setRegister r:15
      &pop
      &pushregister 15
      &pushregister 16
      &pushbyte 2
      &pushsdbgv 32							//'Math'
      &pushsdb 33							//'min'
      &callMethod
      &setRegister r:18
      &pop
      &pushregister 18
      &pushregister 7
      &pushbyte 2
      &multiply
      &lessThan
      &not
      &jnz label12      
      &pushregister 18
      &pushfloat 0.500000000000000
      &multiply
      &setRegister r:19
      &pop
      &pushregister 13
      &pushregister 19
      &pushregister 7
      &divide
      &multiply
      &setRegister r:13
      &pop
      &pushregister 19
      &setRegister r:7
      &pop
     label12:
      &pushregister 13
      &pushregister 7
      &add
      &setRegister r:12
      &pop
      &pushregister 1
      &pushsdb 23							//'_x'
      &pushregister 8
      &setMember
      &pushregister 1
      &pushsdb 24							//'_y'
      &pushregister 9
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 35							//'tl'
      &pushsdb 23							//'_x'
      &pushzero
      &pushregister 13
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 35							//'tl'
      &pushsdb 24							//'_y'
      &pushzero
      &pushregister 13
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 35							//'tl'
      &pushsdb 36							//'_width'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 35							//'tl'
      &pushsdb 37							//'_height'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 38							//'top'
      &pushsdb 23							//'_x'
      &pushregister 7
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 38							//'top'
      &pushsdb 24							//'_y'
      &pushzero
      &pushregister 13
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 38							//'top'
      &pushsdb 36							//'_width'
      &pushregister 16
      &pushregister 7
      &pushbyte 2
      &multiply
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 38							//'top'
      &pushsdb 37							//'_height'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 39							//'tr'
      &pushsdb 23							//'_x'
      &pushregister 16
      &pushregister 7
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 39							//'tr'
      &pushsdb 24							//'_y'
      &pushzero
      &pushregister 13
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 39							//'tr'
      &pushsdb 36							//'_width'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 39							//'tr'
      &pushsdb 37							//'_height'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 40							//'right'
      &pushsdb 23							//'_x'
      &pushregister 16
      &pushregister 7
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 40							//'right'
      &pushsdb 24							//'_y'
      &pushregister 7
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 40							//'right'
      &pushsdb 36							//'_width'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 40							//'right'
      &pushsdb 37							//'_height'
      &pushregister 15
      &pushregister 7
      &pushbyte 2
      &multiply
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 41							//'br'
      &pushsdb 23							//'_x'
      &pushregister 16
      &pushregister 7
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 41							//'br'
      &pushsdb 24							//'_y'
      &pushregister 15
      &pushregister 7
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 41							//'br'
      &pushsdb 36							//'_width'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 41							//'br'
      &pushsdb 37							//'_height'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 42							//'bottom'
      &pushsdb 23							//'_x'
      &pushregister 7
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 42							//'bottom'
      &pushsdb 24							//'_y'
      &pushregister 15
      &pushregister 7
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 42							//'bottom'
      &pushsdb 36							//'_width'
      &pushregister 16
      &pushregister 7
      &pushbyte 2
      &multiply
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 42							//'bottom'
      &pushsdb 37							//'_height'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 43							//'bl'
      &pushsdb 23							//'_x'
      &pushzero
      &pushregister 13
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 43							//'bl'
      &pushsdb 24							//'_y'
      &pushregister 15
      &pushregister 7
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 43							//'bl'
      &pushsdb 36							//'_width'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 43							//'bl'
      &pushsdb 37							//'_height'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 44							//'left'
      &pushsdb 23							//'_x'
      &pushzero
      &pushregister 13
      &subtract
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 44							//'left'
      &pushsdb 24							//'_y'
      &pushregister 7
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 44							//'left'
      &pushsdb 36							//'_width'
      &pushregister 12
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'boxMC'
      &pushsdbgm 44							//'left'
      &pushsdb 37							//'_height'
      &pushregister 15
      &pushregister 7
      &pushbyte 2
      &multiply
      &subtract
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 45							//'onEnterFrame'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 3							//'m_targetClip'
      &pushundef
      &equals
      &not
      &jnz label13      
      &pushzero
      &pushregister 1
      &dcallmp 11							// SelfDestruct()
      &pushundef
      &return
     label13:
      &pushzero
      &pushregister 1
      &dcallmp 6							// TrackTarget()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 46							//'onEndFlashPulse'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 10							//'m_done'
      &not
      &jnz label14      
      &pushzero
      &pushregister 1
      &dcallmp 11							// SelfDestruct()
      &jmp label15      
     label14:
      &pushone
      &pushone
      &pushregister 1
      &pushsdb 47							//'gotoAndPlay'
      &callMethod
     label15:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 10							//'m_done'
    &pushfalse
    &setMember
    &pushregister 0
    &pushsdb 30							//'defaultBorderOuterSize'
    &pushbyte 14
    &setMember
    &pushregister 0
    &pushsdb 31							//'defaultBorderInnerSize'
    &pushbyte 9
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'igm_gadgetFlash'
    &pushsdbgm 8							//'prototype'
    &pushbyte 3
    &pushsdb 48							//'ASSetPropFlags'
    &callFunction
   label16:
    &pop
  &end // of initMovieClip 22
&end
