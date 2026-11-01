movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_INF~1\\IG_INF~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants '_x', '_y', '_parent', '', 'boxMC', 'imageMC', '_visible', 'LayoutBox', 'messageTopMargin', 'messageHeight', 'messageBottomMargin', '_height', 'bottomMC', 'bgMC', 'topMC', 'middleMC', 'imageTopMargin', 'messageTF', 'Move', 'ShowImage', 'onUnload', 'initialized', 'this', 'OnContentLoaded', 'text', '$', 'Message', 'qualifyName', '&outline', 'autoSize', 'left', 'FSCommand:', 'OnInitialized'  
    &function2 Move (r:6='x', r:5='y') (r:1='_root', r:2='_parent')
      &pushregister 1
      &setRegister r:4
      &pop
      &pushregister 2
      &setRegister r:3
      &pop
     label1:
      &pushregister 3
      &pushregister 4
      &equals
      &not
      &not
      &jnz label2      
      &pushregister 6
      &pushregister 3
      &pushsdbgm 0							//'_x'
      &subtract
      &setRegister r:6
      &pop
      &pushregister 5
      &pushregister 3
      &pushsdbgm 1							//'_y'
      &subtract
      &setRegister r:5
      &pop
      &pushregister 3
      &pushsdbgm 2							//'_parent'
      &setRegister r:3
      &pop
      &jmp label1      
     label2:
      &pushsdb 3							//''
      &pushzero
      &pushregister 6
      &setProperty
      &pushsdb 3							//''
      &pushone
      &pushregister 5
      &setProperty
    &end // of function Move

    &function ShowImage (    )    
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 5							//'imageMC'
      &pushsdb 6							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &dcallfp 7							// LayoutBox()
    &end // of function ShowImage

    &function2 LayoutBox () ()
      &pushsdbgv 8							//'messageTopMargin'
      &pushsdbgv 9							//'messageHeight'
      &add
      &pushsdbgv 10							//'messageBottomMargin'
      &add
      &setRegister r:2
      &pop
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 5							//'imageMC'
      &pushsdbgm 6							//'_visible'
      &not
      &jnz label3      
      &pushregister 2
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 5							//'imageMC'
      &pushsdbgm 11							//'_height'
      &add
      &setRegister r:2
      &pop
     label3:
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 12							//'bottomMC'
      &pushsdbgm 1							//'_y'
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 12							//'bottomMC'
      &pushsdbgm 11							//'_height'
      &add
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 2
      &subtract
      &setRegister r:1
      &pop
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 13							//'bgMC'
      &pushsdb 1							//'_y'
      &pushregister 1
      &setMember
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 13							//'bgMC'
      &pushsdb 11							//'_height'
      &pushregister 2
      &setMember
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 14							//'topMC'
      &pushsdb 1							//'_y'
      &pushregister 1
      &setMember
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 15							//'middleMC'
      &pushsdb 1							//'_y'
      &pushregister 1
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 14							//'topMC'
      &pushsdbgm 11							//'_height'
      &add
      &setMember
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 15							//'middleMC'
      &pushsdb 11							//'_height'
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 12							//'bottomMC'
      &pushsdbgm 1							//'_y'
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 15							//'middleMC'
      &pushsdbgm 1							//'_y'
      &subtract
      &setMember
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 5							//'imageMC'
      &pushsdb 1							//'_y'
      &pushregister 1
      &pushsdbgv 16							//'imageTopMargin'
      &add
      &setMember
      &pushsdbgv 4							//'boxMC'
      &pushsdbgm 17							//'messageTF'
      &pushsdb 1							//'_y'
      &pushregister 3
      &pushsdbgv 10							//'messageBottomMargin'
      &subtract
      &pushsdbgv 9							//'messageHeight'
      &subtract
      &setMember
    &end // of function LayoutBox

    &function onUnload (    )    
      &pushsdb 18							//'Move'
      &delete2
      &pop
      &pushsdb 19							//'ShowImage'
      &delete2
      &pop
      &pushsdb 7							//'LayoutBox'
      &delete2
      &pop
      &pushsdb 20							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 21							//'initialized'
    &not
    &not
    &jnz label4    
    &pushthisgv
    &pushone
    &pushsdbgv 2							//'_parent'
    &dcallmp 23							// OnContentLoaded()
    &pushsdb 8							//'messageTopMargin'
    &pushbyte 9
    &setVariable
    &pushsdb 10							//'messageBottomMargin'
    &pushbyte 9
    &setVariable
    &pushsdb 16							//'imageTopMargin'
    &pushbyte 5
    &setVariable
    &pushsdbgv 4							//'boxMC'
    &pushsdbgm 5							//'imageMC'
    &pushsdb 6							//'_visible'
    &pushfalse
    &setMember
    &pushsdbgv 4							//'boxMC'
    &pushsdbgm 17							//'messageTF'
    &pushsdb 24							//'text'
    &pushsdb 25							//'$'
    &pushsdb 26							//'Message'
    &pushthisgv
    &pushbyte 2
    &pushsdb 27							//'qualifyName'
    &callFunction
    &add
    &pushsdb 28							//'&outline'
    &add
    &setMember
    &pushsdbgv 4							//'boxMC'
    &pushsdbgm 17							//'messageTF'
    &pushsdb 29							//'autoSize'
    &pushsdb 30							//'left'
    &setMember
    &pushsdb 9							//'messageHeight'
    &pushsdbgv 4							//'boxMC'
    &pushsdbgm 17							//'messageTF'
    &pushsdbgm 11							//'_height'
    &setVariable
    &pushzero
    &dcallfp 7							// LayoutBox()
    &pushsdb 21							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 31							//'FSCommand:'
    &pushsdb 32							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 27							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 3							//''
    &getURL2
   label4:
    &stop
  &end // of frame 0

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 8 // total frames: 2

    &placeMovieClip 7 &as 'imageMC'
    
      &onClipEvent &construct
        &constants '_type', 'RenderImage', '_imageMap', '', '_mode'  
        &pushsdb 0							//'_type'
        &pushsdb 1							//'RenderImage'
        &setVariable
        &pushsdb 2							//'_imageMap'
        &pushsdb 3							//''
        &setVariable
        &pushsdb 4							//'_mode'
        &pushsdb 3							//''
        &setVariable
      &end
    
      &onClipEvent &load
        &constants '_imageMap', 'Image', '_parent', 'qualifyName'  
        &pushsdb 0							//'_imageMap'
        &pushsdb 1							//'Image'
        &pushsdbgv 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushsdbgm 2							//'_parent'
        &pushbyte 2
        &dcallfsv 3							// qualifyName()
      &end
    &end // of placeMovieClip 7
  &end // of defineMovieClip 8

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &defineMovieClip 22 // total frames: 1
  &end // of defineMovieClip 22

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23
&end
