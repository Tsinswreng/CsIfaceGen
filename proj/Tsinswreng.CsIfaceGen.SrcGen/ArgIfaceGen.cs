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
}
