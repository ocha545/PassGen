using System;
using System.IO;
using System.Text;
using System.Reflection;

#pragma warning disable IDE0090
#pragma warning disable IDE0057
#pragma warning disable CA1862

class Program
{
	readonly static int SEED = ((DateTime.Now.Microsecond & 65533) << 8) | (DateTime.Now.Millisecond & 65533);
	readonly static Random RNG = new Random(SEED);
	readonly static string VERSION = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

	public static void Main(string[] args)
	{
		const string defaultExclusion = "\"\'";
		string exclusionChars = "";
		string outputFilepath = string.Empty;
		int generatePassCount = 0;
		int generatePassLength = 16;
		if (args.Length == 0)
		{
			Console.WriteLine("PassGen Version: " + VERSION + " !");
			Console.WriteLine("Copyright (C) 2026 ocha--. All rights reserved.");
			Console.WriteLine("このコマンドでは文字数指定や、除外する文字の指定を行い、パスワードを生成することが出来ます。");
			Console.WriteLine("詳しいコマンドは　PassGen　--help を実行し、ご確認ください。");
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
					else if (currentArg.Substring(2).ToLower() == "output")
					{
						outputFilepath = args[i + 1];
					}
					else if (currentArg.Substring(2).ToLower() == "version")
					{
						Console.WriteLine("PassGen Version: " + VERSION);
					}
					else if (currentArg.Substring(2).ToLower() == "help")
					{
						WriteLineUsage();
						return;
					}
					else
					{
						Console.WriteLine("無効なオプションが指定されました: " + currentArg.Substring(2).ToLower());
						Console.WriteLine("ヘルプを確認して正しいオプションを指定してください");
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
				else if (char.ToUpper(currentArg[1]) == 'O' && (i + 1) < args.Length)
				{
					outputFilepath = args[i + 1];
				}
				else if (char.ToUpper(currentArg[1]) == 'V')
				{
					Console.WriteLine("PassGen Version: " + VERSION);
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

		StringBuilder allPass = new StringBuilder("");
		for (int pgi = 0; pgi < generatePassCount; pgi++)
		{
			// 取り敢えず16文字
			StringBuilder pass = new StringBuilder("");
			for (ushort i = 0; i < generatePassLength; i++)
			{
				pass.Append(RandomChar(minRange, maxRange, defaultExclusion + exclusionChars));
			}

			allPass.AppendLine(pass.ToString());
		}

		if (outputFilepath == string.Empty)
		{
			Console.WriteLine(allPass.ToString());
		}
		else
		{
			WritePassFile(outputFilepath, allPass.ToString());
		}
	}

	public static void WritePassFile(string path, string passwords)
	{
		if (File.Exists(path))
		{
			Console.WriteLine("同じファイルが見つかったので以下の選択肢から操作を選択してください");
			Console.WriteLine("追記する(a) 上書きする(o) 中止する(s)");
			Console.Write(" > ");
			string input = Console.ReadLine() ?? string.Empty;
			if (input == "a")
			{
				using StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
				sw.WriteLine(passwords);
				sw.Flush();
				Console.WriteLine("追記を行います");
			}
			else if (input == "o")
			{
				using StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
				sw.WriteLine(passwords);
				sw.Flush();
				Console.WriteLine("上書きします");
			}
			else if (input == "s")
			{
				Console.WriteLine("中止します");
				return;
			}
			else
			{
				Console.WriteLine("操作が選択されなかったか、無効な操作が選択されました");
				Console.WriteLine("a, o, s　のいずれかを選択してください");
			}
		}
		else
		{
			using StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
			sw.WriteLine(passwords);
			sw.Flush();
			return;
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
		Console.WriteLine("  -V|--version                 Displays the version.");
		Console.WriteLine("                               バージョンを表示します");
		Console.WriteLine("  -H|--help                    Displays help.");
		Console.WriteLine("                               ヘルプを表示します");
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
