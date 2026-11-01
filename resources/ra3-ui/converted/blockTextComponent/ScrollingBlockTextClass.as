// WARNING: This class was decompiled without detecting uninitialized class fields.
class ScrollingBlockTextClass
{
   var m_step = 10;
   var m_totalLines = 20;
   var m_testStr = "Flash and APT support masking.\nIt would be implemented on the Flash / Art side in the manner you describe.\n\nThat is one way to go.\n\nThe problem with that method is that we would need one string token that contained all the objectives with embedded line breaks.\n\nScott - we discussed this a bit yesterday - you seemed to favor this method. Should we explore this method that Jefferson is suggesting or the method that I outlined in the email below? BTW I looked at the block text scrolling that we are doing on the 360 for the Terms and Conditions text - a C++ component (fscmds handles scrolling) is used to manage an index into the data which is mapped to a set of string tokens that have static positions on the screen. - didn\'t seem ideal for this application).";
   function ScrollingBlockTextClass()
   {
      trace("ScrollingBlockTextClass");
      ScrollingBlockTextClass.m_thisClass = this;
   }
   function onLoad()
   {
      trace("ScrollingBlockTextClass::onLoad()");
      ScrollingBlockTextClass.m_thisClass.setMaskSize();
      ScrollingBlockTextClass.m_thisClass.setLimits();
      ScrollingBlockTextClass.m_thisClass.upButton.onRelease = ScrollingBlockTextClass.m_thisClass.onUp;
      ScrollingBlockTextClass.m_thisClass.downButton.onRelease = ScrollingBlockTextClass.m_thisClass.onDown;
      ScrollingBlockTextClass.m_thisClass.setText(this.m_testStr);
   }
   function setMaskSize()
   {
      trace("ScrollingBlockTextClass::setMaskSize()");
      ScrollingBlockTextClass.m_thisClass.myText.maskMC._width = this.m_width;
      ScrollingBlockTextClass.m_thisClass.myText.maskMC._height = this.m_visibleLines * (this.m_lineHeight + this.m_lineSpacing);
   }
   function onUp()
   {
      trace("ScrollingBlockTextClass::onUp()");
      var _loc1_ = ScrollingBlockTextClass.m_thisClass.myText.blockTF._y + ScrollingBlockTextClass.m_thisClass.m_step;
      if(ScrollingBlockTextClass.m_thisClass.m_bottomLimit < _loc1_)
      {
         _loc1_ = ScrollingBlockTextClass.m_thisClass.m_bottomLimit;
      }
      ScrollingBlockTextClass.m_thisClass.myText.blockTF._y = _loc1_;
   }
   function onDown()
   {
      trace("ScrollingBlockTextClass::onDown()");
      var _loc1_ = ScrollingBlockTextClass.m_thisClass.myText.blockTF._y - ScrollingBlockTextClass.m_thisClass.m_step;
      if(_loc1_ < ScrollingBlockTextClass.m_thisClass.m_topLimit)
      {
         _loc1_ = ScrollingBlockTextClass.m_thisClass.m_topLimit;
      }
      ScrollingBlockTextClass.m_thisClass.myText.blockTF._y = _loc1_;
   }
   function setLimits()
   {
      this.m_topLimit = ScrollingBlockTextClass.m_thisClass.myText.maskMC._y - (this.m_totalLines - this.m_visibleLines + 1) * (this.m_lineHeight + this.m_lineSpacing);
      this.m_bottomLimit = ScrollingBlockTextClass.m_thisClass.myText.maskMC._y;
   }
   function setTotalLines(totalLines)
   {
      this.m_totalLines = totalLines;
   }
   function setText(theText)
   {
      ScrollingBlockTextClass.m_thisClass.myText.blockTF.text = theText;
   }
}
