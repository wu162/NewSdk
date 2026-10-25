namespace Mvp.Xml.XPointer;

internal class LexUtils
{
	private const int FSTARTNAME = 1;

	private const int FNAME = 2;

	private const string _NameChars = "-.0:AZ__az··ÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁːˑ\u0300\u0345\u0360\u0361ΆΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁ\u0483\u0486ҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆ\u0591\u05a1\u05a3\u05b9\u05bb\u05bd\u05bf\u05bf\u05c1\u05c2\u05c4\u05c4אתװײءغـ\u0652٠٩\u0670ڷںھۀێېۓە\u06e8\u06ea\u06ed۰۹\u0901\u0903अह\u093c\u094d\u0951\u0954क़\u0963०९\u0981\u0983অঌএঐওনপরললশহ\u09bc\u09bc\u09be\u09c4\u09c7\u09c8\u09cb\u09cd\u09d7\u09d7ড়ঢ়য়\u09e3০ৱ\u0a02\u0a02ਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹ\u0a3c\u0a3c\u0a3e\u0a42\u0a47\u0a48\u0a4b\u0a4dਖ਼ੜਫ਼ਫ਼੦ੴ\u0a81\u0a83અઋઍઍએઑઓનપરલળવહ\u0abc\u0ac5\u0ac7\u0ac9\u0acb\u0acdૠૠ૦૯\u0b01\u0b03ଅଌଏଐଓନପରଲଳଶହ\u0b3c\u0b43\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57ଡ଼ଢ଼ୟୡ୦୯\u0b82ஃஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹ\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd\u0bd7\u0bd7௧௯\u0c01\u0c03అఌఎఐఒనపళవహ\u0c3e\u0c44\u0c46\u0c48\u0c4a\u0c4d\u0c55\u0c56ౠౡ౦౯\u0c82\u0c83ಅಌಎಐಒನಪಳವಹ\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd\u0cd5\u0cd6ೞೞೠೡ೦೯\u0d02\u0d03അഌഎഐഒനപഹ\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0d57\u0d57ൠൡ൦൯กฮะ\u0e3aเ\u0e4e๐๙ກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະ\u0eb9\u0ebbຽເໄໆໆ\u0ec8\u0ecd໐໙\u0f18\u0f19༠༩\u0f35\u0f35\u0f37\u0f37\u0f39\u0f39\u0f3eཇཉཀྵ\u0f71\u0f84\u0f86ྋ\u0f90\u0f95\u0f97\u0f97\u0f99\u0fad\u0fb1\u0fb7\u0fb9\u0fb9ႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼ\u20d0\u20dc\u20e1\u20e1ΩΩKÅ℮℮ↀↂ々々〇〇〡\u302f〱〵ぁゔ\u3099\u309aゝゞァヺーヾㄅㄬ一龥가힣";

	private const string _NameStartChars = "::AZ__azÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁΆΆΈΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆאתװײءغفيٱڷںھۀێېۓەەۥۦअहऽऽक़ॡঅঌএঐওনপরললশহড়ঢ়য়ৡৰৱਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹਖ਼ੜਫ਼ਫ਼ੲੴઅઋઍઍએઑઓનપરલળવહઽઽૠૠଅଌଏଐଓନପରଲଳଶହଽଽଡ଼ଢ଼ୟୡஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹఅఌఎఐఒనపళవహౠౡಅಌಎಐಒನಪಳವಹೞೞೠೡഅഌഎഐഒനപഹൠൡกฮะะาำเๅກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະະາຳຽຽເໄཀཇཉཀྵႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼΩΩKÅ℮℮ↀↂ〇〇〡〩ぁゔァヺㄅㄬ一龥가힣";

	private static byte[] _chars;

	static LexUtils()
	{
		_chars = new byte[65536];
		SetChars("::AZ__azÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁΆΆΈΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆאתװײءغفيٱڷںھۀێېۓەەۥۦअहऽऽक़ॡঅঌএঐওনপরললশহড়ঢ়য়ৡৰৱਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹਖ਼ੜਫ਼ਫ਼ੲੴઅઋઍઍએઑઓનપરલળવહઽઽૠૠଅଌଏଐଓନପରଲଳଶହଽଽଡ଼ଢ଼ୟୡஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹఅఌఎఐఒనపళవహౠౡಅಌಎಐಒನಪಳವಹೞೞೠೡഅഌഎഐഒനപഹൠൡกฮะะาำเๅກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະະາຳຽຽເໄཀཇཉཀྵႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼΩΩKÅ℮℮ↀↂ〇〇〡〩ぁゔァヺㄅㄬ一龥가힣", 1);
		SetChars("-.0:AZ__az··ÀÖØöøıĴľŁňŊžƀǃǍǰǴǵǺȗɐʨʻˁːˑ\u0300\u0345\u0360\u0361ΆΊΌΌΎΡΣώϐϖϚϚϜϜϞϞϠϠϢϳЁЌЎяёќўҁ\u0483\u0486ҐӄӇӈӋӌӐӫӮӵӸӹԱՖՙՙաֆ\u0591\u05a1\u05a3\u05b9\u05bb\u05bd\u05bf\u05bf\u05c1\u05c2\u05c4\u05c4אתװײءغـ\u0652٠٩\u0670ڷںھۀێېۓە\u06e8\u06ea\u06ed۰۹\u0901\u0903अह\u093c\u094d\u0951\u0954क़\u0963०९\u0981\u0983অঌএঐওনপরললশহ\u09bc\u09bc\u09be\u09c4\u09c7\u09c8\u09cb\u09cd\u09d7\u09d7ড়ঢ়য়\u09e3০ৱ\u0a02\u0a02ਅਊਏਐਓਨਪਰਲਲ਼ਵਸ਼ਸਹ\u0a3c\u0a3c\u0a3e\u0a42\u0a47\u0a48\u0a4b\u0a4dਖ਼ੜਫ਼ਫ਼੦ੴ\u0a81\u0a83અઋઍઍએઑઓનપરલળવહ\u0abc\u0ac5\u0ac7\u0ac9\u0acb\u0acdૠૠ૦૯\u0b01\u0b03ଅଌଏଐଓନପରଲଳଶହ\u0b3c\u0b43\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57ଡ଼ଢ଼ୟୡ୦୯\u0b82ஃஅஊஎஐஒகஙசஜஜஞடணதநபமவஷஹ\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd\u0bd7\u0bd7௧௯\u0c01\u0c03అఌఎఐఒనపళవహ\u0c3e\u0c44\u0c46\u0c48\u0c4a\u0c4d\u0c55\u0c56ౠౡ౦౯\u0c82\u0c83ಅಌಎಐಒನಪಳವಹ\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd\u0cd5\u0cd6ೞೞೠೡ೦೯\u0d02\u0d03അഌഎഐഒനപഹ\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0d57\u0d57ൠൡ൦൯กฮะ\u0e3aเ\u0e4e๐๙ກຂຄຄງຈຊຊຍຍດທນຟມຣລລວວສຫອຮະ\u0eb9\u0ebbຽເໄໆໆ\u0ec8\u0ecd໐໙\u0f18\u0f19༠༩\u0f35\u0f35\u0f37\u0f37\u0f39\u0f39\u0f3eཇཉཀྵ\u0f71\u0f84\u0f86ྋ\u0f90\u0f95\u0f97\u0f97\u0f99\u0fad\u0fb1\u0fb7\u0fb9\u0fb9ႠჅაჶᄀᄀᄂᄃᄅᄇᄉᄉᄋᄌᄎᄒᄼᄼᄾᄾᅀᅀᅌᅌᅎᅎᅐᅐᅔᅕᅙᅙᅟᅡᅣᅣᅥᅥᅧᅧᅩᅩᅭᅮᅲᅳᅵᅵᆞᆞᆨᆨᆫᆫᆮᆯᆷᆸᆺᆺᆼᇂᇫᇫᇰᇰᇹᇹḀẛẠỹἀἕἘἝἠὅὈὍὐὗὙὙὛὛὝὝὟώᾀᾴᾶᾼιιῂῄῆῌῐΐῖΊῠῬῲῴῶῼ\u20d0\u20dc\u20e1\u20e1ΩΩKÅ℮℮ↀↂ々々〇〇〡\u302f〱〵ぁゔ\u3099\u309aゝゞァヺーヾㄅㄬ一龥가힣", 2);
	}

	private static void SetChars(string ranges, byte value)
	{
		for (int i = 0; i < ranges.Length; i += 2)
		{
			int j = ranges[i];
			for (int num = ranges[i + 1]; j <= num; j++)
			{
				_chars[j] |= value;
			}
		}
	}

	public static bool IsWhitespace(char ch)
	{
		if (ch <= ' ')
		{
			return ((4294981120L >> (int)ch) & 1) != 0;
		}
		return false;
	}

	public static bool IsStartNameChar(char ch)
	{
		return (_chars[(uint)ch] & 1) != 0;
	}

	public static bool IsStartNCNameChar(char ch)
	{
		if (IsStartNameChar(ch))
		{
			return ch != ':';
		}
		return false;
	}

	public static bool IsNameChar(char ch)
	{
		return (_chars[(uint)ch] & 2) != 0;
	}

	public static bool IsNCNameChar(char ch)
	{
		if (IsNameChar(ch))
		{
			return ch != ':';
		}
		return false;
	}
}
