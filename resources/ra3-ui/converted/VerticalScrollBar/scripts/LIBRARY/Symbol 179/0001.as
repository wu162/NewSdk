function SetHeight(newHeight)
{
   var _loc1_ = topPiece._height + bottomPiece._height;
   if(newHeight < _loc1_)
   {
      newHeight = _loc1_;
   }
   middlePiece._height = newHeight - _loc1_;
   bottomPiece._y = middlePiece._y + middlePiece._height;
   bttnHeight = newHeight;
   if(bttn != undefined)
   {
      bttn._height = bttnHeight;
   }
}
function HandlePress()
{
   gotoAndStop("_down");
   play();
   if(ourMouseListener == undefined)
   {
      dragStartY = _Y;
      dragStartYMouse = _root._ymouse;
      this.createEmptyMovieClip("ourMouseListener",0);
      ourMouseListener.onMouseMove = function()
      {
         this._parent.HandleMouseMove();
      };
      ourMouseListener.onMouseUp = function()
      {
         delete this._parent.ourMouseListener;
         this.removeMovieClip();
      };
   }
}
function HandleRelease()
{
   gotoAndStop("_over");
   play();
}
function HandleRollOver()
{
   gotoAndStop("_over");
   play();
}
function HandleRollOut()
{
   gotoAndStop("_up");
   play();
}
function HandleMouseMove()
{
   _parent.OnThumbDrag(this,dragStartY + (_root._ymouse - dragStartYMouse));
}
function onUnload()
{
   if(ourMouseListener != undefined)
   {
      ourMouseListener.removeMovieClip();
      delete ourMouseListener;
   }
   delete SetHeight;
   delete HandlePress;
   delete HandleRelease;
   delete HandleRollOver;
   delete HandleRollOut;
   delete HandleMouseMove;
   delete onUnload;
}
topPiece.gotoAndPlay("_disabled");
middlePiece.gotoAndPlay("_disabled");
bottomPiece.gotoAndPlay("_disabled");
