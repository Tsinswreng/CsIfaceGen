#if false

E:/_code/CsNgaq/Ngaq.Core/Ngaq.Core.csproj
dotnet run -- E:/_code/CsNgaq/Ngaq.Core/Ngaq.Core.csproj  ./Out
#endif
namespace Tsinswreng.CsIfaceGen.Cli;
using Tsinswreng.CsIfaceGen.SrcGen;
using System.IO;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

class Program {
	static async Task<int> Main(string[] args) {
		if (args.Length < 1) {
			Console.WriteLine("用法: mygen <project.csproj>");
			return 1;
		}

		var csprojPath = Path.GetFullPath(args[0]);
		var ProjDir = Path.GetDirectoryName(csprojPath);
		var cliArgOutDir = Path.GetFullPath(args[1]);

		var OutDir = cliArgOutDir;

		if(str.IsNullOrEmpty(cliArgOutDir)){
			OutDir = ToolGen.CombinePath(ProjDir??"", "./Tsinswreng.CsIfaceGen.Out");
		}

		// 初始化 MSBuild
		MSBuildLocator.RegisterDefaults();

		using var workspace = MSBuildWorkspace.Create();
		var project = await workspace.OpenProjectAsync(csprojPath);
		var compilation = await project.GetCompilationAsync();
		if (compilation == null) return 2;


		// async Task<nil> OnAddSrc(CtxCodeGenResult CtxCode, CT Ct){
		// 	//System.Console.WriteLine(CtxCode.HintName);
		// 	System.Console.WriteLine(CtxCode.Code);
		// 	//System.Console.WriteLine("====");
		// 	return NIL;
		// }

		async Task<nil> OnAddSrc(CtxCodeGenResult CtxCode, CT CT){
			var AttrArg = CtxCode.ArgIfaceGen;
			if(
				string.IsNullOrEmpty(AttrArg.OutDir)
				|| OutDir == null
			){
				// Logger.Append("----use AddSource----");//t
				// Logger.Append("OutDir: "+AttrArg.OutDir??"");//t
				// Logger.Append("ProjectDir: "+ProjectDir??"");//t
				// Logger.Append("Template: "+AttrArg.Template);//t
				// context.AddSource(CtxCode.HintName, CtxCode.Code);
			}else{
				var FullOutDir = ToolGen.CombinePath(OutDir, AttrArg.OutDir??"");
				var FullFileName = FullOutDir + "/" + AttrArg.FilePrefix + CtxCode.HintName;
				ToolGen.EnsureFile(FullFileName);
				// Logger.Append("FullOutDir: "+FullOutDir);//t
				// Logger.Append("FullFileName: "+FullFileName);//t
				#pragma warning disable RS1035
				File.WriteAllText(FullFileName, CtxCode.Code);
			}
			return NIL;
		}//~OnAddSrc

		var CfgGen = new CfgGen{
			FnAddSrc = OnAddSrc
		};

		var svcGen = new SvcGen();
		await svcGen.ExeAsy(compilation, CfgGen, new CT());

		return 0;
	}
}
