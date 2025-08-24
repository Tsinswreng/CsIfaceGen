namespace Tsinswreng.CsIfaceGen.SrcGen;
using static ToolGen;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

[Generator]
public sealed class IfaceGenerator : ISourceGenerator {
	public void Initialize(GeneratorInitializationContext context) {
		// 无需额外初始化
	}


	public void Execute(GeneratorExecutionContext context) {
		if (context.AnalyzerConfigOptions.GlobalOptions
			.TryGetValue("build_property.ProjectDir", out var ProjectDir)
		){
			//Logger.Append(ProjectDir);
		}

		async Task<nil> OnAddSrc(CtxCodeGenResult CtxCode, CT CT){
			var AttrArg = CtxCode.ArgIfaceGen;
			if(
				string.IsNullOrEmpty(AttrArg.OutDir)
				|| ProjectDir == null
			){
				Logger.Append("----use AddSource----");//t
				Logger.Append("OutDir: "+AttrArg.OutDir??"");//t
				Logger.Append("ProjectDir: "+ProjectDir??"");//t
				Logger.Append("Template: "+AttrArg.Template);//t
				context.AddSource(CtxCode.HintName, CtxCode.Code);
			}else{
				var FullOutDir = CombinePath(ProjectDir, AttrArg.OutDir??"");
				var FullFileName = FullOutDir + "/" + CtxCode.HintName;
				ToolGen.EnsureFile(FullFileName);
				// Logger.Append("FullOutDir: "+FullOutDir);//t
				// Logger.Append("FullFileName: "+FullFileName);//t
				#pragma warning disable RS1035
				System.IO.File.WriteAllText(FullFileName, CtxCode.Code);
			}
			return NIL;
		}//~OnAddSrc

		var Compilation = context.Compilation;

		var Cfg = new CfgGen{
			FnAddSrc = OnAddSrc,
		};

		var svc = new SvcGen();
		svc.ExeAsy(Compilation, Cfg, context.CancellationToken).Wait();

	}

}


