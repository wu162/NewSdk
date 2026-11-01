if(hideMainCount != undefined && hideMainCount++ == 0)
{
   main._visible = false;
}
stop();
fscommand(qualifyName(this,"OnFadeOutComplete"),"");
