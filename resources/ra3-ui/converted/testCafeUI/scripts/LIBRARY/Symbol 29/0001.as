#initclip
class testCafeUI extends Cafe2_BaseUIScreen
{
   var setLineNumberTo;
   var toggleIconStatus;
   var m_simpleScrollingListData = new Array();
   var m_values = new Array();
   var m_dropDownMenuNames = ["Edit Menu","Copy","Cut","Paste","Replace","Find"];
   function testCafeUI(screen)
   {
      super();
      trace("testCafeUI::testCafeUI() screen = " + screen);
      Cafe2_BaseUIScreen.m_screen = screen;
      Cafe2_BaseUIScreen.m_thisClass = this;
      Cafe2_BaseUIScreen.m_thisClass.initGUI();
   }
   function initGUI()
   {
      trace("testCafeUI::initGUI() " + _global.gEH);
      Cafe2_BaseUIScreen.m_thisClass.createBackButton();
      Cafe2_BaseUIScreen.m_thisClass.createSimpleButton();
      Cafe2_BaseUIScreen.m_thisClass.createSimpleCheckBox();
      Cafe2_BaseUIScreen.m_thisClass.createSimpleScrollingList();
      Cafe2_BaseUIScreen.m_thisClass.createMultiColumnScrollList();
      Cafe2_BaseUIScreen.m_thisClass.createDropDownMenu();
      Cafe2_BaseUIScreen.m_thisClass.createEditableText();
      this.registerIntroOuttroComponents([Cafe2_BaseUIScreen.m_screen.uiButton,Cafe2_BaseUIScreen.m_screen.backButton]);
      this.startIntro();
      _global.gSM.setOnExitScreen(this.onScreenExit);
   }
   function createSimpleButton()
   {
      Cafe2_BaseUIScreen.m_screen.uiSlider1._visible = true;
      Cafe2_BaseUIScreen.m_screen.uiSlider1.logic_init();
      Cafe2_BaseUIScreen.m_screen.uiSlider1.setOnChange(Cafe2_BaseUIScreen.m_thisClass.slider1Change);
      Cafe2_BaseUIScreen.m_screen.uiSlider2._visible = true;
      Cafe2_BaseUIScreen.m_screen.uiSlider2.logic_init();
      Cafe2_BaseUIScreen.m_screen.uiSlider2.setOnChange(Cafe2_BaseUIScreen.m_thisClass.slider2Change);
      Cafe2_BaseUIScreen.m_screen.uiButton._visible = true;
      Cafe2_BaseUIScreen.m_screen.uiButton.enableGUI();
      Cafe2_BaseUIScreen.m_screen.uiButton.setOnFocus(Cafe2_BaseUIScreen.m_thisClass.buttonGotFocus);
      Cafe2_BaseUIScreen.m_screen.uiButton.setOnBlur(Cafe2_BaseUIScreen.m_thisClass.buttonLostFocus);
      Cafe2_BaseUIScreen.m_screen.uiButton.setOnMouseDownFunction(Cafe2_BaseUIScreen.m_thisClass.buttonClicked);
      Cafe2_BaseUIScreen.m_screen.uiButton.setIntroCallback(this.introComplete);
   }
   function createBackButton()
   {
      Cafe2_BaseUIScreen.m_screen.backButton._visible = true;
      Cafe2_BaseUIScreen.m_screen.backButton.enableGUI();
      Cafe2_BaseUIScreen.m_screen.backButton.setOnFocus(Cafe2_BaseUIScreen.m_thisClass.backButtonGotFocus);
      Cafe2_BaseUIScreen.m_screen.backButton.setOnBlur(Cafe2_BaseUIScreen.m_thisClass.backButtonLostFocus);
      Cafe2_BaseUIScreen.m_screen.backButton.setOnMouseUpFunction(Cafe2_BaseUIScreen.m_thisClass.backButtonClicked);
      Cafe2_BaseUIScreen.m_screen.backButton.setIntroCallback(this.introComplete);
   }
   function createSimpleCheckBox()
   {
      Cafe2_BaseUIScreen.m_screen.uiCheckBox._visible = true;
      Cafe2_BaseUIScreen.m_screen.uiCheckBox.enableGUI();
      Cafe2_BaseUIScreen.m_screen.uiCheckBox.setOnMouseDownFunction(Cafe2_BaseUIScreen.m_thisClass.checkBoxClicked);
      Cafe2_BaseUIScreen.m_screen.uiCheckBox.setIntroCallback(this.introComplete);
   }
   function createSimpleScrollingList()
   {
      Cafe2_BaseUIScreen.m_screen.uiScrollBar._visible = true;
      Cafe2_BaseUIScreen.m_screen.uiScrollBar.enableGUI();
      Cafe2_BaseUIScreen.m_screen.uiScrollBar.setOnFocus(Cafe2_BaseUIScreen.m_thisClass.scrollBarGotFocus);
      Cafe2_BaseUIScreen.m_screen.uiScrollBar.setOnBlur(Cafe2_BaseUIScreen.m_thisClass.scrollBarLostFocus);
      Cafe2_BaseUIScreen.m_screen.uiScrollBar.setOnChange(Cafe2_BaseUIScreen.m_thisClass.scrollChanaged,Cafe2_BaseUIScreen.m_thisClass.scrollThumbToolChanaged);
      this.m_values = [1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21];
      var _loc5_ = ["_1","_2"];
      var _loc7_ = ["_1","_2"];
      var _loc6_ = ["_1","_2"];
      var _loc2_ = 0;
      while(_loc2_ < this.m_values.length)
      {
         this.m_simpleScrollingListData.push("$GeneralData_" + _loc2_);
         _loc2_ = _loc2_ + 1;
      }
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.setNumLines(15);
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.setBufferSize(50);
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.setVerticalPadding(10);
      var _loc3_ = new Array();
      var _loc4_ = ["_1","_2"];
      _loc3_[0] = new scrollLineIconDef();
      _loc3_[0].init(_loc4_,"icon_1_Symbol");
      _loc3_[1] = new scrollLineIconDef();
      _loc3_[1].init(_loc4_,"icon_2_Symbol");
      _loc3_[2] = new scrollLineIconDef();
      _loc3_[2].init(_loc4_,"icon_3_Symbol");
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.init(_loc3_);
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.addLines(this.m_simpleScrollingListData,[_loc5_,_loc7_,_loc6_]);
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.enableGUI();
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.moveToFirst();
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.highlight();
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.refreshDisplay();
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.show();
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.setNotifyMouseDown(Cafe2_BaseUIScreen.m_thisClass.simpleScrollingListCallback);
   }
   function simpleScrollingListCallback(lineNumber, whoGotClicked)
   {
      trace("simpleScrollingListCallback " + this);
      var _loc2_ = this.setLineNumberTo(lineNumber);
      var _loc3_;
      if(whoGotClicked >= 1)
      {
         _loc3_ = this.toggleIconStatus(_loc2_,whoGotClicked);
      }
   }
   function initColData(colMC, theData, theStatus, iconDefArray)
   {
      colMC.setNumLines(10);
      colMC.setBufferSize(50);
      colMC.setVerticalPadding(10);
      colMC.init(iconDefArray);
      colMC.addLines(theData,theStatus);
      colMC.enableGUI();
      colMC.moveToFirst();
      colMC.highlight();
      colMC.refreshDisplay();
      colMC.show();
   }
   function createMultiColumnScrollList()
   {
      Cafe2_BaseUIScreen.m_screen.spreadsheetScrollBar._visible = true;
      Cafe2_BaseUIScreen.m_screen.spreadsheetScrollBar.enableGUI();
      Cafe2_BaseUIScreen.m_screen.spreadsheetScrollBar.setOnFocus(Cafe2_BaseUIScreen.m_thisClass.scrollBarGotFocus);
      Cafe2_BaseUIScreen.m_screen.spreadsheetScrollBar.setOnBlur(Cafe2_BaseUIScreen.m_thisClass.scrollBarLostFocus);
      Cafe2_BaseUIScreen.m_screen.spreadsheetScrollBar.setOnChange(Cafe2_BaseUIScreen.m_thisClass.multiColumnChanged,Cafe2_BaseUIScreen.m_thisClass.multiColumnThumbToolChanaged);
      var _loc10_ = ["_1","_2"];
      var _loc11_ = ["_1","_2"];
      var _loc15_ = new Array();
      var _loc14_ = new Array();
      var _loc8_ = 100;
      var _loc7_ = new Array();
      var _loc6_ = new Array();
      var _loc5_ = new Array();
      var _loc1_ = 0;
      while(_loc1_ < _loc8_)
      {
         _loc7_[_loc1_] = "Col_1_Line_" + String(_loc1_);
         _loc6_[_loc1_] = "C2_" + String(_loc1_);
         _loc5_[_loc1_] = "Col_3_" + String(_loc1_);
         _loc1_ = _loc1_ + 1;
      }
      var _loc9_ = new Array();
      var _loc12_ = ["_1","_2"];
      _loc9_[0] = new scrollLineIconDef();
      _loc9_[0].init(_loc12_,"icon_1_Symbol");
      _loc9_[1] = new scrollLineIconDef();
      _loc9_[1].init(_loc12_,"icon_2_Symbol");
      var _loc2_ = new scrollColumnDefinition();
      _loc2_.init();
      _loc2_.setData(_loc7_);
      _loc2_.setIconStates([_loc10_,_loc11_]);
      _loc2_.setNumOfVisColumn(20);
      _loc2_.setBufferSize(50);
      _loc2_.setVerticalPadding(10);
      _loc2_.setIconDefArray(_loc9_);
      _loc2_.setWidth(200);
      var _loc3_ = new scrollColumnDefinition();
      _loc3_.init();
      _loc3_.setData(_loc6_);
      _loc3_.setIconStates([_loc10_,_loc11_]);
      _loc3_.setNumOfVisColumn(20);
      _loc3_.setBufferSize(50);
      _loc3_.setVerticalPadding(10);
      _loc3_.setIconDefArray(_loc9_);
      _loc3_.setWidth(100);
      var _loc4_ = new scrollColumnDefinition();
      _loc4_.init();
      _loc4_.setData(_loc5_);
      _loc4_.setIconStates([_loc10_,_loc11_]);
      _loc4_.setNumOfVisColumn(20);
      _loc4_.setBufferSize(50);
      _loc4_.setVerticalPadding(10);
      _loc4_.setIconDefArray([]);
      _loc4_.setWidth(100);
      var _loc13_ = [_loc2_,_loc3_,_loc4_];
      Cafe2_BaseUIScreen.m_screen.multiCol_SS.init(_loc13_,"_parent.spreadsheetScrollBar");
      Cafe2_BaseUIScreen.m_screen.multiCol_SS.setNotifyMouseDown(Cafe2_BaseUIScreen.m_thisClass.multiColLineGotClicked);
   }
   function multiColLineGotClicked(theLine, whoGotClicked, colID)
   {
      trace(" M U L T I C O L U M N G O T C L I C K E D");
      var _loc1_ = Cafe2_BaseUIScreen.m_screen.multiCol_SS.setLineNumberTo(theLine);
      trace("theLine : " + _loc1_ + " whoGotClicked : " + whoGotClicked + " col ID = " + colID);
      if(whoGotClicked > 0)
      {
         Cafe2_BaseUIScreen.m_screen.multiCol_SS.toggleIconStatus(theLine,whoGotClicked,colID);
      }
   }
   function createDropDownMenu()
   {
      trace(" ADD THE DROP DOWN MENU " + Cafe2_BaseUIScreen.m_screen.dropDownMenuUI);
      Cafe2_BaseUIScreen.m_screen.dropDownMenuUI.addMenuItems(this.m_dropDownMenuNames);
      Cafe2_BaseUIScreen.m_screen.dropDownMenuUI.setNotifyMouseDown(Cafe2_BaseUIScreen.m_thisClass.dropDownMenuGotClicked);
      Cafe2_BaseUIScreen.m_screen.dropDownMenuUI.setIntroCallback(this.introComplete);
   }
   function createEditableText()
   {
      trace(" ADD THE editable text field " + Cafe2_BaseUIScreen.m_screen.uiEditableTextField);
      Cafe2_BaseUIScreen.m_screen.uiEditableTextField._visible = true;
      Cafe2_BaseUIScreen.m_screen.uiEditableTextField.enableGUI();
      Cafe2_BaseUIScreen.m_screen.uiEditableTextField.setOnFocus(Cafe2_BaseUIScreen.m_thisClass.editTextGotFocus);
      Cafe2_BaseUIScreen.m_screen.uiEditableTextField.setOnBlur(Cafe2_BaseUIScreen.m_thisClass.editTextLostFocus);
      Cafe2_BaseUIScreen.m_screen.uiEditableTextField.setOnMouseDownFunction(Cafe2_BaseUIScreen.m_thisClass.editTextClicked);
      Cafe2_BaseUIScreen.m_screen.uiEditableTextField.setIntroCallback(this.introComplete);
      trace("END ADD");
   }
   function colLineGotClicked(theLine, whoGotClicked)
   {
      Cafe2_BaseUIScreen.m_screen.col_1.setLineNumberTo(theLine);
      var _loc2_ = Cafe2_BaseUIScreen.m_screen.col_2.setLineNumberTo(theLine);
      var _loc3_;
      if(whoGotClicked > 0)
      {
         _loc3_ = this.toggleIconStatus(_loc2_,whoGotClicked);
      }
   }
   function onScreenExit()
   {
      trace("testCafeUI::onScreenExit()");
      delete _global.testCafeUI;
   }
   function slider1Change()
   {
      trace("testCafeUI::slider1Change");
      var _loc1_ = uiSlider1.getValue();
      trace("New Value of Slider 1 is = " + String(_loc1_));
   }
   function slider2Change()
   {
      trace("testCafeUI::slider2Change");
      var _loc1_ = uiSlider2.getValue();
      trace("New Value of Slider 2 is = " + String(_loc1_));
   }
   function buttonGotFocus()
   {
      trace("testCafeUI::button got focus");
   }
   function buttonLostFocus()
   {
      trace("testCafeUI::button lost focus");
   }
   function buttonClicked()
   {
      trace("testCafeUI::button got mouse down : now do something useful for the screen");
   }
   function scrollBarGotFocus()
   {
      trace("testCafeUI::scrollBar got focus");
   }
   function scrollBarLostFocus()
   {
      trace("testCafeUI::scrollBar lost focus");
   }
   function scrollChanaged(dataChangeDirection)
   {
      switch(dataChangeDirection)
      {
         case 0:
            Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.moveUp();
            break;
         case 1:
            Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.moveDown();
         default:
            return;
      }
   }
   function scrollThumbToolChanaged(percentChange)
   {
      Cafe2_BaseUIScreen.m_screen.uiScrollingTextBox.setTo(percentChange);
   }
   function checkBoxClicked()
   {
      trace("testCafeUI::button got mouse down : now do something useful for the screen");
   }
   function backButtonGotFocus()
   {
      trace("testCafeUI::Back button got focus");
   }
   function backButtonLostFocus()
   {
      trace("testCafeUI::Back button lost focus");
   }
   function multiColumnChanged(dataChangedDirection, who)
   {
      switch(dataChangedDirection)
      {
         case 0:
            Cafe2_BaseUIScreen.m_screen.multiCol_SS.moveUp();
            break;
         case 1:
            Cafe2_BaseUIScreen.m_screen.multiCol_SS.moveDown();
         default:
            return;
      }
   }
   function multiColumnThumbToolChanaged(percentChange)
   {
      Cafe2_BaseUIScreen.m_screen.multiCol_SS.setTo(percentChange);
   }
   function dropDownMenuGotClicked(menuItemIndex)
   {
      trace("The drop down menu (EDIT) got clicked : The menu item selected is " + Cafe2_BaseUIScreen.m_screen.m_dropDownMenuNames[menuItemIndex]);
   }
   function editTextGotFocus()
   {
      trace("text field focus");
   }
   function editTextLostFocus()
   {
      trace("text field lost focus");
   }
   function editTextClicked()
   {
      trace("text field clicked");
   }
}
#endinitclip
stop();
