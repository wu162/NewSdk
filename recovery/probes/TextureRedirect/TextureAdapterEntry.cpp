#using "Managed.Foundation.netmodule"
#using "Managed.TextureAdapterProbe.netmodule"
using namespace System;
public ref class TextureAdapterEntry abstract sealed
{
public:
    static int Main(array<String^>^ args) { return TextureProbe::Run(args); }
};
