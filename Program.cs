using System;
using System.Text;

#pragma warning disable IDE0090
#pragma warning disable IDE0057
#pragma warning disable CA1862

class Program
{
	readonly static int SEED = ((DateTime.Now.Microsecond & 65533) << 8) | (DateTime.Now.Millisecond & 65533);
	readonly static Random RNG = new Random(SEED);

	public static void Main(string[] args)
	{
		const string defaultExclusion = "\"\'";
		string exclusionChars = "";
		int generatePassCount = 0;
		int generatePassLength = 16;
		if(args.Length == 0)
		{
			Console.WriteLine("ヘルプが必要なときは--helpオプションを付けて実行してください");
			return;
		}

		for (int i = 0; i < args.Length; i++)
		{
			string currentArg = args[i].Trim();
			if (currentArg[0] == '/' || currentArg[0] == '-')
			{
				// コマンドの文字省略無効の時
				if (currentArg[1] == '-')
				{
					if (currentArg.Substring(2).ToLower() == "exclude" && (i + 1) < args.Length)
					{
						exclusionChars = args[i + 1];
					}
					else if (currentArg.Substring(2).ToLower() == "count" && (i + 1) < args.Length)
					{
						generatePassCount = int.Parse(args[i + 1]);
					}
					else if (currentArg.Substring(2).ToLower() == "length" && (i + 1) < args.Length)
					{
						generatePassLength = int.Parse(args[i + 1]);
					}
					else if (currentArg.Substring(2).ToLower() == "help")
					{
						WriteLineUsage();
						return;
					}
					else
					{
						Console.WriteLine("無効なオプションが指定されました");
						Console.WriteLine("以下のオプションでヘルプを確認し、オプションが正しいか確認してください");
						Console.WriteLine("PassGen /?");
						Console.WriteLine("PassGen /H");
						Console.WriteLine("PassGen --help");
						return;
					}
				}

				if (char.ToUpper(currentArg[1]) == 'E' && (i + 1) < args.Length)
				{
					exclusionChars = args[i + 1];
				}
				else if (char.ToUpper(currentArg[1]) == 'C' && (i + 1) < args.Length)
				{
					generatePassCount = int.Parse(args[i + 1]);
				}
				else if (char.ToUpper(currentArg[1]) == 'L' && (i + 1) < args.Length)
				{
					generatePassLength = int.Parse(args[i + 1]);
				}
				else if (char.ToUpper(currentArg[1]) == 'H' || char.ToUpper(currentArg[1]) == '?')
				{
					WriteLineUsage();
					return;
				}
			}
		}

		// ここでパスワード生成
		// 使って良い文字列は、数字・アルファベット・除外されていない記号 のみ(日本語や変形したローマ字などは無効)
		const char minRange = '!';
		const char maxRange = '~';
		for (int pgi = 0; pgi < generatePassCount; pgi++)
		{
			// 取り敢えず16文字
			StringBuilder pass = new StringBuilder("");
			for (ushort i = 0; i < generatePassLength; i++)
			{
				pass.Append(RandomChar(minRange, maxRange, defaultExclusion + exclusionChars));
			}
			Console.WriteLine("[info] Gen Pass! \"" + pass.ToString() + "\"");
		}
	}

	public static char RandomChar(char minRange, char maxRange, string exclusionChars = "")
	{
		char tmp = (char)RNG.Next(minRange, maxRange);
		// ここで除外する文字を含んでいるかチェック 含まれていたなら再起
		for (int i = 0; i < exclusionChars.Length; i++)
		{
			if (tmp == exclusionChars[i])
			{
				tmp = RandomChar(minRange, maxRange, exclusionChars);
			}
		}

		return tmp;
	}

	public static void WriteLineUsage()
	{
		Console.WriteLine("Usage: passgen [<commands>...]");
		Console.WriteLine("commands:");
		Console.WriteLine("  -E|--exclude [default:\"\']  Specify characters to exclude.");
		Console.WriteLine("                               除外する文字を指定します。");
		Console.WriteLine("  -C|--count   [default: 1]    Specify the number of passwords to generate.");
		Console.WriteLine("                               生成するパスワードの数を指定します。");
		Console.WriteLine("  -L|--length  [default: 16]   Specify the number of characters for the generated password.");
		Console.WriteLine("                               生成するパスワードの文字数を指定します。");
	}
}
#pragma warning restore CA1862
#pragma warning restore IDE0057
#pragma warning restore IDE0090

// MEMO *^_^*
// Usage: passgen [<commands>...]
// commands:
//   -E|--exclude　<number>	除外する文字を指定します[default: " ']
//   -C|--count	<number>	生成するパスワードの数を指定します[default: 1]
//   -L|--length <string>	生成するパスワードの文字数を指定します[default: 16]

// Usage: passgen [<commands>...]
// commands:
//   -E|--exclude　	<number>[default: " ']	Specify characters to exclude.
// 											除外する文字を指定します。
//   -C|--count		<number>[default: 1]	Specify the number of passwords to generate.
// 											生成するパスワードの数を指定します。
//   -L|--length 	<string>[default: 16]	Specify the number of characters for the generated password.
// 											生成するパスワードの文字数を指定します。
//  dotnet publish -c Release -r win-x64 -o "..\binary\PassGen\"
