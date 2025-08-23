#pragma warning disable RS1035
#define TswG_DEBUG
namespace Tsinswreng.CsIfaceGen.SrcGen;
using System.IO;

internal class Logger{
	//public static string Path = "./Tsinswreng.CsIfaceGen.log.txt"; //用相對路徑會在vscode目錄與項目目錄下分別生成日誌
	public static string Path = @"E:\_code\CsNgaq\Tsinswreng.CsIfaceGen.log.txt";
	public static void Append(string s){
#if TswG_DEBUG
		File.AppendAllText(Path, s+"\n");
#endif
	}

	public static void Write(string Path, string s){
#if TswG_DEBUG
		File.WriteAllText(Path, s);
#endif
	}

	public static void Debug(string Path, string s){
#if TswG_DEBUG
		var Base = @"E:\_code\CsNgaq\Tsinswreng.CsDictMapper.LogDir";
		Directory.CreateDirectory(Base);
		Path = $"{Base}/"+Path;
		File.WriteAllText(""+Path, s);
#endif
	}
}
