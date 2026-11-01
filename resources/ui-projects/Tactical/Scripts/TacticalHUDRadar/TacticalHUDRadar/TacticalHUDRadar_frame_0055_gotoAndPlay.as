if(_parent.done == undefined || !_parent.done)
{
   _parent.gotoAndPlay(1);
}
else if(_parent._parent.initialized)
{
   _parent._parent.DestroyPing(_parent);
}
else
{
   _parent.removeMovieClip();
}
