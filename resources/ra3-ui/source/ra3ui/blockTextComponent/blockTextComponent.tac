movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\BLOCKT~1\\BLOCKT~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px
  
  &exportAssets
    4 &as 'scrollButton Symbol'
  &end // of exportAssets

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as 'maskClip Symbol'
  &end // of exportAssets
  
  &exportAssets
    6 &as 'maskClip Symbol'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as 'blockTextField Symbol'
  &end // of exportAssets
  
  &exportAssets
    9 &as 'blockTextField Symbol'
  &end // of exportAssets
  
  &exportAssets
    4 &as 'scrollButton Symbol'
  &end // of exportAssets
  
  &exportAssets
    4 &as 'scrollButton Symbol'
  &end // of exportAssets

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10
  
  &exportAssets
    10 &as 'ScrollingBlockText Symbol'
  &end // of exportAssets
  
  &exportAssets
    10 &as 'ScrollingBlockText Symbol'
  &end // of exportAssets

  &placeMovieClip 10 
  
    &onClipEvent &construct
      &pushs 'm_visibleLines'
      &pushbyte 10
      &setVariable
      &pushs 'm_width'
      &pushshort 380
      &setVariable
      &pushs 'm_lineHeight'
      &pushbyte 20
      &setVariable
      &pushs 'm_lineSpacing'
      &pushone
      &setVariable
    &end
  &end // of placeMovieClip 10

  &defineMovieClip 11 // total frames: 0
  &end // of defineMovieClip 11
  
  &exportAssets
    11 &as '__Packages.ScrollingBlockTextClass'
  &end // of exportAssets
  
  &initMovieClip 11
    &constants '_global', 'ScrollingBlockTextClass', 'm_thisClass', 'prototype', 'onLoad', 'ScrollingBlockTextClass::onLoad()', 'setMaskSize', 'setLimits', 'upButton', 'onRelease', 'onUp', 'downButton', 'onDown', 'm_testStr', 'setText', 'ScrollingBlockTextClass::setMaskSize()', 'myText', 'maskMC', '_width', 'm_width', '_height', 'm_visibleLines', 'm_lineHeight', 'm_lineSpacing', 'ScrollingBlockTextClass::onUp()', 'blockTF', '_y', 'm_step', 'm_bottomLimit', 'ScrollingBlockTextClass::onDown()', 'm_topLimit', 'm_totalLines', 'setTotalLines', 'text', 'Flash and APT support masking.\nIt would be implemented on the Flash / Art side in the manner you describe.\n\nThat is one way to go.\n\nThe problem with that method is that we would need one string token that contained all the objectives with embedded line breaks.\n\nScott - we discussed this a bit yesterday - you seemed to favor this method. Should we explore this method that Jefferson is suggesting or the method that I outlined in the email below? BTW I looked at the block text scrolling that we are doing on the 360 for the Terms and Conditions text - a C++ component (fscmds handles scrolling) is used to manage an index into the data which is mapped to a set of string tokens that have static positions on the screen. - didn\'t seem ideal for this application).', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'ScrollingBlockTextClass'
    &not
    &not
    &jnz label3    
    &pushglobalgv
    &pushsdb 1							//'ScrollingBlockTextClass'
    &function2  () (r:1='this')
      &pushsdb 1							//'ScrollingBlockTextClass'
      &trace
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdb 2							//'m_thisClass'
      &pushregister 1
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 3							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 4							//'onLoad'
    &function2  () (r:1='this')
      &pushsdb 5							//'ScrollingBlockTextClass::onLoad()'
      &trace
      &pushzero
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &dcallmp 6							// setMaskSize()
      &pushzero
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &dcallmp 7							// setLimits()
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 8							//'upButton'
      &pushsdb 9							//'onRelease'
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 10							//'onUp'
      &setMember
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 11							//'downButton'
      &pushsdb 9							//'onRelease'
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 12							//'onDown'
      &setMember
      &pushregister 1
      &pushsdbgm 13							//'m_testStr'
      &pushone
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &dcallmp 14							// setText()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 6							//'setMaskSize'
    &function2  () (r:1='this')
      &pushsdb 15							//'ScrollingBlockTextClass::setMaskSize()'
      &trace
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 17							//'maskMC'
      &pushsdb 18							//'_width'
      &pushregister 1
      &pushsdbgm 19							//'m_width'
      &setMember
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 17							//'maskMC'
      &pushsdb 20							//'_height'
      &pushregister 1
      &pushsdbgm 21							//'m_visibleLines'
      &pushregister 1
      &pushsdbgm 22							//'m_lineHeight'
      &pushregister 1
      &pushsdbgm 23							//'m_lineSpacing'
      &add
      &multiply
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 10							//'onUp'
    &function2  () ()
      &pushsdb 24							//'ScrollingBlockTextClass::onUp()'
      &trace
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 25							//'blockTF'
      &pushsdbgm 26							//'_y'
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 27							//'m_step'
      &add
      &setRegister r:1
      &pop
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 28							//'m_bottomLimit'
      &pushregister 1
      &lessThan
      &not
      &jnz label1      
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 28							//'m_bottomLimit'
      &setRegister r:1
      &pop
     label1:
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 25							//'blockTF'
      &pushsdb 26							//'_y'
      &pushregister 1
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 12							//'onDown'
    &function2  () ()
      &pushsdb 29							//'ScrollingBlockTextClass::onDown()'
      &trace
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 25							//'blockTF'
      &pushsdbgm 26							//'_y'
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 27							//'m_step'
      &subtract
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 30							//'m_topLimit'
      &lessThan
      &not
      &jnz label2      
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 30							//'m_topLimit'
      &setRegister r:1
      &pop
     label2:
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 25							//'blockTF'
      &pushsdb 26							//'_y'
      &pushregister 1
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 7							//'setLimits'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 30							//'m_topLimit'
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 17							//'maskMC'
      &pushsdbgm 26							//'_y'
      &pushregister 1
      &pushsdbgm 31							//'m_totalLines'
      &pushregister 1
      &pushsdbgm 21							//'m_visibleLines'
      &subtract
      &pushone
      &add
      &pushregister 1
      &pushsdbgm 22							//'m_lineHeight'
      &pushregister 1
      &pushsdbgm 23							//'m_lineSpacing'
      &add
      &multiply
      &subtract
      &setMember
      &pushregister 1
      &pushsdb 28							//'m_bottomLimit'
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 17							//'maskMC'
      &pushsdbgm 26							//'_y'
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 32							//'setTotalLines'
    &function2  (r:2='totalLines') (r:1='this')
      &pushregister 1
      &pushsdb 31							//'m_totalLines'
      &pushregister 2
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 14							//'setText'
    &function2  (r:1='theText') ()
      &pushsdbgv 1							//'ScrollingBlockTextClass'
      &pushsdbgm 2							//'m_thisClass'
      &pushsdbgm 16							//'myText'
      &pushsdbgm 25							//'blockTF'
      &pushsdb 33							//'text'
      &pushregister 1
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 27							//'m_step'
    &pushbyte 10
    &setMember
    &pushregister 1
    &pushsdb 31							//'m_totalLines'
    &pushbyte 20
    &setMember
    &pushregister 1
    &pushsdb 13							//'m_testStr'
    &pushsdb 34							//'Flash and APT support masking.\nIt would be implemented on the Flash / Art side in the manner you describe.\n\nThat is one way to go.\n\nThe problem with that method is that we would need one string token that contained all the objectives with embedded line breaks.\n\nScott - we discussed this a bit yesterday - you seemed to favor this method. Should we explore this method that Jefferson is suggesting or the method that I outlined in the email below? BTW I looked at the block text scrolling that we are doing on the 360 for the Terms and Conditions text - a C++ component (fscmds handles scrolling) is used to manage an index into the data which is mapped to a set of string tokens that have static positions on the screen. - didn\'t seem ideal for this application).'
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'ScrollingBlockTextClass'
    &pushsdbgm 3							//'prototype'
    &pushbyte 3
    &pushsdb 35							//'ASSetPropFlags'
    &callFunction
   label3:
    &pop
  &end // of initMovieClip 11
  
  &initMovieClip 10
    &pushsgv 'ScrollingBlockTextClass'
    &pushs 'ScrollingBlockText Symbol'
    &pushbyte 2
    &pushsgv 'Object'
    &pushs 'registerClass'
    &callmp
  &end // of initMovieClip 10
&end
