// Original Module blob 2d2f471c8008b7c2e1f78e39752d0726da395cea.
std::map<std::string,Layout> MakeLayouts() { return {
    {"Vector2",{8,{{"x",0,Kind::Float,"0.0",""},{"y",4,Kind::Float,"0.0",""}},{}}},
    {"PackedColor",{4,{},{}}},
    {"FontDesc",{16,{{"Name",0,Kind::String,nullptr,""},{"Size",8,Kind::Unsigned,nullptr,""},{"Bold",12,Kind::Bool,"false",""}},{}}},
    {"HotKeySlot",{24,{{"Group",4,Kind::Enum,"Misc",""},{"Index",8,Kind::Signed,"-1",""},{"Name",12,Kind::String,nullptr,""},{"VersionAdded",20,Kind::Signed,"1",""}},{}}},
    {"MouseCursor",{16,{{"File",4,Kind::String,nullptr,""},{"NumDirections",12,Kind::Signed,"1",""}},{}}},
    {"BasicMouseCursors",{20,{{"Normal",4,Kind::Reference,nullptr,""},{"Arrow",8,Kind::Reference,nullptr,""},{"Invalid",12,Kind::Reference,nullptr,""},{"EnterText",16,Kind::Reference,nullptr,""}},{}}},
    {"UIMouseDragSelectionFeedback",{20,{{"BoxEdgeWidth",4,Kind::Float,"2",""},{"LassoWidth",8,Kind::Float,"2",""}},{{"BoxColor","PackedColor",12,ChildKind::Inline},{"LassoColor","PackedColor",16,ChildKind::Inline}}}},
    {"UIFloatingTextDisplay",{32,{{"TimeOut",4,Kind::Time,"333ms",""},{"MoveUpSpeed",8,Kind::Velocity,"1.0",""},{"MoveVanishRate",12,Kind::Velocity,"0.1",""}},{{"Font","FontDesc",16,ChildKind::Inline}}}},
    {"UIMessageDisplay",{32,{{"Delay",4,Kind::Time,"5s",""}},{{"Color1","PackedColor",8,ChildKind::Inline},{"Color2","PackedColor",12,ChildKind::Inline},{"Font","FontDesc",16,ChildKind::Inline}}}},
    {"UIMilitaryCaptionDisplay",{56,{{"Centered",52,Kind::Bool,"false",""},{"Delay",4,Kind::Time,"750ms",""}},{{"Color","PackedColor",8,ChildKind::Inline},{"Position","Vector2",12,ChildKind::Inline},{"TitleFont","FontDesc",20,ChildKind::Inline},{"Font","FontDesc",36,ChildKind::Inline}}}},
    {"UISubtitleDisplay",{36,{{"HoldTime",4,Kind::Time,"20s",""},{"LineCount",8,Kind::Signed,"8",""}},{{"Font","FontDesc",12,ChildKind::Inline},{"PanelColor","PackedColor",28,ChildKind::Inline},{"TextColor","PackedColor",32,ChildKind::Inline}}}},
    {"UITimerDisplay",{64,{{"Centered",60,Kind::Bool,"false",""},{"FlashDuration",4,Kind::Time,"1s",""}},{{"Position","Vector2",8,ChildKind::Inline},{"FlashColor","PackedColor",16,ChildKind::Inline},{"NormalFont","FontDesc",20,ChildKind::Inline},{"NormalColor","PackedColor",36,ChildKind::Inline},{"ReadyFont","FontDesc",40,ChildKind::Inline},{"ReadyColor","PackedColor",56,ChildKind::Inline}}}},
}; }
