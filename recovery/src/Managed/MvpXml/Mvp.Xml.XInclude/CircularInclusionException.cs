using System;

namespace Mvp.Xml.XInclude;

public class CircularInclusionException : FatalException
{
	public CircularInclusionException(Uri uri)
		: base(SR.GetString("CircularInclusion", uri.AbsoluteUri))
	{
	}

	public CircularInclusionException(Uri uri, string locationUri, int line, int position)
		: base(SR.GetString("CircularInclusionLong", uri.AbsoluteUri, locationUri, line, position))
	{
	}
}
