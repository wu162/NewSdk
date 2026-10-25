    static void InitializedModuleRandom(XmlNode^ node,String^ name,unsigned int offset,Native::XmlfxparticleModuleInput& input) {
        Native::XmlfxparticleModuleRandom field; field.offset=offset;
        field.input.present=true; field.input.type="UNIFORM"; field.input.low="1.0"; field.input.high="1.0";
        XmlNode^ child=Child(node,name);
        if(child!=nullptr) {
            Attributes(child,gcnew array<String^>{"Type","Low","High"}); Empty(child);
            field.input.type=Attr(child,"Type","UNIFORM");
            field.input.low=Attr(child,"Low","1.0"); field.input.high=Attr(child,"High","1.0");
        }
        input.random.push_back(field);
    }
    static void ModuleRandCoord(XmlNode^ node,String^ name,unsigned int offset,Native::XmlfxparticleModuleInput& input) {
        XmlNode^ child=Child(node,name); if(child==nullptr) return;
        Attributes(child,gcnew array<String^>{}); Children(child,gcnew array<String^>{"x","y","z"},false);
        array<String^>^ axes={"x","y","z"};
        for(int i=0;i<3;++i) ModuleRandom(child,axes[i],offset+12*i,input);
    }
    static void UniqueModuleChild(XmlNode^ node,String^ name) {
        int count=0;
        for each(XmlNode^ child in node->ChildNodes) if(child->NodeType==XmlNodeType::Element && child->LocalName==name && ++count>1)
            throw gcnew NotSupportedException("Duplicate module child: "+name);
    }
