#using "Managed.Foundation.netmodule"
#using "Managed.TextureParallelProbe.netmodule"
using namespace System;
public ref class TextureParallelEntry abstract sealed
{
public:
    static int Main(array<String^>^ args) { return TextureParallelProbe::Main(args); }
};
