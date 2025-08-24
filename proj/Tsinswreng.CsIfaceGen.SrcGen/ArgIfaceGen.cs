namespace Tsinswreng.CsIfaceGen.SrcGen;

using System;
using Microsoft.CodeAnalysis;

public class ArgIfaceGen
	:IArgIfaceGen
{
	public INamedTypeSymbol ParentType{get;set;}
	//ph: placeholder
	public str? PhFullType{get;set;}
	/// <summary>
	/// 去掉global::, 「_」替換成「__」,「.」,尖括號 替換成「_」
	/// 不支持泛型與nullable
	/// </summary>
	public str? PhIdentifierSafeFullType{get;set;}
	public str Template{get;set;} = "";
	/// <summary>
	/// 相對于項目根目錄之輸出目錄
	/// 若指定則寫文件至相應目錄ⁿ不用AddSource
	/// 開頭不用加/
	/// </summary>
	public str? OutDir{get;set;}
	public str? FilePrefix{get;set;}


	public static IArgIfaceGen GetArg(
		AttributeData Attr
	){
		IArgIfaceGen R = new ArgIfaceGen();
		foreach (var namedArg in Attr.NamedArguments){
			var name = namedArg.Key;
			var value = namedArg.Value.Value;
			if(name == nameof(IfaceGen.PhFullType)){
				R.PhFullType = value?.ToString()??"";
			}else if(name == nameof(IfaceGen.Template)){
				R.Template = value?.ToString()??"";
			}else if(name == nameof(IfaceGen.ParentType)){
				if(value is INamedTypeSymbol ns){
					R.ParentType = ns;
				}
			}else if(name == nameof(IfaceGen.PhIdentifierSafeFullType)){
				R.PhIdentifierSafeFullType = value?.ToString()??"";
			}else if(name == nameof(IfaceGen.OutDir)){
				R.OutDir = value?.ToString();
			}else if(name == nameof(IfaceGen.Name)){
				R.FilePrefix = value?.ToString();
			}
		}
		return R;
	}
}
