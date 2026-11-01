movie 'C:\projects\Ra3\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\LIBPRO~1\\LIBPRO~1.eaf' &compressed // flash 7, total frames: 2, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 4 // total frames: 61

    &frame 0
      &stop
    &end // of frame 0
  &end // of defineMovieClip 4

  &defineMovieClip 5 // total frames: 5

    &frame 0
      &constants '', 'Set Percent:', 'targetLevel', 'barFullSize', '_width', 'targetSize', '_root', 'GetExtern', 'SetPercent', 'Hello'  
      &function2 SetPercent (r:2='value') ()
        &pushsdb 0							//''
        &pushbyte 7
        &pushregister 2
        &pushzero
        &greaterThan
        &setProperty
        &pushsdb 1							//'Set Percent:'
        &pushregister 2
        &add
        &trace
        &pushsdb 2							//'targetLevel'
        &pushregister 2
        &setVariable
        &pushsdbgv 3							//'barFullSize'
        &pushsdbgm 4							//'_width'
        &setRegister r:1
        &pop
        &pushregister 1
        &pushbyte 100
        &divide
        &setRegister r:4
        &pop
        &pushsdbgv 2							//'targetLevel'
        &pushregister 1
        &divide
        &setRegister r:3
        &pop
        &pushsdb 5							//'targetSize'
        &pushregister 1
        &pushregister 3
        &multiply
        &pushregister 4
        &multiply
        &setVariable
        &gotoFrame 1
        &play
      &end // of function SetPercent

      &pushsdb 0							//''
      &pushbyte 13
      &getProperty
      &pushone
      &pushsdbgv 6							//'_root'
      &pushsdb 7							//'GetExtern'
      &callMethod
      &pushone
      &dcallfp 8							// SetPercent()
      &pushsdb 9							//'Hello'
      &trace
    &end // of frame 0

    &frame 3
      &constants 'barSize', 'bar', '_width', 'barScaler', 'targetSize', 'easeFactor'  
      &pushsdb 0							//'barSize'
      &pushsdbgv 1							//'bar'
      &pushsdbgm 2							//'_width'
      &setVariable
      &pushsdb 3							//'barScaler'
      &pushsdbgv 4							//'targetSize'
      &pushsdbgv 0							//'barSize'
      &subtract
      &setVariable
      &pushsdbgv 1							//'bar'
      &pushsdb 2							//'_width'
      &pushsdbgv 0							//'barSize'
      &pushsdbgv 3							//'barScaler'
      &pushsdbgv 5							//'easeFactor'
      &divide
      &add
      &setMember
    &end // of frame 3

    &frame 4
      &constants 'barSize', 'targetSize'  
      &pushsdbgv 0							//'barSize'
      &pushsdbgv 1							//'targetSize'
      &pushfloat 0.200000002980232
      &add
      &greaterThan
      &not
      &dup
      &not
      &jnz label1      
      &pop
      &pushsdbgv 0							//'barSize'
      &pushsdbgv 1							//'targetSize'
      &pushfloat 0.200000002980232
      &subtract
      &lessThan
      &not
     label1:
      &not
      &jnz label2      
      &stop
      &jmp label3      
     label2:
      &gotoLabel 'loop'
      &play
     label3:
    &end // of frame 4
  &end // of defineMovieClip 5
  
  &exportAssets
    5 &as 'loadBarProfile'
  &end // of exportAssets

  &defineMovieClip 8 // total frames: 7

    &frame 0
      &constants '', 'targetSize', 'targetPercent', 'textPercent', 'targetLevel', 'barSize', 'barFullSize', '_width', 'barSizeFactor', 'bar', '%', '&dropShadow', 'SetPercent'  
      &function2 SetPercent (r:1='to') ()
        &pushregister 1
        &pushzero
        &greaterThan
        &not
        &not
        &jnz label1        
        &pushsdb 0							//''
        &pushbyte 7
        &pushfalse
        &setProperty
        &pushsdb 1							//'targetSize'
        &pushzerosv
        &pushsdb 2							//'targetPercent'
        &pushzerosv
        &pushsdb 3							//'textPercent'
        &pushsdb 0							//''
        &setVariable
        &jmp label3        
       label1:
        &pushsdb 0							//''
        &pushbyte 7
        &pushtrue
        &setProperty
        &pushsdb 4							//'targetLevel'
        &pushregister 1
        &int
        &setVariable
        &pushsdbgv 4							//'targetLevel'
        &pushbyte 100
        &lessThan
        &not
        &not
        &jnz label2        
        &pushsdb 4							//'targetLevel'
        &pushbyte 100
        &setVariable
       label2:
        &pushsdb 5							//'barSize'
        &pushsdbgv 6							//'barFullSize'
        &pushsdbgm 7							//'_width'
        &setVariable
        &pushsdb 8							//'barSizeFactor'
        &pushsdbgv 5							//'barSize'
        &pushbyte 100
        &divide
        &setVariable
        &pushsdb 2							//'targetPercent'
        &pushsdbgv 4							//'targetLevel'
        &pushsdbgv 5							//'barSize'
        &divide
        &setVariable
        &pushsdb 1							//'targetSize'
        &pushsdbgv 5							//'barSize'
        &pushsdbgv 2							//'targetPercent'
        &multiply
        &pushsdbgv 8							//'barSizeFactor'
        &multiply
        &setVariable
        &pushsdbgv 9							//'bar'
        &pushsdb 7							//'_width'
        &pushsdbgv 1							//'targetSize'
        &setMember
        &pushsdb 3							//'textPercent'
        &pushsdbgv 4							//'targetLevel'
        &pushsdb 10							//'%'
        &add
        &pushsdb 11							//'&dropShadow'
        &add
        &setVariable
       label3:
      &end // of function SetPercent

      &pushbyte -1
      &pushone
      &dcallfp 12							// SetPercent()
      &stop
    &end // of frame 0
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as 'loadBarProgress'
  &end // of exportAssets
  
  &exportAssets
    8 &as 'loadBarProgress'
  &end // of exportAssets
  
  &exportAssets
    8 &as 'loadBarProgress'
  &end // of exportAssets
&end
